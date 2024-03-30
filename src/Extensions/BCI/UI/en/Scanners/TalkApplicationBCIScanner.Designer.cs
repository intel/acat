namespace ACAT.Extensions.BCI.UI.Scanners
{
    partial class TalkApplicationBCIScanner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TalkApplicationBCIScanner));
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelDateTime = new System.Windows.Forms.Label();
            this.panelLEDStatus = new System.Windows.Forms.Panel();
            this.panelTextbox = new System.Windows.Forms.Panel();
            this.scannerTableLayoutSentences = new ACAT.Lib.Core.WidgetManagement.ScannerTableLayout();
            this.scannerPanelSentences = new ACAT.Lib.Core.WidgetManagement.ScannerPanel();
            this.scannerTableLayoutWordPredictions = new ACAT.Lib.Core.WidgetManagement.ScannerTableLayout();
            this.scannerPanelWordPredictions = new ACAT.Lib.Core.WidgetManagement.ScannerPanel();
            this.scannerTableLayoutKeyboard = new ACAT.Lib.Core.WidgetManagement.ScannerTableLayout();
            this.scannerPanelKeyboard = new ACAT.Lib.Core.WidgetManagement.ScannerPanel();
            this.statusStrip.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.scannerTableLayoutSentences.SuspendLayout();
            this.scannerTableLayoutWordPredictions.SuspendLayout();
            this.scannerTableLayoutKeyboard.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(179, 19);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(179, 19);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel3.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(179, 19);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.statusStrip.Location = new System.Drawing.Point(0, 889);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip.Size = new System.Drawing.Size(1561, 25);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(239)))), ((int)(((byte)(241)))));
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.mainPanel.Controls.Add(this.labelDateTime, 0, 0);
            this.mainPanel.Controls.Add(this.panelLEDStatus, 1, 0);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mainPanel.RowCount = 1;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(1561, 25);
            this.mainPanel.TabIndex = 9;
            // 
            // labelDateTime
            // 
            this.labelDateTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDateTime.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDateTime.Location = new System.Drawing.Point(7, 2);
            this.labelDateTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDateTime.Name = "labelDateTime";
            this.labelDateTime.Padding = new System.Windows.Forms.Padding(3, 2, 3, 30);
            this.labelDateTime.Size = new System.Drawing.Size(1507, 21);
            this.labelDateTime.TabIndex = 66;
            this.labelDateTime.Text = "April 10, 2020.  8:20 AM";
            this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelLEDStatus
            // 
            this.panelLEDStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLEDStatus.Location = new System.Drawing.Point(1518, 2);
            this.panelLEDStatus.Margin = new System.Windows.Forms.Padding(0);
            this.panelLEDStatus.Name = "panelLEDStatus";
            this.panelLEDStatus.Size = new System.Drawing.Size(40, 21);
            this.panelLEDStatus.TabIndex = 67;
            // 
            // panelTextbox
            // 
            this.panelTextbox.BackColor = System.Drawing.Color.Transparent;
            this.panelTextbox.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelTextbox.Location = new System.Drawing.Point(767, 357);
            this.panelTextbox.Name = "panelTextbox";
            this.panelTextbox.Padding = new System.Windows.Forms.Padding(4, 0, 0, 26);
            this.panelTextbox.Size = new System.Drawing.Size(794, 196);
            this.panelTextbox.TabIndex = 60;
            // 
            // scannerTableLayoutSentences
            // 
            this.scannerTableLayoutSentences.BackColor = System.Drawing.Color.Transparent;
            this.scannerTableLayoutSentences.ColumnCount = 1;
            this.scannerTableLayoutSentences.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerTableLayoutSentences.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.scannerTableLayoutSentences.Controls.Add(this.scannerPanelSentences, 0, 0);
            this.scannerTableLayoutSentences.Dock = System.Windows.Forms.DockStyle.Top;
            this.scannerTableLayoutSentences.Location = new System.Drawing.Point(768, 25);
            this.scannerTableLayoutSentences.Margin = new System.Windows.Forms.Padding(0);
            this.scannerTableLayoutSentences.Name = "scannerTableLayoutSentences";
            this.scannerTableLayoutSentences.RowCount = 1;
            this.scannerTableLayoutSentences.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerTableLayoutSentences.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 332F));
            this.scannerTableLayoutSentences.Size = new System.Drawing.Size(793, 332);
            this.scannerTableLayoutSentences.TabIndex = 63;
            // 
            // scannerPanelSentences
            // 
            this.scannerPanelSentences.BackColor = System.Drawing.Color.Transparent;
            this.scannerPanelSentences.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerPanelSentences.Location = new System.Drawing.Point(3, 3);
            this.scannerPanelSentences.Name = "scannerPanelSentences";
            this.scannerPanelSentences.Size = new System.Drawing.Size(787, 326);
            this.scannerPanelSentences.TabIndex = 0;
            // 
            // scannerTableLayoutWordPredictions
            // 
            this.scannerTableLayoutWordPredictions.BackColor = System.Drawing.Color.Transparent;
            this.scannerTableLayoutWordPredictions.ColumnCount = 1;
            this.scannerTableLayoutWordPredictions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerTableLayoutWordPredictions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.scannerTableLayoutWordPredictions.Controls.Add(this.scannerPanelWordPredictions, 0, 0);
            this.scannerTableLayoutWordPredictions.Dock = System.Windows.Forms.DockStyle.Left;
            this.scannerTableLayoutWordPredictions.Location = new System.Drawing.Point(0, 25);
            this.scannerTableLayoutWordPredictions.Margin = new System.Windows.Forms.Padding(0);
            this.scannerTableLayoutWordPredictions.Name = "scannerTableLayoutWordPredictions";
            this.scannerTableLayoutWordPredictions.RowCount = 1;
            this.scannerTableLayoutWordPredictions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerTableLayoutWordPredictions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 528F));
            this.scannerTableLayoutWordPredictions.Size = new System.Drawing.Size(768, 528);
            this.scannerTableLayoutWordPredictions.TabIndex = 62;
            // 
            // scannerPanelWordPredictions
            // 
            this.scannerPanelWordPredictions.BackColor = System.Drawing.Color.Transparent;
            this.scannerPanelWordPredictions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerPanelWordPredictions.Location = new System.Drawing.Point(3, 3);
            this.scannerPanelWordPredictions.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.scannerPanelWordPredictions.Name = "scannerPanelWordPredictions";
            this.scannerPanelWordPredictions.Size = new System.Drawing.Size(762, 525);
            this.scannerPanelWordPredictions.TabIndex = 0;
            // 
            // scannerTableLayoutKeyboard
            // 
            this.scannerTableLayoutKeyboard.BackColor = System.Drawing.Color.Transparent;
            this.scannerTableLayoutKeyboard.ColumnCount = 1;
            this.scannerTableLayoutKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerTableLayoutKeyboard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.scannerTableLayoutKeyboard.Controls.Add(this.scannerPanelKeyboard, 0, 0);
            this.scannerTableLayoutKeyboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scannerTableLayoutKeyboard.Location = new System.Drawing.Point(0, 553);
            this.scannerTableLayoutKeyboard.Margin = new System.Windows.Forms.Padding(0);
            this.scannerTableLayoutKeyboard.Name = "scannerTableLayoutKeyboard";
            this.scannerTableLayoutKeyboard.RowCount = 1;
            this.scannerTableLayoutKeyboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scannerTableLayoutKeyboard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 336F));
            this.scannerTableLayoutKeyboard.Size = new System.Drawing.Size(1561, 336);
            this.scannerTableLayoutKeyboard.TabIndex = 61;
            // 
            // scannerPanelKeyboard
            // 
            this.scannerPanelKeyboard.BackColor = System.Drawing.Color.Transparent;
            this.scannerPanelKeyboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scannerPanelKeyboard.Location = new System.Drawing.Point(3, 3);
            this.scannerPanelKeyboard.Name = "scannerPanelKeyboard";
            this.scannerPanelKeyboard.Size = new System.Drawing.Size(1555, 330);
            this.scannerPanelKeyboard.TabIndex = 0;
            // 
            // TalkApplicationBCIScannerNewUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(1561, 914);
            this.ControlBox = false;
            this.Controls.Add(this.panelTextbox);
            this.Controls.Add(this.scannerTableLayoutSentences);
            this.Controls.Add(this.scannerTableLayoutWordPredictions);
            this.Controls.Add(this.scannerTableLayoutKeyboard);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TalkApplicationBCIScannerNewUI";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ACAT Talk";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.scannerTableLayoutSentences.ResumeLayout(false);
            this.scannerTableLayoutWordPredictions.ResumeLayout(false);
            this.scannerTableLayoutKeyboard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Label labelDateTime;
        private System.Windows.Forms.Panel panelTextbox;
        private System.Windows.Forms.Panel panelLEDStatus;
        private Lib.Core.WidgetManagement.ScannerTableLayout scannerTableLayoutKeyboard;
        private Lib.Core.WidgetManagement.ScannerTableLayout scannerTableLayoutWordPredictions;
        private Lib.Core.WidgetManagement.ScannerTableLayout scannerTableLayoutSentences;
        private Lib.Core.WidgetManagement.ScannerPanel scannerPanelSentences;
        private Lib.Core.WidgetManagement.ScannerPanel scannerPanelWordPredictions;
        private Lib.Core.WidgetManagement.ScannerPanel scannerPanelKeyboard;
    }
}