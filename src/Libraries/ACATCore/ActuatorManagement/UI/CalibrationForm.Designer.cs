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
            this.labelCaption = new System.Windows.Forms.Label();
            this.labelTimePrompt = new System.Windows.Forms.Label();
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.BorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.labelPrompt.BackColor = System.Drawing.Color.AntiqueWhite;
            this.labelPrompt.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrompt.ForeColor = System.Drawing.Color.Red;
            this.labelPrompt.Location = new System.Drawing.Point(12, 53);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(450, 63);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Please stay still";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConfigure.Location = new System.Drawing.Point(174, 158);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(126, 33);
            this.buttonConfigure.TabIndex = 1;
            this.buttonConfigure.Text = "Configure";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // labelCaption
            // 
            this.labelCaption.BackColor = System.Drawing.Color.AntiqueWhite;
            this.labelCaption.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCaption.Location = new System.Drawing.Point(16, 11);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(446, 32);
            this.labelCaption.TabIndex = 2;
            this.labelCaption.Text = "Calibrating Blink Sensor...";
            this.labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTimePrompt
            // 
            this.labelTimePrompt.BackColor = System.Drawing.Color.AntiqueWhite;
            this.labelTimePrompt.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimePrompt.Location = new System.Drawing.Point(98, 126);
            this.labelTimePrompt.Name = "labelTimePrompt";
            this.labelTimePrompt.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelTimePrompt.Size = new System.Drawing.Size(275, 26);
            this.labelTimePrompt.TabIndex = 3;
            this.labelTimePrompt.Text = "Time Remaining: 120 secs";
            this.labelTimePrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BackColor = System.Drawing.Color.AntiqueWhite;
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Controls.Add(this.buttonConfigure);
            this.BorderPanel.Location = new System.Drawing.Point(4, 6);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(466, 203);
            this.BorderPanel.TabIndex = 5;
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(474, 214);
            this.ControlBox = false;
            this.Controls.Add(this.labelTimePrompt);
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.labelPrompt);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CalibrationForm";
            this.Text = "ACAT";
            this.BorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.Button buttonConfigure;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Label labelTimePrompt;
        private System.Windows.Forms.Panel BorderPanel;
    }
}

