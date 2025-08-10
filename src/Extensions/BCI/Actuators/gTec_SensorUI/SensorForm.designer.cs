////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SensorForm.designer.cs
//
// Main form / UI for BCI Onboarding and Signal Check
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Main user control / UI for BCI Onboarding
    /// </summary>
    partial class SensorForm
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelContainer = new System.Windows.Forms.TableLayoutPanel();
            this.panelTriggerBox = new System.Windows.Forms.Panel();
            this.timerPlotData = new System.Windows.Forms.Timer(this.components);
            this.timerProcessData = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.AutoSize = true;
            this.tableLayoutPanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelMain.ColumnCount = 3;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelContainer, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.panelTriggerBox, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1918, 1078);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerBottom, 5);
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Cursor = System.Windows.Forms.Cursors.Default;
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(0, 969);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(1918, 109);
            this.tableLayoutPanelSpacerBottom.TabIndex = 75;
            // 
            // tableLayoutPanelContainer
            // 
            this.tableLayoutPanelContainer.AutoSize = true;
            this.tableLayoutPanelContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelContainer.BackColor = System.Drawing.Color.Red;
            this.tableLayoutPanelContainer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanelContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1026F));
            this.tableLayoutPanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelContainer.Location = new System.Drawing.Point(382, 214);
            this.tableLayoutPanelContainer.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelContainer.Name = "tableLayoutPanelContainer";
            this.tableLayoutPanelContainer.RowCount = 1;
            this.tableLayoutPanelContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 773F));
            this.tableLayoutPanelContainer.Size = new System.Drawing.Size(3068, 1724);
            this.tableLayoutPanelContainer.TabIndex = 7;
            // 
            // panelTriggerBox
            // 
            this.panelTriggerBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tableLayoutPanelMain.SetColumnSpan(this.panelTriggerBox, 3);
            this.panelTriggerBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelTriggerBox.Location = new System.Drawing.Point(0, 0);
            this.panelTriggerBox.Margin = new System.Windows.Forms.Padding(0);
            this.panelTriggerBox.MinimumSize = new System.Drawing.Size(447, 198);
            this.panelTriggerBox.Name = "panelTriggerBox";
            this.panelTriggerBox.Size = new System.Drawing.Size(447, 198);
            this.panelTriggerBox.TabIndex = 6;
            // 
            // timerProcessData
            // 
            this.timerProcessData.Interval = 10;
            // 
            // SensorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(1918, 1078);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "SensorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanelContainer;
        private System.Windows.Forms.Timer timerPlotData;
        public System.Windows.Forms.Panel panelTriggerBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.Timer timerProcessData;
    }
}

