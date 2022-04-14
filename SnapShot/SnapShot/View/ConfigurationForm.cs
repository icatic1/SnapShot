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
using SnapShot.Utilities;

namespace SnapShot
{
    public partial class ConfigurationForm : Form
    {

        #region Attributes
        private Rectangle formSizeOriginal;
        private Rectangle configurationDeviceLabelOriginal;
        private Rectangle camera1RadioOriginal;
        private Rectangle camera2RadioOriginal;
        private Rectangle camera3RadioOriginal;
        private Rectangle deviceConfigOriginal;
        private Rectangle deviceTypeLabelOriginal;
        private Rectangle deviceTypeDropboxOriginal;
        private Rectangle deviceLabelOriginal;
        private Rectangle deviceDropboxOriginal;
        private Rectangle triggerLabelOriginal;
        private Rectangle triggerInputOriginal;
        private Rectangle triggerButtonOriginal;
        private Rectangle outputLabelOriginal;
        private Rectangle outputInputOriginal;
        private Rectangle outputButtonOriginal;
        private Rectangle keepLabelOriginal;
        private Rectangle keepPickerOriginal;
        private Rectangle keepLabel2Original;
        private Rectangle networkConfigOriginal;
        private Rectangle serverVersionLabelOriginal;
        private Rectangle serverVersionInputOriginal;
        private Rectangle serverIPLabelOriginal;
        private Rectangle serverIPInputOriginal;
        private Rectangle serverPortLabelOriginal;
        private Rectangle serverPortInputOriginal;
        private Rectangle checkConnectionButtonOriginal;
        private Rectangle statusLabelOriginal;
        private Rectangle statusOriginal;
        private Rectangle videoConfigOriginal;
        private Rectangle resolutionLabelOriginal;
        private Rectangle resolutionDropboxOriginal;
        private Rectangle contrastLabelOriginal;
        private Rectangle contrastPickerOriginal;
        private Rectangle colorLabelOriginal;
        private Rectangle colorOriginal;
        private Rectangle colorButtonOriginal;
        private Rectangle motionCheckboxOriginal;
        private Rectangle previewButtonOriginal;
        private Rectangle captureConfigOriginal;
        private Rectangle typeCaptureLabelOriginal;
        private Rectangle typeCapturePanel1Original;
        private Rectangle modeLabelOriginal;
        private Rectangle imageRadioOriginal;
        private Rectangle videoRadioOriginal;
        private Rectangle typeCapturePanel2Original;
        private Rectangle singleRadioOriginal;
        private Rectangle burstRadioOriginal;
        private Rectangle durationLabelOriginal;
        private Rectangle durationPicker1Original;
        private Rectangle durationPicker2Original;
        private Rectangle burstLabelOriginal;
        private Rectangle burstPicker1Original;
        private Rectangle burstPicker2Original;
        private Rectangle saveButtonOriginal;
        private Rectangle triggerRegexLabelOriginal;
        private Rectangle triggerRegexInputOriginal;

        #endregion

        #region Constructor

        public ConfigurationForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
            panel1.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.None;

