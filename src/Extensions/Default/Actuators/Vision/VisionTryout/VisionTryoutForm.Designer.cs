namespace ACAT.Extensions.Default.Actuators.Vision.VisionTryout
{
    partial class VisionTryoutForm
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
            this.buttonVision = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonVision
            // 
            this.buttonVision.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonVision.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.buttonVision.FlatAppearance.BorderSize = 4;
            this.buttonVision.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonVision.Location = new System.Drawing.Point(82, 29);
            this.buttonVision.Name = "buttonVision";
            this.buttonVision.Size = new System.Drawing.Size(133, 82);
            this.buttonVision.TabIndex = 0;
            this.buttonVision.Text = "Start";
            this.buttonVision.UseVisualStyleBackColor = false;
            this.buttonVision.Click += new System.EventHandler(this.button1_Click);
            // 
            // VisionTryoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 133);
            this.Controls.Add(this.buttonVision);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VisionTryoutForm";
            this.Text = "ACAT Vision Tryout";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonVision;
    }
}

