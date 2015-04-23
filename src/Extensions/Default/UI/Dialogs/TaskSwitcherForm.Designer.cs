using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class TaskSwitcherForm : Form
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
            this.lblCancel = new System.Windows.Forms.Button();
            this.Title = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCancel
            // 
            this.lblCancel.Location = new System.Drawing.Point(7, 81);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(62, 59);
            this.lblCancel.TabIndex = 128;
            this.lblCancel.TabStop = false;
            this.lblCancel.Text = "^";
            this.lblCancel.UseVisualStyleBackColor = true;
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.White;
            this.Title.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Title.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.ForeColor = System.Drawing.Color.Black;
            this.Title.Location = new System.Drawing.Point(3, 1);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(925, 50);
            this.Title.TabIndex = 127;
            this.Title.TabStop = false;
            this.Title.Text = "Switch Apps";
            this.Title.UseVisualStyleBackColor = false;
            // 
            // TaskSwitcherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(1021, 362);
            this.Controls.Add(this.lblCancel);
            this.Controls.Add(this.Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "TaskSwitcherForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Switch Application";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private Button lblCancel;
        private Button Title;
    }
}