            // automatically select USB camera - disable network configuration
            comboBox2.Text = "USB camera";
            textBox5.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;

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
            else
            {
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
            else
            {
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

            string version = textBox5.Text,
                       ip = textBox3.Text,
                       statusText = label17.Text;

            bool status = statusText == "Connected!" ? true : false;

            int port = 0;

            // IP camera server configuration
            try
            {
                if (type == DeviceType.IPCamera)
                {
                    port = Int32.Parse(textBox4.Text);
                }
                textBox4.BackColor = Color.White;
                errorProvider1.SetError(textBox4, null);
            }
            catch
            {
                errorText = "Server port must be a valid number!";
                errorProvider1.SetError(textBox4, errorText);
                textBox4.BackColor = Color.Red;
            }

            // check whether all fields that must have values have been filled
            if (type == DeviceType.IPCamera && ip.Length < 1)
            {
                errorText = "IP camera server configurations need to be specified!";
                errorProvider1.SetError(textBox3, errorText);
                textBox3.BackColor = Color.Red;
            }
            else
            {
                textBox3.BackColor = Color.White;
                errorProvider1.SetError(textBox3, null);
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
                            toolStripStatusLabel1.Text = "Import successfully completed.";
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
                    toolStripStatusLabel1.Text = "Import successfully completed.";
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

        #region IP camera

        /// <summary>
        /// Change available items on form depending on the selected device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // USB camera selected - disable server configuration
            // and clear error provider
            if (comboBox2.SelectedItem.ToString() == "USB camera")
            {
                errorProvider1.SetError(textBox5, null);
                errorProvider1.SetError(textBox3, null);
                errorProvider1.SetError(textBox4, null);

                textBox5.BackColor = Color.White;
                textBox3.BackColor = Color.White;
                textBox4.BackColor = Color.White;

                textBox5.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                textBox5.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;

                comboBox1.Enabled = true;
            }

            // IP camera selected - disable USB device selection
            // and enable server configurations
            else
            {
                comboBox1.SelectedItem = null;
                comboBox1.Enabled = false;

                textBox5.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox4.ReadOnly = false;
                textBox5.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }
        }

        #endregion

        #region Resize methods
        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            formSizeOriginal = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            configurationDeviceLabelOriginal = new Rectangle(label19.Location.X, label19.Location.Y, label19.Size.Width, label19.Size.Height);
            camera1RadioOriginal = new Rectangle(radioButton5.Location.X, radioButton5.Location.Y, radioButton5.Size.Width, radioButton5.Size.Height);
            camera2RadioOriginal = new Rectangle(radioButton6.Location.X, radioButton6.Location.Y, radioButton6.Size.Width, radioButton6.Size.Height);
            camera3RadioOriginal = new Rectangle(radioButton7.Location.X, radioButton7.Location.Y, radioButton7.Size.Width, radioButton7.Size.Height);
            deviceConfigOriginal = new Rectangle(groupBox1.Location.X, groupBox1.Location.Y, groupBox1.Size.Width, groupBox1.Size.Height);
            deviceTypeLabelOriginal = new Rectangle(label5.Location.X, label5.Location.Y, label5.Size.Width, label5.Size.Height);
            deviceTypeDropboxOriginal = new Rectangle(comboBox2.Location.X, comboBox2.Location.Y, comboBox2.Size.Width, comboBox2.Size.Height);
            deviceLabelOriginal = new Rectangle(label1.Location.X, label1.Location.Y, label1.Size.Width, label1.Size.Height);
            deviceDropboxOriginal = new Rectangle(comboBox1.Location.X, comboBox1.Location.Y, comboBox1.Size.Width, comboBox1.Size.Height);
            triggerLabelOriginal = new Rectangle(label2.Location.X, label2.Location.Y, label2.Size.Width, label2.Size.Height);
            triggerInputOriginal = new Rectangle(textBox1.Location.X, textBox1.Location.Y, textBox1.Size.Width, textBox1.Size.Height);
            triggerButtonOriginal = new Rectangle(button1.Location.X, button1.Location.Y, button1.Size.Width, button1.Size.Height);
            outputLabelOriginal = new Rectangle(label3.Location.X, label3.Location.Y, label3.Size.Width, label3.Size.Height);
            outputInputOriginal = new Rectangle(textBox2.Location.X, textBox2.Location.Y, textBox2.Size.Width, textBox2.Size.Height);
            outputButtonOriginal = new Rectangle(button3.Location.X, button3.Location.Y, button3.Size.Width, button3.Size.Height);
            keepLabelOriginal = new Rectangle(label11.Location.X, label11.Location.Y, label11.Size.Width, label11.Size.Height);
            keepLabel2Original = new Rectangle(label12.Location.X, label12.Location.Y, label12.Size.Width, label12.Size.Height);
            keepPickerOriginal = new Rectangle(numericUpDown1.Location.X, numericUpDown1.Location.Y, numericUpDown1.Size.Width, numericUpDown1.Size.Height);
            networkConfigOriginal = new Rectangle(groupBox3.Location.X, groupBox3.Location.Y, groupBox3.Size.Width, groupBox3.Size.Height);
            serverVersionLabelOriginal = new Rectangle(label10.Location.X, label10.Location.Y, label10.Size.Width, label10.Size.Height);
            serverVersionInputOriginal = new Rectangle(textBox5.Location.X, textBox5.Location.Y, textBox5.Size.Width, textBox5.Size.Height);
            serverIPLabelOriginal = new Rectangle(label4.Location.X, label4.Location.Y, label4.Size.Width, label4.Size.Height);
            serverIPInputOriginal = new Rectangle(textBox3.Location.X, textBox3.Location.Y, textBox3.Size.Width, textBox3.Size.Height);
            serverPortLabelOriginal = new Rectangle(label9.Location.X, label9.Location.Y, label9.Size.Width, label9.Size.Height);
            serverPortInputOriginal = new Rectangle(textBox4.Location.X, textBox4.Location.Y, textBox4.Size.Width, textBox4.Size.Height);
            statusLabelOriginal = new Rectangle(label16.Location.X, label16.Location.Y, label16.Size.Width, label16.Size.Height);
            statusOriginal = new Rectangle(label17.Location.X, label17.Location.Y, label17.Size.Width, label17.Size.Height);
            checkConnectionButtonOriginal = new Rectangle(button2.Location.X, button2.Location.Y, button2.Size.Width, button2.Size.Height);
            videoConfigOriginal = new Rectangle(groupBox2.Location.X, groupBox2.Location.Y, groupBox2.Size.Width, groupBox2.Size.Height);
            resolutionLabelOriginal = new Rectangle(label6.Location.X, label6.Location.Y, label6.Size.Width, label6.Size.Height);
            resolutionDropboxOriginal = new Rectangle(comboBox3.Location.X, comboBox3.Location.Y, comboBox3.Size.Width, comboBox3.Size.Height);
            contrastLabelOriginal = new Rectangle(label7.Location.X, label7.Location.Y, label7.Size.Width, label7.Size.Height);
            contrastPickerOriginal = new Rectangle(trackBar1.Location.X, trackBar1.Location.Y, trackBar1.Size.Width, trackBar1.Size.Height);
            colorLabelOriginal = new Rectangle(label8.Location.X, label8.Location.Y, label8.Size.Width, label8.Size.Height);
            colorOriginal = new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Size.Width, pictureBox1.Size.Height);
            colorButtonOriginal = new Rectangle(button4.Location.X, button4.Location.Y, button4.Size.Width, button4.Size.Height);
            motionCheckboxOriginal = new Rectangle(checkBox1.Location.X, checkBox1.Location.Y, checkBox1.Size.Width, checkBox1.Size.Height);
            previewButtonOriginal = new Rectangle(button5.Location.X, button5.Location.Y, button5.Size.Width, button5.Size.Height);
            captureConfigOriginal = new Rectangle(groupBox4.Location.X, groupBox4.Location.Y, groupBox4.Size.Width, groupBox4.Size.Height);
            typeCaptureLabelOriginal = new Rectangle(label13.Location.X, label13.Location.Y, label13.Size.Width, label13.Size.Height);
            imageRadioOriginal = new Rectangle(radioButton4.Location.X, radioButton4.Location.Y, radioButton4.Size.Width, radioButton4.Size.Height);
            videoRadioOriginal = new Rectangle(radioButton3.Location.X, radioButton3.Location.Y, radioButton3.Size.Width, radioButton3.Size.Height);
            singleRadioOriginal = new Rectangle(radioButton1.Location.X, radioButton1.Location.Y, radioButton1.Size.Width, radioButton1.Size.Height);
            burstRadioOriginal = new Rectangle(radioButton2.Location.X, radioButton2.Location.Y, radioButton2.Size.Width, radioButton2.Size.Height);
            typeCapturePanel1Original = new Rectangle(panel1.Location.X, panel1.Location.Y, panel1.Size.Width, panel1.Size.Height);
            typeCapturePanel2Original = new Rectangle(panel2.Location.X, panel2.Location.Y, panel2.Size.Width, panel2.Size.Height);
            modeLabelOriginal = new Rectangle(label15.Location.X, label15.Location.Y, label15.Size.Width, label15.Size.Height);
            durationLabelOriginal = new Rectangle(label14.Location.X, label14.Location.Y, label14.Size.Width, label14.Size.Height);
            durationPicker1Original = new Rectangle(numericUpDown2.Location.X, numericUpDown2.Location.Y, numericUpDown2.Size.Width, numericUpDown2.Size.Height);
            durationPicker2Original = new Rectangle(domainUpDown1.Location.X, domainUpDown1.Location.Y, domainUpDown1.Size.Width, domainUpDown1.Size.Height);
            burstLabelOriginal = new Rectangle(label18.Location.X, label18.Location.Y, label18.Size.Width, label18.Size.Height);
            burstPicker1Original = new Rectangle(numericUpDown3.Location.X, numericUpDown3.Location.Y, numericUpDown3.Size.Width, numericUpDown3.Size.Height);
            burstPicker2Original = new Rectangle(domainUpDown2.Location.X, domainUpDown2.Location.Y, domainUpDown2.Size.Width, domainUpDown2.Size.Height);
            saveButtonOriginal = new Rectangle(button6.Location.X, button6.Location.Y, button6.Size.Width, button6.Size.Height);
            triggerRegexLabelOriginal = new Rectangle(label20.Location.X, label20.Location.Y, label20.Size.Width, label20.Size.Height);
            triggerRegexInputOriginal = new Rectangle(textBox6.Location.X, textBox6.Location.Y, textBox6.Size.Width, textBox6.Size.Height);
        }

