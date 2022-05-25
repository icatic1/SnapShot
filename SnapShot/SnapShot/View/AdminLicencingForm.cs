using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot.View
{
    public partial class AdminLicencingForm : Form
    {
        #region Attributes

        bool login = true;

        #endregion

        #region Constructor

        public AdminLicencingForm()
        {
            InitializeComponent();

            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Button redirect

        /// <summary>
        /// Either attempt to login the user or save the new licencing server
        /// configuration on OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (login)
            {
                Login();
                this.DialogResult = DialogResult.None;
            }

            else
            {
                this.DialogResult = ChangeServer();
                if (this.DialogResult == DialogResult.OK)
                    this.Close();
            }
        }

        /// <summary>
        /// Cancel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Login

        /// <summary>
        /// Attempt to login the user by contacting the local server
        /// </summary>
        public void Login()
        {
            try
            {
                HttpWebRequest webRequest;

                string baseUrl = "https://siset1.ga";
                if (Program.Snapshot.Configuration.ServerIP.Length > 0)
                    baseUrl = Program.Snapshot.Configuration.ServerIP;
                if (Program.Snapshot.Configuration.ServerPort != 0)
                    baseUrl += ":" + Program.Snapshot.Configuration.ServerPort;

                string requestParams = "email=" + textBox1.Text +
                       "&password=" + textBox2.Text;

                webRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "/api/User/login" + "?" + requestParams);

                webRequest.Method = "POST";

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    toolStripStatusLabel1.Text = "Wrong email and/or password!";
                    return;
                }

                NextStep();
                login = false;
            }
            catch
            {
                toolStripStatusLabel1.Text = "Wrong email and/or password!";
            }
        }

        #endregion

        #region Licencing server change
        
        /// <summary>
        /// Change the form from login information to licencing server information
        /// </summary>
        public void NextStep()
        {
            label2.Text = "IP address:";
            label1.Text = "Port:";

            textBox1.Text = "";
            textBox2.Text = "";
            textBox2.PasswordChar = '\0';

            comboBox1.Visible = true;
            comboBox1.SelectedItem = "https://";
            label3.Visible = true;
        }

        /// <summary>
        /// Save licencing server changes and close the form
        /// </summary>
        public DialogResult ChangeServer()
        {
            string serverIP = "";
            serverIP = comboBox1.Text + textBox1.Text;

            int port = 0;
            if (textBox2.Text.Length > 0)
                try
                {
                    port = Int32.Parse(textBox2.Text);
                }
                catch
                {
                    toolStripStatusLabel1.Text = "Port must be a valid number!";
                    return DialogResult.None;
                }

            Program.LicencingURL = serverIP;
            if (port != 0)
                Program.LicencingURL += port;

            return DialogResult.OK;
        }

        #endregion

    }
}
