namespace ACAT.Extensions.Default.UI.Dialogs
{
    partial class ACATTryoutForm 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACATTryoutForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.labelSteppingTimeValue = new System.Windows.Forms.ToolStripLabel();
            this.buttonSteppingTimeIncrease = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonSteppingTimeDecrease = new System.Windows.Forms.ToolStripButton();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.labelTryWord = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxEntry = new System.Windows.Forms.TextBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonBackspace = new System.Windows.Forms.Button();
            this.B4 = new System.Windows.Forms.Button();
            this.B3 = new System.Windows.Forms.Button();
            this.B2 = new System.Windows.Forms.Button();
            this.B1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.labelTryWord);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxEntry);
            this.panel1.Controls.Add(this.buttonExit);
            this.panel1.Controls.Add(this.buttonReset);
            this.panel1.Controls.Add(this.buttonBackspace);
            this.panel1.Controls.Add(this.B4);
            this.panel1.Controls.Add(this.B3);
            this.panel1.Controls.Add(this.B2);
            this.panel1.Controls.Add(this.B1);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1112, 417);
            this.panel1.TabIndex = 4;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.OldLace;
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.labelSteppingTimeValue,
            this.buttonSteppingTimeIncrease,
            this.toolStripSeparator1,
            this.buttonSteppingTimeDecrease,
            this.buttonSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1112, 39);
            this.toolStrip1.TabIndex = 137;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(144, 36);
            this.toolStripLabel1.Text = "Scan Speed:";
            // 
            // labelSteppingTimeValue
            // 
            this.labelSteppingTimeValue.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSteppingTimeValue.Name = "labelSteppingTimeValue";
            this.labelSteppingTimeValue.Size = new System.Drawing.Size(127, 36);
            this.labelSteppingTimeValue.Text = "(50 msecs)";
            // 
            // buttonSteppingTimeIncrease
            // 
            this.buttonSteppingTimeIncrease.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonSteppingTimeIncrease.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonSteppingTimeIncrease.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSteppingTimeIncrease.Image = ((System.Drawing.Image)(resources.GetObject("buttonSteppingTimeIncrease.Image")));
            this.buttonSteppingTimeIncrease.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSteppingTimeIncrease.Name = "buttonSteppingTimeIncrease";
            this.buttonSteppingTimeIncrease.Size = new System.Drawing.Size(82, 36);
            this.buttonSteppingTimeIncrease.Text = "Faster";
            this.buttonSteppingTimeIncrease.ToolTipText = "Increase Scan Speed";
            this.buttonSteppingTimeIncrease.Click += new System.EventHandler(this.buttonSteppingTimeIncrease_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // buttonSteppingTimeDecrease
            // 
            this.buttonSteppingTimeDecrease.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonSteppingTimeDecrease.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonSteppingTimeDecrease.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSteppingTimeDecrease.Image = ((System.Drawing.Image)(resources.GetObject("buttonSteppingTimeDecrease.Image")));
            this.buttonSteppingTimeDecrease.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSteppingTimeDecrease.Name = "buttonSteppingTimeDecrease";
            this.buttonSteppingTimeDecrease.Size = new System.Drawing.Size(90, 36);
            this.buttonSteppingTimeDecrease.Text = "Slower";
            this.buttonSteppingTimeDecrease.ToolTipText = "Decrease Scan Speed";
            this.buttonSteppingTimeDecrease.Click += new System.EventHandler(this.buttonSteppingTimeDecrease_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buttonSave.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonSave.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(69, 36);
            this.buttonSave.Text = "Save";
            this.buttonSave.ToolTipText = "Save Scan Speed";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelTryWord
            // 
            this.labelTryWord.Font = new System.Drawing.Font("Arial", 63.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTryWord.ForeColor = System.Drawing.Color.Red;
            this.labelTryWord.Location = new System.Drawing.Point(745, 47);
            this.labelTryWord.Name = "labelTryWord";
            this.labelTryWord.Size = new System.Drawing.Size(343, 101);
            this.labelTryWord.TabIndex = 136;
            this.labelTryWord.Text = "Test";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 33.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(521, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 53);
            this.label1.TabIndex = 135;
            this.label1.Text = "Type this: ";
            // 
            // textBoxEntry
            // 
            this.textBoxEntry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEntry.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEntry.Location = new System.Drawing.Point(42, 66);
            this.textBoxEntry.Name = "textBoxEntry";
            this.textBoxEntry.ReadOnly = true;
            this.textBoxEntry.Size = new System.Drawing.Size(456, 63);
            this.textBoxEntry.TabIndex = 134;
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(821, 283);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(252, 81);
            this.buttonExit.TabIndex = 133;
            this.buttonExit.TabStop = false;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(950, 174);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(123, 103);
            this.buttonReset.TabIndex = 132;
            this.buttonReset.TabStop = false;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            // 
            // buttonBackspace
            // 
            this.buttonBackspace.Location = new System.Drawing.Point(821, 174);
            this.buttonBackspace.Name = "buttonBackspace";
            this.buttonBackspace.Size = new System.Drawing.Size(123, 103);
            this.buttonBackspace.TabIndex = 131;
            this.buttonBackspace.TabStop = false;
            this.buttonBackspace.Text = "Backspace";
            this.buttonBackspace.UseVisualStyleBackColor = true;
            // 
            // B4
            // 
            this.B4.Location = new System.Drawing.Point(625, 174);
            this.B4.Name = "B4";
            this.B4.Size = new System.Drawing.Size(190, 190);
            this.B4.TabIndex = 130;
            this.B4.TabStop = false;
            this.B4.Text = "s";
            this.B4.UseVisualStyleBackColor = true;
            // 
            // B3
            // 
            this.B3.Location = new System.Drawing.Point(429, 174);
            this.B3.Name = "B3";
            this.B3.Size = new System.Drawing.Size(190, 190);
            this.B3.TabIndex = 129;
            this.B3.TabStop = false;
            this.B3.Text = "b";
            this.B3.UseVisualStyleBackColor = true;
            // 
            // B2
            // 
            this.B2.Location = new System.Drawing.Point(233, 174);
            this.B2.Name = "B2";
            this.B2.Size = new System.Drawing.Size(190, 190);
            this.B2.TabIndex = 128;
            this.B2.TabStop = false;
            this.B2.Text = "t";
            this.B2.UseVisualStyleBackColor = true;
            // 
            // B1
            // 
            this.B1.Font = new System.Drawing.Font("Microsoft Sans Serif", 120F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B1.Location = new System.Drawing.Point(37, 174);
            this.B1.Name = "B1";
            this.B1.Size = new System.Drawing.Size(190, 190);
            this.B1.TabIndex = 127;
            this.B1.TabStop = false;
            this.B1.Text = "a";
            this.B1.UseVisualStyleBackColor = true;
            // 
            // ACATTryoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1117, 421);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ACATTryoutForm";
            this.ShowIcon = false;
            this.Text = "ACAT Tryout";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button B3;
        private System.Windows.Forms.Button B2;
        private System.Windows.Forms.Button B1;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonBackspace;
        private System.Windows.Forms.Button B4;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label labelTryWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxEntry;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel labelSteppingTimeValue;
        private System.Windows.Forms.ToolStripButton buttonSteppingTimeIncrease;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonSteppingTimeDecrease;
        private System.Windows.Forms.ToolStripButton buttonSave;


    }
}

