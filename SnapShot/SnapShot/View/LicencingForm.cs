using SnapShot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using SnapShot.Utilities;

namespace SnapShot
{
    public partial class LicencingForm : Form
    {
        #region Attributes

        string error = "";
        private Rectangle buttonCheckLicenceOriginal;
        private Rectangle greetingTextOriginal;
        private Rectangle formSizeOriginal;
        private Rectangle groupBoxLicenceOriginal;
        private Rectangle checkBoxOriginal;
        private Rectangle terminalLabelOriginal;
        private Rectangle inputOriginal;
        private Rectangle statusLabelOriginal;
        private Rectangle statusOriginal;
        private Rectangle licencingLabelOriginal;
        private Rectangle versionLabelOriginal;
        private Rectangle groupBoxConnectionOriginal;
        private Rectangle connectionStatusLabelOriginal;
        private Rectangle connectionStatusOriginal;
        private Rectangle connectionStatusButtonOriginal;
        private Rectangle groupBoxTerminalOriginal;

        #endregion

        #region Constructor

        public LicencingForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";
            if (!File.Exists("config.txt"))
            {
                File.Create("config.txt").Close();
                File.WriteAllText("config.txt", Environment.MachineName + "\nFalse");
            }

            string IMPORT = File.ReadAllText("config.txt");
            string[] rows = IMPORT.Split('\n');
            Program.Snapshot.TerminalName = rows[0];
            Program.Snapshot.DebugLog = Convert.ToBoolean(rows[1]);

            textBox2.Text = Program.Snapshot.TerminalName;
            checkBox1.Checked = Program.Snapshot.DebugLog;

            if (Program.Snapshot.Connected)
            {
                label7.Text = "Connected";
                label7.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void LicencingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateConfigurationFile();
            Application.Exit();
        }

        #endregion

        #region Menu items

        /// <summary>
        /// Go to configuration form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateConfigurationFile();
            this.Hide();
            ConfigurationForm f = new ConfigurationForm();
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
            UpdateConfigurationFile();
            this.Hide();
            InformationForm f = new InformationForm();
            f.ShowDialog();
            this.Close();
        }

        #endregion

        #region Settings changes

