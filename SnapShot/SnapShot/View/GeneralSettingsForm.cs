using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;
using SnapShot.View;
using System.Net.NetworkInformation;
using System.Net;
using SnapShot.Model;

namespace SnapShot
{
    public partial class GeneralSettingsForm : Form
    {
        #region Attributes and properties

        static string JSONlocation = "";

        static bool refreshNeeded = false, syncStatus = true, firstCheck = false, updateLabel = false;

        public static string JSONLocation { get => JSONlocation; set => JSONlocation = value; }

        public static bool RefreshNeeded { get => refreshNeeded; set => refreshNeeded = value; }

        public static bool SyncStatus { get => syncStatus; set => syncStatus = value; }

        public static bool FirstCheck { get => firstCheck; set => firstCheck = value; }

        public static bool UpdateLabel { get => updateLabel; set => updateLabel = value; }

        #endregion

        #region Constructor

        public GeneralSettingsForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
            panel3.BorderStyle = BorderStyle.None;
            panel4.BorderStyle = BorderStyle.None;

            // show information from existing configuration
            UpdateConfigurationWindow();
        }

        /// <summary>
        /// Exit the application when clicking on the X button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
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

        #region Save configuration

        /// <summary>
        /// Save configuration for later usage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            // clear all errors if they exist
            string errorText = "";
            errorProvider1.Clear();

            // device configuration
            string triggerPath = textBox11.Text,
                   regex = textBox9.Text,
                   outputPath = textBox10.Text;
            int validity = (int)numericUpDown3.Value;

            // control validation

            // validate trigger file path
            if (triggerPath == "")
            {
                errorText = "Trigger path must not be empty!";
                errorProvider1.SetError(textBox11, errorText);
                textBox11.BackColor = Color.Red;
            }
            else if (!File.Exists(triggerPath))
            {
                errorText = "Trigger file must exist!";
                errorProvider1.SetError(textBox11, errorText);
                textBox11.BackColor = Color.Red;
            }
            else
            {
                textBox11.Text = textBox11.Text.Replace("\\", "/");
                textBox11.BackColor = Color.White;
                errorProvider1.SetError(textBox11, null);
            }

            // validate trigger regex
            if (regex == "")
            {
                errorText = "Regex must not be empty!";
                errorProvider1.SetError(textBox9, errorText);
                textBox9.BackColor = Color.Red;
            }
            else
            {
                textBox9.BackColor = Color.White;
                errorProvider1.SetError(textBox9, null);
            }

            // validate output folder path
            if (outputPath == "")
            {
                errorText = "Output path must not be empty!";
                errorProvider1.SetError(textBox10, errorText);
                textBox10.BackColor = Color.Red;
            }
            else if (!Directory.Exists(outputPath))
            {
                errorText = "Directory must exist!";
                errorProvider1.SetError(textBox10, errorText);
                textBox10.BackColor = Color.Red;
            }
            else
            {
                textBox10.Text = textBox10.Text.Replace("\\", "/");
                textBox10.BackColor = Color.White;
                errorProvider1.SetError(textBox10, null);
            }

            // server configuration
            string ip = textBox3.Text,
                   mediaPath = textBox7.Text,
                   JSONImportPath = textBox5.Text,
                   JSONExportPath = textBox1.Text;

            string statusText = label7.Text;
            bool status = statusText == "Connected" ? true : false;
            int port = 0;

            // control validation

            // if IP port is specified, it needs to be a valid number
            if (textBox4.Text.Length > 0)
            {
                try
                {
                    port = Int32.Parse(textBox4.Text);
                    textBox4.BackColor = Color.White;
                    errorProvider1.SetError(textBox4, null);
                }
                catch
                {
                    errorText = "Server port must be a valid number!";
                    errorProvider1.SetError(textBox4, errorText);
                    textBox4.BackColor = Color.Red;
                }
            }

            // validate media route path
            if (mediaPath.Length < 1)
            {
                errorText = "Server media path needs to be specified!";
                errorProvider1.SetError(textBox7, errorText);
                textBox7.BackColor = Color.Red;
            }
            else
            {
                textBox7.BackColor = Color.White;
                errorProvider1.SetError(textBox7, null);
            }

