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
using System.Threading;
using System.Text.RegularExpressions;

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
            comboBox1.Text = "https://";

            panel1.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.None;
            panel3.BorderStyle = BorderStyle.None;
            panel4.BorderStyle = BorderStyle.None;

            dateTimePicker1.CustomFormat = "HH:mm";
            dateTimePicker2.CustomFormat = "HH:mm";

            checkBox2.Checked = true;

            // show information from existing configuration
            UpdateConfigurationWindow();

            toolTip1.SetToolTip(textBox10, textBox10.Text);
            toolTip1.SetToolTip(textBox11, textBox11.Text);
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
            // if server information is specified but no connection, do not allow the
            // configuration to be saved
            if (textBox3.Text.Length > 0 && label7.Text == "Disconnected")
            {
                toolStripStatusLabel1.Text = "Configuration could not be saved because server connection was not established!";
                return;
            }
            else if (!checkBox2.Checked && !checkBox1.Checked)
            {
                toolStripStatusLabel1.Text = "You must select at least one trigger!";
                return;
            }

            // clear all errors if they exist
            string errorText = "";

            // device configuration
            string triggerPath = textBox11.Text,
                   regex = textBox9.Text,
                   outputPath = textBox10.Text;
            int validity = (int)numericUpDown3.Value;
            bool faceDetectionTrigger = checkBox1.Checked;

            // control validation

            // validate trigger file path
            if (checkBox2.Checked && triggerPath == "")
                errorText = "Trigger path must not be empty!";
            else
                textBox11.Text = textBox11.Text.Replace("\\", "/");

            if (!checkBox2.Checked)
                triggerPath = "";

            // validate trigger regex
            if (checkBox2.Checked && regex == "")
                errorText = "Regex must not be empty!";
            else
                textBox9.BackColor = Color.White;

            if (!checkBox2.Checked)
                regex = "";

            // validate output folder path
            if (outputPath == "")
                errorText = "Output path must not be empty!";
            else if (!Directory.Exists(outputPath))
                errorText = "Directory must exist!";
            else
                textBox10.Text = textBox10.Text.Replace("\\", "/");

            // server configuration
            string ip = comboBox1.Text + textBox3.Text,
                   mediaPath = textBox7.Text;

            string statusText = label7.Text;
            int port = 0;

            // we are not connected - do not save the server
            if (statusText != "Connected")
            {
                ip = "";
                mediaPath = "";
            }

            // control validation
            else if (textBox3.Text.Length > 0)
            {
                // if IP port is specified, it needs to be a valid number
                if (textBox4.Text.Length > 0)
                {
                    try
                    {
                        port = Int32.Parse(textBox4.Text);
                    }
                    catch
                    {
                        errorText = "Server port must be a valid number!";
                    }
                }
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
                toolStripStatusLabel1.Text = errorText;
                return;
            }

            // retain all earlier camera configurations
            List<Camera> oldCameras = Program.Snapshot.Configuration.Cameras;

            string oldTrigger = Program.Snapshot.Configuration.TriggerFilePath;

            // create new configuration for the specified camera
            Program.Snapshot.Configuration = new Configuration()
            {
                TriggerFilePath = triggerPath,
                Regex = regex,
                OutputFolderPath = outputPath,
                OutputValidity = validity,
                FaceDetectionTrigger = faceDetectionTrigger,
                Cameras = oldCameras,
                ServerIP = ip,
                MediaFolderPath = mediaPath,
                ServerPort = port,
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

            // export the configuration locally or to the connected server
            string path = "configuration.json";
            if (Program.Snapshot.Configuration.ServerIP.Length > 9)
            {
                path = Program.Snapshot.Configuration.ServerIP;
                if (Program.Snapshot.Configuration.ServerPort != 0)
                    path += ":" + Program.Snapshot.Configuration.ServerPort;
                if (Program.Snapshot.JSONExport != "")
                    path += "/" + Program.Snapshot.JSONExport;
            }

            // export the configuration in a new thread
            Thread thread = new Thread(() => Configuration.ExportToJSON(path));
            thread.IsBackground = true;
            thread.Start();

            // change the trigger file that is being monitored
            if (oldTrigger != triggerPath && triggerPath.Length > 0)
            {
                Thread newThread = new Thread(() => Program.ChangeTrigger());
                newThread.IsBackground = true;
                newThread.Start();
            }

            // reconfigure all cameras in a separate thread
            Thread threadReconfigure = new Thread(() => Program.ReconfigureAllRecorders());
            threadReconfigure.IsBackground = true;
            threadReconfigure.Start();

            // start thread which will constantly check if faces are present
            if (faceDetectionTrigger)
            {
                var snap = Program.Snapshot;
                Thread faceChecker = new Thread(() => Program.FaceDetectionTrigger(ref snap));
                faceChecker.IsBackground = true;
                faceChecker.Start();
            }

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
            checkBox1.Checked = config.FaceDetectionTrigger;

            // server configuration
            string type = "", ip = "";

            if (config.ServerIP.Length > 0)
            {
                type = config.ServerIP.Substring(0, 7);
                ip = config.ServerIP.Substring(7);

                if (type == "http://")
                    comboBox1.Text = type;
                else
                {
                    comboBox1.Text = "https://";
                    ip = config.ServerIP.Substring(8);
                }
            }
            else
                comboBox1.Text = "https://";

            textBox3.Text = ip;

            var splittedMediaPath = config.MediaFolderPath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            textBox7.Text = string.Join((char)92, splittedMediaPath);
            
            if (config.ServerIP.Length > 0 && config.ServerPort != 0)
                textBox4.Text = config.ServerPort.ToString();
            else
                textBox4.Text = "";
            if (Configuration.ValidateToken())
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
                    textBox11.Text = openFileDialog.FileName;
            }

            toolStripStatusLabel1.Text = "";

            toolTip1.SetToolTip(textBox11, textBox11.Text);
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
                textBox10.Text = folderBrowserDialog1.SelectedPath;

            toolTip1.SetToolTip(textBox10, textBox10.Text);

            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Radio buttons and checkboxes

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

        /// <summary>
        /// Change the availability of trigger file configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            button8.Enabled = checkBox2.Checked;
            textBox9.Enabled = checkBox2.Checked;
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
                // create url from currently entered content
                string url = comboBox1.Text + textBox3.Text;
                if (textBox4.Text.Length > 0)
                    url += ":" + textBox4.Text;

                // check if user already has a valid token
                if (!Configuration.ValidateToken(url))
                {
                    // ask the user to enter activation code
                    ServerConnectionForm popup = new ServerConnectionForm(url);
                    var result = popup.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        // set media folder path
                        var success = SetMediaFolderPath(url, textBox7.Text);

                        label7.Text = "Connected";
                        label7.ForeColor = Color.Green;
                        if (success)
                            toolStripStatusLabel1.Text = "Successfully updated media folder path!";
                    }
                    else
                    {
                        label7.Text = "Disconnected";
                        label7.ForeColor = Color.Red;
                        toolStripStatusLabel1.Text = "Could not connect to server and/or update media folder path!";
                    }
                }
                else
                {
                    // set media folder path
                    var success = SetMediaFolderPath(url, textBox7.Text);

                    label7.Text = "Connected";
                    label7.ForeColor = Color.Green;
                    if (success)
                        toolStripStatusLabel1.Text = "Successfully updated media folder path!";
                }

                // add the computer to device table if it is not already present
                DeviceCheck();

            }
            catch
            {
                label7.Text = "Disconnected";
                label7.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Could not connect to server and/or update media folder path!";
            }
        }

        /// <summary>
        /// Send new media folder path to server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SetMediaFolderPath(string url, string path)
        {
            try
            {
                if (path == null || path == "") path = @"C:\\inetpub\\site\\wwwroot\\UserContent";
                else
                {
                    path = Regex.Replace(path, @"(?<!\\)\\(?!\\)", @"\\");
                }
                HttpWebRequest webRequest;
                string requestParams = Configuration.GetMACAddress() ?? "";
                webRequest = (HttpWebRequest)WebRequest.Create(url + "/api/FileUpload/SetPathForUser" + "/" + requestParams);
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.Accept = "application/json, text/javascript, */*";
                webRequest.Method = "POST";
                using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write("{ \"path\": \"" + path + "\" }");
                }

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    toolStripStatusLabel1.Text = "Could not set media folder path!";
                    return false;
                }

                return true;
            }
            catch
            {
                toolStripStatusLabel1.Text = "Could not set media folder path!";
                return false;
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
            System.Windows.Forms.Timer timerJSON = new System.Windows.Forms.Timer();
            System.Windows.Forms.Timer timerMedia = new System.Windows.Forms.Timer();

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
            if (!Program.SyncsActive[0])
            {
                Program.SyncsActive[0] = true;
                var snapshot = Program.Snapshot;
                Thread JSONSynchronization = new Thread(() => Program.SynchronizeJSON(ref snapshot, 2));
                JSONSynchronization.IsBackground = true;
                JSONSynchronization.Start();
            }
        }

        /// <summary>
        /// Method for manual synchronization of media files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (!Program.SyncsActive[1])
            {
                Program.SyncsActive[1] = true;
                var snapshot = Program.Snapshot;
                Thread mediaSynchronization = new Thread(() => Program.SynchronizeMedia(ref snapshot, 2));
                mediaSynchronization.IsBackground = true;
                mediaSynchronization.Start();
            }
        }

        /// <summary>
        /// Disable the user from changing media file path when using deployed server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text == "siset1.ga")
            {
                textBox7.Text = @"h:\root\home\sigrupa4-001\www\site1\wwwroot\UserContent";
                textBox7.Enabled = false;
            }
            else
            {
                textBox7.Enabled = true;
            }
        }

        #endregion

        #region Device check

        /// <summary>
        /// Adding computer to device list when connecting to server
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void DeviceCheck()
        {
            string baseUrl = comboBox1.Text + textBox3.Text;
            try
            {
                baseUrl += ":" + Int32.Parse(textBox4.Text);
            }
            catch
            {
                // base url does not need a port
            }

            // add to device table if not exists
            HttpWebRequest webRequest;
            string requestParams = "MacAddress=" + Configuration.GetMACAddress() + "&"
                                    + "TerminalID=" + Program.Snapshot.TerminalName;

            webRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "/api/Licence/AddDevice" + "?" + requestParams);

            webRequest.Method = "POST";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Bad request!");
        }

        #endregion
    }
}