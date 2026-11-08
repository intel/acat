using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionContracts.Contracts;

namespace VisionWindowsAppCS
{
    public partial class Form1 : Form
    {
        private enum SessionState { Stopped, Starting, Running, Stopping, Error, Closing }

        private VisionClientController visionController;
        private readonly SemaphoreSlim operationGate = new SemaphoreSlim(1, 1);
        private SessionState state = SessionState.Stopped;
        private bool startCommandSent;
        private bool initReceived;
        private bool configuring;
        private bool videoRequested;
        private bool closing;
        private bool closeReady;
        private bool recovering;
        private int startGeneration;
        private string stopCorrelationId;
        private TaskCompletionSource<bool> stopAcknowledged;

        public Form1()
        {
            InitializeComponent();
            AttachController();
            WireSettings();
            RenderState("Ready to start the camera");
        }

        private void AttachController()
        {
            visionController = new VisionClientController();
            visionController.MessageReceived += OnNativeMessage;
            visionController.HostEventReceived += OnHostEvent;
            visionController.HostDisconnected += OnHostDisconnected;
        }

        private void DetachController(VisionClientController controller)
        {
            controller.MessageReceived -= OnNativeMessage;
            controller.HostEventReceived -= OnHostEvent;
            controller.HostDisconnected -= OnHostDisconnected;
        }

        private void WireSettings()
        {
            trackBar_headMovement.ValueChanged += (s, e) => labelHeadValue.Text = trackBar_headMovement.Value.ToString();
            trackBar_cheekSensitivity.ValueChanged += (s, e) => labelCheekValue.Text = trackBar_cheekSensitivity.Value.ToString();
            trackBar_cheekHoldMsec.ValueChanged += (s, e) => labelCheekHoldValue.Text = trackBar_cheekHoldMsec.Value.ToString();
            trackBar_ERSensitivity.ValueChanged += (s, e) => labelBrowValue.Text = trackBar_ERSensitivity.Value.ToString();
            trackBarERHoldMsec.ValueChanged += (s, e) => labelBrowHoldValue.Text = trackBarERHoldMsec.Value.ToString();
        }

        private void RenderState(string detail)
        {
            labelStatus.Text = state.ToString();
            labelStatus.ForeColor = state == SessionState.Running ? Color.FromArgb(22, 126, 91)
                : state == SessionState.Error ? Color.FromArgb(177, 52, 52) : Color.FromArgb(30, 43, 61);
            labelStatusDetail.Text = detail;
            buttonVision.Text = state == SessionState.Running ? "Stop vision" : "Start vision";
            buttonVision.Enabled = state == SessionState.Stopped || state == SessionState.Running;
            buttonRecalibrate.Enabled = state == SessionState.Running;
            buttonVideo.Enabled = state == SessionState.Running;
            buttonApply.Enabled = state == SessionState.Running;
            buttonExit.Enabled = !closing;
        }

        private async void buttonVision_Click(object sender, EventArgs e)
        {
            if (state == SessionState.Running)
            {
                await StopVisionAsync();
            }
            else if (state == SessionState.Stopped)
            {
                await StartVisionAsync();
            }
        }

        private async Task StartVisionAsync()
        {
            int generation = ++startGeneration;
            state = SessionState.Starting;
            startCommandSent = false;
            initReceived = false;
            videoRequested = false;
            buttonVideo.Text = "Show video window";
            labelLastGesture.Text = "Waiting for a gesture";
            labelGestureTime.Text = "Initializing camera…";
            RenderState("Connecting to the camera…");
            try
            {
                await operationGate.WaitAsync();
                try
                {
                    if (closing) return;
                    await Task.Run(() => visionController.StartVision());
                    startCommandSent = true;
                }
                finally
                {
                    operationGate.Release();
                }

                if (!closing && state == SessionState.Starting && initReceived)
                {
                    await ConfigureAfterStartAsync();
                }
                else if (!closing && state == SessionState.Starting)
                {
                    _ = CheckStartupTimeoutAsync(generation);
                }
            }
            catch (Exception ex)
            {
                await RecoverAsync("Unable to start vision: " + ex.Message);
            }
        }

        private async Task CheckStartupTimeoutAsync(int generation)
        {
            await Task.Delay(15000);
            if (!closing && generation == startGeneration && state == SessionState.Starting && !initReceived)
                await RecoverAsync("Camera initialization timed out.");
        }

