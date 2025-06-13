using System;

namespace ACAT.Extensions.Default.Actuators.EEG.EEGDataAcquisition
{
    partial class SignalMonitorForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea9 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend9 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this._timerPlotData = new System.Windows.Forms.Timer(this.components);
            this.cbPorts = new System.Windows.Forms.ComboBox();
            this.cbNotch = new System.Windows.Forms.ComboBox();
            this.lblNotch = new System.Windows.Forms.Label();
            this.btnStartStopDevice = new System.Windows.Forms.Button();
            this.lblPort = new System.Windows.Forms.Label();
            this.chartSignal1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cbScale = new System.Windows.Forms.ComboBox();
            this.chartSignal2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal5 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal6 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal7 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartSignal8 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chMarker = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panelSignal = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chMarker)).BeginInit();
            this.panelSignal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbPorts
            // 
            this.cbPorts.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbPorts.FormattingEnabled = true;
            this.cbPorts.Location = new System.Drawing.Point(223, 27);
            this.cbPorts.Margin = new System.Windows.Forms.Padding(4);
            this.cbPorts.Name = "cbPorts";
            this.cbPorts.Size = new System.Drawing.Size(78, 24);
            this.cbPorts.TabIndex = 23;
            this.cbPorts.SelectedIndexChanged += new System.EventHandler(this.CbPorts_SelectedIndexChanged);
            // 
            // cbNotch
            // 
            this.cbNotch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbNotch.FormattingEnabled = true;
            this.cbNotch.Items.AddRange(new object[] {
            "None",
            "50 Hz (Europe)",
            "60 Hz (US)"});
            this.cbNotch.Location = new System.Drawing.Point(411, 27);
            this.cbNotch.Margin = new System.Windows.Forms.Padding(4);
            this.cbNotch.Name = "cbNotch";
            this.cbNotch.Size = new System.Drawing.Size(115, 24);
            this.cbNotch.TabIndex = 33;
            this.cbNotch.SelectedIndexChanged += new System.EventHandler(this.CbNotch_SelectedIndexChanged);
            // 
            // lblNotch
            // 
            this.lblNotch.AutoSize = true;
            this.lblNotch.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.lblNotch.Location = new System.Drawing.Point(327, 30);
            this.lblNotch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNotch.Name = "lblNotch";
            this.lblNotch.Size = new System.Drawing.Size(70, 16);
            this.lblNotch.TabIndex = 35;
            this.lblNotch.Text = "Notch filter";
            // 
            // btnStartStopDevice
            // 
            this.btnStartStopDevice.BackColor = System.Drawing.Color.SteelBlue;
            this.btnStartStopDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStopDevice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnStartStopDevice.Location = new System.Drawing.Point(25, 19);
            this.btnStartStopDevice.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStopDevice.Name = "btnStartStopDevice";
            this.btnStartStopDevice.Size = new System.Drawing.Size(148, 40);
            this.btnStartStopDevice.TabIndex = 37;
            this.btnStartStopDevice.Text = "Start Device";
            this.btnStartStopDevice.UseVisualStyleBackColor = false;
            this.btnStartStopDevice.Click += new System.EventHandler(this.BtnStartStopDevice_Click);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.lblPort.Location = new System.Drawing.Point(181, 30);
            this.lblPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(32, 16);
            this.lblPort.TabIndex = 36;
            this.lblPort.Text = "Port";
            // 
            // chartSignal1
            // 
            this.chartSignal1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.chartSignal1.BorderlineColor = System.Drawing.Color.DimGray;
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
            chartArea1.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea1.Name = "ChartArea1";
            this.chartSignal1.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chartSignal1.Legends.Add(legend1);
            this.chartSignal1.Location = new System.Drawing.Point(9, 4);
            this.chartSignal1.Margin = new System.Windows.Forms.Padding(1);
            this.chartSignal1.Name = "chartSignal1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.Silver;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartSignal1.Series.Add(series1);
            this.chartSignal1.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal1.TabIndex = 8;
            this.chartSignal1.Text = "chart2";
            // 
            // cbScale
            // 
            this.cbScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbScale.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbScale.FormattingEnabled = true;
            this.cbScale.Items.AddRange(new object[] {
            "± 50 uV",
            "± 100 uV",
            "± 200 uV",
            "± 500 uV",
            "± 1 mV"});
            this.cbScale.Location = new System.Drawing.Point(1325, 28);
            this.cbScale.Margin = new System.Windows.Forms.Padding(4);
            this.cbScale.Name = "cbScale";
            this.cbScale.Size = new System.Drawing.Size(105, 24);
            this.cbScale.TabIndex = 31;
            this.cbScale.SelectedIndexChanged += new System.EventHandler(this.CbScale_SelectedIndexChanged);
            // 
            // chartSignal2
            // 
            this.chartSignal2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal2.BackColor = System.Drawing.SystemColors.WindowFrame;
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
            chartArea2.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea2.Name = "ChartArea1";
            this.chartSignal2.ChartAreas.Add(chartArea2);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chartSignal2.Legends.Add(legend2);
            this.chartSignal2.Location = new System.Drawing.Point(9, 105);
            this.chartSignal2.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal2.Name = "chartSignal2";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.Silver;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartSignal2.Series.Add(series2);
            this.chartSignal2.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal2.TabIndex = 41;
            this.chartSignal2.Text = "chart2";
            // 
            // chartSignal3
            // 
            this.chartSignal3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal3.BackColor = System.Drawing.SystemColors.WindowFrame;
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
            chartArea3.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea3.Name = "ChartArea1";
            this.chartSignal3.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chartSignal3.Legends.Add(legend3);
            this.chartSignal3.Location = new System.Drawing.Point(9, 206);
            this.chartSignal3.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal3.Name = "chartSignal3";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Color = System.Drawing.Color.Silver;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartSignal3.Series.Add(series3);
            this.chartSignal3.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal3.TabIndex = 43;
            this.chartSignal3.Text = "chart2";
            // 
            // chartSignal4
            // 
            this.chartSignal4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal4.BackColor = System.Drawing.SystemColors.WindowFrame;
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
            chartArea4.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea4.Name = "ChartArea1";
            this.chartSignal4.ChartAreas.Add(chartArea4);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chartSignal4.Legends.Add(legend4);
            this.chartSignal4.Location = new System.Drawing.Point(9, 306);
            this.chartSignal4.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal4.Name = "chartSignal4";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Color = System.Drawing.Color.Silver;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartSignal4.Series.Add(series4);
            this.chartSignal4.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal4.TabIndex = 45;
            this.chartSignal4.Text = "chart2";
            // 
            // chartSignal5
            // 
            this.chartSignal5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal5.BackColor = System.Drawing.SystemColors.WindowFrame;
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
            chartArea5.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea5.Name = "ChartArea1";
            this.chartSignal5.ChartAreas.Add(chartArea5);
            legend5.Enabled = false;
            legend5.Name = "Legend1";
            this.chartSignal5.Legends.Add(legend5);
            this.chartSignal5.Location = new System.Drawing.Point(9, 407);
            this.chartSignal5.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal5.Name = "chartSignal5";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series5.Color = System.Drawing.Color.Silver;
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chartSignal5.Series.Add(series5);
            this.chartSignal5.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal5.TabIndex = 47;
            this.chartSignal5.Text = "chart2";
            // 
            // chartSignal6
            // 
            this.chartSignal6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal6.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea6.AxisX.IsLabelAutoFit = false;
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
            chartArea6.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea6.Name = "ChartArea1";
            this.chartSignal6.ChartAreas.Add(chartArea6);
            legend6.Enabled = false;
            legend6.Name = "Legend1";
            this.chartSignal6.Legends.Add(legend6);
            this.chartSignal6.Location = new System.Drawing.Point(9, 508);
            this.chartSignal6.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal6.Name = "chartSignal6";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series6.Color = System.Drawing.Color.Silver;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chartSignal6.Series.Add(series6);
            this.chartSignal6.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal6.TabIndex = 49;
            this.chartSignal6.Text = "chart2";
            // 
            // chartSignal7
            // 
            this.chartSignal7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal7.BackColor = System.Drawing.SystemColors.WindowFrame;
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
            chartArea7.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea7.Name = "ChartArea1";
            this.chartSignal7.ChartAreas.Add(chartArea7);
            legend7.Enabled = false;
            legend7.Name = "Legend1";
            this.chartSignal7.Legends.Add(legend7);
            this.chartSignal7.Location = new System.Drawing.Point(9, 609);
            this.chartSignal7.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal7.Name = "chartSignal7";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series7.Color = System.Drawing.Color.Silver;
            series7.Legend = "Legend1";
            series7.Name = "Series1";
            this.chartSignal7.Series.Add(series7);
            this.chartSignal7.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal7.TabIndex = 51;
            this.chartSignal7.Text = "chart2";
            // 
            // chartSignal8
            // 
            this.chartSignal8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartSignal8.BackColor = System.Drawing.SystemColors.WindowFrame;
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
            chartArea8.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea8.Name = "ChartArea1";
            this.chartSignal8.ChartAreas.Add(chartArea8);
            legend8.Enabled = false;
            legend8.Name = "Legend1";
            this.chartSignal8.Legends.Add(legend8);
            this.chartSignal8.Location = new System.Drawing.Point(9, 710);
            this.chartSignal8.Margin = new System.Windows.Forms.Padding(4);
            this.chartSignal8.Name = "chartSignal8";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series8.Color = System.Drawing.Color.Silver;
            series8.Legend = "Legend1";
            series8.Name = "Series1";
            this.chartSignal8.Series.Add(series8);
            this.chartSignal8.Size = new System.Drawing.Size(1405, 92);
            this.chartSignal8.TabIndex = 53;
            this.chartSignal8.Text = "chart2";
            // 
            // chMarker
            // 
            this.chMarker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chMarker.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea9.AxisX.IsLabelAutoFit = false;
            chartArea9.AxisX.IsMarginVisible = false;
            chartArea9.AxisX.LabelStyle.Enabled = false;
            chartArea9.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea9.AxisX.MajorGrid.Enabled = false;
            chartArea9.AxisX.MajorTickMark.Enabled = false;
            chartArea9.AxisX.Maximum = 1250D;
            chartArea9.AxisX.Minimum = 0D;
            chartArea9.AxisX.ScaleView.Zoomable = false;
            chartArea9.AxisX.ScrollBar.Enabled = false;
            chartArea9.AxisY.IsLabelAutoFit = false;
            chartArea9.AxisY.LabelStyle.Enabled = false;
            chartArea9.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea9.AxisY.MajorGrid.Enabled = false;
            chartArea9.AxisY.MajorTickMark.Enabled = false;
            chartArea9.BackColor = System.Drawing.SystemColors.WindowFrame;
            chartArea9.Name = "ChartArea1";
            this.chMarker.ChartAreas.Add(chartArea9);
            legend9.Enabled = false;
            legend9.Name = "Legend1";
            this.chMarker.Legends.Add(legend9);
            this.chMarker.Location = new System.Drawing.Point(9, 822);
            this.chMarker.Margin = new System.Windows.Forms.Padding(4);
            this.chMarker.Name = "chMarker";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series9.Color = System.Drawing.Color.Silver;
            series9.Legend = "Legend1";
            series9.Name = "Series1";
            this.chMarker.Series.Add(series9);
            this.chMarker.Size = new System.Drawing.Size(1405, 70);
            this.chMarker.TabIndex = 55;
            this.chMarker.Text = "chart2";
            // 
            // panelSignal
            // 
            this.panelSignal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSignal.Controls.Add(this.chMarker);
            this.panelSignal.Controls.Add(this.chartSignal8);
            this.panelSignal.Controls.Add(this.chartSignal7);
            this.panelSignal.Controls.Add(this.chartSignal6);
            this.panelSignal.Controls.Add(this.chartSignal5);
            this.panelSignal.Controls.Add(this.chartSignal4);
            this.panelSignal.Controls.Add(this.chartSignal3);
            this.panelSignal.Controls.Add(this.chartSignal2);
            this.panelSignal.Controls.Add(this.chartSignal1);
            this.panelSignal.Location = new System.Drawing.Point(16, 67);
            this.panelSignal.Margin = new System.Windows.Forms.Padding(4);
            this.panelSignal.Name = "panelSignal";
            this.panelSignal.Size = new System.Drawing.Size(1418, 912);
            this.panelSignal.TabIndex = 39;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.label1.Location = new System.Drawing.Point(1274, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 40;
            this.label1.Text = "Scale";
            // 
            // SignalMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1447, 942);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbNotch);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblNotch);
            this.Controls.Add(this.panelSignal);
            this.Controls.Add(this.btnStartStopDevice);
            this.Controls.Add(this.cbPorts);
            this.Controls.Add(this.cbScale);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SignalMonitorForm";
            this.Text = " ";
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartSignal8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chMarker)).EndInit();
            this.panelSignal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer _timerPlotData;
        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.ComboBox cbNotch;
        private System.Windows.Forms.Label lblNotch;
        private System.Windows.Forms.Button btnStartStopDevice;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal1;
        private System.Windows.Forms.ComboBox cbScale;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal5;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSignal8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chMarker;
        private System.Windows.Forms.Panel panelSignal;
        private System.Windows.Forms.Label label1;
    }
}

