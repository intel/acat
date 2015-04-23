using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class MuteScreenSettingsForm : Form
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
            this.lblOK = new System.Windows.Forms.Button();
            this.svalMaxDigit = new System.Windows.Forms.Label();
            this.ltbMaxDigitLess = new System.Windows.Forms.Label();
            this.lblPINCode = new System.Windows.Forms.Label();
            this.smaxMaxDigit = new System.Windows.Forms.Label();
            this.ltbMaxDigitMore = new System.Windows.Forms.Label();
            this.tbMaxDigit = new System.Windows.Forms.TrackBar();
            this.sminMaxDigit = new System.Windows.Forms.Label();
            this.lblMaxDigit = new System.Windows.Forms.Label();
            this.tbPINCode = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxDigit)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCancel
            // 
            this.lblCancel.Location = new System.Drawing.Point(206, 177);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(111, 47);
            this.lblCancel.TabIndex = 139;
            this.lblCancel.TabStop = false;
            this.lblCancel.Text = "Cancel";
            this.lblCancel.UseVisualStyleBackColor = true;
            // 
            // lblOK
            // 
            this.lblOK.Location = new System.Drawing.Point(62, 177);
            this.lblOK.Name = "lblOK";
            this.lblOK.Size = new System.Drawing.Size(111, 47);
            this.lblOK.TabIndex = 138;
            this.lblOK.TabStop = false;
            this.lblOK.Text = "OK";
            this.lblOK.UseVisualStyleBackColor = true;
            // 
            // svalMaxDigit
            // 
            this.svalMaxDigit.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.svalMaxDigit.Location = new System.Drawing.Point(159, 118);
            this.svalMaxDigit.Name = "svalMaxDigit";
            this.svalMaxDigit.Size = new System.Drawing.Size(36, 23);
            this.svalMaxDigit.TabIndex = 137;
            this.svalMaxDigit.Text = "9";
            // 
            // ltbMaxDigitLess
            // 
            this.ltbMaxDigitLess.BackColor = System.Drawing.Color.Transparent;
            this.ltbMaxDigitLess.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbMaxDigitLess.Location = new System.Drawing.Point(199, 117);
            this.ltbMaxDigitLess.Name = "ltbMaxDigitLess";
            this.ltbMaxDigitLess.Size = new System.Drawing.Size(25, 25);
            this.ltbMaxDigitLess.TabIndex = 134;
            this.ltbMaxDigitLess.Text = "<";
            this.ltbMaxDigitLess.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPINCode
            // 
            this.lblPINCode.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPINCode.Location = new System.Drawing.Point(125, 52);
            this.lblPINCode.Name = "lblPINCode";
            this.lblPINCode.Size = new System.Drawing.Size(47, 26);
            this.lblPINCode.TabIndex = 136;
            this.lblPINCode.Text = "PIN";
            this.lblPINCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // smaxMaxDigit
            // 
            this.smaxMaxDigit.AutoSize = true;
            this.smaxMaxDigit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.smaxMaxDigit.Location = new System.Drawing.Point(324, 141);
            this.smaxMaxDigit.Name = "smaxMaxDigit";
            this.smaxMaxDigit.Size = new System.Drawing.Size(13, 13);
            this.smaxMaxDigit.TabIndex = 132;
            this.smaxMaxDigit.Text = "9";
            this.smaxMaxDigit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ltbMaxDigitMore
            // 
            this.ltbMaxDigitMore.BackColor = System.Drawing.Color.Transparent;
            this.ltbMaxDigitMore.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbMaxDigitMore.Location = new System.Drawing.Point(319, 117);
            this.ltbMaxDigitMore.Name = "ltbMaxDigitMore";
            this.ltbMaxDigitMore.Size = new System.Drawing.Size(25, 25);
            this.ltbMaxDigitMore.TabIndex = 135;
            this.ltbMaxDigitMore.Text = ">";
            this.ltbMaxDigitMore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbMaxDigit
            // 
            this.tbMaxDigit.Location = new System.Drawing.Point(217, 117);
            this.tbMaxDigit.Name = "tbMaxDigit";
            this.tbMaxDigit.Size = new System.Drawing.Size(110, 45);
            this.tbMaxDigit.TabIndex = 133;
            this.tbMaxDigit.TabStop = false;
            // 
            // sminMaxDigit
            // 
            this.sminMaxDigit.AutoSize = true;
            this.sminMaxDigit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sminMaxDigit.Location = new System.Drawing.Point(204, 141);
            this.sminMaxDigit.Name = "sminMaxDigit";
            this.sminMaxDigit.Size = new System.Drawing.Size(13, 13);
            this.sminMaxDigit.TabIndex = 131;
            this.sminMaxDigit.Text = "0";
            this.sminMaxDigit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMaxDigit
            // 
            this.lblMaxDigit.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxDigit.Location = new System.Drawing.Point(26, 116);
            this.lblMaxDigit.Name = "lblMaxDigit";
            this.lblMaxDigit.Size = new System.Drawing.Size(135, 26);
            this.lblMaxDigit.TabIndex = 130;
            this.lblMaxDigit.Text = "Digits: 0  to  ";
            this.lblMaxDigit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbPINCode
            // 
            this.tbPINCode.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPINCode.Location = new System.Drawing.Point(172, 49);
            this.tbPINCode.MaxLength = 4;
            this.tbPINCode.Name = "tbPINCode";
            this.tbPINCode.Size = new System.Drawing.Size(80, 32);
            this.tbPINCode.TabIndex = 129;
            this.tbPINCode.TabStop = false;
            this.tbPINCode.WordWrap = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(346, 248);
            this.groupBox1.TabIndex = 161;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, -2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 24);
            this.label1.TabIndex = 161;
            this.label1.Text = "Mute Settings";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AsterMutePrefsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 272);
            this.Controls.Add(this.lblCancel);
            this.Controls.Add(this.lblOK);
            this.Controls.Add(this.svalMaxDigit);
            this.Controls.Add(this.ltbMaxDigitLess);
            this.Controls.Add(this.lblPINCode);
            this.Controls.Add(this.smaxMaxDigit);
            this.Controls.Add(this.ltbMaxDigitMore);
            this.Controls.Add(this.tbMaxDigit);
            this.Controls.Add(this.sminMaxDigit);
            this.Controls.Add(this.lblMaxDigit);
            this.Controls.Add(this.tbPINCode);
            this.Controls.Add(this.groupBox1);
            this.Name = "AsterMutePrefsForm";
            this.Text = "ACAT";
            ((System.ComponentModel.ISupportInitialize)(this.tbMaxDigit)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button lblCancel;
        private Button lblOK;
        private Label svalMaxDigit;
        private Label ltbMaxDigitLess;
        private Label lblPINCode;
        private Label smaxMaxDigit;
        private Label ltbMaxDigitMore;
        private TrackBar tbMaxDigit;
        private Label sminMaxDigit;
        private Label lblMaxDigit;
        private TextBox tbPINCode;
        private GroupBox groupBox1;
        private Label label1;

    }
}