////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorUsbDongle.designer.cs
//
// User control which displays information on errors related to connecting
// to the BCI board usb dongle which streams data from the BCI board
// through bluetooth
//
////////////////////////////////////////////////////////////////////////////
using ACAT.Core.WidgetManagement;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{

    /// <summary>
    /// User control which displays information on errors related to connecting to the BCI board
    /// usb dongle which streams data from the BCI board through bluetooth
    /// </summary>
    partial class UserControlErrorBluetoothDisconnected
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExit_userControlErrorBluetoothDisconnected = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSpacerTop = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonNext_userControlErrorBluetoothDisconnected = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.listViewDevices = new ACAT.Core.PanelManagement.ListBoxUserControl();
            this.tableLayoutPanelMain.SuspendLayout();
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
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.90234F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.61328F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.41406F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.tableLayoutPanelMain.Controls.Add(this.label1, 1, 2);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit_userControlErrorBluetoothDisconnected, 1, 11);
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 1, 12);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerTop, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.buttonNext_userControlErrorBluetoothDisconnected, 3, 11);
            this.tableLayoutPanelMain.Controls.Add(this.label3, 2, 5);
            this.tableLayoutPanelMain.Controls.Add(this.listViewDevices, 2, 6);
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
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelMain.TabIndex = 9;
            // 
            // buttonExit_userControlErrorBluetoothDisconnected
            // 
            this.buttonExit_userControlErrorBluetoothDisconnected.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonExit_userControlErrorBluetoothDisconnected.AutoSize = true;
            this.buttonExit_userControlErrorBluetoothDisconnected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit_userControlErrorBluetoothDisconnected.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit_userControlErrorBluetoothDisconnected.FlatAppearance.BorderSize = 0;
            this.buttonExit_userControlErrorBluetoothDisconnected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit_userControlErrorBluetoothDisconnected.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExit_userControlErrorBluetoothDisconnected.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit_userControlErrorBluetoothDisconnected.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit_userControlErrorBluetoothDisconnected.Location = new System.Drawing.Point(112, 657);
            this.buttonExit_userControlErrorBluetoothDisconnected.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit_userControlErrorBluetoothDisconnected.Name = "buttonExit_userControlErrorBluetoothDisconnected";
            this.buttonExit_userControlErrorBluetoothDisconnected.Size = new System.Drawing.Size(128, 58);
            this.buttonExit_userControlErrorBluetoothDisconnected.TabIndex = 76;
            this.buttonExit_userControlErrorBluetoothDisconnected.Text = "Exit";
            this.buttonExit_userControlErrorBluetoothDisconnected.UseCompatibleTextRendering = true;
            this.buttonExit_userControlErrorBluetoothDisconnected.UseVisualStyleBackColor = false;
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
            this.labelTitle.Size = new System.Drawing.Size(798, 86);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "GTec Unicorn Pairing";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.UseCompatibleTextRendering = true;
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerBottom, 3);
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(112, 717);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(798, 51);
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
            this.tableLayoutPanelSpacerTop.Size = new System.Drawing.Size(798, 47);
            this.tableLayoutPanelSpacerTop.TabIndex = 73;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Montserrat", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(370, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(287, 47);
            this.label3.TabIndex = 90;
            this.label3.Text = "Available Bluetooth Devices";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.tableLayoutPanelMain.SetColumnSpan(this.label1, 3);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(292, 133);
            this.label1.Margin = new System.Windows.Forms.Padding(180, 0, 180, 0);
            this.label1.Name = "label1";
            this.tableLayoutPanelMain.SetRowSpan(this.label1, 2);
            this.label1.Size = new System.Drawing.Size(438, 81);
            this.label1.TabIndex = 0;
            this.label1.Text = "To ensure optimal data acquisition from the GTEC device, please select the CSR851" +
    "0 A10 Bluetooth adapter from Cambridge Silicon Radio Ltd., as it is the recommen" +
    "ded model.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonNext_userControlErrorBluetoothDisconnected
            // 
            this.buttonNext_userControlErrorBluetoothDisconnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext_userControlErrorBluetoothDisconnected.AutoSize = true;
            this.buttonNext_userControlErrorBluetoothDisconnected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNext_userControlErrorBluetoothDisconnected.BorderColor = System.Drawing.Color.Black;
            this.buttonNext_userControlErrorBluetoothDisconnected.BorderRadiusBottomLeft = 0;
            this.buttonNext_userControlErrorBluetoothDisconnected.BorderRadiusBottomRight = 0;
            this.buttonNext_userControlErrorBluetoothDisconnected.BorderRadiusTopLeft = 0;
            this.buttonNext_userControlErrorBluetoothDisconnected.BorderRadiusTopRight = 0;
            this.buttonNext_userControlErrorBluetoothDisconnected.BorderWidth = 2F;
            this.buttonNext_userControlErrorBluetoothDisconnected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext_userControlErrorBluetoothDisconnected.Font = new System.Drawing.Font("Montserrat Medium", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext_userControlErrorBluetoothDisconnected.Location = new System.Drawing.Point(798, 657);
            this.buttonNext_userControlErrorBluetoothDisconnected.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNext_userControlErrorBluetoothDisconnected.Name = "buttonNext_userControlErrorBluetoothDisconnected";
            this.buttonNext_userControlErrorBluetoothDisconnected.Size = new System.Drawing.Size(112, 48);
            this.buttonNext_userControlErrorBluetoothDisconnected.TabIndex = 92;
            this.buttonNext_userControlErrorBluetoothDisconnected.Text = "Next";
            this.buttonNext_userControlErrorBluetoothDisconnected.UseCompatibleTextRendering = true;
            this.buttonNext_userControlErrorBluetoothDisconnected.UseMnemonic = false;
            this.buttonNext_userControlErrorBluetoothDisconnected.UseVisualStyleBackColor = false;
            // 
            // listViewDevices
            // 
            this.listViewDevices.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.listViewDevices.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDevices.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listViewDevices.Font = new System.Drawing.Font("Montserrat", 21.75F);
            this.listViewDevices.ForeColor = System.Drawing.Color.White;
            this.listViewDevices.FormattingEnabled = true;
            this.listViewDevices.Location = new System.Drawing.Point(370, 298);
            this.listViewDevices.Name = "listViewDevices";
            this.tableLayoutPanelMain.SetRowSpan(this.listViewDevices, 3);
            this.listViewDevices.Size = new System.Drawing.Size(287, 275);
            this.listViewDevices.TabIndex = 0;
            // 
            // UserControlErrorBluetoothDisconnected
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlErrorBluetoothDisconnected";
            this.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerTop;
        public System.Windows.Forms.Button buttonExit_userControlErrorBluetoothDisconnected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        public ScannerRoundedButtonControl buttonNext_userControlErrorBluetoothDisconnected;
        private Core.PanelManagement.ListBoxUserControl listViewDevices;
    }
}
