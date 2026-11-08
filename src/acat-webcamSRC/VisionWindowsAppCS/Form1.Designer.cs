namespace VisionWindowsAppCS
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button buttonVision;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonRecalibrate;
        private System.Windows.Forms.Button buttonVideo;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStatusDetail;
        private System.Windows.Forms.Label labelLastGesture;
        private System.Windows.Forms.Label labelGestureTime;
        private System.Windows.Forms.ListBox listActivity;
        private System.Windows.Forms.RichTextBox textDiagnostics;
        private System.Windows.Forms.TrackBar trackBar_headMovement;
        private System.Windows.Forms.TrackBar trackBar_cheekSensitivity;
        private System.Windows.Forms.TrackBar trackBar_cheekHoldMsec;
        private System.Windows.Forms.TrackBar trackBar_ERSensitivity;
        private System.Windows.Forms.TrackBar trackBarERHoldMsec;
        private System.Windows.Forms.NumericUpDown numberCheekLength;
        private System.Windows.Forms.Label labelHeadValue;
        private System.Windows.Forms.Label labelCheekValue;
        private System.Windows.Forms.Label labelCheekHoldValue;
        private System.Windows.Forms.Label labelBrowValue;
        private System.Windows.Forms.Label labelBrowHoldValue;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var background = System.Drawing.Color.FromArgb(245, 247, 250);
            var ink = System.Drawing.Color.FromArgb(30, 43, 61);
            var muted = System.Drawing.Color.FromArgb(91, 108, 128);
            var accent = System.Drawing.Color.FromArgb(34, 96, 182);

            var root = new System.Windows.Forms.TableLayoutPanel {
                Dock = System.Windows.Forms.DockStyle.Fill, BackColor = background,
                Padding = new System.Windows.Forms.Padding(24, 20, 24, 20), ColumnCount = 1, RowCount = 3
            };
            root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72));
            root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 185));
            root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));

            var header = new System.Windows.Forms.TableLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 2 };
            header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100));
            var title = new System.Windows.Forms.Label {
                Text = "ACAT Vision", Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold),
                ForeColor = ink, Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            buttonExit = new System.Windows.Forms.Button {
                Name = "buttonExit", Text = "Exit", Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(0, 12, 0, 12), FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                BackColor = System.Drawing.Color.White, ForeColor = ink, TabIndex = 5
            };
            buttonExit.Click += buttonExit_Click;
            header.Controls.Add(title, 0, 0);
            header.Controls.Add(buttonExit, 1, 0);

            var session = new System.Windows.Forms.TableLayoutPanel {
                Dock = System.Windows.Forms.DockStyle.Fill, BackColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(20), ColumnCount = 2, RowCount = 3,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 16)
            };
            session.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            session.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 164));
            session.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28));
            session.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46));
            session.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            session.Controls.Add(new System.Windows.Forms.Label {
                Text = "VISION SESSION", ForeColor = muted, Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)
            }, 0, 0);
            labelStatus = new System.Windows.Forms.Label {
                Name = "labelStatus", Text = "Stopped", ForeColor = ink, Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold)
            };
            labelStatusDetail = new System.Windows.Forms.Label {
                Name = "labelStatusDetail", Text = "Ready to start the camera", ForeColor = muted,
                Dock = System.Windows.Forms.DockStyle.Fill, AutoEllipsis = true
            };
            session.Controls.Add(labelStatus, 0, 1);
            session.Controls.Add(labelStatusDetail, 0, 2);
            buttonVision = new System.Windows.Forms.Button {
                Name = "buttonVision", Text = "Start vision", Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = accent, ForeColor = System.Drawing.Color.White, FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold), TabIndex = 0,
                Margin = new System.Windows.Forms.Padding(4, 12, 0, 12)
            };
            buttonVision.FlatAppearance.BorderSize = 0;
            buttonVision.Click += buttonVision_Click;
            session.Controls.Add(buttonVision, 1, 1);

            var tabs = new System.Windows.Forms.TabControl {
                Dock = System.Windows.Forms.DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 10F), TabIndex = 1
            };
            var overview = new System.Windows.Forms.TabPage("Overview") { BackColor = background, Padding = new System.Windows.Forms.Padding(12) };
            var settings = new System.Windows.Forms.TabPage("Settings") { BackColor = background, Padding = new System.Windows.Forms.Padding(12) };
            var diagnostics = new System.Windows.Forms.TabPage("Diagnostics") { BackColor = background, Padding = new System.Windows.Forms.Padding(12) };

            var overviewGrid = new System.Windows.Forms.TableLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 2 };
            overviewGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60));
            overviewGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40));
            var gestureCard = new System.Windows.Forms.TableLayoutPanel {
                Dock = System.Windows.Forms.DockStyle.Fill, BackColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(20), RowCount = 4, Margin = new System.Windows.Forms.Padding(0, 0, 10, 0)
            };
            gestureCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27));
            gestureCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60));
            gestureCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28));
            gestureCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            gestureCard.Controls.Add(new System.Windows.Forms.Label { Text = "LATEST GESTURE", Dock = System.Windows.Forms.DockStyle.Fill, ForeColor = muted, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) }, 0, 0);
            labelLastGesture = new System.Windows.Forms.Label {
                Name = "labelLastGesture", Text = "Waiting for a gesture", Dock = System.Windows.Forms.DockStyle.Fill,
                AutoEllipsis = true, ForeColor = ink, Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold)
            };
            labelGestureTime = new System.Windows.Forms.Label { Name = "labelGestureTime", Text = "Start vision to begin", Dock = System.Windows.Forms.DockStyle.Fill, ForeColor = muted };
            gestureCard.Controls.Add(labelLastGesture, 0, 1);
            gestureCard.Controls.Add(labelGestureTime, 0, 2);
            listActivity = new System.Windows.Forms.ListBox {
                Name = "listActivity", Dock = System.Windows.Forms.DockStyle.Fill, BorderStyle = System.Windows.Forms.BorderStyle.None,
                BackColor = System.Drawing.Color.White, ForeColor = ink, IntegralHeight = false, TabIndex = 2
            };
            gestureCard.Controls.Add(listActivity, 0, 3);

            var actions = new System.Windows.Forms.TableLayoutPanel {
                Dock = System.Windows.Forms.DockStyle.Fill, BackColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(20), RowCount = 4
            };
            actions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34));
            actions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58));
            actions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58));
            actions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            actions.Controls.Add(new System.Windows.Forms.Label { Text = "QUICK ACTIONS", Dock = System.Windows.Forms.DockStyle.Fill, ForeColor = muted, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) }, 0, 0);
            buttonRecalibrate = new System.Windows.Forms.Button { Name = "buttonRecalibrate", Text = "Recalibrate", Dock = System.Windows.Forms.DockStyle.Fill, Margin = new System.Windows.Forms.Padding(0, 2, 0, 10), TabIndex = 3 };
            buttonVideo = new System.Windows.Forms.Button { Name = "buttonVideo", Text = "Show video window", Dock = System.Windows.Forms.DockStyle.Fill, Margin = new System.Windows.Forms.Padding(0, 2, 0, 10), TabIndex = 4 };
            buttonRecalibrate.Click += buttonRecalibrate_Click;
            buttonVideo.Click += buttonVideo_Click;
            actions.Controls.Add(buttonRecalibrate, 0, 1);
            actions.Controls.Add(buttonVideo, 0, 2);
            actions.Controls.Add(new System.Windows.Forms.Label {
                Text = "Video opens in a separate window.", ForeColor = muted,
                Dock = System.Windows.Forms.DockStyle.Fill, AutoSize = true
            }, 0, 3);
            overviewGrid.Controls.Add(gestureCard, 0, 0);
            overviewGrid.Controls.Add(actions, 1, 0);
            overview.Controls.Add(overviewGrid);

            var settingsGrid = new System.Windows.Forms.TableLayoutPanel {
                Dock = System.Windows.Forms.DockStyle.Fill, AutoScroll = true, ColumnCount = 3,
                RowCount = 8, BackColor = System.Drawing.Color.White, Padding = new System.Windows.Forms.Padding(20)
            };
            settingsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210));
            settingsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            settingsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75));
            for (int i = 0; i < 6; i++) settingsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55));
            settingsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65));
            settingsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            trackBar_headMovement = AddSlider(settingsGrid, 0, "Head movement", 20, 100, 40, out labelHeadValue);
            trackBar_cheekSensitivity = AddSlider(settingsGrid, 1, "Cheek sensitivity", 5, 50, 20, out labelCheekValue);
            trackBar_cheekHoldMsec = AddSlider(settingsGrid, 2, "Cheek hold (ms)", 0, 1000, 0, out labelCheekHoldValue);
            trackBar_ERSensitivity = AddSlider(settingsGrid, 3, "Eyebrow sensitivity", 5, 50, 10, out labelBrowValue);
            trackBarERHoldMsec = AddSlider(settingsGrid, 4, "Eyebrow hold (ms)", 0, 2000, 0, out labelBrowHoldValue);
            settingsGrid.Controls.Add(new System.Windows.Forms.Label { Text = "Minimum cheek gesture (ms)", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 5);
            numberCheekLength = new System.Windows.Forms.NumericUpDown { Name = "numberCheekLength", Minimum = 1, Maximum = 10000, Value = 150, Width = 110, Anchor = System.Windows.Forms.AnchorStyles.Left, TabIndex = 11 };
            settingsGrid.Controls.Add(numberCheekLength, 1, 5);
            buttonApply = new System.Windows.Forms.Button { Name = "buttonApply", Text = "Apply settings", Width = 150, Height = 40, Anchor = System.Windows.Forms.AnchorStyles.Left, TabIndex = 12 };
            buttonApply.Click += buttonApply_Click;
            settingsGrid.Controls.Add(buttonApply, 1, 6);
            settings.Controls.Add(settingsGrid);

            textDiagnostics = new System.Windows.Forms.RichTextBox {
                Name = "textDiagnostics", Dock = System.Windows.Forms.DockStyle.Fill, ReadOnly = true,
                BackColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Consolas", 9F),
                WordWrap = false, TabIndex = 13
            };
            diagnostics.Controls.Add(textDiagnostics);
            tabs.TabPages.Add(overview);
            tabs.TabPages.Add(settings);
            tabs.TabPages.Add(diagnostics);
            root.Controls.Add(header, 0, 0);
            root.Controls.Add(session, 0, 1);
            root.Controls.Add(tabs, 0, 2);
            Controls.Add(root);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(850, 640);
            MinimumSize = new System.Drawing.Size(720, 550);
            Font = new System.Drawing.Font("Segoe UI", 10F);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Name = "Form1";
            Text = "ACAT Vision — Demo";
            FormClosing += Form1_FormClosing;
        }

        private static System.Windows.Forms.TrackBar AddSlider(System.Windows.Forms.TableLayoutPanel grid, int row, string caption, int minimum, int maximum, int value, out System.Windows.Forms.Label valueLabel)
        {
            grid.Controls.Add(new System.Windows.Forms.Label { Text = caption, Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, row);
            var slider = new System.Windows.Forms.TrackBar {
                Minimum = minimum, Maximum = maximum, Value = value, TickStyle = System.Windows.Forms.TickStyle.None,
                Dock = System.Windows.Forms.DockStyle.Fill, AutoSize = false, Height = 35, TabIndex = 6 + row
            };
            valueLabel = new System.Windows.Forms.Label { Text = value.ToString(), Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            grid.Controls.Add(slider, 1, row);
            grid.Controls.Add(valueLabel, 2, row);
            return slider;
        }
    }
}