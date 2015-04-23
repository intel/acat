using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class WindowMoveResizeScannerForm : Form
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
            this.Title = new System.Windows.Forms.Label();
            this.B1 = new System.Windows.Forms.Button();
            this.B2 = new System.Windows.Forms.Button();
            this.B3 = new System.Windows.Forms.Button();
            this.B4 = new System.Windows.Forms.Button();
            this.B6 = new System.Windows.Forms.Button();
            this.B5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.Enabled = false;
            this.Title.Font = new System.Drawing.Font("Arial", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.ForeColor = System.Drawing.Color.White;
            this.Title.Location = new System.Drawing.Point(49, 4);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(1, 1);
            this.Title.TabIndex = 84;
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // B1
            // 
            this.B1.Location = new System.Drawing.Point(12, 14);
            this.B1.Name = "B1";
            this.B1.Size = new System.Drawing.Size(40, 40);
            this.B1.TabIndex = 85;
            this.B1.TabStop = false;
            this.B1.UseVisualStyleBackColor = true;
            // 
            // B2
            // 
            this.B2.Location = new System.Drawing.Point(60, 14);
            this.B2.Name = "B2";
            this.B2.Size = new System.Drawing.Size(40, 40);
            this.B2.TabIndex = 86;
            this.B2.TabStop = false;
            this.B2.UseVisualStyleBackColor = true;
            // 
            // B3
            // 
            this.B3.Location = new System.Drawing.Point(108, 14);
            this.B3.Name = "B3";
            this.B3.Size = new System.Drawing.Size(40, 40);
            this.B3.TabIndex = 87;
            this.B3.TabStop = false;
            this.B3.UseVisualStyleBackColor = true;
            // 
            // B4
            // 
            this.B4.Location = new System.Drawing.Point(156, 14);
            this.B4.Name = "B4";
            this.B4.Size = new System.Drawing.Size(40, 40);
            this.B4.TabIndex = 88;
            this.B4.TabStop = false;
            this.B4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.B4.UseVisualStyleBackColor = true;
            // 
            // B6
            // 
            this.B6.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.B6.Location = new System.Drawing.Point(252, 14);
            this.B6.Name = "B6";
            this.B6.Size = new System.Drawing.Size(40, 40);
            this.B6.TabIndex = 89;
            this.B6.TabStop = false;
            this.B6.UseVisualStyleBackColor = true;
            // 
            // B5
            // 
            this.B5.Location = new System.Drawing.Point(204, 14);
            this.B5.Name = "B5";
            this.B5.Size = new System.Drawing.Size(40, 40);
            this.B5.TabIndex = 90;
            this.B5.TabStop = false;
            this.B5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.B5.UseVisualStyleBackColor = true;
            // 
            // WindowMoveResizeScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 68);
            this.Controls.Add(this.B5);
            this.Controls.Add(this.B6);
            this.Controls.Add(this.B4);
            this.Controls.Add(this.B3);
            this.Controls.Add(this.B2);
            this.Controls.Add(this.B1);
            this.Controls.Add(this.Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "WindowMoveResizeScannerForm";
            this.Text = "ACAT";
            this.ResumeLayout(false);

        }

        #endregion

        private Label Title;
        private Button B1;
        private Button B2;
        private Button B3;
        private Button B4;
        private Button B6;
        private Button B5;
    }
}