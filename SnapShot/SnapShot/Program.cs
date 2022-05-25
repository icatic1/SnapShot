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
using Newtonsoft.Json.Linq;

namespace SnapShot
{
    internal static class Program
    {
        #region Attributes and properties

        static Snapshot snapshot = new Snapshot();

        static string licencingURL = "https://siset1.ga";

        static int previousContent = 0;

        static FileSystemWatcher watcher = new FileSystemWatcher();

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

        static List<bool> streamsActive = new List<bool>()
        { false, false, false };

        static List<bool> stopFaceDetection = new List<bool>()
        { false, false, false };

        static List<bool> syncsActive = new List<bool>()
        { false, false };
        
        public static Snapshot Snapshot { get => snapshot; set => snapshot = value; }

        public static List<Recorder> Recorders { get => recorders; set => recorders = value; }

        public static string LicencingURL { get => licencingURL; set => licencingURL = value; }

        public static List<bool> StopFaceDetection { get => stopFaceDetection; set => stopFaceDetection = value; }

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

            // start thread which will synchronize configuration with server
            Thread listener = new Thread(() => ListenFromServer());
            listener.IsBackground = true;
            listener.Start();

            // start thread which will constantly check if faces are present
            Thread faceChecker = new Thread(() => FaceDetectionTrigger());
            faceChecker.IsBackground = true;
            faceChecker.Start();

            Application.Run(new LicencingForm());
        }

        #endregion

        #region Recording

        /// <summary>
        /// Change the trigger file fileSystemWatcher is monitoring
        /// </summary>
        /// <param name="snapshot"></param>
        public static void ChangeTrigger()
        {
            try
            {
                previousContent = File.ReadAllLines(snapshot.Configuration.TriggerFilePath).Length;
                watcher.Path = Path.GetDirectoryName(snapshot.Configuration.TriggerFilePath) ?? "";
                watcher.Filter = Path.GetFileName(snapshot.Configuration.TriggerFilePath);
                watcher.Changed += (sender, EventArgs) =>
                {
                    OnChanged(sender, EventArgs);
                };
                watcher.EnableRaisingEvents = true;
            }
            catch
            {
                // ignore any errors
            }
        }