            // validate JSON import route path
            if (JSONImportPath.Length < 1)
            {
                errorText = "Server JSON import configuration path needs to be specified!";
                errorProvider1.SetError(textBox5, errorText);
                textBox5.BackColor = Color.Red;
            }
            else
            {
                textBox5.BackColor = Color.White;
                errorProvider1.SetError(textBox5, null);
            }

            // validate JSON export route path
            if (JSONExportPath.Length < 1)
            {
                errorText = "Server JSON export configuration path needs to be specified!";
                errorProvider1.SetError(textBox1, errorText);
                textBox1.BackColor = Color.Red;
            }
            else
            {
                textBox1.BackColor = Color.White;
                errorProvider1.SetError(textBox1, null);
            }

            // capture configuration
            bool image = radioButton8.Checked,
                 single = radioButton5.Checked;

            // calculate duration of snap
            int duration = (int)numericUpDown6.Value;
            if (domainUpDown6.Text == "minutes")
                duration *= 60;
            else if (domainUpDown6.Text == "hours")
                duration *= 3600;

            // calculate burst snap period
            int period = (int)numericUpDown5.Value;
            if (domainUpDown5.Text == "minutes")
                period *= 60;
            else if (domainUpDown5.Text == "hours")
                period *= 3600;

            // synchronization configuration
            int JSONSyncPeriod = (int)numericUpDown1.Value;
            if (domainUpDown4.Text == "minutes")
                JSONSyncPeriod *= 60;
            else if (domainUpDown4.Text == "hours")
                JSONSyncPeriod *= 3600;
            else if (domainUpDown4.Text == "days")
                JSONSyncPeriod *= 86400;
            int JSONTicks = Program.Snapshot.Configuration.JSONSyncPeriod;

            int mediaSyncPeriod = (int)numericUpDown2.Value;
            if (domainUpDown2.Text == "minutes")
                mediaSyncPeriod *= 60;
            else if (domainUpDown2.Text == "hours")
                mediaSyncPeriod *= 3600;
            else if (domainUpDown2.Text == "days")
                mediaSyncPeriod *= 86400;
            int mediaTicks = Program.Snapshot.Configuration.MediaSyncPeriod;

            // something was not correct - do not allow configuration to be created
            if (errorText.Length > 0)
            {
                toolStripStatusLabel1.Text = "Configuration could not be saved due to errors!";
                return;
            }

            // clear all errors from previous attempts
            errorProvider1.Clear();

            // retain all earlier camera configurations
            List<Camera> oldCameras = Program.Snapshot.Configuration.Cameras;

            // create new configuration for the specified camera
            Program.Snapshot.Configuration = new Configuration()
            {
                TriggerFilePath = triggerPath,
                Regex = regex,
                OutputFolderPath = outputPath,
                OutputValidity = validity,
                Cameras = oldCameras,
                ServerIP = ip,
                MediaPath = mediaPath,
                JSONImportLocation = JSONImportPath,
                JSONExportLocation = JSONExportPath,
                ServerPort = port,
                ConnectionStatus = status,
                JSONSyncPeriod = JSONSyncPeriod,
                JSONTicks = JSONTicks,
                MediaSyncPeriod = mediaSyncPeriod,
                MediaTicks = mediaTicks,
                ImageCapture = image,
                SingleMode = single,
                Duration = duration,
                Period = period
            };

            // reconfigure all recorders with new configurations
            Program.Recorders.ForEach(r => r.Reconfigure());

