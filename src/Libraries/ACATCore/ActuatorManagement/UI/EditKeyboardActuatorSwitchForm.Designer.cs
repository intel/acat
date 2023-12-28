////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EditKeyboardActuatorSwitchForm.cs
//
// Configure keyboard actuator switch form
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.ActuatorManagement.UI
{
    partial class EditKeyboardActuatorSwitchForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxKeyboardShortcut = new System.Windows.Forms.TextBox();
            this.buttonCommand = new System.Windows.Forms.Button();
            this.radioButtonMapToCommand = new System.Windows.Forms.RadioButton();
            this.radioButtonTriggerSelect = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(51, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(530, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "Click in the box below and press shortcut keys";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxKeyboardShortcut
            // 
            this.textBoxKeyboardShortcut.BackColor = System.Drawing.Color.White;
            this.textBoxKeyboardShortcut.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Bold);
            this.textBoxKeyboardShortcut.Location = new System.Drawing.Point(46, 91);
            this.textBoxKeyboardShortcut.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxKeyboardShortcut.Name = "textBoxKeyboardShortcut";
            this.textBoxKeyboardShortcut.ReadOnly = true;
            this.textBoxKeyboardShortcut.Size = new System.Drawing.Size(530, 34);
            this.textBoxKeyboardShortcut.TabIndex = 1;
            // 
            // buttonCommand
            // 
            this.buttonCommand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonCommand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCommand.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Bold);
            this.buttonCommand.Location = new System.Drawing.Point(331, 0);
            this.buttonCommand.Margin = new System.Windows.Forms.Padding(0);
            this.buttonCommand.Name = "buttonCommand";
            this.buttonCommand.Size = new System.Drawing.Size(403, 45);
            this.buttonCommand.TabIndex = 6;
            this.buttonCommand.UseVisualStyleBackColor = false;
            this.buttonCommand.Click += new System.EventHandler(this.buttonCommand_Click);
            // 
            // radioButtonMapToCommand
            // 
            this.radioButtonMapToCommand.AutoSize = true;
            this.radioButtonMapToCommand.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Bold);
            this.radioButtonMapToCommand.ForeColor = System.Drawing.Color.White;
            this.radioButtonMapToCommand.Location = new System.Drawing.Point(3, 3);
            this.radioButtonMapToCommand.Name = "radioButtonMapToCommand";
            this.radioButtonMapToCommand.Size = new System.Drawing.Size(252, 34);
            this.radioButtonMapToCommand.TabIndex = 5;
            this.radioButtonMapToCommand.TabStop = true;
            this.radioButtonMapToCommand.Text = "Map to a Command";
            this.radioButtonMapToCommand.UseVisualStyleBackColor = true;
            this.radioButtonMapToCommand.CheckedChanged += new System.EventHandler(this.radioButtonMapToCommand_CheckedChanged);
            // 
            // radioButtonTriggerSelect
            // 
            this.radioButtonTriggerSelect.AutoSize = true;
            this.radioButtonTriggerSelect.Font = new System.Drawing.Font("Montserrat Medium", 16F, System.Drawing.FontStyle.Bold);
            this.radioButtonTriggerSelect.ForeColor = System.Drawing.Color.White;
            this.radioButtonTriggerSelect.Location = new System.Drawing.Point(51, 228);
            this.radioButtonTriggerSelect.Margin = new System.Windows.Forms.Padding(5);
            this.radioButtonTriggerSelect.Name = "radioButtonTriggerSelect";
            this.radioButtonTriggerSelect.Size = new System.Drawing.Size(262, 34);
            this.radioButtonTriggerSelect.TabIndex = 4;
            this.radioButtonTriggerSelect.TabStop = true;
            this.radioButtonTriggerSelect.Text = "Use as Trigger Select";
            this.radioButtonTriggerSelect.UseVisualStyleBackColor = true;
            this.radioButtonTriggerSelect.CheckedChanged += new System.EventHandler(this.radioButtonTriggerSelect_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.radioButtonTriggerSelect, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxKeyboardShortcut, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.89685F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.40656F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.75695F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.68944F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.2502F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(921, 544);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Controls.Add(this.buttonCommand, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButtonMapToCommand, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(46, 319);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(828, 123);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.56391F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.06015F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.06266F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.81454F));
            this.tableLayoutPanel3.Controls.Add(this.buttonCancel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonOK, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(46, 442);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(828, 102);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.Font = new System.Drawing.Font("Montserrat Medium", 18F, System.Drawing.FontStyle.Bold);
            this.buttonOK.Location = new System.Drawing.Point(748, 28);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(0);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 45);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonCancel.FlatAppearance.BorderSize = 0;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Font = new System.Drawing.Font("Montserrat Thin", 22F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.ForeColor = System.Drawing.Color.Silver;
            this.buttonCancel.Location = new System.Drawing.Point(0, 23);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(153, 55);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // EditKeyboardActuatorSwitchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(921, 544);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditKeyboardActuatorSwitchForm";
            this.Text = "Keyboard Shortcut";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxKeyboardShortcut;
        private System.Windows.Forms.RadioButton radioButtonMapToCommand;
        private System.Windows.Forms.RadioButton radioButtonTriggerSelect;
        private System.Windows.Forms.Button buttonCommand;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}