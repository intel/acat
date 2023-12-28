namespace ACAT.Lib.Extension.Onboarding
{
    partial class UserControlFinish
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
            this.labelStartingACAT = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelStartingACAT
            // 
            this.labelStartingACAT.AutoSize = true;
            this.labelStartingACAT.Font = new System.Drawing.Font("Montserrat Black", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStartingACAT.ForeColor = System.Drawing.Color.White;
            this.labelStartingACAT.Location = new System.Drawing.Point(310, 333);
            this.labelStartingACAT.Name = "labelStartingACAT";
            this.labelStartingACAT.Size = new System.Drawing.Size(459, 66);
            this.labelStartingACAT.TabIndex = 0;
            this.labelStartingACAT.Text = "Starting ACAT....";
            // 
            // UserControlFinish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.labelStartingACAT);
            this.Name = "UserControlFinish";
            this.Size = new System.Drawing.Size(1020, 797);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStartingACAT;
    }
}
