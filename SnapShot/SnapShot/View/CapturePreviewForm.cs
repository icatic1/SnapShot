using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.Drawing.Imaging;
using OpenCvSharp.Extensions;

namespace SnapShot
{
    public partial class CapturePreviewForm : Form
    {
        #region Attributes

        Bitmap? image;
        Thread? camera;
        static bool cancel = false, faceDetection = false;
        int cameraNumber;
        string deviceType;

        #endregion

        #region Constructor

        public CapturePreviewForm(string device, int camNo, string type)
        {
            InitializeComponent();
            cancel = false;
            textBox1.Text = device;
            cameraNumber = camNo;
            deviceType = type;
            toolStripStatusLabel1.Text = "";

            // configure and start using camera input
            CaptureCamera();
        }

        /// <summary>
        /// Stop the livestream when the X button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CapturePreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancel = true;
        }

        #endregion

        #region Camera Capture

        /// <summary>
        /// Starting thread for livestream
        /// </summary>
        void CaptureCamera()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.IsBackground = true;
            camera.Start();
        }

        /// <summary>
        /// Performing the livestream by constantly snapping images
        /// </summary>
        void CaptureCameraCallback()
        {
            // start the camera recorder
            Program.Recorders[cameraNumber].Snap(0, true);

            while (1 == 1)
            {
                // snap picture
                image = Program.Recorders[cameraNumber].Snap(1, true);

                // frame face if face detection is selected
                List<Tuple<OpenCvSharp.Point, OpenCvSharp.Point>> rectangles =
                    new List<Tuple<OpenCvSharp.Point, OpenCvSharp.Point>>();
                if (faceDetection)
                    rectangles = FrameFace(image);

                // set the pictureBox value
                SetPicture(image, rectangles);

                // stop the livestream
                if (cancel)
                    break;
            }

            // stop the camera recorder
            Program.Recorders[cameraNumber].Snap(2, true);
        }

        /// <summary>
        /// Setting the pictureBox value from a remote thread
        /// </summary>
        /// <param name="img"></param>
        void SetPicture(Image img, List<Tuple<OpenCvSharp.Point, OpenCvSharp.Point>> rectangles)
        {
            try
            {
                Graphics g = Graphics.FromImage(img);
                // put demo watermark on image if not licenced
                if (!Program.Snapshot.Licenced)
                { 
                    Font myFont = new Font("Arial", 14);
                    g.DrawString("Demo version", myFont, Brushes.Black, new System.Drawing.Point(2, 2));
                }

                // draw rectangles on faces
                Pen pen = new Pen(Color.Black, 3);
                foreach (var rectangle in rectangles)
                {
                    int width = rectangle.Item2.X - rectangle.Item1.X,
                        height = rectangle.Item2.Y - rectangle.Item1.Y;
                    
                    g.DrawRectangle(pen, new Rectangle(rectangle.Item1.X, rectangle.Item1.Y, width, height));
                }

                // send the image to the painter
                if (pictureBox1.InvokeRequired)
                {
                    pictureBox1.Invoke(new MethodInvoker(
                    delegate ()
                    {
                        pictureBox1.Image = img;
                    }));
                }
                else
                {
                    pictureBox1.Image = img;
                }
            }
            catch
            {
                // ignore errors
            }
        }

        #endregion

        #region Face Detection

        /// <summary>
        /// Set face detection depending on whether the check box is checked or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            faceDetection = checkBox1.Checked;
        }

        /// <summary>
        /// Put a frame around faces present in the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public List<Tuple<OpenCvSharp.Point, OpenCvSharp.Point>> FrameFace(Bitmap image)
        {
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

            // add rectangle for every detected face
            foreach (var faceRect in faces)
            {
                rectangles.Add(new Tuple<OpenCvSharp.Point, OpenCvSharp.Point>(faceRect.TopLeft, faceRect.BottomRight));
            }

            return rectangles;
        }

        #endregion
    }
}
