namespace SnapShot
{
    partial class InformationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.postavkeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registracijaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pomoćToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 170);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(662, 26);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Firebrick;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(615, 40);
            this.label1.TabIndex = 5;
            this.label1.Text = "This app was created by a team of students at the Faculty of Electrical Engineeri" +
    "ng Sarajevo.\r\nContact us at sbehic2@etf.unsa.ba if you encounter any problems wi" +
    "th the application.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(27, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 28);
            this.label2.TabIndex = 6;
            this.label2.Text = "SnapShot, version: 2.3";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Highlight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.postavkeToolStripMenuItem,
            this.registracijaToolStripMenuItem,
            this.pomoćToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(662, 30);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // postavkeToolStripMenuItem
            // 
            this.postavkeToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.postavkeToolStripMenuItem.Name = "postavkeToolStripMenuItem";
            this.postavkeToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.postavkeToolStripMenuItem.Text = "Configuration";
            this.postavkeToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // registracijaToolStripMenuItem
            // 
            this.registracijaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.registracijaToolStripMenuItem.Name = "registracijaToolStripMenuItem";
            this.registracijaToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
            this.registracijaToolStripMenuItem.Text = "Licence";
            this.registracijaToolStripMenuItem.Click += new System.EventHandler(this.registracijaToolStripMenuItem_Click);
            // 
            // pomoćToolStripMenuItem
            // 
            this.pomoćToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.pomoćToolStripMenuItem.Name = "pomoćToolStripMenuItem";
            this.pomoćToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.pomoćToolStripMenuItem.Text = "Help";
            // 
            // InformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(662, 196);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(680, 243);
            this.MinimumSize = new System.Drawing.Size(680, 243);
            this.Name = "InformationForm";
            this.Text = "SnapShot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InformationForm_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem postavkeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registracijaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pomoćToolStripMenuItem;
    }
}