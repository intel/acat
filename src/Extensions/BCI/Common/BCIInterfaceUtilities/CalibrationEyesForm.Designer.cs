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
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.panelTriggerBox, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel2, 3);
            // 
            // panelTriggerBox
            // 
            this.panelTriggerBox.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.panelTriggerBox, "panelTriggerBox");
            this.panelTriggerBox.Name = "panelTriggerBox";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.panelPictures, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelInstruction, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.ButtonExit, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.ButtonCancel, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.ButtonStart, 5, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
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
            resources.ApplyResources(this.ButtonExit, "ButtonExit");
            this.ButtonExit.Name = "ButtonExit";
            this.ButtonExit.UseMnemonic = false;
            this.ButtonExit.UseVisualStyleBackColor = false;
            this.ButtonExit.Click += new System.EventHandler(this.ButtonExit_Click);
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
            resources.ApplyResources(this.ButtonCancel, "ButtonCancel");
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.UseMnemonic = false;
            this.ButtonCancel.UseVisualStyleBackColor = false;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click_1);
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
            resources.ApplyResources(this.ButtonStart, "ButtonStart");
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.UseMnemonic = false;
            this.ButtonStart.UseVisualStyleBackColor = false;
            this.ButtonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // panelPictures
            // 
            this.panelPictures.Controls.Add(this.labelCountdown);
            this.panelPictures.Controls.Add(this.pictureBoxEyesOpen);
            this.panelPictures.Controls.Add(this.pictureBoxEyesClosed);
            resources.ApplyResources(this.panelPictures, "panelPictures");
            this.panelPictures.Name = "panelPictures";
            // 
            // labelCountdown
            // 
            resources.ApplyResources(this.labelCountdown, "labelCountdown");
            this.labelCountdown.ForeColor = System.Drawing.Color.White;
            this.labelCountdown.Name = "labelCountdown";
            // 
            // pictureBoxEyesOpen
            // 
            resources.ApplyResources(this.pictureBoxEyesOpen, "pictureBoxEyesOpen");
            this.pictureBoxEyesOpen.Name = "pictureBoxEyesOpen";
            this.pictureBoxEyesOpen.TabStop = false;
            // 
            // pictureBoxEyesClosed
            // 
            resources.ApplyResources(this.pictureBoxEyesClosed, "pictureBoxEyesClosed");
            this.pictureBoxEyesClosed.Name = "pictureBoxEyesClosed";
            this.pictureBoxEyesClosed.TabStop = false;
            // 
            // labelInstruction
            // 
            resources.ApplyResources(this.labelInstruction, "labelInstruction");
            this.labelInstruction.BackColor = System.Drawing.Color.Transparent;
            this.labelInstruction.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelInstruction.Name = "labelInstruction";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.ButtonClose, 2, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // ButtonClose
            // 
            this.ButtonClose.BorderColor = System.Drawing.Color.Transparent;
            this.ButtonClose.BorderRadiusBottomLeft = 25;
            this.ButtonClose.BorderRadiusBottomRight = 25;
            this.ButtonClose.BorderRadiusTopLeft = 25;
            this.ButtonClose.BorderRadiusTopRight = 25;
            this.ButtonClose.BorderWidth = 3F;
            resources.ApplyResources(this.ButtonClose, "ButtonClose");
            this.ButtonClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.UseMnemonic = false;
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonCancel_Close);
            // 
            // CalibrationEyesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
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