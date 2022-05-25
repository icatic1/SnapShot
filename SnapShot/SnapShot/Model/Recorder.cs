using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

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

        public VideoCapture? Capture { get => capture; set => capture = value; }
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
            // wait until camera is unlocked
            while (locked) ;

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
            string path = Program.Snapshot.Configuration.ServerIP;
            if (Program.Snapshot.Configuration.ServerPort != 0)
                path += ":" + Program.Snapshot.Configuration.ServerPort;
            mediaPath = path + "/" + Program.Snapshot.MediaExport;
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

            // wait until camera is unlocked
            while (locked) ;

            // lock the recorder so nobody else can use it
            locked = true;

            // open the camera for recording
            capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

            // take a snapshot
            Mat frame = new Mat();
            capture.Read(frame);

            // close the camera
            capture.Release();

            // unlock the recorder so others can use it
            locked = false;

            // convert the snapshot to an image
            Bitmap image = BitmapConverter.ToBitmap(frame);

            // put demo watermark on image if not licenced
            if (!Program.Snapshot.Licenced)
                using (Graphics g = Graphics.FromImage(image))
                {
                    Font myFont = new Font("Arial", 14);
                    g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                }

            return image;
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

            // wait until camera is unlocked
            while (locked) ;

            // lock the recorder so nobody else can use it
            locked = true;

            capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

            // snap the necessary images
            for (int i = 0; i < noOfImages; i++)
            {
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

                // if the recorder is locked, instead of taking a new picture just return the latest snapshot
                else
                    image = newestImage;
                
                images.Add(image);

                // wait for next burst
                Thread.Sleep(Program.Snapshot.Configuration.Period * 1000);
            }

            // unlock the recorder so others can use it
            locked = false;
            capture.Release();

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

            // wait until camera is unlocked
            while (locked) ;

            // lock the camera so nobody else can use it
            locked = true;

            // open the camera for recording
            capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

            // calculate the necessary number of frames by using the FPS of the camera
            while (frames < Program.Snapshot.Configuration.Duration * capture.Fps)
            {
                capture.Read(frame);

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
            locked = false;
            capture.Release();
        }

        /// <summary>
        /// Take a single picture, but optimized for fast usage
        /// by exploiting the capture already being opened
        /// </summary>
        /// <param name="type"></param>
        /// <param name="overrided"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Bitmap Snap(int type)
        {
            if (capture == null)
                throw new Exception("Cannot allocate camera!");

            // wait until camera is unlocked
            if (type == 0 && locked)
                while (locked) ;

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
        public string SnapBase64(int type)
        {
            if (capture == null)
                throw new Exception("Cannot allocate camera!");

            // wait until camera is unlocked
            if (type == 0 && locked)
                while (locked) ;

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

        #region Face detection

        public bool FaceDetection(int state)
        {
            // camera has not been configured - return
            if (capture == null)
                throw new Exception("Camera has not been configured!");

            // wait until camera is unlocked
            if (state == 0 && locked)
                while (locked) ;

            // lock the camera so others cannot use it
            locked = true;

            // first frame - open the stream
            if (state == 0)
                capture.Open(Program.Snapshot.Configuration.Cameras[index].CameraNumber);

            // snap an image
            Mat frame = new Mat();
            capture.Read(frame);

            // convert the snapshot to an image
            Bitmap image = BitmapConverter.ToBitmap(frame);

            // initialize haar characteristics for detecting faces
            var cascade = new CascadeClassifier(Application.StartupPath + "haarcascade_frontalface_alt.xml");
            List<Tuple<OpenCvSharp.Point, OpenCvSharp.Point>> rectangles = new List<Tuple<OpenCvSharp.Point, OpenCvSharp.Point>>();

            // detect frontal faces on image
            var faces = cascade.DetectMultiScale(
                image: image.ToMat(),
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionTypes.DoRoughSearch | HaarDetectionTypes.ScaleImage,
                minSize: new OpenCvSharp.Size(30, 30)
                );

            // check if faces are present
            bool result = false;
            if (faces.Length > 0)
                result = true;

            // last frame - close the stream
            if (state == 2)
                capture.Release();

            // unlock the camera so others can use it
            locked = false;

            return result;
        }

        #endregion

        #endregion
    }
}
