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

        Snapshot snapshot;
        Bitmap? image;
        Mat? frame;
        Thread? camera;
        VideoCapture? capture;
        static bool cancel;
        int cameraNumber;

        #endregion

        #region Constructor

        public CapturePreviewForm(Snapshot s, string device, int camNo)
        {
            InitializeComponent();
            cancel = false;
            snapshot = s;
            textBox1.Text = device;
            cameraNumber = camNo;
            toolStripStatusLabel1.Text = "";

            // configure and start using camera input
            CaptureCamera();
        }

        private void CapturePreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancel = true;
        }

        #endregion

        #region Camera capture

        private void CaptureCamera()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.IsBackground = true;
            camera.Start();
        }

        private void CaptureCameraCallback()
        {

            frame = new Mat();
            capture = new VideoCapture(cameraNumber);
            capture.Open(cameraNumber);

            if (capture.IsOpened())
            {
                while (1 == 1)
                {
                    capture.Read(frame);
                    image = BitmapConverter.ToBitmap(frame);
                    SetPicture(image);

                    if (cancel)
                    {
                        capture.Release();
                        break;
                    }
                }
            }
        }

        private void SetPicture(Image img)
        {
            try
            {
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

            }
        }

        #endregion
    }
}
