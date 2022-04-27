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
    public partial class JSONPopup : Form
    {
        #region Constructor

        public JSONPopup()
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
            ConfigurationForm.JSONLocation = comboBox2.Text;
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

            foreach (var camera in Program.Snapshot.Camera)
            {
                string path = "http://" + camera.ServerIP;
                if (camera.ServerPort != 0)
                    path += ":" + camera.ServerPort;
                if (camera.JSONConfigPath != "")
                    path += "/" + camera.JSONConfigPath;
                comboBox2.Items.Add(path);
            }

            if (additionalLocation.Length > 0)
                comboBox2.Items.Add(additionalLocation);
        }

        #endregion
    }
}
