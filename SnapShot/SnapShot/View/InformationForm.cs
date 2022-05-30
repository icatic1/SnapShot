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
        #region Constructor

        public InformationForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
        }

        private void InformationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
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
            f.ShowDialog();
            this.Close();
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
            f.ShowDialog();
            this.Close();
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
            f.ShowDialog();
            this.Close();
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
            f.ShowDialog();
            this.Close();
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
            f.ShowDialog();
            this.Close();
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
                Thread threadReconfigure = new Thread(() => Program.ReconfigureAllRecorders());
                threadReconfigure.IsBackground = true;
                threadReconfigure.Start();

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
            f.ShowDialog();
            this.Close();
        }

        #endregion
    }
}
