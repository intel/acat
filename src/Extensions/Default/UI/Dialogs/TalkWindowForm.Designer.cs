namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class TalkWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TalkWindowForm));
            this.textBox = new System.Windows.Forms.TextBox();
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelDate = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabelIcon = new System.Windows.Forms.ToolStripLabel();
            this.labelVolumeLevel = new System.Windows.Forms.Label();
            this.labelSpeaker = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.BorderPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.Color.Black;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(16, 39);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(924, 281);
            this.textBox.TabIndex = 0;
            this.textBox.TabStop = false;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BackColor = System.Drawing.SystemColors.Control;
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Controls.Add(this.toolStrip1);
            this.BorderPanel.Controls.Add(this.labelVolumeLevel);
            this.BorderPanel.Controls.Add(this.labelSpeaker);
            this.BorderPanel.Controls.Add(this.trackBarVolume);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(957, 353);
            this.BorderPanel.TabIndex = 12;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonClear,
            this.toolStripSeparator1,
            this.toolStripLabelDate,
            this.toolStripLabel1,
            this.toolStripLabelIcon});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(955, 43);
            this.toolStrip1.TabIndex = 15;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonClear.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClear.Image")));
            this.toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClear.Margin = new System.Windows.Forms.Padding(0, 1, 20, 2);
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.toolStripButtonClear.Size = new System.Drawing.Size(41, 40);
            this.toolStripButtonClear.Text = "0";
            this.toolStripButtonClear.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 43);
            // 
            // toolStripLabelDate
            // 
            this.toolStripLabelDate.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelDate.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabelDate.Name = "toolStripLabelDate";
            this.toolStripLabelDate.Size = new System.Drawing.Size(97, 40);
            this.toolStripLabelDate.Text = "<Date>";
            this.toolStripLabelDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 40);
            // 
            // toolStripLabelIcon
            // 
            this.toolStripLabelIcon.AutoSize = false;
            this.toolStripLabelIcon.BackColor = System.Drawing.Color.Transparent;
            this.toolStripLabelIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabelIcon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripLabelIcon.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.toolStripLabelIcon.Name = "toolStripLabelIcon";
            this.toolStripLabelIcon.Size = new System.Drawing.Size(40, 40);
            this.toolStripLabelIcon.Text = ".";
            // 
            // labelVolumeLevel
            // 
            this.labelVolumeLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVolumeLevel.BackColor = System.Drawing.Color.Transparent;
            this.labelVolumeLevel.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVolumeLevel.ForeColor = System.Drawing.Color.Black;
            this.labelVolumeLevel.Location = new System.Drawing.Point(901, 318);
            this.labelVolumeLevel.Name = "labelVolumeLevel";
            this.labelVolumeLevel.Size = new System.Drawing.Size(33, 30);
            this.labelVolumeLevel.TabIndex = 14;
            this.labelVolumeLevel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelSpeaker
            // 
            this.labelSpeaker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSpeaker.BackColor = System.Drawing.Color.Transparent;
            this.labelSpeaker.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.labelSpeaker.ForeColor = System.Drawing.Color.Black;
            this.labelSpeaker.Location = new System.Drawing.Point(728, 313);
            this.labelSpeaker.Name = "labelSpeaker";
            this.labelSpeaker.Size = new System.Drawing.Size(33, 30);
            this.labelSpeaker.TabIndex = 13;
            this.labelSpeaker.Text = "F";
            this.labelSpeaker.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarVolume.AutoSize = false;
            this.trackBarVolume.LargeChange = 3;
            this.trackBarVolume.Location = new System.Drawing.Point(756, 322);
            this.trackBarVolume.Maximum = 9;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(148, 26);
            this.trackBarVolume.TabIndex = 12;
            this.trackBarVolume.TabStop = false;
            this.trackBarVolume.TickFrequency = 3;
            // 
            // TalkWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(957, 353);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TalkWindowForm";
            this.RightToLeftLayout = true;
            this.Text = "ACAT Talk Window";
            this.TopMost = true;
            this.BorderPanel.ResumeLayout(false);
            this.BorderPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Panel BorderPanel;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelSpeaker;
        private System.Windows.Forms.Label labelVolumeLevel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelDate;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelIcon;


    }
}