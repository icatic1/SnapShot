using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace SnapShot.Model
{
    public class Recorder
    {
        #region Attributes

        int index;
        int typeOfSaving = 0;
        string[] dimensions = new string[2];
        VideoCapture? capture;
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

        public Recorder (int i)
        {
            index = i;
        }

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Change recorder settings when the configuration settings are changed
        /// </summary>
        public void Reconfigure()
        {
            // determine image size
            string resolution = Program.Snapshot.Configuration.Cameras[index].Resolution.ToString();
            resolution = resolution.Replace("Resolution", "");
            dimensions = resolution.Split("x");

            // set video source - USB camera
            capture = new VideoCapture(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

            // set desired resolution
            capture.FrameHeight = Int32.Parse(dimensions[0]);
            capture.FrameWidth = Int32.Parse(dimensions[1]);

            // change contrast

            // change image color
            
            // define remote path (for uploading media)
            string path = "http://" + Program.Snapshot.Configuration.ServerIP;
            if (Program.Snapshot.Configuration.ServerPort != 0)
                path += ":" + Program.Snapshot.Configuration.ServerPort;
            mediaPath = path + "/" + Program.Snapshot.Configuration.MediaPath;
        }

        /// <summary>
        /// Determine folder and media content names and create the folders if they do not exist
        /// </summary>
        /// <param name="mode"></param>
        public void CreateFolders(int mode = 0)
        {
            // create folder with date if not already available
            folderName = DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000");
            Directory.CreateDirectory(Program.Snapshot.Configuration.OutputFolderPath + "\\" + folderName);

            // create base image name
            timestamp = folderName + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");

            // for burst images, we need to create an additional folder
            if (mode == 1)
            {
                burstFolderName = "BURST-" + DateTime.Now.Day.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
                Directory.CreateDirectory(Program.Snapshot.Configuration.OutputFolderPath + "\\" + folderName + "\\" + burstFolderName);
            }

            // for video, we need to create a different name
            filename = @Program.Snapshot.Configuration.OutputFolderPath + "\\" + folderName + "\\VID" + timestamp + ".mp4";
        }

        #endregion

        #region Recording

        /// <summary>
        /// Take a single picture and return it as the result
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Bitmap TakeAPicture()
        {
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            if (!locked)
            {
                // lock the recorder so nobody else can use it
                locked = true;

                // open the camera for recording
                capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

                // take a snapshot
                Mat frame = new Mat();
                capture.Read(frame);

                // convert the snapshot to an image
                Bitmap image = BitmapConverter.ToBitmap(frame);

                // put demo watermark on image if not licenced
                if (!Program.Snapshot.Licenced)
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        Font myFont = new Font("Arial", 14);
                        g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                    }

                // close the camera
                capture.Release();

                // unlock the recorder so others can use it
                locked = false;

                return image;
            }
            // if the recorder is locked, instead of taking a new picture just return the latest snapshot
            else
                return newestImage;
        }

        /// <summary>
        /// Take multiple burst images and return them as the result
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Bitmap> TakeBurstImages()
        {
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            List<Bitmap> images = new List<Bitmap>();
            Bitmap image = new Bitmap(1, 1);

            // determine how many images need to be taken
            int noOfImages = (int)(Program.Snapshot.Configuration.Duration / Program.Snapshot.Configuration.Period);
            bool burstLocked = false;
            
            // snap the necessary images
            for (int i = 0; i < noOfImages; i++)
            {
                if (!locked)
                {
                    // lock the recorder so nobody else can use it
                    locked = true;
                    burstLocked = true;

                    // open the camera only the first time
                    if (i == 0)
                        capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

                    // take a snapshot
                    Mat frame = new Mat();
                    capture.Read(frame);

                    // convert the snapshot to an image
                    image = BitmapConverter.ToBitmap(frame);

                    // put demo watermark on image if not licenced
                    if (!Program.Snapshot.Licenced)
                        using (Graphics g = Graphics.FromImage(image))
                        {
                            Font myFont = new Font("Arial", 14);
                            g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                        }
                }
                // if the recorder is locked, instead of taking a new picture just return the latest snapshot
                else
                    image = newestImage;
                
                images.Add(image);

                // wait for next burst
                Thread.Sleep(Program.Snapshot.Configuration.Period * 1000);
            }

            // unlock the recorder so others can use it
            if (burstLocked)
            {
                locked = false;
                capture.Release();
            }

            return images;
        }

        /// <summary>
        /// Take a video and save it locally
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void TakeAVideo()
        {
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            // open the camera for video recording
            VideoWriter videoWriter = new VideoWriter(filename, FourCC.MPG4, (int)capture.Fps, new OpenCvSharp.Size(Int32.Parse(dimensions[0]), Int32.Parse(dimensions[1])));
            Mat frame = new Mat();
            int frames = 0;
            bool videoLocked = false;

            // calculate the necessary number of frames by using the FPS of the camera
            while (frames < Program.Snapshot.Configuration.Duration * capture.Fps)
            {
                // setup the camera when taking the first frame
                if (!locked && frames == 0)
                {
                    // lock the camera so nobody else can use it
                    locked = true;
                    videoLocked = true;

                    // open the camera for recording
                    capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);
                    
                    // snap the first frame
                    capture.Read(frame);
                }

                // just read the next frame
                else if (videoLocked)
                    capture.Read(frame);

                // capture is locked - use latest snap
                else
                    frame = newestImage.ToMat();

                // put demo watermark to frame if not licenced
                if (!Program.Snapshot.Licenced)
                    frame.PutText("Demo version", new OpenCvSharp.Point(20, 20), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));

                // locally save the video
                videoWriter.Write(frame);
                frames++;
            }

            // close the local file writer
            videoWriter.Release();

            // unlock the recorder so others can use it
            if (videoLocked)
            {
                locked = false;
                capture.Release();
            }
        }

        /// <summary>
        /// Take a single picture, but optimized for fast usage by exploiting the capture already being opened
        /// </summary>
        /// <param name="type"></param>
        /// <param name="overrided"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Bitmap Snap(int type, bool overrided)
        {
            if ((locked && !overrided) || capture == null)
                throw new Exception("Cannot allocate camera!");

            // we are opening the capture for the first time
            if (type == 0)
            {
                capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);
                locked = true;
            }

            // snap a new picture
            Mat frame = new Mat();
            capture.Read(frame);

            // we are closing the capture - release the recorder so that others can use it
            if (type == 2)
            {
                locked = false;
                capture.Release();
            }

            // convert the snapshot to an image
            return BitmapConverter.ToBitmap(frame);
        }

        /// <summary>
        /// Take a single picture, but as the lightweight Base64String property - optimized for sending to server
        /// </summary>
        /// <param name="type"></param>
        /// <param name="overrided"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string SnapBase64(int type, bool overrided)
        {
            if ((locked && !overrided) || capture == null)
                throw new Exception("Cannot allocate camera!");

            // we are opening the capture for the first time
            if (type == 0)
            {
                capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);
                locked = true;
            }

            // snap a new picture
            Mat frame = new Mat();
            capture.Read(frame);

            // we are closing the capture - release the recorder so that others can use it
            if (type == 2)
            {
                locked = false;
                capture.Release();
            }

            // convert the snapshot to its Base64String representation
            return Convert.ToBase64String(frame.ToBytes());
        }

        #endregion

        #region Saving output

        /// <summary>
        /// Save picture to the local path
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        /// <param name="number"></param>
        public void SavePictureLocally(Bitmap image, int type, int number = 0)
        {
            // save single image
            if (type == 0)
                image.Save(@Program.Snapshot.Configuration.OutputFolderPath + "\\" + folderName + "\\IMG" + timestamp + ".png");
            
            // save burst image
            else if (type == 1)
                image.Save(@Program.Snapshot.Configuration.OutputFolderPath + "\\" + folderName + "\\" + burstFolderName + "\\IMG" + (number + 1) + ".png");
            
            // save livestream temporary image
            else
            {
                if (File.Exists("snap" + number + ".png"))
                    File.Delete("snap" + number + ".png");
                image.Save("snap" + number + ".png");
            }
        }

        /// <summary>
        /// Save picture to the remote server
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        public void SaveMediaRemotely(int type, int number = 0)
        {
            // single image
            if (type == 0)
                Configuration.UploadFile(mediaPath, folderName + "\\IMG" + timestamp + ".png", Program.Snapshot.Configuration.OutputFolderPath);
            
            // burst image
            else if (type == 1)
                Configuration.UploadFile(mediaPath, folderName + "\\" + burstFolderName + "\\IMG" + (number + 1) + ".png", Program.Snapshot.Configuration.OutputFolderPath);
            
            // video
            else
                Configuration.UploadFile(mediaPath, folderName + "\\VID" + timestamp + ".mp4", Program.Snapshot.Configuration.OutputFolderPath);
        }

        #endregion

        #endregion
    }
}
