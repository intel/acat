using ACAT.Core.WidgetManagement;

namespace ACAT.Core.PanelManagement
{
    partial class ConfirmBoxTwoOption
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
            this.labelPrompt = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonOp3 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonOp1 = new ACAT.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelPrompt, 5);
            this.labelPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPrompt.Font = new System.Drawing.Font("Montserrat Medium", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrompt.ForeColor = System.Drawing.Color.White;
            this.labelPrompt.Location = new System.Drawing.Point(33, 73);
            this.labelPrompt.Name = "labelPrompt";
            this.tableLayoutPanel1.SetRowSpan(this.labelPrompt, 2);
            this.labelPrompt.Size = new System.Drawing.Size(745, 112);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "This is a prompt for the message box that will appear when needed";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonOp3, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelPrompt, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonOp1, 5, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(821, 304);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // labelTitle
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelTitle, 5);
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Montserrat Black", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(33, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(745, 57);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "This is a prompt for title";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOp3
            // 
            this.buttonOp3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonOp3.BorderColor = System.Drawing.Color.Transparent;
            this.buttonOp3.BorderRadiusBottomLeft = 0;
            this.buttonOp3.BorderRadiusBottomRight = 0;
            this.buttonOp3.BorderRadiusTopLeft = 0;
            this.buttonOp3.BorderRadiusTopRight = 0;
            this.buttonOp3.BorderWidth = 0F;
            this.buttonOp3.DialogResult = System.Windows.Forms.DialogResult.No;
            this.buttonOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp3.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp3.ForeColor = System.Drawing.Color.Silver;
            this.buttonOp3.Location = new System.Drawing.Point(33, 216);
            this.buttonOp3.Name = "buttonOp3";
            this.buttonOp3.Size = new System.Drawing.Size(219, 60);
            this.buttonOp3.TabIndex = 3;
            this.buttonOp3.Text = "Op3";
            this.buttonOp3.UseMnemonic = false;
            this.buttonOp3.UseVisualStyleBackColor = false;
            this.buttonOp3.Click += new System.EventHandler(this.buttonOp3_Click);
            // 
            // buttonOp1
            // 
            this.buttonOp1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOp1.BorderColor = System.Drawing.Color.Transparent;
            this.buttonOp1.BorderRadiusBottomLeft = 0;
            this.buttonOp1.BorderRadiusBottomRight = 0;
            this.buttonOp1.BorderRadiusTopLeft = 0;
            this.buttonOp1.BorderRadiusTopRight = 0;
            this.buttonOp1.BorderWidth = 0F;
            this.buttonOp1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.buttonOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp1.Font = new System.Drawing.Font("Montserrat Medium", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp1.ForeColor = System.Drawing.Color.Black;
            this.buttonOp1.Location = new System.Drawing.Point(559, 216);
            this.buttonOp1.Name = "buttonOp1";
            this.buttonOp1.Size = new System.Drawing.Size(219, 60);
            this.buttonOp1.TabIndex = 1;
            this.buttonOp1.Text = "Op1";
            this.buttonOp1.UseMnemonic = false;
            this.buttonOp1.UseVisualStyleBackColor = false;
            this.buttonOp1.Click += new System.EventHandler(this.buttonOp1_Click);
            // 
            // ConfirmBoxTwoOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(821, 304);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfirmBoxTwoOption";
            this.Text = "ACAT";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private ScannerRoundedButtonControl buttonOp1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScannerRoundedButtonControl buttonOp3;
        private System.Windows.Forms.Label labelTitle;
    }
}