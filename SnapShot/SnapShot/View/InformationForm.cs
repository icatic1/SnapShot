using SnapShot.View;
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

namespace SnapShot
{
    public partial class InformationForm : Form
    {
        #region Attributes

        bool closeTheApp = true;

        #endregion

        #region Constructor

        public InformationForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
        }

        /// <summary>
        /// Hide the form when closing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InformationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closeTheApp)
                Application.Exit();
            else
                this.Hide();
        }

        #endregion

        #region Menu items

        /// <summary>
        /// Redirect to licence form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void licencingOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LicencingForm f = new LicencingForm();
            this.Hide();
            f.Show();
        }

        /// <summary>
        /// Redirect to licencing server change form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void administratorOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminLicencingForm popup = new AdminLicencingForm();
            var result = popup.ShowDialog();
            if (result == DialogResult.OK)
                toolStripStatusLabel1.Text = "Licencing server successfully changed!";
        }

        /// <summary>
        /// Redirect to general settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            GeneralSettingsForm f = new GeneralSettingsForm();
            this.Hide();
            f.Show();
        }

        /// <summary>
        /// Redirect to camera 1-specific settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            CameraSettingsForm f = new CameraSettingsForm(0);
            this.Hide();
            f.Show();
        }

        /// <summary>
        /// Redirect to camera 2-specific settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            CameraSettingsForm f = new CameraSettingsForm(1);
            this.Hide();
            f.Show();
        }

        /// <summary>
        /// Redirect to camera 3-specific settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            CameraSettingsForm f = new CameraSettingsForm(2);
            this.Hide();
            f.Show();
        }

        /// <summary>
        /// Redirect to JSON import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importExistingConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = "configuration.json";
            if (Program.Snapshot.Configuration.ServerIP.Length > 9)
            {
                path = Program.Snapshot.Configuration.ServerIP;
                if (Program.Snapshot.Configuration.ServerPort != 0)
                    path += ":" + Program.Snapshot.Configuration.ServerPort;
                if (Program.Snapshot.JSONImport != "")
                    path += "/" + Program.Snapshot.JSONImport;
            }

            bool res = Configuration.ImportFromJSON(path);
            if (res)
            {
                // change the trigger file that is being monitored
                Thread newThread = new Thread(() => Program.ChangeTrigger());
                newThread.IsBackground = true;
                newThread.Start();

                // reconfigure all cameras in a separate thread
                Thread threadReconfigure = new Thread(() => Program.ReconfigureAllRecorders());
                threadReconfigure.IsBackground = true;
                threadReconfigure.Start();

                // start thread which will constantly check if faces are present
                if (Program.Snapshot.Configuration.FaceDetectionTrigger)
                {
                    var snap = Program.Snapshot;
                    Thread faceChecker = new Thread(() => Program.FaceDetectionTrigger(ref snap));
                    faceChecker.IsBackground = true;
                    faceChecker.Start();
                }

                toolStripStatusLabel1.Text = "Import successfully completed.";
            }
            else
                toolStripStatusLabel1.Text = "The import could not be completed successfully. Check JSON file for errors.";
        }

        /// <summary>
        /// Redirect to help
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pomoćToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformationForm f = new InformationForm();
            this.Hide();
            f.Show();
        }

        #endregion

        #region Minimizing to system tray

        /// <summary>
        /// When the form is minimized, it goes to system tray and a notification is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InformationForm_Resize(object sender, EventArgs e)
        {
            notifyIcon1.Icon = SystemIcons.Information;
            if (this.WindowState == FormWindowState.Minimized)
            {
                closeTheApp = false;
                this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        /// <summary>
        /// When the form is maximized, the notification is hidden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            closeTheApp = true;
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        #endregion
    }
}
