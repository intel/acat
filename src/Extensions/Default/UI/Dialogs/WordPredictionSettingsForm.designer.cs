namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class WordPredictionSettingsForm
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
            this.svalWordCount = new System.Windows.Forms.Label();
            this.ltbWordCountLess = new System.Windows.Forms.Label();
            this.lblUseCorpus = new System.Windows.Forms.Label();
            this.lblDynamicLearning = new System.Windows.Forms.Label();
            this.ltbWordCountMore = new System.Windows.Forms.Label();
            this.tbWordCount = new System.Windows.Forms.TrackBar();
            this.lblWordCount = new System.Windows.Forms.Label();
            this.lblRestoreDefaults = new System.Windows.Forms.Button();
            this.lblBack = new System.Windows.Forms.Button();
            this.lblOK = new System.Windows.Forms.Button();
            this.sminWordCount = new System.Windows.Forms.Label();
            this.smaxWordCount = new System.Windows.Forms.Label();
            this.pbDynamicLearning = new System.Windows.Forms.Label();
            this.pbUseCorpus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbWordCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // svalWordCount
            // 
            this.svalWordCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.svalWordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.svalWordCount.Location = new System.Drawing.Point(347, 171);
            this.svalWordCount.Name = "svalWordCount";
            this.svalWordCount.Size = new System.Drawing.Size(23, 23);
            this.svalWordCount.TabIndex = 82;
            this.svalWordCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ltbWordCountLess
            // 
            this.ltbWordCountLess.BackColor = System.Drawing.Color.Transparent;
            this.ltbWordCountLess.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbWordCountLess.Location = new System.Drawing.Point(196, 172);
            this.ltbWordCountLess.Name = "ltbWordCountLess";
            this.ltbWordCountLess.Size = new System.Drawing.Size(25, 25);
            this.ltbWordCountLess.TabIndex = 80;
            this.ltbWordCountLess.Text = "<";
            this.ltbWordCountLess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUseCorpus
            // 
            this.lblUseCorpus.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUseCorpus.Location = new System.Drawing.Point(100, 117);
            this.lblUseCorpus.Name = "lblUseCorpus";
            this.lblUseCorpus.Size = new System.Drawing.Size(207, 29);
            this.lblUseCorpus.TabIndex = 81;
            this.lblUseCorpus.Text = "Use Corpus Models for Word Prediction";
            this.lblUseCorpus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDynamicLearning
            // 
            this.lblDynamicLearning.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDynamicLearning.Location = new System.Drawing.Point(100, 69);
            this.lblDynamicLearning.Name = "lblDynamicLearning";
            this.lblDynamicLearning.Size = new System.Drawing.Size(254, 29);
            this.lblDynamicLearning.TabIndex = 69;
            this.lblDynamicLearning.Text = "Enable Dynamic Learning";
            this.lblDynamicLearning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ltbWordCountMore
            // 
            this.ltbWordCountMore.BackColor = System.Drawing.Color.Transparent;
            this.ltbWordCountMore.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbWordCountMore.Location = new System.Drawing.Point(317, 172);
            this.ltbWordCountMore.Name = "ltbWordCountMore";
            this.ltbWordCountMore.Size = new System.Drawing.Size(25, 25);
            this.ltbWordCountMore.TabIndex = 64;
            this.ltbWordCountMore.Text = ">";
            this.ltbWordCountMore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbWordCount
            // 
            this.tbWordCount.Location = new System.Drawing.Point(216, 172);
            this.tbWordCount.Name = "tbWordCount";
            this.tbWordCount.Size = new System.Drawing.Size(110, 45);
            this.tbWordCount.TabIndex = 65;
            this.tbWordCount.TabStop = false;
            // 
            // lblWordCount
            // 
            this.lblWordCount.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWordCount.Location = new System.Drawing.Point(55, 171);
            this.lblWordCount.Name = "lblWordCount";
            this.lblWordCount.Size = new System.Drawing.Size(135, 22);
            this.lblWordCount.TabIndex = 65;
            this.lblWordCount.Text = "Word Count";
            // 
            // lblRestoreDefaults
            // 
            this.lblRestoreDefaults.BackColor = System.Drawing.SystemColors.Control;
            this.lblRestoreDefaults.Location = new System.Drawing.Point(264, 246);
            this.lblRestoreDefaults.Name = "lblRestoreDefaults";
            this.lblRestoreDefaults.Size = new System.Drawing.Size(107, 47);
            this.lblRestoreDefaults.TabIndex = 130;
            this.lblRestoreDefaults.TabStop = false;
            this.lblRestoreDefaults.Text = "Defaults";
            this.lblRestoreDefaults.UseVisualStyleBackColor = false;
            // 
            // lblBack
            // 
            this.lblBack.BackColor = System.Drawing.SystemColors.Control;
            this.lblBack.Location = new System.Drawing.Point(151, 246);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(107, 47);
            this.lblBack.TabIndex = 129;
            this.lblBack.TabStop = false;
            this.lblBack.Text = "Cancel";
            this.lblBack.UseVisualStyleBackColor = false;
            // 
            // lblOK
            // 
            this.lblOK.BackColor = System.Drawing.SystemColors.Control;
            this.lblOK.Location = new System.Drawing.Point(38, 246);
            this.lblOK.Name = "lblOK";
            this.lblOK.Size = new System.Drawing.Size(107, 47);
            this.lblOK.TabIndex = 128;
            this.lblOK.TabStop = false;
            this.lblOK.Text = "OK";
            this.lblOK.UseVisualStyleBackColor = false;
            // 
            // sminWordCount
            // 
            this.sminWordCount.Location = new System.Drawing.Point(202, 200);
            this.sminWordCount.Name = "sminWordCount";
            this.sminWordCount.Size = new System.Drawing.Size(13, 13);
            this.sminWordCount.TabIndex = 63;
            this.sminWordCount.Text = "0";
            this.sminWordCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // smaxWordCount
            // 
            this.smaxWordCount.Location = new System.Drawing.Point(318, 199);
            this.smaxWordCount.Name = "smaxWordCount";
            this.smaxWordCount.Size = new System.Drawing.Size(19, 13);
            this.smaxWordCount.TabIndex = 63;
            this.smaxWordCount.Text = "10";
            this.smaxWordCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbDynamicLearning
            // 
            this.pbDynamicLearning.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbDynamicLearning.Location = new System.Drawing.Point(57, 72);
            this.pbDynamicLearning.Name = "pbDynamicLearning";
            this.pbDynamicLearning.Size = new System.Drawing.Size(28, 29);
            this.pbDynamicLearning.TabIndex = 69;
            this.pbDynamicLearning.Text = "N";
            // 
            // pbUseCorpus
            // 
            this.pbUseCorpus.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbUseCorpus.Location = new System.Drawing.Point(57, 119);
            this.pbUseCorpus.Name = "pbUseCorpus";
            this.pbUseCorpus.Size = new System.Drawing.Size(28, 29);
            this.pbUseCorpus.TabIndex = 70;
            this.pbUseCorpus.Text = "N";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, -3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 24);
            this.label1.TabIndex = 161;
            this.label1.Text = "Word Prediction Settings";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(16, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 294);
            this.groupBox1.TabIndex = 172;
            this.groupBox1.TabStop = false;
            // 
            // AsterWordPredictionPrefsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(408, 323);
            this.Controls.Add(this.smaxWordCount);
            this.Controls.Add(this.sminWordCount);
            this.Controls.Add(this.svalWordCount);
            this.Controls.Add(this.ltbWordCountLess);
            this.Controls.Add(this.lblRestoreDefaults);
            this.Controls.Add(this.ltbWordCountMore);
            this.Controls.Add(this.lblUseCorpus);
            this.Controls.Add(this.tbWordCount);
            this.Controls.Add(this.lblWordCount);
            this.Controls.Add(this.lblDynamicLearning);
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.pbUseCorpus);
            this.Controls.Add(this.lblOK);
            this.Controls.Add(this.pbDynamicLearning);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WordPredictionSettingsForm";
            this.Text = "ACAT";
            ((System.ComponentModel.ISupportInitialize)(this.tbWordCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ltbWordCountMore;
        private System.Windows.Forms.TrackBar tbWordCount;
        private System.Windows.Forms.Label ltbWordCountLess;
        private System.Windows.Forms.Label lblWordCount;
        private System.Windows.Forms.Label sminWordCount;
        private System.Windows.Forms.Label smaxWordCount;
        private System.Windows.Forms.Label lblDynamicLearning;
        private System.Windows.Forms.Label pbDynamicLearning;
        private System.Windows.Forms.Label lblUseCorpus;
        private System.Windows.Forms.Label pbUseCorpus;
        private System.Windows.Forms.Label svalWordCount;
        private System.Windows.Forms.Button lblRestoreDefaults;
        private System.Windows.Forms.Button lblBack;
        private System.Windows.Forms.Button lblOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