        private async Task StopVisionAsync()
        {
            state = SessionState.Stopping;
            RenderState("Stopping the camera…");
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            stopAcknowledged = completion;
            stopCorrelationId = Guid.NewGuid().ToString("N");
            try
            {
                await operationGate.WaitAsync();
                try
                {
                    if (closing) return;
                    await Task.Run(() => visionController.StopVision(stopCorrelationId));
                }
                finally
                {
                    operationGate.Release();
                }

                var finished = await Task.WhenAny(completion.Task, Task.Delay(4500));
                if (closing || state != SessionState.Stopping) return;
                if (finished != completion.Task || !completion.Task.Result)
                {
                    await RecoverAsync("Camera did not stop cleanly.");
                    return;
                }

                videoRequested = false;
                buttonVideo.Text = "Show video window";
                state = SessionState.Stopped;
                RenderState("Camera stopped. You can start another session.");
                AddActivity("Vision stopped");
            }
            catch (Exception ex)
            {
                await RecoverAsync("Unable to stop vision: " + ex.Message);
            }
            finally
            {
                if (stopAcknowledged == completion)
                {
                    stopAcknowledged = null;
                    stopCorrelationId = null;
                }
            }
        }

        private async Task ConfigureAfterStartAsync()
        {
            if (configuring || closing || state != SessionState.Starting || !startCommandSent || !initReceived) return;
            configuring = true;
            RenderState("Camera detected. Applying settings…");
            try
            {
                await ApplySettingsAsync();
                if (closing || state != SessionState.Starting) return;
                state = SessionState.Running;
                RenderState("Camera active · gesture detection is ready");
                labelGestureTime.Text = "No gestures detected yet";
                AddActivity("Vision started");
            }
            catch (Exception ex)
            {
                await RecoverAsync("Camera setup failed: " + ex.Message);
            }
            finally
            {
                configuring = false;
            }
        }

        private async Task ApplySettingsAsync()
        {
            double[] values = {
                (double)numberCheekLength.Value, trackBar_headMovement.Value,
                trackBar_cheekSensitivity.Value, trackBar_cheekHoldMsec.Value,
                trackBar_ERSensitivity.Value, trackBarERHoldMsec.Value
            };
            string[] commands = {
                "action=SET_CT_MIN_GESTURE_LEN", "action=HEAD_MOVEMENT_SENSITIVITY",
                "action=CHEEK_MOVEMENT_SENSITIVITY", "action=CHEEK_HOLD_MSEC",
                "action=EYEBROW_MOVEMENT_SENSITIVITY", "action=EYEBROW_HOLD_MSEC"
            };
            await operationGate.WaitAsync();
            try
            {
                if (closing) return;
                await Task.Run(() => {
                    for (int i = 0; i < commands.Length; i++)
                    {
                        visionController.SendNativeCommand(commands[i], values[i]);
                    }
                });
            }
            finally
            {
                operationGate.Release();
            }
        }

        private async void buttonApply_Click(object sender, EventArgs e)
        {
            if (state != SessionState.Running) return;
            buttonApply.Enabled = false;
            try
            {
                await ApplySettingsAsync();
                if (!closing && state == SessionState.Running) AddActivity("Settings applied");
            }
            catch (Exception ex)
            {
                await RecoverAsync("Unable to apply settings: " + ex.Message);
            }
            finally
            {
                if (!closing && state == SessionState.Running) buttonApply.Enabled = true;
            }
        }

        private async void buttonRecalibrate_Click(object sender, EventArgs e)
        {
            if (state != SessionState.Running) return;
            await RunQuickActionAsync(() => visionController.SendNativeCommand("action=RECALIBRATE", 0), "Recalibration requested");
        }

        private async void buttonVideo_Click(object sender, EventArgs e)
        {
            if (state != SessionState.Running) return;
            bool show = !videoRequested;
            await RunQuickActionAsync(() => { if (show) visionController.ShowVideo(); else visionController.HideVideo(); },
                show ? "Video window requested" : "Video window hidden");
            if (!closing && state == SessionState.Running)
            {
                videoRequested = show;
                buttonVideo.Text = show ? "Hide video window" : "Show video window";
            }
        }

        private async Task RunQuickActionAsync(Action action, string message)
        {
            buttonRecalibrate.Enabled = false;
            buttonVideo.Enabled = false;
            try
            {
                await operationGate.WaitAsync();
                try { if (closing) return; await Task.Run(action); }
                finally { operationGate.Release(); }
                if (!closing && state == SessionState.Running) AddActivity(message);
            }
            catch (Exception ex)
            {
                await RecoverAsync("Action failed: " + ex.Message);
            }
            finally
            {
                if (!closing && state == SessionState.Running)
                {
                    buttonRecalibrate.Enabled = true;
                    buttonVideo.Enabled = true;
                }
            }
        }

        private void OnNativeMessage(string text)
        {
            OnUi(() => HandleNativeMessage(text));
        }

        private void OnHostEvent(VisionHostEvent hostEvent)
        {
            if (hostEvent.EventType == VisionHostEventType.VisionStopped && hostEvent.CorrelationId == stopCorrelationId)
                stopAcknowledged?.TrySetResult(true);
            if (hostEvent.EventType == VisionHostEventType.HostError && hostEvent.CorrelationId == stopCorrelationId)
                stopAcknowledged?.TrySetResult(false);

            OnUi(() => {
                if (closing) return;
                if (hostEvent.EventType == VisionHostEventType.HostError)
                {
                    AppendDiagnostic("Host error: " + hostEvent.Payload);
                    if (state == SessionState.Starting || state == SessionState.Running)
                        _ = RecoverAsync(hostEvent.Payload ?? "Host error");
                }
            });
        }

