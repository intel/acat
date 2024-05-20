namespace ConvAssistTest
{
    partial class ShowDialogHistoryForm
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
            this.buttonClose = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonClose.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.buttonClose.BorderRadiusBottomLeft = 5;
            this.buttonClose.BorderRadiusBottomRight = 5;
            this.buttonClose.BorderRadiusTopLeft = 5;
            this.buttonClose.BorderRadiusTopRight = 5;
            this.buttonClose.BorderWidth = 0F;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(312, 762);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(140, 63);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseMnemonic = false;
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(28, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(740, 677);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "";
            // 
            // ShowDialogHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(800, 845);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonClose);
            this.Name = "ShowDialogHistoryForm";
            this.Text = "History";
            this.ResumeLayout(false);

        }

        #endregion
        private ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonClose;
        private System.Windows.Forms.RichTextBox textBox1;
    }
}