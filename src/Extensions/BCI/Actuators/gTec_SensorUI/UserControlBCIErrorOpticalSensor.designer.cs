////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorOpticalSensor.designer.cs
//
// User control which displays information on errors related to the optical sensor
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which displays information on errors related to the optical sensor
    /// </summary>
    partial class UserControlBCIErrorOpticalSensor
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(1D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(3D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint7 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(3D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint8 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(4D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint9 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(4D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint10 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(5D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint11 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(5D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint12 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(6D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint13 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(6D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint14 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(7D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint15 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(7D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint16 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(8D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint17 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(8D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint18 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(9D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint19 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(9D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint20 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(10D, 1D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint21 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(10D, 0D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint22 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(11D, 0D);
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlBCIErrorOpticalSensor));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.luxSlider = new ColorSlider.ColorSlider();
            this.tableLayoutPanelSpacerTop = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanelBCIOpticalSensor = new System.Windows.Forms.TableLayoutPanel();
            this.chartExample = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panelImageOpticalSensorError = new System.Windows.Forms.Panel();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExit = new System.Windows.Forms.Button();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.webBrowserBottom = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonLuxSliderMinus = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonLuxSliderPlus = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonRetry = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelBCIOpticalSensor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartExample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.44032F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.37312F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.37312F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.37312F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.44032F));
            this.tableLayoutPanelMain.Controls.Add(this.luxSlider, 2, 4);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerTop, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelBCIOpticalSensor, 1, 6);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 1, 11);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit, 1, 10);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowserTop, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowserBottom, 1, 7);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel1, 1, 4);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel2, 3, 4);
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 12;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.340979F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.613762F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.340979F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.545568F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.814319F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.340979F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.80015F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.340979F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.272784F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.272784F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.975747F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.340979F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.TabIndex = 9;
            // 
            // luxSlider
            // 
            this.luxSlider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.luxSlider.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.luxSlider.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.luxSlider.BarPenColorTop = System.Drawing.Color.Empty;
            this.luxSlider.BorderRoundRectSize = new System.Drawing.Size(20, 20);
            this.luxSlider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.luxSlider.DrawSemitransparentThumb = false;
            this.luxSlider.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.luxSlider.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.luxSlider.ElapsedPenColorTop = System.Drawing.Color.Empty;
            this.luxSlider.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.luxSlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.luxSlider.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.luxSlider.Location = new System.Drawing.Point(386, 218);
            this.luxSlider.Margin = new System.Windows.Forms.Padding(0);
            this.luxSlider.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.luxSlider.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.luxSlider.MouseEffects = false;
            this.luxSlider.Name = "luxSlider";
            this.tableLayoutPanelMain.SetRowSpan(this.luxSlider, 2);
            this.luxSlider.ScaleDivisions = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.luxSlider.ScaleSubDivisions = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.luxSlider.ShowDivisionsText = false;
            this.luxSlider.ShowSmallScale = false;
            this.luxSlider.Size = new System.Drawing.Size(249, 92);
            this.luxSlider.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.luxSlider.TabIndex = 87;
            this.luxSlider.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.luxSlider.ThumbOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.luxSlider.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.luxSlider.ThumbRoundRectSize = new System.Drawing.Size(56, 56);
            this.luxSlider.ThumbSize = new System.Drawing.Size(56, 56);
            this.luxSlider.TickAdd = 0F;
            this.luxSlider.TickColor = System.Drawing.Color.White;
            this.luxSlider.TickDivide = 0F;
            this.luxSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.luxSlider.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // tableLayoutPanelSpacerTop
            // 
            this.tableLayoutPanelSpacerTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerTop.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerTop, 3);
            this.tableLayoutPanelSpacerTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelSpacerTop.Location = new System.Drawing.Point(137, 0);
            this.tableLayoutPanelSpacerTop.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerTop.Name = "tableLayoutPanelSpacerTop";
            this.tableLayoutPanelSpacerTop.RowCount = 1;
            this.tableLayoutPanelSpacerTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Size = new System.Drawing.Size(747, 38);
            this.tableLayoutPanelSpacerTop.TabIndex = 73;
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelTitle, 3);
            this.labelTitle.Font = new System.Drawing.Font("Montserrat", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(220, 40);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(580, 64);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "BCI Optical Sensor Error";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.UseCompatibleTextRendering = true;
            // 
            // tableLayoutPanelBCIOpticalSensor
            // 
            this.tableLayoutPanelBCIOpticalSensor.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelBCIOpticalSensor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanelBCIOpticalSensor.ColumnCount = 6;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelBCIOpticalSensor, 3);
            this.tableLayoutPanelBCIOpticalSensor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.52248F));
            this.tableLayoutPanelBCIOpticalSensor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.2955F));
            this.tableLayoutPanelBCIOpticalSensor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.2955F));
            this.tableLayoutPanelBCIOpticalSensor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.2955F));
            this.tableLayoutPanelBCIOpticalSensor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.2955F));
            this.tableLayoutPanelBCIOpticalSensor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.2955F));
            this.tableLayoutPanelBCIOpticalSensor.Controls.Add(this.chartExample, 1, 1);
            this.tableLayoutPanelBCIOpticalSensor.Controls.Add(this.chartSignal, 1, 0);
            this.tableLayoutPanelBCIOpticalSensor.Controls.Add(this.panelImageOpticalSensorError, 0, 0);
            this.tableLayoutPanelBCIOpticalSensor.Font = new System.Drawing.Font("Montserrat Medium", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanelBCIOpticalSensor.ForeColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelBCIOpticalSensor.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tableLayoutPanelBCIOpticalSensor.Location = new System.Drawing.Point(137, 310);
            this.tableLayoutPanelBCIOpticalSensor.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelBCIOpticalSensor.Name = "tableLayoutPanelBCIOpticalSensor";
            this.tableLayoutPanelBCIOpticalSensor.RowCount = 2;
            this.tableLayoutPanelBCIOpticalSensor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBCIOpticalSensor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBCIOpticalSensor.Size = new System.Drawing.Size(747, 184);
            this.tableLayoutPanelBCIOpticalSensor.TabIndex = 70;
            this.tableLayoutPanelBCIOpticalSensor.Text = "Row for error visualization";
            // 
            // chartExample
            // 
            this.chartExample.BackColor = System.Drawing.Color.Transparent;
            this.chartExample.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.Maximum = 9.25D;
            chartArea1.AxisX.Minimum = 0.25D;
            chartArea1.AxisX.ScaleView.Zoomable = false;
            chartArea1.AxisX.ScrollBar.Enabled = false;
            chartArea1.AxisX2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea1.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.BorderColor = System.Drawing.Color.White;
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "chartAreaExample";
            this.chartExample.ChartAreas.Add(chartArea1);
            this.tableLayoutPanelBCIOpticalSensor.SetColumnSpan(this.chartExample, 5);
            this.chartExample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartExample.Location = new System.Drawing.Point(138, 92);
            this.chartExample.Margin = new System.Windows.Forms.Padding(0);
            this.chartExample.Name = "chartExample";
            series1.BorderWidth = 3;
            series1.ChartArea = "chartAreaExample";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            series1.IsVisibleInLegend = false;
            series1.Name = "SignalExample";
            series1.Points.Add(dataPoint1);
            series1.Points.Add(dataPoint2);
            series1.Points.Add(dataPoint3);
            series1.Points.Add(dataPoint4);
            series1.Points.Add(dataPoint5);
            series1.Points.Add(dataPoint6);
            series1.Points.Add(dataPoint7);
            series1.Points.Add(dataPoint8);
            series1.Points.Add(dataPoint9);
            series1.Points.Add(dataPoint10);
            series1.Points.Add(dataPoint11);
            series1.Points.Add(dataPoint12);
            series1.Points.Add(dataPoint13);
            series1.Points.Add(dataPoint14);
            series1.Points.Add(dataPoint15);
            series1.Points.Add(dataPoint16);
            series1.Points.Add(dataPoint17);
            series1.Points.Add(dataPoint18);
            series1.Points.Add(dataPoint19);
            series1.Points.Add(dataPoint20);
            series1.Points.Add(dataPoint21);
            series1.Points.Add(dataPoint22);
            series1.ShadowColor = System.Drawing.Color.Transparent;
            this.chartExample.Series.Add(series1);
            this.chartExample.Size = new System.Drawing.Size(609, 92);
            this.chartExample.TabIndex = 61;
            this.chartExample.Text = "Chart Example";
            title1.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            title1.DockedToChartArea = "chartAreaExample";
            title1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            title1.Font = new System.Drawing.Font("Montserrat", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.ForeColor = System.Drawing.Color.White;
            title1.Name = "Good Signal Example";
            title1.Position.Auto = false;
            title1.Position.Height = 87.94772F;
            title1.Position.Width = 63.52163F;
            title1.Position.X = 2.671532F;
            title1.Position.Y = 5.806842F;
            title1.Text = "Good Signal Example";
            title1.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartExample.Titles.Add(title1);
            // 
            // chartSignal
            // 
            this.chartSignal.BackColor = System.Drawing.Color.Transparent;
            this.chartSignal.BorderlineColor = System.Drawing.Color.Transparent;
            this.chartSignal.BorderlineWidth = 0;
            this.chartSignal.BorderSkin.BackColor = System.Drawing.Color.Transparent;
            this.chartSignal.BorderSkin.BorderWidth = 0;
            chartArea2.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
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
            chartArea2.AxisX2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea2.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea2.AxisY.IsLabelAutoFit = false;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorTickMark.Enabled = false;
            chartArea2.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea2.BackColor = System.Drawing.Color.Transparent;
            chartArea2.BorderColor = System.Drawing.Color.White;
            chartArea2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.Name = "chartAreaSignal";
            this.chartSignal.ChartAreas.Add(chartArea2);
            this.tableLayoutPanelBCIOpticalSensor.SetColumnSpan(this.chartSignal, 5);
            this.chartSignal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartSignal.Location = new System.Drawing.Point(138, 0);
            this.chartSignal.Margin = new System.Windows.Forms.Padding(0);
            this.chartSignal.Name = "chartSignal";
            series2.BorderWidth = 3;
            series2.ChartArea = "chartAreaSignal";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.Red;
            series2.Name = "Signal";
            this.chartSignal.Series.Add(series2);
            this.chartSignal.Size = new System.Drawing.Size(609, 92);
            this.chartSignal.TabIndex = 60;
            this.chartSignal.Text = "Chart Signal";
            // 
            // panelImageOpticalSensorError
            // 
            this.panelImageOpticalSensorError.BackColor = System.Drawing.Color.White;
            this.panelImageOpticalSensorError.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelImageOpticalSensorError.BackgroundImage")));
            this.panelImageOpticalSensorError.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelImageOpticalSensorError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelImageOpticalSensorError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImageOpticalSensorError.ForeColor = System.Drawing.Color.White;
            this.panelImageOpticalSensorError.Location = new System.Drawing.Point(0, 0);
            this.panelImageOpticalSensorError.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.panelImageOpticalSensorError.Name = "panelImageOpticalSensorError";
            this.tableLayoutPanelBCIOpticalSensor.SetRowSpan(this.panelImageOpticalSensorError, 2);
            this.panelImageOpticalSensorError.Size = new System.Drawing.Size(128, 184);
            this.panelImageOpticalSensorError.TabIndex = 0;
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerBottom, 3);
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(137, 720);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(747, 46);
            this.tableLayoutPanelSpacerBottom.TabIndex = 74;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonExit.AutoSize = true;
            this.buttonExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExit.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit.Location = new System.Drawing.Point(137, 657);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(145, 61);
            this.buttonExit.TabIndex = 84;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseCompatibleTextRendering = true;
            this.buttonExit.UseVisualStyleBackColor = false;
            // 
            // webBrowserTop
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.webBrowserTop, 3);
            this.webBrowserTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserTop.Location = new System.Drawing.Point(140, 116);
            this.webBrowserTop.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserTop.Name = "webBrowserTop";
            this.tableLayoutPanelMain.SetRowSpan(this.webBrowserTop, 2);
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.Size = new System.Drawing.Size(741, 99);
            this.webBrowserTop.TabIndex = 85;
            // 
            // webBrowserBottom
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.webBrowserBottom, 3);
            this.webBrowserBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserBottom.Location = new System.Drawing.Point(140, 556);
            this.webBrowserBottom.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserBottom.Name = "webBrowserBottom";
            this.tableLayoutPanelMain.SetRowSpan(this.webBrowserBottom, 3);
            this.webBrowserBottom.ScrollBarsEnabled = false;
            this.webBrowserBottom.Size = new System.Drawing.Size(741, 98);
            this.webBrowserBottom.TabIndex = 86;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.buttonLuxSliderMinus, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(137, 218);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanelMain.SetRowSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(249, 92);
            this.tableLayoutPanel1.TabIndex = 88;
            // 
            // buttonLuxSliderMinus
            // 
            this.buttonLuxSliderMinus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLuxSliderMinus.FlatAppearance.BorderSize = 0;
            this.buttonLuxSliderMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLuxSliderMinus.Font = new System.Drawing.Font("Montserrat SemiBold", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLuxSliderMinus.ForeColor = System.Drawing.Color.White;
            this.buttonLuxSliderMinus.Location = new System.Drawing.Point(186, 0);
            this.buttonLuxSliderMinus.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLuxSliderMinus.Name = "buttonLuxSliderMinus";
            this.tableLayoutPanel1.SetRowSpan(this.buttonLuxSliderMinus, 3);
            this.buttonLuxSliderMinus.Size = new System.Drawing.Size(63, 92);
            this.buttonLuxSliderMinus.TabIndex = 1;
            this.buttonLuxSliderMinus.Text = "-";
            this.buttonLuxSliderMinus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonLuxSliderMinus.UseVisualStyleBackColor = true;
            this.buttonLuxSliderMinus.Click += new System.EventHandler(this.buttonLuxSliderMinus_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.15152F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.28283F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.28283F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.28283F));
            this.tableLayoutPanel2.Controls.Add(this.buttonLuxSliderPlus, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(638, 221);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanelMain.SetRowSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(243, 86);
            this.tableLayoutPanel2.TabIndex = 89;
            // 
            // buttonLuxSliderPlus
            // 
            this.buttonLuxSliderPlus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLuxSliderPlus.FlatAppearance.BorderSize = 0;
            this.buttonLuxSliderPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLuxSliderPlus.Font = new System.Drawing.Font("Montserrat SemiBold", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLuxSliderPlus.ForeColor = System.Drawing.Color.White;
            this.buttonLuxSliderPlus.Location = new System.Drawing.Point(0, 0);
            this.buttonLuxSliderPlus.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLuxSliderPlus.Name = "buttonLuxSliderPlus";
            this.tableLayoutPanel2.SetRowSpan(this.buttonLuxSliderPlus, 3);
            this.buttonLuxSliderPlus.Size = new System.Drawing.Size(36, 86);
            this.buttonLuxSliderPlus.TabIndex = 0;
            this.buttonLuxSliderPlus.Text = "+";
            this.buttonLuxSliderPlus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonLuxSliderPlus.UseVisualStyleBackColor = true;
            this.buttonLuxSliderPlus.Click += new System.EventHandler(this.buttonLuxSliderPlus_Click);
            // 
            // panel1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.buttonRetry);
            this.panel1.Location = new System.Drawing.Point(39, 20);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 20);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel2.SetRowSpan(this.panel1, 3);
            this.panel1.Size = new System.Drawing.Size(177, 41);
            this.panel1.TabIndex = 1;
            // 
            // buttonRetry
            // 
            this.buttonRetry.AutoSize = true;
            this.buttonRetry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonRetry.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonRetry.BorderColor = System.Drawing.Color.Transparent;
            this.buttonRetry.BorderRadiusBottomLeft = 0;
            this.buttonRetry.BorderRadiusBottomRight = 0;
            this.buttonRetry.BorderRadiusTopLeft = 0;
            this.buttonRetry.BorderRadiusTopRight = 0;
            this.buttonRetry.BorderWidth = 0F;
            this.buttonRetry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRetry.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonRetry.FlatAppearance.BorderSize = 0;
            this.buttonRetry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRetry.Font = new System.Drawing.Font("Montserrat", 20F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRetry.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonRetry.Image = ((System.Drawing.Image)(resources.GetObject("buttonRetry.Image")));
            this.buttonRetry.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRetry.Location = new System.Drawing.Point(0, 0);
            this.buttonRetry.Margin = new System.Windows.Forms.Padding(0);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(177, 41);
            this.buttonRetry.TabIndex = 83;
            this.buttonRetry.Text = "Retry";
            this.buttonRetry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRetry.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonRetry.UseCompatibleTextRendering = true;
            this.buttonRetry.UseMnemonic = false;
            this.buttonRetry.UseVisualStyleBackColor = false;
            // 
            // UserControlBCIErrorOpticalSensor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlBCIErrorOpticalSensor";
            this.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanelBCIOpticalSensor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartExample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBCIOpticalSensor;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.Panel panelImageOpticalSensorError;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartExample;
        public System.Windows.Forms.DataVisualization.Charting.Chart chartSignal;
        public System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.WebBrowser webBrowserTop;
        private System.Windows.Forms.WebBrowser webBrowserBottom;
        private ColorSlider.ColorSlider luxSlider;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonLuxSliderMinus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonLuxSliderPlus;
        private System.Windows.Forms.Panel panel1;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonRetry;
    }
}