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

        static List<bool> refreshNeeded = new List<bool> { false, false },
                          syncStatus = new List<bool> { true, true },
                          firstCheck = new List<bool> { false, false },
                          updateLabel = new List<bool> { false, false };

        public static string JSONLocation { get => JSONlocation; set => JSONlocation = value; }

        public static List<bool> RefreshNeeded { get => refreshNeeded; set => refreshNeeded = value; }

        public static List<bool> SyncStatus { get => syncStatus; set => syncStatus = value; }

        public static List<bool> FirstCheck { get => firstCheck; set => firstCheck = value; }

        public static List<bool> UpdateLabel { get => updateLabel; set => updateLabel = value; }

        #endregion

        #region Constructor

        public GeneralSettingsForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";

            panel1.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.None;
            panel3.BorderStyle = BorderStyle.None;
            panel4.BorderStyle = BorderStyle.None;
            
            dateTimePicker1.CustomFormat = "HH:mm";
            dateTimePicker2.CustomFormat = "HH:mm";

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

            int JSONSyncPeriod = 0,
                JSONTicks = Program.Snapshot.Configuration.JSONSyncPeriod;
            TimeSpan JSONSync = new TimeSpan(0, 0, 0);

            if (radioButton1.Checked)
            {
                JSONSyncPeriod = (int)numericUpDown1.Value;
                if (domainUpDown4.Text == "minutes")
                    JSONSyncPeriod *= 60;
                else if (domainUpDown4.Text == "hours")
                    JSONSyncPeriod *= 3600;
                else if (domainUpDown4.Text == "days")
                    JSONSyncPeriod *= 86400;
                
            }
            else
                JSONSync = dateTimePicker1.Value.TimeOfDay;

            int mediaSyncPeriod = 0,
                mediaTicks = Program.Snapshot.Configuration.MediaSyncPeriod;
            TimeSpan mediaSync = new TimeSpan(0, 0, 0);

            if (radioButton4.Checked)
            {
                mediaSyncPeriod = (int)numericUpDown2.Value;
                if (domainUpDown2.Text == "minutes")
                    mediaSyncPeriod *= 60;
                else if (domainUpDown2.Text == "hours")
                    mediaSyncPeriod *= 3600;
                else if (domainUpDown2.Text == "days")
                    mediaSyncPeriod *= 86400;
            }
            else
                mediaSync = dateTimePicker2.Value.TimeOfDay;

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
                JSONTime = JSONSync,
                MediaSyncPeriod = mediaSyncPeriod,
                MediaTicks = mediaTicks,
                MediaTime = mediaSync,
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
            domainUpDown6.Enabled = !config.SingleMode;

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
                radioButton1.Checked = true;
            }
            else
            {
                numericUpDown1.Value = 1;
                domainUpDown4.Text = "seconds";
                radioButton2.Checked = true;
                dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, config.JSONTime.Hours, config.JSONTime.Minutes, config.JSONTime.Seconds);
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
                radioButton4.Checked = true;
            }
            else
            {
                numericUpDown2.Value = 1;
                domainUpDown2.Text = "seconds";
                radioButton3.Checked = true;
                dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, config.MediaTime.Hours, config.MediaTime.Minutes, config.MediaTime.Seconds);
            }
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

            // enable/disable burst period
            numericUpDown5.Enabled = radioButton6.Checked;
            domainUpDown5.Enabled = radioButton6.Checked;

            if (radioButton6.Checked)
            {
                // enable duration
                numericUpDown6.Enabled = true;
                domainUpDown6.Enabled = true;
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

            // disable burst button
            radioButton6.Enabled = !radioButton7.Checked;

            // disable burst period
            numericUpDown5.Enabled = !radioButton7.Checked;
            domainUpDown5.Enabled = !radioButton7.Checked;

            if (radioButton7.Checked)
            {
                // automatically select single
                radioButton5.Checked = true;

                // unselect burst button
                radioButton6.Checked = false; 

                // enable duration
                numericUpDown6.Enabled = true;
                domainUpDown6.Enabled = true;
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
                domainUpDown5.Enabled = false;

                // disable duration
                numericUpDown6.Enabled = false;
                domainUpDown6.Enabled = false;
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
            numericUpDown6.Enabled = !(radioButton8.Checked && radioButton5.Checked);
            domainUpDown6.Enabled = !(radioButton8.Checked && radioButton5.Checked);
        }

        /// <summary>
        /// Disable selecting date when JSON sync period is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = radioButton1.Checked;
            domainUpDown4.Enabled = radioButton1.Checked;

            dateTimePicker1.Enabled = !radioButton1.Checked;
        }

        /// <summary>
        /// Disable selecting period when JSON sync date is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = radioButton2.Checked;

            numericUpDown1.Enabled = !radioButton2.Checked;
            domainUpDown4.Enabled = !radioButton2.Checked;
        }

        /// <summary>
        /// Disable selecting date when media sync period is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = radioButton4.Checked;
            domainUpDown2.Enabled = radioButton4.Checked;

            dateTimePicker2.Enabled = !radioButton4.Checked;
        }

        /// <summary>
        /// Disable selecting period when media sync date is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Enabled = radioButton3.Checked;

            numericUpDown2.Enabled = !radioButton3.Checked;
            domainUpDown2.Enabled = !radioButton3.Checked;
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

                // uncomment this if it is necessary - prolongs the connection time
                //Configuration.DeviceCheck(url);
            }
            catch
            {
                label7.Text = "Disconnected";
                label7.ForeColor = Color.Red;
            }
        }

        #endregion

        #region Synchronization

        /// <summary>
        /// Initialize timers which will update the synchronization status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            Timer timerJSON = new Timer();
            Timer timerMedia = new Timer();

            // minimum possible sync time is 1000 ms (1 s) - check for updates every 1 s
            timerJSON.Interval = 1000;
            timerJSON.Tick += new EventHandler(UpdateJSONSynchronization);
            timerJSON.Start();

            // minimum possible sync time is 1000 ms (1 s) - check for updates every 1 s
            timerMedia.Interval = 1000;
            timerMedia.Tick += new EventHandler(UpdateMediaSynchronization);
            timerMedia.Start();
        }

        /// <summary>
        /// Method which updates the labels to show current synchronization status for JSON config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateJSONSynchronization(object? sender, EventArgs e)
        {
            if (syncStatus[0] && firstCheck[0] && refreshNeeded[0] && updateLabel[0])
                label3.Text = "Synchronization successful.";
            else if (firstCheck[0] && updateLabel[0])
                label3.Text = "Synchronization failed.";
            else
                label3.Text = "Waiting.";

            // we updated the form - reset the update
            if (updateLabel[0])
                updateLabel[0] = false;

            // when the JSON config is synchronized, camera recorders need to be reset
            if (refreshNeeded[0])
            {
                refreshNeeded[0] = false;
                Program.Recorders.ForEach(r => r.Reconfigure());
                UpdateConfigurationWindow();
            }
        }
        
        /// <summary>
        /// Method which updates the labels to show current synchronization status for media files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateMediaSynchronization(object? sender, EventArgs e)
        {
            if (syncStatus[1] && firstCheck[1] && refreshNeeded[1] && updateLabel[1])
                label14.Text = "Synchronization successful.";
            else if (firstCheck[1] && updateLabel[1])
                label14.Text = "Synchronization failed.";
            else
                label14.Text = "Waiting.";

            // we updated the form - reset the update
            if (updateLabel[1])
                updateLabel[1] = false;
        }

        /// <summary>
        /// Method for manual synchronization of JSON configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // formulate the path for importing JSON configuration from server
                string path = "http://" + Program.Snapshot.Configuration.ServerIP;
                if (Program.Snapshot.Configuration.ServerPort != 0)
                    path += ":" + Program.Snapshot.Configuration.ServerPort;
                string JSONPath = path + "/" + Program.Snapshot.Configuration.JSONImportLocation;

                Configuration.ImportFromJSON(JSONPath);

                // send update signal to keep the label active
                firstCheck[0] = true;
                syncStatus[0] = true;
                refreshNeeded[0] = true;
                updateLabel[0] = true;

                label3.Text = "Synchronization successful.";

                // denote that the synchronization has occured
                Program.Snapshot.Configuration.JSONTicks = (int)DateTime.Now.Ticks;

                // when the JSON config is synchronized, camera recorders need to be reset
                Program.Recorders.ForEach(r => r.Reconfigure());
                UpdateConfigurationWindow();
            }
            catch
            {
                // send failed signal to keep the label active
                syncStatus[0] = false;
                updateLabel[0] = true;

                label3.Text = "Synchronization failed.";
            }
        }

        /// <summary>
        /// Method for manual synchronization of media files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // formulate the path for exporting files to server
                string path = "http://" + Program.Snapshot.Configuration.ServerIP;
                if (Program.Snapshot.Configuration.ServerPort != 0)
                    path += ":" + Program.Snapshot.Configuration.ServerPort;
                string mediaPath = path + "/" + Program.Snapshot.Configuration.MediaPath;

                // get all locally created images and videos
                string[] localEntries = Directory.GetFileSystemEntries(Program.Snapshot.Configuration.OutputFolderPath, "*", SearchOption.AllDirectories);

                for (int i = 0; i < localEntries.Length; i++)
                    localEntries[i] = localEntries[i].Replace(Program.Snapshot.Configuration.OutputFolderPath, "").TrimStart('\\');

                // get all images and videos located on the server
                string[] serverEntries = Program.GetEntriesFromServer(Program.Snapshot.Configuration.ServerIP, Program.Snapshot.Configuration.ServerPort.ToString(), Program.Snapshot.Configuration.MediaPath);

                // find all local entries which are not present among server entries
                List<string> newEntries = Program.FindNewEntries(localEntries, serverEntries);

                // upload every new local file to server
                foreach (var newEntry in newEntries)
                    Configuration.UploadFile(mediaPath, newEntry, Program.Snapshot.Configuration.OutputFolderPath);

                // send update signal to keep the label active
                firstCheck[1] = true;
                syncStatus[1] = true;
                refreshNeeded[1] = true;
                updateLabel[1] = true;

                label14.Text = "Synchronization successful.";
            }
            catch
            {
                // send failed signal to keep the label active
                syncStatus[1] = false;
                updateLabel[1] = true;

                label14.Text = "Synchronization failed.";
            }
        }

        #endregion
    }
}
