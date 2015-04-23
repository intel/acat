using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class ResizeScannerForm : Form
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
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.labelToolTip = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblBack
            // 
            this.lblBack.Location = new System.Drawing.Point(22, 19);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(30, 30);
            this.lblBack.TabIndex = 85;
            this.lblBack.TabStop = false;
            this.lblBack.Text = "^";
            this.lblBack.UseVisualStyleBackColor = true;
            // 
            // lblIncrease
            // 
            this.lblIncrease.Location = new System.Drawing.Point(68, 19);
            this.lblIncrease.Name = "lblIncrease";
            this.lblIncrease.Size = new System.Drawing.Size(30, 30);
            this.lblIncrease.TabIndex = 86;
            this.lblIncrease.TabStop = false;
            this.lblIncrease.Text = "-";
            this.lblIncrease.UseVisualStyleBackColor = true;
            // 
            // lblDecrease
            // 
            this.lblDecrease.Location = new System.Drawing.Point(115, 19);
            this.lblDecrease.Name = "lblDecrease";
            this.lblDecrease.Size = new System.Drawing.Size(30, 30);
            this.lblDecrease.TabIndex = 87;
            this.lblDecrease.TabStop = false;
            this.lblDecrease.Text = "-";
            this.lblDecrease.UseVisualStyleBackColor = true;
            // 
            // lblMove
            // 
            this.lblMove.Location = new System.Drawing.Point(161, 19);
            this.lblMove.Name = "lblMove";
            this.lblMove.Size = new System.Drawing.Size(30, 30);
            this.lblMove.TabIndex = 88;
            this.lblMove.TabStop = false;
            this.lblMove.Text = "*";
            this.lblMove.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMove.UseVisualStyleBackColor = true;
            // 
            // lblDefault
            // 
            this.lblDefault.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblDefault.Location = new System.Drawing.Point(208, 19);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(30, 30);
            this.lblDefault.TabIndex = 89;
            this.lblDefault.TabStop = false;
            this.lblDefault.Text = "a";
            this.lblDefault.UseVisualStyleBackColor = true;
            // 
            // BorderPanel
            // 
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Location = new System.Drawing.Point(3, 1);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(256, 104);
            this.BorderPanel.TabIndex = 90;
            // 
            // labelToolTip
            // 
            this.labelToolTip.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelToolTip.Location = new System.Drawing.Point(7, 55);
            this.labelToolTip.Name = "labelToolTip";
            this.labelToolTip.Size = new System.Drawing.Size(248, 36);
            this.labelToolTip.TabIndex = 91;
            this.labelToolTip.Text = "Back";
            this.labelToolTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResizeScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 108);
            this.Controls.Add(this.labelToolTip);
            this.Controls.Add(this.lblDefault);
            this.Controls.Add(this.lblMove);
            this.Controls.Add(this.lblDecrease);
            this.Controls.Add(this.lblIncrease);
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.BorderPanel);
            this.Name = "ResizeScannerForm";
            this.Text = "ACAT Scanner Design";
            this.ResumeLayout(false);

        }

        #endregion

        private Button lblBack;
        private Button lblIncrease;
        private Button lblDecrease;
        private Button lblMove;
        private Button lblDefault;
        private Panel BorderPanel;
        private Label labelToolTip;
    }
}