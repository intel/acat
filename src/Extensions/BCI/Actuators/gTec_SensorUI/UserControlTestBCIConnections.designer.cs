////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlTestBCIConnections.designer.cs
//
// Displays "BCI Connecting..." gif while user waits for results from BCI device tests
//
////////////////////////////////////////////////////////////////////////////

using ACAT.UserControls;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Displays "BCI Connecting..." gif while user waits for results from BCI device tests
    /// </summary>
    partial class UserControlTestBCIConnections
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
            this.tableLayoutPanelSpacerTop = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExit_userControlTestBCIConnections = new System.Windows.Forms.Button();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ScrollingText = new ACAT.UserControls.ScrollingText();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.26531F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.31641F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.09766F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.92578F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.26531F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerTop, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit_userControlTestBCIConnections, 1, 5);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 0, 6);
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel1, 2, 3);
            this.tableLayoutPanelMain.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 7;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.565657F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.17172F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.565657F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.39394F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.565657F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.17172F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.565657F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelMain.TabIndex = 9;
            this.tableLayoutPanelMain.UseWaitCursor = true;
            // 
            // tableLayoutPanelSpacerTop
            // 
            this.tableLayoutPanelSpacerTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerTop.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerTop, 5);
            this.tableLayoutPanelSpacerTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSpacerTop.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelSpacerTop.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerTop.Name = "tableLayoutPanelSpacerTop";
            this.tableLayoutPanelSpacerTop.RowCount = 1;
            this.tableLayoutPanelSpacerTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Size = new System.Drawing.Size(1024, 50);
            this.tableLayoutPanelSpacerTop.TabIndex = 73;
            this.tableLayoutPanelSpacerTop.UseWaitCursor = true;
            // 
            // buttonExit_userControlTestBCIConnections
            // 
            this.buttonExit_userControlTestBCIConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExit_userControlTestBCIConnections.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonExit_userControlTestBCIConnections, 2);
            this.buttonExit_userControlTestBCIConnections.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.buttonExit_userControlTestBCIConnections.FlatAppearance.BorderSize = 0;
            this.buttonExit_userControlTestBCIConnections.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit_userControlTestBCIConnections.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExit_userControlTestBCIConnections.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit_userControlTestBCIConnections.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit_userControlTestBCIConnections.Location = new System.Drawing.Point(136, 656);
            this.buttonExit_userControlTestBCIConnections.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit_userControlTestBCIConnections.Name = "buttonExit_userControlTestBCIConnections";
            this.buttonExit_userControlTestBCIConnections.Size = new System.Drawing.Size(149, 58);
            this.buttonExit_userControlTestBCIConnections.TabIndex = 77;
            this.buttonExit_userControlTestBCIConnections.Text = "Exit";
            this.buttonExit_userControlTestBCIConnections.UseCompatibleTextRendering = true;
            this.buttonExit_userControlTestBCIConnections.UseVisualStyleBackColor = false;
            this.buttonExit_userControlTestBCIConnections.UseWaitCursor = true;
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerBottom, 5);
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(0, 714);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(1024, 54);
            this.tableLayoutPanelSpacerBottom.TabIndex = 74;
            this.tableLayoutPanelSpacerBottom.UseWaitCursor = true;
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelTitle, 3);
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Montserrat", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(136, 50);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(751, 131);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "Brain Computer Interface Setup";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.UseCompatibleTextRendering = true;
            this.labelTitle.UseWaitCursor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ScrollingText, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(388, 234);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(251, 296);
            this.tableLayoutPanel1.TabIndex = 78;
            this.tableLayoutPanel1.UseWaitCursor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Montserrat Light", 72F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 222);
            this.label1.TabIndex = 0;
            this.label1.Text = "BCI";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.UseWaitCursor = true;
            // 
            // ScrollingText
            // 
            this.ScrollingText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ScrollingText.DisplayText = "Initializing BCI...";
            this.ScrollingText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScrollingText.Font = new System.Drawing.Font("Montserrat", 30F);
            this.ScrollingText.ForeColor = System.Drawing.Color.White;
            this.ScrollingText.Location = new System.Drawing.Point(3, 225);
            this.ScrollingText.Name = "ScrollingText";
            this.ScrollingText.ScrollSpeed = 2;
            this.ScrollingText.Size = new System.Drawing.Size(245, 68);
            this.ScrollingText.TabIndex = 1;
            this.ScrollingText.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 0;
            // 
            // UserControlTestBCIConnections
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlTestBCIConnections";
            this.Size = new System.Drawing.Size(1024, 768);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.Label labelTitle;
        public System.Windows.Forms.Button buttonExit_userControlTestBCIConnections;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ScrollingText ScrollingText;
    }
}
