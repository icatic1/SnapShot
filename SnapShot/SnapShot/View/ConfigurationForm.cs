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

namespace SnapShot
{
    public partial class ConfigurationForm : Form
    {
        #region Constructor

        public ConfigurationForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
            comboBox2.Text = "USB camera";
            panel1.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.None;

            // add all available camera devices to collection
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    comboBox1.Items.Add(device["Caption"].ToString());
                    comboBox1.Text = device["Caption"].ToString();
                }
            }

            comboBox3.Text = "1280x1024";

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
            this.Hide();
            LicencingForm f = new LicencingForm();
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
            this.Hide();
            InformationForm f = new InformationForm();
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

            // device configuration
            DeviceType type = DeviceType.USBCamera;
            if (comboBox2.Text == "IP camera")
                type = DeviceType.IPCamera;
            else if (comboBox2.Text != "USB camera")
                errorText = "You need to select device type!";

            string deviceName = comboBox1.Text,
                   triggerPath = textBox1.Text,
                   regex = textBox6.Text,
                   outputPath = textBox2.Text;
            if (deviceName == "" || triggerPath == "" || regex == "" || outputPath == "")
                errorText = "Empty values found in configuration options!";

            int validity = (int)numericUpDown1.Value;
            int cameraNo = (int)comboBox1.SelectedIndex;

            // video configuration
            Resolution resolution = Resolution.Resolution720x480;
            if (comboBox3.Text == "1280x960")
                resolution = Resolution.Resolution1280x960;
            if (comboBox3.Text == "1280x1024")
                resolution = Resolution.Resolution1280x1024;
            else if (comboBox3.Text == "1920X1080")
                resolution = Resolution.Resolution1920x1080;
            else if (comboBox3.Text == "2048x1536")
                resolution = Resolution.Resolution2048x1536;
            else if (comboBox3.Text != "720x480")
                errorText = "No capture device resolution selected!";

            int contrast = trackBar1.Value;
            Color color = pictureBox1.BackColor;
            bool motionDetection = checkBox1.Checked;

            // network configuration
            string version = textBox5.Text,
                   ip = textBox3.Text,
                   statusText = label17.Text;

            bool status = statusText == "Connected!" ? true : false;

            int port = 0;

            try
            {
                port = Int32.Parse(textBox4.Text);
            }
            catch
            {
                if (textBox4.Text.Length > 0)
                    errorText = "Server port must be a valid number!";
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
                toolStripStatusLabel1.Text = errorText;
                return;
            }

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
                ServerVersion = version,
                ServerIP = ip,
                ServerPort = port,
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
            int mode = 1;
            if (Program.Snapshot.Connected)
                mode = 2;

            if (mode == 1)
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.FileName = "configuration.json";
                    openFileDialog.Filter = "JSON files (*.json)|*.json";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.CheckFileExists = false;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var result = Configuration.ExportToJSON(openFileDialog.FileName, mode);
                        if (result)
                            toolStripStatusLabel1.Text = "Export successfully completed.";
                        else
                            toolStripStatusLabel1.Text = "The export could not be completed successfully.";
                    }
                }
            else
            {
                var result = Configuration.ExportToJSON("", mode);
                if (result)
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
            int mode = 1;
            if (Program.Snapshot.Connected)
                mode = 2;

            if (mode == 1)
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.FileName = "configuration.json";
                    openFileDialog.Filter = "JSON files (*.json)|*.json";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        bool result = Configuration.ImportFromJSON(openFileDialog.FileName, mode);
                        if (result)
                        {
                            radioButton5.Checked = true;
                            UpdateConfigurationWindow(0);
                        }
                        else
                        {
                            toolStripStatusLabel1.Text = "The import could not be completed successfully. Check JSON file for errors.";
                        }
                    }
                }
            else
            {
                bool result = Configuration.ImportFromJSON("", mode);
                if (result)
                {
                    radioButton5.Checked = true;
                    UpdateConfigurationWindow(0);
                }
                else
                {
                    toolStripStatusLabel1.Text = "The import could not be completed successfully. Check JSON file for errors.";
                }
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
            CapturePreviewForm f = new CapturePreviewForm(comboBox1.Text, comboBox1.SelectedIndex);
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
            textBox2.Text = config.OutputFolderPath;
            if (config.OutputValidity > 0)
                numericUpDown1.Value = config.OutputValidity;
            else
                numericUpDown1.Value = 1;

            comboBox3.Text = config.Resolution.ToString();
            trackBar1.Value = config.ContrastLevel;
            pictureBox1.BackColor = config.ImageColor;
            checkBox1.Checked = config.MotionDetection;

            textBox5.Text = config.ServerVersion;
            textBox3.Text = config.ServerIP;
            if (config.ServerIP.Length > 0)
                textBox4.Text = config.ServerPort.ToString();
            else
                textBox4.Text = "";

            radioButton4.Checked = config.ImageCapture;
            radioButton3.Checked = !config.ImageCapture;
            radioButton1.Checked = config.SingleMode;
            radioButton2.Checked = !config.SingleMode;

            numericUpDown3.Enabled = !config.SingleMode;
            numericUpDown3.ReadOnly = config.SingleMode;
            domainUpDown2.Enabled = !config.SingleMode;
            domainUpDown2.ReadOnly = config.SingleMode;

            string unit = "seconds";
            double time = config.Duration;
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
    }
}
