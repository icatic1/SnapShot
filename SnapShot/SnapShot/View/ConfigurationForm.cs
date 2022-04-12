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
                   outputPath = textBox2.Text;
            if (deviceName == "" || triggerPath == "" || outputPath == "")
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
            try
            {
                string EXPORT = "";
                EXPORT += "{\n";
                EXPORT += "\t\"cameras\":\n";
                EXPORT += "\t[\n";
                int i = 0;
                foreach (var config in Program.Snapshot.Camera)
                {
                    EXPORT += "\t\t{\n";

                    EXPORT += "\t\t\t\"device_configuration\":\n";
                    EXPORT += "\t\t\t{\n";
                    EXPORT += "\t\t\t\t\"device_type\": \"" + config.Type + "\",\n";
                    EXPORT += "\t\t\t\t\"id\": \"" + config.Id + "\",\n";
                    EXPORT += "\t\t\t\t\"trigger_file_path\": \"" + config.TriggerFilePath + "\",\n";
                    EXPORT += "\t\t\t\t\"output_folder_path\": \"" + config.OutputFolderPath + "\",\n";
                    EXPORT += "\t\t\t\t\"output_validity_days\": \"" + config.OutputValidity + "\",\n";
                    EXPORT += "\t\t\t\t\"camera_number\": \"" + config.CameraNumber + "\"\n";
                    EXPORT += "\t\t\t},\n";

                    EXPORT += "\t\t\t\"video_configuration\":\n";
                    EXPORT += "\t\t\t{\n";
                    EXPORT += "\t\t\t\t\"resolution\": \"" + config.Resolution + "\",\n";
                    EXPORT += "\t\t\t\t\"contrast_level\": \"" + config.ContrastLevel + "\",\n";
                    EXPORT += "\t\t\t\t\"image_color\": \"" + config.ImageColor.Name.ToString() + "\",\n";
                    EXPORT += "\t\t\t\t\"motion_detection\": \"" + config.MotionDetection + "\"\n";
                    EXPORT += "\t\t\t},\n";

                    EXPORT += "\t\t\t\"network_configuration\":\n";
                    EXPORT += "\t\t\t{\n";
                    EXPORT += "\t\t\t\t\"server_version\": \"" + config.ServerVersion + "\",\n";
                    EXPORT += "\t\t\t\t\"server_IP_address\": \"" + config.ServerIP + "\",\n";
                    EXPORT += "\t\t\t\t\"server_port\": \"" + config.ServerPort + "\",\n";
                    EXPORT += "\t\t\t\t\"connection_status\": \"" + config.ConnectionStatus + "\"\n";
                    EXPORT += "\t\t\t},\n";

                    EXPORT += "\t\t\t\"capture_configuration\":\n";
                    EXPORT += "\t\t\t{\n";
                    EXPORT += "\t\t\t\t\"image_capture\": \"" + config.ImageCapture + "\",\n";
                    EXPORT += "\t\t\t\t\"single_mode\": \"" + config.SingleMode + "\",\n";
                    EXPORT += "\t\t\t\t\"duration\": \"" + config.Duration + "\",\n";
                    EXPORT += "\t\t\t\t\"burst_period\": \"" + config.Period + "\"\n";
                    EXPORT += "\t\t\t}\n";

                    EXPORT += "\t\t}";
                    if (i < 2)
                        EXPORT += ",";
                    EXPORT += "\n";
                    i++;
                }
                EXPORT += "\t]\n";
                EXPORT += "}";

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
                        File.WriteAllText(openFileDialog.FileName, EXPORT);
                        toolStripStatusLabel1.Text = "Export successfully completed.";
                    }
                }
            }
            catch
            {
                toolStripStatusLabel1.Text = "The export could not be completed successfully.";            }

        }

        /// <summary>
        /// Import configuration from JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
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
                        string IMPORT = File.ReadAllText(openFileDialog.FileName);
                        string[] rows = IMPORT.Split('\n');
                        Snapshot newSnapshot = new Snapshot();
                        int camera = 0;
                        int i = 4;
                        while (camera < 3)
                        {
                            Configuration config = new Configuration();
                            if (rows[i].Contains("device_configuration"))
                            {
                                i += 2;

                                string[] device_type = rows[i].Split(" ");
                                device_type[1] = device_type[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] id = rows[i].Split(" ");
                                id[1] = id[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] trigger_file_path = rows[i].Split(" ");
                                trigger_file_path[1] = trigger_file_path[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] output_folder_path = rows[i].Split(" ");
                                output_folder_path[1] = output_folder_path[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] output_validity_days = rows[i].Split(" ");
                                output_validity_days[1] = output_validity_days[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] camera_number = rows[i].Split(" ");
                                camera_number[1] = camera_number[1].Replace("\"", "").Replace(",", "");

                                config.Type = (DeviceType)Enum.Parse(typeof(DeviceType), device_type[1]);
                                config.Id = id[1];
                                config.TriggerFilePath = trigger_file_path[1];
                                config.OutputFolderPath = output_folder_path[1];
                                config.OutputValidity = Int32.Parse(output_validity_days[1]);
                                config.CameraNumber = Int32.Parse(camera_number[1]);
                            }

                            i += 2;
                            if (rows[i].Contains("video_configuration"))
                            {
                                i += 2;

                                string[] resolution = rows[i].Split(" ");
                                resolution[1] = resolution[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] contrast_level = rows[i].Split(" ");
                                contrast_level[1] = contrast_level[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] image_color = rows[i].Split(" ");
                                image_color[1] = image_color[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] motion_detection = rows[i].Split(" ");
                                motion_detection[1] = motion_detection[1].Replace("\"", "").Replace(",", "");

                                config.Resolution = (Resolution)Enum.Parse(typeof(Resolution), resolution[1]);
                                config.ContrastLevel = Int32.Parse(contrast_level[1]);
                                config.ImageColor = Color.FromName(image_color[1]);
                                config.MotionDetection = Convert.ToBoolean(motion_detection[1]);
                            }

                            i += 2;
                            if (rows[i].Contains("network_configuration"))
                            {
                                i += 2;

                                string[] server_version = rows[i].Split(" ");
                                server_version[1] = server_version[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] server_IP_address = rows[i].Split(" ");
                                server_IP_address[1] = server_IP_address[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] server_port = rows[i].Split(" ");
                                server_port[1] = server_port[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] connection_status = rows[i].Split(" ");
                                connection_status[1] = connection_status[1].Replace("\"", "").Replace(",", "");

                                config.ServerVersion = server_version[1];
                                config.ServerIP = server_IP_address[1];
                                config.ServerPort = Int32.Parse(server_port[1]);
                                config.ConnectionStatus = Convert.ToBoolean(connection_status[1]);
                            }

                            i += 2;
                            if (rows[i].Contains("capture_configuration"))
                            {
                                i += 2;

                                string[] image_capture = rows[i].Split(" ");
                                image_capture[1] = image_capture[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] single_mode = rows[i].Split(" ");
                                single_mode[1] = single_mode[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] duration = rows[i].Split(" ");
                                duration[1] = duration[1].Replace("\"", "").Replace(",", "");
                                i++;

                                string[] burst_period = rows[i].Split(" ");
                                burst_period[1] = burst_period[1].Replace("\"", "").Replace(",", "");

                                config.ImageCapture = Convert.ToBoolean(image_capture[1]);
                                config.SingleMode = Convert.ToBoolean(single_mode[1]);
                                config.Duration = Int32.Parse(duration[1]);
                                config.Period = Int32.Parse(burst_period[1]);
                            }

                            i += 4;
                            newSnapshot.Camera[camera] = config;
                            camera++;
                        }

                        Program.Snapshot = newSnapshot;
                    }
                }
                
                radioButton5.Checked = true;
                UpdateConfigurationWindow(0);
            }
            catch
            {
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
            numericUpDown3.Enabled = !numericUpDown3.Enabled;
            numericUpDown3.ReadOnly = !numericUpDown3.ReadOnly;
            domainUpDown2.Enabled = !domainUpDown2.Enabled;
            domainUpDown2.ReadOnly = !domainUpDown2.ReadOnly;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = !radioButton3.Checked;
            numericUpDown3.ReadOnly = radioButton3.Checked;
            domainUpDown2.Enabled = !radioButton3.Checked;
            domainUpDown2.ReadOnly = radioButton3.Checked;
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
