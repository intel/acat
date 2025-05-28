using ACATResources;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    partial class CalibrationEyesSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationEyesSettingsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonExit = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.ButtonSave = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutIterationPerTarget = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutMinScore = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutScanningTime = new System.Windows.Forms.TableLayoutPanel();
            this.BtnDownInterval = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.BtnUpInterval = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutTargets = new System.Windows.Forms.TableLayoutPanel();
            this.BtnDownRepetitions = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.BtnUpRepetitions = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.textBoxReps = new System.Windows.Forms.TextBox();
            this.labelScanTime = new System.Windows.Forms.Label();
            this.labelTargets = new System.Windows.Forms.Label();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonClose = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutScanningTime.SuspendLayout();
            this.tableLayoutTargets.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel2, 3);
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
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.panelOptions, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel7, 0, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            this.label1.Text = StringResources.EyesClosedCalibration;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.ButtonExit, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.ButtonSave, 5, 0);
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
            this.ButtonExit.Text = StringResources.Exit;
            resources.ApplyResources(this.ButtonExit, "ButtonExit");
            this.ButtonExit.Name = "ButtonExit";
            this.ButtonExit.UseMnemonic = false;
            this.ButtonExit.UseVisualStyleBackColor = false;
            this.ButtonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // ButtonSave
            // 
            this.ButtonSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ButtonSave.BorderColor = System.Drawing.Color.Transparent;
            this.ButtonSave.BorderRadiusBottomLeft = 5;
            this.ButtonSave.BorderRadiusBottomRight = 5;
            this.ButtonSave.BorderRadiusTopLeft = 5;
            this.ButtonSave.BorderRadiusTopRight = 5;
            this.ButtonSave.BorderWidth = 0F;
            resources.ApplyResources(this.ButtonSave, "ButtonSave");
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Text = StringResources.Save;
            this.ButtonSave.UseMnemonic = false;
            this.ButtonSave.UseVisualStyleBackColor = false;
            this.ButtonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.tableLayoutPanel6);
            resources.ApplyResources(this.panelOptions, "panelOptions");
            this.panelOptions.Name = "panelOptions";
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutIterationPerTarget, 2, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutMinScore, 2, 3);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutScanningTime, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutTargets, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.textBoxInterval, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.textBoxReps, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.labelScanTime, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.labelTargets, 0, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // tableLayoutIterationPerTarget
            // 
            resources.ApplyResources(this.tableLayoutIterationPerTarget, "tableLayoutIterationPerTarget");
            this.tableLayoutIterationPerTarget.Name = "tableLayoutIterationPerTarget";
            // 
            // tableLayoutMinScore
            // 
            resources.ApplyResources(this.tableLayoutMinScore, "tableLayoutMinScore");
            this.tableLayoutMinScore.Name = "tableLayoutMinScore";
            // 
            // tableLayoutScanningTime
            // 
            resources.ApplyResources(this.tableLayoutScanningTime, "tableLayoutScanningTime");
            this.tableLayoutScanningTime.Controls.Add(this.BtnDownInterval, 0, 1);
            this.tableLayoutScanningTime.Controls.Add(this.BtnUpInterval, 0, 0);
            this.tableLayoutScanningTime.Name = "tableLayoutScanningTime";
            // 
            // BtnDownInterval
            // 
            resources.ApplyResources(this.BtnDownInterval, "BtnDownInterval");
            this.BtnDownInterval.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.BtnDownInterval.BorderRadiusBottomLeft = 25;
            this.BtnDownInterval.BorderRadiusBottomRight = 25;
            this.BtnDownInterval.BorderRadiusTopLeft = 25;
            this.BtnDownInterval.BorderRadiusTopRight = 25;
            this.BtnDownInterval.BorderWidth = 3F;
            this.BtnDownInterval.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnDownInterval.Name = "BtnDownInterval";
            this.BtnDownInterval.UseMnemonic = false;
            this.BtnDownInterval.UseVisualStyleBackColor = true;
            this.BtnDownInterval.Click += new System.EventHandler(this.BtnDownInterval_Click);
            // 
            // BtnUpInterval
            // 
            resources.ApplyResources(this.BtnUpInterval, "BtnUpInterval");
            this.BtnUpInterval.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.BtnUpInterval.BorderRadiusBottomLeft = 25;
            this.BtnUpInterval.BorderRadiusBottomRight = 25;
            this.BtnUpInterval.BorderRadiusTopLeft = 25;
            this.BtnUpInterval.BorderRadiusTopRight = 25;
            this.BtnUpInterval.BorderWidth = 3F;
            this.BtnUpInterval.ForeColor = System.Drawing.Color.White;
            this.BtnUpInterval.Name = "BtnUpInterval";
            this.BtnUpInterval.UseMnemonic = false;
            this.BtnUpInterval.UseVisualStyleBackColor = true;
            this.BtnUpInterval.Click += new System.EventHandler(this.BtnUpInterval_Click);
            // 
            // tableLayoutTargets
            // 
            resources.ApplyResources(this.tableLayoutTargets, "tableLayoutTargets");
            this.tableLayoutTargets.Controls.Add(this.BtnDownRepetitions, 0, 1);
            this.tableLayoutTargets.Controls.Add(this.BtnUpRepetitions, 0, 0);
            this.tableLayoutTargets.Name = "tableLayoutTargets";
            // 
            // BtnDownRepetitions
            // 
            resources.ApplyResources(this.BtnDownRepetitions, "BtnDownRepetitions");
            this.BtnDownRepetitions.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.BtnDownRepetitions.BorderRadiusBottomLeft = 25;
            this.BtnDownRepetitions.BorderRadiusBottomRight = 25;
            this.BtnDownRepetitions.BorderRadiusTopLeft = 25;
            this.BtnDownRepetitions.BorderRadiusTopRight = 25;
            this.BtnDownRepetitions.BorderWidth = 3F;
            this.BtnDownRepetitions.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnDownRepetitions.Name = "BtnDownRepetitions";
            this.BtnDownRepetitions.UseMnemonic = false;
            this.BtnDownRepetitions.UseVisualStyleBackColor = true;
            this.BtnDownRepetitions.Click += new System.EventHandler(this.BtnDownRepetitions_Click);
            // 
            // BtnUpRepetitions
            // 
            resources.ApplyResources(this.BtnUpRepetitions, "BtnUpRepetitions");
            this.BtnUpRepetitions.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.BtnUpRepetitions.BorderRadiusBottomLeft = 25;
            this.BtnUpRepetitions.BorderRadiusBottomRight = 25;
            this.BtnUpRepetitions.BorderRadiusTopLeft = 25;
            this.BtnUpRepetitions.BorderRadiusTopRight = 25;
            this.BtnUpRepetitions.BorderWidth = 3F;
            this.BtnUpRepetitions.ForeColor = System.Drawing.Color.White;
            this.BtnUpRepetitions.Name = "BtnUpRepetitions";
            this.BtnUpRepetitions.UseMnemonic = false;
            this.BtnUpRepetitions.UseVisualStyleBackColor = true;
            this.BtnUpRepetitions.Click += new System.EventHandler(this.BtnUpRepetitions_Click);
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.textBoxInterval.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxInterval, "textBoxInterval");
            this.textBoxInterval.ForeColor = System.Drawing.Color.White;
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Text = StringResources.Number5000;
            this.textBoxInterval.TextChanged += new System.EventHandler(this.textBoxInterval_TextChanged);
            // 
            // textBoxReps
            // 
            this.textBoxReps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.textBoxReps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxReps, "textBoxReps");
            this.textBoxReps.ForeColor = System.Drawing.Color.White;
            this.textBoxReps.Name = "textBoxReps";
            this.textBoxReps.Text = StringResources.Number10;
            this.textBoxReps.TextChanged += new System.EventHandler(this.textBoxReps_TextChanged);
            // 
            // labelScanTime
            // 
            resources.ApplyResources(this.labelScanTime, "labelScanTime");
            this.labelScanTime.ForeColor = System.Drawing.Color.White;
            this.labelScanTime.Name = "labelScanTime";
            this.labelScanTime.Text = StringResources.IntervalTime;
            // 
            // labelTargets
            // 
            resources.ApplyResources(this.labelTargets, "labelTargets");
            this.labelTargets.ForeColor = System.Drawing.Color.White;
            this.labelTargets.Name = "labelTargets";
            this.labelTargets.Text = StringResources.NumberofRepetitions;
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.pictureBox2, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
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
            this.ButtonClose.Text = StringResources.X;
            this.ButtonClose.UseMnemonic = false;
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonCancel_Close);
            // 
            // CalibrationEyesSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CalibrationEyesSettingsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationEyesForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panelOptions.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutScanningTime.ResumeLayout(false);
            this.tableLayoutTargets.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonSave;
        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonClose;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl ButtonExit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutIterationPerTarget;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMinScore;
        private System.Windows.Forms.TableLayoutPanel tableLayoutScanningTime;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl BtnDownInterval;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl BtnUpInterval;
        private System.Windows.Forms.Label labelScanTime;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.TextBox textBoxReps;
        private System.Windows.Forms.Label labelTargets;
        private System.Windows.Forms.TableLayoutPanel tableLayoutTargets;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl BtnDownRepetitions;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl BtnUpRepetitions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}