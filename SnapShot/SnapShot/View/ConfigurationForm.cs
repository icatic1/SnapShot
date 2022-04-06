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
        #region Attributes

        Snapshot snapshot;

        #endregion

        #region Constructor

        public ConfigurationForm(Snapshot s)
        {
            InitializeComponent();
            snapshot = s;
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
            LicencingForm f = new LicencingForm(snapshot);
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
            InformationForm f = new InformationForm(snapshot);
            f.ShowDialog();
            this.Close();
        }

        #endregion

        #region Configuration saving, export and import

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
            snapshot.Camera[camera] = new Configuration()
            {
                Type = type,
                Id = deviceName,
                TriggerFilePath = triggerPath,
                OutputFolderPath = outputPath,
                OutputValidity = validity,
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
                EXPORT += "[\n";
                foreach (var config in snapshot.Camera)
                {
                    EXPORT += "\t{\n";

                    EXPORT += "\t\t'device_configuration':\n";
                    EXPORT += "\t\t{\n";
                    EXPORT += "\t\t\t'device_type': '" + config.Type + "'\n";
                    EXPORT += "\t\t\t'id': '" + config.Id + "'\n";
                    EXPORT += "\t\t\t'trigger_file_path': '" + config.TriggerFilePath + "'\n";
                    EXPORT += "\t\t\t'output_folder_path': '" + config.OutputFolderPath + "'\n";
                    EXPORT += "\t\t\t'output_validity_days': " + config.OutputValidity + "\n";
                    EXPORT += "\t\t}\n";

                    EXPORT += "\t\t'video_configuration':\n";
                    EXPORT += "\t\t{\n";
                    EXPORT += "\t\t\t'resolution': '" + config.Resolution + "'\n";
                    EXPORT += "\t\t\t'contrast_level': " + config.ContrastLevel + "\n";
                    EXPORT += "\t\t\t'image_color': '" + config.ImageColor.ToString() + "'\n";
                    EXPORT += "\t\t\t'motion_detection': " + config.MotionDetection + "\n";
                    EXPORT += "\t\t}\n";

                    EXPORT += "\t\t'network_configuration':\n";
                    EXPORT += "\t\t{\n";
                    EXPORT += "\t\t\t'server_version': '" + config.ServerVersion + "'\n";
                    EXPORT += "\t\t\t'server_IP_address': '" + config.ServerIP + "'\n";
                    EXPORT += "\t\t\t'server_port': " + config.ServerPort + "\n";
                    EXPORT += "\t\t\t'connection_status': " + config.ConnectionStatus + "\n";
                    EXPORT += "\t\t}\n";

                    EXPORT += "\t\t'capture_configuration':\n";
                    EXPORT += "\t\t{\n";
                    EXPORT += "\t\t\t'image_capture': " + config.ImageCapture + "\n";
                    EXPORT += "\t\t\t'single_mode': " + config.SingleMode + "\n";
                    EXPORT += "\t\t\t'duration': " + config.Duration + "\n";
                    EXPORT += "\t\t\t'burst_period': " + config.Period + "\n";
                    EXPORT += "\t\t}\n";

                    EXPORT += "\t}\n";
                }
                EXPORT += "]";

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.FileName = "configuration.json";
                    openFileDialog.Filter = "JSON files (*.JSON)|*.JSON";
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
            //this.Hide();
            CapturePreviewForm f = new CapturePreviewForm(snapshot, comboBox1.Text, comboBox1.SelectedIndex);
            f.Show();
            //f.ShowDialog();
            //this.Close();
        }

        #endregion
    }
}
