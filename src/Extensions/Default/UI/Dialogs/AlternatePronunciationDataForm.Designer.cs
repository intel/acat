namespace Aster.Extensions.Base.UI.Dialogs
{
    // TODO see if we should make a base class to encapsulate these three inherited classes/interfaces
    partial class AlternatePronunciationDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlternatePronunciationDataForm));
            this.listViewAP = new System.Windows.Forms.ListView();
            this.chTerm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlternate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCancel = new System.Windows.Forms.Button();
            this.lblOK = new System.Windows.Forms.Button();
            this.lblDelete = new System.Windows.Forms.Button();
            this.lblEdit = new System.Windows.Forms.Button();
            this.lblAdd = new System.Windows.Forms.Button();
            this.Title = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewAP
            // 
            this.listViewAP.AutoArrange = false;
            this.listViewAP.CausesValidation = false;
            this.listViewAP.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTerm,
            this.chAlternate});
            this.listViewAP.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewAP.FullRowSelect = true;
            this.listViewAP.GridLines = true;
            this.listViewAP.Location = new System.Drawing.Point(25, 16);
            this.listViewAP.MultiSelect = false;
            this.listViewAP.Name = "listViewAP";
            this.listViewAP.Size = new System.Drawing.Size(623, 316);
            this.listViewAP.TabIndex = 81;
            this.listViewAP.UseCompatibleStateImageBehavior = false;
            this.listViewAP.View = System.Windows.Forms.View.Details;
            // 
            // chTerm
            // 
            this.chTerm.Text = "Original Term";
            this.chTerm.Width = 212;
            // 
            // chAlternate
            // 
            this.chAlternate.Text = "Alternative Pronunciation";
            this.chAlternate.Width = 406;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblCancel);
            this.panel1.Controls.Add(this.listViewAP);
            this.panel1.Controls.Add(this.lblOK);
            this.panel1.Controls.Add(this.lblDelete);
            this.panel1.Controls.Add(this.lblEdit);
            this.panel1.Controls.Add(this.lblAdd);
            this.panel1.Location = new System.Drawing.Point(5, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(866, 419);
            this.panel1.TabIndex = 94;
            // 
            // lblCancel
            // 
            this.lblCancel.Location = new System.Drawing.Point(368, 354);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(152, 47);
            this.lblCancel.TabIndex = 4;
            this.lblCancel.TabStop = false;
            this.lblCancel.Text = "Cancel";
            this.lblCancel.UseVisualStyleBackColor = true;
            // 
            // lblOK
            // 
            this.lblOK.Location = new System.Drawing.Point(158, 354);
            this.lblOK.Name = "lblOK";
            this.lblOK.Size = new System.Drawing.Size(152, 47);
            this.lblOK.TabIndex = 3;
            this.lblOK.TabStop = false;
            this.lblOK.Text = "OK";
            this.lblOK.UseVisualStyleBackColor = true;
            // 
            // lblDelete
            // 
            this.lblDelete.Location = new System.Drawing.Point(697, 247);
            this.lblDelete.Name = "lblDelete";
            this.lblDelete.Size = new System.Drawing.Size(123, 47);
            this.lblDelete.TabIndex = 2;
            this.lblDelete.TabStop = false;
            this.lblDelete.Text = "Delete";
            this.lblDelete.UseVisualStyleBackColor = true;
            // 
            // lblEdit
            // 
            this.lblEdit.Location = new System.Drawing.Point(697, 150);
            this.lblEdit.Name = "lblEdit";
            this.lblEdit.Size = new System.Drawing.Size(123, 47);
            this.lblEdit.TabIndex = 1;
            this.lblEdit.TabStop = false;
            this.lblEdit.Text = "Edit";
            this.lblEdit.UseVisualStyleBackColor = true;
            // 
            // lblAdd
            // 
            this.lblAdd.Location = new System.Drawing.Point(697, 53);
            this.lblAdd.Name = "lblAdd";
            this.lblAdd.Size = new System.Drawing.Size(123, 47);
            this.lblAdd.TabIndex = 0;
            this.lblAdd.TabStop = false;
            this.lblAdd.Text = "Add";
            this.lblAdd.UseVisualStyleBackColor = true;
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Navy;
            this.Title.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.ForeColor = System.Drawing.Color.White;
            this.Title.Location = new System.Drawing.Point(5, 2);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(866, 38);
            this.Title.TabIndex = 95;
            this.Title.TabStop = false;
            this.Title.Text = "Alternative Pronunciations";
            this.Title.UseVisualStyleBackColor = false;
            // 
            // AlternatePronunciationDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 467);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AlternatePronunciationDataForm";
            this.Text = "Aster Abbreviations";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewAP;
        private System.Windows.Forms.ColumnHeader chTerm;
        private System.Windows.Forms.ColumnHeader chAlternate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Title;
        private System.Windows.Forms.Button lblDelete;
        private System.Windows.Forms.Button lblEdit;
        private System.Windows.Forms.Button lblAdd;
        private System.Windows.Forms.Button lblCancel;
        private System.Windows.Forms.Button lblOK;
    }
}