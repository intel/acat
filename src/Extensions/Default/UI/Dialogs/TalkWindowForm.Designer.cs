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
            this.labelTalk = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.labelClose = new System.Windows.Forms.Label();
            this.labelClearText = new System.Windows.Forms.Label();
            this.labelDateTime = new System.Windows.Forms.Label();
            this.lblIntelIcon = new System.Windows.Forms.Label();
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.labelVolumeLevel = new System.Windows.Forms.Label();
            this.labelSpeaker = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.BorderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTalk
            // 
            this.labelTalk.AutoSize = true;
            this.labelTalk.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTalk.ForeColor = System.Drawing.Color.Black;
            this.labelTalk.Location = new System.Drawing.Point(56, 6);
            this.labelTalk.Name = "labelTalk";
            this.labelTalk.Size = new System.Drawing.Size(59, 29);
            this.labelTalk.TabIndex = 2;
            this.labelTalk.Text = "Talk";
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.Color.Black;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(16, 39);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(924, 281);
            this.textBox.TabIndex = 0;
            // 
            // labelClose
            // 
            this.labelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelClose.BackColor = System.Drawing.Color.Transparent;
            this.labelClose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelClose.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClose.Location = new System.Drawing.Point(900, 2);
            this.labelClose.Name = "labelClose";
            this.labelClose.Size = new System.Drawing.Size(39, 30);
            this.labelClose.TabIndex = 7;
            this.labelClose.Text = "X";
            this.labelClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelClose.Click += new System.EventHandler(this.labelClose_Click);
            // 
            // labelClearText
            // 
            this.labelClearText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelClearText.BackColor = System.Drawing.Color.Transparent;
            this.labelClearText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelClearText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelClearText.Location = new System.Drawing.Point(862, 2);
            this.labelClearText.Name = "labelClearText";
            this.labelClearText.Size = new System.Drawing.Size(39, 30);
            this.labelClearText.TabIndex = 8;
            this.labelClearText.Text = "0";
            this.labelClearText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelClearText.Click += new System.EventHandler(this.labelClearText_Click);
            // 
            // labelDateTime
            // 
            this.labelDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDateTime.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDateTime.ForeColor = System.Drawing.Color.Black;
            this.labelDateTime.Location = new System.Drawing.Point(526, 9);
            this.labelDateTime.Name = "labelDateTime";
            this.labelDateTime.Size = new System.Drawing.Size(326, 23);
            this.labelDateTime.TabIndex = 10;
            this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIntelIcon
            // 
            this.lblIntelIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblIntelIcon.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblIntelIcon.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblIntelIcon.Location = new System.Drawing.Point(3, 2);
            this.lblIntelIcon.Name = "lblIntelIcon";
            this.lblIntelIcon.Size = new System.Drawing.Size(40, 34);
            this.lblIntelIcon.TabIndex = 11;
            this.lblIntelIcon.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblIntelIcon.UseMnemonic = false;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Controls.Add(this.labelVolumeLevel);
            this.BorderPanel.Controls.Add(this.labelSpeaker);
            this.BorderPanel.Controls.Add(this.trackBarVolume);
            this.BorderPanel.Controls.Add(this.lblIntelIcon);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(957, 353);
            this.BorderPanel.TabIndex = 12;
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
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(957, 353);
            this.Controls.Add(this.labelDateTime);
            this.Controls.Add(this.labelClearText);
            this.Controls.Add(this.labelClose);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.labelTalk);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TalkWindowForm";
            this.RightToLeftLayout = true;
            this.Text = "ACAT Talk Window";
            this.TopMost = true;
            this.BorderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTalk;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label labelClose;
        private System.Windows.Forms.Label labelClearText;
        private System.Windows.Forms.Label labelDateTime;
        private System.Windows.Forms.Label lblIntelIcon;
        private System.Windows.Forms.Panel BorderPanel;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelSpeaker;
        private System.Windows.Forms.Label labelVolumeLevel;


    }
}