
namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    partial class ConfirmBoxThreeOption
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
            this.buttonOp3 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonOp1 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonOp2 = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelPrompt, 5);
            this.labelPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPrompt.Font = new System.Drawing.Font("Montserrat Medium", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrompt.ForeColor = System.Drawing.Color.White;
            this.labelPrompt.Location = new System.Drawing.Point(44, 90);
            this.labelPrompt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPrompt.Name = "labelPrompt";
            this.tableLayoutPanel1.SetRowSpan(this.labelPrompt, 2);
            this.labelPrompt.Size = new System.Drawing.Size(994, 138);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "This is a prompt for the message box that will appear  when needed";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutPanel1.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonOp3, 5, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelPrompt, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonOp1, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonOp2, 3, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1095, 374);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // labelTitle
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelTitle, 5);
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Montserrat Black", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(44, 20);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(994, 70);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "This is a prompt for title";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOp3
            // 
            this.buttonOp3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOp3.BorderColor = System.Drawing.Color.Transparent;
            this.buttonOp3.BorderRadiusBottomLeft = 0;
            this.buttonOp3.BorderRadiusBottomRight = 0;
            this.buttonOp3.BorderRadiusTopLeft = 0;
            this.buttonOp3.BorderRadiusTopRight = 0;
            this.buttonOp3.BorderWidth = 0F;
            this.buttonOp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOp3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp3.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp3.ForeColor = System.Drawing.Color.Black;
            this.buttonOp3.Location = new System.Drawing.Point(746, 266);
            this.buttonOp3.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOp3.Name = "buttonOp3";
            this.buttonOp3.Size = new System.Drawing.Size(292, 73);
            this.buttonOp3.TabIndex = 3;
            this.buttonOp3.Text = "Op3";
            this.buttonOp3.UseMnemonic = false;
            this.buttonOp3.UseVisualStyleBackColor = false;
            this.buttonOp3.Click += new System.EventHandler(this.buttonOp3_Click);
            // 
            // buttonOp1
            // 
            this.buttonOp1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonOp1.BorderColor = System.Drawing.Color.Transparent;
            this.buttonOp1.BorderRadiusBottomLeft = 0;
            this.buttonOp1.BorderRadiusBottomRight = 0;
            this.buttonOp1.BorderRadiusTopLeft = 0;
            this.buttonOp1.BorderRadiusTopRight = 0;
            this.buttonOp1.BorderWidth = 0F;
            this.buttonOp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOp1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp1.Font = new System.Drawing.Font("Montserrat Medium", 17F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp1.ForeColor = System.Drawing.Color.Silver;
            this.buttonOp1.Location = new System.Drawing.Point(44, 266);
            this.buttonOp1.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOp1.Name = "buttonOp1";
            this.buttonOp1.Size = new System.Drawing.Size(292, 73);
            this.buttonOp1.TabIndex = 1;
            this.buttonOp1.Text = "Op1";
            this.buttonOp1.UseMnemonic = false;
            this.buttonOp1.UseVisualStyleBackColor = false;
            this.buttonOp1.Click += new System.EventHandler(this.buttonOp1_Click);
            // 
            // buttonOp2
            // 
            this.buttonOp2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOp2.BorderColor = System.Drawing.Color.Transparent;
            this.buttonOp2.BorderRadiusBottomLeft = 0;
            this.buttonOp2.BorderRadiusBottomRight = 0;
            this.buttonOp2.BorderRadiusTopLeft = 0;
            this.buttonOp2.BorderRadiusTopRight = 0;
            this.buttonOp2.BorderWidth = 0F;
            this.buttonOp2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonOp2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOp2.Font = new System.Drawing.Font("Montserrat", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOp2.ForeColor = System.Drawing.Color.Black;
            this.buttonOp2.Location = new System.Drawing.Point(395, 266);
            this.buttonOp2.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOp2.Name = "buttonOp2";
            this.buttonOp2.Size = new System.Drawing.Size(292, 73);
            this.buttonOp2.TabIndex = 2;
            this.buttonOp2.Text = "Op2";
            this.buttonOp2.UseMnemonic = false;
            this.buttonOp2.UseVisualStyleBackColor = false;
            this.buttonOp2.Click += new System.EventHandler(this.buttonOp2_Click);
            // 
            // ConfirmBoxThreeOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(1095, 374);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfirmBoxThreeOption";
            this.Text = "ACAT";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonOp1;
        private ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonOp2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonOp3;
        private System.Windows.Forms.Label labelTitle;
    }
}