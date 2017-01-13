using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class ResizeScannerForm
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
            this.lblBack = new System.Windows.Forms.Button();
            this.lblIncrease = new System.Windows.Forms.Button();
            this.lblDecrease = new System.Windows.Forms.Button();
            this.lblMove = new System.Windows.Forms.Button();
            this.lblDefault = new System.Windows.Forms.Button();
            this.labelToolTip = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblBack
            // 
            this.lblBack.Location = new System.Drawing.Point(16, 27);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(66, 66);
            this.lblBack.TabIndex = 92;
            this.lblBack.TabStop = false;
            this.lblBack.Text = "^";
            this.lblBack.UseVisualStyleBackColor = true;
            // 
            // lblIncrease
            // 
            this.lblIncrease.Location = new System.Drawing.Point(99, 26);
            this.lblIncrease.Name = "lblIncrease";
            this.lblIncrease.Size = new System.Drawing.Size(66, 66);
            this.lblIncrease.TabIndex = 93;
            this.lblIncrease.TabStop = false;
            this.lblIncrease.Text = "-";
            this.lblIncrease.UseVisualStyleBackColor = true;
            // 
            // lblDecrease
            // 
            this.lblDecrease.Location = new System.Drawing.Point(182, 27);
            this.lblDecrease.Name = "lblDecrease";
            this.lblDecrease.Size = new System.Drawing.Size(66, 66);
            this.lblDecrease.TabIndex = 94;
            this.lblDecrease.TabStop = false;
            this.lblDecrease.Text = "-";
            this.lblDecrease.UseVisualStyleBackColor = true;
            // 
            // lblMove
            // 
            this.lblMove.Location = new System.Drawing.Point(265, 27);
            this.lblMove.Name = "lblMove";
            this.lblMove.Size = new System.Drawing.Size(66, 66);
            this.lblMove.TabIndex = 95;
            this.lblMove.TabStop = false;
            this.lblMove.Text = "*";
            this.lblMove.UseVisualStyleBackColor = true;
            // 
            // lblDefault
            // 
            this.lblDefault.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblDefault.Location = new System.Drawing.Point(348, 27);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(66, 66);
            this.lblDefault.TabIndex = 96;
            this.lblDefault.TabStop = false;
            this.lblDefault.Text = "a";
            this.lblDefault.UseVisualStyleBackColor = true;
            // 
            // labelToolTip
            // 
            this.labelToolTip.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelToolTip.Location = new System.Drawing.Point(12, 105);
            this.labelToolTip.Name = "labelToolTip";
            this.labelToolTip.Size = new System.Drawing.Size(415, 47);
            this.labelToolTip.TabIndex = 97;
            this.labelToolTip.Text = "Back";
            this.labelToolTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResizeScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 188);
            this.ControlBox = false;
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.lblIncrease);
            this.Controls.Add(this.lblDecrease);
            this.Controls.Add(this.lblMove);
            this.Controls.Add(this.lblDefault);
            this.Controls.Add(this.labelToolTip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ResizeScannerForm";
            this.Text = "Resize / Reposition Scanner";
            this.ResumeLayout(false);

        }

        #endregion

        private Button lblBack;
        private Button lblIncrease;
        private Button lblDecrease;
        private Button lblMove;
        private Button lblDefault;
        private Label labelToolTip;

    }
}