﻿using SnapShot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot.View
{
    public partial class CameraSettingsForm : Form
    {
        #region Attributes

        int cameraNumber;

        #endregion

        #region Constructor

        public CameraSettingsForm(int cameraIndex)
        {
            InitializeComponent();
            cameraNumber = cameraIndex;
            InitializeForm();
        }

        /// <summary>
        /// Exit the application when clicking on the X button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void registracijaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LicencingForm f = new LicencingForm();
            this.Hide();
            f.ShowDialog();
            this.Close();
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
        /// Redirect to JSON export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JSONPopupForm popup = new JSONPopupForm();
            var result = popup.ShowDialog();
            if (result == DialogResult.OK)
            {
                bool res = Configuration.ExportToJSON(GeneralSettingsForm.JSONLocation);
                if (res)
                    toolStripStatusLabel1.Text = "Export successfully completed.";
                else
                    toolStripStatusLabel1.Text = "The export could not be completed successfully.";
            }
        }

        /// <summary>
        /// Redirect to JSON import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JSONPopupForm popup = new JSONPopupForm();
            var result = popup.ShowDialog();
            if (result == DialogResult.OK)
            {
                bool res = Configuration.ImportFromJSON(GeneralSettingsForm.JSONLocation);
                if (res)
                {
                    Program.Recorders.ForEach(r => r.Reconfigure());
                    toolStripStatusLabel1.Text = "Import successfully completed.";
                }
                else
                    toolStripStatusLabel1.Text = "The import could not be completed successfully. Check JSON file for errors.";
            }

            // update the form to show new configuration
            UpdateConfigurationWindow();
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

        #region Configuration initialization

        /// <summary>
        /// Show information about camera[cameraIndex] on the form
        /// </summary>
        /// <param name="cameraIndex"></param>
        public void InitializeForm()
        {
            // automatically select USB camera
            comboBox3.Text = "USB camera";

            // add all available USB camera devices to combo box list
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    comboBox4.Items.Add(device["Caption"].ToString());
                    comboBox4.Text = device["Caption"].ToString();
                }
            }

            // automatically select standard resolution
            comboBox5.Text = "640x480";

            // update other controls depending on existing configuration
            UpdateConfigurationWindow();
        }

        /// <summary>
        /// Update form controls by using existing configuration
        /// </summary>
        public void UpdateConfigurationWindow()
        {
            var config = Program.Snapshot.Configuration.Cameras[cameraNumber];

            // initialize recorder for the selected camera - make recording available
            Program.Recorders[cameraNumber].Reconfigure();
            
            // initialize all other controls
            comboBox3.Text = config.Type.ToString();
            comboBox4.Text = config.Id;
            comboBox3.Text = config.Resolution.ToString();
            trackBar1.Value = config.ContrastLevel;
            pictureBox1.BackColor = config.ImageColor;
            checkBox1.Checked = config.MotionDetection;

            // erase any existing message on the form
            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Configuration saving

        /// <summary>
        /// Save configuration button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            // device configuration
            DeviceType type = DeviceType.USBCamera;
            if (comboBox3.Text == "IP camera")
                type = DeviceType.IPCamera;

            string deviceName = comboBox4.Text;
            int cameraNumber = (int)comboBox4.SelectedIndex;

            // video configuration
            string res = "Resolution" + comboBox5.Text;
            Resolution resolution = (Resolution)Enum.Parse(typeof(Resolution), res);

            int contrast = trackBar1.Value;
            Color color = pictureBox1.BackColor;
            bool motionDetection = checkBox1.Checked;

            // save settings
            Camera camera = new Camera()
            {
                Type = type,
                Id = deviceName,
                CameraNumber = cameraNumber,
                Resolution = resolution,
                ContrastLevel = contrast,
                ImageColor = color,
                MotionDetection = motionDetection
            };

            Program.Snapshot.Configuration.Cameras[cameraNumber] = camera;

            // reconfigure the recorder to use new settings
            Program.Recorders[cameraNumber].Reconfigure();

            // notify the user that the new configuration has been saved
            toolStripStatusLabel1.Text = "Configuration successfully saved!";
        }

        #endregion

        #region Camera preview

        /// <summary>
        /// Enter new form which shows camera input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            // just show new form (without closing the currently active one)
            CapturePreviewForm f = new CapturePreviewForm(comboBox4.Text, comboBox4.SelectedIndex, "USB");
            f.Show();
        }

        #endregion

        #region Color palette

        /// <summary>
        /// Choose color from color palette dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                pictureBox1.BackColor = colorDialog1.Color;

            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Motion detection

        /// <summary>
        /// Not part of this sprint - just simulated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                toolStripStatusLabel1.Text = "Motion detection ON!";
            else
                toolStripStatusLabel1.Text = "Motion detection OFF!";
        }

        #endregion
    }
}