        /// <summary>
        /// Terminal ID change saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Program.Snapshot.TerminalName = textBox2.Text;
        }

        /// <summary>
        /// Debug logging activation/deactivation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Program.Snapshot.DebugLog = checkBox1.Checked;
        }

        private void UpdateConfigurationFile()
        {
            string EXPORT = Program.Snapshot.TerminalName + "\n" + Program.Snapshot.DebugLog;
            File.WriteAllText("config.txt", EXPORT);
        }

        #endregion

        #region Licence check

        /// <summary>
        /// Perform licence check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            error = "";
            LicenceCheck();
            toolStripStatusLabel1.Text = error;

            if (Program.Snapshot.Licenced)
            {
                label3.Text = "Licenced version";
                textBox1.Text = "Your licence has been successfully found. Enjoy using the application!";
            }
            else
            {
                label3.Text = "Demo version";
                textBox1.Text = "Unfortunately, this machine has not been licenced yet. Contact us at icatic1@etf.unsa.ba to get your licence.";
            }
        }

        /// <summary>
        /// Helper method for determining whether the user is licenced or not
        /// </summary>
       private void LicenceCheck()
        {
            try
            {
                HttpWebRequest webRequest;

                webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/" + Configuration.GetMACAddress());

                webRequest.Method = "GET";

                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                        string Json = rdr.ReadToEnd();
                        Program.Snapshot.Licenced = Json == "true";
                    }
                }
            }
            catch
            {
                error = "Licence check could not be performed. Contact nbadzak1@etf.unsa.ba for help.";
            }
        }

        #endregion

        #region Server connection

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = @Properties.Resources.connectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                label7.Text = "Connected";
                label7.ForeColor = System.Drawing.Color.Green;

                Program.Snapshot.Connected = true;
            }
            catch
            {
                label7.Text = "Disconnected";
                label7.ForeColor = System.Drawing.Color.Red;

                Program.Snapshot.Connected = false;
            }
        }

        #endregion

        #region Resize methods
        private void LicencingForm_Load(object sender, EventArgs e)
        {
            formSizeOriginal = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            buttonCheckLicenceOriginal = new Rectangle(button1.Location.X, button1.Location.Y, button1.Size.Width, button1.Size.Height);
            greetingTextOriginal = new Rectangle(label1.Location.X, label1.Location.Y, label1.Size.Width, label1.Size.Height);
            groupBoxLicenceOriginal = new Rectangle(groupBox1.Location.X, groupBox1.Location.Y, groupBox1.Size.Width, groupBox1.Size.Height);
            checkBoxOriginal = new Rectangle(checkBox1.Location.X, checkBox1.Location.Y, checkBox1.Size.Width, checkBox1.Size.Height);
            inputOriginal = new Rectangle(textBox2.Location.X, textBox2.Location.Y, textBox2.Size.Width, textBox2.Size.Height);
            statusLabelOriginal = new Rectangle(label4.Location.X, label4.Location.Y, label4.Size.Width, label4.Size.Height);
            statusOriginal = new Rectangle(textBox1.Location.X, textBox1.Location.Y, textBox1.Size.Width, textBox1.Size.Height);
            terminalLabelOriginal = new Rectangle(label5.Location.X, label5.Location.Y, label5.Size.Width, label5.Size.Height);
            licencingLabelOriginal = new Rectangle(label2.Location.X, label2.Location.Y, label2.Size.Width, label2.Size.Height);
            versionLabelOriginal = new Rectangle(label3.Location.X, label3.Location.Y, label3.Size.Width, label3.Size.Height);
            groupBoxConnectionOriginal = new Rectangle(groupBox3.Location.X, groupBox3.Location.Y, groupBox3.Size.Width, groupBox3.Size.Height);
            connectionStatusLabelOriginal = new Rectangle(label6.Location.X, label6.Location.Y, label6.Size.Width, label6.Size.Height);
            connectionStatusOriginal = new Rectangle(label7.Location.X, label7.Location.Y, label7.Size.Width, label7.Size.Height);
            connectionStatusButtonOriginal = new Rectangle(button2.Location.X, button2.Location.Y, button2.Size.Width, button2.Size.Height);
            groupBoxTerminalOriginal = new Rectangle(groupBox2.Location.X, groupBox2.Location.Y, groupBox2.Size.Width, groupBox2.Size.Height);
        }

        

        private void LicencingForm_Resize(object sender, EventArgs e)
        {
            ResizeUtil.resizeControl(this, formSizeOriginal,buttonCheckLicenceOriginal, button1);
            ResizeUtil.resizeControl(this, formSizeOriginal, greetingTextOriginal, label1);
            ResizeUtil.resizeControl(this, formSizeOriginal, statusOriginal, textBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, statusLabelOriginal, label4);
            ResizeUtil.resizeControl(this, formSizeOriginal, inputOriginal, textBox2);
            ResizeUtil.resizeControl(this, formSizeOriginal, groupBoxLicenceOriginal, groupBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, checkBoxOriginal, checkBox1);
            ResizeUtil.resizeControl(this, formSizeOriginal, terminalLabelOriginal, label5);
            ResizeUtil.resizeControl(this, formSizeOriginal, licencingLabelOriginal, label2);
            ResizeUtil.resizeControl(this, formSizeOriginal, versionLabelOriginal, label3);
            ResizeUtil.resizeControl(this, formSizeOriginal, groupBoxTerminalOriginal, groupBox2);
            ResizeUtil.resizeControl(this, formSizeOriginal, groupBoxConnectionOriginal, groupBox3);
            ResizeUtil.resizeControl(this, formSizeOriginal, connectionStatusLabelOriginal, label6);
            ResizeUtil.resizeControl(this, formSizeOriginal, connectionStatusOriginal, label7);
            ResizeUtil.resizeControl(this, formSizeOriginal, connectionStatusButtonOriginal, button2);

            ResizeUtil.resizeFontTypes(this, label1, "Normal", formSizeOriginal, 18);
            ResizeUtil.resizeFontTypes(this, label2, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label3, "Normal", formSizeOriginal, 13.5f);
            ResizeUtil.resizeFontTypes(this, label4, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label5, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label6, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, label7, "Normal", formSizeOriginal, 13.5f);
            ResizeUtil.resizeFontTypes(this, button1, "Inside", buttonCheckLicenceOriginal, 9);
            ResizeUtil.resizeFontTypes(this, button2, "Inside", connectionStatusButtonOriginal, 9);
            ResizeUtil.resizeFontTypes(this, groupBox1, "Inside", groupBoxLicenceOriginal, 9);
            ResizeUtil.resizeFontTypes(this, groupBox2, "Inside", groupBoxTerminalOriginal, 9);
            ResizeUtil.resizeFontTypes(this, groupBox3, "Inside", groupBoxConnectionOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox1, "Inside", statusOriginal, 9);
            ResizeUtil.resizeFontTypes(this, textBox2, "Inside", inputOriginal, 9);
            ResizeUtil.resizeFontTypes(this, checkBox1, "Normal", formSizeOriginal, 9);
            ResizeUtil.resizeFontTypes(this, menuStrip1, "Normal", formSizeOriginal, 9);
        }

        #endregion
    }
}
