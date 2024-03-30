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

namespace ACAT.Extensions.BCI.Actuators.SensorUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SensorForm));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelContainer = new System.Windows.Forms.TableLayoutPanel();
            this.panelTriggerBox = new System.Windows.Forms.Panel();
            this.TriggerBox = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.labelTriggerBox = new System.Windows.Forms.Label();
            this.pictureBoxTriggerBox = new System.Windows.Forms.PictureBox();
            this.timerPlotData = new System.Windows.Forms.Timer(this.components);
            this.timerProcessData = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelTriggerBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTriggerBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 3;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1024F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 0, 3);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelContainer, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.panelTriggerBox, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 4;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 198F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 768F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
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
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(0, 994);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(1918, 84);
            this.tableLayoutPanelSpacerBottom.TabIndex = 75;
            // 
            // tableLayoutPanelContainer
            // 
            this.tableLayoutPanelContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelContainer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanelContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1026F));
            this.tableLayoutPanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelContainer.Location = new System.Drawing.Point(447, 226);
            this.tableLayoutPanelContainer.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelContainer.Name = "tableLayoutPanelContainer";
            this.tableLayoutPanelContainer.RowCount = 1;
            this.tableLayoutPanelContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 773F));
            this.tableLayoutPanelContainer.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelContainer.TabIndex = 7;
            // 
            // panelTriggerBox
            // 
            this.panelTriggerBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tableLayoutPanelMain.SetColumnSpan(this.panelTriggerBox, 3);
            this.panelTriggerBox.Controls.Add(this.TriggerBox);
            this.panelTriggerBox.Controls.Add(this.labelTriggerBox);
            this.panelTriggerBox.Controls.Add(this.pictureBoxTriggerBox);
            this.panelTriggerBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelTriggerBox.Location = new System.Drawing.Point(0, 0);
            this.panelTriggerBox.Margin = new System.Windows.Forms.Padding(0);
            this.panelTriggerBox.MinimumSize = new System.Drawing.Size(447, 198);
            this.panelTriggerBox.Name = "panelTriggerBox";
            this.panelTriggerBox.Size = new System.Drawing.Size(447, 198);
            this.panelTriggerBox.TabIndex = 6;
            // 
            // TriggerBox
            // 
            this.TriggerBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TriggerBox.BorderColor = System.Drawing.Color.DimGray;
            this.TriggerBox.BorderRadiusBottomLeft = 2;
            this.TriggerBox.BorderRadiusBottomRight = 2;
            this.TriggerBox.BorderRadiusTopLeft = 2;
            this.TriggerBox.BorderRadiusTopRight = 2;
            this.TriggerBox.BorderWidth = 4F;
            this.TriggerBox.Enabled = false;
            this.TriggerBox.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.TriggerBox.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.TriggerBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TriggerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TriggerBox.ForeColor = System.Drawing.Color.Black;
            this.TriggerBox.Location = new System.Drawing.Point(0, 38);
            this.TriggerBox.Margin = new System.Windows.Forms.Padding(0);
            this.TriggerBox.MaximumSize = new System.Drawing.Size(73, 160);
            this.TriggerBox.MinimumSize = new System.Drawing.Size(73, 160);
            this.TriggerBox.Name = "TriggerBox";
            this.TriggerBox.Size = new System.Drawing.Size(73, 160);
            this.TriggerBox.TabIndex = 14;
            this.TriggerBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.TriggerBox.UseMnemonic = false;
            this.TriggerBox.UseVisualStyleBackColor = true;
            // 
            // labelTriggerBox
            // 
            this.labelTriggerBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelTriggerBox.AutoSize = true;
            this.labelTriggerBox.Font = new System.Drawing.Font("Montserrat Medium", 11F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTriggerBox.ForeColor = System.Drawing.Color.White;
            this.labelTriggerBox.Location = new System.Drawing.Point(85, 63);
            this.labelTriggerBox.Margin = new System.Windows.Forms.Padding(0);
            this.labelTriggerBox.MaximumSize = new System.Drawing.Size(342, 26);
            this.labelTriggerBox.MinimumSize = new System.Drawing.Size(342, 26);
            this.labelTriggerBox.Name = "labelTriggerBox";
            this.labelTriggerBox.Size = new System.Drawing.Size(342, 26);
            this.labelTriggerBox.TabIndex = 1;
            this.labelTriggerBox.Text = "Position optical sensor in this box";
            // 
            // pictureBoxTriggerBox
            // 
            this.pictureBoxTriggerBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTriggerBox.Image")));
            this.pictureBoxTriggerBox.Location = new System.Drawing.Point(73, 89);
            this.pictureBoxTriggerBox.Margin = new System.Windows.Forms.Padding(73, 100, 0, 0);
            this.pictureBoxTriggerBox.MaximumSize = new System.Drawing.Size(188, 53);
            this.pictureBoxTriggerBox.MinimumSize = new System.Drawing.Size(188, 53);
            this.pictureBoxTriggerBox.Name = "pictureBoxTriggerBox";
            this.pictureBoxTriggerBox.Size = new System.Drawing.Size(188, 53);
            this.pictureBoxTriggerBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTriggerBox.TabIndex = 0;
            this.pictureBoxTriggerBox.TabStop = false;
            // 
            // timerProcessData
            // 
            this.timerProcessData.Interval = 10;
            // 
            // SensorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(1918, 1078);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "SensorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelTriggerBox.ResumeLayout(false);
            this.panelTriggerBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTriggerBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanelContainer;
        private System.Windows.Forms.Timer timerPlotData;
        public System.Windows.Forms.Panel panelTriggerBox;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl TriggerBox;
        public System.Windows.Forms.Label labelTriggerBox;
        public System.Windows.Forms.PictureBox pictureBoxTriggerBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.Timer timerProcessData;
    }
}

