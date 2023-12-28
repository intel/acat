////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CalibrationForm.cs
//
// Form to display the status of calibration. Displays calibration status
// of the actuator including an optional count-down or count-up timer
//
////////////////////////////////////////////////////////////////////////////

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
            this.labelTimePrompt = new System.Windows.Forms.Label();
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.BorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.labelPrompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPrompt.Font = new System.Drawing.Font("Montserrat SemiBold", 28F, System.Drawing.FontStyle.Bold);
            this.labelPrompt.ForeColor = System.Drawing.Color.White;
            this.labelPrompt.Location = new System.Drawing.Point(48, 11);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(367, 57);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Please stay still";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTimePrompt
            // 
            this.labelTimePrompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelTimePrompt.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Bold);
            this.labelTimePrompt.ForeColor = System.Drawing.Color.White;
            this.labelTimePrompt.Location = new System.Drawing.Point(94, 81);
            this.labelTimePrompt.Name = "labelTimePrompt";
            this.labelTimePrompt.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelTimePrompt.Size = new System.Drawing.Size(275, 41);
            this.labelTimePrompt.TabIndex = 3;
            this.labelTimePrompt.Text = "Time Remaining: 120 secs";
            this.labelTimePrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BorderPanel.Controls.Add(this.labelPrompt);
            this.BorderPanel.Controls.Add(this.labelTimePrompt);
            this.BorderPanel.Controls.Add(this.buttonConfigure);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(474, 224);
            this.BorderPanel.TabIndex = 5;
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonConfigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConfigure.Font = new System.Drawing.Font("Montserrat Medium", 18F, System.Drawing.FontStyle.Bold);
            this.buttonConfigure.Location = new System.Drawing.Point(149, 137);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(162, 49);
            this.buttonConfigure.TabIndex = 4;
            this.buttonConfigure.Text = "Configure";
            this.buttonConfigure.UseVisualStyleBackColor = false;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(474, 224);
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
        private System.Windows.Forms.Label labelTimePrompt;
        private System.Windows.Forms.Panel BorderPanel;
        private System.Windows.Forms.Button buttonConfigure;
    }
}

