////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCISignalCheck.designer.cs
//
// Makes sure the BCI signals are good before continuing onto calibration.
// Displays signals from electrodes and does railing and impedance tests
//
// The original insipiration from this class is the OpenBCI GUI application:
// https://github.com/OpenBCI/OpenBCI_GUI
// It is licensed under the MIT License
// Copyright (c) 2018 OpenBCI
// https://github.com/OpenBCI/OpenBCI_GUI/blob/master/LICENSE
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Makes sure the BCI signals are good before continuing onto calibration. 
    /// Displays signals from electrodes and does railing and impedance tests
    /// </summary>
    partial class UserControlBCISignalCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlBCISignalCheck));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title5 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title6 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title7 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title8 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonNext_userControlBCISignalCheck = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonExit_userControlBCISignalCheck = new System.Windows.Forms.Button();
            this.panelSignalQualitySlider = new System.Windows.Forms.Panel();
            this.labelBCISignalCheckDescription = new System.Windows.Forms.Label();
            this.tabControlSignalQuality = new System.Windows.Forms.TabControl();
            this.tabPageRailing = new System.Windows.Forms.TabPage();
            this.tableLayoutRailingTest = new System.Windows.Forms.TableLayoutPanel();
            this.labelRailingTestInfo3 = new System.Windows.Forms.Label();
            this.labelRailingTestInfo2 = new System.Windows.Forms.Label();
            this.labelRailingTestInfo1 = new System.Windows.Forms.Label();
            this.labelRailingTest = new System.Windows.Forms.Label();
            this.btnElectrodeRailingTestR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelElectrodeRailingRailingTest = new System.Windows.Forms.Label();
            this.labelRequiredRailingTest = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.btnElectrodeRailingTestR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.label73 = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.chartRailingTestR8 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR7 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR6 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR5 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelBCISignalCheck = new System.Windows.Forms.Label();
            this.panelSignalQuality = new System.Windows.Forms.Panel();
            this.btnElectrodeCapE2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapE7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.label53 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.btnElectrodeCapOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tabControlSignalQuality.SuspendLayout();
            this.tabPageRailing.SuspendLayout();
            this.tableLayoutRailingTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR3)).BeginInit();
            this.panelSignalQuality.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.ColumnCount = 38;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.Controls.Add(this.buttonBack, 29, 20);
            this.tableLayoutPanelMain.Controls.Add(this.buttonNext_userControlBCISignalCheck, 33, 20);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit_userControlBCISignalCheck, 1, 20);
            this.tableLayoutPanelMain.Controls.Add(this.panelSignalQualitySlider, 4, 12);
            this.tableLayoutPanelMain.Controls.Add(this.labelBCISignalCheckDescription, 0, 4);
            this.tableLayoutPanelMain.Controls.Add(this.tabControlSignalQuality, 12, 1);
            this.tableLayoutPanelMain.Controls.Add(this.labelBCISignalCheck, 2, 0);
            this.tableLayoutPanelMain.Controls.Add(this.panelSignalQuality, 4, 14);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowser, 12, 19);
            this.tableLayoutPanelMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 22;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1920, 1080);
            this.tableLayoutPanelMain.TabIndex = 9;
            // 
            // buttonBack
            // 
            this.buttonBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonBack, 3);
            this.buttonBack.Enabled = false;
            this.buttonBack.FlatAppearance.BorderSize = 0;
            this.buttonBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBack.Font = new System.Drawing.Font("Montserrat Thin", 28F, System.Drawing.FontStyle.Underline);
            this.buttonBack.ForeColor = System.Drawing.Color.Silver;
            this.buttonBack.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonBack.Location = new System.Drawing.Point(1460, 990);
            this.buttonBack.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(150, 55);
            this.buttonBack.TabIndex = 94;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseCompatibleTextRendering = true;
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Visible = false;
            // 
            // buttonNext_userControlBCISignalCheck
            // 
            this.buttonNext_userControlBCISignalCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext_userControlBCISignalCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNext_userControlBCISignalCheck.BorderColor = System.Drawing.Color.Black;
            this.buttonNext_userControlBCISignalCheck.BorderRadiusBottomLeft = 0;
            this.buttonNext_userControlBCISignalCheck.BorderRadiusBottomRight = 0;
            this.buttonNext_userControlBCISignalCheck.BorderRadiusTopLeft = 0;
            this.buttonNext_userControlBCISignalCheck.BorderRadiusTopRight = 0;
            this.buttonNext_userControlBCISignalCheck.BorderWidth = 2F;
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonNext_userControlBCISignalCheck, 4);
            this.buttonNext_userControlBCISignalCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext_userControlBCISignalCheck.Font = new System.Drawing.Font("Montserrat Medium", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext_userControlBCISignalCheck.Location = new System.Drawing.Point(1710, 990);
            this.buttonNext_userControlBCISignalCheck.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNext_userControlBCISignalCheck.Name = "buttonNext_userControlBCISignalCheck";
            this.buttonNext_userControlBCISignalCheck.Size = new System.Drawing.Size(150, 55);
            this.buttonNext_userControlBCISignalCheck.TabIndex = 91;
            this.buttonNext_userControlBCISignalCheck.Text = "Next";
            this.buttonNext_userControlBCISignalCheck.UseCompatibleTextRendering = true;
            this.buttonNext_userControlBCISignalCheck.UseMnemonic = false;
            this.buttonNext_userControlBCISignalCheck.UseVisualStyleBackColor = false;
            // 
            // buttonExit_userControlBCISignalCheck
            // 
            this.buttonExit_userControlBCISignalCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExit_userControlBCISignalCheck.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonExit_userControlBCISignalCheck, 3);
            this.buttonExit_userControlBCISignalCheck.FlatAppearance.BorderSize = 0;
            this.buttonExit_userControlBCISignalCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit_userControlBCISignalCheck.Font = new System.Drawing.Font("Montserrat Thin", 30F, System.Drawing.FontStyle.Underline);
            this.buttonExit_userControlBCISignalCheck.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit_userControlBCISignalCheck.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit_userControlBCISignalCheck.Location = new System.Drawing.Point(60, 990);
            this.buttonExit_userControlBCISignalCheck.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit_userControlBCISignalCheck.Name = "buttonExit_userControlBCISignalCheck";
            this.buttonExit_userControlBCISignalCheck.Size = new System.Drawing.Size(150, 55);
            this.buttonExit_userControlBCISignalCheck.TabIndex = 84;
            this.buttonExit_userControlBCISignalCheck.Text = "Exit";
            this.buttonExit_userControlBCISignalCheck.UseCompatibleTextRendering = true;
            this.buttonExit_userControlBCISignalCheck.UseVisualStyleBackColor = false;
            // 
            // panelSignalQualitySlider
            // 
            this.panelSignalQualitySlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSignalQualitySlider.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelSignalQualitySlider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.panelSignalQualitySlider.BackgroundImage = global::gTecSensorUI.Properties.Resources.signalQualityGradient_1AcceptableChannel;
            this.panelSignalQualitySlider.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanelMain.SetColumnSpan(this.panelSignalQualitySlider, 6);
            this.panelSignalQualitySlider.Location = new System.Drawing.Point(210, 595);
            this.panelSignalQualitySlider.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.panelSignalQualitySlider.Name = "panelSignalQualitySlider";
            this.tableLayoutPanelMain.SetRowSpan(this.panelSignalQualitySlider, 2);
            this.panelSignalQualitySlider.Size = new System.Drawing.Size(310, 70);
            this.panelSignalQualitySlider.TabIndex = 100;
            // 
            // labelBCISignalCheckDescription
            // 
            this.labelBCISignalCheckDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelBCISignalCheckDescription, 9);
            this.labelBCISignalCheckDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelBCISignalCheckDescription.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBCISignalCheckDescription.ForeColor = System.Drawing.Color.White;
            this.labelBCISignalCheckDescription.Location = new System.Drawing.Point(5, 185);
            this.labelBCISignalCheckDescription.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelBCISignalCheckDescription.Name = "labelBCISignalCheckDescription";
            this.tableLayoutPanelMain.SetRowSpan(this.labelBCISignalCheckDescription, 8);
            this.labelBCISignalCheckDescription.Size = new System.Drawing.Size(455, 400);
            this.labelBCISignalCheckDescription.TabIndex = 101;
            this.labelBCISignalCheckDescription.Text = resources.GetString("labelBCISignalCheckDescription.Text");
            this.labelBCISignalCheckDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabControlSignalQuality
            // 
            this.tabControlSignalQuality.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tableLayoutPanelMain.SetColumnSpan(this.tabControlSignalQuality, 25);
            this.tabControlSignalQuality.Controls.Add(this.tabPageRailing);
            this.tabControlSignalQuality.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSignalQuality.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlSignalQuality.ItemSize = new System.Drawing.Size(150, 50);
            this.tabControlSignalQuality.Location = new System.Drawing.Point(610, 35);
            this.tabControlSignalQuality.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlSignalQuality.Multiline = true;
            this.tabControlSignalQuality.Name = "tabControlSignalQuality";
            this.tabControlSignalQuality.Padding = new System.Drawing.Point(75, 2);
            this.tableLayoutPanelMain.SetRowSpan(this.tabControlSignalQuality, 18);
            this.tabControlSignalQuality.SelectedIndex = 0;
            this.tabControlSignalQuality.Size = new System.Drawing.Size(1250, 900);
            this.tabControlSignalQuality.TabIndex = 102;
            this.tabControlSignalQuality.TabStop = false;
            this.tabControlSignalQuality.SelectedIndexChanged += new System.EventHandler(this.tabControlElectrodeQuality_SelectedIndexChanged);
            // 
            // tabPageRailing
            // 
            this.tabPageRailing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tabPageRailing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPageRailing.Controls.Add(this.tableLayoutRailingTest);
            this.tabPageRailing.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.tabPageRailing.ImageKey = "(none)";
            this.tabPageRailing.Location = new System.Drawing.Point(4, 54);
            this.tabPageRailing.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageRailing.Name = "tabPageRailing";
            this.tabPageRailing.Size = new System.Drawing.Size(1242, 842);
            this.tabPageRailing.TabIndex = 2;
            this.tabPageRailing.Text = "- 1. Railing -";
            // 
            // tableLayoutRailingTest
            // 
            this.tableLayoutRailingTest.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutRailingTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutRailingTest.ColumnCount = 23;
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.349653F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.349653F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.347544F));
            this.tableLayoutRailingTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.349847F));
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTestInfo3, 0, 15);
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTestInfo2, 0, 8);
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTestInfo1, 0, 5);
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTest, 0, 0);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR1, 8, 1);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR1, 10, 1);
            this.tableLayoutRailingTest.Controls.Add(this.labelElectrodeRailingRailingTest, 17, 0);
            this.tableLayoutRailingTest.Controls.Add(this.labelRequiredRailingTest, 8, 0);
            this.tableLayoutRailingTest.Controls.Add(this.label66, 9, 1);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR2, 8, 3);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR3, 8, 5);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR4, 8, 7);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR5, 8, 9);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR6, 8, 11);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR7, 8, 13);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR8, 8, 15);
            this.tableLayoutRailingTest.Controls.Add(this.label73, 9, 15);
            this.tableLayoutRailingTest.Controls.Add(this.label72, 9, 13);
            this.tableLayoutRailingTest.Controls.Add(this.label71, 9, 11);
            this.tableLayoutRailingTest.Controls.Add(this.label70, 9, 9);
            this.tableLayoutRailingTest.Controls.Add(this.label69, 9, 7);
            this.tableLayoutRailingTest.Controls.Add(this.label68, 9, 5);
            this.tableLayoutRailingTest.Controls.Add(this.label67, 9, 3);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR8, 10, 15);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR2, 10, 3);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR7, 10, 13);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR6, 10, 11);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR5, 10, 9);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR4, 10, 7);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR3, 10, 5);
            this.tableLayoutRailingTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tableLayoutRailingTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutRailingTest.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutRailingTest.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutRailingTest.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutRailingTest.Name = "tableLayoutRailingTest";
            this.tableLayoutRailingTest.RowCount = 19;
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.110723F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.348432F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.267109F));
            this.tableLayoutRailingTest.Size = new System.Drawing.Size(1242, 842);
            this.tableLayoutRailingTest.TabIndex = 4;
            // 
            // labelRailingTestInfo3
            // 
            this.labelRailingTestInfo3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRailingTestInfo3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutRailingTest.SetColumnSpan(this.labelRailingTestInfo3, 7);
            this.labelRailingTestInfo3.Font = new System.Drawing.Font("Montserrat", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRailingTestInfo3.ForeColor = System.Drawing.Color.White;
            this.labelRailingTestInfo3.Location = new System.Drawing.Point(20, 660);
            this.labelRailingTestInfo3.Margin = new System.Windows.Forms.Padding(20, 0, 0, 10);
            this.labelRailingTestInfo3.Name = "labelRailingTestInfo3";
            this.tableLayoutRailingTest.SetRowSpan(this.labelRailingTestInfo3, 4);
            this.labelRailingTestInfo3.Size = new System.Drawing.Size(377, 172);
            this.labelRailingTestInfo3.TabIndex = 148;
            this.labelRailingTestInfo3.Text = "Once Railing is green, go to the Impedance tab";
            this.labelRailingTestInfo3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelRailingTestInfo2
            // 
            this.labelRailingTestInfo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRailingTestInfo2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutRailingTest.SetColumnSpan(this.labelRailingTestInfo2, 7);
            this.labelRailingTestInfo2.Font = new System.Drawing.Font("Montserrat SemiBold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRailingTestInfo2.ForeColor = System.Drawing.Color.White;
            this.labelRailingTestInfo2.Location = new System.Drawing.Point(20, 352);
            this.labelRailingTestInfo2.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.labelRailingTestInfo2.Name = "labelRailingTestInfo2";
            this.tableLayoutRailingTest.SetRowSpan(this.labelRailingTestInfo2, 7);
            this.labelRailingTestInfo2.Size = new System.Drawing.Size(377, 308);
            this.labelRailingTestInfo2.TabIndex = 146;
            this.labelRailingTestInfo2.Text = "have added gel to GND and T4\r\n\r\nhave added gel to the electrode\r\n\r\nare grounded\r\n" +
    "\r\nAdd a little more gel";
            this.labelRailingTestInfo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelRailingTestInfo1
            // 
            this.labelRailingTestInfo1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRailingTestInfo1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutRailingTest.SetColumnSpan(this.labelRailingTestInfo1, 7);
            this.labelRailingTestInfo1.Font = new System.Drawing.Font("Montserrat", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRailingTestInfo1.ForeColor = System.Drawing.Color.White;
            this.labelRailingTestInfo1.Location = new System.Drawing.Point(20, 220);
            this.labelRailingTestInfo1.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.labelRailingTestInfo1.Name = "labelRailingTestInfo1";
            this.tableLayoutRailingTest.SetRowSpan(this.labelRailingTestInfo1, 3);
            this.labelRailingTestInfo1.Size = new System.Drawing.Size(377, 132);
            this.labelRailingTestInfo1.TabIndex = 102;
            this.labelRailingTestInfo1.Text = "If the railing signal is not green,\r\ncheck if you:";
            this.labelRailingTestInfo1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRailingTest
            // 
            this.labelRailingTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRailingTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutRailingTest.SetColumnSpan(this.labelRailingTest, 7);
            this.labelRailingTest.Font = new System.Drawing.Font("Montserrat", 44F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRailingTest.ForeColor = System.Drawing.Color.White;
            this.labelRailingTest.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelRailingTest.Location = new System.Drawing.Point(20, 30);
            this.labelRailingTest.Margin = new System.Windows.Forms.Padding(20, 30, 0, 0);
            this.labelRailingTest.Name = "labelRailingTest";
            this.labelRailingTest.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutRailingTest.SetRowSpan(this.labelRailingTest, 5);
            this.labelRailingTest.Size = new System.Drawing.Size(377, 190);
            this.labelRailingTest.TabIndex = 98;
            this.labelRailingTest.Text = "Railing\r\nTest";
            this.labelRailingTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnElectrodeRailingTestR1
            // 
            this.btnElectrodeRailingTestR1.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR1.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR1.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR1.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR1.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR1.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR1.BorderWidth = 2F;
            this.btnElectrodeRailingTestR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR1.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR1.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR1.Location = new System.Drawing.Point(450, 44);
            this.btnElectrodeRailingTestR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR1.Name = "btnElectrodeRailingTestR1";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR1, 2);
            this.btnElectrodeRailingTestR1.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR1.TabIndex = 5;
            this.btnElectrodeRailingTestR1.Text = "R1";
            this.btnElectrodeRailingTestR1.UseMnemonic = false;
            this.btnElectrodeRailingTestR1.UseVisualStyleBackColor = false;
            // 
            // chartRailingTestR1
            // 
            this.chartRailingTestR1.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR1.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR1.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR1.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.Maximum = 1250D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.ScaleView.Zoomable = false;
            chartArea1.AxisX.ScrollBar.Enabled = false;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.BorderWidth = 0;
            chartArea1.Name = "chartAreaR1";
            this.chartRailingTestR1.ChartAreas.Add(chartArea1);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR1, 12);
            this.chartRailingTestR1.IsSoftShadows = false;
            legend1.Enabled = false;
            legend1.Name = "LegenOp7";
            this.chartRailingTestR1.Legends.Add(legend1);
            this.chartRailingTestR1.Location = new System.Drawing.Point(556, 44);
            this.chartRailingTestR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR1.Name = "chartRailingTestR1";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR1, 2);
            series1.ChartArea = "chartAreaR1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "LegenOp7";
            series1.Name = "Series1";
            this.chartRailingTestR1.Series.Add(series1);
            this.chartRailingTestR1.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR1.TabIndex = 32;
            this.chartRailingTestR1.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title1.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title1.BackColor = System.Drawing.Color.Transparent;
            title1.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Right;
            title1.BackSecondaryColor = System.Drawing.Color.Transparent;
            title1.BorderColor = System.Drawing.Color.Transparent;
            title1.DockedToChartArea = "chartAreaR1";
            title1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title1.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.ForeColor = System.Drawing.Color.White;
            title1.IsDockedInsideChartArea = false;
            title1.Name = "railingResRailingTestR1";
            title1.Text = "railR1";
            title1.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR1.Titles.Add(title1);
            // 
            // labelElectrodeRailingRailingTest
            // 
            this.tableLayoutRailingTest.SetColumnSpan(this.labelElectrodeRailingRailingTest, 5);
            this.labelElectrodeRailingRailingTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelElectrodeRailingRailingTest.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelElectrodeRailingRailingTest.ForeColor = System.Drawing.Color.White;
            this.labelElectrodeRailingRailingTest.Location = new System.Drawing.Point(927, 0);
            this.labelElectrodeRailingRailingTest.Margin = new System.Windows.Forms.Padding(0);
            this.labelElectrodeRailingRailingTest.Name = "labelElectrodeRailingRailingTest";
            this.labelElectrodeRailingRailingTest.Size = new System.Drawing.Size(265, 44);
            this.labelElectrodeRailingRailingTest.TabIndex = 2;
            this.labelElectrodeRailingRailingTest.Text = "Electrode Railing";
            this.labelElectrodeRailingRailingTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRequiredRailingTest
            // 
            this.tableLayoutRailingTest.SetColumnSpan(this.labelRequiredRailingTest, 3);
            this.labelRequiredRailingTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRequiredRailingTest.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold);
            this.labelRequiredRailingTest.ForeColor = System.Drawing.Color.White;
            this.labelRequiredRailingTest.Location = new System.Drawing.Point(450, 0);
            this.labelRequiredRailingTest.Margin = new System.Windows.Forms.Padding(0);
            this.labelRequiredRailingTest.Name = "labelRequiredRailingTest";
            this.labelRequiredRailingTest.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.labelRequiredRailingTest.Size = new System.Drawing.Size(159, 44);
            this.labelRequiredRailingTest.TabIndex = 1;
            this.labelRequiredRailingTest.Text = "Required";
            this.labelRequiredRailingTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label66
            // 
            this.label66.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label66.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label66.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label66.ForeColor = System.Drawing.Color.White;
            this.label66.Location = new System.Drawing.Point(503, 44);
            this.label66.Margin = new System.Windows.Forms.Padding(0);
            this.label66.Name = "label66";
            this.tableLayoutRailingTest.SetRowSpan(this.label66, 2);
            this.label66.Size = new System.Drawing.Size(53, 88);
            this.label66.TabIndex = 114;
            this.label66.Text = "=";
            this.label66.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label66.UseCompatibleTextRendering = true;
            // 
            // btnElectrodeRailingTestR2
            // 
            this.btnElectrodeRailingTestR2.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR2.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR2.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR2.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR2.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR2.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR2.BorderWidth = 2F;
            this.btnElectrodeRailingTestR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR2.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR2.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR2.Location = new System.Drawing.Point(450, 132);
            this.btnElectrodeRailingTestR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR2.Name = "btnElectrodeRailingTestR2";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR2, 2);
            this.btnElectrodeRailingTestR2.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR2.TabIndex = 8;
            this.btnElectrodeRailingTestR2.Text = "R2";
            this.btnElectrodeRailingTestR2.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR2.UseMnemonic = false;
            this.btnElectrodeRailingTestR2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestR3
            // 
            this.btnElectrodeRailingTestR3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR3.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR3.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR3.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR3.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR3.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR3.BorderWidth = 2F;
            this.btnElectrodeRailingTestR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR3.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR3.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR3.Location = new System.Drawing.Point(450, 220);
            this.btnElectrodeRailingTestR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR3.Name = "btnElectrodeRailingTestR3";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR3, 2);
            this.btnElectrodeRailingTestR3.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR3.TabIndex = 11;
            this.btnElectrodeRailingTestR3.Text = "R3";
            this.btnElectrodeRailingTestR3.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR3.UseMnemonic = false;
            this.btnElectrodeRailingTestR3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestR4
            // 
            this.btnElectrodeRailingTestR4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR4.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR4.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR4.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR4.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR4.BorderWidth = 2F;
            this.btnElectrodeRailingTestR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR4.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR4.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR4.Location = new System.Drawing.Point(450, 308);
            this.btnElectrodeRailingTestR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR4.Name = "btnElectrodeRailingTestR4";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR4, 2);
            this.btnElectrodeRailingTestR4.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR4.TabIndex = 21;
            this.btnElectrodeRailingTestR4.Text = "R4";
            this.btnElectrodeRailingTestR4.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR4.UseMnemonic = false;
            this.btnElectrodeRailingTestR4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestR5
            // 
            this.btnElectrodeRailingTestR5.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR5.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR5.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR5.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR5.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR5.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR5.BorderWidth = 2F;
            this.btnElectrodeRailingTestR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR5.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR5.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR5.Location = new System.Drawing.Point(450, 396);
            this.btnElectrodeRailingTestR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR5.Name = "btnElectrodeRailingTestR5";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR5, 2);
            this.btnElectrodeRailingTestR5.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR5.TabIndex = 24;
            this.btnElectrodeRailingTestR5.Text = "R5";
            this.btnElectrodeRailingTestR5.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR5.UseMnemonic = false;
            this.btnElectrodeRailingTestR5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestR6
            // 
            this.btnElectrodeRailingTestR6.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR6.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR6.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR6.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR6.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR6.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR6.BorderWidth = 2F;
            this.btnElectrodeRailingTestR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR6.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR6.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR6.Location = new System.Drawing.Point(450, 484);
            this.btnElectrodeRailingTestR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR6.Name = "btnElectrodeRailingTestR6";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR6, 2);
            this.btnElectrodeRailingTestR6.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR6.TabIndex = 27;
            this.btnElectrodeRailingTestR6.Text = "R6";
            this.btnElectrodeRailingTestR6.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR6.UseMnemonic = false;
            this.btnElectrodeRailingTestR6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestR7
            // 
            this.btnElectrodeRailingTestR7.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR7.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR7.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR7.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR7.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR7.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR7.BorderWidth = 2F;
            this.btnElectrodeRailingTestR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR7.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR7.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR7.Location = new System.Drawing.Point(450, 572);
            this.btnElectrodeRailingTestR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR7.Name = "btnElectrodeRailingTestR7";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR7, 2);
            this.btnElectrodeRailingTestR7.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR7.TabIndex = 30;
            this.btnElectrodeRailingTestR7.Text = "R7";
            this.btnElectrodeRailingTestR7.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR7.UseMnemonic = false;
            this.btnElectrodeRailingTestR7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestR8
            // 
            this.btnElectrodeRailingTestR8.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR8.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR8.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR8.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR8.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR8.BorderWidth = 2F;
            this.btnElectrodeRailingTestR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR8.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR8.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR8.Location = new System.Drawing.Point(450, 660);
            this.btnElectrodeRailingTestR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR8.Name = "btnElectrodeRailingTestR8";
            this.tableLayoutRailingTest.SetRowSpan(this.btnElectrodeRailingTestR8, 2);
            this.btnElectrodeRailingTestR8.Size = new System.Drawing.Size(53, 86);
            this.btnElectrodeRailingTestR8.TabIndex = 33;
            this.btnElectrodeRailingTestR8.Text = "R8";
            this.btnElectrodeRailingTestR8.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR8.UseMnemonic = false;
            this.btnElectrodeRailingTestR8.UseVisualStyleBackColor = false;
            // 
            // label73
            // 
            this.label73.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label73.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label73.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label73.ForeColor = System.Drawing.Color.White;
            this.label73.Location = new System.Drawing.Point(503, 660);
            this.label73.Margin = new System.Windows.Forms.Padding(0);
            this.label73.Name = "label73";
            this.tableLayoutRailingTest.SetRowSpan(this.label73, 2);
            this.label73.Size = new System.Drawing.Size(53, 86);
            this.label73.TabIndex = 121;
            this.label73.Text = "=";
            this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label73.UseCompatibleTextRendering = true;
            // 
            // label72
            // 
            this.label72.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label72.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label72.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label72.ForeColor = System.Drawing.Color.White;
            this.label72.Location = new System.Drawing.Point(503, 572);
            this.label72.Margin = new System.Windows.Forms.Padding(0);
            this.label72.Name = "label72";
            this.tableLayoutRailingTest.SetRowSpan(this.label72, 2);
            this.label72.Size = new System.Drawing.Size(53, 81);
            this.label72.TabIndex = 120;
            this.label72.Text = "=";
            this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label72.UseCompatibleTextRendering = true;
            // 
            // label71
            // 
            this.label71.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label71.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label71.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label71.ForeColor = System.Drawing.Color.White;
            this.label71.Location = new System.Drawing.Point(503, 484);
            this.label71.Margin = new System.Windows.Forms.Padding(0);
            this.label71.Name = "label71";
            this.tableLayoutRailingTest.SetRowSpan(this.label71, 2);
            this.label71.Size = new System.Drawing.Size(53, 86);
            this.label71.TabIndex = 119;
            this.label71.Text = "=";
            this.label71.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label71.UseCompatibleTextRendering = true;
            // 
            // label70
            // 
            this.label70.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label70.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label70.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label70.ForeColor = System.Drawing.Color.White;
            this.label70.Location = new System.Drawing.Point(503, 396);
            this.label70.Margin = new System.Windows.Forms.Padding(0);
            this.label70.Name = "label70";
            this.tableLayoutRailingTest.SetRowSpan(this.label70, 2);
            this.label70.Size = new System.Drawing.Size(53, 86);
            this.label70.TabIndex = 118;
            this.label70.Text = "=";
            this.label70.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label70.UseCompatibleTextRendering = true;
            // 
            // label69
            // 
            this.label69.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label69.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label69.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label69.ForeColor = System.Drawing.Color.White;
            this.label69.Location = new System.Drawing.Point(503, 308);
            this.label69.Margin = new System.Windows.Forms.Padding(0);
            this.label69.Name = "label69";
            this.tableLayoutRailingTest.SetRowSpan(this.label69, 2);
            this.label69.Size = new System.Drawing.Size(53, 86);
            this.label69.TabIndex = 117;
            this.label69.Text = "=";
            this.label69.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label69.UseCompatibleTextRendering = true;
            // 
            // label68
            // 
            this.label68.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label68.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label68.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label68.ForeColor = System.Drawing.Color.White;
            this.label68.Location = new System.Drawing.Point(503, 220);
            this.label68.Margin = new System.Windows.Forms.Padding(0);
            this.label68.Name = "label68";
            this.tableLayoutRailingTest.SetRowSpan(this.label68, 2);
            this.label68.Size = new System.Drawing.Size(53, 86);
            this.label68.TabIndex = 116;
            this.label68.Text = "=";
            this.label68.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label68.UseCompatibleTextRendering = true;
            // 
            // label67
            // 
            this.label67.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label67.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label67.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label67.ForeColor = System.Drawing.Color.White;
            this.label67.Location = new System.Drawing.Point(503, 132);
            this.label67.Margin = new System.Windows.Forms.Padding(0);
            this.label67.Name = "label67";
            this.tableLayoutRailingTest.SetRowSpan(this.label67, 2);
            this.label67.Size = new System.Drawing.Size(53, 88);
            this.label67.TabIndex = 115;
            this.label67.Text = "=";
            this.label67.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label67.UseCompatibleTextRendering = true;
            // 
            // chartRailingTestR8
            // 
            this.chartRailingTestR8.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR8.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR8.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR8.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR8.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.IsMarginVisible = false;
            chartArea2.AxisX.LabelStyle.Enabled = false;
            chartArea2.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisX.MajorTickMark.Enabled = false;
            chartArea2.AxisX.Maximum = 1250D;
            chartArea2.AxisX.Minimum = 0D;
            chartArea2.AxisX.ScaleView.Zoomable = false;
            chartArea2.AxisX.ScrollBar.Enabled = false;
            chartArea2.AxisY.IsLabelAutoFit = false;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorTickMark.Enabled = false;
            chartArea2.BackColor = System.Drawing.Color.Transparent;
            chartArea2.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea2.BorderWidth = 0;
            chartArea2.Name = "chartAreaR8";
            this.chartRailingTestR8.ChartAreas.Add(chartArea2);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR8, 12);
            this.chartRailingTestR8.IsSoftShadows = false;
            legend2.Enabled = false;
            legend2.Name = "LegenOp3";
            this.chartRailingTestR8.Legends.Add(legend2);
            this.chartRailingTestR8.Location = new System.Drawing.Point(556, 660);
            this.chartRailingTestR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR8.Name = "chartRailingTestR8";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR8, 2);
            series2.ChartArea = "chartAreaR8";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Legend = "LegenOp3";
            series2.Name = "Series1";
            this.chartRailingTestR8.Series.Add(series2);
            this.chartRailingTestR8.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR8.TabIndex = 35;
            this.chartRailingTestR8.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title2.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title2.DockedToChartArea = "chartAreaR8";
            title2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title2.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title2.ForeColor = System.Drawing.Color.White;
            title2.IsDockedInsideChartArea = false;
            title2.Name = "railingResRailingTestR8";
            title2.Text = "railR8";
            title2.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR8.Titles.Add(title2);
            // 
            // chartRailingTestR2
            // 
            this.chartRailingTestR2.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR2.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR2.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR2.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR2.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea3.AxisX.IsLabelAutoFit = false;
            chartArea3.AxisX.IsMarginVisible = false;
            chartArea3.AxisX.LabelStyle.Enabled = false;
            chartArea3.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea3.AxisX.MajorGrid.Enabled = false;
            chartArea3.AxisX.MajorTickMark.Enabled = false;
            chartArea3.AxisX.Maximum = 1250D;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisX.ScaleView.Zoomable = false;
            chartArea3.AxisX.ScrollBar.Enabled = false;
            chartArea3.AxisY.IsLabelAutoFit = false;
            chartArea3.AxisY.LabelStyle.Enabled = false;
            chartArea3.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea3.AxisY.MajorGrid.Enabled = false;
            chartArea3.AxisY.MajorTickMark.Enabled = false;
            chartArea3.BackColor = System.Drawing.Color.Transparent;
            chartArea3.Name = "chartAreaR2";
            this.chartRailingTestR2.ChartAreas.Add(chartArea3);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR2, 12);
            this.chartRailingTestR2.IsSoftShadows = false;
            legend3.Enabled = false;
            legend3.Name = "LegenOp8";
            this.chartRailingTestR2.Legends.Add(legend3);
            this.chartRailingTestR2.Location = new System.Drawing.Point(556, 132);
            this.chartRailingTestR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR2.Name = "chartRailingTestR2";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR2, 2);
            series3.ChartArea = "chartAreaR2";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Legend = "LegenOp8";
            series3.Name = "Series1";
            this.chartRailingTestR2.Series.Add(series3);
            this.chartRailingTestR2.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR2.TabIndex = 6;
            this.chartRailingTestR2.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title3.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title3.DockedToChartArea = "chartAreaR2";
            title3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title3.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title3.ForeColor = System.Drawing.Color.White;
            title3.IsDockedInsideChartArea = false;
            title3.Name = "railingResRailingTestR2";
            title3.Text = "railR2";
            title3.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR2.Titles.Add(title3);
            // 
            // chartRailingTestR7
            // 
            this.chartRailingTestR7.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR7.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR7.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR7.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR7.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea4.AxisX.IsLabelAutoFit = false;
            chartArea4.AxisX.IsMarginVisible = false;
            chartArea4.AxisX.LabelStyle.Enabled = false;
            chartArea4.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea4.AxisX.MajorGrid.Enabled = false;
            chartArea4.AxisX.MajorTickMark.Enabled = false;
            chartArea4.AxisX.Maximum = 1250D;
            chartArea4.AxisX.Minimum = 0D;
            chartArea4.AxisX.ScaleView.Zoomable = false;
            chartArea4.AxisX.ScrollBar.Enabled = false;
            chartArea4.AxisY.IsLabelAutoFit = false;
            chartArea4.AxisY.LabelStyle.Enabled = false;
            chartArea4.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea4.AxisY.MajorGrid.Enabled = false;
            chartArea4.AxisY.MajorTickMark.Enabled = false;
            chartArea4.BackColor = System.Drawing.Color.Transparent;
            chartArea4.BorderWidth = 0;
            chartArea4.Name = "chartAreaR7";
            this.chartRailingTestR7.ChartAreas.Add(chartArea4);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR7, 12);
            this.chartRailingTestR7.IsSoftShadows = false;
            legend4.Enabled = false;
            legend4.Name = "LegenOp7";
            this.chartRailingTestR7.Legends.Add(legend4);
            this.chartRailingTestR7.Location = new System.Drawing.Point(556, 572);
            this.chartRailingTestR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR7.Name = "chartRailingTestR7";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR7, 2);
            series4.ChartArea = "chartAreaR7";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Legend = "LegenOp7";
            series4.Name = "Series1";
            this.chartRailingTestR7.Series.Add(series4);
            this.chartRailingTestR7.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR7.TabIndex = 32;
            this.chartRailingTestR7.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title4.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title4.DockedToChartArea = "chartAreaR7";
            title4.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title4.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title4.ForeColor = System.Drawing.Color.White;
            title4.IsDockedInsideChartArea = false;
            title4.Name = "railingResRailingTestR7";
            title4.Text = "railR7";
            title4.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR7.Titles.Add(title4);
            // 
            // chartRailingTestR6
            // 
            this.chartRailingTestR6.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR6.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR6.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR6.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR6.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea5.AxisX.IsLabelAutoFit = false;
            chartArea5.AxisX.IsMarginVisible = false;
            chartArea5.AxisX.LabelStyle.Enabled = false;
            chartArea5.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea5.AxisX.MajorGrid.Enabled = false;
            chartArea5.AxisX.MajorTickMark.Enabled = false;
            chartArea5.AxisX.Maximum = 1250D;
            chartArea5.AxisX.Minimum = 0D;
            chartArea5.AxisX.ScaleView.Zoomable = false;
            chartArea5.AxisX.ScrollBar.Enabled = false;
            chartArea5.AxisY.IsLabelAutoFit = false;
            chartArea5.AxisY.LabelStyle.Enabled = false;
            chartArea5.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea5.AxisY.MajorGrid.Enabled = false;
            chartArea5.AxisY.MajorTickMark.Enabled = false;
            chartArea5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea5.BorderWidth = 0;
            chartArea5.Name = "chartAreaR6";
            this.chartRailingTestR6.ChartAreas.Add(chartArea5);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR6, 12);
            this.chartRailingTestR6.IsSoftShadows = false;
            legend5.Enabled = false;
            legend5.Name = "LegenOp5";
            this.chartRailingTestR6.Legends.Add(legend5);
            this.chartRailingTestR6.Location = new System.Drawing.Point(556, 484);
            this.chartRailingTestR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR6.Name = "chartRailingTestR6";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR6, 2);
            series5.ChartArea = "chartAreaR6";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series5.Legend = "LegenOp5";
            series5.Name = "Series1";
            this.chartRailingTestR6.Series.Add(series5);
            this.chartRailingTestR6.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR6.TabIndex = 29;
            this.chartRailingTestR6.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title5.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title5.DockedToChartArea = "chartAreaR6";
            title5.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title5.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title5.ForeColor = System.Drawing.Color.White;
            title5.IsDockedInsideChartArea = false;
            title5.Name = "railingResRailingTestR6";
            title5.Text = "railR6";
            title5.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR6.Titles.Add(title5);
            // 
            // chartRailingTestR5
            // 
            this.chartRailingTestR5.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR5.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR5.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR5.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR5.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea6.AxisX.IsMarginVisible = false;
            chartArea6.AxisX.LabelStyle.Enabled = false;
            chartArea6.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea6.AxisX.MajorGrid.Enabled = false;
            chartArea6.AxisX.MajorTickMark.Enabled = false;
            chartArea6.AxisX.Maximum = 1250D;
            chartArea6.AxisX.Minimum = 0D;
            chartArea6.AxisX.ScaleView.Zoomable = false;
            chartArea6.AxisX.ScrollBar.Enabled = false;
            chartArea6.AxisY.IsLabelAutoFit = false;
            chartArea6.AxisY.LabelStyle.Enabled = false;
            chartArea6.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea6.AxisY.MajorGrid.Enabled = false;
            chartArea6.AxisY.MajorTickMark.Enabled = false;
            chartArea6.BackColor = System.Drawing.Color.Transparent;
            chartArea6.Name = "chartAreaR5";
            this.chartRailingTestR5.ChartAreas.Add(chartArea6);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR5, 12);
            this.chartRailingTestR5.IsSoftShadows = false;
            legend6.Enabled = false;
            legend6.Name = "LegenOp4";
            this.chartRailingTestR5.Legends.Add(legend6);
            this.chartRailingTestR5.Location = new System.Drawing.Point(556, 396);
            this.chartRailingTestR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR5.Name = "chartRailingTestR5";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR5, 2);
            series6.ChartArea = "chartAreaR5";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series6.Legend = "LegenOp4";
            series6.Name = "Series1";
            this.chartRailingTestR5.Series.Add(series6);
            this.chartRailingTestR5.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR5.TabIndex = 26;
            this.chartRailingTestR5.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title6.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title6.DockedToChartArea = "chartAreaR5";
            title6.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title6.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title6.ForeColor = System.Drawing.Color.White;
            title6.IsDockedInsideChartArea = false;
            title6.Name = "railingResRailingTestR5";
            title6.Text = "railR5";
            title6.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR5.Titles.Add(title6);
            // 
            // chartRailingTestR4
            // 
            this.chartRailingTestR4.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR4.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR4.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR4.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR4.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea7.AxisX.IsLabelAutoFit = false;
            chartArea7.AxisX.IsMarginVisible = false;
            chartArea7.AxisX.LabelStyle.Enabled = false;
            chartArea7.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea7.AxisX.MajorGrid.Enabled = false;
            chartArea7.AxisX.MajorTickMark.Enabled = false;
            chartArea7.AxisX.Maximum = 1250D;
            chartArea7.AxisX.Minimum = 0D;
            chartArea7.AxisX.ScaleView.Zoomable = false;
            chartArea7.AxisX.ScrollBar.Enabled = false;
            chartArea7.AxisY.IsLabelAutoFit = false;
            chartArea7.AxisY.LabelStyle.Enabled = false;
            chartArea7.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea7.AxisY.MajorGrid.Enabled = false;
            chartArea7.AxisY.MajorTickMark.Enabled = false;
            chartArea7.BackColor = System.Drawing.Color.Transparent;
            chartArea7.Name = "chartAreaR4";
            this.chartRailingTestR4.ChartAreas.Add(chartArea7);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR4, 12);
            this.chartRailingTestR4.IsSoftShadows = false;
            legend7.Enabled = false;
            legend7.Name = "LegenOp4";
            this.chartRailingTestR4.Legends.Add(legend7);
            this.chartRailingTestR4.Location = new System.Drawing.Point(556, 308);
            this.chartRailingTestR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR4.Name = "chartRailingTestR4";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR4, 2);
            series7.ChartArea = "chartAreaR4";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series7.Legend = "LegenOp4";
            series7.Name = "Series1";
            this.chartRailingTestR4.Series.Add(series7);
            this.chartRailingTestR4.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR4.TabIndex = 23;
            this.chartRailingTestR4.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title7.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title7.DockedToChartArea = "chartAreaR4";
            title7.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title7.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title7.ForeColor = System.Drawing.Color.White;
            title7.IsDockedInsideChartArea = false;
            title7.Name = "railingResRailingTestR4";
            title7.Text = "railR4";
            title7.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR4.Titles.Add(title7);
            // 
            // chartRailingTestR3
            // 
            this.chartRailingTestR3.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR3.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR3.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR3.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR3.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea8.AxisX.IsLabelAutoFit = false;
            chartArea8.AxisX.IsMarginVisible = false;
            chartArea8.AxisX.LabelStyle.Enabled = false;
            chartArea8.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea8.AxisX.MajorGrid.Enabled = false;
            chartArea8.AxisX.MajorTickMark.Enabled = false;
            chartArea8.AxisX.Maximum = 1250D;
            chartArea8.AxisX.Minimum = 0D;
            chartArea8.AxisX.ScaleView.Zoomable = false;
            chartArea8.AxisX.ScrollBar.Enabled = false;
            chartArea8.AxisY.IsLabelAutoFit = false;
            chartArea8.AxisY.LabelStyle.Enabled = false;
            chartArea8.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea8.AxisY.MajorGrid.Enabled = false;
            chartArea8.AxisY.MajorTickMark.Enabled = false;
            chartArea8.BackColor = System.Drawing.Color.Transparent;
            chartArea8.Name = "chartAreaR3";
            this.chartRailingTestR3.ChartAreas.Add(chartArea8);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR3, 12);
            this.chartRailingTestR3.IsSoftShadows = false;
            legend8.Enabled = false;
            legend8.Name = "LegenOp3";
            this.chartRailingTestR3.Legends.Add(legend8);
            this.chartRailingTestR3.Location = new System.Drawing.Point(556, 220);
            this.chartRailingTestR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR3.Name = "chartRailingTestR3";
            this.tableLayoutRailingTest.SetRowSpan(this.chartRailingTestR3, 2);
            series8.ChartArea = "chartAreaR3";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series8.Legend = "LegenOp3";
            series8.Name = "Series1";
            this.chartRailingTestR3.Series.Add(series8);
            this.chartRailingTestR3.Size = new System.Drawing.Size(636, 86);
            this.chartRailingTestR3.TabIndex = 10;
            this.chartRailingTestR3.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title8.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title8.DockedToChartArea = "chartAreaR3";
            title8.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title8.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title8.ForeColor = System.Drawing.Color.White;
            title8.IsDockedInsideChartArea = false;
            title8.Name = "railingResRailingTestR3";
            title8.Text = "railR3";
            title8.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR3.Titles.Add(title8);
            // 
            // labelBCISignalCheck
            // 
            this.labelBCISignalCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBCISignalCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelBCISignalCheck, 8);
            this.labelBCISignalCheck.Font = new System.Drawing.Font("Montserrat", 46F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBCISignalCheck.ForeColor = System.Drawing.Color.White;
            this.labelBCISignalCheck.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelBCISignalCheck.Location = new System.Drawing.Point(110, 10);
            this.labelBCISignalCheck.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelBCISignalCheck.Name = "labelBCISignalCheck";
            this.labelBCISignalCheck.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutPanelMain.SetRowSpan(this.labelBCISignalCheck, 4);
            this.labelBCISignalCheck.Size = new System.Drawing.Size(410, 175);
            this.labelBCISignalCheck.TabIndex = 97;
            this.labelBCISignalCheck.Text = "BCI Signal\r\nCheck";
            this.labelBCISignalCheck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelSignalQuality
            // 
            this.panelSignalQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSignalQuality.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.panelSignalQuality.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelSignalQuality.BackgroundImage")));
            this.panelSignalQuality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanelMain.SetColumnSpan(this.panelSignalQuality, 6);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE2);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE3);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE4);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE8);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE5);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE1);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE6);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapE7);
            this.panelSignalQuality.Font = new System.Drawing.Font("Montserrat", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSignalQuality.Location = new System.Drawing.Point(220, 685);
            this.panelSignalQuality.Margin = new System.Windows.Forms.Padding(0);
            this.panelSignalQuality.Name = "panelSignalQuality";
            this.tableLayoutPanelMain.SetRowSpan(this.panelSignalQuality, 7);
            this.panelSignalQuality.Size = new System.Drawing.Size(300, 300);
            this.panelSignalQuality.TabIndex = 97;
            // 
            // btnElectrodeCapE2
            // 
            this.btnElectrodeCapE2.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE2.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE2.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE2.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE2.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE2.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE2.BorderWidth = 1F;
            this.btnElectrodeCapE2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE2.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE2.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE2.Location = new System.Drawing.Point(26, 93);
            this.btnElectrodeCapE2.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE2.Name = "btnElectrodeCapE2";
            this.btnElectrodeCapE2.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE2.TabIndex = 55;
            this.btnElectrodeCapE2.Text = "2";
            this.btnElectrodeCapE2.UseMnemonic = false;
            this.btnElectrodeCapE2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE3
            // 
            this.btnElectrodeCapE3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE3.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE3.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE3.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE3.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE3.BorderWidth = 1F;
            this.btnElectrodeCapE3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE3.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE3.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE3.Location = new System.Drawing.Point(127, 111);
            this.btnElectrodeCapE3.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE3.Name = "btnElectrodeCapE3";
            this.btnElectrodeCapE3.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE3.TabIndex = 51;
            this.btnElectrodeCapE3.Text = "3";
            this.btnElectrodeCapE3.UseMnemonic = false;
            this.btnElectrodeCapE3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE4
            // 
            this.btnElectrodeCapE4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE4.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE4.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE4.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE4.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE4.BorderWidth = 1F;
            this.btnElectrodeCapE4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE4.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE4.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE4.Location = new System.Drawing.Point(229, 93);
            this.btnElectrodeCapE4.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE4.Name = "btnElectrodeCapE4";
            this.btnElectrodeCapE4.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE4.TabIndex = 49;
            this.btnElectrodeCapE4.Text = "4";
            this.btnElectrodeCapE4.UseMnemonic = false;
            this.btnElectrodeCapE4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE8
            // 
            this.btnElectrodeCapE8.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE8.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE8.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE8.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE8.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE8.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE8.BorderWidth = 1F;
            this.btnElectrodeCapE8.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE8.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE8.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE8.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE8.Location = new System.Drawing.Point(229, 206);
            this.btnElectrodeCapE8.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE8.Name = "btnElectrodeCapE8";
            this.btnElectrodeCapE8.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE8.TabIndex = 48;
            this.btnElectrodeCapE8.Text = "8";
            this.btnElectrodeCapE8.UseMnemonic = false;
            this.btnElectrodeCapE8.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE5
            // 
            this.btnElectrodeCapE5.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE5.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE5.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE5.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE5.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE5.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE5.BorderWidth = 1F;
            this.btnElectrodeCapE5.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE5.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE5.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE5.Location = new System.Drawing.Point(127, 183);
            this.btnElectrodeCapE5.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE5.Name = "btnElectrodeCapE5";
            this.btnElectrodeCapE5.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE5.TabIndex = 47;
            this.btnElectrodeCapE5.Text = "5";
            this.btnElectrodeCapE5.UseMnemonic = false;
            this.btnElectrodeCapE5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE1
            // 
            this.btnElectrodeCapE1.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE1.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE1.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE1.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE1.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE1.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE1.BorderWidth = 1F;
            this.btnElectrodeCapE1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE1.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE1.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE1.Location = new System.Drawing.Point(127, 39);
            this.btnElectrodeCapE1.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE1.Name = "btnElectrodeCapE1";
            this.btnElectrodeCapE1.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE1.TabIndex = 26;
            this.btnElectrodeCapE1.Text = "1";
            this.btnElectrodeCapE1.UseMnemonic = false;
            this.btnElectrodeCapE1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE6
            // 
            this.btnElectrodeCapE6.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE6.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE6.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE6.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE6.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE6.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE6.BorderWidth = 1F;
            this.btnElectrodeCapE6.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE6.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE6.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE6.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE6.Location = new System.Drawing.Point(26, 208);
            this.btnElectrodeCapE6.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE6.Name = "btnElectrodeCapE6";
            this.btnElectrodeCapE6.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE6.TabIndex = 26;
            this.btnElectrodeCapE6.Text = "6";
            this.btnElectrodeCapE6.UseMnemonic = false;
            this.btnElectrodeCapE6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapE7
            // 
            this.btnElectrodeCapE7.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapE7.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE7.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapE7.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapE7.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapE7.BorderRadiusTopRight = 40;
            this.btnElectrodeCapE7.BorderWidth = 1F;
            this.btnElectrodeCapE7.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE7.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapE7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapE7.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapE7.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapE7.Location = new System.Drawing.Point(127, 260);
            this.btnElectrodeCapE7.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapE7.Name = "btnElectrodeCapE7";
            this.btnElectrodeCapE7.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapE7.TabIndex = 26;
            this.btnElectrodeCapE7.Text = "7";
            this.btnElectrodeCapE7.UseMnemonic = false;
            this.btnElectrodeCapE7.UseVisualStyleBackColor = false;
            // 
            // webBrowser
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.webBrowser, 25);
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(613, 938);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScrollBarsEnabled = false;
            this.webBrowser.Size = new System.Drawing.Size(1244, 39);
            this.webBrowser.TabIndex = 103;
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
            this.label53.Text = "R5 Railing";
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
            this.label59.Text = "R2 Railing";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label59.UseCompatibleTextRendering = true;
            // 
            // btnElectrodeCapOp2
            // 
            this.btnElectrodeCapOp2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnElectrodeCapOp2.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeCapOp2.BorderRadiusBottomLeft = 60;
            this.btnElectrodeCapOp2.BorderRadiusBottomRight = 60;
            this.btnElectrodeCapOp2.BorderRadiusTopLeft = 60;
            this.btnElectrodeCapOp2.BorderRadiusTopRight = 60;
            this.btnElectrodeCapOp2.BorderWidth = 4F;
            this.btnElectrodeCapOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapOp2.Font = new System.Drawing.Font("Montserrat", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapOp2.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeCapOp2.Location = new System.Drawing.Point(62, 78);
            this.btnElectrodeCapOp2.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapOp2.Name = "btnElectrodeCapOp2";
            this.btnElectrodeCapOp2.Size = new System.Drawing.Size(46, 46);
            this.btnElectrodeCapOp2.TabIndex = 35;
            this.btnElectrodeCapOp2.Text = "Op2";
            this.btnElectrodeCapOp2.UseMnemonic = false;
            this.btnElectrodeCapOp2.UseVisualStyleBackColor = false;
            // 
            // UserControlBCISignalCheck
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlBCISignalCheck";
            this.Size = new System.Drawing.Size(1920, 1080);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tabControlSignalQuality.ResumeLayout(false);
            this.tabPageRailing.ResumeLayout(false);
            this.tableLayoutRailingTest.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR3)).EndInit();
            this.panelSignalQuality.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel panelSignalQualitySlider;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Label labelBCISignalCheck;
        public System.Windows.Forms.Button buttonBack;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonNext_userControlBCISignalCheck;
        public System.Windows.Forms.Button buttonExit_userControlBCISignalCheck;
        private System.Windows.Forms.Label labelBCISignalCheckDescription;


        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR8;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR7;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR6;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR5;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR1;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR2;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR3;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestR4;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp1;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp2;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp3;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp4;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp5;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp6;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp7;
        public System.Windows.Forms.DataVisualization.Charting.Title railingResRailingTestOp8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapOp2;
        private System.Windows.Forms.Panel panelSignalQuality;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapE7;
        private System.Windows.Forms.WebBrowser webBrowser;
        public System.Windows.Forms.TabControl tabControlSignalQuality;
        public System.Windows.Forms.TabPage tabPageRailing;
        private System.Windows.Forms.TableLayoutPanel tableLayoutRailingTest;
        private System.Windows.Forms.Label labelRailingTestInfo3;
        private System.Windows.Forms.Label labelRailingTestInfo2;
        private System.Windows.Forms.Label labelRailingTestInfo1;
        private System.Windows.Forms.Label labelRailingTest;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR5;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR3;
        private System.Windows.Forms.Label labelElectrodeRailingRailingTest;
        private System.Windows.Forms.Label labelRequiredRailingTest;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR2;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.Label label73;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR7;
        // public UCCapLEDStatus2 ucCapLEDStatus21;
    }
}
