namespace ACAT.Extensions.Default.Actuators.Vision.VisionActuator
{
    partial class CameraStatusForm
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
            this.textBoxPrompt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxPrompt
            // 
            this.textBoxPrompt.BackColor = System.Drawing.Color.NavajoWhite;
            this.textBoxPrompt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPrompt.Location = new System.Drawing.Point(22, 35);
            this.textBoxPrompt.Multiline = true;
            this.textBoxPrompt.Name = "textBoxPrompt";
            this.textBoxPrompt.ReadOnly = true;
            this.textBoxPrompt.Size = new System.Drawing.Size(378, 47);
            this.textBoxPrompt.TabIndex = 0;
            this.textBoxPrompt.TabStop = false;
            this.textBoxPrompt.Text = "Initializing Camera...";
            this.textBoxPrompt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CameraStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.NavajoWhite;
            this.ClientSize = new System.Drawing.Size(430, 106);
            this.Controls.Add(this.textBoxPrompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CameraStatusForm";
            this.Text = "ACAT Vision";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPrompt;
    }
}