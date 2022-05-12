using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace SnapShot.Model
{
    public class Camera
    {
        #region Attributes

        int index;
        int typeOfSaving = 0;
        string[] dimensions = new string[2];
        OpenCvSharp.VideoCapture? capture;
        string mediaPath = "", folderName = "", timestamp = "", burstFolderName = "", filename = "";
        bool locked;
        Bitmap newestImage = new Bitmap(1, 1);

        #endregion

        #region Properties

        public int TypeOfSaving { get => typeOfSaving; set => typeOfSaving = value; }

        public bool Locked { get => locked; set => locked = value; }

        public Bitmap NewestImage { get => newestImage; set => newestImage = value; }

        #endregion

        #region Constructor

        public Camera (int i)
        {
            index = i;
        }

        #endregion

        #region Methods

        public void Reconfigure()
        {
            // determine image size
            string resolution = Program.Snapshot.Camera[index].Resolution.ToString();
            resolution = resolution.Replace("Resolution", "");
            dimensions = resolution.Split("x");

            // set video source - USB camera
            capture = new OpenCvSharp.VideoCapture(Program.Snapshot.Camera[index].CameraNumber);

            // set desired resolution
            capture.FrameHeight = Int32.Parse(dimensions[0]);
            capture.FrameWidth = Int32.Parse(dimensions[1]);

            // change contrast

            // change image color
            
            // define remote path (for uploading media)
            string path = "http://" + Program.Snapshot.Camera[index].ServerIP;
            if (Program.Snapshot.Camera[index].ServerPort != 0)
                path += ":" + Program.Snapshot.Camera[index].ServerPort;
            mediaPath = path + "/" + Program.Snapshot.Camera[index].MediaPath;
        }

        public void CreateFolders(int mode = 0)
        {
            // create folder with date if not already available
            folderName = DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000");
            Directory.CreateDirectory(Program.Snapshot.Camera[index].OutputFolderPath + "\\" + folderName);

            // create base image name
            timestamp = folderName + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");

            // for burst images, we need to create an additional folder
            if (mode == 1)
            {
                burstFolderName = "BURST-" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
                Directory.CreateDirectory(Program.Snapshot.Camera[index].OutputFolderPath + "\\" + folderName + "\\" + burstFolderName);
            }

            // for video, we need to create a different name
            filename = @Program.Snapshot.Camera[index].OutputFolderPath + "\\" + folderName + "\\VID" + timestamp + ".mp4";
            //filename = filename.Replace("\\", "/");
        }

        public Bitmap TakeAPicture()
        {
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            if (!locked)
            {
                locked = true;
                capture.Open(Program.Snapshot.Camera[index].CameraNumber);

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

                capture.Release();
                locked = false;

                return image;
            }
            else
                return newestImage;
        }

        public List<Bitmap> TakeBurstImages()
        {
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            List<Bitmap> images = new List<Bitmap>();
            Bitmap image = new Bitmap(1, 1);

            int noOfImages = (int)(Program.Snapshot.Camera[index].Duration / Program.Snapshot.Camera[index].Period);
            bool burstLocked = false;
            for (int i = 0; i < noOfImages; i++)
            {
                if (!locked)
                {
                    locked = true;
                    burstLocked = true;
                    if (i == 0)
                        capture.Open(Program.Snapshot.Camera[index].CameraNumber);

                    Mat frame = new Mat();
                    capture.Read(frame);

                    image = BitmapConverter.ToBitmap(frame);

                    // put demo watermark on image if not licenced
                    if (!Program.Snapshot.Licenced)
                        using (Graphics g = Graphics.FromImage(image))
                        {
                            Font myFont = new Font("Arial", 14);
                            g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                        }
                }
                else
                    image = newestImage;
                
                images.Add(image);

                // wait for next burst
                Thread.Sleep(Program.Snapshot.Camera[index].Period * 1000);
            }

            if (burstLocked)
            {
                locked = false;
                capture.Release();
            }

            return images;
        }

        public void TakeAVideo()
        {
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            VideoWriter videoWriter = new VideoWriter(filename, FourCC.MPG4, (int)capture.Fps, new OpenCvSharp.Size(Int32.Parse(dimensions[0]), Int32.Parse(dimensions[1])));
            Mat frame = new Mat();
            int frames = 0;
            bool videoLocked = false;
            while (frames < Program.Snapshot.Camera[index].Duration * capture.Fps)
            {
                if (!locked && frames == 0)
                {
                    locked = true;
                    videoLocked = true;
                    capture.Open(Program.Snapshot.Camera[index].CameraNumber);
                    capture.Read(frame);
                }
                else if (videoLocked)
                    capture.Read(frame);
                else
                    frame = newestImage.ToMat();

                //put demo watermark to frame if not licenced
                if (!Program.Snapshot.Licenced)
                    frame.PutText("Demo version", new OpenCvSharp.Point(20, 20), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));

                videoWriter.Write(frame);
                frames++;
            }

            videoWriter.Release();

            if (videoLocked)
            {
                locked = false;
                capture.Release();
            }
        }

        public Bitmap Snap(int type, bool overrided)
        {
            if ((locked && !overrided) || capture == null)
                throw new Exception("Cannot allocate camera!");

            if (type == 0)
            {
                capture.Open(Program.Snapshot.Camera[index].CameraNumber);
                locked = true;
            }

            Mat frame = new Mat();
            capture.Read(frame);

            if (type == 2)
            {
                locked = false;
                capture.Release();
            }

            return BitmapConverter.ToBitmap(frame);
        }

        public string SnapBase64(int type, bool overrided)
        {
            if ((locked && !overrided) || capture == null)
                throw new Exception("Cannot allocate camera!");

            if (type == 0)
            {
                capture.Open(Program.Snapshot.Camera[index].CameraNumber);
                locked = true;
            }

            Mat frame = new Mat();
            capture.Read(frame);

            if (type == 2)
            {
                locked = false;
                capture.Release();
            }

            return Convert.ToBase64String(frame.ToBytes());
        }

        public void SavePictureLocally(Bitmap image, int type, int number = 0)
        {
            if (type == 0)
                image.Save(@Program.Snapshot.Camera[index].OutputFolderPath + "\\" + folderName + "\\IMG" + timestamp + ".png");
            else if (type == 1)
                image.Save(@Program.Snapshot.Camera[index].OutputFolderPath + "\\" + folderName + "\\" + burstFolderName + "\\IMG" + (number + 1) + ".png");
            else
            {
                if (File.Exists("snap" + number + ".png"))
                    File.Delete("snap" + number + ".png");
                image.Save("snap" + number + ".png");
            }
        }

        public void SaveMediaRemotely(int type, int number = 0)
        {
            if (type == 0)
                Configuration.UploadFile(mediaPath, folderName + "\\IMG" + timestamp + ".png", Program.Snapshot.Camera[index].OutputFolderPath);
            else if (type == 1)
                Configuration.UploadFile(mediaPath, folderName + "\\" + burstFolderName + "\\IMG" + (number + 1) + ".png", Program.Snapshot.Camera[index].OutputFolderPath);
            else
                Configuration.UploadFile(mediaPath, folderName + "\\VID" + timestamp + ".mp4", Program.Snapshot.Camera[index].OutputFolderPath);
        }

        #endregion
    }
}
