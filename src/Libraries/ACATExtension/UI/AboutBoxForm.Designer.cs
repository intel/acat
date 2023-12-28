namespace ACAT.Lib.Extension
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCopyrightInfo = new System.Windows.Forms.Label();
            this.labelVersionInfo = new System.Windows.Forms.Label();
            this.labelURL = new System.Windows.Forms.Label();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonDisclaimer = new System.Windows.Forms.Button();
            this.buttonLicenses = new System.Windows.Forms.Button();
            this.buttonOK = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelCopyrightInfo, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelVersionInfo, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelURL, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAppTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(723, 537);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // labelCopyrightInfo
            // 
            this.labelCopyrightInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelCopyrightInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyrightInfo.Font = new System.Drawing.Font("Montserrat Medium", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCopyrightInfo.ForeColor = System.Drawing.Color.White;
            this.labelCopyrightInfo.Location = new System.Drawing.Point(25, 212);
            this.labelCopyrightInfo.Margin = new System.Windows.Forms.Padding(25, 0, 25, 0);
            this.labelCopyrightInfo.Name = "labelCopyrightInfo";
            this.labelCopyrightInfo.Size = new System.Drawing.Size(673, 78);
            this.labelCopyrightInfo.TabIndex = 32;
            this.labelCopyrightInfo.Text = "Copyright";
            this.labelCopyrightInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersionInfo
            // 
            this.labelVersionInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelVersionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersionInfo.Font = new System.Drawing.Font("Montserrat Light", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionInfo.ForeColor = System.Drawing.Color.White;
            this.labelVersionInfo.Location = new System.Drawing.Point(4, 290);
            this.labelVersionInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVersionInfo.Name = "labelVersionInfo";
            this.labelVersionInfo.Size = new System.Drawing.Size(715, 62);
            this.labelVersionInfo.TabIndex = 28;
            this.labelVersionInfo.Text = "Version 1.00.00";
            this.labelVersionInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelURL
            // 
            this.labelURL.BackColor = System.Drawing.Color.Transparent;
            this.labelURL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelURL.Font = new System.Drawing.Font("Montserrat SemiBold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelURL.ForeColor = System.Drawing.Color.White;
            this.labelURL.Location = new System.Drawing.Point(4, 150);
            this.labelURL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelURL.Name = "labelURL";
            this.labelURL.Size = new System.Drawing.Size(715, 49);
            this.labelURL.TabIndex = 26;
            this.labelURL.Text = "http://01.org/acat";
            this.labelURL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelAppTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAppTitle.Font = new System.Drawing.Font("Montserrat Black", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAppTitle.ForeColor = System.Drawing.Color.White;
            this.labelAppTitle.Location = new System.Drawing.Point(4, 18);
            this.labelAppTitle.Margin = new System.Windows.Forms.Padding(4, 18, 4, 0);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(715, 132);
            this.labelAppTitle.TabIndex = 19;
            this.labelAppTitle.Text = "Assistive Context-Aware Toolkit (ACAT)";
            this.labelAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 5;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.40647F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.43187F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.81755F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.71362F));
            this.tableLayoutPanel4.Controls.Add(this.buttonDisclaimer, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.buttonLicenses, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.buttonOK, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 418);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(723, 78);
            this.tableLayoutPanel4.TabIndex = 30;
            // 
            // buttonDisclaimer
            // 
            this.buttonDisclaimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.buttonDisclaimer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonDisclaimer.FlatAppearance.BorderSize = 0;
            this.buttonDisclaimer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDisclaimer.Font = new System.Drawing.Font("Montserrat Light", 21.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDisclaimer.ForeColor = System.Drawing.Color.Silver;
            this.buttonDisclaimer.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonDisclaimer.Location = new System.Drawing.Point(522, 0);
            this.buttonDisclaimer.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDisclaimer.Name = "buttonDisclaimer";
            this.buttonDisclaimer.Size = new System.Drawing.Size(201, 78);
            this.buttonDisclaimer.TabIndex = 4;
            this.buttonDisclaimer.TabStop = false;
            this.buttonDisclaimer.Text = "Disclaimers";
            this.buttonDisclaimer.UseCompatibleTextRendering = true;
            this.buttonDisclaimer.UseVisualStyleBackColor = false;
            this.buttonDisclaimer.Click += new System.EventHandler(this.buttonDisclaimer_Click);
            // 
            // buttonLicenses
            // 
            this.buttonLicenses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.buttonLicenses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonLicenses.FlatAppearance.BorderSize = 0;
            this.buttonLicenses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLicenses.Font = new System.Drawing.Font("Montserrat Light", 21.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLicenses.ForeColor = System.Drawing.Color.Silver;
            this.buttonLicenses.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonLicenses.Location = new System.Drawing.Point(11, 0);
            this.buttonLicenses.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLicenses.Name = "buttonLicenses";
            this.buttonLicenses.Size = new System.Drawing.Size(181, 78);
            this.buttonLicenses.TabIndex = 3;
            this.buttonLicenses.TabStop = false;
            this.buttonLicenses.Text = "Licenses";
            this.buttonLicenses.UseCompatibleTextRendering = true;
            this.buttonLicenses.UseVisualStyleBackColor = false;
            this.buttonLicenses.Click += new System.EventHandler(this.buttonLicenses_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOK.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOK.BorderRadiusBottomLeft = 0;
            this.buttonOK.BorderRadiusBottomRight = 0;
            this.buttonOK.BorderRadiusTopLeft = 0;
            this.buttonOK.BorderRadiusTopRight = 0;
            this.buttonOK.BorderWidth = 3F;
            this.buttonOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.Font = new System.Drawing.Font("Montserrat", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.Location = new System.Drawing.Point(290, 4);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(136, 70);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseMnemonic = false;
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(717, 13);
            this.label1.TabIndex = 31;
            // 
            // AboutBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(723, 537);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "AboutBoxForm";
            this.Text = "About";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelURL;
        private System.Windows.Forms.Label labelAppTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Core.WidgetManagement.ScannerRoundedButtonControl buttonOK;
        private System.Windows.Forms.Label labelCopyrightInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelVersionInfo;
        private System.Windows.Forms.Button buttonLicenses;
        private System.Windows.Forms.Button buttonDisclaimer;
    }
}