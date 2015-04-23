namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class YesNoDialogForm2
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
            this.Button1Blank = new System.Windows.Forms.Label();
            this.Button2Blank = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button2
            // 
            this.Button2.BackColor = System.Drawing.Color.White;
            this.Button2.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button2.Location = new System.Drawing.Point(6, 251);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(374, 40);
            this.Button2.TabIndex = 129;
            this.Button2.Text = "Yes";
            this.Button2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button1
            // 
            this.Button1.BackColor = System.Drawing.Color.White;
            this.Button1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.Location = new System.Drawing.Point(6, 170);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(374, 40);
            this.Button1.TabIndex = 128;
            this.Button1.Text = "No";
            this.Button1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelCaption
            // 
            this.labelCaption.BackColor = System.Drawing.Color.WhiteSmoke;
            this.labelCaption.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCaption.ForeColor = System.Drawing.Color.Black;
            this.labelCaption.Location = new System.Drawing.Point(5, 2);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(378, 126);
            this.labelCaption.TabIndex = 127;
            this.labelCaption.Text = "Are you sure you want to exit?";
            this.labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(5, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 2);
            this.label2.TabIndex = 133;
            // 
            // Button1Blank
            // 
            this.Button1Blank.BackColor = System.Drawing.Color.White;
            this.Button1Blank.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1Blank.Location = new System.Drawing.Point(6, 128);
            this.Button1Blank.Name = "Button1Blank";
            this.Button1Blank.Size = new System.Drawing.Size(374, 40);
            this.Button1Blank.TabIndex = 134;
            this.Button1Blank.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Button2Blank
            // 
            this.Button2Blank.BackColor = System.Drawing.Color.White;
            this.Button2Blank.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button2Blank.Location = new System.Drawing.Point(6, 211);
            this.Button2Blank.Name = "Button2Blank";
            this.Button2Blank.Size = new System.Drawing.Size(374, 40);
            this.Button2Blank.TabIndex = 135;
            this.Button2Blank.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // YesNoDialogForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(388, 295);
            this.Controls.Add(this.Button2Blank);
            this.Controls.Add(this.Button1Blank);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.labelCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "YesNoDialogForm2";
            this.Text = "ACAT";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Button2;
        private System.Windows.Forms.Label Button1;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Button1Blank;
        private System.Windows.Forms.Label Button2Blank;

    }
}