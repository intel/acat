namespace ACAT.Lib.Core.PanelManagement
{
    partial class SplashScreen 
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.splashPictureBox = new System.Windows.Forms.PictureBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.intelLogo = new System.Windows.Forms.PictureBox();
            this.line3 = new System.Windows.Forms.Label();
            this.line2 = new System.Windows.Forms.Label();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.line1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splashPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intelLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splashPictureBox
            // 
            this.splashPictureBox.Location = new System.Drawing.Point(11, 12);
            this.splashPictureBox.Name = "splashPictureBox";
            this.splashPictureBox.Size = new System.Drawing.Size(149, 301);
            this.splashPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.splashPictureBox.TabIndex = 0;
            this.splashPictureBox.TabStop = false;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Splash1.png");
            this.imageList.Images.SetKeyName(1, "Splash2.png");
            this.imageList.Images.SetKeyName(2, "Splash3.png");
            this.imageList.Images.SetKeyName(3, "Splash4.png");
            this.imageList.Images.SetKeyName(4, "Splash5.png");
            // 
            // intelLogo
            // 
            this.intelLogo.BackColor = System.Drawing.Color.Transparent;
            this.intelLogo.Image = ((System.Drawing.Image)(resources.GetObject("intelLogo.Image")));
            this.intelLogo.Location = new System.Drawing.Point(327, 7);
            this.intelLogo.Name = "intelLogo";
            this.intelLogo.Size = new System.Drawing.Size(75, 75);
            this.intelLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.intelLogo.TabIndex = 1;
            this.intelLogo.TabStop = false;
            // 
            // line3
            // 
            this.line3.BackColor = System.Drawing.Color.Transparent;
            this.line3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.line3.Location = new System.Drawing.Point(173, 192);
            this.line3.Name = "line3";
            this.line3.Size = new System.Drawing.Size(382, 49);
            this.line3.TabIndex = 0;
            this.line3.Text = "line3";
            this.line3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // line2
            // 
            this.line2.BackColor = System.Drawing.Color.Transparent;
            this.line2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.line2.Location = new System.Drawing.Point(263, 141);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(202, 33);
            this.line2.TabIndex = 3;
            this.line2.Text = "line2";
            this.line2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxStatus
            // 
            this.pictureBoxStatus.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxStatus.Location = new System.Drawing.Point(304, 270);
            this.pictureBoxStatus.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.Size = new System.Drawing.Size(123, 18);
            this.pictureBoxStatus.TabIndex = 4;
            this.pictureBoxStatus.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.panel1.BackgroundImage = global::ACAT.Lib.Core.Properties.Resources.SplashScreenBg;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.line1);
            this.panel1.Controls.Add(this.pictureBoxStatus);
            this.panel1.Controls.Add(this.line2);
            this.panel1.Controls.Add(this.line3);
            this.panel1.Controls.Add(this.intelLogo);
            this.panel1.Location = new System.Drawing.Point(4, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(576, 313);
            this.panel1.TabIndex = 5;
            // 
            // line1
            // 
            this.line1.BackColor = System.Drawing.Color.Transparent;
            this.line1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.line1.Location = new System.Drawing.Point(175, 86);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(383, 57);
            this.line1.TabIndex = 5;
            this.line1.Text = "Assistive Context-Aware Toolkit (ACAT)";
            this.line1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(587, 327);
            this.Controls.Add(this.splashPictureBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.splashPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intelLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox splashPictureBox;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.PictureBox intelLogo;
        private System.Windows.Forms.Label line3;
        private System.Windows.Forms.Label line2;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label line1;
    }
}