        private void OnHostDisconnected()
        {
            OnUi(() => {
                if (!closing && state != SessionState.Error)
                    _ = RecoverAsync("Vision host disconnected.");
            });
        }

        private void OnUi(Action action)
        {
            if (IsDisposed || Disposing || !IsHandleCreated) return;
            try { BeginInvoke(new MethodInvoker(() => { if (!IsDisposed && !Disposing) action(); })); }
            catch (InvalidOperationException) { }
        }

        private void HandleNativeMessage(string text)
        {
            if (closing) return;
            AppendDiagnostic(text);
            string gesture = null;
            string action = null;
            string reason = null;
            foreach (var part in text.Split(';'))
            {
                if (part.StartsWith("gesture=", StringComparison.OrdinalIgnoreCase)) gesture = part.Substring(8);
                else if (part.StartsWith("action=", StringComparison.OrdinalIgnoreCase)) action = part.Substring(7);
                else if (part.StartsWith("reason=", StringComparison.OrdinalIgnoreCase)) reason = part.Substring(7);
            }

            if (string.Equals(gesture, "INIT_DONE", StringComparison.OrdinalIgnoreCase))
            {
                initReceived = true;
                if (state == SessionState.Starting && startCommandSent) _ = ConfigureAfterStartAsync();
            }
            else if (string.Equals(gesture, "EXCEPTION", StringComparison.OrdinalIgnoreCase))
            {
                _ = RecoverAsync(string.IsNullOrEmpty(reason) ? "Camera error" : reason);
            }
            else if (string.Equals(gesture, "CALIB_START", StringComparison.OrdinalIgnoreCase))
            {
                if (state == SessionState.Running) RenderState("Calibrating…");
                AddActivity("Calibration started");
            }
            else if (string.Equals(gesture, "CALIB_END", StringComparison.OrdinalIgnoreCase))
            {
                if (state == SessionState.Running) RenderState("Camera active · gesture detection is ready");
                AddActivity("Calibration complete");
            }
            else if (state == SessionState.Running && string.Equals(action, "trigger", StringComparison.OrdinalIgnoreCase))
            {
                string friendly = string.Equals(gesture, "CT", StringComparison.OrdinalIgnoreCase) ? "Cheek twitch"
                    : string.Equals(gesture, "ER", StringComparison.OrdinalIgnoreCase) ? "Eyebrow raise" : null;
                if (friendly != null)
                {
                    labelLastGesture.Text = friendly;
                    labelGestureTime.Text = "Detected at " + DateTime.Now.ToString("h:mm:ss tt");
                    AddActivity(friendly + " detected");
                }
            }
        }

        private void AddActivity(string message)
        {
            listActivity.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + "   " + message);
            while (listActivity.Items.Count > 50) listActivity.Items.RemoveAt(listActivity.Items.Count - 1);
        }

        private void AppendDiagnostic(string message)
        {
            var lines = textDiagnostics.Lines;
            if (lines.Length >= 300)
            {
                textDiagnostics.Lines = new System.Collections.Generic.List<string>(lines).GetRange(lines.Length - 200, 200).ToArray();
            }
            textDiagnostics.AppendText(DateTime.Now.ToString("HH:mm:ss") + "  " + message + Environment.NewLine);
        }

        private async Task RecoverAsync(string message)
        {
            if (closing || recovering) return;
            recovering = true;
            state = SessionState.Error;
            RenderState(message);
            AddActivity("Error: " + message);
            Trace.WriteLine("[VisionDashboard] " + message);
            await operationGate.WaitAsync();
            try
            {
                if (closing) return;
                var old = visionController;
                DetachController(old);
                await Task.Run(() => old.Dispose());
                if (closing) return;
                AttachController();
                startCommandSent = false;
                initReceived = false;
                videoRequested = false;
                buttonVideo.Text = "Show video window";
                state = SessionState.Stopped;
                RenderState("Error: " + message + "  ·  Try starting again");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[VisionDashboard] Recovery failed: " + ex);
                if (!closing) RenderState("Unable to recover: " + ex.Message + "  ·  Exit and try again");
            }
            finally
            {
                operationGate.Release();
                recovering = false;
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closeReady) return;
            e.Cancel = true;
            if (closing) return;
            closing = true;
            state = SessionState.Closing;
            RenderState("Closing camera and vision host…");
            await operationGate.WaitAsync();
            try
            {
                var controller = visionController;
                DetachController(controller);
                await Task.Run(() => controller.Dispose());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[VisionDashboard] Close failure: " + ex);
            }
            finally
            {
                operationGate.Release();
                closeReady = true;
                Close();
            }
        }

    }
}