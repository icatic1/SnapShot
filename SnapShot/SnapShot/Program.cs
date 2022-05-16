using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SnapShot.Model;

namespace SnapShot
{
    internal static class Program
    {
        #region Attributes and Properties

        static Snapshot snapshot = new Snapshot();

        static List<int> previousContent = new List<int>()
        {
            0, 0, 0
        };

        static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>()
        {
            new FileSystemWatcher(),
            new FileSystemWatcher(),
            new FileSystemWatcher()
        };

        static List<Recorder> recorders = new List<Recorder>()
        {
            new Recorder(0),
            new Recorder(1),
            new Recorder(2)
        };

        static List<bool> cancels = new List<bool>() { false, false, false };

        static List<List<string>> buffer = new List<List<string>>()
        {
            new List<string>(),
            new List<string>(),
            new List<string>()
        };

        public static Snapshot Snapshot { get => snapshot; set => snapshot = value; }

        public static List<Recorder> Recorders { get => recorders; set => recorders = value; }

        #endregion

        #region Main

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // start threads which will change triggers when the configurations change
            RunRecordings();

            // start thread which will synchronize configuration with server
            Thread JSONSynchronization = new Thread(() => SynchronizeJSON(ref snapshot));

            JSONSynchronization.IsBackground = true;
            JSONSynchronization.Start();

            // start thread which will synchronize media with server
            Thread mediaSynchronization = new Thread(() => SynchronizeMedia(ref snapshot));

            mediaSynchronization.IsBackground = true;
            mediaSynchronization.Start();

            // start threads which will listen for livestreaming to server
            RunLivestreamListeners();

            Application.Run(new LicencingForm());
        }

        #endregion

        #region Recording

        static void RunRecordings()
        {
            Thread camera = new Thread(() => WatchTrigger(ref snapshot));
            camera.IsBackground = true;
            camera.Start();
        }

