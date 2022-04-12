﻿using System;
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

namespace SnapShot
{
    public partial class LicencingForm : Form
    {
        #region Attributes

        string error = "";

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
        /// Helper method for determining whether the user is licenced or not
        /// </summary>
        private void LicenceCheck()
        {
            try
            {
                var macAddress =
                (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();

                string connectionString = @Properties.Resources.connectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string SQL = "SELECT * FROM licences WHERE MAC_address = '" + macAddress + "';";
                SqlCommand command = new SqlCommand(SQL, connection);
                SqlDataReader result = command.ExecuteReader();

                Program.Snapshot.Licenced = result.HasRows;

                connection.Close();
            }
            catch
            {
                error = "Licence check could not be performed. Contact nbadzak1@etf.unsa.ba for help.";
            }
        }

        #endregion
    }
}
