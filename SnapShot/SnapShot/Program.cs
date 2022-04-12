using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot
{
    internal static class Program
    {
        static Snapshot snapshot = new Snapshot();
        public static Snapshot Snapshot { get => snapshot; set => snapshot = value; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LicencingForm());
        }
        static void Record(ref Snapshot snapshot, int index)
        {
            while (1 == 1)
            {
                // configuration not set
                if (snapshot.Camera[index].OutputFolderPath.Length < 1)
                    Thread.Sleep(100);
                // configuration set - need to check whether trigger has been activated
                else
                {
                    try
                    {
                        string text = File.ReadAllText(snapshot.Camera[index].TriggerFilePath);
                        // start recording
                        if (text == "RECORD")
                        {
                            // change resolution
                            string resolution = snapshot.Camera[index].Resolution.ToString();
                            resolution = resolution.Replace("Resolution", "");
                            var dimensions = resolution.Split("x");
                            VideoCapture capture = new VideoCapture(snapshot.Camera[index].CameraNumber);
                            capture.FrameHeight = Int32.Parse(dimensions[0]);
                            capture.FrameWidth = Int32.Parse(dimensions[1]);

                            // change contrast


                            // change image color


                            // create base image name
                            string timestamp = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

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
                                    image.Save(@snapshot.Camera[index].OutputFolderPath + "/image" + timestamp + ".png");

                                    capture.Release();
                                }

                                // sleep for one minute so that the file contents can change
                                Thread.Sleep(60 * 1000);
                            }
                            else
                                Thread.Sleep(100);
                        }
                    }
                    // file unavailable - probably being edited, wait 5 seconds then check again
                    catch
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
        }
    }
}
