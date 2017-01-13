namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class TextToSpeechSettingsForm
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
            this.lblVolumeText = new System.Windows.Forms.Label();
            this.lblRateText = new System.Windows.Forms.Label();
            this.lblPitchText = new System.Windows.Forms.Label();
            this.lblTest = new System.Windows.Forms.Button();
            this.lblRestoreDefaults = new System.Windows.Forms.Button();
            this.lblBack = new System.Windows.Forms.Button();
            this.lblOK = new System.Windows.Forms.Button();
            this.lblPitch = new System.Windows.Forms.Label();
            this.tbPitch = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.tbRate = new System.Windows.Forms.TextBox();
            this.tbVolume = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTTSEngineName = new System.Windows.Forms.Label();
            this.panelTitle = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblVolumeText
            // 
            this.lblVolumeText.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolumeText.Location = new System.Drawing.Point(322, 122);
            this.lblVolumeText.Name = "lblVolumeText";
            this.lblVolumeText.Size = new System.Drawing.Size(79, 25);
            this.lblVolumeText.TabIndex = 158;
            this.lblVolumeText.Text = "(1 to 15)";
            this.lblVolumeText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRateText
            // 
            this.lblRateText.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRateText.Location = new System.Drawing.Point(322, 178);
            this.lblRateText.Name = "lblRateText";
            this.lblRateText.Size = new System.Drawing.Size(79, 25);
            this.lblRateText.TabIndex = 157;
            this.lblRateText.Text = "(50 to 250)";
            this.lblRateText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPitchText
            // 
            this.lblPitchText.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPitchText.Location = new System.Drawing.Point(322, 236);
            this.lblPitchText.Name = "lblPitchText";
            this.lblPitchText.Size = new System.Drawing.Size(79, 25);
            this.lblPitchText.TabIndex = 156;
            this.lblPitchText.Text = "(50 to 200)";
            this.lblPitchText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTest
            // 
            this.lblTest.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTest.Location = new System.Drawing.Point(183, 291);
            this.lblTest.Name = "lblTest";
            this.lblTest.Size = new System.Drawing.Size(172, 47);
            this.lblTest.TabIndex = 6;
            this.lblTest.TabStop = false;
            this.lblTest.Text = " Test";
            this.lblTest.UseVisualStyleBackColor = true;
            // 
            // lblRestoreDefaults
            // 
            this.lblRestoreDefaults.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestoreDefaults.Location = new System.Drawing.Point(374, 364);
            this.lblRestoreDefaults.Name = "lblRestoreDefaults";
            this.lblRestoreDefaults.Size = new System.Drawing.Size(172, 47);
            this.lblRestoreDefaults.TabIndex = 2;
            this.lblRestoreDefaults.TabStop = false;
            this.lblRestoreDefaults.Text = "Defaults";
            this.lblRestoreDefaults.UseVisualStyleBackColor = true;
            // 
            // lblBack
            // 
            this.lblBack.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBack.Location = new System.Drawing.Point(196, 364);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(172, 47);
            this.lblBack.TabIndex = 1;
            this.lblBack.TabStop = false;
            this.lblBack.Text = "Cancel";
            this.lblBack.UseVisualStyleBackColor = true;
            // 
            // lblOK
            // 
            this.lblOK.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOK.Location = new System.Drawing.Point(18, 364);
            this.lblOK.Name = "lblOK";
            this.lblOK.Size = new System.Drawing.Size(172, 47);
            this.lblOK.TabIndex = 0;
            this.lblOK.TabStop = false;
            this.lblOK.Text = "OK";
            this.lblOK.UseVisualStyleBackColor = true;
            // 
            // lblPitch
            // 
            this.lblPitch.AutoSize = true;
            this.lblPitch.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPitch.Location = new System.Drawing.Point(129, 237);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(57, 24);
            this.lblPitch.TabIndex = 155;
            this.lblPitch.Text = "Pitch";
            this.lblPitch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbPitch
            // 
            this.tbPitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPitch.Location = new System.Drawing.Point(249, 241);
            this.tbPitch.Name = "tbPitch";
            this.tbPitch.Size = new System.Drawing.Size(55, 20);
            this.tbPitch.TabIndex = 5;
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRate.Location = new System.Drawing.Point(129, 179);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(55, 24);
            this.lblRate.TabIndex = 154;
            this.lblRate.Text = "Rate";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.Location = new System.Drawing.Point(129, 123);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(77, 24);
            this.lblVolume.TabIndex = 153;
            this.lblVolume.Text = "Volume";
            this.lblVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbRate
            // 
            this.tbRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRate.Location = new System.Drawing.Point(249, 183);
            this.tbRate.Name = "tbRate";
            this.tbRate.Size = new System.Drawing.Size(55, 20);
            this.tbRate.TabIndex = 4;
            // 
            // tbVolume
            // 
            this.tbVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbVolume.Location = new System.Drawing.Point(249, 127);
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(55, 20);
            this.tbVolume.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblTTSEngineName);
            this.groupBox1.Controls.Add(this.panelTitle);
            this.groupBox1.Controls.Add(this.lblTest);
            this.groupBox1.Location = new System.Drawing.Point(13, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 425);
            this.groupBox1.TabIndex = 160;
            this.groupBox1.TabStop = false;
            // 
            // lblTTSEngineName
            // 
            this.lblTTSEngineName.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTTSEngineName.Location = new System.Drawing.Point(26, 37);
            this.lblTTSEngineName.Name = "lblTTSEngineName";
            this.lblTTSEngineName.Size = new System.Drawing.Size(451, 31);
            this.lblTTSEngineName.TabIndex = 162;
            this.lblTTSEngineName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTitle
            // 
            this.panelTitle.AutoSize = true;
            this.panelTitle.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTitle.Location = new System.Drawing.Point(14, -1);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(217, 24);
            this.panelTitle.TabIndex = 161;
            this.panelTitle.Text = "TextToSpeechSettings";
            this.panelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TextToSpeechSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 445);
            this.Controls.Add(this.lblVolumeText);
            this.Controls.Add(this.lblRateText);
            this.Controls.Add(this.lblPitchText);
            this.Controls.Add(this.lblRestoreDefaults);
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.lblOK);
            this.Controls.Add(this.lblPitch);
            this.Controls.Add(this.tbPitch);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.tbRate);
            this.Controls.Add(this.tbVolume);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TextToSpeechSettingsForm";
            this.Text = "ACAT";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVolumeText;
        private System.Windows.Forms.Label lblRateText;
        private System.Windows.Forms.Label lblPitchText;
        private System.Windows.Forms.Button lblTest;
        private System.Windows.Forms.Button lblRestoreDefaults;
        private System.Windows.Forms.Button lblBack;
        private System.Windows.Forms.Button lblOK;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.TextBox tbPitch;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TextBox tbRate;
        private System.Windows.Forms.TextBox tbVolume;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label panelTitle;
        private System.Windows.Forms.Label lblTTSEngineName;

    }
}

