using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot
{
    public partial class LicencingForm : Form
    {
        #region Constructor

        public LicencingForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "";

            // if terminal ID is empty, put placeholder
            if (String.IsNullOrWhiteSpace(Program.Snapshot.TerminalName))
                Program.Snapshot.TerminalName = Environment.MachineName;

            // we are disconnected - use local config file
            if (!Program.Snapshot.Connected)
            {
                if (!File.Exists("config.txt"))
                    File.WriteAllText("config.txt", Program.Snapshot.TerminalName + "\n" + Program.Snapshot.DebugLog);

                else
                {
                    string IMPORT = File.ReadAllText("config.txt");
                    string[] rows = IMPORT.Split('\n');
                    Program.Snapshot.TerminalName = rows[0];
                    Program.Snapshot.DebugLog = Convert.ToBoolean(rows[1]);
                }
            }

            // we are connected - grab information from server
            else
            {
                label7.Text = "Connected";
                label7.ForeColor = System.Drawing.Color.Green;

                GetInformationFromServer();
            }

            textBox2.Text = Program.Snapshot.TerminalName;
            checkBox1.Checked = Program.Snapshot.DebugLog;
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
            ConfigurationForm f = new ConfigurationForm();
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
            UpdateConfigurationFile();
            InformationForm f = new InformationForm();
            this.Hide();
            f.ShowDialog();
            this.Close();
        }

        #endregion

        #region Settings changes

        private void GetInformationFromServer()
        {
            try
            {
                HttpWebRequest webRequest;
                string requestParams = "MacAddress=" + Configuration.GetMACAddress();

                webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/GetTerminalAndDebugLog" + "?" + requestParams);

                webRequest.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Bad request!");

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                    string Json = rdr.ReadToEnd();
                    var obj = JObject.Parse(Json);
                    string terminalID = obj.Value<string>("terminalID") ?? "";
                    bool debugLog = obj.Value<bool>("debugLog");
                    
                    // server information and local config not synchronized - ask the user whether they want to keep the server or local info
                    if (terminalID != Program.Snapshot.TerminalName || debugLog != Program.Snapshot.DebugLog)

                    {
                        string message = "Do you want to update the current local configuration:\n" +
                                     "terminal name = " + Program.Snapshot.TerminalName + ", debug log = " + Program.Snapshot.DebugLog + "\n" +
                                     "to the configuration currently on server:\n" +
                                     "terminal name = " + terminalID + ", debug log = " + debugLog + "?\n" +
                                     "Choose Yes if you want to keep the server configuration\n" +
                                     "or choose No if you want to keep the local configuration.";

                        DialogResult result = MessageBox.Show(message, "Action necessary - conflict found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            Program.Snapshot.TerminalName = terminalID;
                            Program.Snapshot.DebugLog = debugLog;

                            textBox2.Text = Program.Snapshot.TerminalName;
                            checkBox1.Checked = Program.Snapshot.DebugLog;
                        }
                        else
                        {
                            SendInformationToServer();
                        }
                    }
                }
            }
            // information about the terminal is not present at the server - 
            // create new configuration at the server
            catch
            {
                HttpWebRequest webRequest;
                string requestParams = "MacAddress=" + Configuration.GetMACAddress() + "&"
                                       + "TerminalID=" + Program.Snapshot.TerminalName + "&"
                                       + "DebugLog=" + Program.Snapshot.DebugLog;

                webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/InitialAddDevice" + "?" + requestParams);

                webRequest.Method = "POST";

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Bad request!");
            }
        }

        private void SendInformationToServer()
        {
            // request will fail if terminal name is empty
            if (Program.Snapshot.TerminalName.Length < 1)
            {
                toolStripStatusLabel1.Text = "Terminal name cannot be empty.";
                return;
            }

            toolStripStatusLabel1.Text = "";

            HttpWebRequest webRequest;
            string requestParams = "MacAddress=" + Configuration.GetMACAddress() + "&"
                                   + "TerminalID=" + Program.Snapshot.TerminalName + "&"
                                   + "DebugLog=" + Program.Snapshot.DebugLog;

            webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/UpdateTerminalAndDebugLog" + "?" + requestParams);

            webRequest.Method = "POST";

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Bad request!");
        }

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
            // we are not connected - locally save terminal name and debug log
            string EXPORT = Program.Snapshot.TerminalName + "\n" + Program.Snapshot.DebugLog;
            File.WriteAllText("config.txt", EXPORT);

            // we are connected - save information to server
            if (Program.Snapshot.Connected)
            {
                SendInformationToServer();
            }
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
            // cannot check licence status if we are disconnected
            if (!Program.Snapshot.Connected)
            {
                toolStripStatusLabel1.Text = "You need to connect to the server first.";
                return;
            }

            toolStripStatusLabel1.Text = "";

            LicenceCheck();

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
                string requestParams = Configuration.GetMACAddress() ?? "";

                webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/" + requestParams);

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
                // user not licenced
                Program.Snapshot.Licenced = false;
            }
        }

        #endregion

        #region Server connection

        private void button2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            try
            {
                HttpWebRequest webRequest;

                webRequest = (HttpWebRequest)WebRequest.Create("http://sigrupa4-001-site1.ctempurl.com/api/Licence/ConnectionCheck");

                webRequest.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Disconnected!");

                label7.Text = "Connected";
                label7.ForeColor = System.Drawing.Color.Green;

                Program.Snapshot.Connected = true;

                // get terminal ID and debug log from server
                GetInformationFromServer();
            }
            catch
            {
                label7.Text = "Disconnected";
                label7.ForeColor = System.Drawing.Color.Red;

                Program.Snapshot.Connected = false;
            }
        }

        #endregion
    }
}