        static void WatchTrigger(ref Snapshot snapshot)
        {
            List<string> oldTriggerFilePaths = new List<string>() { "", "", "" };

            while (1 == 1)
            {

                for (int i = 0; i < snapshot.Configuration.Cameras.Count; i++)
                {
                    // configuration not set - wait a little bit, then check again
                    if (snapshot.Configuration.TriggerFilePath.Length < 1)
                        continue;

                    // configuration set - change trigger content
                    else
                    {
                        // output trigger file did not change - do not change the file we are watching
                        if (snapshot.Configuration.TriggerFilePath == oldTriggerFilePaths[i])
                            continue;

                        // output trigger file changed - change the file we are watching
                        // and save current line count
                        else
                        {
                            oldTriggerFilePaths[i] = snapshot.Configuration.TriggerFilePath;
                            try
                            {
                                previousContent[i] = File.ReadAllLines(snapshot.Configuration.TriggerFilePath).Length;
                                watchers[i].Path = Path.GetDirectoryName(snapshot.Configuration.TriggerFilePath) ?? "";
                                watchers[i].Filter = Path.GetFileName(snapshot.Configuration.TriggerFilePath);
                                watchers[i].Changed += (sender, EventArgs) =>
                                {
                                    OnChanged(sender, EventArgs, i);
                                };
                                watchers[i].EnableRaisingEvents = true;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e, int index)
        {
            // ignore anything except changes in file (create, delete, etc.)
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            // read file contents and determine what the new content is
            int noOfLines = 0;
            string[] content;
            try
            {
                content = File.ReadAllLines(e.FullPath);
                noOfLines = content.Length;
            }
            catch
            {
                return;
            }

            // new content detected in file - check whether trigger regex is present
            if (noOfLines > previousContent[index])
            {
                // get everything that was added
                string entireText = "";
                for (int i = previousContent[index]; i < noOfLines; i++)
                    entireText += content[i];

                // check whether regex matches the new content
                Regex regex = new Regex(@snapshot.Configuration.Regex);
                bool matchFound = false;

                if (regex.IsMatch(entireText))
                    matchFound = true;

                // register change for next check
                previousContent[index] = noOfLines;

                // regex match not found - end event
                if (!matchFound)
                    return;

                // new lines found, regex match found - start recording

                // take a picture
                if (snapshot.Configuration.ImageCapture)
                {
                    if (snapshot.Configuration.SingleMode)
                    {
                        // create folders with date if not already available
                        Program.recorders[index].CreateFolders();

                        Bitmap image = Program.recorders[index].TakeAPicture();

                        Program.recorders[index].SavePictureLocally(image, 0);

                        if (Program.Snapshot.Configuration.ConnectionStatus)
                            Program.recorders[index].SaveMediaRemotely(0);
                    }
                    else
                    {
                        // create folders with date if not already available
                        Program.recorders[index].CreateFolders(1);

                        List<Bitmap> images = Program.recorders[index].TakeBurstImages();

                        for (int i = 0; i <images.Count; i++)
                        {
                            Program.recorders[index].SavePictureLocally(images[i], 1, i);

                            if (Program.Snapshot.Configuration.ConnectionStatus)
                                Program.recorders[index].SaveMediaRemotely(1, i);
                        }
                    }
                }
                // take a video
                else
                {
                    // create folders with date if not already available
                    Program.recorders[index].CreateFolders();

                    Program.recorders[index].TakeAVideo();

                    if (Program.Snapshot.Configuration.ConnectionStatus)
                        Program.recorders[index].SaveMediaRemotely(2);
                }
            }
        }

        #endregion

        #region Server Synchronization

        /// <summary>
        /// Method for synchronizing JSON configuration with server
        /// Two scenarios - every X ticks and at the designated time every day
        /// </summary>
        /// <param name="snapshot"></param>
        static void SynchronizeJSON(ref Snapshot snapshot)
        {
            while (1 == 1)
            {
                try
                {
                    // check if camera is connected to the specified server
                    if (snapshot.Configuration.ConnectionStatus && snapshot.Configuration.OutputFolderPath.Length > 0)
                    {
                        // formulate the path for importing JSON configuration from server
                        string path = "http://" + snapshot.Configuration.ServerIP;
                        if (snapshot.Configuration.ServerPort != 0)
                            path += ":" + snapshot.Configuration.ServerPort;
                        string JSONPath = path + "/" + snapshot.Configuration.JSONImportLocation;

                        // no synchronization is possible before first connection to server
                        GeneralSettingsForm.FirstCheck[0] = true;

                        // two possible scenarios
                        // scenario 1 - we synchronize at the designated time
                        if (snapshot.Configuration.JSONSyncPeriod == 0 && DateTime.Now.Hour == snapshot.Configuration.JSONTime.Hours && DateTime.Now.Minute == snapshot.Configuration.JSONTime.Minutes)
                        {
                            Configuration.ImportFromJSON(JSONPath);

                            // send update signal to general settings form
                            GeneralSettingsForm.SyncStatus[0] = true;
                            GeneralSettingsForm.RefreshNeeded[0] = true;
                            GeneralSettingsForm.UpdateLabel[0] = true;

                            // denote that the synchronization has occured
                            snapshot.Configuration.JSONTicks = (int)DateTime.Now.Ticks;
                        }

                        // scenario 2 - we synchronize every X ticks
                        else if (snapshot.Configuration.JSONSyncPeriod != 0)
                        {
                            Configuration.ImportFromJSON(JSONPath);

                            // send update signal to general settings form
                            GeneralSettingsForm.SyncStatus[0] = true;
                            GeneralSettingsForm.RefreshNeeded[0] = true;
                            GeneralSettingsForm.UpdateLabel[0] = true;

                            // denote that the synchronization has occured
                            snapshot.Configuration.JSONTicks = (int)DateTime.Now.Ticks;

                            // wait for next synchronization
                            Thread.Sleep(snapshot.Configuration.JSONSyncPeriod * 1000);
                        }
                    }
                }
                catch
                {
                    // send failed signal to general settings form
                    GeneralSettingsForm.SyncStatus[0] = false;
                    GeneralSettingsForm.UpdateLabel[0] = true;
                }
            }
        }

        /// <summary>
        /// Method for synchronizing media with server
        /// Two scenarios - every X ticks and at the designated time every day
        /// </summary>
        /// <param name="snapshot"></param>
        static void SynchronizeMedia(ref Snapshot snapshot)
        {
            while (1 == 1)
            {
                try
                {
                    // check if camera is connected to the specified server
                    if (snapshot.Configuration.ConnectionStatus && snapshot.Configuration.OutputFolderPath.Length > 0)
                    {
                        // formulate the path for exporting files to server
                        string path = "http://" + snapshot.Configuration.ServerIP;
                        if (snapshot.Configuration.ServerPort != 0)
                            path += ":" + snapshot.Configuration.ServerPort;
                        string mediaPath = path + "/" + snapshot.Configuration.MediaPath;

                        // two possible scenarios
                        // scenario 1 - we synchronize at the designated time
                        if (snapshot.Configuration.MediaSyncPeriod == 0 && DateTime.Now.Hour == snapshot.Configuration.MediaTime.Hours && DateTime.Now.Minute == snapshot.Configuration.MediaTime.Minutes)
                        {
                            // get all locally created images and videos
                            string[] localEntries = Directory.GetFileSystemEntries(snapshot.Configuration.OutputFolderPath, "*", SearchOption.AllDirectories);

                            for (int i = 0; i < localEntries.Length; i++)
                                localEntries[i] = localEntries[i].Replace(snapshot.Configuration.OutputFolderPath, "").TrimStart('\\');

                            GeneralSettingsForm.FirstCheck[1] = true;

                            // get all images and videos located on the server
                            string[] serverEntries = GetEntriesFromServer(snapshot.Configuration.ServerIP, snapshot.Configuration.ServerPort.ToString(), snapshot.Configuration.MediaPath);

                            // find all local entries which are not present among server entries
                            List<string> newEntries = FindNewEntries(localEntries, serverEntries);

                            // upload every new local file to server
                            foreach (var newEntry in newEntries)
                                Configuration.UploadFile(mediaPath, newEntry, snapshot.Configuration.OutputFolderPath);

                            // send update signal to general settings form
                            GeneralSettingsForm.SyncStatus[1] = true;
                            GeneralSettingsForm.RefreshNeeded[1] = true;
                            GeneralSettingsForm.UpdateLabel[1] = true;
                        }

                        // scenario 2 - we synchronize every X ticks
                        else if (snapshot.Configuration.MediaSyncPeriod != 0)
                        {
                            // get all locally created images and videos
                            string[] localEntries = Directory.GetFileSystemEntries(snapshot.Configuration.OutputFolderPath, "*", SearchOption.AllDirectories);

                            for (int i = 0; i < localEntries.Length; i++)
                                localEntries[i] = localEntries[i].Replace(snapshot.Configuration.OutputFolderPath, "").TrimStart('\\');

                            GeneralSettingsForm.FirstCheck[1] = true;

                            // get all images and videos located on the server
                            string[] serverEntries = GetEntriesFromServer(snapshot.Configuration.ServerIP, snapshot.Configuration.ServerPort.ToString(), snapshot.Configuration.MediaPath);

                            // find all local entries which are not present among server entries
                            List<string> newEntries = FindNewEntries(localEntries, serverEntries);

                            // upload every new local file to server
                            foreach (var newEntry in newEntries)
                                Configuration.UploadFile(mediaPath, newEntry, snapshot.Configuration.OutputFolderPath);

                            // send update signal to general settings form
                            GeneralSettingsForm.SyncStatus[1] = true;
                            GeneralSettingsForm.RefreshNeeded[1] = true;
                            GeneralSettingsForm.UpdateLabel[1] = true;

                            // wait for next synchronization
                            Thread.Sleep(snapshot.Configuration.MediaSyncPeriod * 1000);
                        }
                    }
                }
                catch
                {
                    // send failed signal to general settings form
                    GeneralSettingsForm.SyncStatus[1] = false;
                    GeneralSettingsForm.UpdateLabel[1] = true;
                }
            }
        }

        /// <summary>
        /// Method for checking which files have been uploaded to the server
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="mediaPath"></param>
        /// <returns></returns>
        public static string[] GetEntriesFromServer(string ipAddress, string port, string mediaPath)
        {
            HttpWebRequest webRequest;
            string url = "http://" + ipAddress;
            if (port.Length > 0 && port != "0")
                url += ":" + port;
            url += "/" + mediaPath;
            webRequest = (HttpWebRequest)WebRequest.Create(url + "/" + Configuration.GetMACAddress());
            webRequest.Method = "GET";

            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
            string Json = rdr.ReadToEnd();
            Json = Json.Replace("[", "").Replace("]", "").Replace("\"", "");

            if (String.IsNullOrWhiteSpace(Json)) return new string[1];
            string[] files = Json.Split(",");
            for (int i = 0; i < files.Length; i++)
            {
                string MAC = Configuration.GetMACAddress() ?? "";
                int value = files[i].IndexOf(MAC);
                files[i] = files[i].Substring(value + MAC.Length);
                files[i] = files[i].TrimStart('\\').TrimEnd('\"');
                files[i] = files[i].Replace("\\\\", "\\");
            }

            return files;
        }

        /// <summary>
        /// Method for determining which local entries are not present at the server
        /// </summary>
        /// <param name="localEntries"></param>
        /// <param name="serverEntries"></param>
        /// <returns></returns>
        public static List<string> FindNewEntries(string[] localEntries, string[] serverEntries)
        {
            List<string> newEntries = new List<string>();
            foreach (var localEntry in localEntries)
            {
                // entry is a folder - skip
                if (!localEntry.Contains("."))
                    continue;

                // entry is JSON configuration - skip
                if (localEntry.Contains("configuration.json"))
                    continue;

                // entry not on server - add to list of new entries
                if (!serverEntries.Contains(localEntry))
                    newEntries.Add(localEntry);
            }

            return newEntries;
        }

        #endregion

        #region Livestreaming on Server

        /// <summary>
        /// Method for starting threads for camera recorders
        /// </summary>
        static void RunLivestreamListeners()
        {
            List<Thread> listeners = new List<Thread>()
            {
                new Thread(() => Listen(ref recorders, 0)),
                new Thread(() => Listen(ref recorders, 1)),
                new Thread(() => Listen(ref recorders, 2))
            };

            foreach (var listener in listeners)
            {
                listener.IsBackground = true;
                listener.Start();
            }
        }

        /// <summary>
        /// Method which listens for server signal that begins the recording
        /// </summary>
        /// <param name="cameras"></param>
        /// <param name="index"></param>
        static void Listen(ref List<Recorder> cameras, int index)
        {
            bool streamActive = false;

            // check for livestream necessity every 100 ms
            while (1 == 1)
            {
                if (Program.Snapshot.Configuration.ServerIP.Length < 1)
                    continue;

                try
                {
                    // send web request to check whether the server wants to stream
                    HttpWebRequest webRequest;
                    string url = "http://" + Program.Snapshot.Configuration.ServerIP;
                    if (Program.Snapshot.Configuration.ServerPort != 0)
                        url += ":" + Program.Snapshot.Configuration.ServerPort;
                    url += "/api/FileUpload/GetStreamState";
                    webRequest = (HttpWebRequest)WebRequest.Create(url + "/" + Configuration.GetMACAddress());
                    webRequest.Method = "GET";

                    // read the server response
                    WebResponse response = webRequest.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                    string Json = rdr.ReadToEnd();

                    // the server started the stream - start the thread for streaming
                    if (Json == "true" && !streamActive)
                    {
                        streamActive = true;
                        Recorder cam = cameras[index];
                        Thread snapper = new Thread(() => SendSnaps(cam, index));
                        snapper.IsBackground = true;
                        snapper.Start();
                    }

                    // the server stopped the stream or the buffer is overloaded - stop streaming
                    else if (streamActive && (Json == "false" || buffer[index].Count > 1000))
                    {
                        streamActive = false;
                        cancels[index] = true;
                        buffer[index].Clear();
                    }
                }
                catch
                {
                    // ignore any errors
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Method which initializes threads for sending livestream media to server
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="index"></param>
        static void SendSnaps(Recorder camera, int index)
        {
            // fill the first frame
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            buffer[index].Add(camera.SnapBase64(0, true));
            while (sw.ElapsedMilliseconds < 100) ;
            sw.Stop();

            // start upload and further snapping at the same time

            Thread snapper = new Thread(() => KeepSnapping(camera, index));
            snapper.IsBackground = true;
            snapper.Start();

            Thread saver = new Thread(() => KeepSaving(index));
            saver.IsBackground = true;
            saver.Start();
        }

        /// <summary>
        /// Method which takes snaps for livestreaming on server
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="index"></param>
        public static void KeepSnapping(Recorder camera, int index)
        {
            Stopwatch sw = new Stopwatch();
            while (!cancels[index])
            {
                sw.Restart();
                buffer[index].Add(camera.SnapBase64(1, true));
                while (sw.ElapsedMilliseconds < 100) ;
                sw.Stop();
            }

            buffer[index].Add(camera.SnapBase64(2, true));
            cancels[index] = false;
        }

        /// <summary>
        /// Method which sends snaps for livestreaming to server
        /// </summary>
        /// <param name="index"></param>
        public static void KeepSaving(int index)
        {
            // send request for sending bitmap to server
            string url = "http://" + Program.Snapshot.Configuration.ServerIP;
            if (Program.Snapshot.Configuration.ServerPort != 0)
                url += ":" + Program.Snapshot.Configuration.ServerPort;
            url += "/api/FileUpload/StreamBase64";

            Stopwatch sw = new Stopwatch();
            while (!cancels[index])
            {
                sw.Restart();
                // nothing to upload - we are faster than snap
                if (buffer[index].Count < 30)
                    continue;

                Configuration.UploadBase64(url, buffer[index].GetRange(0, 30));
                buffer[index].RemoveRange(0, 30);
                var x = sw.ElapsedMilliseconds;
            }

            cancels[index] = false;
            buffer[index].Clear();
        }

        #endregion
    }
}
