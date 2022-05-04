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

namespace SnapShot
{
    internal static class Program
    {
        #region Properties

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

        public static Snapshot Snapshot { get => snapshot; set => snapshot = value; }

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

            Application.Run(new LicencingForm());
        }

        #endregion

        #region Recording

        static void RunRecordings()
        {
            List<Thread> cameras = new List<Thread>()
            {
                new Thread(() => WatchTrigger(ref snapshot, 0)),
                new Thread(() => WatchTrigger(ref snapshot, 1)),
                new Thread(() => WatchTrigger(ref snapshot, 2))
            };

            foreach (var camera in cameras)
            {
                camera.IsBackground = true;
                camera.Start();
            }
        }

        static void WatchTrigger(ref Snapshot snapshot, int index)
        {
            string oldTriggerFilePath = "";

            while (1 == 1)
            {
                // configuration not set - wait a little bit, then check again
                if (snapshot.Camera[index].TriggerFilePath.Length < 1)
                    continue;

                // configuration set - change trigger content
                else
                {
                    // output trigger file did not change - do not change the file we are watching
                    if (snapshot.Camera[index].TriggerFilePath == oldTriggerFilePath)
                        continue;

                    // output trigger file changed - change the file we are watching
                    // and save current line count
                    else
                    {
                        oldTriggerFilePath = snapshot.Camera[index].TriggerFilePath;
                        try
                        {
                            previousContent[index] = File.ReadAllLines(snapshot.Camera[index].TriggerFilePath).Length;
                            watchers[index].Path = Path.GetDirectoryName(snapshot.Camera[index].TriggerFilePath) ?? "";
                            watchers[index].Filter = Path.GetFileName(snapshot.Camera[index].TriggerFilePath);
                            watchers[index].Changed += (sender, EventArgs) =>
                            {
                                OnChanged(sender, EventArgs, index);
                            };
                            watchers[index].EnableRaisingEvents = true;
                        }
                        catch
                        {
                            continue;
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

                // change resolution
                string resolution = snapshot.Camera[index].Resolution.ToString();
                resolution = resolution.Replace("Resolution", "");
                var dimensions = resolution.Split("x");

                // set video source - USB camera
                OpenCvSharp.VideoCapture capture = new OpenCvSharp.VideoCapture(snapshot.Camera[index].CameraNumber);

                // set desired resolution
                capture.FrameHeight = Int32.Parse(dimensions[0]);
                capture.FrameWidth = Int32.Parse(dimensions[1]);

                // change contrast

                // change image color

                // create folder with date if not already available
                string folderName = DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000");
                Directory.CreateDirectory(snapshot.Camera[index].OutputFolderPath + "/" + folderName);

                // create base image name
                string timestamp = folderName + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");

                capture.Open(snapshot.Camera[index].CameraNumber);

                string path = "http://" + snapshot.Camera[index].ServerIP;
                if (snapshot.Camera[index].ServerPort != 0)
                    path += ":" + snapshot.Camera[index].ServerPort;
                string mediaPath = path + "/" + snapshot.Camera[index].MediaPath;

                // take a picture
                if (snapshot.Camera[index].ImageCapture)
                {
                    if (snapshot.Camera[index].SingleMode)
                    {
                        #region Single image

                        OpenCvSharp.Mat frame = new OpenCvSharp.Mat();
                        capture.Read(frame);

                        Bitmap image = BitmapConverter.ToBitmap(frame);

                        // put demo watermark on image if not licenced
                        if (!Program.Snapshot.Licenced)
                            using (Graphics g = Graphics.FromImage(image))
                            {
                                Font myFont = new Font("Arial", 14);
                                g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                            }

                        // save image locally
                        image.Save(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/IMG" + timestamp + ".png");
                        
                        // save image to server
                        if (snapshot.Camera[index].ConnectionStatus)
                            Configuration.UploadFile(mediaPath, folderName + "/IMG" + timestamp + ".png", snapshot.Camera[index].OutputFolderPath);

                        capture.Release();

                        #endregion
                    }
                    else
                    {
                        #region Burst images

                        // create burst directory
                        string burstFolderName = "BURST-" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
                        Directory.CreateDirectory(snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/" + burstFolderName);

                        int noOfImages = (int)(snapshot.Camera[index].Duration / snapshot.Camera[index].Period);
                        for (int i = 0; i < noOfImages; i++)
                        {
                            OpenCvSharp.Mat frame = new OpenCvSharp.Mat();
                            capture.Read(frame);

                            Bitmap image = BitmapConverter.ToBitmap(frame);

                            // put demo watermark on image if not licenced
                            if (!Program.Snapshot.Licenced)
                                using (Graphics g = Graphics.FromImage(image))
                                {
                                    Font myFont = new Font("Arial", 14);
                                    g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                                }

                            // save image locally
                            image.Save(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/" + burstFolderName + "/IMG" + (i + 1) + ".png");

                            // save image to server
                            if (snapshot.Camera[index].ConnectionStatus)
                                Configuration.UploadFile(mediaPath, folderName + "/" + burstFolderName + "/IMG" + (i + 1) + ".png", snapshot.Camera[index].OutputFolderPath);

                            // wait for next burst
                            Thread.Sleep(snapshot.Camera[index].Period * 1000);
                        }

                        capture.Release();

                        #endregion
                    }
                }
                // take a video
                else
                {
                    #region Video

                    string filename = @snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/VID" + timestamp + ".mp4";
                    filename = filename.Replace("\\", "/");
                    int width = Int32.Parse(dimensions[0]), height = Int32.Parse(dimensions[1]), fps = (int)capture.Fps;
                    VideoWriter videoWriter = new VideoWriter(filename, FourCC.MPG4, fps, new OpenCvSharp.Size(width, height)); 
                    Mat frame = new Mat();
                    int frames = 0;
                    while (frames < snapshot.Camera[index].Duration * capture.Fps)
                    {
                        capture.Read(frame);

                        //put demo watermark to frame if not licenced
                        if (!Program.Snapshot.Licenced)
                            frame.PutText("Demo version", new OpenCvSharp.Point(20, 20), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));

                        videoWriter.Write(frame);
                        frames++;
                    }

                    videoWriter.Release();
                    capture.Release();

                    // save video to server
                    if (snapshot.Camera[index].ConnectionStatus)
                        Configuration.UploadFile(mediaPath, folderName + "/VID" + timestamp + ".mp4", snapshot.Camera[index].OutputFolderPath);

                    #endregion
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

                        string path = "http://" + snapshot.Camera[index].ServerIP;
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
    }
}
