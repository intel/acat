////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ACATConfigMainForm.cs
//
// Main form for ACAT Config application
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Main form for ACAT Config application
    /// </summary>
    partial class ACATConfigMainForm
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelSpacerTop = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelHeader = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExitTopRight = new System.Windows.Forms.Button();
            this.labelACATConfigTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanelConfigMain = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxCategoryTextToSpeech = new System.Windows.Forms.CheckBox();
            this.checkBoxCategoryActuators = new System.Windows.Forms.CheckBox();
            this.checkBoxCategoryGeneral = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelConfigSettings = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxCategoryWordPrediction = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxWrapText = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanelNavigationButtons = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExitBottomLeft = new System.Windows.Forms.Button();
            this.buttonResetToDefault = new System.Windows.Forms.Button();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSave = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelHeader.SuspendLayout();
            this.tableLayoutPanelConfigMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelNavigationButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.AutoSize = true;
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.ColumnCount = 3;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerTop, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelHeader, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelConfigMain, 1, 3);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel1, 1, 4);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelNavigationButtons, 1, 5);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 1, 6);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 7;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1918, 1078);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelSpacerTop
            // 
            this.tableLayoutPanelSpacerTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerTop.ColumnCount = 1;
            this.tableLayoutPanelSpacerTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelSpacerTop.Location = new System.Drawing.Point(134, 0);
            this.tableLayoutPanelSpacerTop.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerTop.Name = "tableLayoutPanelSpacerTop";
            this.tableLayoutPanelSpacerTop.RowCount = 1;
            this.tableLayoutPanelSpacerTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Size = new System.Drawing.Size(1649, 64);
            this.tableLayoutPanelSpacerTop.TabIndex = 74;
            // 
            // tableLayoutPanelHeader
            // 
            this.tableLayoutPanelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelHeader.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelHeader.ColumnCount = 3;
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelHeader.Controls.Add(this.buttonExitTopRight, 2, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.labelACATConfigTitle, 0, 0);
            this.tableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelHeader.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tableLayoutPanelHeader.Location = new System.Drawing.Point(134, 64);
            this.tableLayoutPanelHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            this.tableLayoutPanelHeader.RowCount = 1;
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHeader.Size = new System.Drawing.Size(1649, 97);
            this.tableLayoutPanelHeader.TabIndex = 78;
            // 
            // buttonExitTopRight
            // 
            this.buttonExitTopRight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonExitTopRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExitTopRight.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonExitTopRight.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExitTopRight.FlatAppearance.BorderSize = 0;
            this.buttonExitTopRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExitTopRight.Font = new System.Drawing.Font("Montserrat Thin", 58F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExitTopRight.ForeColor = System.Drawing.Color.Silver;
            this.buttonExitTopRight.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExitTopRight.Location = new System.Drawing.Point(1563, 2);
            this.buttonExitTopRight.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExitTopRight.Name = "buttonExitTopRight";
            this.buttonExitTopRight.Size = new System.Drawing.Size(86, 92);
            this.buttonExitTopRight.TabIndex = 78;
            this.buttonExitTopRight.Text = "X";
            this.buttonExitTopRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonExitTopRight.UseCompatibleTextRendering = true;
            this.buttonExitTopRight.UseVisualStyleBackColor = false;
            this.buttonExitTopRight.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // labelACATConfigTitle
            // 
            this.labelACATConfigTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelACATConfigTitle.Font = new System.Drawing.Font("Montserrat Medium", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelACATConfigTitle.ForeColor = System.Drawing.Color.White;
            this.labelACATConfigTitle.Location = new System.Drawing.Point(0, 0);
            this.labelACATConfigTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelACATConfigTitle.Name = "labelACATConfigTitle";
            this.labelACATConfigTitle.Size = new System.Drawing.Size(549, 97);
            this.labelACATConfigTitle.TabIndex = 62;
            this.labelACATConfigTitle.Text = "ACAT Configuration";
            // 
            // tableLayoutPanelConfigMain
            // 
            this.tableLayoutPanelConfigMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelConfigMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelConfigMain.ColumnCount = 3;
            this.tableLayoutPanelConfigMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanelConfigMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tableLayoutPanelConfigMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanelConfigMain.Controls.Add(this.checkBoxCategoryTextToSpeech, 0, 2);
            this.tableLayoutPanelConfigMain.Controls.Add(this.checkBoxCategoryActuators, 0, 1);
            this.tableLayoutPanelConfigMain.Controls.Add(this.checkBoxCategoryGeneral, 0, 0);
            this.tableLayoutPanelConfigMain.Controls.Add(this.tableLayoutPanelConfigSettings, 2, 0);
            this.tableLayoutPanelConfigMain.Controls.Add(this.checkBoxCategoryWordPrediction, 0, 3);
            this.tableLayoutPanelConfigMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelConfigMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tableLayoutPanelConfigMain.Location = new System.Drawing.Point(134, 225);
            this.tableLayoutPanelConfigMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelConfigMain.Name = "tableLayoutPanelConfigMain";
            this.tableLayoutPanelConfigMain.RowCount = 7;
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.89187F));
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.89187F));
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.89187F));
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.89187F));
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477504F));
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477504F));
            this.tableLayoutPanelConfigMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477504F));
            this.tableLayoutPanelConfigMain.Size = new System.Drawing.Size(1649, 657);
            this.tableLayoutPanelConfigMain.TabIndex = 60;
            // 
            // checkBoxCategoryTextToSpeech
            // 
            this.checkBoxCategoryTextToSpeech.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxCategoryTextToSpeech.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBoxCategoryTextToSpeech.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxCategoryTextToSpeech.FlatAppearance.BorderSize = 0;
            this.checkBoxCategoryTextToSpeech.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.checkBoxCategoryTextToSpeech.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxCategoryTextToSpeech.Font = new System.Drawing.Font("Montserrat Medium", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCategoryTextToSpeech.ForeColor = System.Drawing.Color.White;
            this.checkBoxCategoryTextToSpeech.Location = new System.Drawing.Point(0, 234);
            this.checkBoxCategoryTextToSpeech.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxCategoryTextToSpeech.Name = "checkBoxCategoryTextToSpeech";
            this.checkBoxCategoryTextToSpeech.Size = new System.Drawing.Size(280, 117);
            this.checkBoxCategoryTextToSpeech.TabIndex = 15;
            this.checkBoxCategoryTextToSpeech.Text = "Text to Speech";
            this.checkBoxCategoryTextToSpeech.UseVisualStyleBackColor = true;
            this.checkBoxCategoryTextToSpeech.Click += new System.EventHandler(this.handleConfigCategorySelected);
            // 
            // checkBoxCategoryActuators
            // 
            this.checkBoxCategoryActuators.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxCategoryActuators.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBoxCategoryActuators.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxCategoryActuators.FlatAppearance.BorderSize = 0;
            this.checkBoxCategoryActuators.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.checkBoxCategoryActuators.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxCategoryActuators.Font = new System.Drawing.Font("Montserrat Medium", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCategoryActuators.ForeColor = System.Drawing.Color.White;
            this.checkBoxCategoryActuators.Location = new System.Drawing.Point(0, 117);
            this.checkBoxCategoryActuators.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxCategoryActuators.Name = "checkBoxCategoryActuators";
            this.checkBoxCategoryActuators.Size = new System.Drawing.Size(280, 117);
            this.checkBoxCategoryActuators.TabIndex = 14;
            this.checkBoxCategoryActuators.Text = "Actuators";
            this.checkBoxCategoryActuators.UseVisualStyleBackColor = true;
            this.checkBoxCategoryActuators.Click += new System.EventHandler(this.handleConfigCategorySelected);
            // 
            // checkBoxCategoryGeneral
            // 
            this.checkBoxCategoryGeneral.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxCategoryGeneral.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBoxCategoryGeneral.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxCategoryGeneral.FlatAppearance.BorderSize = 0;
            this.checkBoxCategoryGeneral.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.checkBoxCategoryGeneral.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxCategoryGeneral.Font = new System.Drawing.Font("Montserrat Medium", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCategoryGeneral.ForeColor = System.Drawing.Color.White;
            this.checkBoxCategoryGeneral.Location = new System.Drawing.Point(0, 0);
            this.checkBoxCategoryGeneral.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxCategoryGeneral.Name = "checkBoxCategoryGeneral";
            this.checkBoxCategoryGeneral.Size = new System.Drawing.Size(280, 117);
            this.checkBoxCategoryGeneral.TabIndex = 13;
            this.checkBoxCategoryGeneral.Text = "General";
            this.checkBoxCategoryGeneral.UseVisualStyleBackColor = true;
            this.checkBoxCategoryGeneral.Click += new System.EventHandler(this.handleConfigCategorySelected);
            // 
            // tableLayoutPanelConfigSettings
            // 
            this.tableLayoutPanelConfigSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelConfigSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelConfigSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelConfigSettings.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold);
            this.tableLayoutPanelConfigSettings.Location = new System.Drawing.Point(329, 0);
            this.tableLayoutPanelConfigSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelConfigSettings.Name = "tableLayoutPanelConfigSettings";
            this.tableLayoutPanelConfigSettings.RowCount = 1;
            this.tableLayoutPanelConfigMain.SetRowSpan(this.tableLayoutPanelConfigSettings, 7);
            this.tableLayoutPanelConfigSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelConfigSettings.Size = new System.Drawing.Size(1320, 657);
            this.tableLayoutPanelConfigSettings.TabIndex = 2;
            // 
            // checkBoxCategoryWordPrediction
            // 
            this.checkBoxCategoryWordPrediction.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxCategoryWordPrediction.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBoxCategoryWordPrediction.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxCategoryWordPrediction.FlatAppearance.BorderSize = 0;
            this.checkBoxCategoryWordPrediction.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.checkBoxCategoryWordPrediction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxCategoryWordPrediction.Font = new System.Drawing.Font("Montserrat Medium", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCategoryWordPrediction.ForeColor = System.Drawing.Color.White;
            this.checkBoxCategoryWordPrediction.Location = new System.Drawing.Point(0, 351);
            this.checkBoxCategoryWordPrediction.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxCategoryWordPrediction.Name = "checkBoxCategoryWordPrediction";
            this.checkBoxCategoryWordPrediction.Size = new System.Drawing.Size(280, 117);
            this.checkBoxCategoryWordPrediction.TabIndex = 16;
            this.checkBoxCategoryWordPrediction.Text = "Word Prediction";
            this.checkBoxCategoryWordPrediction.UseVisualStyleBackColor = true;
            this.checkBoxCategoryWordPrediction.Click += new System.EventHandler(this.handleConfigCategorySelected);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.Controls.Add(this.checkBoxWrapText, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(134, 882);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1649, 64);
            this.tableLayoutPanel1.TabIndex = 76;
            // 
            // checkBoxWrapText
            // 
            this.checkBoxWrapText.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.checkBoxWrapText.FlatAppearance.BorderSize = 0;
            this.checkBoxWrapText.Font = new System.Drawing.Font("Montserrat Medium", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxWrapText.ForeColor = System.Drawing.Color.White;
            this.checkBoxWrapText.Location = new System.Drawing.Point(329, 0);
            this.checkBoxWrapText.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxWrapText.Name = "checkBoxWrapText";
            this.checkBoxWrapText.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.checkBoxWrapText.Size = new System.Drawing.Size(166, 61);
            this.checkBoxWrapText.TabIndex = 13;
            this.checkBoxWrapText.Text = "Wrap Text";
            this.checkBoxWrapText.UseVisualStyleBackColor = true;
            this.checkBoxWrapText.Visible = false;
            this.checkBoxWrapText.Click += new System.EventHandler(this.checkBoxWrapText_Click);
            // 
            // tableLayoutPanelNavigationButtons
            // 
            this.tableLayoutPanelNavigationButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelNavigationButtons.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelNavigationButtons.ColumnCount = 3;
            this.tableLayoutPanelNavigationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelNavigationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelNavigationButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelNavigationButtons.Controls.Add(this.buttonExitBottomLeft, 0, 0);
            this.tableLayoutPanelNavigationButtons.Controls.Add(this.buttonResetToDefault, 1, 0);
            this.tableLayoutPanelNavigationButtons.Controls.Add(this.buttonSave, 2, 0);
            this.tableLayoutPanelNavigationButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelNavigationButtons.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tableLayoutPanelNavigationButtons.Location = new System.Drawing.Point(134, 946);
            this.tableLayoutPanelNavigationButtons.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelNavigationButtons.Name = "tableLayoutPanelNavigationButtons";
            this.tableLayoutPanelNavigationButtons.RowCount = 1;
            this.tableLayoutPanelNavigationButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelNavigationButtons.Size = new System.Drawing.Size(1649, 64);
            this.tableLayoutPanelNavigationButtons.TabIndex = 4;
            // 
            // buttonExitBottomLeft
            // 
            this.buttonExitBottomLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExitBottomLeft.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExitBottomLeft.FlatAppearance.BorderSize = 0;
            this.buttonExitBottomLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExitBottomLeft.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExitBottomLeft.ForeColor = System.Drawing.Color.Silver;
            this.buttonExitBottomLeft.Location = new System.Drawing.Point(0, 0);
            this.buttonExitBottomLeft.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExitBottomLeft.Name = "buttonExitBottomLeft";
            this.buttonExitBottomLeft.Size = new System.Drawing.Size(155, 64);
            this.buttonExitBottomLeft.TabIndex = 4;
            this.buttonExitBottomLeft.Text = "Exit";
            this.buttonExitBottomLeft.UseCompatibleTextRendering = true;
            this.buttonExitBottomLeft.UseVisualStyleBackColor = false;
            this.buttonExitBottomLeft.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonResetToDefault
            // 
            this.buttonResetToDefault.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonResetToDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonResetToDefault.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonResetToDefault.FlatAppearance.BorderSize = 0;
            this.buttonResetToDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonResetToDefault.Font = new System.Drawing.Font("Montserrat Thin", 28F, System.Drawing.FontStyle.Underline);
            this.buttonResetToDefault.ForeColor = System.Drawing.Color.Silver;
            this.buttonResetToDefault.Location = new System.Drawing.Point(617, 0);
            this.buttonResetToDefault.Margin = new System.Windows.Forms.Padding(0);
            this.buttonResetToDefault.Name = "buttonResetToDefault";
            this.buttonResetToDefault.Size = new System.Drawing.Size(413, 64);
            this.buttonResetToDefault.TabIndex = 3;
            this.buttonResetToDefault.Text = "Reset to default";
            this.buttonResetToDefault.UseCompatibleTextRendering = true;
            this.buttonResetToDefault.UseVisualStyleBackColor = false;
            this.buttonResetToDefault.Visible = false;
            this.buttonResetToDefault.Click += new System.EventHandler(this.buttonResetToDefault_Click);
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(134, 1010);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(1649, 68);
            this.tableLayoutPanelSpacerBottom.TabIndex = 75;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonSave.BorderColor = System.Drawing.Color.Black;
            this.buttonSave.BorderRadiusBottomLeft = 0;
            this.buttonSave.BorderRadiusBottomRight = 0;
            this.buttonSave.BorderRadiusTopLeft = 0;
            this.buttonSave.BorderRadiusTopRight = 0;
            this.buttonSave.BorderWidth = 2F;
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Font = new System.Drawing.Font("Montserrat Medium", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(1499, 0);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(0);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 64);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseCompatibleTextRendering = true;
            this.buttonSave.UseMnemonic = false;
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Visible = false;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // ACATConfigMainForm
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(1918, 1078);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ACATConfigMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelHeader.ResumeLayout(false);
            this.tableLayoutPanelConfigMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanelNavigationButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelConfigMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelConfigSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelNavigationButtons;
        private ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonSave;
        private System.Windows.Forms.Button buttonResetToDefault;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerTop;
        public System.Windows.Forms.CheckBox checkBoxCategoryGeneral;
        public System.Windows.Forms.CheckBox checkBoxCategoryTextToSpeech;
        public System.Windows.Forms.CheckBox checkBoxCategoryActuators;
        public System.Windows.Forms.CheckBox checkBoxCategoryWordPrediction;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxWrapText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
        private System.Windows.Forms.Button buttonExitTopRight;
        private System.Windows.Forms.Label labelACATConfigTitle;
        private System.Windows.Forms.Button buttonExitBottomLeft;
    }
}

