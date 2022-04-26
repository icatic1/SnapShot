using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                VideoCapture capture = new VideoCapture(snapshot.Camera[index].CameraNumber);

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

                // take a picture
                if (snapshot.Camera[index].ImageCapture)
                {
                    if (snapshot.Camera[index].SingleMode)
                    {
                        #region Single image

                        Mat frame = new Mat();
                        capture.Read(frame);

                        Bitmap image = BitmapConverter.ToBitmap(frame);

                        // put demo watermark on image if not licenced
                        if (!Program.Snapshot.Licenced)
                            using (Graphics g = Graphics.FromImage(image))
                            {
                                Font myFont = new Font("Arial", 14);
                                g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                            }

                        image.Save(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/IMG" + timestamp + ".png");

                        capture.Release();

                        #endregion
                    }
                    else
                    {
                        #region Burst images

                        // create burst directory
                        string burstFolderName = "BURST-" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000") + "-" +DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
                        Directory.CreateDirectory(snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/" + burstFolderName);

                        int noOfImages = (int)(snapshot.Camera[index].Duration / snapshot.Camera[index].Period);
                        for (int i = 0; i < noOfImages; i++)
                        {
                            Mat frame = new Mat();
                            capture.Read(frame);

                            Bitmap image = BitmapConverter.ToBitmap(frame);

                            // put demo watermark on image if not licenced
                            if (!Program.Snapshot.Licenced)
                                using (Graphics g = Graphics.FromImage(image))
                                {
                                    Font myFont = new Font("Arial", 14);
                                    g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                                }

                            image.Save(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/" + burstFolderName + "/IMG" + (i + 1) + ".png");

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

                    using (VideoWriter writer = new VideoWriter(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/VID" + timestamp + ".mp4", FourCC.MPG4, capture.Fps, new OpenCvSharp.Size(Int32.Parse(dimensions[0]), Int32.Parse(dimensions[1]))))
                    {
                        Mat frame = new Mat();
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        while (sw.ElapsedMilliseconds < snapshot.Camera[index].Duration * 1000 + 1000)
                        {
                            capture.Read(frame);

                            //put demo watermark to frame if not licenced
                            if (!Program.Snapshot.Licenced)
                                frame.PutText("Demo version", new OpenCvSharp.Point(20, 20), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));

                            writer.Write(frame);
                        }
                    }

                    capture.Release();

                    #endregion
                }
            }
        }

        #endregion

        #region Server Configuration

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
                // synchronize data with server


                // wait for next synchronization
                Thread.Sleep(snapshot.Camera[index].SynchronizationPeriod * 1000);
            }
        }


        #endregion
    }
}
