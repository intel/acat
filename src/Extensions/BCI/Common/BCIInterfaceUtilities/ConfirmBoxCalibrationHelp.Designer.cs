
using ACAT.Core.WidgetManagement;
using ACATResources;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    partial class ConfirmBoxCalibrationHelp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmBoxCalibrationHelp));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.webBrowserCalibrationHelp = new System.Windows.Forms.WebBrowser();
            this.checkBoxDontShowAgain = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOk = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.webBrowserCalibrationHelp, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxDontShowAgain, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelTitle
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelTitle, 5);
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Text = StringResources.CalibrationHelp;
            // 
            // webBrowserCalibrationHelp
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.webBrowserCalibrationHelp, 5);
            resources.ApplyResources(this.webBrowserCalibrationHelp, "webBrowserCalibrationHelp");
            this.webBrowserCalibrationHelp.Name = "webBrowserCalibrationHelp";
            this.webBrowserCalibrationHelp.ScrollBarsEnabled = false;
            // 
            // checkBoxDontShowAgain
            // 
            resources.ApplyResources(this.checkBoxDontShowAgain, "checkBoxDontShowAgain");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxDontShowAgain, 3);
            this.checkBoxDontShowAgain.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
            this.checkBoxDontShowAgain.Text = StringResources.CalibrationHelp;
            this.checkBoxDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.buttonOk, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOk.BorderColor = System.Drawing.Color.Transparent;
            this.buttonOk.BorderRadiusBottomLeft = 0;
            this.buttonOk.BorderRadiusBottomRight = 0;
            this.buttonOk.BorderRadiusTopLeft = 0;
            this.buttonOk.BorderRadiusTopRight = 0;
            this.buttonOk.BorderWidth = 0F;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.ForeColor = System.Drawing.Color.Black;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Text = StringResources.OK;
            this.buttonOk.UseMnemonic = false;
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOp3_Click);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.panelMain.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // ConfirmBoxCalibrationHelp
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ControlBox = false;
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfirmBoxCalibrationHelp";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.WebBrowser webBrowserCalibrationHelp;
        private System.Windows.Forms.CheckBox checkBoxDontShowAgain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ScannerRoundedButtonControl buttonOk;
        private System.Windows.Forms.Panel panelMain;
    }
}