////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// ScannerDemoForm.cs
//
// WinForms demo of the new Animation POC.
// Displays a simple row-column scanner with 6 labelled buttons.
// Demonstrates: Start/Stop/Pause/Resume, actuator simulation,
// scan speed adjustment, EventBus event log, and performance metrics.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Config;
using ACAT.Experimental.AnimationPOC.Core;
using ACAT.Experimental.AnimationPOC.Events;
using ACAT.Experimental.AnimationPOC.Infrastructure;
using ACAT.Experimental.AnimationPOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Experimental.AnimationPOC.Demo
{
    /// <summary>
    /// Demo form that shows the Animation POC running a simple 6-button scanner.
    ///
    /// Layout:
    ///   [ A ] [ B ] [ C ] [ D ] [ E ] [ F ]   ← scanner buttons
    ///
    ///   [Start] [Stop] [Pause/Resume] [Press Switch] [Speed slider]
    ///   [Selected: none]
    ///   [Event log]
    ///   [Performance metrics]
    /// </summary>
    public class ScannerDemoForm : Form
    {
        // ── Scanner widget names ─────────────────────────────────────
        private static readonly string[] WidgetNames = { "A", "B", "C", "D", "E", "F" };

        // ── UI controls ─────────────────────────────────────────────
        private readonly Dictionary<string, Button> _widgetButtons = new Dictionary<string, Button>();
        private Button _btnStart;
        private Button _btnStop;
        private Button _btnPauseResume;
        private Button _btnPressSwitch;
        private TrackBar _trackSpeed;
        private Label _lblSpeed;
        private Label _lblSelected;
        private ListBox _listLog;
        private Label _lblPerf;

        // ── Animation POC ────────────────────────────────────────────
        private IEventBus _eventBus;
        private IAnimationSession _session;
        private string _lastHighlightedWidget;
        private int _highlightCount;
        private readonly Stopwatch _highlightStopwatch = new Stopwatch();
        private double _lastHighlightMs;
        private bool _paused;

        public ScannerDemoForm()
        {
            InitializeComponent();
            InitializeAnimationPOC();
        }

        // ── Component initialization ─────────────────────────────────

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Animation System POC — Simple Scanner Demo";
            ClientSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.White;

            // ── Scanner buttons ──────────────────────────────────────
            int btnX = 30;
            int btnY = 30;
            foreach (var name in WidgetNames)
            {
                var btn = new Button
                {
                    Text = name,
                    Size = new Size(100, 80),
                    Location = new Point(btnX, btnY),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    Enabled = false  // not interactive — highlight-only in demo
                };
                btn.FlatAppearance.BorderColor = Color.DimGray;
                btn.FlatAppearance.BorderSize = 2;
                _widgetButtons[name] = btn;
                Controls.Add(btn);
                btnX += 110;
            }

            // ── Control buttons ──────────────────────────────────────
            int controlY = 140;

            _btnStart = MakeControlButton("▶ Start", 30, controlY, Color.ForestGreen);
            _btnStart.Click += (s, e) => OnStartClicked();
            Controls.Add(_btnStart);

            _btnStop = MakeControlButton("■ Stop", 160, controlY, Color.Firebrick);
            _btnStop.Click += (s, e) => OnStopClicked();
            _btnStop.Enabled = false;
            Controls.Add(_btnStop);

            _btnPauseResume = MakeControlButton("⏸ Pause", 290, controlY, Color.DarkGoldenrod);
            _btnPauseResume.Click += (s, e) => OnPauseResumeClicked();
            _btnPauseResume.Enabled = false;
            Controls.Add(_btnPauseResume);

            _btnPressSwitch = MakeControlButton("⚡ Press Switch", 420, controlY, Color.DarkCyan);
            _btnPressSwitch.Click += (s, e) => OnPressSwitchClicked();
            _btnPressSwitch.Enabled = false;
            Controls.Add(_btnPressSwitch);

            // ── Speed slider ─────────────────────────────────────────
            _lblSpeed = new Label
            {
                Text = "Scan Speed: 600ms",
                Location = new Point(30, 195),
                Size = new Size(200, 20),
                ForeColor = Color.LightGray
            };
            Controls.Add(_lblSpeed);

            _trackSpeed = new TrackBar
            {
                Minimum = 200,
                Maximum = 2000,
                Value = 600,
                TickFrequency = 200,
                LargeChange = 200,
                SmallChange = 100,
                Location = new Point(30, 215),
                Size = new Size(400, 45)
            };
            _trackSpeed.ValueChanged += (s, e) =>
            {
                _lblSpeed.Text = $"Scan Speed: {_trackSpeed.Value}ms";
                if (_session != null)
                {
                    // Speed change will take effect on the next session Start()
                }
            };
            Controls.Add(_trackSpeed);

            // ── Selected widget label ────────────────────────────────
            _lblSelected = new Label
            {
                Text = "Selected: (none)",
                Location = new Point(30, 270),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.LightGreen
            };
            Controls.Add(_lblSelected);

            // ── Performance label ────────────────────────────────────
            _lblPerf = new Label
            {
                Text = "Highlight latency: —   |   EventBus dispatch: —",
                Location = new Point(30, 300),
                Size = new Size(740, 25),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.SkyBlue
            };
            Controls.Add(_lblPerf);

            // ── Event log ────────────────────────────────────────────
            var lblLog = new Label
            {
                Text = "Event Log:",
                Location = new Point(30, 335),
                Size = new Size(100, 20),
                ForeColor = Color.LightGray
            };
            Controls.Add(lblLog);

            _listLog = new ListBox
            {
                Location = new Point(30, 355),
                Size = new Size(740, 220),
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Consolas", 9f),
                HorizontalScrollbar = true
            };
            Controls.Add(_listLog);

            ResumeLayout(false);
        }

        private static Button MakeControlButton(string text, int x, int y, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(120, 40),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f),
                BackColor = backColor,
                ForeColor = Color.White
            };
        }

        // ── Animation POC setup ─────────────────────────────────────

        private void InitializeAnimationPOC()
        {
            _eventBus = new SimpleEventBus();

            // Subscribe to animation events
            _eventBus.Subscribe<AnimationStateChangedEvent>(OnStateChanged);
            _eventBus.Subscribe<AnimationHighlightEvent>(OnHighlightEvent);
            _eventBus.Subscribe<AnimationTransitionEvent>(OnTransitionEvent);
        }

        private AnimationConfig BuildScannerConfig(double scanTimeMs)
        {
            return new AnimationConfig
            {
                PanelName = "SimpleScanner",
                ScanStrategy = "auto",
                Sequences = new System.Collections.Generic.List<AnimationSequenceConfig>
                {
                    new AnimationSequenceConfig
                    {
                        Name = "MainScan",
                        IsFirst = true,
                        AutoStart = true,
                        Iterations = "0",        // loop indefinitely
                        ScanTime = scanTimeMs.ToString("0"),
                        FirstPauseTime = "0",
                        OnEnter = "",
                        OnEnd = "",
                        Widgets = BuildWidgets()
                    }
                }
            };
        }

        private static List<AnimationWidgetConfig> BuildWidgets()
        {
            var widgets = new List<AnimationWidgetConfig>();
            foreach (var name in WidgetNames)
            {
                widgets.Add(new AnimationWidgetConfig
                {
                    Name = name,
                    PlayBeep = false,
                    OnSelected = $"actuate({name})"
                });
            }
            return widgets;
        }

        // ── Control handlers ─────────────────────────────────────────

        private void OnStartClicked()
        {
            _session?.Dispose();

            var config = BuildScannerConfig(_trackSpeed.Value);
            var renderer = new WinFormsHighlightRenderer(this, _widgetButtons);
            var service = new AnimationService(_eventBus, renderer);

            _session = service.CreateSession(config, "auto");
            _session.Start();

            _highlightCount = 0;
            _btnStart.Enabled = false;
            _btnStop.Enabled = true;
            _btnPauseResume.Enabled = true;
            _btnPressSwitch.Enabled = true;
            _paused = false;
            _btnPauseResume.Text = "⏸ Pause";

            LogEvent("── Session started (strategy: auto, speed: " + _trackSpeed.Value + "ms) ──");
        }

        private void OnStopClicked()
        {
            _session?.Stop();
            _session?.Dispose();
            _session = null;

            _btnStart.Enabled = true;
            _btnStop.Enabled = false;
            _btnPauseResume.Enabled = false;
            _btnPressSwitch.Enabled = false;
            _paused = false;
        }

        private void OnPauseResumeClicked()
        {
            if (_session == null) return;

            if (!_paused)
            {
                _session.Pause();
                _paused = true;
                _btnPauseResume.Text = "▶ Resume";
            }
            else
            {
                _session.Resume();
                _paused = false;
                _btnPauseResume.Text = "⏸ Pause";
            }
        }

        private void OnPressSwitchClicked()
        {
            _session?.Interrupt();
        }

        // ── EventBus handlers ────────────────────────────────────────

        private void OnStateChanged(AnimationStateChangedEvent e)
        {
            SafeInvoke(() =>
            {
                LogEvent($"[STATE] {e.OldState} → {e.NewState}  (seq: {e.CurrentAnimationName})");
            });
        }

        private void OnHighlightEvent(AnimationHighlightEvent e)
        {
            // Measure EventBus dispatch latency
            if (_highlightStopwatch.IsRunning)
            {
                _lastHighlightMs = _highlightStopwatch.Elapsed.TotalMilliseconds;
                _highlightStopwatch.Restart();
            }
            else
            {
                _highlightStopwatch.Restart();
            }

            _highlightCount++;

            SafeInvoke(() =>
            {
                _lastHighlightedWidget = e.WidgetName;
                _lblPerf.Text = $"Highlight → EventBus dispatch: {_lastHighlightMs:F2}ms   |   " +
                                $"Highlights so far: {_highlightCount}";
                LogEvent($"[HIGHLIGHT] {e.PreviousWidgetName ?? "—"} → {e.WidgetName}");
            });
        }

        private void OnTransitionEvent(AnimationTransitionEvent e)
        {
            SafeInvoke(() =>
            {
                LogEvent($"[TRANSITION] {e.FromAnimation} → {e.ToAnimation}");
            });
        }

        private void LogEvent(string message)
        {
            string line = $"{DateTime.Now:HH:mm:ss.fff}  {message}";
            _listLog.Items.Add(line);
            if (_listLog.Items.Count > 200) _listLog.Items.RemoveAt(0);
            _listLog.SelectedIndex = _listLog.Items.Count - 1;
        }

        private void SafeInvoke(Action action)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _session?.Stop();
            _session?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // ── WinForms highlight renderer ───────────────────────────────────

    /// <summary>
    /// IHighlightRenderer that changes button background colors on the WinForms UI thread.
    /// Handles thread marshalling via Control.BeginInvoke.
    /// </summary>
    internal class WinFormsHighlightRenderer : IHighlightRenderer
    {
        private readonly Control _form;
        private readonly Dictionary<string, Button> _buttons;
        private static readonly Color NormalColor = Color.FromArgb(60, 60, 60);
        private static readonly Color HighlightColor = Color.FromArgb(0, 120, 215);

        public WinFormsHighlightRenderer(Control form, Dictionary<string, Button> buttons)
        {
            _form = form;
            _buttons = buttons;
        }

        public void Render(string widgetName, HighlightStyle style)
        {
            SetColor(widgetName, HighlightColor);
        }

        public void ClearHighlight(string widgetName)
        {
            SetColor(widgetName, NormalColor);
        }

        public void ClearAll()
        {
            foreach (var name in _buttons.Keys)
                SetColor(name, NormalColor);
        }

        private void SetColor(string name, Color color)
        {
            if (!_buttons.TryGetValue(name, out var btn)) return;

            if (_form.InvokeRequired)
                _form.BeginInvoke((Action)(() => btn.BackColor = color));
            else
                btn.BackColor = color;
        }
    }
}
