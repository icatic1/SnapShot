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
        Bitmap image;
        Mat frame;
        Thread camera;
        VideoCapture capture;
        static bool cancel;

        #endregion

        #region Constructor

        public CapturePreviewForm(Snapshot s, string device)
        {
            InitializeComponent();
            cancel = false;
            snapshot = s;
            textBox1.Text = device;
            toolStripStatusLabel1.Text = "";

            // configure and start using camera input
            CaptureCamera();
        }

        private void CapturePreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Menu items

        /// <summary>
        /// Go to configuration form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Hide();
            ConfigurationForm f = new ConfigurationForm(snapshot);
            f.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Go to licencing form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registracijaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Hide();
            LicencingForm f = new LicencingForm(snapshot);
            f.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Go to help form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pomoćToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Hide();
            InformationForm f = new InformationForm(snapshot);
            f.ShowDialog();
            this.Close();
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
            capture = new VideoCapture(0);
            capture.Open(0);

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

        #endregion
    }
}
