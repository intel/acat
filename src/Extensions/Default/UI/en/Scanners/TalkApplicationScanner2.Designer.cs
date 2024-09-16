namespace ACAT.Extensions.Default.UI.Scanners
{
    partial class TalkApplicationScanner2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TalkApplicationScanner2));
            this.ScannerBorder = new System.Windows.Forms.TableLayoutPanel();
            this.panelKeyboard = new System.Windows.Forms.Panel();
            this.tableLayoutTop = new System.Windows.Forms.TableLayoutPanel();
            this.panelWordPrediction = new System.Windows.Forms.Panel();
            this.panelSentencePrediction = new System.Windows.Forms.Panel();
            this.panelTextBox = new System.Windows.Forms.Panel();
            this.labelCurrentTypingMode = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ScannerBorder.SuspendLayout();
            this.tableLayoutTop.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScannerBorder
            // 
            this.ScannerBorder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ScannerBorder.ColumnCount = 1;
            this.ScannerBorder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ScannerBorder.Controls.Add(this.panelKeyboard, 0, 1);
            this.ScannerBorder.Controls.Add(this.tableLayoutTop, 0, 0);
            this.ScannerBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScannerBorder.Location = new System.Drawing.Point(0, 0);
            this.ScannerBorder.Margin = new System.Windows.Forms.Padding(4);
            this.ScannerBorder.Name = "ScannerBorder";
            this.ScannerBorder.RowCount = 2;
            this.ScannerBorder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.ScannerBorder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.ScannerBorder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ScannerBorder.Size = new System.Drawing.Size(1184, 996);
            this.ScannerBorder.TabIndex = 0;
            // 
            // panelKeyboard
            // 
            this.panelKeyboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelKeyboard.Location = new System.Drawing.Point(0, 647);
            this.panelKeyboard.Margin = new System.Windows.Forms.Padding(0);
            this.panelKeyboard.Name = "panelKeyboard";
            this.panelKeyboard.Size = new System.Drawing.Size(1184, 349);
            this.panelKeyboard.TabIndex = 5;
            // 
            // tableLayoutTop
            // 
            this.tableLayoutTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutTop.ColumnCount = 2;
            this.tableLayoutTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutTop.Controls.Add(this.panelWordPrediction, 0, 2);
            this.tableLayoutTop.Controls.Add(this.panelSentencePrediction, 1, 0);
            this.tableLayoutTop.Controls.Add(this.panelTextBox, 1, 5);
            this.tableLayoutTop.Controls.Add(this.labelCurrentTypingMode, 0, 0);
            this.tableLayoutTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutTop.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutTop.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutTop.Name = "tableLayoutTop";
            this.tableLayoutTop.RowCount = 7;
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutTop.Size = new System.Drawing.Size(1184, 647);
            this.tableLayoutTop.TabIndex = 6;
            // 
            // panelWordPrediction
            // 
            this.panelWordPrediction.BackColor = System.Drawing.Color.Black;
            this.panelWordPrediction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWordPrediction.Location = new System.Drawing.Point(0, 184);
            this.panelWordPrediction.Margin = new System.Windows.Forms.Padding(0);
            this.panelWordPrediction.Name = "panelWordPrediction";
            this.tableLayoutTop.SetRowSpan(this.panelWordPrediction, 5);
            this.panelWordPrediction.Size = new System.Drawing.Size(592, 463);
            this.panelWordPrediction.TabIndex = 0;
            // 
            // panelSentencePrediction
            // 
            this.panelSentencePrediction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSentencePrediction.Location = new System.Drawing.Point(592, 0);
            this.panelSentencePrediction.Margin = new System.Windows.Forms.Padding(0);
            this.panelSentencePrediction.Name = "panelSentencePrediction";
            this.tableLayoutTop.SetRowSpan(this.panelSentencePrediction, 5);
            this.panelSentencePrediction.Size = new System.Drawing.Size(592, 460);
            this.panelSentencePrediction.TabIndex = 2;
            // 
            // panelTextBox
            // 
            this.panelTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.panelTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTextBox.Location = new System.Drawing.Point(619, 472);
            this.panelTextBox.Margin = new System.Windows.Forms.Padding(27, 12, 10, 6);
            this.panelTextBox.Name = "panelTextBox";
            this.tableLayoutTop.SetRowSpan(this.panelTextBox, 2);
            this.panelTextBox.Size = new System.Drawing.Size(555, 169);
            this.panelTextBox.TabIndex = 3;
            // 
            // labelCurrentTypingMode
            // 
            this.labelCurrentTypingMode.AutoSize = true;
            this.labelCurrentTypingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCurrentTypingMode.Font = new System.Drawing.Font("Montserrat Black", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentTypingMode.ForeColor = System.Drawing.Color.Gainsboro;
            this.labelCurrentTypingMode.Location = new System.Drawing.Point(4, 0);
            this.labelCurrentTypingMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCurrentTypingMode.Name = "labelCurrentTypingMode";
            this.tableLayoutTop.SetRowSpan(this.labelCurrentTypingMode, 2);
            this.labelCurrentTypingMode.Size = new System.Drawing.Size(584, 184);
            this.labelCurrentTypingMode.TabIndex = 4;
            this.labelCurrentTypingMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.statusStrip.Location = new System.Drawing.Point(0, 996);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel3.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // TalkApplicationScanner2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1184, 1018);
            this.Controls.Add(this.ScannerBorder);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TalkApplicationScanner2";
            this.Text = "Talk App";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ScannerBorder.ResumeLayout(false);
            this.tableLayoutTop.ResumeLayout(false);
            this.tableLayoutTop.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel ScannerBorder;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.Panel panelKeyboard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutTop;
        private System.Windows.Forms.Panel panelWordPrediction;
        private System.Windows.Forms.Panel panelSentencePrediction;
        private System.Windows.Forms.Panel panelTextBox;
        private System.Windows.Forms.Label labelCurrentTypingMode;
    }
}