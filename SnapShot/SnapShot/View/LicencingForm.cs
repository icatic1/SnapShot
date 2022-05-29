using Newtonsoft.Json.Linq;
using SnapShot.View;
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
                GetInformationFromServer();

            textBox2.Text = Program.Snapshot.TerminalName;
            checkBox1.Checked = Program.Snapshot.DebugLog;
        }
        
        /// <summary>
        /// Exit the application when clicking on the X button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LicencingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateConfigurationFile();
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

        #region Settings changes

        /// <summary>
        /// Using existing information about the computer from remote server
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void GetInformationFromServer()
        {
            try
            {
                HttpWebRequest webRequest;
                string requestParams = "MacAddress=" + Configuration.GetMACAddress();

                webRequest = (HttpWebRequest)WebRequest.Create(Program.LicencingURL + "/api/Licence/GetTerminalAndDebugLog" + "?" + requestParams);

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

                webRequest = (HttpWebRequest)WebRequest.Create(Program.LicencingURL + "/api/Licence/InitialAddDevice" + "?" + requestParams);

                webRequest.Method = "POST";

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Bad request!");
            }
        }

        /// <summary>
        /// Saving configuration info on remote server
        /// </summary>
        /// <exception cref="Exception"></exception>
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

            webRequest = (HttpWebRequest)WebRequest.Create(Program.LicencingURL + "/api/Licence/UpdateTerminalAndDebugLog" + "?" + requestParams);

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

        /// <summary>
        /// Updating local file with configuration info (backup for offline mode)
        /// </summary>
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
            try
            {
                // if no licencing server has been created, ask the user to enter their own server
                if (Program.LicencingURL.Length < 1)
                {
                    // enter the form for configuring the licencing server
                    AdminLicencingForm popup = new AdminLicencingForm();
                    var result = popup.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        toolStripStatusLabel1.Text = "Licencing server was not configured!";
                        return;
                    }
                }

                // get terminal ID and debug log from server if the device already exists,
                // or create a new device on the server
                GetInformationFromServer();

                // get licencing information
                LicenceCheck();

                toolStripStatusLabel1.Text = "";

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
            catch
            {
                toolStripStatusLabel1.Text = "An error has occured while checking your licence. Check licencing server URL.";
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

                webRequest = (HttpWebRequest)WebRequest.Create(Program.LicencingURL + "/api/Licence/" + requestParams);

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
    }
}
