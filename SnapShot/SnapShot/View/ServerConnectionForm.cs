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
    public partial class ServerConnectionForm : Form
    {
        #region Constructor

        public ServerConnectionForm()
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
            this.DialogResult = Login();
            if (this.DialogResult == DialogResult.OK)
                this.Close();
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
        public DialogResult Login()
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
                    return DialogResult.None;
                }

                return DialogResult.OK;
            }
            catch
            {
                toolStripStatusLabel1.Text = "Wrong email and/or password!";
                return DialogResult.None;
            }
        }

        #endregion

    }
}
