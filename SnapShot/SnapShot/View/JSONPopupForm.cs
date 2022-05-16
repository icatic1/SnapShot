using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapShot.View
{
    public partial class JSONPopupForm : Form
    {
        #region Constructor

        public JSONPopupForm()
        {
            InitializeComponent();

            ResetComboBox("");
        }

        #endregion

        #region Buttons

        /// <summary>
        /// OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            GeneralSettingsForm.JSONLocation = comboBox2.Text;
            this.Close();
        }

        /// <summary>
        /// Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Selecting local file

        /// <summary>
        /// Local folder selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.FileName = "configuration.json";
                openFileDialog.Filter = "JSON files (*.json)|*.json";
                openFileDialog.FilterIndex = 2;
                openFileDialog.CheckFileExists = false;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ResetComboBox(openFileDialog.FileName);
                    comboBox2.Text = openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// Selected file location change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_TextUpdate(object sender, EventArgs e)
        {
            if (comboBox2.Text.Length > 0)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        #endregion

        #region Combo box items

        /// <summary>
        /// Resetting combo box items
        /// </summary>
        /// <param name="additionalLocation"></param>
        private void ResetComboBox(string additionalLocation)
        {
            comboBox2.Items.Clear();

            // add the JSON export path of the server to the list of items
            string path = "http://" + Program.Snapshot.Configuration.ServerIP;
            if (Program.Snapshot.Configuration.ServerPort != 0)
                path += ":" + Program.Snapshot.Configuration.ServerPort;
            if (Program.Snapshot.Configuration.JSONExportLocation != "")
                path += "/" + Program.Snapshot.Configuration.JSONExportLocation;
            comboBox2.Items.Add(path);

            // add the JSON import path of the server to the list of items
            path = "http://" + Program.Snapshot.Configuration.ServerIP;
            if (Program.Snapshot.Configuration.ServerPort != 0)
                path += ":" + Program.Snapshot.Configuration.ServerPort;
            if (Program.Snapshot.Configuration.JSONImportLocation != "")
                path += "/" + Program.Snapshot.Configuration.JSONImportLocation;
            comboBox2.Items.Add(path);

            // add any other local path the user has selected to the list of items
            if (additionalLocation.Length > 0)
                comboBox2.Items.Add(additionalLocation);
        }

        #endregion
    }
}
