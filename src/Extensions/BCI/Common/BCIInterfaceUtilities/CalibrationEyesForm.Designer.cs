using ACATResources;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    partial class CalibrationEyesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationEyesForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panelTriggerBox = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonExit = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.ButtonCancel = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.ButtonStart = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.panelPictures = new System.Windows.Forms.Panel();
            this.labelCountdown = new System.Windows.Forms.Label();
            this.pictureBoxEyesOpen = new System.Windows.Forms.PictureBox();
            this.pictureBoxEyesClosed = new System.Windows.Forms.PictureBox();
            this.labelInstruction = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonClose = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panelPictures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEyesOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEyesClosed)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2048F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1519F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(3848, 2040);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panelTriggerBox, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 308F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(888, 1767);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // panelTriggerBox
            // 
            this.panelTriggerBox.BackColor = System.Drawing.Color.Black;
            this.panelTriggerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTriggerBox.Location = new System.Drawing.Point(0, 48);
            this.panelTriggerBox.Margin = new System.Windows.Forms.Padding(0);
            this.panelTriggerBox.Name = "panelTriggerBox";
            this.panelTriggerBox.Size = new System.Drawing.Size(160, 308);
            this.panelTriggerBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(900, 260);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2048, 1519);
            this.panel1.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.panelPictures, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelInstruction, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 7;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 288F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(2046, 1517);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Montserrat Medium", 36F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(2034, 250);
            this.label1.TabIndex = 1;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Text = StringResources.EyesClosedCalibration;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Montserrat Medium", 20F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(6, 1056);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2034, 173);
            this.label2.TabIndex = 2;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Text = StringResources.OpenAndCloseYourEyes;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 7;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel4.Controls.Add(this.ButtonExit, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.ButtonCancel, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.ButtonStart, 5, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 1306);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0, 77, 0, 77);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(2046, 134);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // ButtonExit
            // 
            this.ButtonExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ButtonExit.BorderColor = System.Drawing.Color.Transparent;
            this.ButtonExit.BorderRadiusBottomLeft = 5;
            this.ButtonExit.BorderRadiusBottomRight = 5;
            this.ButtonExit.BorderRadiusTopLeft = 5;
            this.ButtonExit.BorderRadiusTopRight = 5;
            this.ButtonExit.BorderWidth = 0F;
            this.ButtonExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonExit.Font = new System.Drawing.Font("Montserrat ExtraLight", 24F);
            this.ButtonExit.Location = new System.Drawing.Point(66, 6);
            this.ButtonExit.Margin = new System.Windows.Forms.Padding(6);
            this.ButtonExit.Name = "ButtonExit";
            this.ButtonExit.Size = new System.Drawing.Size(388, 122);
            this.ButtonExit.TabIndex = 4;
            this.ButtonExit.UseMnemonic = false;
            this.ButtonExit.UseVisualStyleBackColor = false;
            this.ButtonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            this.ButtonExit.Text = StringResources.Exit;

            // 
            // ButtonCancel
            // 
            this.ButtonCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ButtonCancel.BorderColor = System.Drawing.Color.Transparent;
            this.ButtonCancel.BorderRadiusBottomLeft = 5;
            this.ButtonCancel.BorderRadiusBottomRight = 5;
            this.ButtonCancel.BorderRadiusTopLeft = 5;
            this.ButtonCancel.BorderRadiusTopRight = 5;
            this.ButtonCancel.BorderWidth = 0F;
            this.ButtonCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonCancel.Font = new System.Drawing.Font("Montserrat ExtraLight", 24F);
            this.ButtonCancel.Location = new System.Drawing.Point(1132, 6);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(6);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(388, 122);
            this.ButtonCancel.TabIndex = 3;
            this.ButtonCancel.UseMnemonic = false;
            this.ButtonCancel.UseVisualStyleBackColor = false;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click_1);
            this.ButtonCancel.Text = StringResources.Cancel;
            // 
            // ButtonStart
            // 
            this.ButtonStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ButtonStart.BorderColor = System.Drawing.Color.Transparent;
            this.ButtonStart.BorderRadiusBottomLeft = 5;
            this.ButtonStart.BorderRadiusBottomRight = 5;
            this.ButtonStart.BorderRadiusTopLeft = 5;
            this.ButtonStart.BorderRadiusTopRight = 5;
            this.ButtonStart.BorderWidth = 0F;
            this.ButtonStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonStart.Font = new System.Drawing.Font("Montserrat ExtraLight", 24F);
            this.ButtonStart.Location = new System.Drawing.Point(1592, 6);
            this.ButtonStart.Margin = new System.Windows.Forms.Padding(6);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(388, 122);
            this.ButtonStart.TabIndex = 2;
            this.ButtonStart.UseMnemonic = false;
            this.ButtonStart.UseVisualStyleBackColor = false;
            this.ButtonStart.Click += new System.EventHandler(this.buttonStart_Click);
            this.ButtonStart.Text = StringResources.Start;
            // 
            // panelPictures
            // 
            this.panelPictures.Controls.Add(this.labelCountdown);
            this.panelPictures.Controls.Add(this.pictureBoxEyesOpen);
            this.panelPictures.Controls.Add(this.pictureBoxEyesClosed);
            this.panelPictures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPictures.Location = new System.Drawing.Point(6, 467);
            this.panelPictures.Margin = new System.Windows.Forms.Padding(6);
            this.panelPictures.Name = "panelPictures";
            this.panelPictures.Padding = new System.Windows.Forms.Padding(100, 96, 100, 96);
            this.panelPictures.Size = new System.Drawing.Size(2034, 545);
            this.panelPictures.TabIndex = 4;
            // 
            // labelCountdown
            // 
            this.labelCountdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCountdown.Font = new System.Drawing.Font("Montserrat SemiBold", 50F);
            this.labelCountdown.ForeColor = System.Drawing.Color.White;
            this.labelCountdown.Location = new System.Drawing.Point(100, 96);
            this.labelCountdown.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelCountdown.Name = "labelCountdown";
            this.labelCountdown.Size = new System.Drawing.Size(1834, 353);
            this.labelCountdown.TabIndex = 2;
            this.labelCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxEyesOpen
            // 
            this.pictureBoxEyesOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxEyesOpen.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxEyesOpen.Image")));
            this.pictureBoxEyesOpen.Location = new System.Drawing.Point(100, 96);
            this.pictureBoxEyesOpen.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBoxEyesOpen.Name = "pictureBoxEyesOpen";
            this.pictureBoxEyesOpen.Size = new System.Drawing.Size(1834, 353);
            this.pictureBoxEyesOpen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxEyesOpen.TabIndex = 1;
            this.pictureBoxEyesOpen.TabStop = false;
            // 
            // pictureBoxEyesClosed
            // 
            this.pictureBoxEyesClosed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxEyesClosed.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxEyesClosed.Image")));
            this.pictureBoxEyesClosed.Location = new System.Drawing.Point(100, 96);
            this.pictureBoxEyesClosed.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBoxEyesClosed.Name = "pictureBoxEyesClosed";
            this.pictureBoxEyesClosed.Size = new System.Drawing.Size(1834, 353);
            this.pictureBoxEyesClosed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxEyesClosed.TabIndex = 0;
            this.pictureBoxEyesClosed.TabStop = false;
            // 
            // labelInstruction
            // 
            this.labelInstruction.AutoSize = true;
            this.labelInstruction.BackColor = System.Drawing.Color.Transparent;
            this.labelInstruction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelInstruction.Font = new System.Drawing.Font("Montserrat Light", 30F);
            this.labelInstruction.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelInstruction.Location = new System.Drawing.Point(6, 250);
            this.labelInstruction.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelInstruction.Name = "labelInstruction";
            this.labelInstruction.Size = new System.Drawing.Size(2034, 173);
            this.labelInstruction.TabIndex = 5;
            this.labelInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelInstruction.Text = StringResources.EyesOpen;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel5.Controls.Add(this.ButtonClose, 2, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(2954, 6);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(888, 118);
            this.tableLayoutPanel5.TabIndex = 5;
            // 
            // ButtonClose
            // 
            this.ButtonClose.BorderColor = System.Drawing.Color.Transparent;
            this.ButtonClose.BorderRadiusBottomLeft = 25;
            this.ButtonClose.BorderRadiusBottomRight = 25;
            this.ButtonClose.BorderRadiusTopLeft = 25;
            this.ButtonClose.BorderRadiusTopRight = 25;
            this.ButtonClose.BorderWidth = 3F;
            this.ButtonClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonClose.Font = new System.Drawing.Font("Montserrat ExtraLight", 25F);
            this.ButtonClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonClose.Location = new System.Drawing.Point(728, 0);
            this.ButtonClose.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(160, 118);
            this.ButtonClose.TabIndex = 0;
            this.ButtonClose.UseMnemonic = false;
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonCancel_Close);
            this.ButtonClose.Text = StringResources.X;
            //
            // CalibrationEyesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(3848, 2040);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "CalibrationEyesForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationEyesForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panelPictures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEyesOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEyesClosed)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelTriggerBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonStart;
        private System.Windows.Forms.Panel panelPictures;
        private System.Windows.Forms.PictureBox pictureBoxEyesOpen;
        private System.Windows.Forms.PictureBox pictureBoxEyesClosed;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonClose;
        private System.Windows.Forms.Label labelInstruction;
        private System.Windows.Forms.Label labelCountdown;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonCancel;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonExit;
    }
}