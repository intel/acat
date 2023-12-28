namespace ACAT.Lib.Extension

{
    partial class UserControlLayoutInterface
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlLayoutInterface));
            this.label1 = new System.Windows.Forms.Label();
            this.labelScanSpeedSelect3 = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.scannerRoundedButtonControl1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.checkBoxDontShowThisOnStartup = new System.Windows.Forms.CheckBox();
            this.pictureBoxInterface = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInterface)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.label1, 9);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Montserrat Medium", 45.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(96, 0);
            this.label1.Name = "label1";
            this.tableLayoutPanelMain.SetRowSpan(this.label1, 2);
            this.label1.Size = new System.Drawing.Size(831, 128);
            this.label1.TabIndex = 0;
            this.label1.Text = "App UI Layout";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelScanSpeedSelect3
            // 
            this.labelScanSpeedSelect3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelScanSpeedSelect3, 9);
            this.labelScanSpeedSelect3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelScanSpeedSelect3.Font = new System.Drawing.Font("Montserrat Medium", 16F);
            this.labelScanSpeedSelect3.ForeColor = System.Drawing.Color.White;
            this.labelScanSpeedSelect3.Location = new System.Drawing.Point(93, 128);
            this.labelScanSpeedSelect3.Margin = new System.Windows.Forms.Padding(0);
            this.labelScanSpeedSelect3.Name = "labelScanSpeedSelect3";
            this.labelScanSpeedSelect3.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelScanSpeedSelect3.Size = new System.Drawing.Size(837, 64);
            this.labelScanSpeedSelect3.TabIndex = 71;
            this.labelScanSpeedSelect3.Text = "Shown below is the layout of the main window of the application ";
            this.labelScanSpeedSelect3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 11;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090908F));
            this.tableLayoutPanelMain.Controls.Add(this.labelScanSpeedSelect3, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.scannerRoundedButtonControl1, 9, 11);
            this.tableLayoutPanelMain.Controls.Add(this.checkBoxDontShowThisOnStartup, 4, 11);
            this.tableLayoutPanelMain.Controls.Add(this.pictureBoxInterface, 1, 3);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 12;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.333332F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // scannerRoundedButtonControl1
            // 
            this.scannerRoundedButtonControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.scannerRoundedButtonControl1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.scannerRoundedButtonControl1.BorderRadiusBottomLeft = 0;
            this.scannerRoundedButtonControl1.BorderRadiusBottomRight = 0;
            this.scannerRoundedButtonControl1.BorderRadiusTopLeft = 0;
            this.scannerRoundedButtonControl1.BorderRadiusTopRight = 0;
            this.scannerRoundedButtonControl1.BorderWidth = 0F;
            this.tableLayoutPanelMain.SetColumnSpan(this.scannerRoundedButtonControl1, 2);
            this.scannerRoundedButtonControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerRoundedButtonControl1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scannerRoundedButtonControl1.Font = new System.Drawing.Font("Montserrat", 21F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scannerRoundedButtonControl1.ForeColor = System.Drawing.Color.Black;
            this.scannerRoundedButtonControl1.Location = new System.Drawing.Point(840, 707);
            this.scannerRoundedButtonControl1.Name = "scannerRoundedButtonControl1";
            this.scannerRoundedButtonControl1.Size = new System.Drawing.Size(181, 58);
            this.scannerRoundedButtonControl1.TabIndex = 82;
            this.scannerRoundedButtonControl1.Text = "Next";
            this.scannerRoundedButtonControl1.UseMnemonic = false;
            this.scannerRoundedButtonControl1.UseVisualStyleBackColor = false;
            this.scannerRoundedButtonControl1.Click += new System.EventHandler(this.buttonDone_Click);
            // 
            // checkBoxDontShowThisOnStartup
            // 
            this.checkBoxDontShowThisOnStartup.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxDontShowThisOnStartup, 4);
            this.checkBoxDontShowThisOnStartup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxDontShowThisOnStartup.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxDontShowThisOnStartup.ForeColor = System.Drawing.Color.White;
            this.checkBoxDontShowThisOnStartup.Location = new System.Drawing.Point(375, 707);
            this.checkBoxDontShowThisOnStartup.Name = "checkBoxDontShowThisOnStartup";
            this.checkBoxDontShowThisOnStartup.Size = new System.Drawing.Size(366, 58);
            this.checkBoxDontShowThisOnStartup.TabIndex = 83;
            this.checkBoxDontShowThisOnStartup.Text = "Don\'t show this again";
            this.checkBoxDontShowThisOnStartup.UseVisualStyleBackColor = true;
            // 
            // pictureBoxInterface
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.pictureBoxInterface, 9);
            this.pictureBoxInterface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxInterface.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxInterface.Image")));
            this.pictureBoxInterface.Location = new System.Drawing.Point(95, 194);
            this.pictureBoxInterface.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxInterface.Name = "pictureBoxInterface";
            this.tableLayoutPanelMain.SetRowSpan(this.pictureBoxInterface, 7);
            this.pictureBoxInterface.Size = new System.Drawing.Size(833, 444);
            this.pictureBoxInterface.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxInterface.TabIndex = 84;
            this.pictureBoxInterface.TabStop = false;
            // 
            // UserControlLayoutInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlLayoutInterface";
            this.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInterface)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelScanSpeedSelect3;
        private System.Windows.Forms.Label label1;
        private Core.WidgetManagement.ScannerRoundedButtonControl scannerRoundedButtonControl1;
        private System.Windows.Forms.CheckBox checkBoxDontShowThisOnStartup;
        private System.Windows.Forms.PictureBox pictureBoxInterface;
    }
}