        /// <summary>
        /// The event that is activated when new lines are inserted into the trigger file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnChanged(object sender, FileSystemEventArgs e)
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
            if (noOfLines > previousContent)
            {
                // get everything that was added
                string entireText = "";
                for (int i = previousContent; i < noOfLines; i++)
                    entireText += content[i];

                // check whether regex matches the new content
                Regex regex = new Regex(@snapshot.Configuration.Regex);
                bool matchFound = false;

                if (regex.IsMatch(entireText))
                    matchFound = true;

                // register change for next check
                previousContent = noOfLines;

                // regex match not found - end event
                if (!matchFound)
                    return;

                for (int i = 0; i < stopFaceDetection.Count; i++)
                    stopFaceDetection[i] = true;

                // new lines found, regex match found - start recording
                Record();

                for (int i = 0; i < stopFaceDetection.Count; i++)
                    stopFaceDetection[i] = false;
            }
        }

        /// <summary>
        /// Record single image, burst images or video
        /// </summary>
        public static void Record()
        { 
            // take a picture
            if (snapshot.Configuration.ImageCapture)
            {
                if (snapshot.Configuration.SingleMode)
                {
                    for (int i = 0; i < Program.recorders.Count; i++)
                    {
                        // skip cameras which have not been configured
                        if (Program.Snapshot.Configuration.Cameras[i].Id.Length < 1)
                            continue;

                        Program.recorders[i].CreateFolders();

                        Bitmap image = Program.recorders[i].TakeAPicture();

                        Program.recorders[i].SavePictureLocally(image, 0);
                    }
                }
                else
                {
                    for (int i = 0; i < Program.recorders.Count; i++)
                    {
                        // skip cameras which have not been configured
                        if (Program.Snapshot.Configuration.Cameras[i].Id.Length < 1)
                            continue;

                        Program.recorders[i].CreateFolders(1);

                        List<Bitmap> images = Program.recorders[i].TakeBurstImages();

                        for (int j = 0; j < images.Count; j++)
                            Program.recorders[i].SavePictureLocally(images[j], 1, j);
                    }
                }
            }
            // take a video
            else
            {
                for (int i = 0; i < Program.recorders.Count; i++)
                {
                    // skip cameras which have not been configured
                    if (Program.Snapshot.Configuration.Cameras[i].Id.Length < 1)
                        continue;

                    Program.recorders[i].CreateFolders();

                    Program.recorders[i].TakeAVideo();
                }
            }
        }

        #endregion

        #region Listener

        /// <summary>
        /// Listen for synchronization or livestream signals from server
        /// </summary>
        public static void ListenFromServer()
        {
            while (1 == 1)
            {
                // not connected - don't even try to contact the server
                if (snapshot.Configuration.ServerIP.Length < 1)
                    continue;

                try
                {
                    // first, check if we need to synchronize JSON
                    int typeOfSync = 0;

                    if (snapshot.Configuration.JSONSyncPeriod != 0)
                        typeOfSync = 1;
                    else if (snapshot.Configuration.JSONSyncPeriod == 0 && DateTime.Now.Hour == snapshot.Configuration.JSONTime.Hours && DateTime.Now.Minute == snapshot.Configuration.JSONTime.Minutes)
                        typeOfSync = 2;

                    if (typeOfSync > 0 && !syncsActive[0])
                    {
                        Thread JSONSynchronization = new Thread(() => SynchronizeJSON(ref snapshot, typeOfSync));
                        JSONSynchronization.IsBackground = true;
                        JSONSynchronization.Start();
                    }

                    // next, check if we need to synchronize media
                    typeOfSync = 0;
                    if (snapshot.Configuration.MediaSyncPeriod != 0)
                        typeOfSync = 1;
                    else if (snapshot.Configuration.MediaSyncPeriod == 0 && DateTime.Now.Hour == snapshot.Configuration.MediaTime.Hours && DateTime.Now.Minute == snapshot.Configuration.MediaTime.Minutes)
                        typeOfSync = 2;

                    if (typeOfSync > 0 && !syncsActive[1])
                    {
                        Thread mediaSynchronization = new Thread(() => SynchronizeMedia(ref snapshot, typeOfSync));
                        mediaSynchronization.IsBackground = true;
                        mediaSynchronization.Start();
                    }

                    // send web request to see if we need to do something for the server
                    HttpWebRequest webRequest;
                    string url = snapshot.Configuration.ServerIP;
                    if (string.IsNullOrWhiteSpace(url))
                        url = "https://siset1.ga";

                    if (snapshot.Configuration.ServerPort != 0)
                        url += ":" + snapshot.Configuration.ServerPort;
                    url += "/" + snapshot.Listener;

                    webRequest = (HttpWebRequest)WebRequest.Create(url + "/" + Configuration.GetMACAddress());
                    webRequest.Method = "GET";

                    WebResponse response = webRequest.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                    string Json = rdr.ReadToEnd();
                    dynamic result = JObject.Parse(Json);
                    List<bool> cameraLivestreams = new List<bool>()
                    { (bool)result.streaming[0].state, (bool)result.streaming[1].state, (bool)result.streaming[2].state };
                    bool mediaSync = result.filestate;

                    // media needs to be synchronized
                    if (mediaSync && !syncsActive[1])
                    {
                        Thread mediaSynchronization = new Thread(() => SynchronizeMedia(ref snapshot, 2));
                        mediaSynchronization.IsBackground = true;
                        mediaSynchronization.Start();
                    }

                    // check whether cameras need livestreaming
                    for (int i = 0; i < cameraLivestreams.Count; i++)
                    {
                        int index = i;
                        if (cameraLivestreams[i] && !streamsActive[i])
                        {
                            streamsActive[i] = true;
                            stopFaceDetection[i] = true;
                            Recorder cam = recorders[i];
                            Thread snapper = new Thread(() => SendSnaps(cam, index));
                            snapper.IsBackground = true;
                            snapper.Start();
                        }

                        // the server stopped the stream or the buffer is overloaded - stop streaming
                        else if (streamsActive[i] && (!cameraLivestreams[i] || buffer[i].Count > 1000))
                        {
                            streamsActive[i] = false;
                            stopFaceDetection[i] = false;
                            cancels[i] = true;
                            buffer[i].Add(recorders[i].SnapBase64(2));
                            buffer[i].Clear();
                        }
                    }

                }
                catch
                {
                    // ignore any errors
                }
            }
        }

        #endregion

        #region Server synchronization

        /// <summary>
        /// Method for synchronizing JSON configuration with server
        /// Two scenarios - every X ticks and at the designated time every day
        /// </summary>
        /// <param name="snapshot"></param>
        static void SynchronizeJSON(ref Snapshot snapshot, int typeOfSync)
        {
            // sync is already active - return
            if (syncsActive[0])
                return;

            try
            {
                // check if camera is connected to the specified server
                if (Configuration.ValidateToken() && snapshot.Configuration.OutputFolderPath.Length > 0)
                {
                    // sync currently being done - do not try to sync again
                    syncsActive[0] = true;

                    // formulate the path for importing JSON configuration from server
                    string path = snapshot.Configuration.ServerIP;
                    if (snapshot.Configuration.ServerPort != 0)
                        path += ":" + snapshot.Configuration.ServerPort;
                    string JSONPath = path + "/" + snapshot.JSONImport;

                    // no synchronization is possible before first connection to server
                    GeneralSettingsForm.FirstCheck[0] = true;

                    Configuration.ImportFromJSON(JSONPath);

                    // send update signal to general settings form
                    GeneralSettingsForm.SyncStatus[0] = true;
                    GeneralSettingsForm.RefreshNeeded[0] = true;
                    GeneralSettingsForm.UpdateLabel[0] = true;

                    // denote that the synchronization has occured
                    snapshot.Configuration.JSONTicks = (int)DateTime.Now.Ticks;

                     if (typeOfSync == 1)
                        // wait for next synchronization
                        Thread.Sleep(snapshot.Configuration.JSONSyncPeriod * 1000);

                    // sync finished - release lock
                    syncsActive[0] = false;
                }
            }
            catch
            {
                // send failed signal to general settings form
                GeneralSettingsForm.SyncStatus[0] = false;
                GeneralSettingsForm.UpdateLabel[0] = true;
                syncsActive[0] = false;
            }
        }

        /// <summary>
        /// Method for synchronizing media with server
        /// Two scenarios - every X ticks and at the designated time every day
        /// </summary>
        /// <param name="snapshot"></param>
        static void SynchronizeMedia(ref Snapshot snapshot, int type)
        {
            // sync is already active - return
            if (syncsActive[1])
                return;

            try
            {
                // check if camera is connected to the specified server
                if (Configuration.ValidateToken() && snapshot.Configuration.OutputFolderPath.Length > 0)
                {
                    // sync currently being done - do not try to sync again
                    syncsActive[1] = true;

                    // formulate the path for exporting files to server
                    string path = snapshot.Configuration.ServerIP;
                    if (snapshot.Configuration.ServerPort != 0)
                        path += ":" + snapshot.Configuration.ServerPort;
                    string mediaPath = path + "/" + snapshot.MediaExport;

                    // get all locally created images and videos
                    string[] localEntries = Directory.GetFileSystemEntries(snapshot.Configuration.OutputFolderPath, "*", SearchOption.AllDirectories);

                    for (int i = 0; i < localEntries.Length; i++)
                        localEntries[i] = localEntries[i].Replace(snapshot.Configuration.OutputFolderPath, "").TrimStart('\\');

                    GeneralSettingsForm.FirstCheck[1] = true;

                    // get all images and videos located on the server
                    string[] serverEntries = GetEntriesFromServer(snapshot.Configuration.ServerIP, snapshot.Configuration.ServerPort.ToString(), snapshot.MediaExport);

                    // find all local entries which are not present among server entries
                    List<string> newEntries = FindNewEntries(localEntries, serverEntries);

                    // upload every new local file to server
                    foreach (var newEntry in newEntries)
                        Configuration.UploadFile(mediaPath, newEntry, snapshot.Configuration.OutputFolderPath);

                    // send update signal to general settings form
                    GeneralSettingsForm.SyncStatus[1] = true;
                    GeneralSettingsForm.RefreshNeeded[1] = true;
                    GeneralSettingsForm.UpdateLabel[1] = true;

                    // scenario 2 - we synchronize every X ticks
                    if (type == 1)
                        // wait for next synchronization
                        Thread.Sleep(snapshot.Configuration.MediaSyncPeriod * 1000);

                    // sync finished - release lock
                    syncsActive[1] = false;
                }
            }
            catch
            {
                // send failed signal to general settings form
                GeneralSettingsForm.SyncStatus[1] = false;
                GeneralSettingsForm.UpdateLabel[1] = true;
                syncsActive[1] = false;
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
            string url = ipAddress;
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

        #region Livestreaming on server

        /// <summary>
        /// Method which initializes threads for sending livestream media to server
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="index"></param>
        static void SendSnaps(Recorder camera, int index)
        {
            camera.Reconfigure();

            // fill the first frame
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            buffer[index].Add(camera.SnapBase64(0));
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
            try
            {
            Stopwatch sw = new Stopwatch();
            while (!cancels[index])
            {
                sw.Restart();
                buffer[index].Add(camera.SnapBase64(1));
                while (sw.ElapsedMilliseconds < 100) ;
                sw.Stop();
            }

            buffer[index].Add(camera.SnapBase64(2));
            cancels[index] = false;
            }
            catch
            {
                // tell recorder to stop snapping and clean up
                cancels[index] = true;
                buffer[index].Clear();
            }
        }

        /// <summary>
        /// Method which sends snaps for livestreaming to server
        /// </summary>
        /// <param name="index"></param>
        public static void KeepSaving(int index)
        {
            try
            {
                // send request for sending bitmap to server
                string url = Program.Snapshot.Configuration.ServerIP;
                if (Program.Snapshot.Configuration.ServerPort != 0)
                    url += ":" + Program.Snapshot.Configuration.ServerPort;
                url += "/api/FileUpload/StreamBase64";

                Stopwatch sw = new Stopwatch();
                while (!cancels[index])
                {
                    sw.Restart();
                    // nothing to upload - we are faster than snap
                    if (buffer[index].Count < 15)
                        continue;

                    Configuration.UploadBase64(url, buffer[index].GetRange(0, 1));
                    buffer[index].RemoveRange(0, 15);
                    var x = sw.ElapsedMilliseconds;
                }

                // clean up
                cancels[index] = false;
                buffer[index].Clear();
            }
            catch
            {
                // tell recorder to stop snapping and clean up
                cancels[index] = true;
                buffer[index].Clear();
            }
        }

        #endregion

        #region Face detection trigger

        /// <summary>
        /// Constantly check if there are faces on the camera and start recording if it is true
        /// </summary>
        public static void FaceDetectionTrigger()
        {
            try
            {
                while (1 == 1)
                {
                    if (!snapshot.Configuration.FaceDetectionTrigger)
                        continue;

                    List<int> initializeStream = new List<int>() { 0, 0, 0 };
                    for (int i = 0; i < recorders.Count; i++)
                    {
                        // face detection stopped - do not attempt to allocate stream again
                        if (stopFaceDetection[i] && initializeStream[i] == 0)
                            continue;

                        // camera has not been setup - skip it
                        if (snapshot.Configuration.Cameras[i].Id.Length < 1)
                            continue;

                        // someone else needs to use the camera - release it
                        if (stopFaceDetection[i])
                        {
                            recorders[i].FaceDetection(2);
                            break;
                        }

                        // configure camera for opening stream
                        recorders[i].Reconfigure();

                        // check if face is present on the camera
                        bool facePresent = recorders[i].FaceDetection(initializeStream[i]);

                        // if initialization has been done, stop the stream from closing
                        if (initializeStream[i] == 0)
                            initializeStream[i]++;

                        // face is present on the camera - start recording
                        if (facePresent)
                            Record();
                    }
                }
            }
            catch
            {
                // ignore any errors
            }
        }

        #endregion
    }
}
