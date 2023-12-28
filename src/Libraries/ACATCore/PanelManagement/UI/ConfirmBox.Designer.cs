
namespace ACAT.Lib.Core.PanelManagement
{
    partial class ConfirmBox
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
            this.buttonYes = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.buttonNo = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.labelPrompt, 3);
            this.labelPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPrompt.Font = new System.Drawing.Font("Montserrat", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrompt.ForeColor = System.Drawing.Color.White;
            this.labelPrompt.Location = new System.Drawing.Point(53, 0);
            this.labelPrompt.Name = "labelPrompt";
            this.tableLayoutPanel1.SetRowSpan(this.labelPrompt, 3);
            this.labelPrompt.Size = new System.Drawing.Size(422, 174);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "This is a prompt for the message box that will appear  when needed";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelPrompt, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonYes, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonNo, 3, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.74229F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.14846F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.10925F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(528, 277);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // buttonYes
            // 
            this.buttonYes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonYes.BorderColor = System.Drawing.Color.DimGray;
            this.buttonYes.BorderRadiusBottomLeft = 0;
            this.buttonYes.BorderRadiusBottomRight = 0;
            this.buttonYes.BorderRadiusTopLeft = 0;
            this.buttonYes.BorderRadiusTopRight = 0;
            this.buttonYes.BorderWidth = 0F;
            this.buttonYes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonYes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonYes.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonYes.Location = new System.Drawing.Point(80, 184);
            this.buttonYes.Margin = new System.Windows.Forms.Padding(30, 10, 0, 10);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(158, 58);
            this.buttonYes.TabIndex = 1;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseMnemonic = false;
            this.buttonYes.UseVisualStyleBackColor = false;
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // buttonNo
            // 
            this.buttonNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNo.BorderColor = System.Drawing.Color.DimGray;
            this.buttonNo.BorderRadiusBottomLeft = 0;
            this.buttonNo.BorderRadiusBottomRight = 0;
            this.buttonNo.BorderRadiusTopLeft = 0;
            this.buttonNo.BorderRadiusTopRight = 0;
            this.buttonNo.BorderWidth = 0F;
            this.buttonNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNo.Font = new System.Drawing.Font("Montserrat", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNo.Location = new System.Drawing.Point(290, 184);
            this.buttonNo.Margin = new System.Windows.Forms.Padding(0, 10, 30, 10);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(158, 58);
            this.buttonNo.TabIndex = 2;
            this.buttonNo.Text = "No";
            this.buttonNo.UseMnemonic = false;
            this.buttonNo.UseVisualStyleBackColor = false;
            this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
            // 
            // ConfirmBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(528, 277);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfirmBox";
            this.Text = "ACAT";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonYes;
        private ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonNo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}