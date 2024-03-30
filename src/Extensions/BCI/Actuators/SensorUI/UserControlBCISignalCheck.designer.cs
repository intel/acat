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

namespace ACAT.Extensions.BCI.Actuators.SensorUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea161 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend161 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series161 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title161 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea162 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend162 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series162 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title162 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea163 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend163 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series163 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title163 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea164 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend164 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series164 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title164 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea165 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend165 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series165 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title165 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea166 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend166 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series166 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title166 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea167 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend167 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series167 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title167 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea168 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend168 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series168 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title168 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea169 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend169 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series169 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title169 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea170 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend170 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series170 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title170 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea171 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend171 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series171 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title171 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea172 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend172 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series172 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title172 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea173 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend173 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series173 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title173 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea174 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend174 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series174 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title174 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea175 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend175 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series175 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title175 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea176 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend176 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series176 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title176 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.TriggerBox = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonNext = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonExit = new System.Windows.Forms.Button();
            this.panelSignalQualitySlider = new System.Windows.Forms.Panel();
            this.labelBCISignalCheckDescription = new System.Windows.Forms.Label();
            this.tabControlSignalQuality = new System.Windows.Forms.TabControl();
            this.tabPageRailing = new System.Windows.Forms.TabPage();
            this.tableLayoutRailingTest = new System.Windows.Forms.TableLayoutPanel();
            this.labelRailingTestInfo3 = new System.Windows.Forms.Label();
            this.labelOptionalRailingTest = new System.Windows.Forms.Label();
            this.labelRailingTestInfo2 = new System.Windows.Forms.Label();
            this.chartRailingTestOp8 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestOp7 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestOp6 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestOp5 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestOp4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestOp3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestOp2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelRailingTestInfo1 = new System.Windows.Forms.Label();
            this.labelRailingTest = new System.Windows.Forms.Label();
            this.btnElectrodeRailingTestR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR8 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR7 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnElectrodeRailingTestR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR6 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnElectrodeRailingTestR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR5 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnElectrodeRailingTestR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnElectrodeRailingTestR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.labelElectrodeRailingRailingTest = new System.Windows.Forms.Label();
            this.labelRequiredRailingTest = new System.Windows.Forms.Label();
            this.btnElectrodeRailingTestR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.chartRailingTestR4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartRailingTestR2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label66 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label70 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.label72 = new System.Windows.Forms.Label();
            this.label73 = new System.Windows.Forms.Label();
            this.btnElectrodeRailingTestOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeRailingTestOp8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelEqualsIRailingTest1 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest2 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest4 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest3 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest5 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest6 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest7 = new System.Windows.Forms.Label();
            this.labelEqualsIRailingTest8 = new System.Windows.Forms.Label();
            this.chartRailingTestOp1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnElectrodeRailingTestR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tabPageImpedance = new System.Windows.Forms.TabPage();
            this.tableLayoutImpedanceTest = new System.Windows.Forms.TableLayoutPanel();
            this.labelImpedanceTestInfo2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelImpedanceTestingState5 = new System.Windows.Forms.Label();
            this.labelImpedanceTestingState4 = new System.Windows.Forms.Label();
            this.labelImpedanceTestingState2 = new System.Windows.Forms.Label();
            this.labelImpedanceTestingState3 = new System.Windows.Forms.Label();
            this.btnImpedanceResImpedanceTestR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelImpedanceTestInfo1 = new System.Windows.Forms.Label();
            this.labelImpedanceTest = new System.Windows.Forms.Label();
            this.btnElectrodeImpedanceTestR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btnElectrodeImpedanceTestOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeImpedanceTestOp8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelEqualsImpedanceTest1 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTest2 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTest4 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTestOp5 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTestOp6 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTestOp7 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTestOp8 = new System.Windows.Forms.Label();
            this.labelImpedanceImpedance = new System.Windows.Forms.Label();
            this.btnImpedanceResImpedanceTestR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResImpedanceTestOp8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelOptionalElectrodesImpedanceTest = new System.Windows.Forms.Label();
            this.buttonTestImpedance = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelImpedanceTestingState1 = new System.Windows.Forms.Label();
            this.labelEqualsImpedanceTest3 = new System.Windows.Forms.Label();
            this.tabPageQuality = new System.Windows.Forms.TabPage();
            this.tableLayoutQualityResults = new System.Windows.Forms.TableLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.labelQualityResultsInfo1 = new System.Windows.Forms.Label();
            this.labelQualityResultsInfo2 = new System.Windows.Forms.Label();
            this.btnImpedanceResQualityResultsR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.btnElectrodeQualityResultsOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeQualityResultsOp8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelEqualsQualityResults1 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResults2 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResults4 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResults3 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResults5 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResults6 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResults7 = new System.Windows.Forms.Label();
            this.labelEqualsQualityResult8 = new System.Windows.Forms.Label();
            this.labelImpedanceQualityResults = new System.Windows.Forms.Label();
            this.btnImpedanceResQualityResultsR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnImpedanceResQualityResultsOp8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelOptionalElectrodesQualityResults = new System.Windows.Forms.Label();
            this.labelRailingQualityResults = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.label75 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults1 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults2 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults3 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults4 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults5 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults6 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults7 = new System.Windows.Forms.Label();
            this.labelPlusQualityResults8 = new System.Windows.Forms.Label();
            this.btnRailingResQualityResultsR1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsR8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnRailingResQualityResultsOp8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelQualityResults = new System.Windows.Forms.Label();
            this.labelBCISignalCheck = new System.Windows.Forms.Label();
            this.panelSignalQuality = new System.Windows.Forms.Panel();
            this.scannerRoundedButtonControl1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapT3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapF7 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapF3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapFp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapFp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapFz = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapF4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapF8 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapC4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapCz = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapT5 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapO2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapO1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCap11 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapT6 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapGND = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapP4 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapP3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapC3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.btnElectrodeCapPz = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.label53 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.btnElectrodeCapOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tabControlSignalQuality.SuspendLayout();
            this.tabPageRailing.SuspendLayout();
            this.tableLayoutRailingTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp1)).BeginInit();
            this.tabPageImpedance.SuspendLayout();
            this.tableLayoutImpedanceTest.SuspendLayout();
            this.tabPageQuality.SuspendLayout();
            this.tableLayoutQualityResults.SuspendLayout();
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
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 49F));
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
            this.tableLayoutPanelMain.Controls.Add(this.TriggerBox, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.buttonBack, 29, 20);
            this.tableLayoutPanelMain.Controls.Add(this.buttonNext, 33, 20);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit, 1, 20);
            this.tableLayoutPanelMain.Controls.Add(this.panelSignalQualitySlider, 4, 12);
            this.tableLayoutPanelMain.Controls.Add(this.labelBCISignalCheckDescription, 0, 4);
            this.tableLayoutPanelMain.Controls.Add(this.tabControlSignalQuality, 12, 1);
            this.tableLayoutPanelMain.Controls.Add(this.labelBCISignalCheck, 2, 0);
            this.tableLayoutPanelMain.Controls.Add(this.panelSignalQuality, 4, 14);
            this.tableLayoutPanelMain.Controls.Add(this.webBrowser, 12, 19);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
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
            // TriggerBox
            // 
            this.TriggerBox.BackColor = System.Drawing.Color.White;
            this.TriggerBox.BorderColor = System.Drawing.Color.DimGray;
            this.TriggerBox.BorderRadiusBottomLeft = 0;
            this.TriggerBox.BorderRadiusBottomRight = 0;
            this.TriggerBox.BorderRadiusTopLeft = 0;
            this.TriggerBox.BorderRadiusTopRight = 0;
            this.TriggerBox.BorderWidth = 0.5F;
            this.TriggerBox.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.TriggerBox.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.TriggerBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TriggerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TriggerBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.TriggerBox.Location = new System.Drawing.Point(0, 38);
            this.TriggerBox.Margin = new System.Windows.Forms.Padding(0, 38, 0, 0);
            this.TriggerBox.Name = "TriggerBox";
            this.tableLayoutPanelMain.SetRowSpan(this.TriggerBox, 5);
            this.TriggerBox.Size = new System.Drawing.Size(60, 160);
            this.TriggerBox.TabIndex = 95;
            this.TriggerBox.UseMnemonic = false;
            this.TriggerBox.UseVisualStyleBackColor = false;
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
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNext.BorderColor = System.Drawing.Color.Black;
            this.buttonNext.BorderRadiusBottomLeft = 0;
            this.buttonNext.BorderRadiusBottomRight = 0;
            this.buttonNext.BorderRadiusTopLeft = 0;
            this.buttonNext.BorderRadiusTopRight = 0;
            this.buttonNext.BorderWidth = 2F;
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonNext, 4);
            this.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext.Font = new System.Drawing.Font("Montserrat Medium", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext.Location = new System.Drawing.Point(1710, 990);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(150, 55);
            this.buttonNext.TabIndex = 91;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseCompatibleTextRendering = true;
            this.buttonNext.UseMnemonic = false;
            this.buttonNext.UseVisualStyleBackColor = false;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExit.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonExit, 3);
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Font = new System.Drawing.Font("Montserrat Thin", 30F, System.Drawing.FontStyle.Underline);
            this.buttonExit.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit.Location = new System.Drawing.Point(60, 990);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(150, 55);
            this.buttonExit.TabIndex = 84;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseCompatibleTextRendering = true;
            this.buttonExit.UseVisualStyleBackColor = false;
            // 
            // panelSignalQualitySlider
            // 
            this.panelSignalQualitySlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSignalQualitySlider.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelSignalQualitySlider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.panelSignalQualitySlider.BackgroundImage = global::SensorUI.Properties.Resources.signalQualityGradient_1AcceptableChannel;
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
            this.labelBCISignalCheckDescription.Location = new System.Drawing.Point(65, 185);
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
            this.tabControlSignalQuality.Controls.Add(this.tabPageImpedance);
            this.tabControlSignalQuality.Controls.Add(this.tabPageQuality);
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
            this.tableLayoutRailingTest.Controls.Add(this.labelOptionalRailingTest, 8, 9);
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTestInfo2, 0, 8);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp8, 10, 17);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp7, 10, 16);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp6, 10, 15);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp5, 10, 14);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp4, 10, 13);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp3, 10, 12);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp2, 10, 11);
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTestInfo1, 0, 5);
            this.tableLayoutRailingTest.Controls.Add(this.labelRailingTest, 0, 0);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR8, 8, 8);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR8, 10, 8);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR7, 10, 7);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR6, 8, 6);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR6, 10, 6);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR5, 8, 5);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR5, 10, 5);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR1, 8, 1);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR1, 10, 1);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR2, 8, 2);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR3, 8, 3);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR3, 10, 3);
            this.tableLayoutRailingTest.Controls.Add(this.labelElectrodeRailingRailingTest, 17, 0);
            this.tableLayoutRailingTest.Controls.Add(this.labelRequiredRailingTest, 8, 0);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR4, 8, 4);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR4, 10, 4);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestR2, 10, 2);
            this.tableLayoutRailingTest.Controls.Add(this.label66, 9, 1);
            this.tableLayoutRailingTest.Controls.Add(this.label67, 9, 2);
            this.tableLayoutRailingTest.Controls.Add(this.label68, 9, 3);
            this.tableLayoutRailingTest.Controls.Add(this.label69, 9, 4);
            this.tableLayoutRailingTest.Controls.Add(this.label70, 9, 5);
            this.tableLayoutRailingTest.Controls.Add(this.label71, 9, 6);
            this.tableLayoutRailingTest.Controls.Add(this.label72, 9, 7);
            this.tableLayoutRailingTest.Controls.Add(this.label73, 9, 8);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp1, 8, 10);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp2, 8, 11);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp3, 8, 12);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp4, 8, 13);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp6, 8, 15);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp5, 8, 14);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp7, 8, 16);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestOp8, 8, 17);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest1, 9, 10);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest2, 9, 11);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest4, 9, 13);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest3, 9, 12);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest5, 9, 14);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest6, 9, 15);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest7, 9, 16);
            this.tableLayoutRailingTest.Controls.Add(this.labelEqualsIRailingTest8, 9, 17);
            this.tableLayoutRailingTest.Controls.Add(this.chartRailingTestOp1, 10, 10);
            this.tableLayoutRailingTest.Controls.Add(this.btnElectrodeRailingTestR7, 8, 7);
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
            // labelOptionalRailingTest
            // 
            this.tableLayoutRailingTest.SetColumnSpan(this.labelOptionalRailingTest, 3);
            this.labelOptionalRailingTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelOptionalRailingTest.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold);
            this.labelOptionalRailingTest.ForeColor = System.Drawing.Color.Gray;
            this.labelOptionalRailingTest.Location = new System.Drawing.Point(450, 396);
            this.labelOptionalRailingTest.Margin = new System.Windows.Forms.Padding(0);
            this.labelOptionalRailingTest.Name = "labelOptionalRailingTest";
            this.labelOptionalRailingTest.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.labelOptionalRailingTest.Size = new System.Drawing.Size(159, 44);
            this.labelOptionalRailingTest.TabIndex = 147;
            this.labelOptionalRailingTest.Text = "Optional";
            this.labelOptionalRailingTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // chartRailingTestOp8
            // 
            this.chartRailingTestOp8.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp8.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp8.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp8.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp8.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp8.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea161.AxisX.IsLabelAutoFit = false;
            chartArea161.AxisX.IsMarginVisible = false;
            chartArea161.AxisX.LabelStyle.Enabled = false;
            chartArea161.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea161.AxisX.MajorGrid.Enabled = false;
            chartArea161.AxisX.MajorTickMark.Enabled = false;
            chartArea161.AxisX.Maximum = 1250D;
            chartArea161.AxisX.Minimum = 0D;
            chartArea161.AxisX.ScaleView.Zoomable = false;
            chartArea161.AxisX.ScrollBar.Enabled = false;
            chartArea161.AxisY.IsLabelAutoFit = false;
            chartArea161.AxisY.LabelStyle.Enabled = false;
            chartArea161.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea161.AxisY.MajorGrid.Enabled = false;
            chartArea161.AxisY.MajorTickMark.Enabled = false;
            chartArea161.BackColor = System.Drawing.Color.Transparent;
            chartArea161.BorderWidth = 0;
            chartArea161.Name = "chartAreaOp8";
            this.chartRailingTestOp8.ChartAreas.Add(chartArea161);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp8, 12);
            this.chartRailingTestOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp8.Enabled = false;
            this.chartRailingTestOp8.IsSoftShadows = false;
            legend161.Enabled = false;
            legend161.Name = "LegenOp7";
            this.chartRailingTestOp8.Legends.Add(legend161);
            this.chartRailingTestOp8.Location = new System.Drawing.Point(556, 748);
            this.chartRailingTestOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp8.Name = "chartRailingTestOp8";
            series161.ChartArea = "chartAreaOp8";
            series161.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series161.Legend = "LegenOp7";
            series161.Name = "Series1";
            this.chartRailingTestOp8.Series.Add(series161);
            this.chartRailingTestOp8.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestOp8.TabIndex = 145;
            this.chartRailingTestOp8.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title161.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title161.BackColor = System.Drawing.Color.Transparent;
            title161.DockedToChartArea = "chartAreaOp8";
            title161.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title161.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title161.ForeColor = System.Drawing.Color.Gray;
            title161.IsDockedInsideChartArea = false;
            title161.Name = "railingResRailingTestOp8";
            title161.Text = "railOp8";
            title161.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp8.Titles.Add(title161);
            // 
            // chartRailingTestOp7
            // 
            this.chartRailingTestOp7.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp7.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp7.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp7.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp7.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp7.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea162.AxisX.IsLabelAutoFit = false;
            chartArea162.AxisX.IsMarginVisible = false;
            chartArea162.AxisX.LabelStyle.Enabled = false;
            chartArea162.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea162.AxisX.MajorGrid.Enabled = false;
            chartArea162.AxisX.MajorTickMark.Enabled = false;
            chartArea162.AxisX.Maximum = 1250D;
            chartArea162.AxisX.Minimum = 0D;
            chartArea162.AxisX.ScaleView.Zoomable = false;
            chartArea162.AxisX.ScrollBar.Enabled = false;
            chartArea162.AxisY.IsLabelAutoFit = false;
            chartArea162.AxisY.LabelStyle.Enabled = false;
            chartArea162.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea162.AxisY.MajorGrid.Enabled = false;
            chartArea162.AxisY.MajorTickMark.Enabled = false;
            chartArea162.BackColor = System.Drawing.Color.Transparent;
            chartArea162.BorderWidth = 0;
            chartArea162.Name = "chartAreaOp7";
            this.chartRailingTestOp7.ChartAreas.Add(chartArea162);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp7, 12);
            this.chartRailingTestOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp7.Enabled = false;
            this.chartRailingTestOp7.IsSoftShadows = false;
            legend162.Enabled = false;
            legend162.Name = "LegenOp7";
            this.chartRailingTestOp7.Legends.Add(legend162);
            this.chartRailingTestOp7.Location = new System.Drawing.Point(556, 703);
            this.chartRailingTestOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp7.Name = "chartRailingTestOp7";
            series162.ChartArea = "chartAreaOp7";
            series162.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series162.Legend = "LegenOp7";
            series162.Name = "Series1";
            this.chartRailingTestOp7.Series.Add(series162);
            this.chartRailingTestOp7.Size = new System.Drawing.Size(636, 43);
            this.chartRailingTestOp7.TabIndex = 144;
            this.chartRailingTestOp7.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title162.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title162.BackColor = System.Drawing.Color.Transparent;
            title162.DockedToChartArea = "chartAreaOp7";
            title162.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title162.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title162.ForeColor = System.Drawing.Color.Gray;
            title162.IsDockedInsideChartArea = false;
            title162.Name = "railingResRailingTestOp7";
            title162.Text = "railOp7";
            title162.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp7.Titles.Add(title162);
            // 
            // chartRailingTestOp6
            // 
            this.chartRailingTestOp6.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp6.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp6.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp6.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp6.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp6.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea163.AxisX.IsLabelAutoFit = false;
            chartArea163.AxisX.IsMarginVisible = false;
            chartArea163.AxisX.LabelStyle.Enabled = false;
            chartArea163.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea163.AxisX.MajorGrid.Enabled = false;
            chartArea163.AxisX.MajorTickMark.Enabled = false;
            chartArea163.AxisX.Maximum = 1250D;
            chartArea163.AxisX.Minimum = 0D;
            chartArea163.AxisX.ScaleView.Zoomable = false;
            chartArea163.AxisX.ScrollBar.Enabled = false;
            chartArea163.AxisY.IsLabelAutoFit = false;
            chartArea163.AxisY.LabelStyle.Enabled = false;
            chartArea163.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea163.AxisY.MajorGrid.Enabled = false;
            chartArea163.AxisY.MajorTickMark.Enabled = false;
            chartArea163.BackColor = System.Drawing.Color.Transparent;
            chartArea163.BorderWidth = 0;
            chartArea163.Name = "chartAreaOp6";
            this.chartRailingTestOp6.ChartAreas.Add(chartArea163);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp6, 12);
            this.chartRailingTestOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp6.Enabled = false;
            this.chartRailingTestOp6.IsSoftShadows = false;
            legend163.Enabled = false;
            legend163.Name = "LegenOp7";
            this.chartRailingTestOp6.Legends.Add(legend163);
            this.chartRailingTestOp6.Location = new System.Drawing.Point(556, 660);
            this.chartRailingTestOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp6.Name = "chartRailingTestOp6";
            series163.ChartArea = "chartAreaOp6";
            series163.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series163.Legend = "LegenOp7";
            series163.Name = "Series1";
            this.chartRailingTestOp6.Series.Add(series163);
            this.chartRailingTestOp6.Size = new System.Drawing.Size(636, 41);
            this.chartRailingTestOp6.TabIndex = 143;
            this.chartRailingTestOp6.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title163.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title163.BackColor = System.Drawing.Color.Transparent;
            title163.DockedToChartArea = "chartAreaOp6";
            title163.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title163.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold);
            title163.ForeColor = System.Drawing.Color.Gray;
            title163.IsDockedInsideChartArea = false;
            title163.Name = "railingResRailingTestOp6";
            title163.Text = "railOp6";
            title163.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp6.Titles.Add(title163);
            // 
            // chartRailingTestOp5
            // 
            this.chartRailingTestOp5.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp5.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp5.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp5.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp5.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp5.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea164.AxisX.IsLabelAutoFit = false;
            chartArea164.AxisX.IsMarginVisible = false;
            chartArea164.AxisX.LabelStyle.Enabled = false;
            chartArea164.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea164.AxisX.MajorGrid.Enabled = false;
            chartArea164.AxisX.MajorTickMark.Enabled = false;
            chartArea164.AxisX.Maximum = 1250D;
            chartArea164.AxisX.Minimum = 0D;
            chartArea164.AxisX.ScaleView.Zoomable = false;
            chartArea164.AxisX.ScrollBar.Enabled = false;
            chartArea164.AxisY.IsLabelAutoFit = false;
            chartArea164.AxisY.LabelStyle.Enabled = false;
            chartArea164.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea164.AxisY.MajorGrid.Enabled = false;
            chartArea164.AxisY.MajorTickMark.Enabled = false;
            chartArea164.BackColor = System.Drawing.Color.Transparent;
            chartArea164.BorderWidth = 0;
            chartArea164.Name = "chartAreaOp5";
            this.chartRailingTestOp5.ChartAreas.Add(chartArea164);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp5, 12);
            this.chartRailingTestOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp5.Enabled = false;
            this.chartRailingTestOp5.IsSoftShadows = false;
            legend164.Enabled = false;
            legend164.Name = "LegenOp7";
            this.chartRailingTestOp5.Legends.Add(legend164);
            this.chartRailingTestOp5.Location = new System.Drawing.Point(556, 616);
            this.chartRailingTestOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp5.Name = "chartRailingTestOp5";
            series164.ChartArea = "chartAreaOp5";
            series164.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series164.Legend = "LegenOp7";
            series164.Name = "Series1";
            this.chartRailingTestOp5.Series.Add(series164);
            this.chartRailingTestOp5.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestOp5.TabIndex = 142;
            this.chartRailingTestOp5.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title164.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title164.BackColor = System.Drawing.Color.Transparent;
            title164.DockedToChartArea = "chartAreaOp5";
            title164.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title164.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold);
            title164.ForeColor = System.Drawing.Color.Gray;
            title164.IsDockedInsideChartArea = false;
            title164.Name = "railingResRailingTestOp5";
            title164.Text = "railOp5";
            title164.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp5.Titles.Add(title164);
            // 
            // chartRailingTestOp4
            // 
            this.chartRailingTestOp4.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp4.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp4.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp4.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp4.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp4.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea165.AxisX.IsLabelAutoFit = false;
            chartArea165.AxisX.IsMarginVisible = false;
            chartArea165.AxisX.LabelStyle.Enabled = false;
            chartArea165.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea165.AxisX.MajorGrid.Enabled = false;
            chartArea165.AxisX.MajorTickMark.Enabled = false;
            chartArea165.AxisX.Maximum = 1250D;
            chartArea165.AxisX.Minimum = 0D;
            chartArea165.AxisX.ScaleView.Zoomable = false;
            chartArea165.AxisX.ScrollBar.Enabled = false;
            chartArea165.AxisY.IsLabelAutoFit = false;
            chartArea165.AxisY.LabelStyle.Enabled = false;
            chartArea165.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea165.AxisY.MajorGrid.Enabled = false;
            chartArea165.AxisY.MajorTickMark.Enabled = false;
            chartArea165.BackColor = System.Drawing.Color.Transparent;
            chartArea165.BorderWidth = 0;
            chartArea165.Name = "chartAreaOp4";
            this.chartRailingTestOp4.ChartAreas.Add(chartArea165);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp4, 12);
            this.chartRailingTestOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp4.Enabled = false;
            this.chartRailingTestOp4.IsSoftShadows = false;
            legend165.Enabled = false;
            legend165.Name = "LegenOp7";
            this.chartRailingTestOp4.Legends.Add(legend165);
            this.chartRailingTestOp4.Location = new System.Drawing.Point(556, 572);
            this.chartRailingTestOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp4.Name = "chartRailingTestOp4";
            series165.ChartArea = "chartAreaOp4";
            series165.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series165.Legend = "LegenOp7";
            series165.Name = "Series1";
            this.chartRailingTestOp4.Series.Add(series165);
            this.chartRailingTestOp4.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestOp4.TabIndex = 141;
            this.chartRailingTestOp4.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title165.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title165.BackColor = System.Drawing.Color.Transparent;
            title165.DockedToChartArea = "chartAreaOp4";
            title165.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title165.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold);
            title165.ForeColor = System.Drawing.Color.Gray;
            title165.IsDockedInsideChartArea = false;
            title165.Name = "railingResRailingTestOp4";
            title165.Text = "railOp4";
            title165.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp4.Titles.Add(title165);
            // 
            // chartRailingTestOp3
            // 
            this.chartRailingTestOp3.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp3.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp3.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp3.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp3.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp3.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea166.AxisX.IsLabelAutoFit = false;
            chartArea166.AxisX.IsMarginVisible = false;
            chartArea166.AxisX.LabelStyle.Enabled = false;
            chartArea166.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea166.AxisX.MajorGrid.Enabled = false;
            chartArea166.AxisX.MajorTickMark.Enabled = false;
            chartArea166.AxisX.Maximum = 1250D;
            chartArea166.AxisX.Minimum = 0D;
            chartArea166.AxisX.ScaleView.Zoomable = false;
            chartArea166.AxisX.ScrollBar.Enabled = false;
            chartArea166.AxisY.IsLabelAutoFit = false;
            chartArea166.AxisY.LabelStyle.Enabled = false;
            chartArea166.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea166.AxisY.MajorGrid.Enabled = false;
            chartArea166.AxisY.MajorTickMark.Enabled = false;
            chartArea166.BackColor = System.Drawing.Color.Transparent;
            chartArea166.BorderWidth = 0;
            chartArea166.Name = "chartAreaOp3";
            this.chartRailingTestOp3.ChartAreas.Add(chartArea166);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp3, 12);
            this.chartRailingTestOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp3.Enabled = false;
            this.chartRailingTestOp3.IsSoftShadows = false;
            legend166.Enabled = false;
            legend166.Name = "LegenOp7";
            this.chartRailingTestOp3.Legends.Add(legend166);
            this.chartRailingTestOp3.Location = new System.Drawing.Point(556, 528);
            this.chartRailingTestOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp3.Name = "chartRailingTestOp3";
            series166.ChartArea = "chartAreaOp3";
            series166.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series166.Legend = "LegenOp7";
            series166.Name = "Series1";
            this.chartRailingTestOp3.Series.Add(series166);
            this.chartRailingTestOp3.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestOp3.TabIndex = 140;
            this.chartRailingTestOp3.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title166.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title166.BackColor = System.Drawing.Color.Transparent;
            title166.DockedToChartArea = "chartAreaOp3";
            title166.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title166.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold);
            title166.ForeColor = System.Drawing.Color.Gray;
            title166.IsDockedInsideChartArea = false;
            title166.Name = "railingResRailingTestOp3";
            title166.Text = "railOp3";
            title166.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp3.Titles.Add(title166);
            // 
            // chartRailingTestOp2
            // 
            this.chartRailingTestOp2.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp2.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp2.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp2.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp2.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp2.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea167.AxisX.IsLabelAutoFit = false;
            chartArea167.AxisX.IsMarginVisible = false;
            chartArea167.AxisX.LabelStyle.Enabled = false;
            chartArea167.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea167.AxisX.MajorGrid.Enabled = false;
            chartArea167.AxisX.MajorTickMark.Enabled = false;
            chartArea167.AxisX.Maximum = 1250D;
            chartArea167.AxisX.Minimum = 0D;
            chartArea167.AxisX.ScaleView.Zoomable = false;
            chartArea167.AxisX.ScrollBar.Enabled = false;
            chartArea167.AxisY.IsLabelAutoFit = false;
            chartArea167.AxisY.LabelStyle.Enabled = false;
            chartArea167.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea167.AxisY.MajorGrid.Enabled = false;
            chartArea167.AxisY.MajorTickMark.Enabled = false;
            chartArea167.BackColor = System.Drawing.Color.Transparent;
            chartArea167.BorderWidth = 0;
            chartArea167.Name = "chartAreaOp2";
            this.chartRailingTestOp2.ChartAreas.Add(chartArea167);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp2, 12);
            this.chartRailingTestOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp2.Enabled = false;
            this.chartRailingTestOp2.IsSoftShadows = false;
            legend167.Enabled = false;
            legend167.Name = "LegenOp7";
            this.chartRailingTestOp2.Legends.Add(legend167);
            this.chartRailingTestOp2.Location = new System.Drawing.Point(556, 484);
            this.chartRailingTestOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp2.Name = "chartRailingTestOp2";
            series167.ChartArea = "chartAreaOp2";
            series167.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series167.Legend = "LegenOp7";
            series167.Name = "Series1";
            this.chartRailingTestOp2.Series.Add(series167);
            this.chartRailingTestOp2.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestOp2.TabIndex = 139;
            this.chartRailingTestOp2.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title167.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title167.BackColor = System.Drawing.Color.Transparent;
            title167.DockedToChartArea = "chartAreaOp2";
            title167.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title167.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold);
            title167.ForeColor = System.Drawing.Color.Gray;
            title167.IsDockedInsideChartArea = false;
            title167.Name = "railingResRailingTestOp2";
            title167.Text = "railOp2";
            title167.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp2.Titles.Add(title167);
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
            // btnElectrodeRailingTestR8
            // 
            this.btnElectrodeRailingTestR8.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR8.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR8.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR8.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR8.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR8.BorderWidth = 2F;
            this.btnElectrodeRailingTestR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR8.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR8.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR8.Location = new System.Drawing.Point(450, 352);
            this.btnElectrodeRailingTestR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR8.Name = "btnElectrodeRailingTestR8";
            this.btnElectrodeRailingTestR8.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestR8.TabIndex = 33;
            this.btnElectrodeRailingTestR8.Text = "R8";
            this.btnElectrodeRailingTestR8.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR8.UseMnemonic = false;
            this.btnElectrodeRailingTestR8.UseVisualStyleBackColor = false;
            // 
            // chartRailingTestR8
            // 
            this.chartRailingTestR8.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR8.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR8.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR8.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR8.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea168.AxisX.IsLabelAutoFit = false;
            chartArea168.AxisX.IsMarginVisible = false;
            chartArea168.AxisX.LabelStyle.Enabled = false;
            chartArea168.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea168.AxisX.MajorGrid.Enabled = false;
            chartArea168.AxisX.MajorTickMark.Enabled = false;
            chartArea168.AxisX.Maximum = 1250D;
            chartArea168.AxisX.Minimum = 0D;
            chartArea168.AxisX.ScaleView.Zoomable = false;
            chartArea168.AxisX.ScrollBar.Enabled = false;
            chartArea168.AxisY.IsLabelAutoFit = false;
            chartArea168.AxisY.LabelStyle.Enabled = false;
            chartArea168.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea168.AxisY.MajorGrid.Enabled = false;
            chartArea168.AxisY.MajorTickMark.Enabled = false;
            chartArea168.BackColor = System.Drawing.Color.Transparent;
            chartArea168.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea168.BorderWidth = 0;
            chartArea168.Name = "chartAreaR8";
            this.chartRailingTestR8.ChartAreas.Add(chartArea168);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR8, 12);
            this.chartRailingTestR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR8.IsSoftShadows = false;
            legend168.Enabled = false;
            legend168.Name = "LegenOp3";
            this.chartRailingTestR8.Legends.Add(legend168);
            this.chartRailingTestR8.Location = new System.Drawing.Point(556, 352);
            this.chartRailingTestR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR8.Name = "chartRailingTestR8";
            series168.ChartArea = "chartAreaR8";
            series168.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series168.Legend = "LegenOp3";
            series168.Name = "Series1";
            this.chartRailingTestR8.Series.Add(series168);
            this.chartRailingTestR8.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR8.TabIndex = 35;
            this.chartRailingTestR8.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title168.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title168.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title168.DockedToChartArea = "chartAreaR8";
            title168.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title168.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title168.ForeColor = System.Drawing.Color.White;
            title168.IsDockedInsideChartArea = false;
            title168.Name = "railingResRailingTestR8";
            title168.Text = "railR8";
            title168.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR8.Titles.Add(title168);
            // 
            // chartRailingTestR7
            // 
            this.chartRailingTestR7.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR7.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR7.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR7.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR7.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea169.AxisX.IsLabelAutoFit = false;
            chartArea169.AxisX.IsMarginVisible = false;
            chartArea169.AxisX.LabelStyle.Enabled = false;
            chartArea169.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea169.AxisX.MajorGrid.Enabled = false;
            chartArea169.AxisX.MajorTickMark.Enabled = false;
            chartArea169.AxisX.Maximum = 1250D;
            chartArea169.AxisX.Minimum = 0D;
            chartArea169.AxisX.ScaleView.Zoomable = false;
            chartArea169.AxisX.ScrollBar.Enabled = false;
            chartArea169.AxisY.IsLabelAutoFit = false;
            chartArea169.AxisY.LabelStyle.Enabled = false;
            chartArea169.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea169.AxisY.MajorGrid.Enabled = false;
            chartArea169.AxisY.MajorTickMark.Enabled = false;
            chartArea169.BackColor = System.Drawing.Color.Transparent;
            chartArea169.BorderWidth = 0;
            chartArea169.Name = "chartAreaR7";
            this.chartRailingTestR7.ChartAreas.Add(chartArea169);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR7, 12);
            this.chartRailingTestR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR7.IsSoftShadows = false;
            legend169.Enabled = false;
            legend169.Name = "LegenOp7";
            this.chartRailingTestR7.Legends.Add(legend169);
            this.chartRailingTestR7.Location = new System.Drawing.Point(556, 308);
            this.chartRailingTestR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR7.Name = "chartRailingTestR7";
            series169.ChartArea = "chartAreaR7";
            series169.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series169.Legend = "LegenOp7";
            series169.Name = "Series1";
            this.chartRailingTestR7.Series.Add(series169);
            this.chartRailingTestR7.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR7.TabIndex = 32;
            this.chartRailingTestR7.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title169.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title169.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title169.DockedToChartArea = "chartAreaR7";
            title169.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title169.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title169.ForeColor = System.Drawing.Color.White;
            title169.IsDockedInsideChartArea = false;
            title169.Name = "railingResRailingTestR7";
            title169.Text = "railR7";
            title169.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR7.Titles.Add(title169);
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
            this.btnElectrodeRailingTestR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR6.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR6.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR6.Location = new System.Drawing.Point(450, 264);
            this.btnElectrodeRailingTestR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR6.Name = "btnElectrodeRailingTestR6";
            this.btnElectrodeRailingTestR6.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestR6.TabIndex = 27;
            this.btnElectrodeRailingTestR6.Text = "R6";
            this.btnElectrodeRailingTestR6.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR6.UseMnemonic = false;
            this.btnElectrodeRailingTestR6.UseVisualStyleBackColor = false;
            // 
            // chartRailingTestR6
            // 
            this.chartRailingTestR6.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR6.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR6.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR6.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR6.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea170.AxisX.IsLabelAutoFit = false;
            chartArea170.AxisX.IsMarginVisible = false;
            chartArea170.AxisX.LabelStyle.Enabled = false;
            chartArea170.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea170.AxisX.MajorGrid.Enabled = false;
            chartArea170.AxisX.MajorTickMark.Enabled = false;
            chartArea170.AxisX.Maximum = 1250D;
            chartArea170.AxisX.Minimum = 0D;
            chartArea170.AxisX.ScaleView.Zoomable = false;
            chartArea170.AxisX.ScrollBar.Enabled = false;
            chartArea170.AxisY.IsLabelAutoFit = false;
            chartArea170.AxisY.LabelStyle.Enabled = false;
            chartArea170.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea170.AxisY.MajorGrid.Enabled = false;
            chartArea170.AxisY.MajorTickMark.Enabled = false;
            chartArea170.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea170.BorderWidth = 0;
            chartArea170.Name = "chartAreaR6";
            this.chartRailingTestR6.ChartAreas.Add(chartArea170);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR6, 12);
            this.chartRailingTestR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR6.IsSoftShadows = false;
            legend170.Enabled = false;
            legend170.Name = "LegenOp5";
            this.chartRailingTestR6.Legends.Add(legend170);
            this.chartRailingTestR6.Location = new System.Drawing.Point(556, 264);
            this.chartRailingTestR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR6.Name = "chartRailingTestR6";
            series170.ChartArea = "chartAreaR6";
            series170.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series170.Legend = "LegenOp5";
            series170.Name = "Series1";
            this.chartRailingTestR6.Series.Add(series170);
            this.chartRailingTestR6.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR6.TabIndex = 29;
            this.chartRailingTestR6.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title170.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title170.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title170.DockedToChartArea = "chartAreaR6";
            title170.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title170.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title170.ForeColor = System.Drawing.Color.White;
            title170.IsDockedInsideChartArea = false;
            title170.Name = "railingResRailingTestR6";
            title170.Text = "railR6";
            title170.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR6.Titles.Add(title170);
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
            this.btnElectrodeRailingTestR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR5.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR5.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR5.Location = new System.Drawing.Point(450, 220);
            this.btnElectrodeRailingTestR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR5.Name = "btnElectrodeRailingTestR5";
            this.btnElectrodeRailingTestR5.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestR5.TabIndex = 24;
            this.btnElectrodeRailingTestR5.Text = "R5";
            this.btnElectrodeRailingTestR5.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR5.UseMnemonic = false;
            this.btnElectrodeRailingTestR5.UseVisualStyleBackColor = false;
            // 
            // chartRailingTestR5
            // 
            this.chartRailingTestR5.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR5.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR5.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR5.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR5.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea171.AxisX.IsMarginVisible = false;
            chartArea171.AxisX.LabelStyle.Enabled = false;
            chartArea171.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea171.AxisX.MajorGrid.Enabled = false;
            chartArea171.AxisX.MajorTickMark.Enabled = false;
            chartArea171.AxisX.Maximum = 1250D;
            chartArea171.AxisX.Minimum = 0D;
            chartArea171.AxisX.ScaleView.Zoomable = false;
            chartArea171.AxisX.ScrollBar.Enabled = false;
            chartArea171.AxisY.IsLabelAutoFit = false;
            chartArea171.AxisY.LabelStyle.Enabled = false;
            chartArea171.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea171.AxisY.MajorGrid.Enabled = false;
            chartArea171.AxisY.MajorTickMark.Enabled = false;
            chartArea171.BackColor = System.Drawing.Color.Transparent;
            chartArea171.Name = "chartAreaR5";
            this.chartRailingTestR5.ChartAreas.Add(chartArea171);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR5, 12);
            this.chartRailingTestR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR5.IsSoftShadows = false;
            legend171.Enabled = false;
            legend171.Name = "LegenOp4";
            this.chartRailingTestR5.Legends.Add(legend171);
            this.chartRailingTestR5.Location = new System.Drawing.Point(556, 220);
            this.chartRailingTestR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR5.Name = "chartRailingTestR5";
            series171.ChartArea = "chartAreaR5";
            series171.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series171.Legend = "LegenOp4";
            series171.Name = "Series1";
            this.chartRailingTestR5.Series.Add(series171);
            this.chartRailingTestR5.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR5.TabIndex = 26;
            this.chartRailingTestR5.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title171.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title171.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title171.DockedToChartArea = "chartAreaR5";
            title171.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title171.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title171.ForeColor = System.Drawing.Color.White;
            title171.IsDockedInsideChartArea = false;
            title171.Name = "railingResRailingTestR5";
            title171.Text = "railR5";
            title171.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR5.Titles.Add(title171);
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
            this.btnElectrodeRailingTestR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR1.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR1.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR1.Location = new System.Drawing.Point(450, 44);
            this.btnElectrodeRailingTestR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR1.Name = "btnElectrodeRailingTestR1";
            this.btnElectrodeRailingTestR1.Size = new System.Drawing.Size(53, 42);
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
            chartArea172.AxisX.IsLabelAutoFit = false;
            chartArea172.AxisX.IsMarginVisible = false;
            chartArea172.AxisX.LabelStyle.Enabled = false;
            chartArea172.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea172.AxisX.MajorGrid.Enabled = false;
            chartArea172.AxisX.MajorTickMark.Enabled = false;
            chartArea172.AxisX.Maximum = 1250D;
            chartArea172.AxisX.Minimum = 0D;
            chartArea172.AxisX.ScaleView.Zoomable = false;
            chartArea172.AxisX.ScrollBar.Enabled = false;
            chartArea172.AxisY.IsLabelAutoFit = false;
            chartArea172.AxisY.LabelStyle.Enabled = false;
            chartArea172.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea172.AxisY.MajorGrid.Enabled = false;
            chartArea172.AxisY.MajorTickMark.Enabled = false;
            chartArea172.BackColor = System.Drawing.Color.Transparent;
            chartArea172.BorderWidth = 0;
            chartArea172.Name = "chartAreaR1";
            this.chartRailingTestR1.ChartAreas.Add(chartArea172);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR1, 12);
            this.chartRailingTestR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR1.IsSoftShadows = false;
            legend172.Enabled = false;
            legend172.Name = "LegenOp7";
            this.chartRailingTestR1.Legends.Add(legend172);
            this.chartRailingTestR1.Location = new System.Drawing.Point(556, 44);
            this.chartRailingTestR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR1.Name = "chartRailingTestR1";
            series172.ChartArea = "chartAreaR1";
            series172.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series172.Legend = "LegenOp7";
            series172.Name = "Series1";
            this.chartRailingTestR1.Series.Add(series172);
            this.chartRailingTestR1.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR1.TabIndex = 32;
            this.chartRailingTestR1.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title172.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title172.BackColor = System.Drawing.Color.Transparent;
            title172.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Right;
            title172.BackSecondaryColor = System.Drawing.Color.Transparent;
            title172.BorderColor = System.Drawing.Color.Transparent;
            title172.DockedToChartArea = "chartAreaR1";
            title172.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title172.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title172.ForeColor = System.Drawing.Color.White;
            title172.IsDockedInsideChartArea = false;
            title172.Name = "railingResRailingTestR1";
            title172.Text = "railR1";
            title172.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR1.Titles.Add(title172);
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
            this.btnElectrodeRailingTestR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR2.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR2.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR2.Location = new System.Drawing.Point(450, 88);
            this.btnElectrodeRailingTestR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR2.Name = "btnElectrodeRailingTestR2";
            this.btnElectrodeRailingTestR2.Size = new System.Drawing.Size(53, 42);
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
            this.btnElectrodeRailingTestR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR3.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR3.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR3.Location = new System.Drawing.Point(450, 132);
            this.btnElectrodeRailingTestR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR3.Name = "btnElectrodeRailingTestR3";
            this.btnElectrodeRailingTestR3.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestR3.TabIndex = 11;
            this.btnElectrodeRailingTestR3.Text = "R3";
            this.btnElectrodeRailingTestR3.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR3.UseMnemonic = false;
            this.btnElectrodeRailingTestR3.UseVisualStyleBackColor = false;
            // 
            // chartRailingTestR3
            // 
            this.chartRailingTestR3.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR3.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR3.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR3.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR3.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea173.AxisX.IsLabelAutoFit = false;
            chartArea173.AxisX.IsMarginVisible = false;
            chartArea173.AxisX.LabelStyle.Enabled = false;
            chartArea173.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea173.AxisX.MajorGrid.Enabled = false;
            chartArea173.AxisX.MajorTickMark.Enabled = false;
            chartArea173.AxisX.Maximum = 1250D;
            chartArea173.AxisX.Minimum = 0D;
            chartArea173.AxisX.ScaleView.Zoomable = false;
            chartArea173.AxisX.ScrollBar.Enabled = false;
            chartArea173.AxisY.IsLabelAutoFit = false;
            chartArea173.AxisY.LabelStyle.Enabled = false;
            chartArea173.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea173.AxisY.MajorGrid.Enabled = false;
            chartArea173.AxisY.MajorTickMark.Enabled = false;
            chartArea173.BackColor = System.Drawing.Color.Transparent;
            chartArea173.Name = "chartAreaR3";
            this.chartRailingTestR3.ChartAreas.Add(chartArea173);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR3, 12);
            this.chartRailingTestR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR3.IsSoftShadows = false;
            legend173.Enabled = false;
            legend173.Name = "LegenOp3";
            this.chartRailingTestR3.Legends.Add(legend173);
            this.chartRailingTestR3.Location = new System.Drawing.Point(556, 132);
            this.chartRailingTestR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR3.Name = "chartRailingTestR3";
            series173.ChartArea = "chartAreaR3";
            series173.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series173.Legend = "LegenOp3";
            series173.Name = "Series1";
            this.chartRailingTestR3.Series.Add(series173);
            this.chartRailingTestR3.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR3.TabIndex = 10;
            this.chartRailingTestR3.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title173.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title173.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title173.DockedToChartArea = "chartAreaR3";
            title173.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title173.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title173.ForeColor = System.Drawing.Color.White;
            title173.IsDockedInsideChartArea = false;
            title173.Name = "railingResRailingTestR3";
            title173.Text = "railR3";
            title173.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR3.Titles.Add(title173);
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
            // btnElectrodeRailingTestR4
            // 
            this.btnElectrodeRailingTestR4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestR4.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestR4.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestR4.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestR4.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestR4.BorderWidth = 2F;
            this.btnElectrodeRailingTestR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR4.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR4.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR4.Location = new System.Drawing.Point(450, 176);
            this.btnElectrodeRailingTestR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR4.Name = "btnElectrodeRailingTestR4";
            this.btnElectrodeRailingTestR4.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestR4.TabIndex = 21;
            this.btnElectrodeRailingTestR4.Text = "R4";
            this.btnElectrodeRailingTestR4.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR4.UseMnemonic = false;
            this.btnElectrodeRailingTestR4.UseVisualStyleBackColor = false;
            // 
            // chartRailingTestR4
            // 
            this.chartRailingTestR4.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR4.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR4.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR4.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR4.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea174.AxisX.IsLabelAutoFit = false;
            chartArea174.AxisX.IsMarginVisible = false;
            chartArea174.AxisX.LabelStyle.Enabled = false;
            chartArea174.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea174.AxisX.MajorGrid.Enabled = false;
            chartArea174.AxisX.MajorTickMark.Enabled = false;
            chartArea174.AxisX.Maximum = 1250D;
            chartArea174.AxisX.Minimum = 0D;
            chartArea174.AxisX.ScaleView.Zoomable = false;
            chartArea174.AxisX.ScrollBar.Enabled = false;
            chartArea174.AxisY.IsLabelAutoFit = false;
            chartArea174.AxisY.LabelStyle.Enabled = false;
            chartArea174.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea174.AxisY.MajorGrid.Enabled = false;
            chartArea174.AxisY.MajorTickMark.Enabled = false;
            chartArea174.BackColor = System.Drawing.Color.Transparent;
            chartArea174.Name = "chartAreaR4";
            this.chartRailingTestR4.ChartAreas.Add(chartArea174);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR4, 12);
            this.chartRailingTestR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR4.IsSoftShadows = false;
            legend174.Enabled = false;
            legend174.Name = "LegenOp4";
            this.chartRailingTestR4.Legends.Add(legend174);
            this.chartRailingTestR4.Location = new System.Drawing.Point(556, 176);
            this.chartRailingTestR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR4.Name = "chartRailingTestR4";
            series174.ChartArea = "chartAreaR4";
            series174.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series174.Legend = "LegenOp4";
            series174.Name = "Series1";
            this.chartRailingTestR4.Series.Add(series174);
            this.chartRailingTestR4.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR4.TabIndex = 23;
            this.chartRailingTestR4.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title174.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title174.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title174.DockedToChartArea = "chartAreaR4";
            title174.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title174.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title174.ForeColor = System.Drawing.Color.White;
            title174.IsDockedInsideChartArea = false;
            title174.Name = "railingResRailingTestR4";
            title174.Text = "railR4";
            title174.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR4.Titles.Add(title174);
            // 
            // chartRailingTestR2
            // 
            this.chartRailingTestR2.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestR2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR2.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestR2.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestR2.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestR2.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea175.AxisX.IsLabelAutoFit = false;
            chartArea175.AxisX.IsMarginVisible = false;
            chartArea175.AxisX.LabelStyle.Enabled = false;
            chartArea175.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea175.AxisX.MajorGrid.Enabled = false;
            chartArea175.AxisX.MajorTickMark.Enabled = false;
            chartArea175.AxisX.Maximum = 1250D;
            chartArea175.AxisX.Minimum = 0D;
            chartArea175.AxisX.ScaleView.Zoomable = false;
            chartArea175.AxisX.ScrollBar.Enabled = false;
            chartArea175.AxisY.IsLabelAutoFit = false;
            chartArea175.AxisY.LabelStyle.Enabled = false;
            chartArea175.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea175.AxisY.MajorGrid.Enabled = false;
            chartArea175.AxisY.MajorTickMark.Enabled = false;
            chartArea175.BackColor = System.Drawing.Color.Transparent;
            chartArea175.Name = "chartAreaR2";
            this.chartRailingTestR2.ChartAreas.Add(chartArea175);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestR2, 12);
            this.chartRailingTestR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestR2.IsSoftShadows = false;
            legend175.Enabled = false;
            legend175.Name = "LegenOp8";
            this.chartRailingTestR2.Legends.Add(legend175);
            this.chartRailingTestR2.Location = new System.Drawing.Point(556, 88);
            this.chartRailingTestR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestR2.Name = "chartRailingTestR2";
            series175.ChartArea = "chartAreaR2";
            series175.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series175.Legend = "LegenOp8";
            series175.Name = "Series1";
            this.chartRailingTestR2.Series.Add(series175);
            this.chartRailingTestR2.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestR2.TabIndex = 6;
            this.chartRailingTestR2.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title175.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title175.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            title175.DockedToChartArea = "chartAreaR2";
            title175.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title175.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title175.ForeColor = System.Drawing.Color.White;
            title175.IsDockedInsideChartArea = false;
            title175.Name = "railingResRailingTestR2";
            title175.Text = "railR2";
            title175.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestR2.Titles.Add(title175);
            // 
            // label66
            // 
            this.label66.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label66.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label66.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label66.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label66.ForeColor = System.Drawing.Color.White;
            this.label66.Location = new System.Drawing.Point(503, 44);
            this.label66.Margin = new System.Windows.Forms.Padding(0);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(53, 44);
            this.label66.TabIndex = 114;
            this.label66.Text = "=";
            this.label66.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label66.UseCompatibleTextRendering = true;
            // 
            // label67
            // 
            this.label67.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label67.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label67.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label67.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label67.ForeColor = System.Drawing.Color.White;
            this.label67.Location = new System.Drawing.Point(503, 88);
            this.label67.Margin = new System.Windows.Forms.Padding(0);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(53, 44);
            this.label67.TabIndex = 115;
            this.label67.Text = "=";
            this.label67.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label67.UseCompatibleTextRendering = true;
            // 
            // label68
            // 
            this.label68.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label68.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label68.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label68.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label68.ForeColor = System.Drawing.Color.White;
            this.label68.Location = new System.Drawing.Point(503, 132);
            this.label68.Margin = new System.Windows.Forms.Padding(0);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(53, 44);
            this.label68.TabIndex = 116;
            this.label68.Text = "=";
            this.label68.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label68.UseCompatibleTextRendering = true;
            // 
            // label69
            // 
            this.label69.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label69.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label69.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label69.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label69.ForeColor = System.Drawing.Color.White;
            this.label69.Location = new System.Drawing.Point(503, 176);
            this.label69.Margin = new System.Windows.Forms.Padding(0);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(53, 44);
            this.label69.TabIndex = 117;
            this.label69.Text = "=";
            this.label69.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label69.UseCompatibleTextRendering = true;
            // 
            // label70
            // 
            this.label70.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label70.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label70.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label70.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label70.ForeColor = System.Drawing.Color.White;
            this.label70.Location = new System.Drawing.Point(503, 220);
            this.label70.Margin = new System.Windows.Forms.Padding(0);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(53, 44);
            this.label70.TabIndex = 118;
            this.label70.Text = "=";
            this.label70.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label70.UseCompatibleTextRendering = true;
            // 
            // label71
            // 
            this.label71.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label71.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label71.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label71.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label71.ForeColor = System.Drawing.Color.White;
            this.label71.Location = new System.Drawing.Point(503, 264);
            this.label71.Margin = new System.Windows.Forms.Padding(0);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(53, 44);
            this.label71.TabIndex = 119;
            this.label71.Text = "=";
            this.label71.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label71.UseCompatibleTextRendering = true;
            // 
            // label72
            // 
            this.label72.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label72.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label72.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label72.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label72.ForeColor = System.Drawing.Color.White;
            this.label72.Location = new System.Drawing.Point(503, 308);
            this.label72.Margin = new System.Windows.Forms.Padding(0);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(53, 44);
            this.label72.TabIndex = 120;
            this.label72.Text = "=";
            this.label72.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label72.UseCompatibleTextRendering = true;
            // 
            // label73
            // 
            this.label73.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label73.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label73.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label73.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label73.ForeColor = System.Drawing.Color.White;
            this.label73.Location = new System.Drawing.Point(503, 352);
            this.label73.Margin = new System.Windows.Forms.Padding(0);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(53, 44);
            this.label73.TabIndex = 121;
            this.label73.Text = "=";
            this.label73.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label73.UseCompatibleTextRendering = true;
            // 
            // btnElectrodeRailingTestOp1
            // 
            this.btnElectrodeRailingTestOp1.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp1.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp1.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp1.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp1.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp1.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp1.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp1.Enabled = false;
            this.btnElectrodeRailingTestOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp1.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp1.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp1.Location = new System.Drawing.Point(450, 440);
            this.btnElectrodeRailingTestOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp1.Name = "btnElectrodeRailingTestOp1";
            this.btnElectrodeRailingTestOp1.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestOp1.TabIndex = 122;
            this.btnElectrodeRailingTestOp1.Text = "Op1";
            this.btnElectrodeRailingTestOp1.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp1.UseMnemonic = false;
            this.btnElectrodeRailingTestOp1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp2
            // 
            this.btnElectrodeRailingTestOp2.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp2.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp2.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp2.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp2.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp2.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp2.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp2.Enabled = false;
            this.btnElectrodeRailingTestOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp2.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp2.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp2.Location = new System.Drawing.Point(450, 484);
            this.btnElectrodeRailingTestOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp2.Name = "btnElectrodeRailingTestOp2";
            this.btnElectrodeRailingTestOp2.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestOp2.TabIndex = 123;
            this.btnElectrodeRailingTestOp2.Text = "Op2";
            this.btnElectrodeRailingTestOp2.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp2.UseMnemonic = false;
            this.btnElectrodeRailingTestOp2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp3
            // 
            this.btnElectrodeRailingTestOp3.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp3.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp3.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp3.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp3.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp3.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp3.Enabled = false;
            this.btnElectrodeRailingTestOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp3.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp3.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp3.Location = new System.Drawing.Point(450, 528);
            this.btnElectrodeRailingTestOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp3.Name = "btnElectrodeRailingTestOp3";
            this.btnElectrodeRailingTestOp3.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestOp3.TabIndex = 124;
            this.btnElectrodeRailingTestOp3.Text = "Op3";
            this.btnElectrodeRailingTestOp3.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp3.UseMnemonic = false;
            this.btnElectrodeRailingTestOp3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp4
            // 
            this.btnElectrodeRailingTestOp4.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp4.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp4.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp4.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp4.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp4.Enabled = false;
            this.btnElectrodeRailingTestOp4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp4.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp4.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp4.Location = new System.Drawing.Point(450, 572);
            this.btnElectrodeRailingTestOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp4.Name = "btnElectrodeRailingTestOp4";
            this.btnElectrodeRailingTestOp4.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestOp4.TabIndex = 125;
            this.btnElectrodeRailingTestOp4.Text = "Op4";
            this.btnElectrodeRailingTestOp4.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp4.UseMnemonic = false;
            this.btnElectrodeRailingTestOp4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp6
            // 
            this.btnElectrodeRailingTestOp6.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp6.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp6.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp6.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp6.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp6.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp6.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp6.Enabled = false;
            this.btnElectrodeRailingTestOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp6.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp6.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp6.Location = new System.Drawing.Point(450, 660);
            this.btnElectrodeRailingTestOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp6.Name = "btnElectrodeRailingTestOp6";
            this.btnElectrodeRailingTestOp6.Size = new System.Drawing.Size(53, 41);
            this.btnElectrodeRailingTestOp6.TabIndex = 127;
            this.btnElectrodeRailingTestOp6.Text = "Op6";
            this.btnElectrodeRailingTestOp6.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp6.UseMnemonic = false;
            this.btnElectrodeRailingTestOp6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp5
            // 
            this.btnElectrodeRailingTestOp5.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp5.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp5.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp5.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp5.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp5.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp5.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp5.Enabled = false;
            this.btnElectrodeRailingTestOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp5.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp5.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp5.Location = new System.Drawing.Point(450, 616);
            this.btnElectrodeRailingTestOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp5.Name = "btnElectrodeRailingTestOp5";
            this.btnElectrodeRailingTestOp5.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestOp5.TabIndex = 126;
            this.btnElectrodeRailingTestOp5.Text = "Op5";
            this.btnElectrodeRailingTestOp5.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp5.UseMnemonic = false;
            this.btnElectrodeRailingTestOp5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp7
            // 
            this.btnElectrodeRailingTestOp7.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp7.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp7.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp7.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp7.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp7.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp7.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp7.Enabled = false;
            this.btnElectrodeRailingTestOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp7.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp7.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp7.Location = new System.Drawing.Point(450, 703);
            this.btnElectrodeRailingTestOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp7.Name = "btnElectrodeRailingTestOp7";
            this.btnElectrodeRailingTestOp7.Size = new System.Drawing.Size(53, 43);
            this.btnElectrodeRailingTestOp7.TabIndex = 128;
            this.btnElectrodeRailingTestOp7.Text = "Op7";
            this.btnElectrodeRailingTestOp7.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp7.UseMnemonic = false;
            this.btnElectrodeRailingTestOp7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeRailingTestOp8
            // 
            this.btnElectrodeRailingTestOp8.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeRailingTestOp8.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeRailingTestOp8.BorderRadiusBottomRight = 5;
            this.btnElectrodeRailingTestOp8.BorderRadiusTopLeft = 5;
            this.btnElectrodeRailingTestOp8.BorderRadiusTopRight = 5;
            this.btnElectrodeRailingTestOp8.BorderWidth = 2F;
            this.btnElectrodeRailingTestOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestOp8.Enabled = false;
            this.btnElectrodeRailingTestOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestOp8.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeRailingTestOp8.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeRailingTestOp8.Location = new System.Drawing.Point(450, 748);
            this.btnElectrodeRailingTestOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestOp8.Name = "btnElectrodeRailingTestOp8";
            this.btnElectrodeRailingTestOp8.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestOp8.TabIndex = 129;
            this.btnElectrodeRailingTestOp8.Text = "Op8";
            this.btnElectrodeRailingTestOp8.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestOp8.UseMnemonic = false;
            this.btnElectrodeRailingTestOp8.UseVisualStyleBackColor = false;
            // 
            // labelEqualsIRailingTest1
            // 
            this.labelEqualsIRailingTest1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest1.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest1.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest1.Location = new System.Drawing.Point(503, 440);
            this.labelEqualsIRailingTest1.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest1.Name = "labelEqualsIRailingTest1";
            this.labelEqualsIRailingTest1.Size = new System.Drawing.Size(53, 44);
            this.labelEqualsIRailingTest1.TabIndex = 130;
            this.labelEqualsIRailingTest1.Text = "=";
            this.labelEqualsIRailingTest1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest1.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest2
            // 
            this.labelEqualsIRailingTest2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest2.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest2.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest2.Location = new System.Drawing.Point(503, 484);
            this.labelEqualsIRailingTest2.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest2.Name = "labelEqualsIRailingTest2";
            this.labelEqualsIRailingTest2.Size = new System.Drawing.Size(53, 44);
            this.labelEqualsIRailingTest2.TabIndex = 131;
            this.labelEqualsIRailingTest2.Text = "=";
            this.labelEqualsIRailingTest2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest2.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest4
            // 
            this.labelEqualsIRailingTest4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest4.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest4.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest4.Location = new System.Drawing.Point(503, 572);
            this.labelEqualsIRailingTest4.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest4.Name = "labelEqualsIRailingTest4";
            this.labelEqualsIRailingTest4.Size = new System.Drawing.Size(53, 44);
            this.labelEqualsIRailingTest4.TabIndex = 132;
            this.labelEqualsIRailingTest4.Text = "=";
            this.labelEqualsIRailingTest4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest4.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest3
            // 
            this.labelEqualsIRailingTest3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest3.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest3.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest3.Location = new System.Drawing.Point(503, 528);
            this.labelEqualsIRailingTest3.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest3.Name = "labelEqualsIRailingTest3";
            this.labelEqualsIRailingTest3.Size = new System.Drawing.Size(53, 44);
            this.labelEqualsIRailingTest3.TabIndex = 133;
            this.labelEqualsIRailingTest3.Text = "=";
            this.labelEqualsIRailingTest3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest3.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest5
            // 
            this.labelEqualsIRailingTest5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest5.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest5.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest5.Location = new System.Drawing.Point(503, 616);
            this.labelEqualsIRailingTest5.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest5.Name = "labelEqualsIRailingTest5";
            this.labelEqualsIRailingTest5.Size = new System.Drawing.Size(53, 44);
            this.labelEqualsIRailingTest5.TabIndex = 134;
            this.labelEqualsIRailingTest5.Text = "=";
            this.labelEqualsIRailingTest5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest5.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest6
            // 
            this.labelEqualsIRailingTest6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest6.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest6.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest6.Location = new System.Drawing.Point(503, 660);
            this.labelEqualsIRailingTest6.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest6.Name = "labelEqualsIRailingTest6";
            this.labelEqualsIRailingTest6.Size = new System.Drawing.Size(53, 43);
            this.labelEqualsIRailingTest6.TabIndex = 135;
            this.labelEqualsIRailingTest6.Text = "=";
            this.labelEqualsIRailingTest6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest6.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest7
            // 
            this.labelEqualsIRailingTest7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest7.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest7.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest7.Location = new System.Drawing.Point(503, 703);
            this.labelEqualsIRailingTest7.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest7.Name = "labelEqualsIRailingTest7";
            this.labelEqualsIRailingTest7.Size = new System.Drawing.Size(53, 45);
            this.labelEqualsIRailingTest7.TabIndex = 136;
            this.labelEqualsIRailingTest7.Text = "=";
            this.labelEqualsIRailingTest7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest7.UseCompatibleTextRendering = true;
            // 
            // labelEqualsIRailingTest8
            // 
            this.labelEqualsIRailingTest8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsIRailingTest8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEqualsIRailingTest8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsIRailingTest8.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsIRailingTest8.ForeColor = System.Drawing.Color.DimGray;
            this.labelEqualsIRailingTest8.Location = new System.Drawing.Point(503, 748);
            this.labelEqualsIRailingTest8.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsIRailingTest8.Name = "labelEqualsIRailingTest8";
            this.labelEqualsIRailingTest8.Size = new System.Drawing.Size(53, 44);
            this.labelEqualsIRailingTest8.TabIndex = 137;
            this.labelEqualsIRailingTest8.Text = "=";
            this.labelEqualsIRailingTest8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsIRailingTest8.UseCompatibleTextRendering = true;
            // 
            // chartRailingTestOp1
            // 
            this.chartRailingTestOp1.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Text;
            this.chartRailingTestOp1.BackColor = System.Drawing.Color.Transparent;
            this.chartRailingTestOp1.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.chartRailingTestOp1.BorderlineColor = System.Drawing.Color.Gray;
            this.chartRailingTestOp1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartRailingTestOp1.BorderSkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            chartArea176.AxisX.IsLabelAutoFit = false;
            chartArea176.AxisX.IsMarginVisible = false;
            chartArea176.AxisX.LabelStyle.Enabled = false;
            chartArea176.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea176.AxisX.MajorGrid.Enabled = false;
            chartArea176.AxisX.MajorTickMark.Enabled = false;
            chartArea176.AxisX.Maximum = 1250D;
            chartArea176.AxisX.Minimum = 0D;
            chartArea176.AxisX.ScaleView.Zoomable = false;
            chartArea176.AxisX.ScrollBar.Enabled = false;
            chartArea176.AxisY.IsLabelAutoFit = false;
            chartArea176.AxisY.LabelStyle.Enabled = false;
            chartArea176.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea176.AxisY.MajorGrid.Enabled = false;
            chartArea176.AxisY.MajorTickMark.Enabled = false;
            chartArea176.BackColor = System.Drawing.Color.Transparent;
            chartArea176.BorderWidth = 0;
            chartArea176.Name = "chartAreaOp1";
            this.chartRailingTestOp1.ChartAreas.Add(chartArea176);
            this.tableLayoutRailingTest.SetColumnSpan(this.chartRailingTestOp1, 12);
            this.chartRailingTestOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartRailingTestOp1.Enabled = false;
            this.chartRailingTestOp1.IsSoftShadows = false;
            legend176.Enabled = false;
            legend176.Name = "LegenOp7";
            this.chartRailingTestOp1.Legends.Add(legend176);
            this.chartRailingTestOp1.Location = new System.Drawing.Point(556, 440);
            this.chartRailingTestOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.chartRailingTestOp1.Name = "chartRailingTestOp1";
            series176.ChartArea = "chartAreaOp1";
            series176.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series176.Legend = "LegenOp7";
            series176.Name = "Series1";
            this.chartRailingTestOp1.Series.Add(series176);
            this.chartRailingTestOp1.Size = new System.Drawing.Size(636, 42);
            this.chartRailingTestOp1.TabIndex = 138;
            this.chartRailingTestOp1.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.SystemDefault;
            title176.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            title176.BackColor = System.Drawing.Color.Transparent;
            title176.DockedToChartArea = "chartAreaOp1";
            title176.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            title176.Font = new System.Drawing.Font("Montserrat Medium", 10F, System.Drawing.FontStyle.Bold);
            title176.ForeColor = System.Drawing.Color.Gray;
            title176.IsDockedInsideChartArea = false;
            title176.Name = "railingResRailingTestOp1";
            title176.Text = "railOp1";
            title176.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            this.chartRailingTestOp1.Titles.Add(title176);
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
            this.btnElectrodeRailingTestR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeRailingTestR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeRailingTestR7.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeRailingTestR7.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeRailingTestR7.Location = new System.Drawing.Point(450, 308);
            this.btnElectrodeRailingTestR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeRailingTestR7.Name = "btnElectrodeRailingTestR7";
            this.btnElectrodeRailingTestR7.Size = new System.Drawing.Size(53, 42);
            this.btnElectrodeRailingTestR7.TabIndex = 30;
            this.btnElectrodeRailingTestR7.Text = "R7";
            this.btnElectrodeRailingTestR7.UseCompatibleTextRendering = true;
            this.btnElectrodeRailingTestR7.UseMnemonic = false;
            this.btnElectrodeRailingTestR7.UseVisualStyleBackColor = false;
            // 
            // tabPageImpedance
            // 
            this.tabPageImpedance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tabPageImpedance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPageImpedance.Controls.Add(this.tableLayoutImpedanceTest);
            this.tabPageImpedance.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.tabPageImpedance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tabPageImpedance.Location = new System.Drawing.Point(4, 54);
            this.tabPageImpedance.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageImpedance.Name = "tabPageImpedance";
            this.tabPageImpedance.Size = new System.Drawing.Size(1242, 842);
            this.tabPageImpedance.TabIndex = 0;
            this.tabPageImpedance.Text = "2. Impedance";
            // 
            // tableLayoutImpedanceTest
            // 
            this.tableLayoutImpedanceTest.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutImpedanceTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.ColumnCount = 23;
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.374686F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.374686F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.374686F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375857F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.393714F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.393714F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.305434F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.221015F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.429093F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.251929F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.375859F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.370391F));
            this.tableLayoutImpedanceTest.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.000348F));
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestInfo2, 0, 12);
            this.tableLayoutImpedanceTest.Controls.Add(this.label5, 14, 0);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestingState5, 7, 7);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestingState4, 7, 6);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestingState2, 7, 3);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestingState3, 7, 4);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR1, 18, 1);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestInfo1, 0, 5);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTest, 0, 0);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR8, 16, 8);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR7, 16, 7);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR6, 16, 6);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR5, 16, 5);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR1, 16, 1);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR2, 16, 2);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR3, 16, 3);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestR4, 16, 4);
            this.tableLayoutImpedanceTest.Controls.Add(this.label7, 17, 1);
            this.tableLayoutImpedanceTest.Controls.Add(this.label8, 17, 2);
            this.tableLayoutImpedanceTest.Controls.Add(this.label9, 17, 3);
            this.tableLayoutImpedanceTest.Controls.Add(this.label10, 17, 4);
            this.tableLayoutImpedanceTest.Controls.Add(this.label11, 17, 5);
            this.tableLayoutImpedanceTest.Controls.Add(this.label12, 17, 6);
            this.tableLayoutImpedanceTest.Controls.Add(this.label13, 17, 7);
            this.tableLayoutImpedanceTest.Controls.Add(this.label14, 17, 8);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp1, 16, 10);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp2, 16, 11);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp3, 16, 12);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp4, 16, 13);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp6, 16, 15);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp5, 16, 14);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp7, 16, 16);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnElectrodeImpedanceTestOp8, 16, 17);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTest1, 17, 10);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTest2, 17, 11);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTest4, 17, 13);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTestOp5, 17, 14);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTestOp6, 17, 15);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTestOp7, 17, 16);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTestOp8, 17, 17);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceImpedance, 18, 0);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR2, 18, 2);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR3, 18, 3);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR4, 18, 4);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR5, 18, 5);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR6, 18, 6);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR7, 18, 7);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestR8, 18, 8);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp1, 18, 10);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp2, 18, 11);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp3, 18, 12);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp4, 18, 13);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp5, 18, 14);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp6, 18, 15);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp7, 18, 16);
            this.tableLayoutImpedanceTest.Controls.Add(this.btnImpedanceResImpedanceTestOp8, 18, 17);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelOptionalElectrodesImpedanceTest, 16, 9);
            this.tableLayoutImpedanceTest.Controls.Add(this.buttonTestImpedance, 9, 9);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelImpedanceTestingState1, 7, 2);
            this.tableLayoutImpedanceTest.Controls.Add(this.labelEqualsImpedanceTest3, 17, 12);
            this.tableLayoutImpedanceTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tableLayoutImpedanceTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutImpedanceTest.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutImpedanceTest.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutImpedanceTest.Name = "tableLayoutImpedanceTest";
            this.tableLayoutImpedanceTest.RowCount = 19;
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.113038F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.362454F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.351938F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.114074F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270565F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutImpedanceTest.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutImpedanceTest.Size = new System.Drawing.Size(1242, 842);
            this.tableLayoutImpedanceTest.TabIndex = 5;
            // 
            // labelImpedanceTestInfo2
            // 
            this.labelImpedanceTestInfo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelImpedanceTestInfo2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestInfo2, 7);
            this.labelImpedanceTestInfo2.Font = new System.Drawing.Font("Montserrat SemiBold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestInfo2.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestInfo2.Location = new System.Drawing.Point(20, 528);
            this.labelImpedanceTestInfo2.Margin = new System.Windows.Forms.Padding(20, 0, 0, 10);
            this.labelImpedanceTestInfo2.Name = "labelImpedanceTestInfo2";
            this.tableLayoutImpedanceTest.SetRowSpan(this.labelImpedanceTestInfo2, 7);
            this.labelImpedanceTestInfo2.Size = new System.Drawing.Size(382, 304);
            this.labelImpedanceTestInfo2.TabIndex = 175;
            this.labelImpedanceTestInfo2.Text = "Check if you are grounded\r\n\r\nPlace the electrode as close to the scalp as possibl" +
    "e, parting hair under the electrode if needed\r\n\r\nAdd a little more gel";
            this.labelImpedanceTestInfo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.tableLayoutImpedanceTest.SetColumnSpan(this.label5, 3);
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(777, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 44);
            this.label5.TabIndex = 174;
            this.label5.Text = "Electrode";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelImpedanceTestingState5
            // 
            this.labelImpedanceTestingState5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestingState5, 9);
            this.labelImpedanceTestingState5.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelImpedanceTestingState5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceTestingState5.Font = new System.Drawing.Font("Montserrat SemiBold", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestingState5.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestingState5.Location = new System.Drawing.Point(417, 308);
            this.labelImpedanceTestingState5.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.labelImpedanceTestingState5.Name = "labelImpedanceTestingState5";
            this.labelImpedanceTestingState5.Size = new System.Drawing.Size(454, 44);
            this.labelImpedanceTestingState5.TabIndex = 172;
            this.labelImpedanceTestingState5.Text = "impedance testing";
            this.labelImpedanceTestingState5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelImpedanceTestingState4
            // 
            this.labelImpedanceTestingState4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestingState4, 9);
            this.labelImpedanceTestingState4.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelImpedanceTestingState4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceTestingState4.Font = new System.Drawing.Font("Montserrat SemiBold", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestingState4.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestingState4.Location = new System.Drawing.Point(417, 264);
            this.labelImpedanceTestingState4.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.labelImpedanceTestingState4.Name = "labelImpedanceTestingState4";
            this.labelImpedanceTestingState4.Size = new System.Drawing.Size(454, 44);
            this.labelImpedanceTestingState4.TabIndex = 171;
            this.labelImpedanceTestingState4.Text = "Press Start to begin";
            this.labelImpedanceTestingState4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelImpedanceTestingState2
            // 
            this.labelImpedanceTestingState2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestingState2, 9);
            this.labelImpedanceTestingState2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelImpedanceTestingState2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceTestingState2.Font = new System.Drawing.Font("Montserrat SemiBold", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestingState2.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestingState2.Location = new System.Drawing.Point(417, 132);
            this.labelImpedanceTestingState2.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.labelImpedanceTestingState2.Name = "labelImpedanceTestingState2";
            this.labelImpedanceTestingState2.Size = new System.Drawing.Size(454, 44);
            this.labelImpedanceTestingState2.TabIndex = 170;
            this.labelImpedanceTestingState2.Text = "0.0 minutes";
            this.labelImpedanceTestingState2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelImpedanceTestingState3
            // 
            this.labelImpedanceTestingState3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestingState3, 9);
            this.labelImpedanceTestingState3.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelImpedanceTestingState3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceTestingState3.Font = new System.Drawing.Font("Montserrat SemiBold", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestingState3.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestingState3.Location = new System.Drawing.Point(417, 176);
            this.labelImpedanceTestingState3.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.labelImpedanceTestingState3.Name = "labelImpedanceTestingState3";
            this.labelImpedanceTestingState3.Size = new System.Drawing.Size(454, 44);
            this.labelImpedanceTestingState3.TabIndex = 169;
            this.labelImpedanceTestingState3.Text = "since your last test";
            this.labelImpedanceTestingState3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnImpedanceResImpedanceTestR1
            // 
            this.btnImpedanceResImpedanceTestR1.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestR1.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR1.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR1.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR1.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR1.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR1.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR1, 2);
            this.btnImpedanceResImpedanceTestR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR1.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR1.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR1.Location = new System.Drawing.Point(992, 44);
            this.btnImpedanceResImpedanceTestR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR1.Name = "btnImpedanceResImpedanceTestR1";
            this.btnImpedanceResImpedanceTestR1.Size = new System.Drawing.Size(108, 41);
            this.btnImpedanceResImpedanceTestR1.TabIndex = 147;
            this.btnImpedanceResImpedanceTestR1.Text = "-";
            this.btnImpedanceResImpedanceTestR1.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR1.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR1.UseVisualStyleBackColor = false;
            // 
            // labelImpedanceTestInfo1
            // 
            this.labelImpedanceTestInfo1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelImpedanceTestInfo1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestInfo1, 7);
            this.labelImpedanceTestInfo1.Font = new System.Drawing.Font("Montserrat", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestInfo1.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestInfo1.Location = new System.Drawing.Point(20, 220);
            this.labelImpedanceTestInfo1.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.labelImpedanceTestInfo1.Name = "labelImpedanceTestInfo1";
            this.tableLayoutImpedanceTest.SetRowSpan(this.labelImpedanceTestInfo1, 7);
            this.labelImpedanceTestInfo1.Size = new System.Drawing.Size(382, 308);
            this.labelImpedanceTestInfo1.TabIndex = 102;
            this.labelImpedanceTestInfo1.Text = "After Railing on the previous tab shows green or yellow\r\n“Start” impedance test\r\n" +
    "\r\nIf the impedance value on an electrode is red or yellow:";
            this.labelImpedanceTestInfo1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelImpedanceTest
            // 
            this.labelImpedanceTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelImpedanceTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTest, 7);
            this.labelImpedanceTest.Font = new System.Drawing.Font("Montserrat", 44F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTest.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTest.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelImpedanceTest.Location = new System.Drawing.Point(20, 30);
            this.labelImpedanceTest.Margin = new System.Windows.Forms.Padding(20, 30, 0, 0);
            this.labelImpedanceTest.Name = "labelImpedanceTest";
            this.labelImpedanceTest.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutImpedanceTest.SetRowSpan(this.labelImpedanceTest, 5);
            this.labelImpedanceTest.Size = new System.Drawing.Size(382, 190);
            this.labelImpedanceTest.TabIndex = 98;
            this.labelImpedanceTest.Text = "Impedance\r\nTest";
            this.labelImpedanceTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnElectrodeImpedanceTestR8
            // 
            this.btnElectrodeImpedanceTestR8.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR8.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR8.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR8.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR8.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR8.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR8.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR8.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR8.Location = new System.Drawing.Point(886, 352);
            this.btnElectrodeImpedanceTestR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR8.Name = "btnElectrodeImpedanceTestR8";
            this.btnElectrodeImpedanceTestR8.Size = new System.Drawing.Size(52, 43);
            this.btnElectrodeImpedanceTestR8.TabIndex = 33;
            this.btnElectrodeImpedanceTestR8.Text = "R8";
            this.btnElectrodeImpedanceTestR8.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR8.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR8.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR7
            // 
            this.btnElectrodeImpedanceTestR7.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR7.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR7.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR7.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR7.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR7.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR7.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR7.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR7.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR7.Location = new System.Drawing.Point(886, 308);
            this.btnElectrodeImpedanceTestR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR7.Name = "btnElectrodeImpedanceTestR7";
            this.btnElectrodeImpedanceTestR7.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestR7.TabIndex = 30;
            this.btnElectrodeImpedanceTestR7.Text = "R7";
            this.btnElectrodeImpedanceTestR7.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR7.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR6
            // 
            this.btnElectrodeImpedanceTestR6.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR6.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR6.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR6.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR6.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR6.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR6.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR6.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR6.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR6.Location = new System.Drawing.Point(886, 264);
            this.btnElectrodeImpedanceTestR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR6.Name = "btnElectrodeImpedanceTestR6";
            this.btnElectrodeImpedanceTestR6.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestR6.TabIndex = 27;
            this.btnElectrodeImpedanceTestR6.Text = "R6";
            this.btnElectrodeImpedanceTestR6.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR6.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR5
            // 
            this.btnElectrodeImpedanceTestR5.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR5.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR5.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR5.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR5.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR5.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR5.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR5.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR5.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR5.Location = new System.Drawing.Point(886, 220);
            this.btnElectrodeImpedanceTestR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR5.Name = "btnElectrodeImpedanceTestR5";
            this.btnElectrodeImpedanceTestR5.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestR5.TabIndex = 24;
            this.btnElectrodeImpedanceTestR5.Text = "R5";
            this.btnElectrodeImpedanceTestR5.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR5.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR1
            // 
            this.btnElectrodeImpedanceTestR1.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR1.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR1.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR1.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR1.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR1.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR1.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR1.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR1.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR1.Location = new System.Drawing.Point(886, 44);
            this.btnElectrodeImpedanceTestR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR1.Name = "btnElectrodeImpedanceTestR1";
            this.btnElectrodeImpedanceTestR1.Size = new System.Drawing.Size(52, 41);
            this.btnElectrodeImpedanceTestR1.TabIndex = 5;
            this.btnElectrodeImpedanceTestR1.Text = "R1";
            this.btnElectrodeImpedanceTestR1.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR1.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR2
            // 
            this.btnElectrodeImpedanceTestR2.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR2.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR2.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR2.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR2.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR2.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR2.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR2.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR2.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR2.Location = new System.Drawing.Point(886, 87);
            this.btnElectrodeImpedanceTestR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR2.Name = "btnElectrodeImpedanceTestR2";
            this.btnElectrodeImpedanceTestR2.Size = new System.Drawing.Size(52, 43);
            this.btnElectrodeImpedanceTestR2.TabIndex = 8;
            this.btnElectrodeImpedanceTestR2.Text = "R2";
            this.btnElectrodeImpedanceTestR2.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR2.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR3
            // 
            this.btnElectrodeImpedanceTestR3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR3.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR3.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR3.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR3.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR3.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR3.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR3.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR3.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR3.Location = new System.Drawing.Point(886, 132);
            this.btnElectrodeImpedanceTestR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR3.Name = "btnElectrodeImpedanceTestR3";
            this.btnElectrodeImpedanceTestR3.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestR3.TabIndex = 11;
            this.btnElectrodeImpedanceTestR3.Text = "R3";
            this.btnElectrodeImpedanceTestR3.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR3.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestR4
            // 
            this.btnElectrodeImpedanceTestR4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestR4.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestR4.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestR4.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestR4.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestR4.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestR4.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestR4.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeImpedanceTestR4.Location = new System.Drawing.Point(886, 176);
            this.btnElectrodeImpedanceTestR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestR4.Name = "btnElectrodeImpedanceTestR4";
            this.btnElectrodeImpedanceTestR4.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestR4.TabIndex = 21;
            this.btnElectrodeImpedanceTestR4.Text = "R4";
            this.btnElectrodeImpedanceTestR4.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestR4.UseMnemonic = false;
            this.btnElectrodeImpedanceTestR4.UseVisualStyleBackColor = false;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(938, 44);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 41);
            this.label7.TabIndex = 114;
            this.label7.Text = "=";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.UseCompatibleTextRendering = true;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(938, 87);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 43);
            this.label8.TabIndex = 115;
            this.label8.Text = "=";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.UseCompatibleTextRendering = true;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(938, 132);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 42);
            this.label9.TabIndex = 116;
            this.label9.Text = "=";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label9.UseCompatibleTextRendering = true;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(938, 176);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 42);
            this.label10.TabIndex = 117;
            this.label10.Text = "=";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label10.UseCompatibleTextRendering = true;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(938, 220);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 42);
            this.label11.TabIndex = 118;
            this.label11.Text = "=";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label11.UseCompatibleTextRendering = true;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(938, 264);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 42);
            this.label12.TabIndex = 119;
            this.label12.Text = "=";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label12.UseCompatibleTextRendering = true;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(938, 308);
            this.label13.Margin = new System.Windows.Forms.Padding(0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(51, 42);
            this.label13.TabIndex = 120;
            this.label13.Text = "=";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label13.UseCompatibleTextRendering = true;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(938, 352);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 41);
            this.label14.TabIndex = 121;
            this.label14.Text = "=";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label14.UseCompatibleTextRendering = true;
            // 
            // btnElectrodeImpedanceTestOp1
            // 
            this.btnElectrodeImpedanceTestOp1.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp1.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp1.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp1.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp1.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp1.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp1.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp1.Enabled = false;
            this.btnElectrodeImpedanceTestOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp1.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp1.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp1.Location = new System.Drawing.Point(886, 440);
            this.btnElectrodeImpedanceTestOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp1.Name = "btnElectrodeImpedanceTestOp1";
            this.btnElectrodeImpedanceTestOp1.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp1.TabIndex = 122;
            this.btnElectrodeImpedanceTestOp1.Text = "Op1";
            this.btnElectrodeImpedanceTestOp1.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp1.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp2
            // 
            this.btnElectrodeImpedanceTestOp2.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp2.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp2.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp2.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp2.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp2.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp2.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp2.Enabled = false;
            this.btnElectrodeImpedanceTestOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp2.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp2.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp2.Location = new System.Drawing.Point(886, 484);
            this.btnElectrodeImpedanceTestOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp2.Name = "btnElectrodeImpedanceTestOp2";
            this.btnElectrodeImpedanceTestOp2.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp2.TabIndex = 123;
            this.btnElectrodeImpedanceTestOp2.Text = "Op2";
            this.btnElectrodeImpedanceTestOp2.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp2.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp3
            // 
            this.btnElectrodeImpedanceTestOp3.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp3.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp3.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp3.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp3.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp3.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp3.Enabled = false;
            this.btnElectrodeImpedanceTestOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp3.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp3.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp3.Location = new System.Drawing.Point(886, 528);
            this.btnElectrodeImpedanceTestOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp3.Name = "btnElectrodeImpedanceTestOp3";
            this.btnElectrodeImpedanceTestOp3.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp3.TabIndex = 124;
            this.btnElectrodeImpedanceTestOp3.Text = "Op3";
            this.btnElectrodeImpedanceTestOp3.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp3.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp4
            // 
            this.btnElectrodeImpedanceTestOp4.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp4.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp4.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp4.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp4.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp4.Enabled = false;
            this.btnElectrodeImpedanceTestOp4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp4.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp4.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp4.Location = new System.Drawing.Point(886, 572);
            this.btnElectrodeImpedanceTestOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp4.Name = "btnElectrodeImpedanceTestOp4";
            this.btnElectrodeImpedanceTestOp4.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp4.TabIndex = 125;
            this.btnElectrodeImpedanceTestOp4.Text = "Op4";
            this.btnElectrodeImpedanceTestOp4.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp4.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp6
            // 
            this.btnElectrodeImpedanceTestOp6.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp6.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp6.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp6.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp6.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp6.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp6.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp6.Enabled = false;
            this.btnElectrodeImpedanceTestOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp6.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp6.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp6.Location = new System.Drawing.Point(886, 660);
            this.btnElectrodeImpedanceTestOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp6.Name = "btnElectrodeImpedanceTestOp6";
            this.btnElectrodeImpedanceTestOp6.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp6.TabIndex = 127;
            this.btnElectrodeImpedanceTestOp6.Text = "Op6";
            this.btnElectrodeImpedanceTestOp6.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp6.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp5
            // 
            this.btnElectrodeImpedanceTestOp5.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp5.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp5.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp5.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp5.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp5.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp5.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp5.Enabled = false;
            this.btnElectrodeImpedanceTestOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp5.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp5.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp5.Location = new System.Drawing.Point(886, 616);
            this.btnElectrodeImpedanceTestOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp5.Name = "btnElectrodeImpedanceTestOp5";
            this.btnElectrodeImpedanceTestOp5.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp5.TabIndex = 126;
            this.btnElectrodeImpedanceTestOp5.Text = "Op5";
            this.btnElectrodeImpedanceTestOp5.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp5.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp7
            // 
            this.btnElectrodeImpedanceTestOp7.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp7.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp7.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp7.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp7.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp7.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp7.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp7.Enabled = false;
            this.btnElectrodeImpedanceTestOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp7.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp7.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp7.Location = new System.Drawing.Point(886, 704);
            this.btnElectrodeImpedanceTestOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp7.Name = "btnElectrodeImpedanceTestOp7";
            this.btnElectrodeImpedanceTestOp7.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp7.TabIndex = 128;
            this.btnElectrodeImpedanceTestOp7.Text = "Op7";
            this.btnElectrodeImpedanceTestOp7.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp7.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeImpedanceTestOp8
            // 
            this.btnElectrodeImpedanceTestOp8.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeImpedanceTestOp8.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeImpedanceTestOp8.BorderRadiusBottomRight = 5;
            this.btnElectrodeImpedanceTestOp8.BorderRadiusTopLeft = 5;
            this.btnElectrodeImpedanceTestOp8.BorderRadiusTopRight = 5;
            this.btnElectrodeImpedanceTestOp8.BorderWidth = 2F;
            this.btnElectrodeImpedanceTestOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeImpedanceTestOp8.Enabled = false;
            this.btnElectrodeImpedanceTestOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeImpedanceTestOp8.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeImpedanceTestOp8.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeImpedanceTestOp8.Location = new System.Drawing.Point(886, 748);
            this.btnElectrodeImpedanceTestOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeImpedanceTestOp8.Name = "btnElectrodeImpedanceTestOp8";
            this.btnElectrodeImpedanceTestOp8.Size = new System.Drawing.Size(52, 42);
            this.btnElectrodeImpedanceTestOp8.TabIndex = 129;
            this.btnElectrodeImpedanceTestOp8.Text = "Op8";
            this.btnElectrodeImpedanceTestOp8.UseCompatibleTextRendering = true;
            this.btnElectrodeImpedanceTestOp8.UseMnemonic = false;
            this.btnElectrodeImpedanceTestOp8.UseVisualStyleBackColor = false;
            // 
            // labelEqualsImpedanceTest1
            // 
            this.labelEqualsImpedanceTest1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTest1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTest1.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTest1.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTest1.Location = new System.Drawing.Point(938, 440);
            this.labelEqualsImpedanceTest1.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTest1.Name = "labelEqualsImpedanceTest1";
            this.labelEqualsImpedanceTest1.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTest1.TabIndex = 130;
            this.labelEqualsImpedanceTest1.Text = "=";
            this.labelEqualsImpedanceTest1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTest1.UseCompatibleTextRendering = true;
            // 
            // labelEqualsImpedanceTest2
            // 
            this.labelEqualsImpedanceTest2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTest2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTest2.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTest2.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTest2.Location = new System.Drawing.Point(938, 484);
            this.labelEqualsImpedanceTest2.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTest2.Name = "labelEqualsImpedanceTest2";
            this.labelEqualsImpedanceTest2.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTest2.TabIndex = 131;
            this.labelEqualsImpedanceTest2.Text = "=";
            this.labelEqualsImpedanceTest2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTest2.UseCompatibleTextRendering = true;
            // 
            // labelEqualsImpedanceTest4
            // 
            this.labelEqualsImpedanceTest4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTest4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTest4.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTest4.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTest4.Location = new System.Drawing.Point(938, 572);
            this.labelEqualsImpedanceTest4.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTest4.Name = "labelEqualsImpedanceTest4";
            this.labelEqualsImpedanceTest4.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTest4.TabIndex = 132;
            this.labelEqualsImpedanceTest4.Text = "=";
            this.labelEqualsImpedanceTest4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTest4.UseCompatibleTextRendering = true;
            // 
            // labelEqualsImpedanceTestOp5
            // 
            this.labelEqualsImpedanceTestOp5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTestOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTestOp5.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTestOp5.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTestOp5.Location = new System.Drawing.Point(938, 616);
            this.labelEqualsImpedanceTestOp5.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTestOp5.Name = "labelEqualsImpedanceTestOp5";
            this.labelEqualsImpedanceTestOp5.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTestOp5.TabIndex = 134;
            this.labelEqualsImpedanceTestOp5.Text = "=";
            this.labelEqualsImpedanceTestOp5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTestOp5.UseCompatibleTextRendering = true;
            // 
            // labelEqualsImpedanceTestOp6
            // 
            this.labelEqualsImpedanceTestOp6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTestOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTestOp6.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTestOp6.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTestOp6.Location = new System.Drawing.Point(938, 660);
            this.labelEqualsImpedanceTestOp6.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTestOp6.Name = "labelEqualsImpedanceTestOp6";
            this.labelEqualsImpedanceTestOp6.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTestOp6.TabIndex = 135;
            this.labelEqualsImpedanceTestOp6.Text = "=";
            this.labelEqualsImpedanceTestOp6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTestOp6.UseCompatibleTextRendering = true;
            // 
            // labelEqualsImpedanceTestOp7
            // 
            this.labelEqualsImpedanceTestOp7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTestOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTestOp7.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTestOp7.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTestOp7.Location = new System.Drawing.Point(938, 704);
            this.labelEqualsImpedanceTestOp7.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTestOp7.Name = "labelEqualsImpedanceTestOp7";
            this.labelEqualsImpedanceTestOp7.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTestOp7.TabIndex = 136;
            this.labelEqualsImpedanceTestOp7.Text = "=";
            this.labelEqualsImpedanceTestOp7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTestOp7.UseCompatibleTextRendering = true;
            // 
            // labelEqualsImpedanceTestOp8
            // 
            this.labelEqualsImpedanceTestOp8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTestOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTestOp8.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTestOp8.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTestOp8.Location = new System.Drawing.Point(938, 748);
            this.labelEqualsImpedanceTestOp8.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTestOp8.Name = "labelEqualsImpedanceTestOp8";
            this.labelEqualsImpedanceTestOp8.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTestOp8.TabIndex = 137;
            this.labelEqualsImpedanceTestOp8.Text = "=";
            this.labelEqualsImpedanceTestOp8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTestOp8.UseCompatibleTextRendering = true;
            // 
            // labelImpedanceImpedance
            // 
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceImpedance, 5);
            this.labelImpedanceImpedance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceImpedance.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceImpedance.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceImpedance.Location = new System.Drawing.Point(992, 0);
            this.labelImpedanceImpedance.Margin = new System.Windows.Forms.Padding(0);
            this.labelImpedanceImpedance.Name = "labelImpedanceImpedance";
            this.labelImpedanceImpedance.Size = new System.Drawing.Size(250, 44);
            this.labelImpedanceImpedance.TabIndex = 2;
            this.labelImpedanceImpedance.Text = "Impedance (kOhm)";
            this.labelImpedanceImpedance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnImpedanceResImpedanceTestR2
            // 
            this.btnImpedanceResImpedanceTestR2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR2.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR2.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR2.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR2.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR2.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR2.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR2, 2);
            this.btnImpedanceResImpedanceTestR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR2.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR2.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR2.Location = new System.Drawing.Point(992, 87);
            this.btnImpedanceResImpedanceTestR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR2.Name = "btnImpedanceResImpedanceTestR2";
            this.btnImpedanceResImpedanceTestR2.Size = new System.Drawing.Size(108, 43);
            this.btnImpedanceResImpedanceTestR2.TabIndex = 148;
            this.btnImpedanceResImpedanceTestR2.Text = "-";
            this.btnImpedanceResImpedanceTestR2.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR2.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR2.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestR3
            // 
            this.btnImpedanceResImpedanceTestR3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR3.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR3.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR3.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR3.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR3.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR3.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR3, 2);
            this.btnImpedanceResImpedanceTestR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR3.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR3.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR3.Location = new System.Drawing.Point(992, 132);
            this.btnImpedanceResImpedanceTestR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR3.Name = "btnImpedanceResImpedanceTestR3";
            this.btnImpedanceResImpedanceTestR3.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestR3.TabIndex = 149;
            this.btnImpedanceResImpedanceTestR3.Text = "-";
            this.btnImpedanceResImpedanceTestR3.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR3.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR3.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestR4
            // 
            this.btnImpedanceResImpedanceTestR4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR4.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR4.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR4.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR4.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR4.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR4.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR4, 2);
            this.btnImpedanceResImpedanceTestR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR4.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR4.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR4.Location = new System.Drawing.Point(992, 176);
            this.btnImpedanceResImpedanceTestR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR4.Name = "btnImpedanceResImpedanceTestR4";
            this.btnImpedanceResImpedanceTestR4.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestR4.TabIndex = 150;
            this.btnImpedanceResImpedanceTestR4.Text = "-";
            this.btnImpedanceResImpedanceTestR4.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR4.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR4.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestR5
            // 
            this.btnImpedanceResImpedanceTestR5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR5.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR5.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR5.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR5.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR5.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR5.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR5, 2);
            this.btnImpedanceResImpedanceTestR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR5.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR5.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR5.Location = new System.Drawing.Point(992, 220);
            this.btnImpedanceResImpedanceTestR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR5.Name = "btnImpedanceResImpedanceTestR5";
            this.btnImpedanceResImpedanceTestR5.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestR5.TabIndex = 152;
            this.btnImpedanceResImpedanceTestR5.Text = "-";
            this.btnImpedanceResImpedanceTestR5.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR5.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR5.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestR6
            // 
            this.btnImpedanceResImpedanceTestR6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR6.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR6.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR6.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR6.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR6.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR6.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR6, 2);
            this.btnImpedanceResImpedanceTestR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR6.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR6.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR6.Location = new System.Drawing.Point(992, 264);
            this.btnImpedanceResImpedanceTestR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR6.Name = "btnImpedanceResImpedanceTestR6";
            this.btnImpedanceResImpedanceTestR6.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestR6.TabIndex = 151;
            this.btnImpedanceResImpedanceTestR6.Text = "-";
            this.btnImpedanceResImpedanceTestR6.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR6.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR6.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestR7
            // 
            this.btnImpedanceResImpedanceTestR7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR7.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR7.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR7.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR7.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR7.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR7.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR7, 2);
            this.btnImpedanceResImpedanceTestR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR7.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR7.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR7.Location = new System.Drawing.Point(992, 308);
            this.btnImpedanceResImpedanceTestR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR7.Name = "btnImpedanceResImpedanceTestR7";
            this.btnImpedanceResImpedanceTestR7.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestR7.TabIndex = 153;
            this.btnImpedanceResImpedanceTestR7.Text = "-";
            this.btnImpedanceResImpedanceTestR7.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR7.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR7.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestR8
            // 
            this.btnImpedanceResImpedanceTestR8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResImpedanceTestR8.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR8.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestR8.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestR8.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestR8.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestR8.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestR8, 2);
            this.btnImpedanceResImpedanceTestR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestR8.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestR8.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestR8.Location = new System.Drawing.Point(992, 352);
            this.btnImpedanceResImpedanceTestR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestR8.Name = "btnImpedanceResImpedanceTestR8";
            this.btnImpedanceResImpedanceTestR8.Size = new System.Drawing.Size(108, 43);
            this.btnImpedanceResImpedanceTestR8.TabIndex = 154;
            this.btnImpedanceResImpedanceTestR8.Text = "-";
            this.btnImpedanceResImpedanceTestR8.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestR8.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestR8.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp1
            // 
            this.btnImpedanceResImpedanceTestOp1.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp1.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp1.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp1.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp1.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp1.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp1.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp1, 2);
            this.btnImpedanceResImpedanceTestOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp1.Enabled = false;
            this.btnImpedanceResImpedanceTestOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp1.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp1.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp1.Location = new System.Drawing.Point(992, 440);
            this.btnImpedanceResImpedanceTestOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp1.Name = "btnImpedanceResImpedanceTestOp1";
            this.btnImpedanceResImpedanceTestOp1.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp1.TabIndex = 157;
            this.btnImpedanceResImpedanceTestOp1.Text = "-";
            this.btnImpedanceResImpedanceTestOp1.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp1.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp1.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp2
            // 
            this.btnImpedanceResImpedanceTestOp2.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp2.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp2.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp2.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp2.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp2.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp2.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp2, 2);
            this.btnImpedanceResImpedanceTestOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp2.Enabled = false;
            this.btnImpedanceResImpedanceTestOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp2.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp2.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp2.Location = new System.Drawing.Point(992, 484);
            this.btnImpedanceResImpedanceTestOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp2.Name = "btnImpedanceResImpedanceTestOp2";
            this.btnImpedanceResImpedanceTestOp2.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp2.TabIndex = 163;
            this.btnImpedanceResImpedanceTestOp2.Text = "-";
            this.btnImpedanceResImpedanceTestOp2.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp2.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp2.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp3
            // 
            this.btnImpedanceResImpedanceTestOp3.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp3.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp3.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp3.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp3.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp3.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp3.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp3, 2);
            this.btnImpedanceResImpedanceTestOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp3.Enabled = false;
            this.btnImpedanceResImpedanceTestOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp3.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp3.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp3.Location = new System.Drawing.Point(992, 528);
            this.btnImpedanceResImpedanceTestOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp3.Name = "btnImpedanceResImpedanceTestOp3";
            this.btnImpedanceResImpedanceTestOp3.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp3.TabIndex = 164;
            this.btnImpedanceResImpedanceTestOp3.Text = "-";
            this.btnImpedanceResImpedanceTestOp3.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp3.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp3.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp4
            // 
            this.btnImpedanceResImpedanceTestOp4.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp4.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp4.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp4.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp4.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp4.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp4.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp4, 2);
            this.btnImpedanceResImpedanceTestOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp4.Enabled = false;
            this.btnImpedanceResImpedanceTestOp4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp4.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp4.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp4.Location = new System.Drawing.Point(992, 572);
            this.btnImpedanceResImpedanceTestOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp4.Name = "btnImpedanceResImpedanceTestOp4";
            this.btnImpedanceResImpedanceTestOp4.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp4.TabIndex = 165;
            this.btnImpedanceResImpedanceTestOp4.Text = "-";
            this.btnImpedanceResImpedanceTestOp4.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp4.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp4.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp5
            // 
            this.btnImpedanceResImpedanceTestOp5.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp5.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp5.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp5.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp5.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp5.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp5.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp5, 2);
            this.btnImpedanceResImpedanceTestOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp5.Enabled = false;
            this.btnImpedanceResImpedanceTestOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp5.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp5.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp5.Location = new System.Drawing.Point(992, 616);
            this.btnImpedanceResImpedanceTestOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp5.Name = "btnImpedanceResImpedanceTestOp5";
            this.btnImpedanceResImpedanceTestOp5.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp5.TabIndex = 158;
            this.btnImpedanceResImpedanceTestOp5.Text = "-";
            this.btnImpedanceResImpedanceTestOp5.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp5.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp5.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp6
            // 
            this.btnImpedanceResImpedanceTestOp6.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp6.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp6.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp6.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp6.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp6.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp6.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp6, 2);
            this.btnImpedanceResImpedanceTestOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp6.Enabled = false;
            this.btnImpedanceResImpedanceTestOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp6.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp6.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp6.Location = new System.Drawing.Point(992, 660);
            this.btnImpedanceResImpedanceTestOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp6.Name = "btnImpedanceResImpedanceTestOp6";
            this.btnImpedanceResImpedanceTestOp6.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp6.TabIndex = 160;
            this.btnImpedanceResImpedanceTestOp6.Text = "-";
            this.btnImpedanceResImpedanceTestOp6.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp6.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp6.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp7
            // 
            this.btnImpedanceResImpedanceTestOp7.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp7.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp7.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp7.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp7.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp7.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp7.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp7, 2);
            this.btnImpedanceResImpedanceTestOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp7.Enabled = false;
            this.btnImpedanceResImpedanceTestOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp7.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp7.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp7.Location = new System.Drawing.Point(992, 704);
            this.btnImpedanceResImpedanceTestOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp7.Name = "btnImpedanceResImpedanceTestOp7";
            this.btnImpedanceResImpedanceTestOp7.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp7.TabIndex = 156;
            this.btnImpedanceResImpedanceTestOp7.Text = "-";
            this.btnImpedanceResImpedanceTestOp7.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp7.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp7.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResImpedanceTestOp8
            // 
            this.btnImpedanceResImpedanceTestOp8.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResImpedanceTestOp8.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp8.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResImpedanceTestOp8.BorderRadiusBottomRight = 5;
            this.btnImpedanceResImpedanceTestOp8.BorderRadiusTopLeft = 5;
            this.btnImpedanceResImpedanceTestOp8.BorderRadiusTopRight = 5;
            this.btnImpedanceResImpedanceTestOp8.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.btnImpedanceResImpedanceTestOp8, 2);
            this.btnImpedanceResImpedanceTestOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResImpedanceTestOp8.Enabled = false;
            this.btnImpedanceResImpedanceTestOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResImpedanceTestOp8.Font = new System.Drawing.Font("Montserrat SemiBold", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResImpedanceTestOp8.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResImpedanceTestOp8.Location = new System.Drawing.Point(992, 748);
            this.btnImpedanceResImpedanceTestOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResImpedanceTestOp8.Name = "btnImpedanceResImpedanceTestOp8";
            this.btnImpedanceResImpedanceTestOp8.Size = new System.Drawing.Size(108, 42);
            this.btnImpedanceResImpedanceTestOp8.TabIndex = 159;
            this.btnImpedanceResImpedanceTestOp8.Text = "-";
            this.btnImpedanceResImpedanceTestOp8.UseCompatibleTextRendering = true;
            this.btnImpedanceResImpedanceTestOp8.UseMnemonic = false;
            this.btnImpedanceResImpedanceTestOp8.UseVisualStyleBackColor = false;
            // 
            // labelOptionalElectrodesImpedanceTest
            // 
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelOptionalElectrodesImpedanceTest, 5);
            this.labelOptionalElectrodesImpedanceTest.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOptionalElectrodesImpedanceTest.ForeColor = System.Drawing.Color.Gray;
            this.labelOptionalElectrodesImpedanceTest.Location = new System.Drawing.Point(886, 397);
            this.labelOptionalElectrodesImpedanceTest.Margin = new System.Windows.Forms.Padding(0);
            this.labelOptionalElectrodesImpedanceTest.Name = "labelOptionalElectrodesImpedanceTest";
            this.labelOptionalElectrodesImpedanceTest.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.labelOptionalElectrodesImpedanceTest.Size = new System.Drawing.Size(254, 43);
            this.labelOptionalElectrodesImpedanceTest.TabIndex = 111;
            this.labelOptionalElectrodesImpedanceTest.Text = "Optional Electrodes";
            this.labelOptionalElectrodesImpedanceTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonTestImpedance
            // 
            this.buttonTestImpedance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTestImpedance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonTestImpedance.BorderColor = System.Drawing.Color.Black;
            this.buttonTestImpedance.BorderRadiusBottomLeft = 5;
            this.buttonTestImpedance.BorderRadiusBottomRight = 5;
            this.buttonTestImpedance.BorderRadiusTopLeft = 5;
            this.buttonTestImpedance.BorderRadiusTopRight = 5;
            this.buttonTestImpedance.BorderWidth = 2F;
            this.tableLayoutImpedanceTest.SetColumnSpan(this.buttonTestImpedance, 4);
            this.buttonTestImpedance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTestImpedance.Font = new System.Drawing.Font("Montserrat SemiBold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTestImpedance.ForeColor = System.Drawing.Color.Black;
            this.buttonTestImpedance.Location = new System.Drawing.Point(570, 397);
            this.buttonTestImpedance.Margin = new System.Windows.Forms.Padding(0);
            this.buttonTestImpedance.Name = "buttonTestImpedance";
            this.tableLayoutImpedanceTest.SetRowSpan(this.buttonTestImpedance, 2);
            this.buttonTestImpedance.Size = new System.Drawing.Size(153, 53);
            this.buttonTestImpedance.TabIndex = 167;
            this.buttonTestImpedance.Text = "Start";
            this.buttonTestImpedance.UseCompatibleTextRendering = true;
            this.buttonTestImpedance.UseMnemonic = false;
            this.buttonTestImpedance.UseVisualStyleBackColor = false;
            this.buttonTestImpedance.Click += new System.EventHandler(this.buttonTestImpedance_Click);
            // 
            // labelImpedanceTestingState1
            // 
            this.labelImpedanceTestingState1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutImpedanceTest.SetColumnSpan(this.labelImpedanceTestingState1, 9);
            this.labelImpedanceTestingState1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelImpedanceTestingState1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceTestingState1.Font = new System.Drawing.Font("Montserrat SemiBold", 13.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImpedanceTestingState1.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceTestingState1.Location = new System.Drawing.Point(417, 87);
            this.labelImpedanceTestingState1.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.labelImpedanceTestingState1.Name = "labelImpedanceTestingState1";
            this.labelImpedanceTestingState1.Size = new System.Drawing.Size(454, 45);
            this.labelImpedanceTestingState1.TabIndex = 168;
            this.labelImpedanceTestingState1.Text = "It has been";
            this.labelImpedanceTestingState1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelEqualsImpedanceTest3
            // 
            this.labelEqualsImpedanceTest3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsImpedanceTest3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsImpedanceTest3.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsImpedanceTest3.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsImpedanceTest3.Location = new System.Drawing.Point(938, 528);
            this.labelEqualsImpedanceTest3.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsImpedanceTest3.Name = "labelEqualsImpedanceTest3";
            this.labelEqualsImpedanceTest3.Size = new System.Drawing.Size(51, 42);
            this.labelEqualsImpedanceTest3.TabIndex = 133;
            this.labelEqualsImpedanceTest3.Text = "=";
            this.labelEqualsImpedanceTest3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsImpedanceTest3.UseCompatibleTextRendering = true;
            // 
            // tabPageQuality
            // 
            this.tabPageQuality.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tabPageQuality.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPageQuality.Controls.Add(this.tableLayoutQualityResults);
            this.tabPageQuality.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.tabPageQuality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tabPageQuality.ImageKey = "(none)";
            this.tabPageQuality.Location = new System.Drawing.Point(4, 54);
            this.tabPageQuality.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageQuality.Name = "tabPageQuality";
            this.tabPageQuality.Size = new System.Drawing.Size(1242, 842);
            this.tabPageQuality.TabIndex = 1;
            this.tabPageQuality.Text = "Quality";
            // 
            // tableLayoutQualityResults
            // 
            this.tableLayoutQualityResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutQualityResults.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutQualityResults.ColumnCount = 23;
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.369549F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.369549F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.363227F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364395F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.294157F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.462556F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.378356F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.294157F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.209958F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.462556F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.294157F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.462556F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.125758F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364397F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364397F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.417492F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.240791F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364397F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364397F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364397F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4.364397F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.352204F));
            this.tableLayoutQualityResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.352204F));
            this.tableLayoutQualityResults.Controls.Add(this.label16, 9, 0);
            this.tableLayoutQualityResults.Controls.Add(this.labelQualityResultsInfo1, 0, 5);
            this.tableLayoutQualityResults.Controls.Add(this.labelQualityResultsInfo2, 0, 14);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR1, 16, 1);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR8, 11, 8);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR7, 11, 7);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR6, 11, 6);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR5, 11, 5);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR1, 11, 1);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR2, 11, 2);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR3, 11, 3);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsR4, 11, 4);
            this.tableLayoutQualityResults.Controls.Add(this.label25, 12, 1);
            this.tableLayoutQualityResults.Controls.Add(this.label26, 12, 2);
            this.tableLayoutQualityResults.Controls.Add(this.label27, 12, 3);
            this.tableLayoutQualityResults.Controls.Add(this.label28, 12, 4);
            this.tableLayoutQualityResults.Controls.Add(this.label29, 12, 5);
            this.tableLayoutQualityResults.Controls.Add(this.label30, 12, 6);
            this.tableLayoutQualityResults.Controls.Add(this.label31, 12, 7);
            this.tableLayoutQualityResults.Controls.Add(this.label32, 12, 8);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp1, 11, 10);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp2, 11, 11);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp3, 11, 12);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp4, 11, 13);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp6, 11, 15);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp5, 11, 14);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp7, 11, 16);
            this.tableLayoutQualityResults.Controls.Add(this.btnElectrodeQualityResultsOp8, 11, 17);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults1, 12, 10);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults2, 12, 11);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults4, 12, 13);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults3, 12, 12);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults5, 12, 14);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults6, 12, 15);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResults7, 12, 16);
            this.tableLayoutQualityResults.Controls.Add(this.labelEqualsQualityResult8, 12, 17);
            this.tableLayoutQualityResults.Controls.Add(this.labelImpedanceQualityResults, 16, 0);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR2, 16, 2);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR3, 16, 3);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR4, 16, 4);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR5, 16, 5);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR6, 16, 6);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR7, 16, 7);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsR8, 16, 8);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp1, 16, 10);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp2, 16, 11);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp3, 16, 12);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp4, 16, 13);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp5, 16, 14);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp6, 16, 15);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp7, 16, 16);
            this.tableLayoutQualityResults.Controls.Add(this.btnImpedanceResQualityResultsOp8, 16, 17);
            this.tableLayoutQualityResults.Controls.Add(this.labelOptionalElectrodesQualityResults, 11, 9);
            this.tableLayoutQualityResults.Controls.Add(this.labelRailingQualityResults, 13, 0);
            this.tableLayoutQualityResults.Controls.Add(this.label47, 15, 1);
            this.tableLayoutQualityResults.Controls.Add(this.label48, 15, 2);
            this.tableLayoutQualityResults.Controls.Add(this.label50, 15, 3);
            this.tableLayoutQualityResults.Controls.Add(this.label51, 15, 4);
            this.tableLayoutQualityResults.Controls.Add(this.label52, 15, 5);
            this.tableLayoutQualityResults.Controls.Add(this.label65, 15, 6);
            this.tableLayoutQualityResults.Controls.Add(this.label74, 15, 7);
            this.tableLayoutQualityResults.Controls.Add(this.label75, 15, 8);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults1, 15, 10);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults2, 15, 11);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults3, 15, 12);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults4, 15, 13);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults5, 15, 14);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults6, 15, 15);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults7, 15, 16);
            this.tableLayoutQualityResults.Controls.Add(this.labelPlusQualityResults8, 15, 17);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR1, 13, 1);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR2, 13, 2);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR3, 13, 3);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR4, 13, 4);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR5, 13, 5);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR6, 13, 6);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR7, 13, 7);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsR8, 13, 8);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp1, 13, 10);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp2, 13, 11);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp3, 13, 12);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp4, 13, 13);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp5, 13, 14);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp6, 13, 15);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp7, 13, 16);
            this.tableLayoutQualityResults.Controls.Add(this.btnRailingResQualityResultsOp8, 13, 17);
            this.tableLayoutQualityResults.Controls.Add(this.labelQualityResults, 0, 0);
            this.tableLayoutQualityResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tableLayoutQualityResults.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutQualityResults.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutQualityResults.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutQualityResults.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutQualityResults.Name = "tableLayoutQualityResults";
            this.tableLayoutQualityResults.RowCount = 19;
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.995142F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.470871F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.113038F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.362454F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.270566F));
            this.tableLayoutQualityResults.Size = new System.Drawing.Size(1242, 842);
            this.tableLayoutQualityResults.TabIndex = 6;
            // 
            // label16
            // 
            this.tableLayoutQualityResults.SetColumnSpan(this.label16, 3);
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(507, 0);
            this.label16.Margin = new System.Windows.Forms.Padding(0);
            this.label16.Name = "label16";
            this.label16.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label16.Size = new System.Drawing.Size(163, 44);
            this.label16.TabIndex = 202;
            this.label16.Text = "Electrode";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelQualityResultsInfo1
            // 
            this.labelQualityResultsInfo1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQualityResultsInfo1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutQualityResults.SetColumnSpan(this.labelQualityResultsInfo1, 7);
            this.labelQualityResultsInfo1.Font = new System.Drawing.Font("Montserrat", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQualityResultsInfo1.ForeColor = System.Drawing.Color.White;
            this.labelQualityResultsInfo1.Location = new System.Drawing.Point(34, 220);
            this.labelQualityResultsInfo1.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.labelQualityResultsInfo1.Name = "labelQualityResultsInfo1";
            this.tableLayoutQualityResults.SetRowSpan(this.labelQualityResultsInfo1, 9);
            this.labelQualityResultsInfo1.Size = new System.Drawing.Size(368, 394);
            this.labelQualityResultsInfo1.TabIndex = 201;
            this.labelQualityResultsInfo1.Text = resources.GetString("labelQualityResultsInfo1.Text");
            this.labelQualityResultsInfo1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelQualityResultsInfo2
            // 
            this.labelQualityResultsInfo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQualityResultsInfo2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutQualityResults.SetColumnSpan(this.labelQualityResultsInfo2, 7);
            this.labelQualityResultsInfo2.Font = new System.Drawing.Font("Montserrat SemiBold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQualityResultsInfo2.ForeColor = System.Drawing.Color.White;
            this.labelQualityResultsInfo2.Location = new System.Drawing.Point(20, 616);
            this.labelQualityResultsInfo2.Margin = new System.Windows.Forms.Padding(20, 0, 0, 10);
            this.labelQualityResultsInfo2.Name = "labelQualityResultsInfo2";
            this.tableLayoutQualityResults.SetRowSpan(this.labelQualityResultsInfo2, 5);
            this.labelQualityResultsInfo2.Size = new System.Drawing.Size(382, 216);
            this.labelQualityResultsInfo2.TabIndex = 200;
            this.labelQualityResultsInfo2.Text = "Congrats if you have all green (maybe a few yellows)!!! \r\n\r\nPress “Next\" to conti" +
    "nue";
            this.labelQualityResultsInfo2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnImpedanceResQualityResultsR1
            // 
            this.btnImpedanceResQualityResultsR1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR1.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR1.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR1.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR1.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR1.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR1.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR1, 2);
            this.btnImpedanceResQualityResultsR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR1.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR1.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR1.Location = new System.Drawing.Point(883, 44);
            this.btnImpedanceResQualityResultsR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR1.Name = "btnImpedanceResQualityResultsR1";
            this.btnImpedanceResQualityResultsR1.Size = new System.Drawing.Size(106, 40);
            this.btnImpedanceResQualityResultsR1.TabIndex = 147;
            this.btnImpedanceResQualityResultsR1.Text = "-";
            this.btnImpedanceResQualityResultsR1.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR1.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR8
            // 
            this.btnElectrodeQualityResultsR8.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR8.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR8.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR8.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR8.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR8.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR8.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR8.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR8.Location = new System.Drawing.Point(615, 352);
            this.btnElectrodeQualityResultsR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR8.Name = "btnElectrodeQualityResultsR8";
            this.btnElectrodeQualityResultsR8.Size = new System.Drawing.Size(55, 41);
            this.btnElectrodeQualityResultsR8.TabIndex = 33;
            this.btnElectrodeQualityResultsR8.Text = "R8";
            this.btnElectrodeQualityResultsR8.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR8.UseMnemonic = false;
            this.btnElectrodeQualityResultsR8.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR7
            // 
            this.btnElectrodeQualityResultsR7.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR7.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR7.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR7.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR7.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR7.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR7.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR7.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR7.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR7.Location = new System.Drawing.Point(615, 308);
            this.btnElectrodeQualityResultsR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR7.Name = "btnElectrodeQualityResultsR7";
            this.btnElectrodeQualityResultsR7.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsR7.TabIndex = 30;
            this.btnElectrodeQualityResultsR7.Text = "R7";
            this.btnElectrodeQualityResultsR7.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR7.UseMnemonic = false;
            this.btnElectrodeQualityResultsR7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR6
            // 
            this.btnElectrodeQualityResultsR6.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR6.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR6.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR6.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR6.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR6.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR6.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR6.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR6.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR6.Location = new System.Drawing.Point(615, 264);
            this.btnElectrodeQualityResultsR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR6.Name = "btnElectrodeQualityResultsR6";
            this.btnElectrodeQualityResultsR6.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsR6.TabIndex = 27;
            this.btnElectrodeQualityResultsR6.Text = "R6";
            this.btnElectrodeQualityResultsR6.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR6.UseMnemonic = false;
            this.btnElectrodeQualityResultsR6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR5
            // 
            this.btnElectrodeQualityResultsR5.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR5.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR5.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR5.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR5.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR5.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR5.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR5.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR5.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR5.Location = new System.Drawing.Point(615, 220);
            this.btnElectrodeQualityResultsR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR5.Name = "btnElectrodeQualityResultsR5";
            this.btnElectrodeQualityResultsR5.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsR5.TabIndex = 24;
            this.btnElectrodeQualityResultsR5.Text = "R5";
            this.btnElectrodeQualityResultsR5.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR5.UseMnemonic = false;
            this.btnElectrodeQualityResultsR5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR1
            // 
            this.btnElectrodeQualityResultsR1.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR1.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR1.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR1.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR1.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR1.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR1.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR1.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR1.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR1.Location = new System.Drawing.Point(615, 44);
            this.btnElectrodeQualityResultsR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR1.Name = "btnElectrodeQualityResultsR1";
            this.btnElectrodeQualityResultsR1.Size = new System.Drawing.Size(55, 40);
            this.btnElectrodeQualityResultsR1.TabIndex = 5;
            this.btnElectrodeQualityResultsR1.Text = "R1";
            this.btnElectrodeQualityResultsR1.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR1.UseMnemonic = false;
            this.btnElectrodeQualityResultsR1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR2
            // 
            this.btnElectrodeQualityResultsR2.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR2.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR2.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR2.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR2.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR2.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR2.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR2.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR2.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR2.Location = new System.Drawing.Point(615, 86);
            this.btnElectrodeQualityResultsR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR2.Name = "btnElectrodeQualityResultsR2";
            this.btnElectrodeQualityResultsR2.Size = new System.Drawing.Size(55, 44);
            this.btnElectrodeQualityResultsR2.TabIndex = 8;
            this.btnElectrodeQualityResultsR2.Text = "R2";
            this.btnElectrodeQualityResultsR2.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR2.UseMnemonic = false;
            this.btnElectrodeQualityResultsR2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR3
            // 
            this.btnElectrodeQualityResultsR3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR3.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR3.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR3.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR3.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR3.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR3.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR3.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR3.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR3.Location = new System.Drawing.Point(615, 132);
            this.btnElectrodeQualityResultsR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR3.Name = "btnElectrodeQualityResultsR3";
            this.btnElectrodeQualityResultsR3.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsR3.TabIndex = 11;
            this.btnElectrodeQualityResultsR3.Text = "R3";
            this.btnElectrodeQualityResultsR3.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR3.UseMnemonic = false;
            this.btnElectrodeQualityResultsR3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsR4
            // 
            this.btnElectrodeQualityResultsR4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsR4.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsR4.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsR4.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsR4.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsR4.BorderWidth = 2F;
            this.btnElectrodeQualityResultsR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsR4.Font = new System.Drawing.Font("Montserrat", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeQualityResultsR4.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeQualityResultsR4.Location = new System.Drawing.Point(615, 176);
            this.btnElectrodeQualityResultsR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsR4.Name = "btnElectrodeQualityResultsR4";
            this.btnElectrodeQualityResultsR4.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsR4.TabIndex = 21;
            this.btnElectrodeQualityResultsR4.Text = "R4";
            this.btnElectrodeQualityResultsR4.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsR4.UseMnemonic = false;
            this.btnElectrodeQualityResultsR4.UseVisualStyleBackColor = false;
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label25.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label25.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.White;
            this.label25.Location = new System.Drawing.Point(670, 44);
            this.label25.Margin = new System.Windows.Forms.Padding(0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(49, 41);
            this.label25.TabIndex = 114;
            this.label25.Text = "=";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label25.UseCompatibleTextRendering = true;
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label26.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label26.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.White;
            this.label26.Location = new System.Drawing.Point(670, 86);
            this.label26.Margin = new System.Windows.Forms.Padding(0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(49, 43);
            this.label26.TabIndex = 115;
            this.label26.Text = "=";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label26.UseCompatibleTextRendering = true;
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label27.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label27.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ForeColor = System.Drawing.Color.White;
            this.label27.Location = new System.Drawing.Point(670, 132);
            this.label27.Margin = new System.Windows.Forms.Padding(0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(49, 42);
            this.label27.TabIndex = 116;
            this.label27.Text = "=";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label27.UseCompatibleTextRendering = true;
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label28.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label28.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ForeColor = System.Drawing.Color.White;
            this.label28.Location = new System.Drawing.Point(670, 176);
            this.label28.Margin = new System.Windows.Forms.Padding(0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(49, 42);
            this.label28.TabIndex = 117;
            this.label28.Text = "=";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label28.UseCompatibleTextRendering = true;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label29.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label29.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.Color.White;
            this.label29.Location = new System.Drawing.Point(670, 220);
            this.label29.Margin = new System.Windows.Forms.Padding(0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(49, 42);
            this.label29.TabIndex = 118;
            this.label29.Text = "=";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label29.UseCompatibleTextRendering = true;
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label30.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label30.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.ForeColor = System.Drawing.Color.White;
            this.label30.Location = new System.Drawing.Point(670, 264);
            this.label30.Margin = new System.Windows.Forms.Padding(0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(49, 42);
            this.label30.TabIndex = 119;
            this.label30.Text = "=";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label30.UseCompatibleTextRendering = true;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label31.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label31.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.White;
            this.label31.Location = new System.Drawing.Point(670, 308);
            this.label31.Margin = new System.Windows.Forms.Padding(0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(49, 42);
            this.label31.TabIndex = 120;
            this.label31.Text = "=";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label31.UseCompatibleTextRendering = true;
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label32.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label32.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.Color.White;
            this.label32.Location = new System.Drawing.Point(670, 352);
            this.label32.Margin = new System.Windows.Forms.Padding(0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(49, 41);
            this.label32.TabIndex = 121;
            this.label32.Text = "=";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label32.UseCompatibleTextRendering = true;
            // 
            // btnElectrodeQualityResultsOp1
            // 
            this.btnElectrodeQualityResultsOp1.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp1.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp1.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp1.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp1.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp1.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp1.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp1.Enabled = false;
            this.btnElectrodeQualityResultsOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp1.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp1.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp1.Location = new System.Drawing.Point(615, 440);
            this.btnElectrodeQualityResultsOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp1.Name = "btnElectrodeQualityResultsOp1";
            this.btnElectrodeQualityResultsOp1.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp1.TabIndex = 122;
            this.btnElectrodeQualityResultsOp1.Text = "Op1";
            this.btnElectrodeQualityResultsOp1.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp1.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp2
            // 
            this.btnElectrodeQualityResultsOp2.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp2.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp2.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp2.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp2.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp2.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp2.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp2.Enabled = false;
            this.btnElectrodeQualityResultsOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp2.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp2.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp2.Location = new System.Drawing.Point(615, 484);
            this.btnElectrodeQualityResultsOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp2.Name = "btnElectrodeQualityResultsOp2";
            this.btnElectrodeQualityResultsOp2.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp2.TabIndex = 123;
            this.btnElectrodeQualityResultsOp2.Text = "Op2";
            this.btnElectrodeQualityResultsOp2.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp2.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp3
            // 
            this.btnElectrodeQualityResultsOp3.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp3.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp3.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp3.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp3.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp3.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp3.Enabled = false;
            this.btnElectrodeQualityResultsOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp3.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp3.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp3.Location = new System.Drawing.Point(615, 528);
            this.btnElectrodeQualityResultsOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp3.Name = "btnElectrodeQualityResultsOp3";
            this.btnElectrodeQualityResultsOp3.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp3.TabIndex = 124;
            this.btnElectrodeQualityResultsOp3.Text = "Op3";
            this.btnElectrodeQualityResultsOp3.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp3.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp4
            // 
            this.btnElectrodeQualityResultsOp4.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp4.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp4.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp4.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp4.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp4.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp4.Enabled = false;
            this.btnElectrodeQualityResultsOp4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp4.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp4.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp4.Location = new System.Drawing.Point(615, 572);
            this.btnElectrodeQualityResultsOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp4.Name = "btnElectrodeQualityResultsOp4";
            this.btnElectrodeQualityResultsOp4.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp4.TabIndex = 125;
            this.btnElectrodeQualityResultsOp4.Text = "Op4";
            this.btnElectrodeQualityResultsOp4.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp4.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp6
            // 
            this.btnElectrodeQualityResultsOp6.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp6.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp6.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp6.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp6.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp6.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp6.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp6.Enabled = false;
            this.btnElectrodeQualityResultsOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp6.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp6.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp6.Location = new System.Drawing.Point(615, 660);
            this.btnElectrodeQualityResultsOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp6.Name = "btnElectrodeQualityResultsOp6";
            this.btnElectrodeQualityResultsOp6.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp6.TabIndex = 127;
            this.btnElectrodeQualityResultsOp6.Text = "Op6";
            this.btnElectrodeQualityResultsOp6.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp6.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp5
            // 
            this.btnElectrodeQualityResultsOp5.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp5.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp5.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp5.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp5.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp5.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp5.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp5.Enabled = false;
            this.btnElectrodeQualityResultsOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp5.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp5.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp5.Location = new System.Drawing.Point(615, 616);
            this.btnElectrodeQualityResultsOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp5.Name = "btnElectrodeQualityResultsOp5";
            this.btnElectrodeQualityResultsOp5.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp5.TabIndex = 126;
            this.btnElectrodeQualityResultsOp5.Text = "Op5";
            this.btnElectrodeQualityResultsOp5.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp5.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp7
            // 
            this.btnElectrodeQualityResultsOp7.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp7.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp7.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp7.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp7.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp7.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp7.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp7.Enabled = false;
            this.btnElectrodeQualityResultsOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp7.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp7.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp7.Location = new System.Drawing.Point(615, 704);
            this.btnElectrodeQualityResultsOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp7.Name = "btnElectrodeQualityResultsOp7";
            this.btnElectrodeQualityResultsOp7.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp7.TabIndex = 128;
            this.btnElectrodeQualityResultsOp7.Text = "Op7";
            this.btnElectrodeQualityResultsOp7.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp7.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeQualityResultsOp8
            // 
            this.btnElectrodeQualityResultsOp8.BackColor = System.Drawing.Color.Transparent;
            this.btnElectrodeQualityResultsOp8.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp8.BorderRadiusBottomLeft = 5;
            this.btnElectrodeQualityResultsOp8.BorderRadiusBottomRight = 5;
            this.btnElectrodeQualityResultsOp8.BorderRadiusTopLeft = 5;
            this.btnElectrodeQualityResultsOp8.BorderRadiusTopRight = 5;
            this.btnElectrodeQualityResultsOp8.BorderWidth = 2F;
            this.btnElectrodeQualityResultsOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnElectrodeQualityResultsOp8.Enabled = false;
            this.btnElectrodeQualityResultsOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeQualityResultsOp8.Font = new System.Drawing.Font("Montserrat", 8.75F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeQualityResultsOp8.ForeColor = System.Drawing.Color.Gray;
            this.btnElectrodeQualityResultsOp8.Location = new System.Drawing.Point(615, 748);
            this.btnElectrodeQualityResultsOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnElectrodeQualityResultsOp8.Name = "btnElectrodeQualityResultsOp8";
            this.btnElectrodeQualityResultsOp8.Size = new System.Drawing.Size(55, 42);
            this.btnElectrodeQualityResultsOp8.TabIndex = 129;
            this.btnElectrodeQualityResultsOp8.Text = "Op8";
            this.btnElectrodeQualityResultsOp8.UseCompatibleTextRendering = true;
            this.btnElectrodeQualityResultsOp8.UseMnemonic = false;
            this.btnElectrodeQualityResultsOp8.UseVisualStyleBackColor = false;
            // 
            // labelEqualsQualityResults1
            // 
            this.labelEqualsQualityResults1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults1.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults1.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults1.Location = new System.Drawing.Point(670, 440);
            this.labelEqualsQualityResults1.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults1.Name = "labelEqualsQualityResults1";
            this.labelEqualsQualityResults1.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults1.TabIndex = 130;
            this.labelEqualsQualityResults1.Text = "=";
            this.labelEqualsQualityResults1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults1.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResults2
            // 
            this.labelEqualsQualityResults2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults2.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults2.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults2.Location = new System.Drawing.Point(670, 484);
            this.labelEqualsQualityResults2.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults2.Name = "labelEqualsQualityResults2";
            this.labelEqualsQualityResults2.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults2.TabIndex = 131;
            this.labelEqualsQualityResults2.Text = "=";
            this.labelEqualsQualityResults2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults2.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResults4
            // 
            this.labelEqualsQualityResults4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults4.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults4.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults4.Location = new System.Drawing.Point(670, 572);
            this.labelEqualsQualityResults4.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults4.Name = "labelEqualsQualityResults4";
            this.labelEqualsQualityResults4.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults4.TabIndex = 132;
            this.labelEqualsQualityResults4.Text = "=";
            this.labelEqualsQualityResults4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults4.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResults3
            // 
            this.labelEqualsQualityResults3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults3.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults3.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults3.Location = new System.Drawing.Point(670, 528);
            this.labelEqualsQualityResults3.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults3.Name = "labelEqualsQualityResults3";
            this.labelEqualsQualityResults3.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults3.TabIndex = 133;
            this.labelEqualsQualityResults3.Text = "=";
            this.labelEqualsQualityResults3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults3.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResults5
            // 
            this.labelEqualsQualityResults5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults5.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults5.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults5.Location = new System.Drawing.Point(670, 616);
            this.labelEqualsQualityResults5.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults5.Name = "labelEqualsQualityResults5";
            this.labelEqualsQualityResults5.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults5.TabIndex = 134;
            this.labelEqualsQualityResults5.Text = "=";
            this.labelEqualsQualityResults5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults5.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResults6
            // 
            this.labelEqualsQualityResults6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults6.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults6.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults6.Location = new System.Drawing.Point(670, 660);
            this.labelEqualsQualityResults6.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults6.Name = "labelEqualsQualityResults6";
            this.labelEqualsQualityResults6.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults6.TabIndex = 135;
            this.labelEqualsQualityResults6.Text = "=";
            this.labelEqualsQualityResults6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults6.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResults7
            // 
            this.labelEqualsQualityResults7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResults7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResults7.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResults7.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResults7.Location = new System.Drawing.Point(670, 704);
            this.labelEqualsQualityResults7.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResults7.Name = "labelEqualsQualityResults7";
            this.labelEqualsQualityResults7.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResults7.TabIndex = 136;
            this.labelEqualsQualityResults7.Text = "=";
            this.labelEqualsQualityResults7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResults7.UseCompatibleTextRendering = true;
            // 
            // labelEqualsQualityResult8
            // 
            this.labelEqualsQualityResult8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelEqualsQualityResult8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelEqualsQualityResult8.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEqualsQualityResult8.ForeColor = System.Drawing.Color.Gray;
            this.labelEqualsQualityResult8.Location = new System.Drawing.Point(670, 748);
            this.labelEqualsQualityResult8.Margin = new System.Windows.Forms.Padding(0);
            this.labelEqualsQualityResult8.Name = "labelEqualsQualityResult8";
            this.labelEqualsQualityResult8.Size = new System.Drawing.Size(49, 42);
            this.labelEqualsQualityResult8.TabIndex = 137;
            this.labelEqualsQualityResult8.Text = "=";
            this.labelEqualsQualityResult8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelEqualsQualityResult8.UseCompatibleTextRendering = true;
            // 
            // labelImpedanceQualityResults
            // 
            this.tableLayoutQualityResults.SetColumnSpan(this.labelImpedanceQualityResults, 5);
            this.labelImpedanceQualityResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelImpedanceQualityResults.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold);
            this.labelImpedanceQualityResults.ForeColor = System.Drawing.Color.White;
            this.labelImpedanceQualityResults.Location = new System.Drawing.Point(883, 0);
            this.labelImpedanceQualityResults.Margin = new System.Windows.Forms.Padding(0);
            this.labelImpedanceQualityResults.Name = "labelImpedanceQualityResults";
            this.labelImpedanceQualityResults.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.labelImpedanceQualityResults.Size = new System.Drawing.Size(268, 44);
            this.labelImpedanceQualityResults.TabIndex = 2;
            this.labelImpedanceQualityResults.Text = "Impedance (kOhm)";
            this.labelImpedanceQualityResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnImpedanceResQualityResultsR2
            // 
            this.btnImpedanceResQualityResultsR2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR2.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR2.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR2.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR2.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR2.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR2.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR2, 2);
            this.btnImpedanceResQualityResultsR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR2.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR2.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR2.Location = new System.Drawing.Point(883, 86);
            this.btnImpedanceResQualityResultsR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR2.Name = "btnImpedanceResQualityResultsR2";
            this.btnImpedanceResQualityResultsR2.Size = new System.Drawing.Size(106, 44);
            this.btnImpedanceResQualityResultsR2.TabIndex = 148;
            this.btnImpedanceResQualityResultsR2.Text = "-";
            this.btnImpedanceResQualityResultsR2.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR2.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR2.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsR3
            // 
            this.btnImpedanceResQualityResultsR3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR3.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR3.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR3.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR3.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR3.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR3.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR3, 2);
            this.btnImpedanceResQualityResultsR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR3.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR3.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR3.Location = new System.Drawing.Point(883, 132);
            this.btnImpedanceResQualityResultsR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR3.Name = "btnImpedanceResQualityResultsR3";
            this.btnImpedanceResQualityResultsR3.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsR3.TabIndex = 149;
            this.btnImpedanceResQualityResultsR3.Text = "-";
            this.btnImpedanceResQualityResultsR3.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR3.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR3.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsR4
            // 
            this.btnImpedanceResQualityResultsR4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR4.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR4.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR4.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR4.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR4.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR4.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR4, 2);
            this.btnImpedanceResQualityResultsR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR4.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR4.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR4.Location = new System.Drawing.Point(883, 176);
            this.btnImpedanceResQualityResultsR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR4.Name = "btnImpedanceResQualityResultsR4";
            this.btnImpedanceResQualityResultsR4.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsR4.TabIndex = 150;
            this.btnImpedanceResQualityResultsR4.Text = "-";
            this.btnImpedanceResQualityResultsR4.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR4.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR4.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsR5
            // 
            this.btnImpedanceResQualityResultsR5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR5.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR5.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR5.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR5.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR5.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR5.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR5, 2);
            this.btnImpedanceResQualityResultsR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR5.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR5.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR5.Location = new System.Drawing.Point(883, 220);
            this.btnImpedanceResQualityResultsR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR5.Name = "btnImpedanceResQualityResultsR5";
            this.btnImpedanceResQualityResultsR5.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsR5.TabIndex = 152;
            this.btnImpedanceResQualityResultsR5.Text = "-";
            this.btnImpedanceResQualityResultsR5.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR5.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR5.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsR6
            // 
            this.btnImpedanceResQualityResultsR6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR6.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR6.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR6.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR6.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR6.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR6.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR6, 2);
            this.btnImpedanceResQualityResultsR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR6.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR6.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR6.Location = new System.Drawing.Point(883, 264);
            this.btnImpedanceResQualityResultsR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR6.Name = "btnImpedanceResQualityResultsR6";
            this.btnImpedanceResQualityResultsR6.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsR6.TabIndex = 151;
            this.btnImpedanceResQualityResultsR6.Text = "-";
            this.btnImpedanceResQualityResultsR6.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR6.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR6.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsR7
            // 
            this.btnImpedanceResQualityResultsR7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR7.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR7.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR7.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR7.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR7.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR7.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR7, 2);
            this.btnImpedanceResQualityResultsR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR7.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR7.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR7.Location = new System.Drawing.Point(883, 308);
            this.btnImpedanceResQualityResultsR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR7.Name = "btnImpedanceResQualityResultsR7";
            this.btnImpedanceResQualityResultsR7.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsR7.TabIndex = 153;
            this.btnImpedanceResQualityResultsR7.Text = "-";
            this.btnImpedanceResQualityResultsR7.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR7.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR7.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsR8
            // 
            this.btnImpedanceResQualityResultsR8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnImpedanceResQualityResultsR8.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR8.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsR8.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsR8.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsR8.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsR8.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsR8, 2);
            this.btnImpedanceResQualityResultsR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsR8.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsR8.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsR8.Location = new System.Drawing.Point(883, 352);
            this.btnImpedanceResQualityResultsR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsR8.Name = "btnImpedanceResQualityResultsR8";
            this.btnImpedanceResQualityResultsR8.Size = new System.Drawing.Size(106, 41);
            this.btnImpedanceResQualityResultsR8.TabIndex = 154;
            this.btnImpedanceResQualityResultsR8.Text = "-";
            this.btnImpedanceResQualityResultsR8.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsR8.UseMnemonic = false;
            this.btnImpedanceResQualityResultsR8.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp1
            // 
            this.btnImpedanceResQualityResultsOp1.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp1.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp1.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp1.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp1.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp1.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp1.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp1, 2);
            this.btnImpedanceResQualityResultsOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp1.Enabled = false;
            this.btnImpedanceResQualityResultsOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp1.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp1.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp1.Location = new System.Drawing.Point(883, 440);
            this.btnImpedanceResQualityResultsOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp1.Name = "btnImpedanceResQualityResultsOp1";
            this.btnImpedanceResQualityResultsOp1.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp1.TabIndex = 157;
            this.btnImpedanceResQualityResultsOp1.Text = "-";
            this.btnImpedanceResQualityResultsOp1.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp1.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp1.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp2
            // 
            this.btnImpedanceResQualityResultsOp2.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp2.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp2.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp2.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp2.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp2.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp2.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp2, 2);
            this.btnImpedanceResQualityResultsOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp2.Enabled = false;
            this.btnImpedanceResQualityResultsOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp2.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp2.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp2.Location = new System.Drawing.Point(883, 484);
            this.btnImpedanceResQualityResultsOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp2.Name = "btnImpedanceResQualityResultsOp2";
            this.btnImpedanceResQualityResultsOp2.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp2.TabIndex = 163;
            this.btnImpedanceResQualityResultsOp2.Text = "-";
            this.btnImpedanceResQualityResultsOp2.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp2.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp2.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp3
            // 
            this.btnImpedanceResQualityResultsOp3.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp3.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp3.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp3.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp3.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp3.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp3.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp3, 2);
            this.btnImpedanceResQualityResultsOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp3.Enabled = false;
            this.btnImpedanceResQualityResultsOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp3.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp3.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp3.Location = new System.Drawing.Point(883, 528);
            this.btnImpedanceResQualityResultsOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp3.Name = "btnImpedanceResQualityResultsOp3";
            this.btnImpedanceResQualityResultsOp3.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp3.TabIndex = 164;
            this.btnImpedanceResQualityResultsOp3.Text = "-";
            this.btnImpedanceResQualityResultsOp3.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp3.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp3.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp4
            // 
            this.btnImpedanceResQualityResultsOp4.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp4.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp4.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp4.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp4.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp4.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp4.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp4, 2);
            this.btnImpedanceResQualityResultsOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp4.Enabled = false;
            this.btnImpedanceResQualityResultsOp4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp4.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp4.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp4.Location = new System.Drawing.Point(883, 572);
            this.btnImpedanceResQualityResultsOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp4.Name = "btnImpedanceResQualityResultsOp4";
            this.btnImpedanceResQualityResultsOp4.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp4.TabIndex = 165;
            this.btnImpedanceResQualityResultsOp4.Text = "-";
            this.btnImpedanceResQualityResultsOp4.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp4.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp4.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp5
            // 
            this.btnImpedanceResQualityResultsOp5.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp5.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp5.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp5.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp5.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp5.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp5.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp5, 2);
            this.btnImpedanceResQualityResultsOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp5.Enabled = false;
            this.btnImpedanceResQualityResultsOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp5.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp5.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp5.Location = new System.Drawing.Point(883, 616);
            this.btnImpedanceResQualityResultsOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp5.Name = "btnImpedanceResQualityResultsOp5";
            this.btnImpedanceResQualityResultsOp5.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp5.TabIndex = 158;
            this.btnImpedanceResQualityResultsOp5.Text = "-";
            this.btnImpedanceResQualityResultsOp5.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp5.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp5.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp6
            // 
            this.btnImpedanceResQualityResultsOp6.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp6.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp6.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp6.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp6.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp6.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp6.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp6, 2);
            this.btnImpedanceResQualityResultsOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp6.Enabled = false;
            this.btnImpedanceResQualityResultsOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp6.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp6.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp6.Location = new System.Drawing.Point(883, 660);
            this.btnImpedanceResQualityResultsOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp6.Name = "btnImpedanceResQualityResultsOp6";
            this.btnImpedanceResQualityResultsOp6.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp6.TabIndex = 160;
            this.btnImpedanceResQualityResultsOp6.Text = "-";
            this.btnImpedanceResQualityResultsOp6.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp6.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp6.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp7
            // 
            this.btnImpedanceResQualityResultsOp7.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp7.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp7.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp7.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp7.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp7.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp7.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp7, 2);
            this.btnImpedanceResQualityResultsOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp7.Enabled = false;
            this.btnImpedanceResQualityResultsOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp7.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp7.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp7.Location = new System.Drawing.Point(883, 704);
            this.btnImpedanceResQualityResultsOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp7.Name = "btnImpedanceResQualityResultsOp7";
            this.btnImpedanceResQualityResultsOp7.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp7.TabIndex = 156;
            this.btnImpedanceResQualityResultsOp7.Text = "-";
            this.btnImpedanceResQualityResultsOp7.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp7.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp7.UseVisualStyleBackColor = false;
            // 
            // btnImpedanceResQualityResultsOp8
            // 
            this.btnImpedanceResQualityResultsOp8.BackColor = System.Drawing.Color.Transparent;
            this.btnImpedanceResQualityResultsOp8.BorderColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp8.BorderRadiusBottomLeft = 5;
            this.btnImpedanceResQualityResultsOp8.BorderRadiusBottomRight = 5;
            this.btnImpedanceResQualityResultsOp8.BorderRadiusTopLeft = 5;
            this.btnImpedanceResQualityResultsOp8.BorderRadiusTopRight = 5;
            this.btnImpedanceResQualityResultsOp8.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnImpedanceResQualityResultsOp8, 2);
            this.btnImpedanceResQualityResultsOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpedanceResQualityResultsOp8.Enabled = false;
            this.btnImpedanceResQualityResultsOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImpedanceResQualityResultsOp8.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImpedanceResQualityResultsOp8.ForeColor = System.Drawing.Color.Gray;
            this.btnImpedanceResQualityResultsOp8.Location = new System.Drawing.Point(883, 748);
            this.btnImpedanceResQualityResultsOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnImpedanceResQualityResultsOp8.Name = "btnImpedanceResQualityResultsOp8";
            this.btnImpedanceResQualityResultsOp8.Size = new System.Drawing.Size(106, 42);
            this.btnImpedanceResQualityResultsOp8.TabIndex = 159;
            this.btnImpedanceResQualityResultsOp8.Text = "-";
            this.btnImpedanceResQualityResultsOp8.UseCompatibleTextRendering = true;
            this.btnImpedanceResQualityResultsOp8.UseMnemonic = false;
            this.btnImpedanceResQualityResultsOp8.UseVisualStyleBackColor = false;
            // 
            // labelOptionalElectrodesQualityResults
            // 
            this.tableLayoutQualityResults.SetColumnSpan(this.labelOptionalElectrodesQualityResults, 6);
            this.labelOptionalElectrodesQualityResults.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOptionalElectrodesQualityResults.ForeColor = System.Drawing.Color.Gray;
            this.labelOptionalElectrodesQualityResults.Location = new System.Drawing.Point(615, 395);
            this.labelOptionalElectrodesQualityResults.Margin = new System.Windows.Forms.Padding(0);
            this.labelOptionalElectrodesQualityResults.Name = "labelOptionalElectrodesQualityResults";
            this.labelOptionalElectrodesQualityResults.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.labelOptionalElectrodesQualityResults.Size = new System.Drawing.Size(308, 45);
            this.labelOptionalElectrodesQualityResults.TabIndex = 111;
            this.labelOptionalElectrodesQualityResults.Text = "Optional Electrodes";
            this.labelOptionalElectrodesQualityResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelRailingQualityResults
            // 
            this.tableLayoutQualityResults.SetColumnSpan(this.labelRailingQualityResults, 3);
            this.labelRailingQualityResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRailingQualityResults.Font = new System.Drawing.Font("Montserrat Medium", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRailingQualityResults.ForeColor = System.Drawing.Color.White;
            this.labelRailingQualityResults.Location = new System.Drawing.Point(721, 0);
            this.labelRailingQualityResults.Margin = new System.Windows.Forms.Padding(0);
            this.labelRailingQualityResults.Name = "labelRailingQualityResults";
            this.labelRailingQualityResults.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.labelRailingQualityResults.Size = new System.Drawing.Size(162, 44);
            this.labelRailingQualityResults.TabIndex = 167;
            this.labelRailingQualityResults.Text = "Railing";
            this.labelRailingQualityResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label47
            // 
            this.label47.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label47.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label47.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.ForeColor = System.Drawing.Color.White;
            this.label47.Location = new System.Drawing.Point(829, 44);
            this.label47.Margin = new System.Windows.Forms.Padding(0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(50, 41);
            this.label47.TabIndex = 168;
            this.label47.Text = "+";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label47.UseCompatibleTextRendering = true;
            // 
            // label48
            // 
            this.label48.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label48.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label48.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label48.ForeColor = System.Drawing.Color.White;
            this.label48.Location = new System.Drawing.Point(829, 86);
            this.label48.Margin = new System.Windows.Forms.Padding(0);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(50, 41);
            this.label48.TabIndex = 169;
            this.label48.Text = "+";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label48.UseCompatibleTextRendering = true;
            // 
            // label50
            // 
            this.label50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label50.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label50.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ForeColor = System.Drawing.Color.White;
            this.label50.Location = new System.Drawing.Point(829, 132);
            this.label50.Margin = new System.Windows.Forms.Padding(0);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(50, 41);
            this.label50.TabIndex = 170;
            this.label50.Text = "+";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label50.UseCompatibleTextRendering = true;
            // 
            // label51
            // 
            this.label51.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label51.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label51.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.ForeColor = System.Drawing.Color.White;
            this.label51.Location = new System.Drawing.Point(829, 176);
            this.label51.Margin = new System.Windows.Forms.Padding(0);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(50, 41);
            this.label51.TabIndex = 171;
            this.label51.Text = "+";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label51.UseCompatibleTextRendering = true;
            // 
            // label52
            // 
            this.label52.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label52.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label52.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.White;
            this.label52.Location = new System.Drawing.Point(829, 220);
            this.label52.Margin = new System.Windows.Forms.Padding(0);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(50, 41);
            this.label52.TabIndex = 172;
            this.label52.Text = "+";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label52.UseCompatibleTextRendering = true;
            // 
            // label65
            // 
            this.label65.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label65.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label65.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label65.ForeColor = System.Drawing.Color.White;
            this.label65.Location = new System.Drawing.Point(829, 264);
            this.label65.Margin = new System.Windows.Forms.Padding(0);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(50, 41);
            this.label65.TabIndex = 173;
            this.label65.Text = "+";
            this.label65.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label65.UseCompatibleTextRendering = true;
            // 
            // label74
            // 
            this.label74.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label74.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label74.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label74.ForeColor = System.Drawing.Color.White;
            this.label74.Location = new System.Drawing.Point(829, 308);
            this.label74.Margin = new System.Windows.Forms.Padding(0);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(50, 41);
            this.label74.TabIndex = 174;
            this.label74.Text = "+";
            this.label74.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label74.UseCompatibleTextRendering = true;
            // 
            // label75
            // 
            this.label75.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.label75.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label75.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label75.ForeColor = System.Drawing.Color.White;
            this.label75.Location = new System.Drawing.Point(829, 352);
            this.label75.Margin = new System.Windows.Forms.Padding(0);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(50, 41);
            this.label75.TabIndex = 175;
            this.label75.Text = "+";
            this.label75.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label75.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults1
            // 
            this.labelPlusQualityResults1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults1.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults1.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults1.Location = new System.Drawing.Point(829, 440);
            this.labelPlusQualityResults1.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults1.Name = "labelPlusQualityResults1";
            this.labelPlusQualityResults1.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults1.TabIndex = 176;
            this.labelPlusQualityResults1.Text = "+";
            this.labelPlusQualityResults1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults1.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults2
            // 
            this.labelPlusQualityResults2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults2.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults2.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults2.Location = new System.Drawing.Point(829, 484);
            this.labelPlusQualityResults2.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults2.Name = "labelPlusQualityResults2";
            this.labelPlusQualityResults2.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults2.TabIndex = 177;
            this.labelPlusQualityResults2.Text = "+";
            this.labelPlusQualityResults2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults2.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults3
            // 
            this.labelPlusQualityResults3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults3.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults3.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults3.Location = new System.Drawing.Point(829, 528);
            this.labelPlusQualityResults3.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults3.Name = "labelPlusQualityResults3";
            this.labelPlusQualityResults3.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults3.TabIndex = 178;
            this.labelPlusQualityResults3.Text = "+";
            this.labelPlusQualityResults3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults3.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults4
            // 
            this.labelPlusQualityResults4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults4.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults4.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults4.Location = new System.Drawing.Point(829, 572);
            this.labelPlusQualityResults4.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults4.Name = "labelPlusQualityResults4";
            this.labelPlusQualityResults4.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults4.TabIndex = 179;
            this.labelPlusQualityResults4.Text = "+";
            this.labelPlusQualityResults4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults4.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults5
            // 
            this.labelPlusQualityResults5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults5.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults5.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults5.Location = new System.Drawing.Point(829, 616);
            this.labelPlusQualityResults5.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults5.Name = "labelPlusQualityResults5";
            this.labelPlusQualityResults5.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults5.TabIndex = 180;
            this.labelPlusQualityResults5.Text = "+";
            this.labelPlusQualityResults5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults5.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults6
            // 
            this.labelPlusQualityResults6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults6.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults6.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults6.Location = new System.Drawing.Point(829, 660);
            this.labelPlusQualityResults6.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults6.Name = "labelPlusQualityResults6";
            this.labelPlusQualityResults6.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults6.TabIndex = 181;
            this.labelPlusQualityResults6.Text = "+";
            this.labelPlusQualityResults6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults6.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults7
            // 
            this.labelPlusQualityResults7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults7.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults7.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults7.Location = new System.Drawing.Point(829, 704);
            this.labelPlusQualityResults7.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults7.Name = "labelPlusQualityResults7";
            this.labelPlusQualityResults7.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults7.TabIndex = 182;
            this.labelPlusQualityResults7.Text = "+";
            this.labelPlusQualityResults7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults7.UseCompatibleTextRendering = true;
            // 
            // labelPlusQualityResults8
            // 
            this.labelPlusQualityResults8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPlusQualityResults8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPlusQualityResults8.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlusQualityResults8.ForeColor = System.Drawing.Color.Gray;
            this.labelPlusQualityResults8.Location = new System.Drawing.Point(829, 748);
            this.labelPlusQualityResults8.Margin = new System.Windows.Forms.Padding(0);
            this.labelPlusQualityResults8.Name = "labelPlusQualityResults8";
            this.labelPlusQualityResults8.Size = new System.Drawing.Size(50, 41);
            this.labelPlusQualityResults8.TabIndex = 183;
            this.labelPlusQualityResults8.Text = "+";
            this.labelPlusQualityResults8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPlusQualityResults8.UseCompatibleTextRendering = true;
            // 
            // btnRailingResQualityResultsR1
            // 
            this.btnRailingResQualityResultsR1.AccessibleName = "";
            this.btnRailingResQualityResultsR1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR1.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR1.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR1.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR1.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR1.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR1.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR1, 2);
            this.btnRailingResQualityResultsR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR1.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR1.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR1.Location = new System.Drawing.Point(721, 44);
            this.btnRailingResQualityResultsR1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR1.Name = "btnRailingResQualityResultsR1";
            this.btnRailingResQualityResultsR1.Size = new System.Drawing.Size(108, 40);
            this.btnRailingResQualityResultsR1.TabIndex = 184;
            this.btnRailingResQualityResultsR1.Tag = "";
            this.btnRailingResQualityResultsR1.Text = "-";
            this.btnRailingResQualityResultsR1.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR1.UseMnemonic = false;
            this.btnRailingResQualityResultsR1.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR2
            // 
            this.btnRailingResQualityResultsR2.AccessibleName = "";
            this.btnRailingResQualityResultsR2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR2.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR2.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR2.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR2.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR2.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR2.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR2, 2);
            this.btnRailingResQualityResultsR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR2.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR2.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR2.Location = new System.Drawing.Point(721, 86);
            this.btnRailingResQualityResultsR2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR2.Name = "btnRailingResQualityResultsR2";
            this.btnRailingResQualityResultsR2.Size = new System.Drawing.Size(108, 44);
            this.btnRailingResQualityResultsR2.TabIndex = 185;
            this.btnRailingResQualityResultsR2.Text = "-";
            this.btnRailingResQualityResultsR2.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR2.UseMnemonic = false;
            this.btnRailingResQualityResultsR2.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR3
            // 
            this.btnRailingResQualityResultsR3.AccessibleName = "";
            this.btnRailingResQualityResultsR3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR3.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR3.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR3.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR3.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR3.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR3.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR3, 2);
            this.btnRailingResQualityResultsR3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR3.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR3.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR3.Location = new System.Drawing.Point(721, 132);
            this.btnRailingResQualityResultsR3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR3.Name = "btnRailingResQualityResultsR3";
            this.btnRailingResQualityResultsR3.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsR3.TabIndex = 186;
            this.btnRailingResQualityResultsR3.Text = "-";
            this.btnRailingResQualityResultsR3.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR3.UseMnemonic = false;
            this.btnRailingResQualityResultsR3.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR4
            // 
            this.btnRailingResQualityResultsR4.AccessibleName = "";
            this.btnRailingResQualityResultsR4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR4.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR4.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR4.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR4.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR4.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR4.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR4, 2);
            this.btnRailingResQualityResultsR4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR4.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR4.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR4.Location = new System.Drawing.Point(721, 176);
            this.btnRailingResQualityResultsR4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR4.Name = "btnRailingResQualityResultsR4";
            this.btnRailingResQualityResultsR4.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsR4.TabIndex = 187;
            this.btnRailingResQualityResultsR4.Text = "-";
            this.btnRailingResQualityResultsR4.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR4.UseMnemonic = false;
            this.btnRailingResQualityResultsR4.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR5
            // 
            this.btnRailingResQualityResultsR5.AccessibleName = "";
            this.btnRailingResQualityResultsR5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR5.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR5.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR5.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR5.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR5.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR5.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR5, 2);
            this.btnRailingResQualityResultsR5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR5.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR5.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR5.Location = new System.Drawing.Point(721, 220);
            this.btnRailingResQualityResultsR5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR5.Name = "btnRailingResQualityResultsR5";
            this.btnRailingResQualityResultsR5.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsR5.TabIndex = 188;
            this.btnRailingResQualityResultsR5.Text = "-";
            this.btnRailingResQualityResultsR5.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR5.UseMnemonic = false;
            this.btnRailingResQualityResultsR5.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR6
            // 
            this.btnRailingResQualityResultsR6.AccessibleName = "";
            this.btnRailingResQualityResultsR6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR6.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR6.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR6.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR6.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR6.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR6.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR6, 2);
            this.btnRailingResQualityResultsR6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR6.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR6.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR6.Location = new System.Drawing.Point(721, 264);
            this.btnRailingResQualityResultsR6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR6.Name = "btnRailingResQualityResultsR6";
            this.btnRailingResQualityResultsR6.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsR6.TabIndex = 189;
            this.btnRailingResQualityResultsR6.Text = "-";
            this.btnRailingResQualityResultsR6.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR6.UseMnemonic = false;
            this.btnRailingResQualityResultsR6.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR7
            // 
            this.btnRailingResQualityResultsR7.AccessibleName = "";
            this.btnRailingResQualityResultsR7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR7.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR7.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR7.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR7.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR7.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR7.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR7, 2);
            this.btnRailingResQualityResultsR7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR7.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR7.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR7.Location = new System.Drawing.Point(721, 308);
            this.btnRailingResQualityResultsR7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR7.Name = "btnRailingResQualityResultsR7";
            this.btnRailingResQualityResultsR7.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsR7.TabIndex = 190;
            this.btnRailingResQualityResultsR7.Text = "-";
            this.btnRailingResQualityResultsR7.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR7.UseMnemonic = false;
            this.btnRailingResQualityResultsR7.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsR8
            // 
            this.btnRailingResQualityResultsR8.AccessibleName = "";
            this.btnRailingResQualityResultsR8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.btnRailingResQualityResultsR8.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR8.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsR8.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsR8.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsR8.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsR8.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsR8, 2);
            this.btnRailingResQualityResultsR8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsR8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsR8.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsR8.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsR8.Location = new System.Drawing.Point(721, 352);
            this.btnRailingResQualityResultsR8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsR8.Name = "btnRailingResQualityResultsR8";
            this.btnRailingResQualityResultsR8.Size = new System.Drawing.Size(108, 41);
            this.btnRailingResQualityResultsR8.TabIndex = 191;
            this.btnRailingResQualityResultsR8.Text = "-";
            this.btnRailingResQualityResultsR8.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsR8.UseMnemonic = false;
            this.btnRailingResQualityResultsR8.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp1
            // 
            this.btnRailingResQualityResultsOp1.AccessibleName = "";
            this.btnRailingResQualityResultsOp1.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp1.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp1.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp1.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp1.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp1.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp1.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp1, 2);
            this.btnRailingResQualityResultsOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp1.Enabled = false;
            this.btnRailingResQualityResultsOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp1.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp1.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp1.Location = new System.Drawing.Point(721, 440);
            this.btnRailingResQualityResultsOp1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp1.Name = "btnRailingResQualityResultsOp1";
            this.btnRailingResQualityResultsOp1.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp1.TabIndex = 192;
            this.btnRailingResQualityResultsOp1.Text = "-";
            this.btnRailingResQualityResultsOp1.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp1.UseMnemonic = false;
            this.btnRailingResQualityResultsOp1.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp2
            // 
            this.btnRailingResQualityResultsOp2.AccessibleName = "";
            this.btnRailingResQualityResultsOp2.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp2.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp2.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp2.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp2.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp2.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp2.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp2, 2);
            this.btnRailingResQualityResultsOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp2.Enabled = false;
            this.btnRailingResQualityResultsOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp2.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp2.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp2.Location = new System.Drawing.Point(721, 484);
            this.btnRailingResQualityResultsOp2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp2.Name = "btnRailingResQualityResultsOp2";
            this.btnRailingResQualityResultsOp2.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp2.TabIndex = 193;
            this.btnRailingResQualityResultsOp2.Text = "-";
            this.btnRailingResQualityResultsOp2.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp2.UseMnemonic = false;
            this.btnRailingResQualityResultsOp2.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp3
            // 
            this.btnRailingResQualityResultsOp3.AccessibleName = "";
            this.btnRailingResQualityResultsOp3.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp3.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp3.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp3.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp3.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp3.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp3.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp3, 2);
            this.btnRailingResQualityResultsOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp3.Enabled = false;
            this.btnRailingResQualityResultsOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp3.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp3.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp3.Location = new System.Drawing.Point(721, 528);
            this.btnRailingResQualityResultsOp3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp3.Name = "btnRailingResQualityResultsOp3";
            this.btnRailingResQualityResultsOp3.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp3.TabIndex = 194;
            this.btnRailingResQualityResultsOp3.Text = "-";
            this.btnRailingResQualityResultsOp3.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp3.UseMnemonic = false;
            this.btnRailingResQualityResultsOp3.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp4
            // 
            this.btnRailingResQualityResultsOp4.AccessibleName = "";
            this.btnRailingResQualityResultsOp4.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp4.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp4.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp4.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp4.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp4.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp4.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp4, 2);
            this.btnRailingResQualityResultsOp4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp4.Enabled = false;
            this.btnRailingResQualityResultsOp4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp4.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp4.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp4.Location = new System.Drawing.Point(721, 572);
            this.btnRailingResQualityResultsOp4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp4.Name = "btnRailingResQualityResultsOp4";
            this.btnRailingResQualityResultsOp4.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp4.TabIndex = 195;
            this.btnRailingResQualityResultsOp4.Text = "-";
            this.btnRailingResQualityResultsOp4.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp4.UseMnemonic = false;
            this.btnRailingResQualityResultsOp4.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp5
            // 
            this.btnRailingResQualityResultsOp5.AccessibleName = "";
            this.btnRailingResQualityResultsOp5.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp5.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp5.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp5.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp5.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp5.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp5.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp5, 2);
            this.btnRailingResQualityResultsOp5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp5.Enabled = false;
            this.btnRailingResQualityResultsOp5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp5.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp5.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp5.Location = new System.Drawing.Point(721, 616);
            this.btnRailingResQualityResultsOp5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp5.Name = "btnRailingResQualityResultsOp5";
            this.btnRailingResQualityResultsOp5.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp5.TabIndex = 196;
            this.btnRailingResQualityResultsOp5.Text = "-";
            this.btnRailingResQualityResultsOp5.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp5.UseMnemonic = false;
            this.btnRailingResQualityResultsOp5.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp6
            // 
            this.btnRailingResQualityResultsOp6.AccessibleName = "";
            this.btnRailingResQualityResultsOp6.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp6.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp6.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp6.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp6.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp6.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp6.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp6, 2);
            this.btnRailingResQualityResultsOp6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp6.Enabled = false;
            this.btnRailingResQualityResultsOp6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp6.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp6.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp6.Location = new System.Drawing.Point(721, 660);
            this.btnRailingResQualityResultsOp6.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp6.Name = "btnRailingResQualityResultsOp6";
            this.btnRailingResQualityResultsOp6.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp6.TabIndex = 197;
            this.btnRailingResQualityResultsOp6.Text = "-";
            this.btnRailingResQualityResultsOp6.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp6.UseMnemonic = false;
            this.btnRailingResQualityResultsOp6.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp7
            // 
            this.btnRailingResQualityResultsOp7.AccessibleName = "";
            this.btnRailingResQualityResultsOp7.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp7.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp7.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp7.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp7.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp7.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp7.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp7, 2);
            this.btnRailingResQualityResultsOp7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp7.Enabled = false;
            this.btnRailingResQualityResultsOp7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp7.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp7.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp7.Location = new System.Drawing.Point(721, 704);
            this.btnRailingResQualityResultsOp7.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp7.Name = "btnRailingResQualityResultsOp7";
            this.btnRailingResQualityResultsOp7.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp7.TabIndex = 198;
            this.btnRailingResQualityResultsOp7.Text = "-";
            this.btnRailingResQualityResultsOp7.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp7.UseMnemonic = false;
            this.btnRailingResQualityResultsOp7.UseVisualStyleBackColor = false;
            // 
            // btnRailingResQualityResultsOp8
            // 
            this.btnRailingResQualityResultsOp8.AccessibleName = "";
            this.btnRailingResQualityResultsOp8.BackColor = System.Drawing.Color.Transparent;
            this.btnRailingResQualityResultsOp8.BorderColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp8.BorderRadiusBottomLeft = 5;
            this.btnRailingResQualityResultsOp8.BorderRadiusBottomRight = 5;
            this.btnRailingResQualityResultsOp8.BorderRadiusTopLeft = 5;
            this.btnRailingResQualityResultsOp8.BorderRadiusTopRight = 5;
            this.btnRailingResQualityResultsOp8.BorderWidth = 2F;
            this.tableLayoutQualityResults.SetColumnSpan(this.btnRailingResQualityResultsOp8, 2);
            this.btnRailingResQualityResultsOp8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRailingResQualityResultsOp8.Enabled = false;
            this.btnRailingResQualityResultsOp8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRailingResQualityResultsOp8.Font = new System.Drawing.Font("Montserrat Medium", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRailingResQualityResultsOp8.ForeColor = System.Drawing.Color.Gray;
            this.btnRailingResQualityResultsOp8.Location = new System.Drawing.Point(721, 748);
            this.btnRailingResQualityResultsOp8.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnRailingResQualityResultsOp8.Name = "btnRailingResQualityResultsOp8";
            this.btnRailingResQualityResultsOp8.Size = new System.Drawing.Size(108, 42);
            this.btnRailingResQualityResultsOp8.TabIndex = 199;
            this.btnRailingResQualityResultsOp8.Text = "-";
            this.btnRailingResQualityResultsOp8.UseCompatibleTextRendering = true;
            this.btnRailingResQualityResultsOp8.UseMnemonic = false;
            this.btnRailingResQualityResultsOp8.UseVisualStyleBackColor = false;
            // 
            // labelQualityResults
            // 
            this.labelQualityResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelQualityResults.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutQualityResults.SetColumnSpan(this.labelQualityResults, 7);
            this.labelQualityResults.Font = new System.Drawing.Font("Montserrat", 44F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQualityResults.ForeColor = System.Drawing.Color.White;
            this.labelQualityResults.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelQualityResults.Location = new System.Drawing.Point(20, 30);
            this.labelQualityResults.Margin = new System.Windows.Forms.Padding(20, 30, 0, 0);
            this.labelQualityResults.Name = "labelQualityResults";
            this.labelQualityResults.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tableLayoutQualityResults.SetRowSpan(this.labelQualityResults, 5);
            this.labelQualityResults.Size = new System.Drawing.Size(382, 190);
            this.labelQualityResults.TabIndex = 98;
            this.labelQualityResults.Text = "Quality\r\nResults";
            this.labelQualityResults.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.panelSignalQuality.Controls.Add(this.scannerRoundedButtonControl1);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapT3);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapF7);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapF3);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapFp1);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapFp2);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapFz);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapF4);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapF8);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapC4);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapCz);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapT5);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapO2);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapO1);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCap11);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapT6);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapGND);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapP4);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapP3);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapC3);
            this.panelSignalQuality.Controls.Add(this.btnElectrodeCapPz);
            this.panelSignalQuality.Font = new System.Drawing.Font("Montserrat", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSignalQuality.Location = new System.Drawing.Point(220, 685);
            this.panelSignalQuality.Margin = new System.Windows.Forms.Padding(0);
            this.panelSignalQuality.Name = "panelSignalQuality";
            this.tableLayoutPanelMain.SetRowSpan(this.panelSignalQuality, 7);
            this.panelSignalQuality.Size = new System.Drawing.Size(300, 300);
            this.panelSignalQuality.TabIndex = 97;
            // 
            // scannerRoundedButtonControl1
            // 
            this.scannerRoundedButtonControl1.BackColor = System.Drawing.Color.Gray;
            this.scannerRoundedButtonControl1.BorderColor = System.Drawing.Color.Gray;
            this.scannerRoundedButtonControl1.BorderRadiusBottomLeft = 40;
            this.scannerRoundedButtonControl1.BorderRadiusBottomRight = 40;
            this.scannerRoundedButtonControl1.BorderRadiusTopLeft = 40;
            this.scannerRoundedButtonControl1.BorderRadiusTopRight = 40;
            this.scannerRoundedButtonControl1.BorderWidth = 1F;
            this.scannerRoundedButtonControl1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.scannerRoundedButtonControl1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scannerRoundedButtonControl1.Font = new System.Drawing.Font("Montserrat Medium", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scannerRoundedButtonControl1.ForeColor = System.Drawing.Color.White;
            this.scannerRoundedButtonControl1.Location = new System.Drawing.Point(126, 181);
            this.scannerRoundedButtonControl1.Margin = new System.Windows.Forms.Padding(0);
            this.scannerRoundedButtonControl1.Name = "scannerRoundedButtonControl1";
            this.scannerRoundedButtonControl1.Size = new System.Drawing.Size(46, 38);
            this.scannerRoundedButtonControl1.TabIndex = 57;
            this.scannerRoundedButtonControl1.Text = "Ref";
            this.scannerRoundedButtonControl1.UseMnemonic = false;
            this.scannerRoundedButtonControl1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapT3
            // 
            this.btnElectrodeCapT3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapT3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT3.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapT3.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapT3.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapT3.BorderRadiusTopRight = 40;
            this.btnElectrodeCapT3.BorderWidth = 1F;
            this.btnElectrodeCapT3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapT3.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapT3.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapT3.Location = new System.Drawing.Point(-1, 138);
            this.btnElectrodeCapT3.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapT3.Name = "btnElectrodeCapT3";
            this.btnElectrodeCapT3.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapT3.TabIndex = 56;
            this.btnElectrodeCapT3.Text = "T3";
            this.btnElectrodeCapT3.UseMnemonic = false;
            this.btnElectrodeCapT3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapF7
            // 
            this.btnElectrodeCapF7.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapF7.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF7.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapF7.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapF7.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapF7.BorderRadiusTopRight = 40;
            this.btnElectrodeCapF7.BorderWidth = 1F;
            this.btnElectrodeCapF7.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF7.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapF7.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapF7.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapF7.Location = new System.Drawing.Point(1, 87);
            this.btnElectrodeCapF7.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapF7.Name = "btnElectrodeCapF7";
            this.btnElectrodeCapF7.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapF7.TabIndex = 55;
            this.btnElectrodeCapF7.Text = "F7";
            this.btnElectrodeCapF7.UseMnemonic = false;
            this.btnElectrodeCapF7.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapF3
            // 
            this.btnElectrodeCapF3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapF3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF3.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapF3.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapF3.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapF3.BorderRadiusTopRight = 40;
            this.btnElectrodeCapF3.BorderWidth = 1F;
            this.btnElectrodeCapF3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapF3.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapF3.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapF3.Location = new System.Drawing.Point(59, 78);
            this.btnElectrodeCapF3.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapF3.Name = "btnElectrodeCapF3";
            this.btnElectrodeCapF3.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapF3.TabIndex = 54;
            this.btnElectrodeCapF3.Text = "F3";
            this.btnElectrodeCapF3.UseMnemonic = false;
            this.btnElectrodeCapF3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapFp1
            // 
            this.btnElectrodeCapFp1.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFp1.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFp1.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapFp1.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapFp1.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapFp1.BorderRadiusTopRight = 40;
            this.btnElectrodeCapFp1.BorderWidth = 1F;
            this.btnElectrodeCapFp1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapFp1.Font = new System.Drawing.Font("Montserrat SemiBold", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapFp1.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapFp1.Location = new System.Drawing.Point(71, 19);
            this.btnElectrodeCapFp1.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapFp1.Name = "btnElectrodeCapFp1";
            this.btnElectrodeCapFp1.Size = new System.Drawing.Size(48, 39);
            this.btnElectrodeCapFp1.TabIndex = 53;
            this.btnElectrodeCapFp1.Text = "Fp1";
            this.btnElectrodeCapFp1.UseMnemonic = false;
            this.btnElectrodeCapFp1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapFp2
            // 
            this.btnElectrodeCapFp2.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFp2.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFp2.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapFp2.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapFp2.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapFp2.BorderRadiusTopRight = 40;
            this.btnElectrodeCapFp2.BorderWidth = 1F;
            this.btnElectrodeCapFp2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapFp2.Font = new System.Drawing.Font("Montserrat SemiBold", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapFp2.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapFp2.Location = new System.Drawing.Point(181, 20);
            this.btnElectrodeCapFp2.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapFp2.Name = "btnElectrodeCapFp2";
            this.btnElectrodeCapFp2.Size = new System.Drawing.Size(48, 39);
            this.btnElectrodeCapFp2.TabIndex = 52;
            this.btnElectrodeCapFp2.Text = "Fp2";
            this.btnElectrodeCapFp2.UseMnemonic = false;
            this.btnElectrodeCapFp2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapFz
            // 
            this.btnElectrodeCapFz.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFz.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapFz.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFz.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapFz.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapFz.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapFz.BorderRadiusTopRight = 40;
            this.btnElectrodeCapFz.BorderWidth = 1F;
            this.btnElectrodeCapFz.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFz.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapFz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapFz.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapFz.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapFz.Location = new System.Drawing.Point(127, 81);
            this.btnElectrodeCapFz.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapFz.Name = "btnElectrodeCapFz";
            this.btnElectrodeCapFz.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapFz.TabIndex = 51;
            this.btnElectrodeCapFz.Text = "Fz";
            this.btnElectrodeCapFz.UseMnemonic = false;
            this.btnElectrodeCapFz.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapF4
            // 
            this.btnElectrodeCapF4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapF4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF4.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapF4.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapF4.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapF4.BorderRadiusTopRight = 40;
            this.btnElectrodeCapF4.BorderWidth = 1F;
            this.btnElectrodeCapF4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapF4.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapF4.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapF4.Location = new System.Drawing.Point(192, 78);
            this.btnElectrodeCapF4.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapF4.Name = "btnElectrodeCapF4";
            this.btnElectrodeCapF4.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapF4.TabIndex = 50;
            this.btnElectrodeCapF4.Text = "F4";
            this.btnElectrodeCapF4.UseMnemonic = false;
            this.btnElectrodeCapF4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapF8
            // 
            this.btnElectrodeCapF8.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapF8.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF8.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapF8.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapF8.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapF8.BorderRadiusTopRight = 40;
            this.btnElectrodeCapF8.BorderWidth = 1F;
            this.btnElectrodeCapF8.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapF8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapF8.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapF8.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapF8.Location = new System.Drawing.Point(254, 87);
            this.btnElectrodeCapF8.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapF8.Name = "btnElectrodeCapF8";
            this.btnElectrodeCapF8.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapF8.TabIndex = 49;
            this.btnElectrodeCapF8.Text = "F8";
            this.btnElectrodeCapF8.UseMnemonic = false;
            this.btnElectrodeCapF8.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapC4
            // 
            this.btnElectrodeCapC4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapC4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC4.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapC4.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapC4.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapC4.BorderRadiusTopRight = 40;
            this.btnElectrodeCapC4.BorderWidth = 1F;
            this.btnElectrodeCapC4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapC4.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapC4.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapC4.Location = new System.Drawing.Point(195, 137);
            this.btnElectrodeCapC4.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapC4.Name = "btnElectrodeCapC4";
            this.btnElectrodeCapC4.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapC4.TabIndex = 48;
            this.btnElectrodeCapC4.Text = "C4";
            this.btnElectrodeCapC4.UseMnemonic = false;
            this.btnElectrodeCapC4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapCz
            // 
            this.btnElectrodeCapCz.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapCz.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapCz.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapCz.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapCz.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapCz.BorderRadiusTopRight = 40;
            this.btnElectrodeCapCz.BorderWidth = 1F;
            this.btnElectrodeCapCz.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapCz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapCz.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapCz.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapCz.Location = new System.Drawing.Point(127, 136);
            this.btnElectrodeCapCz.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapCz.Name = "btnElectrodeCapCz";
            this.btnElectrodeCapCz.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapCz.TabIndex = 47;
            this.btnElectrodeCapCz.Text = "Cz";
            this.btnElectrodeCapCz.UseMnemonic = false;
            this.btnElectrodeCapCz.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapT5
            // 
            this.btnElectrodeCapT5.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapT5.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT5.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapT5.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapT5.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapT5.BorderRadiusTopRight = 40;
            this.btnElectrodeCapT5.BorderWidth = 1F;
            this.btnElectrodeCapT5.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT5.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapT5.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapT5.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapT5.Location = new System.Drawing.Point(2, 195);
            this.btnElectrodeCapT5.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapT5.Name = "btnElectrodeCapT5";
            this.btnElectrodeCapT5.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapT5.TabIndex = 46;
            this.btnElectrodeCapT5.Text = "T5";
            this.btnElectrodeCapT5.UseMnemonic = false;
            this.btnElectrodeCapT5.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapO2
            // 
            this.btnElectrodeCapO2.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapO2.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO2.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapO2.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapO2.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapO2.BorderRadiusTopRight = 40;
            this.btnElectrodeCapO2.BorderWidth = 1F;
            this.btnElectrodeCapO2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapO2.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapO2.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapO2.Location = new System.Drawing.Point(178, 256);
            this.btnElectrodeCapO2.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapO2.Name = "btnElectrodeCapO2";
            this.btnElectrodeCapO2.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapO2.TabIndex = 45;
            this.btnElectrodeCapO2.Text = "O2";
            this.btnElectrodeCapO2.UseMnemonic = false;
            this.btnElectrodeCapO2.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapO1
            // 
            this.btnElectrodeCapO1.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapO1.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO1.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapO1.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapO1.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapO1.BorderRadiusTopRight = 40;
            this.btnElectrodeCapO1.BorderWidth = 1F;
            this.btnElectrodeCapO1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapO1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapO1.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapO1.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapO1.Location = new System.Drawing.Point(77, 256);
            this.btnElectrodeCapO1.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapO1.Name = "btnElectrodeCapO1";
            this.btnElectrodeCapO1.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapO1.TabIndex = 44;
            this.btnElectrodeCapO1.Text = "O1";
            this.btnElectrodeCapO1.UseMnemonic = false;
            this.btnElectrodeCapO1.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCap11
            // 
            this.btnElectrodeCap11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(170)))), ((int)(((byte)(225)))));
            this.btnElectrodeCap11.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeCap11.BorderRadiusBottomLeft = 60;
            this.btnElectrodeCap11.BorderRadiusBottomRight = 60;
            this.btnElectrodeCap11.BorderRadiusTopLeft = 60;
            this.btnElectrodeCap11.BorderRadiusTopRight = 60;
            this.btnElectrodeCap11.BorderWidth = 1F;
            this.btnElectrodeCap11.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeCap11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCap11.Font = new System.Drawing.Font("Montserrat SemiBold", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCap11.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeCap11.Location = new System.Drawing.Point(254, 136);
            this.btnElectrodeCap11.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCap11.Name = "btnElectrodeCap11";
            this.btnElectrodeCap11.Size = new System.Drawing.Size(46, 47);
            this.btnElectrodeCap11.TabIndex = 30;
            this.btnElectrodeCap11.Text = "T4 \r\nREF";
            this.btnElectrodeCap11.UseMnemonic = false;
            this.btnElectrodeCap11.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapT6
            // 
            this.btnElectrodeCapT6.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapT6.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT6.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapT6.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapT6.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapT6.BorderRadiusTopRight = 40;
            this.btnElectrodeCapT6.BorderWidth = 1F;
            this.btnElectrodeCapT6.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT6.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapT6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapT6.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapT6.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapT6.Location = new System.Drawing.Point(255, 194);
            this.btnElectrodeCapT6.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapT6.Name = "btnElectrodeCapT6";
            this.btnElectrodeCapT6.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapT6.TabIndex = 26;
            this.btnElectrodeCapT6.Text = "T6";
            this.btnElectrodeCapT6.UseMnemonic = false;
            this.btnElectrodeCapT6.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapGND
            // 
            this.btnElectrodeCapGND.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(170)))), ((int)(((byte)(225)))));
            this.btnElectrodeCapGND.BorderColor = System.Drawing.Color.Black;
            this.btnElectrodeCapGND.BorderRadiusBottomLeft = 50;
            this.btnElectrodeCapGND.BorderRadiusBottomRight = 50;
            this.btnElectrodeCapGND.BorderRadiusTopLeft = 50;
            this.btnElectrodeCapGND.BorderRadiusTopRight = 50;
            this.btnElectrodeCapGND.BorderWidth = 1F;
            this.btnElectrodeCapGND.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapGND.Font = new System.Drawing.Font("Montserrat SemiBold", 6.5F, System.Drawing.FontStyle.Bold);
            this.btnElectrodeCapGND.ForeColor = System.Drawing.Color.Black;
            this.btnElectrodeCapGND.Location = new System.Drawing.Point(125, 35);
            this.btnElectrodeCapGND.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapGND.Name = "btnElectrodeCapGND";
            this.btnElectrodeCapGND.Size = new System.Drawing.Size(46, 40);
            this.btnElectrodeCapGND.TabIndex = 29;
            this.btnElectrodeCapGND.Text = "GND";
            this.btnElectrodeCapGND.UseMnemonic = false;
            this.btnElectrodeCapGND.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapP4
            // 
            this.btnElectrodeCapP4.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapP4.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP4.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapP4.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapP4.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapP4.BorderRadiusTopRight = 40;
            this.btnElectrodeCapP4.BorderWidth = 1F;
            this.btnElectrodeCapP4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapP4.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapP4.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapP4.Location = new System.Drawing.Point(199, 205);
            this.btnElectrodeCapP4.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapP4.Name = "btnElectrodeCapP4";
            this.btnElectrodeCapP4.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapP4.TabIndex = 26;
            this.btnElectrodeCapP4.Text = "P4";
            this.btnElectrodeCapP4.UseMnemonic = false;
            this.btnElectrodeCapP4.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapP3
            // 
            this.btnElectrodeCapP3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapP3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP3.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapP3.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapP3.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapP3.BorderRadiusTopRight = 40;
            this.btnElectrodeCapP3.BorderWidth = 1F;
            this.btnElectrodeCapP3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapP3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapP3.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapP3.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapP3.Location = new System.Drawing.Point(59, 206);
            this.btnElectrodeCapP3.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapP3.Name = "btnElectrodeCapP3";
            this.btnElectrodeCapP3.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapP3.TabIndex = 26;
            this.btnElectrodeCapP3.Text = "P3";
            this.btnElectrodeCapP3.UseMnemonic = false;
            this.btnElectrodeCapP3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapC3
            // 
            this.btnElectrodeCapC3.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapC3.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC3.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapC3.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapC3.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapC3.BorderRadiusTopRight = 40;
            this.btnElectrodeCapC3.BorderWidth = 1F;
            this.btnElectrodeCapC3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapC3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapC3.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapC3.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapC3.Location = new System.Drawing.Point(58, 137);
            this.btnElectrodeCapC3.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapC3.Name = "btnElectrodeCapC3";
            this.btnElectrodeCapC3.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapC3.TabIndex = 26;
            this.btnElectrodeCapC3.Text = "C3";
            this.btnElectrodeCapC3.UseMnemonic = false;
            this.btnElectrodeCapC3.UseVisualStyleBackColor = false;
            // 
            // btnElectrodeCapPz
            // 
            this.btnElectrodeCapPz.BackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapPz.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnElectrodeCapPz.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapPz.BorderRadiusBottomLeft = 40;
            this.btnElectrodeCapPz.BorderRadiusBottomRight = 40;
            this.btnElectrodeCapPz.BorderRadiusTopLeft = 40;
            this.btnElectrodeCapPz.BorderRadiusTopRight = 40;
            this.btnElectrodeCapPz.BorderWidth = 1F;
            this.btnElectrodeCapPz.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapPz.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.btnElectrodeCapPz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnElectrodeCapPz.Font = new System.Drawing.Font("Montserrat SemiBold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnElectrodeCapPz.ForeColor = System.Drawing.Color.White;
            this.btnElectrodeCapPz.Location = new System.Drawing.Point(127, 226);
            this.btnElectrodeCapPz.Margin = new System.Windows.Forms.Padding(0);
            this.btnElectrodeCapPz.Name = "btnElectrodeCapPz";
            this.btnElectrodeCapPz.Size = new System.Drawing.Size(44, 40);
            this.btnElectrodeCapPz.TabIndex = 26;
            this.btnElectrodeCapPz.Text = "Pz";
            this.btnElectrodeCapPz.UseMnemonic = false;
            this.btnElectrodeCapPz.UseVisualStyleBackColor = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestR2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartRailingTestOp1)).EndInit();
            this.tabPageImpedance.ResumeLayout(false);
            this.tableLayoutImpedanceTest.ResumeLayout(false);
            this.tabPageQuality.ResumeLayout(false);
            this.tableLayoutQualityResults.ResumeLayout(false);
            this.panelSignalQuality.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl TriggerBox;
        private System.Windows.Forms.Panel panelSignalQualitySlider;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TableLayoutPanel tableLayoutRailingTest;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp5;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp2;
        private System.Windows.Forms.Label labelRailingTestInfo1;
        private System.Windows.Forms.Label labelRailingTest;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestR8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestR7;
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
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeRailingTestOp8;

        private System.Windows.Forms.Label labelEqualsIRailingTest1;
        private System.Windows.Forms.Label labelEqualsIRailingTest2;
        private System.Windows.Forms.Label labelEqualsIRailingTest4;
        private System.Windows.Forms.Label labelEqualsIRailingTest3;
        private System.Windows.Forms.Label labelEqualsIRailingTest5;
        private System.Windows.Forms.Label labelEqualsIRailingTest6;
        private System.Windows.Forms.Label labelEqualsIRailingTest7;
        private System.Windows.Forms.Label labelEqualsIRailingTest8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRailingTestOp1;
        private System.Windows.Forms.Label labelBCISignalCheck;
        public System.Windows.Forms.Button buttonBack;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonNext;
        public System.Windows.Forms.Button buttonExit;
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutQualityResults;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR1;
        private System.Windows.Forms.Label labelQualityResults;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsR4;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeQualityResultsOp8;
        private System.Windows.Forms.Label labelEqualsQualityResults1;
        private System.Windows.Forms.Label labelEqualsQualityResults2;
        private System.Windows.Forms.Label labelEqualsQualityResults4;
        private System.Windows.Forms.Label labelEqualsQualityResults3;
        private System.Windows.Forms.Label labelEqualsQualityResults5;
        private System.Windows.Forms.Label labelEqualsQualityResults6;
        private System.Windows.Forms.Label labelEqualsQualityResults7;
        private System.Windows.Forms.Label labelEqualsQualityResult8;
        private System.Windows.Forms.Label labelImpedanceQualityResults;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsR8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResQualityResultsOp8;
        private System.Windows.Forms.Label labelOptionalElectrodesQualityResults;
        private System.Windows.Forms.Label labelRailingQualityResults;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.Label labelPlusQualityResults1;
        private System.Windows.Forms.Label labelPlusQualityResults2;
        private System.Windows.Forms.Label labelPlusQualityResults3;
        private System.Windows.Forms.Label labelPlusQualityResults4;
        private System.Windows.Forms.Label labelPlusQualityResults5;
        private System.Windows.Forms.Label labelPlusQualityResults6;
        private System.Windows.Forms.Label labelPlusQualityResults7;
        private System.Windows.Forms.Label labelPlusQualityResults8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsR8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnRailingResQualityResultsOp8;
        public System.Windows.Forms.TabControl tabControlSignalQuality;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapOp2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutImpedanceTest;
        private System.Windows.Forms.Label labelImpedanceTestingState5;
        private System.Windows.Forms.Label labelImpedanceTestingState4;
        private System.Windows.Forms.Label labelImpedanceTestingState2;
        private System.Windows.Forms.Label labelImpedanceTestingState3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR1;
        private System.Windows.Forms.Label labelImpedanceTestInfo1;
        private System.Windows.Forms.Label labelImpedanceTest;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestR4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeImpedanceTestOp8;
        private System.Windows.Forms.Label labelEqualsImpedanceTest1;
        private System.Windows.Forms.Label labelEqualsImpedanceTest2;
        private System.Windows.Forms.Label labelEqualsImpedanceTest4;
        private System.Windows.Forms.Label labelEqualsImpedanceTest3;
        private System.Windows.Forms.Label labelEqualsImpedanceTestOp5;
        private System.Windows.Forms.Label labelEqualsImpedanceTestOp6;
        private System.Windows.Forms.Label labelEqualsImpedanceTestOp7;
        private System.Windows.Forms.Label labelEqualsImpedanceTestOp8;
        private System.Windows.Forms.Label labelImpedanceImpedance;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestR8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnImpedanceResImpedanceTestOp8;
        private System.Windows.Forms.Label labelOptionalElectrodesImpedanceTest;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonTestImpedance;
        private System.Windows.Forms.Label labelImpedanceTestingState1;
        private System.Windows.Forms.Label labelRailingTestInfo2;
        private System.Windows.Forms.Label labelOptionalRailingTest;
        private System.Windows.Forms.Label labelRailingTestInfo3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelQualityResultsInfo1;
        private System.Windows.Forms.Label labelQualityResultsInfo2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panelSignalQuality;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl scannerRoundedButtonControl1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapT3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapF7;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapF3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapFp1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapFp2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapFz;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapF4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapF8;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapC4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapCz;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapT5;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapO2;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapO1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCap11;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapT6;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapGND;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapP4;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapP3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapC3;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl btnElectrodeCapPz;
        private System.Windows.Forms.Label labelImpedanceTestInfo2;
        public System.Windows.Forms.TabPage tabPageRailing;
        public System.Windows.Forms.TabPage tabPageQuality;
        public System.Windows.Forms.TabPage tabPageImpedance;
        private System.Windows.Forms.WebBrowser webBrowser;
        // public UCCapLEDStatus2 ucCapLEDStatus21;
    }
}
