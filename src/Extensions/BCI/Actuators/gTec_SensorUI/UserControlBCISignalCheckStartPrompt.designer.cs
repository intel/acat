////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCISignalCheckStartPrompt.designer.cs
//
// User control which prompts the user for input to determine whether signal
// quality check should be executed (first step in the signal check process)
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which prompts the user for input to determine whether signal quality check 
    /// should be executed (first step in the signal check process)
    /// </summary>
    partial class UserControlBCISignalCheckStartPrompt
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
            System.Windows.Forms.Label labelBCISignalCheck;
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnUserRequestSignalQualityRecheck = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonNext = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelBCISignalCheckDescription = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.label53 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            labelBCISignalCheck = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelBCISignalCheck
            // 
            labelBCISignalCheck.AutoSize = true;
            labelBCISignalCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(labelBCISignalCheck, 23);
            labelBCISignalCheck.Dock = System.Windows.Forms.DockStyle.Fill;
            labelBCISignalCheck.Font = new System.Drawing.Font("Montserrat", 34F);
            labelBCISignalCheck.ForeColor = System.Drawing.Color.White;
            labelBCISignalCheck.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            labelBCISignalCheck.Location = new System.Drawing.Point(139, 42);
            labelBCISignalCheck.Margin = new System.Windows.Forms.Padding(0);
            labelBCISignalCheck.Name = "labelBCISignalCheck";
            labelBCISignalCheck.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutPanelMain.SetRowSpan(labelBCISignalCheck, 2);
            labelBCISignalCheck.Size = new System.Drawing.Size(736, 76);
            labelBCISignalCheck.TabIndex = 97;
            labelBCISignalCheck.Text = "Start BCI Signal Check";
            labelBCISignalCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            labelBCISignalCheck.UseCompatibleTextRendering = true;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.ColumnCount = 25;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.65495F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163348F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.230921F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.035108F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.160211F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.160211F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.163349F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.65496F));
            this.tableLayoutPanelMain.Controls.Add(this.label6, 3, 18);
            this.tableLayoutPanelMain.Controls.Add(this.label5, 11, 16);
            this.tableLayoutPanelMain.Controls.Add(this.btnUserRequestSignalQualityRecheck, 10, 20);
            this.tableLayoutPanelMain.Controls.Add(this.label4, 1, 14);
            this.tableLayoutPanelMain.Controls.Add(this.label3, 11, 12);
            this.tableLayoutPanelMain.Controls.Add(this.label2, 1, 10);
            this.tableLayoutPanelMain.Controls.Add(this.buttonNext, 19, 20);
            this.tableLayoutPanelMain.Controls.Add(this.labelBCISignalCheckDescription, 1, 4);
            this.tableLayoutPanelMain.Controls.Add(labelBCISignalCheck, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.label1, 2, 8);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit, 1, 20);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 23;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.572761F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.001535F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.001535F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.001535F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.004337F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.004337F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.004337F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.250962F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.568686F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.562895F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.572762F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.502697F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.148556F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.441297F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.250946F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.378667F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.07583F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.378667F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.570531F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.116761F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.567065F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.451213F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.57208F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.label6, 19);
            this.label6.Cursor = System.Windows.Forms.Cursors.Default;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(203, 542);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(608, 42);
            this.label6.TabIndex = 109;
            this.label6.Text = "you wish to re-check the signal quality ​";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.label5, 3);
            this.label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.label5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label5.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(459, 501);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 23);
            this.label5.TabIndex = 108;
            this.label5.Text = "or";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.UseCompatibleTextRendering = true;
            // 
            // btnUserRequestSignalQualityRecheck
            // 
            this.btnUserRequestSignalQualityRecheck.AutoSize = true;
            this.btnUserRequestSignalQualityRecheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnUserRequestSignalQualityRecheck.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.btnUserRequestSignalQualityRecheck.BorderRadiusBottomLeft = 0;
            this.btnUserRequestSignalQualityRecheck.BorderRadiusBottomRight = 0;
            this.btnUserRequestSignalQualityRecheck.BorderRadiusTopLeft = 0;
            this.btnUserRequestSignalQualityRecheck.BorderRadiusTopRight = 0;
            this.btnUserRequestSignalQualityRecheck.BorderWidth = 7F;
            this.tableLayoutPanelMain.SetColumnSpan(this.btnUserRequestSignalQualityRecheck, 5);
            this.btnUserRequestSignalQualityRecheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserRequestSignalQualityRecheck.Font = new System.Drawing.Font("Montserrat SemiBold", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUserRequestSignalQualityRecheck.ForeColor = System.Drawing.Color.White;
            this.btnUserRequestSignalQualityRecheck.Location = new System.Drawing.Point(452, 600);
            this.btnUserRequestSignalQualityRecheck.Margin = new System.Windows.Forms.Padding(25, 0, 25, 0);
            this.btnUserRequestSignalQualityRecheck.Name = "btnUserRequestSignalQualityRecheck";
            this.tableLayoutPanelMain.SetRowSpan(this.btnUserRequestSignalQualityRecheck, 2);
            this.btnUserRequestSignalQualityRecheck.Size = new System.Drawing.Size(105, 57);
            this.btnUserRequestSignalQualityRecheck.TabIndex = 107;
            this.btnUserRequestSignalQualityRecheck.Text = "Yes";
            this.btnUserRequestSignalQualityRecheck.UseCompatibleTextRendering = true;
            this.btnUserRequestSignalQualityRecheck.UseMnemonic = false;
            this.btnUserRequestSignalQualityRecheck.UseVisualStyleBackColor = false;
            this.btnUserRequestSignalQualityRecheck.Click += new System.EventHandler(this.btnUserRequestSignalQualityRecheck_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.label4, 23);
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(139, 451);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(736, 32);
            this.label4.TabIndex = 106;
            this.label4.Text = "you have removed or adjusted the position of the cap on your head";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.label3, 3);
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label3.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(459, 409);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 24);
            this.label3.TabIndex = 105;
            this.label3.Text = "or";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.label2, 23);
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(139, 348);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(736, 42);
            this.label2.TabIndex = 104;
            this.label2.Text = "​​you have added more gel to any of the electrodes​";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.AutoSize = true;
            this.buttonNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNext.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNext.BorderRadiusBottomLeft = 0;
            this.buttonNext.BorderRadiusBottomRight = 0;
            this.buttonNext.BorderRadiusTopLeft = 0;
            this.buttonNext.BorderRadiusTopRight = 0;
            this.buttonNext.BorderWidth = 2F;
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonNext, 5);
            this.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext.Font = new System.Drawing.Font("Montserrat Medium", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext.Location = new System.Drawing.Point(738, 654);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonNext.Name = "buttonNext";
            this.tableLayoutPanelMain.SetRowSpan(this.buttonNext, 2);
            this.buttonNext.Size = new System.Drawing.Size(137, 60);
            this.buttonNext.TabIndex = 91;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseCompatibleTextRendering = true;
            this.buttonNext.UseMnemonic = false;
            this.buttonNext.UseVisualStyleBackColor = false;
            // 
            // labelBCISignalCheckDescription
            // 
            this.labelBCISignalCheckDescription.AutoSize = true;
            this.labelBCISignalCheckDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelBCISignalCheckDescription, 23);
            this.labelBCISignalCheckDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelBCISignalCheckDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelBCISignalCheckDescription.Font = new System.Drawing.Font("Montserrat Medium", 14F);
            this.labelBCISignalCheckDescription.ForeColor = System.Drawing.Color.White;
            this.labelBCISignalCheckDescription.Location = new System.Drawing.Point(139, 156);
            this.labelBCISignalCheckDescription.Margin = new System.Windows.Forms.Padding(0);
            this.labelBCISignalCheckDescription.Name = "labelBCISignalCheckDescription";
            this.tableLayoutPanelMain.SetRowSpan(this.labelBCISignalCheckDescription, 3);
            this.labelBCISignalCheckDescription.Size = new System.Drawing.Size(736, 114);
            this.labelBCISignalCheckDescription.TabIndex = 101;
            this.labelBCISignalCheckDescription.Text = "For BCI switch to work well, we need to make sure that we\r\nare getting good quali" +
    "ty signals from the BCI cap that\r\nyou are wearing.\r\n";
            this.labelBCISignalCheckDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.label1, 21);
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(171, 287);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(672, 42);
            this.label1.TabIndex = 102;
            this.label1.Text = "Select Yes if,";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExit.AutoSize = true;
            this.buttonExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonExit, 5);
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExit.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit.Location = new System.Drawing.Point(139, 639);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonExit.Name = "buttonExit";
            this.tableLayoutPanelMain.SetRowSpan(this.buttonExit, 2);
            this.buttonExit.Size = new System.Drawing.Size(150, 75);
            this.buttonExit.TabIndex = 84;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseCompatibleTextRendering = true;
            this.buttonExit.UseVisualStyleBackColor = false;
            // 
            // label53
            // 
            this.label53.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label53.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label53.ForeColor = System.Drawing.Color.White;
            this.label53.Location = new System.Drawing.Point(539, 462);
            this.label53.Margin = new System.Windows.Forms.Padding(0);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(49, 42);
            this.label53.TabIndex = 107;
            this.label53.Text = "P3 Railing";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label53.UseCompatibleTextRendering = true;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label59.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label59.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label59.ForeColor = System.Drawing.Color.White;
            this.label59.Location = new System.Drawing.Point(539, 462);
            this.label59.Margin = new System.Windows.Forms.Padding(0);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(45, 40);
            this.label59.TabIndex = 104;
            this.label59.Text = "C3 Railing";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label59.UseCompatibleTextRendering = true;
            // 
            // UserControlBCISignalCheckStartPrompt
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlBCISignalCheckStartPrompt";
            this.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label59;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonNext;
        private System.Windows.Forms.Label labelBCISignalCheckDescription;


        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestO2;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestO1;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestP4;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestP3;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestCz;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestC3;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestC4;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestPz;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD1;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD2;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD3;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD4;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD6;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD5;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD7;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestD8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnUserRequestSignalQualityRecheck;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        // public UCCapLEDStatus2 ucCapLEDStatus21;
    }
}
