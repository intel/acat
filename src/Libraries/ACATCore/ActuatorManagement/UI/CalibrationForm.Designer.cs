namespace ACAT.Lib.Core.ActuatorManagement
{
    partial class CalibrationForm
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
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.labelTimePrompt = new System.Windows.Forms.Label();
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.BorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.labelPrompt.BackColor = System.Drawing.Color.NavajoWhite;
            this.labelPrompt.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrompt.ForeColor = System.Drawing.Color.Black;
            this.labelPrompt.Location = new System.Drawing.Point(12, 10);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(450, 63);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Please stay still";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConfigure.Location = new System.Drawing.Point(174, 143);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(126, 33);
            this.buttonConfigure.TabIndex = 1;
            this.buttonConfigure.Text = "Configure";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // labelTimePrompt
            // 
            this.labelTimePrompt.BackColor = System.Drawing.Color.NavajoWhite;
            this.labelTimePrompt.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimePrompt.Location = new System.Drawing.Point(98, 94);
            this.labelTimePrompt.Name = "labelTimePrompt";
            this.labelTimePrompt.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelTimePrompt.Size = new System.Drawing.Size(275, 26);
            this.labelTimePrompt.TabIndex = 3;
            this.labelTimePrompt.Text = "Time Remaining: 120 secs";
            this.labelTimePrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BackColor = System.Drawing.Color.NavajoWhite;
            this.BorderPanel.Controls.Add(this.buttonConfigure);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(474, 199);
            this.BorderPanel.TabIndex = 5;
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(474, 199);
            this.Controls.Add(this.labelTimePrompt);
            this.Controls.Add(this.labelPrompt);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationForm";
            this.Text = "ACAT Vision";
            this.BorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.Button buttonConfigure;
        private System.Windows.Forms.Label labelTimePrompt;
        private System.Windows.Forms.Panel BorderPanel;
    }
}