            // notify the user that the configuration has been saved
            toolStripStatusLabel1.Text = "Configuration successfully saved!";
        }

        #endregion

        #region Update configuration

        /// <summary>
        /// Update configuration after import/export
        /// </summary>
        public void UpdateConfigurationWindow()
        {
            var config = Program.Snapshot.Configuration;

            // device configuration
            textBox11.Text = config.TriggerFilePath;
            textBox9.Text = config.Regex;
            textBox10.Text = config.OutputFolderPath;
            if (config.OutputValidity > 0)
                numericUpDown3.Value = config.OutputValidity;
            else
                numericUpDown3.Value = 1;

            // server configuration
            textBox3.Text = config.ServerIP;
            textBox7.Text = config.MediaPath;
            textBox5.Text = config.JSONImportLocation;
            textBox1.Text = config.JSONExportLocation;
            if (config.ServerIP.Length > 0 && config.ServerPort != 0)
                textBox4.Text = config.ServerPort.ToString();
            else
                textBox4.Text = "";
            if (config.ConnectionStatus)
            {
                label7.Text = "Connected";
                label7.ForeColor = Color.Green;
            }
            else
            {
                label7.Text = "Disconnected";
                label7.ForeColor = Color.Red;
            }

            // capture configuration
            radioButton8.Checked = config.ImageCapture;
            radioButton7.Checked = !config.ImageCapture;
            radioButton5.Checked = config.SingleMode;
            radioButton6.Checked = !config.SingleMode;

            numericUpDown6.Enabled = !config.SingleMode;
            numericUpDown6.ReadOnly = config.SingleMode;
            domainUpDown6.Enabled = !config.SingleMode;
            domainUpDown6.ReadOnly = config.SingleMode;

            string unit = "seconds";
            int time = config.Duration;
            while (unit != "hours" && (int)(time / 60) > 0)
            {
                time = time / 60;
                if (unit == "seconds")
                    unit = "minutes";
                else if (unit == "minutes")
                    unit = "hours";
            }
            if (time > 0)
            {
                numericUpDown6.Value = (int)time;
                domainUpDown6.Text = unit;
            }
            else
            {
                numericUpDown6.Value = 1;
                domainUpDown6.Text = "seconds";
            }

            unit = "seconds";
            time = config.Period;
            while (unit != "hours" && (int)(time / 60) > 0)
            {
                time = time / 60;
                if (unit == "seconds")
                    unit = "minutes";
                else if (unit == "minutes")
                    unit = "hours";
            }
            if (time > 0)
            {
                numericUpDown5.Value = (int)time;
                domainUpDown5.Text = unit;
            }
            else
            {
                numericUpDown5.Value = 1;
                domainUpDown5.Text = "seconds";
            }

            // synchronization configuration
            unit = "seconds";
            time = config.JSONSyncPeriod;
            while (unit != "days" && (int)(time / 60) > 0)
            {
                if (unit == "seconds")
                {
                    unit = "minutes";
                    time = time / 60;
                }
                else if (unit == "minutes")
                {
                    unit = "hours";
                    time = time / 60;
                }
                else if (unit == "hours")
                {
                    unit = "days";
                    time = time / 24;
                }
            }
            if (time > 0)
            {
                numericUpDown1.Value = (int)time;
                domainUpDown4.Text = unit;
            }
            else
            {
                numericUpDown1.Value = 1;
                domainUpDown4.Text = "seconds";
            }
            unit = "seconds";
            time = config.MediaSyncPeriod;
            while (unit != "days" && (int)(time / 60) > 0)
            {
                if (unit == "seconds")
                {
                    unit = "minutes";
                    time = time / 60;
                }
                else if (unit == "minutes")
                {
                    unit = "hours";
                    time = time / 60;
                }
                else if (unit == "hours")
                {
                    unit = "days";
                    time = time / 24;
                }
            }
            if (time > 0)
            {
                numericUpDown2.Value = (int)time;
                domainUpDown2.Text = unit;
            }
            else
            {
                numericUpDown2.Value = 1;
                domainUpDown2.Text = "seconds";
            }

            // clear any previous output
            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region File paths

        /// <summary>
        /// Select the file that contains capture trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox11.Text = openFileDialog.FileName;
                    errorProvider1.SetError(textBox11, null);
                    textBox11.BackColor = Color.White;
                }
            }

            toolStripStatusLabel1.Text = "";
        }

        /// <summary>
        /// Select the folder where the capture will be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox10.Text = folderBrowserDialog1.SelectedPath;
                errorProvider1.SetError(textBox10, null);
                textBox10.BackColor = Color.White;
            }

            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Radio buttons

        /// <summary>
        /// Enable/disable burst period selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            // burst selected - allow for burst duration and period to be selected
            if (radioButton6.Checked)
            {
                // enable duration
                numericUpDown6.Enabled = true;
                numericUpDown6.ReadOnly = false;
                domainUpDown6.Enabled = true;
                domainUpDown6.ReadOnly = false;

                // enable burst period
                numericUpDown5.Enabled = true;
                numericUpDown5.ReadOnly = false;
                domainUpDown5.Enabled = true;
                domainUpDown5.ReadOnly = false;
            }

            // burst not selected - do not allow burst period to be selected
            else
            {
                // disable burst period
                numericUpDown5.Enabled = false;
                numericUpDown5.ReadOnly = true;
                domainUpDown5.Enabled = false;
                domainUpDown5.ReadOnly = true;
            }
        }

        /// <summary>
        /// Disable burst when video mode is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            // video is selected - burst not available, duration is available
            if (radioButton7.Checked)
            {
                // automatically select single
                radioButton5.Checked = true;

                // disable burst button
                radioButton6.Enabled = false;
                radioButton6.Checked = false;

                // disable burst period
                numericUpDown5.Enabled = false;
                numericUpDown5.ReadOnly = true;
                domainUpDown5.Enabled = false;
                domainUpDown5.ReadOnly = true;

                // enable duration
                numericUpDown6.Enabled = true;
                numericUpDown6.ReadOnly = false;
                domainUpDown6.Enabled = true;
                domainUpDown6.ReadOnly = false;
            }

            // video is not selected - enable burst to be selected
            else
            {
                // enable burst button
                radioButton6.Enabled = true;

                // enable burst period
                numericUpDown5.Enabled = true;
                numericUpDown5.ReadOnly = false;
                domainUpDown5.Enabled = true;
                domainUpDown5.ReadOnly = false;
            }
        }

        /// <summary>
        /// Automatically select single when selecting image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            // image selected - select single
            if (radioButton8.Checked)
            {
                // automatically select single
                radioButton5.Checked = true;

                // disable burst period
                numericUpDown5.Enabled = false;
                numericUpDown5.ReadOnly = true;
                domainUpDown5.Enabled = false;
                domainUpDown5.ReadOnly = true;

                // disable duration
                numericUpDown6.Enabled = false;
                numericUpDown6.ReadOnly = true;
                domainUpDown6.Enabled = false;
                domainUpDown6.ReadOnly = true;
            }
        }

        /// <summary>
        /// Automatically disable duration when single image is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            // single image selected - disable duration
            if (radioButton8.Checked && radioButton5.Checked)
            {
                numericUpDown6.Enabled = false;
                numericUpDown6.ReadOnly = true;
                domainUpDown6.Enabled = false;
                domainUpDown6.ReadOnly = true;
            }
            // any other combination - enable duration
            else
            {
                numericUpDown6.Enabled = true;
                numericUpDown6.ReadOnly = false;
                domainUpDown6.Enabled = true;
                domainUpDown6.ReadOnly = false;
            }
        }

        #endregion

        #region Server connection

        /// <summary>
        /// Connect to provided remote server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // contact the server IP address
                var ping = new Ping();
                var reply = ping.Send(textBox3.Text, 1000);

                label7.Text = "Connected";
                label7.ForeColor = Color.Green;
                
                // check whether the device is registered on the server
                string url = textBox3.Text;
                if (textBox4.Text.Length > 0)
                    url += ":" + textBox4.Text;

                Configuration.DeviceCheck(url);
            }
            catch
            {
                label7.Text = "Disconnected";
                label7.ForeColor = Color.Red;
            }
        }

        #endregion

        #region Synchronization

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            if (syncStatus && firstCheck && RefreshNeeded && UpdateLabel)
                label3.Text = "Synchronization successful.";
            else if (firstCheck && UpdateLabel)
                label3.Text = "Synchronization failed.";
            else
                label3.Text = "";

            if (UpdateLabel)
                UpdateLabel = false;

            if (RefreshNeeded)
            {
                RefreshNeeded = false;
                Program.Recorders.ForEach(r => r.Reconfigure());
                UpdateConfigurationWindow();
            }
        }

        #endregion
    }
}
