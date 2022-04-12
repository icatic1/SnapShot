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

            RunRecordings();

            Application.Run(new LicencingForm());
        }

        #endregion

        #region Recording

        static void RunRecordings()
        {
            List<Thread> cameras = new List<Thread>()
            {
                new Thread(() => Record(ref snapshot, 0)),
                new Thread(() => Record(ref snapshot, 1)),
                new Thread(() => Record(ref snapshot, 2))
            };

            foreach (var camera in cameras)
            {
                camera.IsBackground = true;
                camera.Start();
            }
        }

        static void Record(ref Snapshot snapshot, int index)
        {
            int initialLines = 0;
            bool firstCheck = false;
            
            while (1 == 1)
            {
                // configuration not set - wait a little bit, then check again
                if (snapshot.Camera[index].OutputFolderPath.Length < 1)
                    Thread.Sleep(1000);

                // ignore any old content of trigger file, then check again
                else if (!firstCheck)
                {
                    firstCheck = true;
                    initialLines = File.ReadAllLines(snapshot.Camera[index].TriggerFilePath).Length;
                }

                // old content already ignored - check whether trigger has been activated
                else
                {
                    try
                    {
                        string[] entireText = File.ReadAllLines(snapshot.Camera[index].TriggerFilePath);
                        int lines = entireText.Length;

                        // new lines detected in file - check whether trigger regex is present
                        if (lines > initialLines)
                        {
                            // check whether regex matches the new content
                            Regex regex = new Regex(@snapshot.Camera[index].Regex);
                            bool matchFound = false;

                            for (int i = initialLines; i < lines; i++)
                                if (regex.IsMatch(entireText[i]))
                                {
                                    matchFound = true;
                                    break;
                                }

                            // register change for next check
                            initialLines = lines;

                            // regex match not found - wait for a while, then check again for file lines change
                            if (!matchFound)
                            {
                                Thread.Sleep(1000);
                                continue;
                            }
                        }

                        // wait for a while, then check again for file lines change
                        else
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        // new lines found, regex match found - start recording

                        // change resolution
                        string resolution = snapshot.Camera[index].Resolution.ToString();
                        resolution = resolution.Replace("Resolution", "");
                        var dimensions = resolution.Split("x");
                        VideoCapture capture = new VideoCapture(snapshot.Camera[index].CameraNumber);
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
                            // take a single picture
                            if (snapshot.Camera[index].SingleMode)
                            {
                                Mat frame = new Mat();
                                capture.Read(frame);

                                Bitmap image = BitmapConverter.ToBitmap(frame);
                                image.Save(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/IMG" + timestamp + ".png");

                                capture.Release();
                            }
                            // take burst images
                            else
                            {
                                int noOfImages = (int)(snapshot.Camera[index].Duration / snapshot.Camera[index].Period);
                                for (int i = 0; i < noOfImages; i++)
                                {
                                    Mat frame = new Mat();
                                    capture.Read(frame);

                                    Bitmap image = BitmapConverter.ToBitmap(frame);
                                    image.Save(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/IMG" + timestamp + "-BURST" + (i + 1) + ".png");
                                        
                                    // wait for next burst
                                    Thread.Sleep(snapshot.Camera[index].Period * 1000);
                                }

                                capture.Release();
                            }
                        }
                        // record a video
                        else
                        {
                            using (VideoWriter writer = new VideoWriter(@snapshot.Camera[index].OutputFolderPath + "/" + folderName + "/VID" + timestamp + ".mp4", FourCC.MPG4, capture.Fps, new OpenCvSharp.Size(Int32.Parse(dimensions[0]), Int32.Parse(dimensions[1]))))
                            {
                                Mat frame = new Mat();
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                while (sw.ElapsedMilliseconds < snapshot.Camera[index].Duration * 1000 + 1000)
                                {
                                    capture.Read(frame);
                                    writer.Write(frame);
                                }
                            }

                            capture.Release();
                        }
                    }
                    // file unavailable - probably being edited, wait 5 seconds then check again
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        #endregion
    }
}
