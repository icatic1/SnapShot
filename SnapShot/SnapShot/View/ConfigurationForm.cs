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

namespace SnapShot
{
    public partial class ConfigurationForm : Form
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

        public ConfigurationForm()
        {
            InitializeComponent();
            label23.Text = "";
            toolStripStatusLabel1.Text = "";
            panel1.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.None;

            // automatically select USB camera - disable network configuration
            comboBox2.Text = "USB camera";

            // add all available USB camera devices to combo box list
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    comboBox1.Items.Add(device["Caption"].ToString());
                    comboBox1.Text = device["Caption"].ToString();
                }
            }

            comboBox3.Text = "640x480";

            UpdateConfigurationWindow(0);
        }

        private void ConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Menu items

        /// <summary>
        /// Go to licencing form
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
        /// Go to help form
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
            string errorText = "";
            errorProvider1.Clear();

            // device configuration
            DeviceType type = DeviceType.USBCamera;
            if (comboBox2.Text == "IP camera")
                type = DeviceType.IPCamera;

            string deviceName = comboBox1.Text,
                   triggerPath = textBox1.Text,
                   regex = textBox6.Text,
                   outputPath = textBox2.Text;

            // check whether all fields that must have values have been filled
            if (triggerPath == "")
            {
                errorText = "Trigger path must not be empty!";
                errorProvider1.SetError(textBox1, errorText);
                textBox1.BackColor = Color.Red;
            }
            else if (!File.Exists(triggerPath)) {
                errorText = "Trigger file must exist!";
                errorProvider1.SetError(textBox1, errorText);
                textBox1.BackColor = Color.Red;
            }
            else
            {
                textBox1.Text = textBox1.Text.Replace("\\", "/");
                textBox1.BackColor = Color.White;
                errorProvider1.SetError(textBox1, null);
            }

            if (regex == "")
            {
                errorText = "Regex must not be empty!";
                errorProvider1.SetError(textBox6, errorText);
                textBox6.BackColor = Color.Red;
            }
            else
            {
                textBox6.BackColor = Color.White;
                errorProvider1.SetError(textBox6, null);
            }

            if (outputPath == "")
            {
                errorText = "Output path must not be empty!";
                errorProvider1.SetError(textBox2, errorText);
                textBox2.BackColor = Color.Red;
            }
            else if (!Directory.Exists(outputPath)) {
                errorText = "Directory must exist!";
                errorProvider1.SetError(textBox2, errorText);
                textBox2.BackColor = Color.Red;
            }
            else
            {
                textBox2.Text = textBox2.Text.Replace("\\", "/");
                textBox2.BackColor = Color.White;
                errorProvider1.SetError(textBox2, null);
            }

            if (type == DeviceType.USBCamera && deviceName == "")
            {
                errorText = "USB device needs to be selected!";
                errorProvider1.SetError(comboBox1, errorText);
                comboBox1.BackColor = Color.Red;
            }
            else
            {
                comboBox1.BackColor = Color.White;
                errorProvider1.SetError(comboBox1, null);
            }

            int validity = (int)numericUpDown1.Value;
            int cameraNo = (int)comboBox1.SelectedIndex;

            // video configuration
            string res = "Resolution" + comboBox3.Text;
            Resolution resolution = (Resolution)Enum.Parse(typeof(Resolution), res);

            int contrast = trackBar1.Value;
            Color color = pictureBox1.BackColor;
            bool motionDetection = checkBox1.Checked;

            string ip = textBox3.Text,
                   mediaPath = textBox7.Text,
                   JSONconfigPath = textBox5.Text,
                   statusText = label17.Text;

            int syncPeriod = (int)numericUpDown4.Value;
            if (domainUpDown3.Text == "minutes")
                syncPeriod *= 60;
            else if (domainUpDown3.Text == "hours")
                syncPeriod *= 3600;
            else if (domainUpDown3.Text == "days")
                syncPeriod *= 86400;

            bool status = statusText == "Connected" ? true : false;

            int ticks = Program.Snapshot.Camera[cameraNo].LatestSynchronizationTicks;

            int port = 0;

            // server configuration
            if (ip.Length > 0 || mediaPath.Length > 0 || JSONconfigPath.Length > 0 || textBox4.Text.Length > 0)
            {
                try
                {
                    if (textBox4.Text.Length > 0)
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

                if (ip.Length < 1)
                {
                    errorText = "Server configuration IP address needs to be specified!";
                    errorProvider1.SetError(textBox3, errorText);
                    textBox3.BackColor = Color.Red;
                }
                else
                {
                    textBox3.BackColor = Color.White;
                    errorProvider1.SetError(textBox3, null);
                }

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

                if (ip.Length < 1)
                {
                    errorText = "Server JSON configuration path needs to be specified!";
                    errorProvider1.SetError(textBox5, errorText);
                    textBox5.BackColor = Color.Red;
                }
                else
                {
                    textBox5.BackColor = Color.White;
                    errorProvider1.SetError(textBox5, null);
                }
            }

            // capture configuration
            bool image = radioButton4.Checked,
                 single = radioButton1.Checked;

            int duration = (int)numericUpDown2.Value;
            if (domainUpDown1.Text == "minutes")
                duration *= 60;
            else if (domainUpDown1.Text == "hours")
                duration *= 3600;

            int period = (int)numericUpDown3.Value;
            if (domainUpDown2.Text == "minutes")
                period *= 60;
            else if (domainUpDown2.Text == "hours")
                period *= 3600;

            // create configuration for given camera
            int camera = 0;
            if (radioButton6.Checked)
                camera = 1;
            else if (radioButton7.Checked)
                camera = 2;

            // something was not correct - do not allow configuration to be created
            if (errorText.Length > 0)
            {
                toolStripStatusLabel1.Text = "Configuration could not be saved due to errors!";
                return;
            }

            errorProvider1.Clear();

            // create new configuration for the specified camera
            Program.Snapshot.Camera[camera] = new Configuration()
            {
                Type = type,
                Id = deviceName,
                TriggerFilePath = triggerPath,
                Regex = regex,
                OutputFolderPath = outputPath,
                OutputValidity = validity,
                CameraNumber = cameraNo,
                Resolution = resolution,
                ContrastLevel = contrast,
                ImageColor = color,
                MotionDetection = motionDetection,
                ServerIP = ip,
                MediaPath = mediaPath,
                JSONConfigPath = JSONconfigPath,
                ServerPort = port,
                SynchronizationPeriod = syncPeriod,
                LatestSynchronizationTicks = ticks,
                ConnectionStatus = status,
                ImageCapture = image,
                SingleMode = single,
                Duration = duration,
                Period = period
            };

            toolStripStatusLabel1.Text = "Configuration successfully saved!";
        }

        #endregion

        #region JSON export/import

        /// <summary>
        /// Export configuration to JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JSONPopup popup = new JSONPopup();
            var result = popup.ShowDialog();
            if (result == DialogResult.OK)
            {
                bool res = Configuration.ExportToJSON(JSONLocation);
                if (res)
                    toolStripStatusLabel1.Text = "Export successfully completed.";
                else
                    toolStripStatusLabel1.Text = "The export could not be completed successfully.";
            }
        }

        /// <summary>
        /// Import configuration from JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JSONPopup popup = new JSONPopup();
            var result = popup.ShowDialog();
            if (result == DialogResult.OK)
            {
                bool res = Configuration.ImportFromJSON(JSONLocation);
                if (res)
                {
                    radioButton5.Checked = true;
                    UpdateConfigurationWindow(0);
                    toolStripStatusLabel1.Text = "Import successfully completed.";
                }
                else
                    toolStripStatusLabel1.Text = "The import could not be completed successfully. Check JSON file for errors.";
            }
        }

        #endregion

        #region File paths

        /// <summary>
        /// Select the file that contains capture trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog.FileName;
                    errorProvider1.SetError(textBox1, null);
                    textBox1.BackColor = Color.White;
                }
            }

            toolStripStatusLabel1.Text = "";
        }

        /// <summary>
        /// Select the folder where the capture will be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                errorProvider1.SetError(textBox2, null);
                textBox2.BackColor = Color.White;
            }

            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Color palette and radio buttons

        /// <summary>
        /// Show color palette dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                pictureBox1.BackColor = colorDialog1.Color;

            toolStripStatusLabel1.Text = "";
        }

        /// <summary>
        /// Enable/disable burst period selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // burst selected - allow for burst duration and period to be selected
            if (radioButton2.Checked)
            {
                numericUpDown3.Enabled = true;
                numericUpDown3.ReadOnly = false;
                domainUpDown2.Enabled = true;
                domainUpDown2.ReadOnly = false;

                numericUpDown2.Enabled = true;
                numericUpDown2.ReadOnly = false;
                domainUpDown1.Enabled = true;
                domainUpDown1.ReadOnly = false;
            }

            // burst not selected - do not allow burst period to be selected
            else
            {
                numericUpDown3.Enabled = false;
                numericUpDown3.ReadOnly = true;
                domainUpDown2.Enabled = false;
                domainUpDown2.ReadOnly = true;
            }
        }

        /// <summary>
        /// Disable burst when video mode is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            // video is selected - burst not available, duration is available
            if (radioButton3.Checked)
            {
                radioButton1.Checked = true;

                radioButton2.Enabled = false;
                radioButton2.Checked = false;

                numericUpDown3.Enabled = false;
                numericUpDown3.ReadOnly = true;
                domainUpDown2.Enabled = false;
                domainUpDown2.ReadOnly = true;

                numericUpDown2.Enabled = true;
                numericUpDown2.ReadOnly = false;
                domainUpDown1.Enabled = true;
                domainUpDown1.ReadOnly = false;
            }

            // video is not selected - enable burst to be selected
            else
            {
                radioButton2.Enabled = true;

                numericUpDown3.Enabled = true;
                numericUpDown3.ReadOnly = false;
                domainUpDown2.Enabled = true;
                domainUpDown2.ReadOnly = false;
            }
        }

        /// <summary>
        /// Automatically select single when selecting image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                radioButton1.Checked = true;
                numericUpDown3.Enabled = false;
                numericUpDown3.ReadOnly = true;
                domainUpDown2.Enabled = false;
                domainUpDown2.ReadOnly = true;

                numericUpDown2.Enabled = false;
                numericUpDown2.ReadOnly = true;
                domainUpDown1.Enabled = false;
                domainUpDown1.ReadOnly = true;
            }
        }

        /// <summary>
        /// Automatically disable duration when single image is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // single image selected - disable duration
            if (radioButton4.Checked && radioButton1.Checked)
            {
                numericUpDown2.Enabled = false;
                numericUpDown2.ReadOnly = true;
                domainUpDown1.Enabled = false;
                domainUpDown1.ReadOnly = true;
            }
            // any other combination - enable duration
            else
            {
                numericUpDown2.Enabled = true;
                numericUpDown2.ReadOnly = false;
                domainUpDown1.Enabled = true;
                domainUpDown1.ReadOnly = false;
            }
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

        #region Camera preview

        /// <summary>
        /// Enter new form which shows camera input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            CapturePreviewForm f;
            if (comboBox2.Text == "USB camera")
                f = new CapturePreviewForm(comboBox1.Text, comboBox1.SelectedIndex, "USB");
            else
            {
                int port;
                try
                {
                    port = Int32.Parse(textBox4.Text);
                    f = new CapturePreviewForm(textBox3.Text, port, "IP");
                }
                catch
                {
                    toolStripStatusLabel1.Text = "Server port must be a valid number!";
                    return;
                }
            }

            f.Show();
        }

        #endregion

        #region Radio buttons (devices)

        /// <summary>
        /// Camera 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
                UpdateConfigurationWindow(0);
        }

        /// <summary>
        /// Camera 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
                UpdateConfigurationWindow(1);
        }

        /// <summary>
        /// Camera 3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
                UpdateConfigurationWindow(2);
        }

        /// <summary>
        /// Helper method for changing configuration for different camera
        /// </summary>
        /// <param name="cameraNumber"></param>
        public void UpdateConfigurationWindow(int cameraNumber)
        {
            var config = Program.Snapshot.Camera[cameraNumber];

            comboBox2.Text = config.Type.ToString();
            comboBox1.Text = config.Id;
            textBox1.Text = config.TriggerFilePath;
            textBox6.Text = config.Regex;
            textBox2.Text = config.OutputFolderPath;
            if (config.OutputValidity > 0)
                numericUpDown1.Value = config.OutputValidity;
            else
                numericUpDown1.Value = 1;

            comboBox3.Text = config.Resolution.ToString();
            trackBar1.Value = config.ContrastLevel;
            pictureBox1.BackColor = config.ImageColor;
            checkBox1.Checked = config.MotionDetection;

            textBox3.Text = config.ServerIP;
            textBox7.Text = config.MediaPath;
            textBox5.Text = config.JSONConfigPath;
            if (config.ServerIP.Length > 0 && config.ServerPort != 0)
                textBox4.Text = config.ServerPort.ToString();
            else
                textBox4.Text = "";

            string unit = "seconds";
            double time = config.SynchronizationPeriod;
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
                numericUpDown4.Value = (int)time;
                domainUpDown3.Text = unit;
            }
            else
            {
                numericUpDown4.Value = 1;
                domainUpDown3.Text = "seconds";
            }

            if (config.ConnectionStatus)
            {
                numericUpDown4.Enabled = true;
                numericUpDown4.ReadOnly = false;
                domainUpDown3.Enabled = true;
                domainUpDown3.ReadOnly = false;
                label17.Text = "Connected";
                label17.ForeColor = Color.Green;
            }
            else
            {
                numericUpDown4.Enabled = false;
                numericUpDown4.ReadOnly = true;
                domainUpDown3.Enabled = false;
                domainUpDown3.ReadOnly = true;
                label17.Text = "Disconnected";
                label17.ForeColor = Color.Red;
            }

            radioButton4.Checked = config.ImageCapture;
            radioButton3.Checked = !config.ImageCapture;
            radioButton1.Checked = config.SingleMode;
            radioButton2.Checked = !config.SingleMode;

            numericUpDown3.Enabled = !config.SingleMode;
            numericUpDown3.ReadOnly = config.SingleMode;
            domainUpDown2.Enabled = !config.SingleMode;
            domainUpDown2.ReadOnly = config.SingleMode;

            unit = "seconds";
            time = config.Duration;
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
                numericUpDown2.Value = (int)time;
                domainUpDown1.Text = unit;
            }
            else
            {
                numericUpDown2.Value = 1;
                domainUpDown1.Text = "seconds";
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
                numericUpDown3.Value = (int)time;
                domainUpDown2.Text = unit;
            }
            else
            {
                numericUpDown3.Value = 1;
                domainUpDown2.Text = "seconds";
            }

            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Server connection

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string url = textBox3.Text;
                
                var ping = new Ping();
                var reply = ping.Send(url, 1000);

                label17.Text = "Connected";
                label17.ForeColor = System.Drawing.Color.Green;

                numericUpDown4.ReadOnly = false;
                numericUpDown4.Enabled = true;
                domainUpDown3.ReadOnly = false;
                domainUpDown3.Enabled = true;
            }
            catch
            {
                label17.Text = "Disconnected";
                label17.ForeColor = System.Drawing.Color.Red;

                numericUpDown4.ReadOnly = true;
                numericUpDown4.Enabled = false;
                domainUpDown3.ReadOnly = true;
                domainUpDown3.Enabled = false;
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
                label23.Text = "Synchronization successful.";
            else if (firstCheck && UpdateLabel)
                label23.Text = "Synchronization failed.";
            else
                label23.Text = "";

            if (UpdateLabel)
                UpdateLabel = false;

            if (RefreshNeeded)
            {
                RefreshNeeded = false;
                UpdateConfigurationWindow(0);
            }
        }

        #endregion
    }
}
