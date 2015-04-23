namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class YesNoDialogForm
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
            this.Button2 = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Label();
            this.labelCaption = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button2
            // 
            this.Button2.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button2.Location = new System.Drawing.Point(7, 151);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(374, 40);
            this.Button2.TabIndex = 129;
            this.Button2.Text = "No";
            this.Button2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button1
            // 
            this.Button1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.Location = new System.Drawing.Point(7, 104);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(374, 40);
            this.Button1.TabIndex = 128;
            this.Button1.Text = "Yes";
            this.Button1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelCaption
            // 
            this.labelCaption.BackColor = System.Drawing.Color.WhiteSmoke;
            this.labelCaption.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCaption.ForeColor = System.Drawing.Color.Black;
            this.labelCaption.Location = new System.Drawing.Point(5, 2);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(378, 57);
            this.labelCaption.TabIndex = 127;
            this.labelCaption.Text = "Are you sure you want to exit?";
            this.labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(5, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 2);
            this.label2.TabIndex = 133;
            // 
            // YesNoDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(388, 238);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.labelCaption);
            this.Name = "YesNoDialogForm";
            this.Text = "ACAT";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Button2;
        private System.Windows.Forms.Label Button1;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Label label2;

    }
}