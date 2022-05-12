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

        static List<Camera> recorders = new List<Camera>()
        {
            new Camera(0),
            new Camera(1),
            new Camera(2)
        };

        static List<bool> cancels = new List<bool>() { false, false, false };

        static List<List<string>> buffer = new List<List<string>>()
        {
            new List<string>(),
            new List<string>(),
            new List<string>()
        };

        public static Snapshot Snapshot { get => snapshot; set => snapshot = value; }

        public static List<Camera> Recorders { get => recorders; set => recorders = value; }

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

            // start threads which will synchronize configuration with server
            RunServerSynchronizations();

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

                for (int i = 0; i < snapshot.Camera.Count; i++)
                {
                    // configuration not set - wait a little bit, then check again
                    if (snapshot.Camera[i].TriggerFilePath.Length < 1)
                        continue;

                    // configuration set - change trigger content
                    else
                    {
                        // output trigger file did not change - do not change the file we are watching
                        if (snapshot.Camera[i].TriggerFilePath == oldTriggerFilePaths[i])
                            continue;

                        // output trigger file changed - change the file we are watching
                        // and save current line count
                        else
                        {
                            oldTriggerFilePaths[i] = snapshot.Camera[i].TriggerFilePath;
                            try
                            {
                                previousContent[i] = File.ReadAllLines(snapshot.Camera[i].TriggerFilePath).Length;
                                watchers[i].Path = Path.GetDirectoryName(snapshot.Camera[i].TriggerFilePath) ?? "";
                                watchers[i].Filter = Path.GetFileName(snapshot.Camera[i].TriggerFilePath);
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
                Regex regex = new Regex(@snapshot.Camera[index].Regex);
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
                if (snapshot.Camera[index].ImageCapture)
                {
                    if (snapshot.Camera[index].SingleMode)
                    {
                        // create folders with date if not already available
                        Program.recorders[index].CreateFolders();

                        Bitmap image = Program.recorders[index].TakeAPicture();

                        Program.recorders[index].SavePictureLocally(image, 0);

                        if (Program.Snapshot.Camera[index].ConnectionStatus)
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

                            if (Program.Snapshot.Camera[index].ConnectionStatus)
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

                    if (Program.Snapshot.Camera[index].ConnectionStatus)
                        Program.recorders[index].SaveMediaRemotely(2);
                }
            }
        }

        #endregion

        #region Server Synchronization

        static void RunServerSynchronizations()
        {
            List<Thread> cameras = new List<Thread>()
            {
                new Thread(() => Synchronize(ref snapshot, 0)),
                new Thread(() => Synchronize(ref snapshot, 1)),
                new Thread(() => Synchronize(ref snapshot, 2))
            };

            foreach (var camera in cameras)
            {
                camera.IsBackground = true;
                camera.Start();
            }
        }

        static void Synchronize(ref Snapshot snapshot, int index)
        {
            while (1 == 1)
            {
                try
                {
                    // camera is connected to the specified server
                    if (snapshot.Camera[index].ConnectionStatus && snapshot.Camera[index].OutputFolderPath.Length > 0)
                    {
                        // get all locally created images and videos
                        string[] localEntries = Directory.GetFileSystemEntries(snapshot.Camera[index].OutputFolderPath, "*", SearchOption.AllDirectories);

                        for (int i = 0; i < localEntries.Length; i++)
                            localEntries[i] = localEntries[i].Replace(snapshot.Camera[index].OutputFolderPath, "").TrimStart('\\');

                        ConfigurationForm.FirstCheck = true;

                        // get all images and videos located on the server
                        string[] serverEntries = GetEntriesFromServer(snapshot.Camera[index].ServerIP, snapshot.Camera[index].ServerPort.ToString(), snapshot.Camera[index].MediaPath);

                        // find all local entries which are not present among server entries
                        List<string> newEntries = FindNewEntries(localEntries, serverEntries);

                        // upload every new local file to server

                        string path = "https://" + snapshot.Camera[index].ServerIP;
                        if (snapshot.Camera[index].ServerPort != 0)
                            path += ":" + snapshot.Camera[index].ServerPort;
                        string mediaPath = path + "/" + snapshot.Camera[index].MediaPath;
                        string JSONPath = path + "/" + snapshot.Camera[index].JSONConfigPath;

                        foreach (var newEntry in newEntries)
                            Configuration.UploadFile(mediaPath, newEntry, snapshot.Camera[index].OutputFolderPath);

                        // synchronize JSON configuration with server
                        Configuration.ImportFromJSON(JSONPath);

                        ConfigurationForm.SyncStatus = true;
                        ConfigurationForm.RefreshNeeded = true;
                        ConfigurationForm.UpdateLabel = true;

                        // wait for next synchronization
                        snapshot.Camera[index].LatestSynchronizationTicks = (int)DateTime.Now.Ticks;
                    }
                }
                catch
                {
                    ConfigurationForm.SyncStatus = false;
                    ConfigurationForm.UpdateLabel = true;
                }

                // wait for next synchronization
                Thread.Sleep(snapshot.Camera[index].SynchronizationPeriod * 1000);
            }
        }

        static string[] GetEntriesFromServer(string ipAddress, string port, string mediaPath)
        {
            HttpWebRequest webRequest;
            string url = "https://" + ipAddress;
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

        static List<string> FindNewEntries(string[] localEntries, string[] serverEntries)
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

        static void Listen(ref List<Camera> cameras, int index)
        {
            bool streamActive = false;

            // check for livestream necessity every 100 ms
            while (1 == 1)
            {
                if (Program.Snapshot.Camera[index].ServerIP.Length < 1)
                    continue;

                HttpWebRequest webRequest;
                string url = "http://" + Program.Snapshot.Camera[index].ServerIP;
                if (Program.Snapshot.Camera[index].ServerPort != 0)
                    url += ":" + Program.Snapshot.Camera[index].ServerPort;
                url += "/api/FileUpload/GetStreamState";
               
                    webRequest = (HttpWebRequest)WebRequest.Create(url + "/" + Configuration.GetMACAddress());
                     webRequest.Method = "GET";

                     WebResponse response = webRequest.GetResponse();
                     Stream responseStream = response.GetResponseStream();
                     StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                     string Json = rdr.ReadToEnd();

                    if (Json == "true" && !streamActive)
                    {
                        streamActive = true;
                        Camera cam = cameras[index];
                        Thread snapper = new Thread(() => SendSnaps(cam, index));
                        snapper.IsBackground = true;
                        snapper.Start();
                    }
                    else if (Json == "false" && streamActive)
                    {
                        streamActive = false;
                        cancels[index] = true;
                    }

                
                
                Thread.Sleep(100);
            }
        }

        static void SendSnaps(Camera camera, int index)
        {
            // fill the buffer first - delay of 2 seconds
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            buffer[index].Add(camera.SnapBase64(0, true));
            while (sw.ElapsedMilliseconds < 33) ;
            sw.Stop();
            for (int i = 0; i < 30 * 2 - 1; i++)
            {
                sw.Restart();
                buffer[index].Add(camera.SnapBase64(1, true));
                while (sw.ElapsedMilliseconds < 33) ;
                sw.Stop();
            }

            // start upload and further snapping at the same time

            Thread snapper = new Thread(() => KeepSnapping(camera, index));
            snapper.IsBackground = true;
            snapper.Start();

            Thread saver = new Thread(() => KeepSaving(index));
            saver.IsBackground = true;
            saver.Start();
        }

        public static void KeepSnapping(Camera camera, int index)
        {
            Stopwatch sw = new Stopwatch();
            while (!cancels[index])
            {
                sw.Restart();
                buffer[index].Add(camera.SnapBase64(1, true));
                while (sw.ElapsedMilliseconds < 33) ;
                sw.Stop();
            }

            buffer[index].Add(camera.SnapBase64(2, true));
            cancels[index] = false;
        }

        public static void KeepSaving(int index)
        {
            // send request for sending bitmap to server
            string url = "http://" + Program.Snapshot.Camera[index].ServerIP;
            if (Program.Snapshot.Camera[index].ServerPort != 0)
                url += ":" + Program.Snapshot.Camera[index].ServerPort;
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
