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
            toolTip1.SetToolTip(textBox1, "Enter your username. Hint: the username is 'administrator'.");
            toolTip1.SetToolTip(textBox2, "Enter your password. Hint: the password is 'administrator'.");
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
                // use static data for login                
                if (textBox1.Text != "administrator" || textBox2.Text != "administrator")
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

            toolTip1.SetToolTip(textBox1, "Enter the licencing server IP address. 'localhost' and web-addresses are also supported.");
            toolTip1.SetToolTip(textBox2, "Enter the licencing server port. The port value can be ommitted if it is not necessary. Port must be a valid integer number.");

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
                Program.LicencingURL += ":" + port;

            // save new information to local file
            string EXPORT = Program.LicencingURL + "\n" + Program.Snapshot.TerminalName + "\n" + Program.Snapshot.DebugLog;
            File.WriteAllText("config.txt", EXPORT);

            return DialogResult.OK;
        }

        #endregion
    }
}
