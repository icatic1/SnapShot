using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot.View
{
    public partial class ServerConnectionForm : Form
    {
        #region Attributes

        string url = "";

        #endregion

        #region Constructor

        public ServerConnectionForm(string baseUrl)
        {
            InitializeComponent();

            url = baseUrl;
            toolStripStatusLabel1.Text = "";
        }

        #endregion

        #region Button redirect

        /// <summary>
        /// Attempt to validate user activation code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = Activation();
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

        #region Activation code

        /// <summary>
        /// Attempt to verify the user credentials by contacting the local server
        /// </summary>
        public DialogResult Activation()
        {
            try
            {
                HttpWebRequest webRequest;
                string requestParams = Configuration.GetMACAddress() +
                                       "?activationKey=" + textBox1.Text;

                webRequest = (HttpWebRequest)WebRequest.Create(url + "/api/Licence/ActivateDevice/" + requestParams);
                webRequest.Method = "GET";

                // verify activation code
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    toolStripStatusLabel1.Text = "Invalid activation code!";
                    return DialogResult.None;
                }

                // add received token to configuration
                Stream responseStream = response.GetResponseStream();
                StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                string Json = rdr.ReadToEnd();

                Configuration.Token = Json;

                return DialogResult.OK;
            }
            catch
            {
                toolStripStatusLabel1.Text = "Invalid activation code!";
                return DialogResult.None;
            }
        }

        #endregion

    }
}
