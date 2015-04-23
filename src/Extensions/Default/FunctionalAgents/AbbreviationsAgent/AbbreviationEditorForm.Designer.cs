namespace ACAT.Extensions.Hawking.FunctionalAgents.Abbreviations

{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class AbbreviationEditorForm
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
            this.lblExpansion = new System.Windows.Forms.Label();
            this.tbExpansion = new System.Windows.Forms.TextBox();
            this.lblSpoken = new System.Windows.Forms.Label();
            this.lblFinished = new System.Windows.Forms.Button();
            this.lblAbbreviation = new System.Windows.Forms.Label();
            this.boxType = new System.Windows.Forms.Panel();
            this.lblWritten = new System.Windows.Forms.Label();
            this.pbTypeSpoken = new System.Windows.Forms.Label();
            this.pbTypeWritten = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.tbAbbreviation = new System.Windows.Forms.TextBox();
            this.lblCancel = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.boxType.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblExpansion
            // 
            this.lblExpansion.AutoSize = true;
            this.lblExpansion.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpansion.Location = new System.Drawing.Point(239, 37);
            this.lblExpansion.Name = "lblExpansion";
            this.lblExpansion.Size = new System.Drawing.Size(117, 24);
            this.lblExpansion.TabIndex = 96;
            this.lblExpansion.Text = "Expansion";
            // 
            // tbExpansion
            // 
            this.tbExpansion.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbExpansion.Location = new System.Drawing.Point(243, 64);
            this.tbExpansion.Multiline = true;
            this.tbExpansion.Name = "tbExpansion";
            this.tbExpansion.Size = new System.Drawing.Size(476, 185);
            this.tbExpansion.TabIndex = 93;
            // 
            // lblSpoken
            // 
            this.lblSpoken.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpoken.Location = new System.Drawing.Point(69, 46);
            this.lblSpoken.Name = "lblSpoken";
            this.lblSpoken.Size = new System.Drawing.Size(94, 29);
            this.lblSpoken.TabIndex = 2;
            this.lblSpoken.Text = "Spoken";
            this.lblSpoken.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFinished
            // 
            this.lblFinished.Location = new System.Drawing.Point(355, 266);
            this.lblFinished.Name = "lblFinished";
            this.lblFinished.Size = new System.Drawing.Size(114, 47);
            this.lblFinished.TabIndex = 88;
            this.lblFinished.TabStop = false;
            this.lblFinished.Text = "OK";
            this.lblFinished.UseVisualStyleBackColor = true;
            // 
            // lblAbbreviation
            // 
            this.lblAbbreviation.AutoSize = true;
            this.lblAbbreviation.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbbreviation.Location = new System.Drawing.Point(52, 57);
            this.lblAbbreviation.Name = "lblAbbreviation";
            this.lblAbbreviation.Size = new System.Drawing.Size(139, 24);
            this.lblAbbreviation.TabIndex = 91;
            this.lblAbbreviation.Text = "Abbreviation";
            // 
            // boxType
            // 
            this.boxType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boxType.Controls.Add(this.lblSpoken);
            this.boxType.Controls.Add(this.lblWritten);
            this.boxType.Controls.Add(this.pbTypeSpoken);
            this.boxType.Controls.Add(this.pbTypeWritten);
            this.boxType.Location = new System.Drawing.Point(44, 162);
            this.boxType.Name = "boxType";
            this.boxType.Size = new System.Drawing.Size(183, 85);
            this.boxType.TabIndex = 95;
            // 
            // lblWritten
            // 
            this.lblWritten.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWritten.Location = new System.Drawing.Point(69, 9);
            this.lblWritten.Name = "lblWritten";
            this.lblWritten.Size = new System.Drawing.Size(94, 29);
            this.lblWritten.TabIndex = 1;
            this.lblWritten.Text = "Written";
            this.lblWritten.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbTypeSpoken
            // 
            this.pbTypeSpoken.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbTypeSpoken.Location = new System.Drawing.Point(9, 46);
            this.pbTypeSpoken.Name = "pbTypeSpoken";
            this.pbTypeSpoken.Size = new System.Drawing.Size(39, 29);
            this.pbTypeSpoken.TabIndex = 67;
            this.pbTypeSpoken.Text = "N";
            this.pbTypeSpoken.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbTypeWritten
            // 
            this.pbTypeWritten.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbTypeWritten.Location = new System.Drawing.Point(9, 10);
            this.pbTypeWritten.Name = "pbTypeWritten";
            this.pbTypeWritten.Size = new System.Drawing.Size(39, 29);
            this.pbTypeWritten.TabIndex = 66;
            this.pbTypeWritten.Text = "N";
            this.pbTypeWritten.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.Location = new System.Drawing.Point(40, 133);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(65, 24);
            this.lblType.TabIndex = 94;
            this.lblType.Text = "Mode";
            // 
            // tbAbbreviation
            // 
            this.tbAbbreviation.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAbbreviation.Location = new System.Drawing.Point(56, 84);
            this.tbAbbreviation.Name = "tbAbbreviation";
            this.tbAbbreviation.Size = new System.Drawing.Size(183, 26);
            this.tbAbbreviation.TabIndex = 92;
            // 
            // lblCancel
            // 
            this.lblCancel.Location = new System.Drawing.Point(503, 266);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(114, 47);
            this.lblCancel.TabIndex = 90;
            this.lblCancel.TabStop = false;
            this.lblCancel.Text = "Cancel";
            this.lblCancel.UseVisualStyleBackColor = true;
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(14, -2);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(226, 24);
            this.labelTitle.TabIndex = 161;
            this.labelTitle.Text = "Edit / Add Abbreviation";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblType);
            this.groupBox1.Controls.Add(this.boxType);
            this.groupBox1.Controls.Add(this.tbExpansion);
            this.groupBox1.Controls.Add(this.lblExpansion);
            this.groupBox1.Controls.Add(this.lblFinished);
            this.groupBox1.Controls.Add(this.lblCancel);
            this.groupBox1.Controls.Add(this.labelTitle);
            this.groupBox1.Location = new System.Drawing.Point(9, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(739, 336);
            this.groupBox1.TabIndex = 172;
            this.groupBox1.TabStop = false;
            // 
            // AbbreviationEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 360);
            this.Controls.Add(this.lblAbbreviation);
            this.Controls.Add(this.tbAbbreviation);
            this.Controls.Add(this.groupBox1);
            this.Name = "AbbreviationEditorForm";
            this.Text = "Abbreviations Editor";
            this.boxType.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExpansion;
        private System.Windows.Forms.TextBox tbExpansion;
        private System.Windows.Forms.Label lblSpoken;
        private System.Windows.Forms.Button lblFinished;
        private System.Windows.Forms.Label lblAbbreviation;
        private System.Windows.Forms.Panel boxType;
        private System.Windows.Forms.Label lblWritten;
        private System.Windows.Forms.Label pbTypeSpoken;
        private System.Windows.Forms.Label pbTypeWritten;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox tbAbbreviation;
        private System.Windows.Forms.Button lblCancel;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}