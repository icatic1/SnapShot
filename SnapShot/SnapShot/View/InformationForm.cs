using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnapShot.Utilities;

namespace SnapShot
{
    public partial class InformationForm : Form
    {
        #region Attributes
        private Rectangle formSizeOriginal;
        private Rectangle label1Original;
        private Rectangle label2Original;
        #endregion
        #region Constructor

        public InformationForm()
        {
            InitializeComponent();
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
        private void registracijaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LicencingForm f = new LicencingForm();
            this.Hide();
            f.ShowDialog();
            this.Close();
        }

        #endregion

        #region Methods
        private void InformationForm_Load(object sender, EventArgs e)
        {
            formSizeOriginal = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            label1Original = new Rectangle(label1.Location.X, label1.Location.Y, label1.Size.Width, label1.Size.Height);
            label2Original = new Rectangle(label2.Location.X, label2.Location.Y, label2.Size.Width, label2.Size.Height);
        }

       
        private void InformationForm_Resize(object sender, EventArgs e)
        {
            ResizeUtil.resizeControl(this, formSizeOriginal, label1Original, label1);
            ResizeUtil.resizeControl(this, formSizeOriginal, label2Original, label2);


            ResizeUtil.resizeFont(this, formSizeOriginal, label1, 9);
            ResizeUtil.resizeFont(this, formSizeOriginal, label2, 12);
            ResizeUtil.resizeFont(this, formSizeOriginal, menuStrip1, 9);

        }


        #endregion

    }
}
