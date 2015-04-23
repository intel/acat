namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class AboutBoxForm
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.textBoxOtherInfo = new System.Windows.Forms.TextBox();
            this.labelCopyrightInfo = new System.Windows.Forms.Label();
            this.labelVersionInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(185, 280);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(199, 40);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelAppTitle.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAppTitle.ForeColor = System.Drawing.Color.Black;
            this.labelAppTitle.Location = new System.Drawing.Point(149, 10);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(407, 39);
            this.labelAppTitle.TabIndex = 8;
            this.labelAppTitle.Text = "ACAT - Assistive Context-Aware Toolkit";
            this.labelAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(15, 12);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxLogo.TabIndex = 10;
            this.pictureBoxLogo.TabStop = false;
            // 
            // textBoxOtherInfo
            // 
            this.textBoxOtherInfo.BackColor = System.Drawing.Color.White;
            this.textBoxOtherInfo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOtherInfo.Location = new System.Drawing.Point(15, 147);
            this.textBoxOtherInfo.Multiline = true;
            this.textBoxOtherInfo.Name = "textBoxOtherInfo";
            this.textBoxOtherInfo.ReadOnly = true;
            this.textBoxOtherInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxOtherInfo.Size = new System.Drawing.Size(544, 125);
            this.textBoxOtherInfo.TabIndex = 11;
            this.textBoxOtherInfo.TabStop = false;
            this.textBoxOtherInfo.Text = "Presage Software used for word prediction";
            // 
            // labelCopyrightInfo
            // 
            this.labelCopyrightInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelCopyrightInfo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCopyrightInfo.ForeColor = System.Drawing.Color.Black;
            this.labelCopyrightInfo.Location = new System.Drawing.Point(149, 86);
            this.labelCopyrightInfo.Name = "labelCopyrightInfo";
            this.labelCopyrightInfo.Size = new System.Drawing.Size(407, 58);
            this.labelCopyrightInfo.TabIndex = 13;
            this.labelCopyrightInfo.Text = "Copyright 2013-2015 Intel Corporation";
            this.labelCopyrightInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersionInfo
            // 
            this.labelVersionInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelVersionInfo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionInfo.ForeColor = System.Drawing.Color.Black;
            this.labelVersionInfo.Location = new System.Drawing.Point(149, 50);
            this.labelVersionInfo.Name = "labelVersionInfo";
            this.labelVersionInfo.Size = new System.Drawing.Size(407, 34);
            this.labelVersionInfo.TabIndex = 14;
            this.labelVersionInfo.Text = "Version 1.00.00";
            this.labelVersionInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 357);
            this.ControlBox = false;
            this.Controls.Add(this.labelVersionInfo);
            this.Controls.Add(this.labelCopyrightInfo);
            this.Controls.Add(this.textBoxOtherInfo);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelAppTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AboutBoxForm";
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelAppTitle;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.TextBox textBoxOtherInfo;
        private System.Windows.Forms.Label labelCopyrightInfo;
        private System.Windows.Forms.Label labelVersionInfo;

    }
}