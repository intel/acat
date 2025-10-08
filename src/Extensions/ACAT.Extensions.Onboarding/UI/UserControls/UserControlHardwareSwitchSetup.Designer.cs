using ACAT.Core.WidgetManagement;

namespace ACAT.Extensions.Onboarding.UI.UserControls
{
    partial class UserControlHardwareSwitchSetup
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
                webBrowser.Navigating -= webBrowser_Navigating;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlHardwareSwitchSetup));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonF12 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF11 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF10 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF9 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF8 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF7 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF6 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF5 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF4 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF3 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonF2 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.labelPrompt = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonF1 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.scannerPanel1 = new ACAT.Core.WidgetManagement.ScannerPanel();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
            this.tableLayoutPanelMain.Controls.Add(this.buttonF12, 8, 7);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF11, 7, 7);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF10, 6, 7);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF9, 5, 7);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF8, 4, 7);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF7, 3, 7);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF6, 8, 6);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF5, 7, 6);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF4, 6, 6);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF3, 5, 6);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF2, 4, 6);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowser, 3, 10);
            this.tableLayoutPanelMain.Controls.Add(this.labelPrompt, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.buttonF1, 3, 6);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel1, 3, 8);
            this.tableLayoutPanelMain.Controls.Add(this.scannerPanel1, 0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            // 
            // buttonF12
            // 
            this.buttonF12.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF12.BorderRadiusBottomLeft = 10;
            this.buttonF12.BorderRadiusBottomRight = 10;
            this.buttonF12.BorderRadiusTopLeft = 10;
            this.buttonF12.BorderRadiusTopRight = 10;
            this.buttonF12.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF12, "buttonF12");
            this.buttonF12.ForeColor = System.Drawing.Color.White;
            this.buttonF12.Name = "buttonF12";
            this.buttonF12.UseMnemonic = false;
            this.buttonF12.UseVisualStyleBackColor = true;
            this.buttonF12.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF11
            // 
            this.buttonF11.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF11.BorderRadiusBottomLeft = 10;
            this.buttonF11.BorderRadiusBottomRight = 10;
            this.buttonF11.BorderRadiusTopLeft = 10;
            this.buttonF11.BorderRadiusTopRight = 10;
            this.buttonF11.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF11, "buttonF11");
            this.buttonF11.ForeColor = System.Drawing.Color.White;
            this.buttonF11.Name = "buttonF11";
            this.buttonF11.UseMnemonic = false;
            this.buttonF11.UseVisualStyleBackColor = true;
            this.buttonF11.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF10
            // 
            this.buttonF10.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF10.BorderRadiusBottomLeft = 10;
            this.buttonF10.BorderRadiusBottomRight = 10;
            this.buttonF10.BorderRadiusTopLeft = 10;
            this.buttonF10.BorderRadiusTopRight = 10;
            this.buttonF10.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF10, "buttonF10");
            this.buttonF10.ForeColor = System.Drawing.Color.White;
            this.buttonF10.Name = "buttonF10";
            this.buttonF10.UseMnemonic = false;
            this.buttonF10.UseVisualStyleBackColor = true;
            this.buttonF10.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF9
            // 
            this.buttonF9.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF9.BorderRadiusBottomLeft = 10;
            this.buttonF9.BorderRadiusBottomRight = 10;
            this.buttonF9.BorderRadiusTopLeft = 10;
            this.buttonF9.BorderRadiusTopRight = 10;
            this.buttonF9.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF9, "buttonF9");
            this.buttonF9.ForeColor = System.Drawing.Color.White;
            this.buttonF9.Name = "buttonF9";
            this.buttonF9.UseMnemonic = false;
            this.buttonF9.UseVisualStyleBackColor = true;
            this.buttonF9.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF8
            // 
            this.buttonF8.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF8.BorderRadiusBottomLeft = 10;
            this.buttonF8.BorderRadiusBottomRight = 10;
            this.buttonF8.BorderRadiusTopLeft = 10;
            this.buttonF8.BorderRadiusTopRight = 10;
            this.buttonF8.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF8, "buttonF8");
            this.buttonF8.ForeColor = System.Drawing.Color.White;
            this.buttonF8.Name = "buttonF8";
            this.buttonF8.UseMnemonic = false;
            this.buttonF8.UseVisualStyleBackColor = true;
            this.buttonF8.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF7
            // 
            this.buttonF7.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF7.BorderRadiusBottomLeft = 10;
            this.buttonF7.BorderRadiusBottomRight = 10;
            this.buttonF7.BorderRadiusTopLeft = 10;
            this.buttonF7.BorderRadiusTopRight = 10;
            this.buttonF7.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF7, "buttonF7");
            this.buttonF7.ForeColor = System.Drawing.Color.White;
            this.buttonF7.Name = "buttonF7";
            this.buttonF7.UseMnemonic = false;
            this.buttonF7.UseVisualStyleBackColor = true;
            this.buttonF7.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF6
            // 
            this.buttonF6.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF6.BorderRadiusBottomLeft = 10;
            this.buttonF6.BorderRadiusBottomRight = 10;
            this.buttonF6.BorderRadiusTopLeft = 10;
            this.buttonF6.BorderRadiusTopRight = 10;
            this.buttonF6.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF6, "buttonF6");
            this.buttonF6.ForeColor = System.Drawing.Color.White;
            this.buttonF6.Name = "buttonF6";
            this.buttonF6.UseMnemonic = false;
            this.buttonF6.UseVisualStyleBackColor = true;
            this.buttonF6.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF5
            // 
            this.buttonF5.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF5.BorderRadiusBottomLeft = 10;
            this.buttonF5.BorderRadiusBottomRight = 10;
            this.buttonF5.BorderRadiusTopLeft = 10;
            this.buttonF5.BorderRadiusTopRight = 10;
            this.buttonF5.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF5, "buttonF5");
            this.buttonF5.ForeColor = System.Drawing.Color.White;
            this.buttonF5.Name = "buttonF5";
            this.buttonF5.UseMnemonic = false;
            this.buttonF5.UseVisualStyleBackColor = true;
            this.buttonF5.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF4
            // 
            this.buttonF4.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF4.BorderRadiusBottomLeft = 10;
            this.buttonF4.BorderRadiusBottomRight = 10;
            this.buttonF4.BorderRadiusTopLeft = 10;
            this.buttonF4.BorderRadiusTopRight = 10;
            this.buttonF4.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF4, "buttonF4");
            this.buttonF4.ForeColor = System.Drawing.Color.White;
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.UseMnemonic = false;
            this.buttonF4.UseVisualStyleBackColor = true;
            this.buttonF4.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF3
            // 
            this.buttonF3.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF3.BorderRadiusBottomLeft = 10;
            this.buttonF3.BorderRadiusBottomRight = 10;
            this.buttonF3.BorderRadiusTopLeft = 10;
            this.buttonF3.BorderRadiusTopRight = 10;
            this.buttonF3.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF3, "buttonF3");
            this.buttonF3.ForeColor = System.Drawing.Color.White;
            this.buttonF3.Name = "buttonF3";
            this.buttonF3.UseMnemonic = false;
            this.buttonF3.UseVisualStyleBackColor = true;
            this.buttonF3.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // buttonF2
            // 
            this.buttonF2.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF2.BorderRadiusBottomLeft = 10;
            this.buttonF2.BorderRadiusBottomRight = 10;
            this.buttonF2.BorderRadiusTopLeft = 10;
            this.buttonF2.BorderRadiusTopRight = 10;
            this.buttonF2.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF2, "buttonF2");
            this.buttonF2.ForeColor = System.Drawing.Color.White;
            this.buttonF2.Name = "buttonF2";
            this.buttonF2.UseMnemonic = false;
            this.buttonF2.UseVisualStyleBackColor = true;
            this.buttonF2.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // webBrowser
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.webBrowser, 6);
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScrollBarsEnabled = false;
            // 
            // labelPrompt
            // 
            this.labelPrompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelPrompt, 10);
            resources.ApplyResources(this.labelPrompt, "labelPrompt");
            this.labelPrompt.ForeColor = System.Drawing.Color.White;
            this.labelPrompt.Name = "labelPrompt";
            this.tableLayoutPanelMain.SetRowSpan(this.labelPrompt, 3);
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelTitle, 10);
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Name = "labelTitle";
            this.tableLayoutPanelMain.SetRowSpan(this.labelTitle, 2);
            // 
            // buttonF1
            // 
            this.buttonF1.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonF1.BorderRadiusBottomLeft = 10;
            this.buttonF1.BorderRadiusBottomRight = 10;
            this.buttonF1.BorderRadiusTopLeft = 10;
            this.buttonF1.BorderRadiusTopRight = 10;
            this.buttonF1.BorderWidth = 3F;
            resources.ApplyResources(this.buttonF1, "buttonF1");
            this.buttonF1.ForeColor = System.Drawing.Color.White;
            this.buttonF1.Name = "buttonF1";
            this.buttonF1.UseMnemonic = false;
            this.buttonF1.UseVisualStyleBackColor = true;
            this.buttonF1.Click += new System.EventHandler(this.button_FunctionKeyClick);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanel1, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // scannerPanel1
            // 
            this.scannerPanel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.scannerPanel1, "scannerPanel1");
            this.scannerPanel1.Name = "scannerPanel1";
            // 
            // UserControlHardwareSwitchSetup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "UserControlHardwareSwitchSetup";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelPrompt;
        private ScannerRoundedButtonControl buttonF12;
        private ScannerRoundedButtonControl buttonF11;
        private ScannerRoundedButtonControl buttonF10;
        private ScannerRoundedButtonControl buttonF9;
        private ScannerRoundedButtonControl buttonF8;
        private ScannerRoundedButtonControl buttonF7;
        private ScannerRoundedButtonControl buttonF6;
        private ScannerRoundedButtonControl buttonF5;
        private ScannerRoundedButtonControl buttonF4;
        private ScannerRoundedButtonControl buttonF3;
        private ScannerRoundedButtonControl buttonF2;
        private System.Windows.Forms.WebBrowser webBrowser;
        private ScannerRoundedButtonControl buttonF1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScannerPanel scannerPanel1;
    }
}
