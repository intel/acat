namespace ACAT.Lib.Extension
{
    partial class MouseScanner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MouseScannerButtons = new ACAT.Lib.Core.Widgets.ACATFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // MouseScannerButtons
            // 
            this.MouseScannerButtons.AutoSize = true;
            this.MouseScannerButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MouseScannerButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.MouseScannerButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MouseScannerButtons.Location = new System.Drawing.Point(0, 0);
            this.MouseScannerButtons.Name = "MouseScannerButtons";
            this.MouseScannerButtons.Size = new System.Drawing.Size(284, 261);
            this.MouseScannerButtons.TabIndex = 0;
            this.MouseScannerButtons.WrapContents = false;
            // 
            // MouseScanner
            // 
            this.AccessibleDescription = "ACAT Mouse Controls";
            this.AccessibleName = "ACAT Mouse Controls";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Application;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.MouseScannerButtons);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.Name = "MouseScanner";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Mouse";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ACAT.Lib.Core.Widgets.ACATFlowLayoutPanel MouseScannerButtons;
    }
}