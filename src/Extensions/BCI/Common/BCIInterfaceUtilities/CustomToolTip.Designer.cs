namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    partial class CustomToolTip
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTooltip = new System.Windows.Forms.Label();
            this.scannerRoundedButtonControl1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82F));
            this.tableLayoutPanel1.Controls.Add(this.labelTooltip, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.scannerRoundedButtonControl1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(476, 193);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelTooltip
            // 
            this.labelTooltip.AutoSize = true;
            this.labelTooltip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTooltip.Font = new System.Drawing.Font("Montserrat SemiBold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTooltip.ForeColor = System.Drawing.Color.White;
            this.labelTooltip.Location = new System.Drawing.Point(89, 0);
            this.labelTooltip.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTooltip.Name = "labelTooltip";
            this.labelTooltip.Size = new System.Drawing.Size(383, 193);
            this.labelTooltip.TabIndex = 0;
            this.labelTooltip.Text = "Hint tooltip";
            this.labelTooltip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // scannerRoundedButtonControl1
            // 
            this.scannerRoundedButtonControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.scannerRoundedButtonControl1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.scannerRoundedButtonControl1.BorderRadiusBottomLeft = 90;
            this.scannerRoundedButtonControl1.BorderRadiusBottomRight = 90;
            this.scannerRoundedButtonControl1.BorderRadiusTopLeft = 90;
            this.scannerRoundedButtonControl1.BorderRadiusTopRight = 90;
            this.scannerRoundedButtonControl1.BorderWidth = 5F;
            this.scannerRoundedButtonControl1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scannerRoundedButtonControl1.Font = new System.Drawing.Font("Montserrat SemiBold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scannerRoundedButtonControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.scannerRoundedButtonControl1.Location = new System.Drawing.Point(16, 73);
            this.scannerRoundedButtonControl1.Margin = new System.Windows.Forms.Padding(0);
            this.scannerRoundedButtonControl1.Name = "scannerRoundedButtonControl1";
            this.scannerRoundedButtonControl1.Size = new System.Drawing.Size(53, 47);
            this.scannerRoundedButtonControl1.TabIndex = 1;
            this.scannerRoundedButtonControl1.Text = "i";
            this.scannerRoundedButtonControl1.UseMnemonic = false;
            this.scannerRoundedButtonControl1.UseVisualStyleBackColor = true;
            // 
            // CustomToolTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(484, 201);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CustomToolTip";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "CustomToolTip";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelTooltip;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl scannerRoundedButtonControl1;
    }
}