        private void ConfigurationForm_Resize(object sender, EventArgs e)
        {
            ResizeUtil.resizeControl(this, formSizeOriginal,configurationDeviceLabelOriginal, label19);
            ResizeUtil.resizeControl(this, formSizeOriginal, camera1RadioOriginal, radioButton5);
            ResizeUtil.resizeControl(this, formSizeOriginal, camera2RadioOriginal, radioButton6);
            ResizeUtil.resizeControl(this, formSizeOriginal, camera3RadioOriginal, radioButton7);
            ResizeUtil.resizeControl(this, formSizeOriginal, deviceConfigOriginal, groupBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, deviceTypeLabelOriginal, label5);
            ResizeUtil.resizeControl(this, formSizeOriginal, deviceTypeDropboxOriginal, comboBox2);
            ResizeUtil.resizeControl(this, formSizeOriginal, deviceLabelOriginal, label1);
            ResizeUtil.resizeControl(this, formSizeOriginal, deviceDropboxOriginal, comboBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, triggerLabelOriginal, label2);
            ResizeUtil.resizeControl(this, formSizeOriginal, triggerInputOriginal, textBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, triggerButtonOriginal, button1);
            ResizeUtil.resizeControl(this, formSizeOriginal, outputLabelOriginal, label3);
            ResizeUtil.resizeControl(this, formSizeOriginal, outputInputOriginal, textBox2);
            ResizeUtil.resizeControl(this, formSizeOriginal, outputButtonOriginal, button3);
            ResizeUtil.resizeControl(this, formSizeOriginal, keepLabelOriginal, label11);
            ResizeUtil.resizeControl(this, formSizeOriginal, keepPickerOriginal, numericUpDown1);
            ResizeUtil.resizeControl(this, formSizeOriginal, outputButtonOriginal, button3);
            ResizeUtil.resizeControl(this, formSizeOriginal, keepLabel2Original, label12);
            ResizeUtil.resizeControl(this, formSizeOriginal, networkConfigOriginal, groupBox3);
            ResizeUtil.resizeControl(this, formSizeOriginal, serverVersionLabelOriginal, label10);
            ResizeUtil.resizeControl(this, formSizeOriginal, serverVersionInputOriginal, textBox5);
            ResizeUtil.resizeControl(this, formSizeOriginal, serverIPLabelOriginal, label4);
            ResizeUtil.resizeControl(this, formSizeOriginal, serverIPInputOriginal, textBox3);
            ResizeUtil.resizeControl(this, formSizeOriginal, serverPortLabelOriginal, label9);
            ResizeUtil.resizeControl(this, formSizeOriginal, serverPortInputOriginal, textBox4);
            ResizeUtil.resizeControl(this, formSizeOriginal, checkConnectionButtonOriginal, button2);
            ResizeUtil.resizeControl(this, formSizeOriginal, statusLabelOriginal, label16);
            ResizeUtil.resizeControl(this, formSizeOriginal, statusOriginal, label17);
            ResizeUtil.resizeControl(this, formSizeOriginal, videoConfigOriginal, groupBox2);
            ResizeUtil.resizeControl(this, formSizeOriginal, resolutionLabelOriginal, label6);
            ResizeUtil.resizeControl(this, formSizeOriginal, resolutionDropboxOriginal, comboBox3);
            ResizeUtil.resizeControl(this, formSizeOriginal, contrastLabelOriginal, label7);
            ResizeUtil.resizeControl(this, formSizeOriginal, contrastPickerOriginal, trackBar1);
            ResizeUtil.resizeControl(this, formSizeOriginal, colorLabelOriginal, label8);
            ResizeUtil.resizeControl(this, formSizeOriginal, colorOriginal, pictureBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, colorButtonOriginal, button4);
            ResizeUtil.resizeControl(this, formSizeOriginal, previewButtonOriginal, button5);
            ResizeUtil.resizeControl(this, formSizeOriginal, motionCheckboxOriginal, checkBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, captureConfigOriginal, groupBox4);
            ResizeUtil.resizeControl(this, formSizeOriginal, typeCaptureLabelOriginal, label13);
            ResizeUtil.resizeControl(this, formSizeOriginal, typeCapturePanel1Original, panel1);
            ResizeUtil.resizeControl(this, formSizeOriginal, typeCapturePanel2Original, panel2);
            ResizeUtil.resizeControl(this, formSizeOriginal, imageRadioOriginal, radioButton4);
            ResizeUtil.resizeControl(this, formSizeOriginal, videoRadioOriginal, radioButton3);
            ResizeUtil.resizeControl(this, formSizeOriginal, singleRadioOriginal, radioButton1);
            ResizeUtil.resizeControl(this, formSizeOriginal, burstRadioOriginal, radioButton2);
            ResizeUtil.resizeControl(this, formSizeOriginal, modeLabelOriginal, label15);
            ResizeUtil.resizeControl(this, formSizeOriginal, durationLabelOriginal, label14);
            ResizeUtil.resizeControl(this, formSizeOriginal, durationPicker1Original, numericUpDown2);
            ResizeUtil.resizeControl(this, formSizeOriginal, durationPicker2Original, domainUpDown1);
            ResizeUtil.resizeControl(this, formSizeOriginal, burstLabelOriginal, label18);
            ResizeUtil.resizeControl(this, formSizeOriginal, burstPicker1Original, numericUpDown3);
            ResizeUtil.resizeControl(this, formSizeOriginal, burstPicker2Original, domainUpDown2);
            ResizeUtil.resizeControl(this, formSizeOriginal, saveButtonOriginal, button6);
            ResizeUtil.resizeControl(this, formSizeOriginal, triggerRegexInputOriginal, textBox6);
            ResizeUtil.resizeControl(this, formSizeOriginal, triggerRegexLabelOriginal, label20);

            ResizeUtil.resizeFontTypes(this,menuStrip1, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label19, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton5, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton6, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton7, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, groupBox1, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label5, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label1, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label2, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label3, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label11, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label12, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, groupBox3, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label10, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label4, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label9, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label16, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label17, "Normal", formSizeOriginal, 12);
            ResizeUtil.resizeFontTypes(this, groupBox2, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label6, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label7, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label8, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, checkBox1, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, groupBox4, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label13, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label15, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label14, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label18, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label20, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton4, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton3, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton2, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, radioButton1, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, button6, "Inside", saveButtonOriginal, 9);
            ResizeUtil.resizeFontTypes(this, button4, "Inside", colorButtonOriginal, 9);
            ResizeUtil.resizeFontTypes(this, button5, "Inside", previewButtonOriginal, 9);
            ResizeUtil.resizeFontTypes(this, button2, "Inside", checkConnectionButtonOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox6, "Inside", triggerRegexInputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox1, "Inside", triggerInputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox2, "Inside", outputInputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox5, "Inside", serverVersionInputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox3, "Inside", serverIPInputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox2, "Inside", serverPortInputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, comboBox2, "Inside", deviceTypeDropboxOriginal, 9);
            ResizeUtil.resizeFontTypes(this, comboBox1, "Inside", deviceDropboxOriginal, 9);
            ResizeUtil.resizeFontTypes(this, comboBox3, "Inside", resolutionDropboxOriginal, 9);
        }

        #endregion

        #region Dropdown Color
        
        private void postavkeToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            menuStrip1.Items[0].ForeColor = Color.White;
        }

        private void postavkeToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            menuStrip1.Items[0].ForeColor = SystemColors.ControlText;
        }

        private void postavkeToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            menuStrip1.Items[0].ForeColor = SystemColors.ControlText;
        }


        #endregion

    }
}
