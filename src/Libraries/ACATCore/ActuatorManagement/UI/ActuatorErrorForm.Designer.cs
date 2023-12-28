////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorErrorForm.cs
//
// This form displays an actuator error
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.ActuatorManagement
{
    partial class ActuatorErrorForm
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
            this.labelCaption = new System.Windows.Forms.Label();
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.BorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPrompt
            // 
            this.labelPrompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelPrompt.Font = new System.Drawing.Font("Montserrat SemiBold", 18F, System.Drawing.FontStyle.Bold);
            this.labelPrompt.ForeColor = System.Drawing.Color.Red;
            this.labelPrompt.Location = new System.Drawing.Point(7, 71);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(450, 89);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Prompt";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelCaption
            // 
            this.labelCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.labelCaption.Font = new System.Drawing.Font("Montserrat SemiBold", 24F, System.Drawing.FontStyle.Bold);
            this.labelCaption.ForeColor = System.Drawing.Color.White;
            this.labelCaption.Location = new System.Drawing.Point(16, 11);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(446, 57);
            this.labelCaption.TabIndex = 2;
            this.labelCaption.Text = "Caption";
            this.labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Controls.Add(this.buttonOK);
            this.BorderPanel.Controls.Add(this.buttonConfigure);
            this.BorderPanel.Controls.Add(this.labelPrompt);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(474, 267);
            this.BorderPanel.TabIndex = 5;
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.Font = new System.Drawing.Font("Montserrat Medium", 18F, System.Drawing.FontStyle.Bold);
            this.buttonOK.Location = new System.Drawing.Point(55, 178);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(173, 49);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonConfigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConfigure.Font = new System.Drawing.Font("Montserrat Medium", 18F, System.Drawing.FontStyle.Bold);
            this.buttonConfigure.Location = new System.Drawing.Point(244, 178);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(162, 49);
            this.buttonConfigure.TabIndex = 5;
            this.buttonConfigure.Text = "Configure";
            this.buttonConfigure.UseVisualStyleBackColor = false;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // ActuatorErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(474, 267);
            this.ControlBox = false;
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ActuatorErrorForm";
            this.Text = "ACAT";
            this.BorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Panel BorderPanel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonConfigure;
    }
}

