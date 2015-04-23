namespace Aster.Extensions.Base.UI.Dialogs
{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class AlternatePronunciationEditorForm
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
            this.lbTerm = new System.Windows.Forms.Label();
            this.tbOriginalTerm = new System.Windows.Forms.TextBox();
            this.tbReplacementTerm = new System.Windows.Forms.TextBox();
            this.lblAlternate = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCancel = new System.Windows.Forms.Button();
            this.lblUndo = new System.Windows.Forms.Button();
            this.lblFinished = new System.Windows.Forms.Button();
            this.Title = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbTerm
            // 
            this.lbTerm.AutoSize = true;
            this.lbTerm.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTerm.Location = new System.Drawing.Point(14, 52);
            this.lbTerm.Name = "lbTerm";
            this.lbTerm.Size = new System.Drawing.Size(143, 24);
            this.lbTerm.TabIndex = 81;
            this.lbTerm.Text = "Original Term";
            // 
            // tbOriginalTerm
            // 
            this.tbOriginalTerm.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOriginalTerm.Location = new System.Drawing.Point(18, 79);
            this.tbOriginalTerm.Name = "tbOriginalTerm";
            this.tbOriginalTerm.Size = new System.Drawing.Size(738, 26);
            this.tbOriginalTerm.TabIndex = 83;
            // 
            // tbReplacementTerm
            // 
            this.tbReplacementTerm.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbReplacementTerm.Location = new System.Drawing.Point(12, 119);
            this.tbReplacementTerm.Name = "tbReplacementTerm";
            this.tbReplacementTerm.Size = new System.Drawing.Size(738, 26);
            this.tbReplacementTerm.TabIndex = 84;
            // 
            // lblAlternate
            // 
            this.lblAlternate.AutoSize = true;
            this.lblAlternate.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlternate.Location = new System.Drawing.Point(8, 92);
            this.lblAlternate.Name = "lblAlternate";
            this.lblAlternate.Size = new System.Drawing.Size(198, 24);
            this.lblAlternate.TabIndex = 87;
            this.lblAlternate.Text = "Replacement Term";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblCancel);
            this.panel1.Controls.Add(this.tbReplacementTerm);
            this.panel1.Controls.Add(this.lblAlternate);
            this.panel1.Controls.Add(this.lblUndo);
            this.panel1.Controls.Add(this.lblFinished);
            this.panel1.Location = new System.Drawing.Point(6, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 245);
            this.panel1.TabIndex = 88;
            // 
            // lblCancel
            // 
            this.lblCancel.Location = new System.Drawing.Point(485, 171);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(152, 47);
            this.lblCancel.TabIndex = 2;
            this.lblCancel.TabStop = false;
            this.lblCancel.Text = "Cancel";
            this.lblCancel.UseVisualStyleBackColor = true;
            // 
            // lblUndo
            // 
            this.lblUndo.Location = new System.Drawing.Point(303, 171);
            this.lblUndo.Name = "lblUndo";
            this.lblUndo.Size = new System.Drawing.Size(152, 47);
            this.lblUndo.TabIndex = 1;
            this.lblUndo.TabStop = false;
            this.lblUndo.Text = "Undo";
            this.lblUndo.UseVisualStyleBackColor = true;
            // 
            // lblFinished
            // 
            this.lblFinished.Location = new System.Drawing.Point(121, 171);
            this.lblFinished.Name = "lblFinished";
            this.lblFinished.Size = new System.Drawing.Size(152, 47);
            this.lblFinished.TabIndex = 0;
            this.lblFinished.TabStop = false;
            this.lblFinished.Text = "Finished";
            this.lblFinished.UseVisualStyleBackColor = true;
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Navy;
            this.Title.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.ForeColor = System.Drawing.Color.White;
            this.Title.Location = new System.Drawing.Point(4, 2);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(766, 38);
            this.Title.TabIndex = 96;
            this.Title.TabStop = false;
            this.Title.Text = "Edit/Add Alternative Pronunciation";
            this.Title.UseVisualStyleBackColor = false;
            // 
            // AlternatePronunciationEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 292);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.tbOriginalTerm);
            this.Controls.Add(this.lbTerm);
            this.Controls.Add(this.panel1);
            this.Name = "AlternatePronunciationEditorForm";
            this.Text = "Aster Edit/Add Abbreviation";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTerm;
        private System.Windows.Forms.TextBox tbOriginalTerm;
        private System.Windows.Forms.TextBox tbReplacementTerm;
        private System.Windows.Forms.Label lblAlternate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Title;
        private System.Windows.Forms.Button lblCancel;
        private System.Windows.Forms.Button lblUndo;
        private System.Windows.Forms.Button lblFinished;
    }
}