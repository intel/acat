////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorGTecBoard.designer.cs
//
// User control which displays information on errors related to connecting
// to the BCI gTec board
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    ///  User control which displays information on errors related to connecting to the BCI gTec board
    /// </summary>
    partial class UserControlBCIErrorGTecBoard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlBCIErrorGTecBoard));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExit_userControlBCIErrorgTecBoard = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanelBCIError = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRetry_userControlBCIErrorgTecBoard = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSpacerTop = new System.Windows.Forms.TableLayoutPanel();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.webBrowserBottom = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelBCIError.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit_userControlBCIErrorgTecBoard, 1, 11);
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelBCIError, 1, 6);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 1, 12);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerTop, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowserTop, 1, 3);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowserBottom, 1, 8);
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 13;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.140637F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.25783F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.140636F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.524978F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.524978F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.140636F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.14432F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.140636F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.524978F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.177546F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.396867F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.654111F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.140636F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.TabIndex = 9;
            // 
            // buttonExit_userControlBCIErrorgTecBoard
            // 
            this.buttonExit_userControlBCIErrorgTecBoard.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonExit_userControlBCIErrorgTecBoard.AutoSize = true;
            this.buttonExit_userControlBCIErrorgTecBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit_userControlBCIErrorgTecBoard.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit_userControlBCIErrorgTecBoard.FlatAppearance.BorderSize = 0;
            this.buttonExit_userControlBCIErrorgTecBoard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit_userControlBCIErrorgTecBoard.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExit_userControlBCIErrorgTecBoard.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit_userControlBCIErrorgTecBoard.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit_userControlBCIErrorgTecBoard.Location = new System.Drawing.Point(112, 657);
            this.buttonExit_userControlBCIErrorgTecBoard.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit_userControlBCIErrorgTecBoard.Name = "buttonExit_userControlBCIErrorgTecBoard";
            this.buttonExit_userControlBCIErrorgTecBoard.Size = new System.Drawing.Size(128, 58);
            this.buttonExit_userControlBCIErrorgTecBoard.TabIndex = 76;
            this.buttonExit_userControlBCIErrorgTecBoard.Text = "Exit";
            this.buttonExit_userControlBCIErrorgTecBoard.UseCompatibleTextRendering = true;
            this.buttonExit_userControlBCIErrorgTecBoard.UseVisualStyleBackColor = false;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelTitle, 3);
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Montserrat", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(112, 47);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(795, 86);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "BCI gTec Board Error";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.UseCompatibleTextRendering = true;
            // 
            // tableLayoutPanelBCIError
            // 
            this.tableLayoutPanelBCIError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelBCIError.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanelBCIError.ColumnCount = 3;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelBCIError, 3);
            this.tableLayoutPanelBCIError.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelBCIError.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelBCIError.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelBCIError.Controls.Add(this.buttonRetry_userControlBCIErrorgTecBoard, 2, 0);
            this.tableLayoutPanelBCIError.Font = new System.Drawing.Font("Montserrat Medium", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanelBCIError.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanelBCIError.Location = new System.Drawing.Point(112, 295);
            this.tableLayoutPanelBCIError.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelBCIError.Name = "tableLayoutPanelBCIError";
            this.tableLayoutPanelBCIError.RowCount = 1;
            this.tableLayoutPanelBCIError.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelBCIError.Size = new System.Drawing.Size(795, 200);
            this.tableLayoutPanelBCIError.TabIndex = 70;
            this.tableLayoutPanelBCIError.Text = "Row for error visualization";
            // 
            // buttonRetry_userControlBCIErrorgTecBoard
            // 
            this.buttonRetry_userControlBCIErrorgTecBoard.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonRetry_userControlBCIErrorgTecBoard.AutoSize = true;
            this.buttonRetry_userControlBCIErrorgTecBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonRetry_userControlBCIErrorgTecBoard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonRetry_userControlBCIErrorgTecBoard.BorderColor = System.Drawing.Color.Transparent;
            this.buttonRetry_userControlBCIErrorgTecBoard.BorderRadiusBottomLeft = 0;
            this.buttonRetry_userControlBCIErrorgTecBoard.BorderRadiusBottomRight = 0;
            this.buttonRetry_userControlBCIErrorgTecBoard.BorderRadiusTopLeft = 0;
            this.buttonRetry_userControlBCIErrorgTecBoard.BorderRadiusTopRight = 0;
            this.buttonRetry_userControlBCIErrorgTecBoard.BorderWidth = 0F;
            this.buttonRetry_userControlBCIErrorgTecBoard.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonRetry_userControlBCIErrorgTecBoard.FlatAppearance.BorderSize = 0;
            this.buttonRetry_userControlBCIErrorgTecBoard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRetry_userControlBCIErrorgTecBoard.Font = new System.Drawing.Font("Montserrat", 28F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRetry_userControlBCIErrorgTecBoard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonRetry_userControlBCIErrorgTecBoard.Image = ((System.Drawing.Image)(resources.GetObject("buttonRetry_userControlBCIErrorgTecBoard.Image")));
            this.buttonRetry_userControlBCIErrorgTecBoard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRetry_userControlBCIErrorgTecBoard.Location = new System.Drawing.Point(555, 69);
            this.buttonRetry_userControlBCIErrorgTecBoard.Margin = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.buttonRetry_userControlBCIErrorgTecBoard.Name = "buttonRetry_userControlBCIErrorgTecBoard";
            this.buttonRetry_userControlBCIErrorgTecBoard.Size = new System.Drawing.Size(199, 61);
            this.buttonRetry_userControlBCIErrorgTecBoard.TabIndex = 85;
            this.buttonRetry_userControlBCIErrorgTecBoard.Text = "Retry";
            this.buttonRetry_userControlBCIErrorgTecBoard.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonRetry_userControlBCIErrorgTecBoard.UseCompatibleTextRendering = true;
            this.buttonRetry_userControlBCIErrorgTecBoard.UseMnemonic = false;
            this.buttonRetry_userControlBCIErrorgTecBoard.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerBottom, 3);
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(112, 715);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(795, 51);
            this.tableLayoutPanelSpacerBottom.TabIndex = 74;
            // 
            // tableLayoutPanelSpacerTop
            // 
            this.tableLayoutPanelSpacerTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerTop.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerTop, 3);
            this.tableLayoutPanelSpacerTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelSpacerTop.Location = new System.Drawing.Point(112, 0);
            this.tableLayoutPanelSpacerTop.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerTop.Name = "tableLayoutPanelSpacerTop";
            this.tableLayoutPanelSpacerTop.RowCount = 1;
            this.tableLayoutPanelSpacerTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Size = new System.Drawing.Size(795, 47);
            this.tableLayoutPanelSpacerTop.TabIndex = 73;
            // 
            // webBrowserTop
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.webBrowserTop, 3);
            this.webBrowserTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserTop.Location = new System.Drawing.Point(115, 183);
            this.webBrowserTop.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserTop.Name = "webBrowserTop";
            this.tableLayoutPanelMain.SetRowSpan(this.webBrowserTop, 3);
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.Size = new System.Drawing.Size(789, 109);
            this.webBrowserTop.TabIndex = 81;
            // 
            // webBrowserBottom
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.webBrowserBottom, 3);
            this.webBrowserBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserBottom.Location = new System.Drawing.Point(115, 545);
            this.webBrowserBottom.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserBottom.Name = "webBrowserBottom";
            this.tableLayoutPanelMain.SetRowSpan(this.webBrowserBottom, 3);
            this.webBrowserBottom.ScrollBarsEnabled = false;
            this.webBrowserBottom.Size = new System.Drawing.Size(789, 109);
            this.webBrowserBottom.TabIndex = 82;
            // 
            // UserControlBCIErrorGTecBoard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlBCIErrorGTecBoard";
            this.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanelBCIError.ResumeLayout(false);
            this.tableLayoutPanelBCIError.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBCIError;
        public System.Windows.Forms.Button buttonExit_userControlBCIErrorgTecBoard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonRetry_userControlBCIErrorgTecBoard;
        private System.Windows.Forms.WebBrowser webBrowserTop;
        private System.Windows.Forms.WebBrowser webBrowserBottom;
    }
}
