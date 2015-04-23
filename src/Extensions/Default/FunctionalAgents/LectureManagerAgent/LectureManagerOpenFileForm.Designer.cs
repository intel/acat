namespace Aster.Extensions.Hawking.AppAgents.LectureManager
{
    partial class LectureManagerOpenFileForm : System.Windows.Forms.Form
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
            this.lblOpenFile = new System.Windows.Forms.Label();
            this.lboxFiles = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblOpenFile
            // 
            this.lblOpenFile.AutoSize = true;
            this.lblOpenFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpenFile.Location = new System.Drawing.Point(130, 36);
            this.lblOpenFile.Name = "lblOpenFile";
            this.lblOpenFile.Size = new System.Drawing.Size(103, 24);
            this.lblOpenFile.TabIndex = 0;
            this.lblOpenFile.Text = "Open File";
            // 
            // lboxFiles
            // 
            this.lboxFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lboxFiles.FormattingEnabled = true;
            this.lboxFiles.ItemHeight = 24;
            this.lboxFiles.Location = new System.Drawing.Point(134, 74);
            this.lboxFiles.Name = "lboxFiles";
            this.lboxFiles.Size = new System.Drawing.Size(606, 532);
            this.lboxFiles.TabIndex = 1;
            this.lboxFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lboxFiles_MouseDown);
            // 
            // LectureManagerOpenFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 671);
            this.Controls.Add(this.lboxFiles);
            this.Controls.Add(this.lblOpenFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LectureManagerOpenFileForm";
            this.Text = "Open File";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.formOpenFile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOpenFile;
        private System.Windows.Forms.ListBox lboxFiles;
    }
}