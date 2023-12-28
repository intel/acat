
namespace ACAT.Extensions
{
    partial class TalkWindowTextBoxUserControl
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
            this.TextBoxTalkWindow = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TextBoxTalkWindow
            // 
            this.TextBoxTalkWindow.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBoxTalkWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxTalkWindow.Font = new System.Drawing.Font("Montserrat SemiBold", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxTalkWindow.Location = new System.Drawing.Point(0, 0);
            this.TextBoxTalkWindow.Margin = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.TextBoxTalkWindow.Multiline = true;
            this.TextBoxTalkWindow.Name = "TextBoxTalkWindow";
            this.TextBoxTalkWindow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxTalkWindow.Size = new System.Drawing.Size(428, 147);
            this.TextBoxTalkWindow.TabIndex = 0;
            // 
            // TalkWindowTextBoxUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.TextBoxTalkWindow);
            this.Name = "TalkWindowTextBoxUserControl";
            this.Size = new System.Drawing.Size(428, 147);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxTalkWindow;
    }
}
