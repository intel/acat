////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIFilterSettings.designer.cs
//
// User control which prompts the user for input to select the best filter setting
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which prompts the user for input to select the best filter setting
    /// </summary>
    partial class UserControlBCIFilterSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonExit = new System.Windows.Forms.Button();
            this.tableLayoutPanelSpacerTop = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.checkBoxConfirm60HzCountry = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel60HzCountries = new System.Windows.Forms.TableLayoutPanel();
            this.label47 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanelSpacerBottom = new System.Windows.Forms.TableLayoutPanel();
            this.buttonNext = new ACAT.Lib.Core.WidgetManagement.ScannerRoundedButtonControl();
            this.checkBoxDontShowStartup = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanel60HzCountries.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.77361F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.27264F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.10732F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.07282F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.77361F));
            this.tableLayoutPanelMain.Controls.Add(this.buttonExit, 1, 10);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerTop, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.checkBoxConfirm60HzCountry, 1, 6);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel60HzCountries, 1, 8);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSpacerBottom, 1, 11);
            this.tableLayoutPanelMain.Controls.Add(this.buttonNext, 3, 10);
            this.tableLayoutPanelMain.Controls.Add(this.checkBoxDontShowStartup, 2, 10);
            this.tableLayoutPanelMain.Controls.Add(this.label22, 1, 2);
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 12;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.031852F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.567685F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.031852F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.977672F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.031852F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.440623F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.031852F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.394637F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.73765F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.031852F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.690615F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.031852F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.TabIndex = 9;
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonExit.AutoSize = true;
            this.buttonExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Font = new System.Drawing.Font("Montserrat Thin", 36F, System.Drawing.FontStyle.Underline);
            this.buttonExit.ForeColor = System.Drawing.Color.Silver;
            this.buttonExit.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonExit.Location = new System.Drawing.Point(140, 670);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(0);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(155, 58);
            this.buttonExit.TabIndex = 84;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseCompatibleTextRendering = true;
            this.buttonExit.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanelSpacerTop
            // 
            this.tableLayoutPanelSpacerTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerTop.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerTop, 3);
            this.tableLayoutPanelSpacerTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Location = new System.Drawing.Point(140, 0);
            this.tableLayoutPanelSpacerTop.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerTop.Name = "tableLayoutPanelSpacerTop";
            this.tableLayoutPanelSpacerTop.RowCount = 1;
            this.tableLayoutPanelSpacerTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerTop.Size = new System.Drawing.Size(739, 30);
            this.tableLayoutPanelSpacerTop.TabIndex = 73;
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelMain.SetColumnSpan(this.labelTitle, 3);
            this.labelTitle.Font = new System.Drawing.Font("Montserrat", 44F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(312, 30);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(394, 65);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "BCI Settings";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.UseCompatibleTextRendering = true;
            // 
            // checkBoxConfirm60HzCountry
            // 
            this.checkBoxConfirm60HzCountry.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBoxConfirm60HzCountry.AutoSize = true;
            this.checkBoxConfirm60HzCountry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.checkBoxConfirm60HzCountry.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanelMain.SetColumnSpan(this.checkBoxConfirm60HzCountry, 3);
            this.checkBoxConfirm60HzCountry.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.checkBoxConfirm60HzCountry.FlatAppearance.BorderSize = 0;
            this.checkBoxConfirm60HzCountry.Font = new System.Drawing.Font("Montserrat", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxConfirm60HzCountry.ForeColor = System.Drawing.Color.White;
            this.checkBoxConfirm60HzCountry.Location = new System.Drawing.Point(229, 219);
            this.checkBoxConfirm60HzCountry.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxConfirm60HzCountry.Name = "checkBoxConfirm60HzCountry";
            this.checkBoxConfirm60HzCountry.Size = new System.Drawing.Size(560, 30);
            this.checkBoxConfirm60HzCountry.TabIndex = 82;
            this.checkBoxConfirm60HzCountry.Text = "Check this box if you are located in one of these regions​";
            this.checkBoxConfirm60HzCountry.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.checkBoxConfirm60HzCountry.UseCompatibleTextRendering = true;
            this.checkBoxConfirm60HzCountry.UseMnemonic = false;
            this.checkBoxConfirm60HzCountry.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanel60HzCountries
            // 
            this.tableLayoutPanel60HzCountries.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel60HzCountries.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanel60HzCountries.ColumnCount = 3;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanel60HzCountries, 3);
            this.tableLayoutPanel60HzCountries.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel60HzCountries.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel60HzCountries.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label47, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label46, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label45, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label44, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label43, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label42, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label41, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label40, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label39, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label38, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label37, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label36, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label35, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label34, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label33, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label32, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label31, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label30, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label29, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label28, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label27, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label26, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label25, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label24, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label19, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label18, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label17, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label16, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label15, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label14, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label13, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label12, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label11, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label10, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label9, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label8, 2, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label7, 1, 0);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label23, 1, 14);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label6, 0, 14);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label21, 0, 15);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label20, 2, 14);
            this.tableLayoutPanel60HzCountries.Controls.Add(this.label5, 2, 13);
            this.tableLayoutPanel60HzCountries.Font = new System.Drawing.Font("Montserrat Medium", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel60HzCountries.ForeColor = System.Drawing.Color.White;
            this.tableLayoutPanel60HzCountries.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tableLayoutPanel60HzCountries.Location = new System.Drawing.Point(140, 282);
            this.tableLayoutPanel60HzCountries.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel60HzCountries.Name = "tableLayoutPanel60HzCountries";
            this.tableLayoutPanel60HzCountries.RowCount = 16;
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.249498F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.252173F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.252173F));
            this.tableLayoutPanel60HzCountries.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.252173F));
            this.tableLayoutPanel60HzCountries.Size = new System.Drawing.Size(739, 357);
            this.tableLayoutPanel60HzCountries.TabIndex = 70;
            this.tableLayoutPanel60HzCountries.Text = "Row for error visualization";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label47.Location = new System.Drawing.Point(249, 176);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(73, 16);
            this.label47.TabIndex = 46;
            this.label47.Text = "Montserrat";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label46.Location = new System.Drawing.Point(3, 176);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(53, 16);
            this.label46.TabIndex = 45;
            this.label46.Text = "Canada";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label45.Location = new System.Drawing.Point(3, 198);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(103, 16);
            this.label45.TabIndex = 44;
            this.label45.Text = "Cayman Islands";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label44.Location = new System.Drawing.Point(495, 176);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(154, 16);
            this.label44.TabIndex = 43;
            this.label44.Text = "Turks and Caicos Islands";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label43.Location = new System.Drawing.Point(495, 154);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(107, 16);
            this.label43.TabIndex = 42;
            this.label43.Text = "Trinidad & Tobago";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label42.Location = new System.Drawing.Point(495, 132);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(50, 16);
            this.label42.TabIndex = 41;
            this.label42.Text = "Taiwan";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label41.Location = new System.Drawing.Point(249, 132);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(47, 16);
            this.label41.TabIndex = 40;
            this.label41.Text = "Liberia";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label40.Location = new System.Drawing.Point(249, 154);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(50, 16);
            this.label40.TabIndex = 39;
            this.label40.Text = "Mexico";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label39.Location = new System.Drawing.Point(3, 154);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(39, 16);
            this.label39.TabIndex = 38;
            this.label39.Text = "Brazil";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label38.Location = new System.Drawing.Point(249, 198);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(68, 16);
            this.label38.TabIndex = 37;
            this.label38.Text = "Nicaragua";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label37.Location = new System.Drawing.Point(495, 242);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(68, 16);
            this.label37.TabIndex = 36;
            this.label37.Text = "Venezuela";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label36.Location = new System.Drawing.Point(249, 242);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(58, 16);
            this.label36.TabIndex = 35;
            this.label36.Text = "Panama";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label35.Location = new System.Drawing.Point(249, 264);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(36, 16);
            this.label35.TabIndex = 34;
            this.label35.Text = "Peru";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label34.Location = new System.Drawing.Point(3, 264);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(39, 16);
            this.label34.TabIndex = 33;
            this.label34.Text = "Cuba";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label33.Location = new System.Drawing.Point(3, 242);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(71, 16);
            this.label33.TabIndex = 32;
            this.label33.Text = "Costa Rica";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label32.Location = new System.Drawing.Point(3, 220);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(66, 16);
            this.label32.TabIndex = 31;
            this.label32.Text = "Colombia";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label31.Location = new System.Drawing.Point(495, 198);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(192, 16);
            this.label31.TabIndex = 30;
            this.label31.Text = "United States of America (USA)";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label30.Location = new System.Drawing.Point(495, 220);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(171, 16);
            this.label30.TabIndex = 29;
            this.label30.Text = "United States Virgin Islands";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label29.Location = new System.Drawing.Point(249, 220);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(41, 16);
            this.label29.TabIndex = 28;
            this.label29.Text = "Palau";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label28.Location = new System.Drawing.Point(249, 44);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(53, 16);
            this.label28.TabIndex = 27;
            this.label28.Text = "Guyana";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label27.Location = new System.Drawing.Point(3, 44);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(136, 16);
            this.label27.TabIndex = 26;
            this.label27.Text = "Antigua and Barbuda";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label26.Location = new System.Drawing.Point(3, 66);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(44, 16);
            this.label26.TabIndex = 25;
            this.label26.Text = "Aruba";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(495, 44);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(83, 16);
            this.label25.TabIndex = 24;
            this.label25.Text = "Sint Maarten";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label24.Location = new System.Drawing.Point(3, 22);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(56, 16);
            this.label24.TabIndex = 23;
            this.label24.Text = "Anguilla";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(495, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(130, 16);
            this.label19.TabIndex = 18;
            this.label19.Text = "Saint Kitts and Nevis";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(495, 22);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(89, 16);
            this.label18.TabIndex = 17;
            this.label18.Text = "Sint Eustatius";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(249, 22);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(73, 16);
            this.label17.TabIndex = 16;
            this.label17.Text = "Guatemala";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(249, 66);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 16);
            this.label16.TabIndex = 15;
            this.label16.Text = "Haiti";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(249, 110);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(85, 16);
            this.label15.TabIndex = 14;
            this.label15.Text = "Korea, South";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(3, 110);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(42, 16);
            this.label14.TabIndex = 13;
            this.label14.Text = "Belize";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(3, 132);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 16);
            this.label13.TabIndex = 12;
            this.label13.Text = "Bermuda";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(495, 110);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 16);
            this.label12.TabIndex = 11;
            this.label12.Text = "Suriname";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(3, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 16);
            this.label11.TabIndex = 10;
            this.label11.Text = "Bahamas";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(495, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 16);
            this.label10.TabIndex = 9;
            this.label10.Text = "Saudi Arabia";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(495, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 16);
            this.label9.TabIndex = 8;
            this.label9.Text = "South Korea";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(249, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 16);
            this.label8.TabIndex = 7;
            this.label8.Text = "Honduras";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "American Samoa";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(3, 286);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Dominican Republic";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(249, 286);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Philippines";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(249, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "Guam";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(495, 264);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Virgin Islands (British)";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(249, 308);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(79, 16);
            this.label23.TabIndex = 22;
            this.label23.Text = "Puerto Rico";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(3, 308);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "Ecuador";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label21.Location = new System.Drawing.Point(3, 330);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(73, 16);
            this.label21.TabIndex = 20;
            this.label21.Text = "El Salvador";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(495, 308);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(0, 16);
            this.label20.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat Medium", 8.999999F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(495, 286);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Virgin Islands (USA)";
            // 
            // tableLayoutPanelSpacerBottom
            // 
            this.tableLayoutPanelSpacerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.tableLayoutPanelSpacerBottom.ColumnCount = 1;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelSpacerBottom, 3);
            this.tableLayoutPanelSpacerBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelSpacerBottom.Location = new System.Drawing.Point(140, 729);
            this.tableLayoutPanelSpacerBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelSpacerBottom.Name = "tableLayoutPanelSpacerBottom";
            this.tableLayoutPanelSpacerBottom.RowCount = 1;
            this.tableLayoutPanelSpacerBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSpacerBottom.Size = new System.Drawing.Size(739, 37);
            this.tableLayoutPanelSpacerBottom.TabIndex = 74;
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.AutoSize = true;
            this.buttonNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.buttonNext.BorderColor = System.Drawing.Color.Black;
            this.buttonNext.BorderRadiusBottomLeft = 0;
            this.buttonNext.BorderRadiusBottomRight = 0;
            this.buttonNext.BorderRadiusTopLeft = 0;
            this.buttonNext.BorderRadiusTopRight = 0;
            this.buttonNext.BorderWidth = 2F;
            this.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNext.Font = new System.Drawing.Font("Montserrat Medium", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext.Location = new System.Drawing.Point(761, 670);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(0);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(118, 55);
            this.buttonNext.TabIndex = 86;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseCompatibleTextRendering = true;
            this.buttonNext.UseMnemonic = false;
            this.buttonNext.UseVisualStyleBackColor = false;
            // 
            // checkBoxDontShowStartup
            // 
            this.checkBoxDontShowStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.checkBoxDontShowStartup.AutoSize = true;
            this.checkBoxDontShowStartup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.checkBoxDontShowStartup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBoxDontShowStartup.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.checkBoxDontShowStartup.FlatAppearance.BorderSize = 0;
            this.checkBoxDontShowStartup.Font = new System.Drawing.Font("Montserrat", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxDontShowStartup.ForeColor = System.Drawing.Color.White;
            this.checkBoxDontShowStartup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.checkBoxDontShowStartup.Location = new System.Drawing.Point(429, 670);
            this.checkBoxDontShowStartup.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxDontShowStartup.Name = "checkBoxDontShowStartup";
            this.checkBoxDontShowStartup.Size = new System.Drawing.Size(224, 58);
            this.checkBoxDontShowStartup.TabIndex = 85;
            this.checkBoxDontShowStartup.Text = "Don\'t show this on startup";
            this.checkBoxDontShowStartup.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.checkBoxDontShowStartup.UseCompatibleTextRendering = true;
            this.checkBoxDontShowStartup.UseMnemonic = false;
            this.checkBoxDontShowStartup.UseVisualStyleBackColor = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.label22, 3);
            this.label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label22.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(143, 95);
            this.label22.Name = "label22";
            this.tableLayoutPanelMain.SetRowSpan(this.label22, 4);
            this.label22.Size = new System.Drawing.Size(733, 124);
            this.label22.TabIndex = 87;
            this.label22.Text = "Powerlines produce electromagnetic interference at 50Hz or 60Hz.\r\nWe need to opti" +
    "mize for your region\'s powerline settings";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UserControlBCIFilterSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlBCIFilterSettings";
            this.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanel60HzCountries.ResumeLayout(false);
            this.tableLayoutPanel60HzCountries.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel60HzCountries;
        public System.Windows.Forms.CheckBox checkBoxConfirm60HzCountry;
        public System.Windows.Forms.Button buttonExit;
        public System.Windows.Forms.CheckBox checkBoxDontShowStartup;
        public Lib.Core.WidgetManagement.ScannerRoundedButtonControl buttonNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSpacerBottom;
        private System.Windows.Forms.Label label22;
    }
}