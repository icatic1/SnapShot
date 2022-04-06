﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot
{
    public partial class InformationForm : Form
    {
        #region Attributes

        Snapshot snapshot;

        #endregion

        #region Constructor

        public InformationForm(Snapshot s)
        {
            InitializeComponent();
            snapshot = s;
            toolStripStatusLabel1.Text = "";
        }

        private void InformationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            this.Hide();
            ConfigurationForm f = new ConfigurationForm(snapshot);
            f.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Go to help form
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

        #endregion
    }
}
