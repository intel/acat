////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TalkApplicationBCIScanner.cs
//
// Entry point into BCI initialization and UI display
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Extensions.BCI.Common.AnimationSharp;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Extensions.BCI.Common.BCIInterfaceUtilities;
using ACAT.Extensions.BCI.UI.UserControls;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension.CommandHandlers;
using ACAT.Lib.Extension;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace ACAT.Extensions.BCI.UI.Scanners
{
    /// <summary>
    /// Scanner form for a Talk-only interface.  Displays a text box with a
    /// reduced alphabet scanner below it enabling the user to type text (with
    /// word prediction) and have the text converted to speech.  The keyboard
    /// layout is ABC.
    /// </summary>
    [DescriptorAttribute("48222D57-1EA8-44FF-8706-C2399D0B4CFA",
                        "TalkApplicationScannerSmallLayout",
                        "Talk application window with circular layout with large buttons, added features")]
    public partial class TalkApplicationBCIScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        /// <summary>
        /// Main object for BCI animations
        /// </summary>
        public AnimationSharpManagerV2 animationSharpManager;

        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// The AlphabetScannerCommon object. Has a number of
        /// helper functions
        /// </summary>
        private readonly ScannerCommon2 _scannerCommon;

        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _bciActuator = null;

        /// <summary>
        /// Object of the state of BCI initialization
        /// </summary>
        private BCIState _BCIState = BCIState.None;

        /// <summary>
        /// Decision of the user (for calibration)
        /// </summary>
        private volatile BCISimpleParameters _CalibrationParameters = new BCISimpleParameters();

        /// <summary>
        /// Should the scanner be dimmed
        /// </summary>
        private bool _dimScanner;

        /// <summary>
        /// If the form is fully loaded and painted
        /// </summary>
        private bool _formShown = false;

        private bool _getCaretPosition = false;

        /// <summary>
        /// Name of the class for the panel
        /// </summary>
        private String _panelClass;

        /// <summary>
        /// Last caret position 
        /// </summary>
        private int _prevCaretPosition = 0;

        /// <summary>
        /// Text from the Text box active User control
        /// </summary>
        private string _prevText = string.Empty;

        /// <summary>
        /// Latest user control placed
        /// </summary>
        private UserControl _prevUserControl = null;

        /// <summary>
        /// Decision of the user (for calibration)
        /// </summary>
        private volatile bool _RequestCalibration = false;

        /// <summary>
        /// The ScannerHelper object
        /// </summary>
        private ScannerHelper _scannerHelper;

        private volatile BCIScanSections _ScanningSection = BCIScanSections.None;

        /// <summary>
        /// Text box user control (Lock screeb)
        /// </summary>
        ScreenLockTextBoxUserControl _screenLockTextBoxUserControl;

        /// <summary>
        /// Flag used to show the main 3 options (Exit, calibrate, typing)
        /// </summary>
        private bool _ShowMainOptions = true;

        /// <summary>
        /// Text box user control (Canned Mode)
        /// </summary>
        TalkWindowTextBoxPhraseModeUserControl _textBoxPhraseModeUserControl;

        /// <summary>
        /// Text box user control (Prompt)
        /// </summary>
        TalkWindowTextBoxPromptUserControl _textBoxPromptUserControl;

        /// <summary>
        /// Text box user control (Prompt)
        /// </summary>
        TalkWindowTextBoxPromptUserControlLabel _textBoxPromptUserControlLabel;

        /// <summary>
        /// Oobject of the Text box from the main form
        /// </summary>
        private TextBox _textBoxTalkWindow;

        /// <summary>
        /// Text box user control (Normal)
        /// </summary>
        TalkWindowTextBoxUserControl _textBoxUserControl;

        /// <summary>
        /// Text used to save in Lear request
        /// </summary>
        private string _TextToLearnCanned = string.Empty;

        /// <summary>
        /// Ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// If the actuator initialized correctly
        /// </summary>
        private bool isSignalMonitorCalledFromMenu = false;
        /// <summary>
        /// Option selected when changing User conntrol 
        /// </summary>
        private int selectedOption;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TalkApplicationBCIScanner()
        {
            _scannerCommon = new ScannerCommon2(this);
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SubscribeToEvents();
            _dispatcher = new Dispatcher(this);
            Paint += (s, args) => { _formShown = true; };
            _dimScanner = true;
            EvtBCIInitState?.Invoke(BCIState.UIRefresh);
            LogAssemblyVersion();
            AnimationManagerUtils.Init();
        }
        /// <summary>
        /// Event for the states for BCI initialization
        /// </summary>
        /// <param name="state"></param>
        private delegate void BCIInitState(BCIState state);

        private event BCIInitState EvtBCIInitState;

        /// <summary>
        /// State of the initialization of BCI
        /// </summary>
        private enum BCIState
        {
            None = 0,
            UIRefresh,
            Initializing,
            StartBCIReqParams,
            ReqCalibrationStatus,
            BCIStartSession,
            BCIInitDone,
        }
        /// <summary>
        /// Gets the command dispatcher object
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; } //_alphabetScannerCommon.Dispatcher; }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets this form object
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class for the scanner
        /// </summary>
        public String PanelClass
        {
            get { return _panelClass; }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

        public ScannerCommon ScannerCommon
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon2 ScannerCommon2
        {
            get { return _scannerCommon; }
        }
        /// <summary>
        /// Gets the status bar control for this scanner
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon2.StatusBar; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object for this scanner
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        public async Task BCIRequestCalibrationStatus()
        {
            await Task.Delay(25);
            _bciActuator?.IoctlRequest((int)OpCodes.RequestCalibrationStatus, String.Empty);
        }

        /// <summary>
        /// Task to request initial parameters for BCI
        /// </summary>
        /// <returns></returns>
        public async Task BCIStartBCIReqParams()
        {
            //STEP - 5
            await Task.Delay(5);
            BCIMode bCIMode = new BCIMode();
            if (_RequestCalibration)
                bCIMode.BciMode = BCIModes.CALIBRATION;
            else
                bCIMode.BciMode = BCIModes.TYPING;
            bCIMode.BciCalibrationMode = _ScanningSection;
            animationSharpManager.RequestParameters(bCIMode, _CalibrationParameters);
            await Task.Delay(5);
        }

        /// <summary>
        /// Task to requesto BCI actuator to start BCI
        /// </summary>
        /// <returns></returns>
        public async Task BCIStartSession()
        {
            //STEP - 7
            await Task.Delay(25);
            switch (_RequestCalibration)
            {
                case true:
                    animationSharpManager.CalibrationRequest(_ScanningSection);
                    break;
                case false:
                    if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)//So if the textbox panel has a Prompt texbox UC can return to the typing texbox user control
                        AddTextBoxUserControl(_textBoxUserControl);
                    else
                        AddTextBoxUserControl(_textBoxPhraseModeUserControl);
                    if (_prevCaretPosition != 0)
                        Windows.SetCaretPosition(_textBoxTalkWindow, _prevCaretPosition);
                    _prevCaretPosition = 0;
                    _getCaretPosition = true;
                    animationSharpManager.TypingRequest();
                    break;
                default:
                    break;
            }
            await Task.Delay(25);
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdEditScanner":
                case "CmdTalkWindowClear":
                    arg.Handled = true;
                    arg.Enabled = _textBoxTalkWindow != null && _textBoxTalkWindow.Text.Length != 0;
                    break;

                case "CmdSpeak":
                    arg.Handled = true;
                    arg.Enabled = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases &&
                                    _textBoxTalkWindow != null && _textBoxTalkWindow.Text.Length != 0;
                    break;

                case "CmdActuatorToggleCalibrationWindow":
                    arg.Handled = true;
                    arg.Enabled = true;
                    break;

                case "CmdSaveToCanned":
                    arg.Handled = true;
                    var mode = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode();
                    arg.Enabled = mode != WordPredictionModes.None && mode != WordPredictionModes.CannedPhrases &&
                        _textBoxTalkWindow != null && _textBoxTalkWindow.Text.Length != 0;
                    break;

                case "CmdEnterKey":
                    arg.Handled = true;
                    arg.Enabled = _textBoxTalkWindow != null && _textBoxTalkWindow.Multiline;
                    break;

                case "CmdEntryModeTyping":
                    arg.Handled = true;
                    arg.Enabled = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.Sentence;
                    break;

                case "CmdEntryModeShortHand":
                    arg.Handled = true;
                    arg.Enabled = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.Shorthand;
                    break;

                case "CmdEntryModePhrase":
                    arg.Handled = true;
                    arg.Enabled = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases;
                    break;
                default:
                    _scannerHelper.CheckCommandEnabled(arg);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Task in charge to adjust the UI controls to scale and fit corretly 
        /// </summary>
        /// <returns></returns>
        public async Task ControlsUIAdjustment()
        {
            while (true)//Loop until the Windows controls has shown completely so the heigh and width can be changed with real values
            {
                await Task.Delay(100);//Since SharpDx transparency Mode did not worked to show the TextBox the UI had to be force to be shaped and assemble to match the UI from The final design version
                if (_formShown)
                {
                    int height = this.Height - mainPanel.Height - statusStrip.Height - 25;
                    int unit = height / 11;
                    scannerTableLayoutKeyboard.Height = unit * 4;
                    scannerTableLayoutWordPredictions.Width = (mainPanel.Width / 2);
                    scannerTableLayoutSentences.Height = (unit * 5) - 3;
                    panelTextbox.Width = scannerTableLayoutSentences.Width - 10;
                    await Task.Delay(50);
                    break;
                }
            }
            EvtBCIInitState?.Invoke(BCIState.Initializing);
        }

        /// <summary>
        /// Returns all the controls in the form (recusrively finds them)
        /// </summary>
        /// <param name="control">parent control</param>
        /// <param name="type">type of control to look for</param>
        /// <returns>list of controls</returns>
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public List<Control> GetControls(string type, Control control)
        {
            List<Control> controls = new List<Control>();
            foreach (Control c in control.Controls)
            {
                if (c.GetType().Name == type)
                {
                    controls.Add(c);
                }
                else if (c.Controls.Count > 0)
                {
                    controls.AddRange(GetControls(type, c));
                }
            }
            return controls;
        }

        /// <summary>
        /// Intitialize the class
        /// </summary>
        /// <param name="startupArg">startup params</param>
        /// <returns>true on cussess</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _panelClass = startupArg.PanelClass;
            _scannerHelper = new ScannerHelper(this, startupArg);
            bool retVal = _scannerCommon.Initialize(startupArg);
            if (retVal)
            {
                _scannerCommon.SetStatusBar(statusStrip);
            }
            ControlBox = true;
            _textBoxUserControl = new TalkWindowTextBoxUserControl(this, panelTextbox);
            _textBoxPhraseModeUserControl = new TalkWindowTextBoxPhraseModeUserControl(this, panelTextbox);
            _textBoxPromptUserControl = new TalkWindowTextBoxPromptUserControl(this, panelTextbox);
            _textBoxPromptUserControlLabel = new TalkWindowTextBoxPromptUserControlLabel(this, panelTextbox);
            _screenLockTextBoxUserControl = new ScreenLockTextBoxUserControl(this, panelTextbox);
            _screenLockTextBoxUserControl.EvtScreenUnlocked += ScreenLockTextBoxUserControl_EvtScreenUnlocked;
            AddTextBoxUserControl(_textBoxUserControl);
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged += ActiveWordPredictor_EvtModeChanged;
            _scannerCommon.UserControlManager.GridScanIterations = Lib.Extension.Common.AppPreferences.GridScanIterations;
            return retVal;
        }
        /// <summary>
        /// Task to Initialize objects used by BCI 
        /// </summary>
        /// <returns></returns>
        public async Task InitializeBCI()
        {
            await Task.Delay(50);
            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(scannerPanelWordPredictions, "wordPrediction", "WordPredictionUserControlBCI");
            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(scannerPanelSentences, "sentencePrediction", "PhrasesUserControlBCI");
            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(scannerPanelKeyboard, "keyboard", "KeyboardABCUserControlBCI");
            Windows.SetText(_textBoxTalkWindow, BCIInterfaceUtils.INITIALIZING);
            await Task.Delay(50);
            animationSharpManager.Init(this);
            animationSharpManager.CRGText = GetCRGText();
            var widgets = SaveUserControlWidgets();
            SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), KeyboardABCUserControlBCI.getpathConfigFile(),true);
            await Task.Delay(700);
            animationSharpManager.DrawMainLayout();
            Windows.SetText(_textBoxTalkWindow, string.Empty);
            await Task.Delay(25);
            EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animations
        /// </summary>
        public void OnPause()
        {
            _windowActiveWatchdog?.Pause();
            _scannerCommon.UserControlManager.OnPause();
            _scannerCommon.OnPause(_dimScanner ?
                                ScannerCommon2.PauseDisplayMode.FadeScanner :
                                ScannerCommon2.PauseDisplayMode.None);
            if (panelTextbox.Controls.Count > 0 && panelTextbox.Controls[0] is ITalkWindowTextBox)
            {
                ITalkWindowTextBox tb = panelTextbox.Controls[0] as ITalkWindowTextBox;
                tb.OnPause();
            }
        }
        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="eventArg"></param>
        /// <returns></returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            if (panelTextbox.Controls.Count > 0 && panelTextbox.Controls[0] is ITalkWindowTextBox)
            {
                ITalkWindowTextBox tb = panelTextbox.Controls[0] as ITalkWindowTextBox;
                tb.OnResume();
            }
            _windowActiveWatchdog?.Resume();
            _dimScanner = true;
            _scannerCommon.UserControlManager.OnResume();
            _scannerCommon.OnResume();
            _scannerCommon.ResizeToFitDesktop(this);
        }

        /// <summary>
        /// Triggered when the user actuates a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled?</param>
        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        public void SetTargetControl(Form parent, Widget widget)
        {
        }
        /// <summary>
        /// Size of the client changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            _scannerCommon.OnClientSizeChanged();
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">closing param</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;
            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                {
                    base.WndProc(ref m);
                    return;
                }
            }
            if (!_scannerCommon.HandleWndProc(m))
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Event to show box text User control when the mode change
        /// </summary>
        /// <param name="newMode"></param>
        private void ActiveWordPredictor_EvtModeChanged(WordPredictionModes newMode)
        {
            Invoke(new MethodInvoker(delegate
            {
                if (newMode == WordPredictionModes.CannedPhrases)
                {
                    AddTextBoxUserControl(_textBoxPhraseModeUserControl);
                }
                else
                {
                    AddTextBoxUserControl(_textBoxUserControl);
                }
            }));
            if (Lib.Extension.Common.AppPreferences.ClearTalkWindowOnTypeModeChange)
            {
                Windows.SetText(_textBoxTalkWindow, String.Empty);
            }
        }
        /// <summary>
        /// Adds the Text box user control to the panel
        /// </summary>
        /// <param name="userControl"></param>
        private void AddTextBoxUserControl(UserControl userControl)
        {
            bool changeTextBoxUserControl = true;
            if (panelTextbox.Controls.Count > 0)//To validate if the Panel TextBox contains the same User control and avoid replacing the same object
            {
                changeTextBoxUserControl = ChangeTextBoxUserControl(userControl, panelTextbox);
            }
            if (changeTextBoxUserControl)
            {
                if (panelTextbox.Controls.Count > 0 && panelTextbox.Controls[0] is ITalkWindowTextBox)
                {
                    var textBox = (panelTextbox.Controls[0] as ITalkWindowTextBox).TextBoxControl;
                    if (textBox != null)
                    {
                        textBox.KeyPress -= TextBoxTalkWindowOnKeyPress;
                    }
                }
                SaveCaretPositionForTextBoxUC(panelTextbox);//Changing the Textbox user control could change the caret position with this if it happens it restores the carets positions
                panelTextbox.Controls.Clear();
                panelTextbox.Controls.Add(userControl);
                userControl.Dock = DockStyle.Fill;
                userControl.TabStop = true;
                userControl.TabIndex = 0;
                if (userControl is ITalkWindowTextBox)
                {
                    _textBoxTalkWindow = (userControl as ITalkWindowTextBox).TextBoxControl;
                    if (_textBoxTalkWindow != null)
                    {
                        _textBoxTalkWindow.KeyPress += TextBoxTalkWindowOnKeyPress;
                        _textBoxTalkWindow.Focus();
                    }
                }
                if (_textBoxTalkWindow != null)
                    SetCaretPositionForTextBoxUC(userControl, panelTextbox, Windows.GetCaretPosition(_textBoxTalkWindow));
            }
        }

        /// <summary>
        /// Handler for the actuator response from BCI
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="response"></param>
        private void BciActuator_EvtIoctlResponse(int opcode, string response)
        {
            switch (opcode)
            {
                case (int)OpCodes.CalibrationWindowPreShow:
                    OnPause();
                    if (_bciActuator != null)
                    {
                        _bciActuator.IoctlRequest((int)OpCodes.Pause, "true");
                        _bciActuator?.IoctlRequest((int)OpCodes.CalibrationWindowShow, String.Empty);
                    }
                    break;
                case (int)OpCodes.CalibrationWindowClose:
                    if (_bciActuator != null)
                    {
                        _bciActuator.IoctlRequest((int)OpCodes.Pause, "false");
                        animationSharpManager.SuspendAnimations = false;
                        animationSharpManager.LockAnimation(false);
                        if (isSignalMonitorCalledFromMenu)
                            ChangeToMainKeyboard();
                        isSignalMonitorCalledFromMenu = false;
                        EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                    }
                    OnResume();
                    break;
                case (int)OpCodes.SendParameters:
                    //STEP - 6
                    if (_BCIState == BCIState.StartBCIReqParams)
                    {
                        if (!_RequestCalibration)//have a delay before start typing so the user can see the options and suggestions and start looking where is desired
                        {
                            Log.Debug("BCI LOG | Delay before Typing ");//Run the delay in another therad to avoid blocking the UI
                            _ = ShowTimedMessageBoxAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            BCIInterfaceUtils.ShowTimedMessageBox(this);
                            EvtBCIInitState?.Invoke(BCIState.BCIStartSession);
                        }
                    }
                    break;
                case (int)OpCodes.StartSessionResult:
                    if (_BCIState == BCIState.BCIStartSession)
                    {
                        _BCIState = BCIState.BCIInitDone;
                        Log.Debug("BCI LOG | BCI Init state: " + BCIState.BCIInitDone);
                    }
                    break;
                case (int)OpCodes.SendCalibrationStatus:
                    //STEP - 1
                    var bciCalibrationStatus = JsonConvert.DeserializeObject<BCICalibrationStatus>(response);
                    Log.Debug("BCI LOG | bciCalibrationStatus.OkToGoToTyping: " + bciCalibrationStatus.OkToGoToTyping);
                    if (_ShowMainOptions)//This window should only display once 
                    {
                        _ShowMainOptions = false;
                        var resultmenu = BCIShowMainOptionsWindow(bciCalibrationStatus);
                        switch (resultmenu)
                        {
                            case BCIMenuOptions.MainMenuOptions.ExitApplication:
                                bool confirmRes = BCIInterfaceUtils.ShowExitAppWindow(this);
                                if (confirmRes)
                                    ExitApplication();
                                else
                                    EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                                break;
                            case BCIMenuOptions.MainMenuOptions.CalibrateOrShowCalibrationModes:
                                var result = BCIShowCalibrationModesWindow(bciCalibrationStatus);
                                BCIProcessCalibrationFormResult(result);
                                break;
                            case BCIMenuOptions.MainMenuOptions.TypingOrRecalibrate:
                                BCIProcessCalibrationFormResult(new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Typing, new BCISimpleParameters()));
                                break;
                        }
                    }
                    else
                    {
                        var result = BCIShowCalibrationModesWindow(bciCalibrationStatus);
                        BCIProcessCalibrationFormResult(result);
                    }
                    break;
            }
        }
        /// <summary>
        /// Event triggered when is necessary to resume Watchdog from the AnimationSharpmanager
        /// </summary>
        private void BCIEvent()
        {
            OnResume();
        }

        /// <summary>
        /// Process confirm box main modes result
        /// </summary>
        /// <param name="option"></param>
        private void BCIProcessCalibrationFormResult(Tuple<BCIMenuOptions.Options, BCISimpleParameters> options)
        {
            //STEP - 3
            _CalibrationParameters.ScannTime = options.Item2.ScannTime;
            _CalibrationParameters.Targets = options.Item2.Targets;
            _CalibrationParameters.IterationsPertarget = options.Item2.IterationsPertarget;
            _CalibrationParameters.MinScore = options.Item2.MinScore;
            switch (options.Item1)
            {
                case BCIMenuOptions.Options.Exit://Exit
                    bool confirmRes = BCIInterfaceUtils.ShowExitAppWindow(this);
                    if (confirmRes)
                        ExitApplication();
                    else
                        EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                    break;
                case BCIMenuOptions.Options.Box:
                    _RequestCalibration = true;
                    _ScanningSection = BCIScanSections.Box;
                    EvtBCIInitState?.Invoke(BCIState.StartBCIReqParams);
                    break;
                case BCIMenuOptions.Options.Sentence:
                    _RequestCalibration = true;
                    _ScanningSection = BCIScanSections.Sentence;
                    EvtBCIInitState?.Invoke(BCIState.StartBCIReqParams);
                    break;
                case BCIMenuOptions.Options.KeyboardL:
                    _RequestCalibration = true;
                    _ScanningSection = BCIScanSections.KeyboardL;
                    EvtBCIInitState?.Invoke(BCIState.StartBCIReqParams);
                    break;
                case BCIMenuOptions.Options.Word:
                    _RequestCalibration = true;
                    _ScanningSection = BCIScanSections.Word;
                    EvtBCIInitState?.Invoke(BCIState.StartBCIReqParams);
                    break;
                case BCIMenuOptions.Options.KeyboardR:
                    _RequestCalibration = true;
                    _ScanningSection = BCIScanSections.KeyboardR;
                    EvtBCIInitState?.Invoke(BCIState.StartBCIReqParams);
                    break;
                case BCIMenuOptions.Options.Typing:
                    _ScanningSection = BCIScanSections.Box;
                    _RequestCalibration = false;
                    EvtBCIInitState?.Invoke(BCIState.StartBCIReqParams);
                    break;
                case BCIMenuOptions.Options.EyesCalibration:
                    OnPause();
                    animationSharpManager.CalibrationEyesCloseRequest();
                    BCIInterfaceUtils.ShowCalibrationEyesForm(this);
                    OnResume();
                    EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                    break;
                case BCIMenuOptions.Options.TriggerTest:
                    OnPause();
                    var triggertestParams2 = BCIInterfaceUtils.ShowTriggerTestSettingsForm(this);
                    switch (triggertestParams2.Item1)
                    {
                        case BCIMenuOptions.Options.TriggerTest:
                            animationSharpManager.SetParametersTriggerTest(triggertestParams2.Item2.ScannTime, triggertestParams2.Item2.Targets);
                            BCIInterfaceUtils.ShowTimedMessageBox(this);
                            animationSharpManager.TriggerTestRequest();
                            break;
                        case BCIMenuOptions.Options.Exit:
                            EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                            break;
                    }
                    OnPause();
                    break;
                case BCIMenuOptions.Options.SignalCheck:
                    if (_bciActuator != null)
                        animationSharpManager.RequestSignalMonitor(false);
                    break;
                case BCIMenuOptions.Options.RemapCalibrations:
                    EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                    break;
            }
        }

        /// <summary>
        /// Process calibration status result and display the calibration modes configuration form
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private Tuple<BCIMenuOptions.Options, BCISimpleParameters> BCIShowCalibrationModesWindow(BCICalibrationStatus bciCalibrationStatus)
        {
            //STEP - 2
            _bciActuator.EvtIoctlResponse -= BciActuator_EvtIoctlResponse;
            OnPause();
            Tuple<BCIMenuOptions.Options, BCISimpleParameters> result = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Box, new BCISimpleParameters());
            result = BCIInterfaceUtils.ShowCalibrationModesWindow(bciCalibrationStatus, bciCalibrationStatus.OkToGoToTyping, this);
            OnResume();
            _bciActuator.EvtIoctlResponse += BciActuator_EvtIoctlResponse;
            return result;
        }

        /// <summary>
        /// Show message box - Calibration modes after a calibration finished successfully
        /// </summary>
        private void BCIShowCalibrationResult(string message)
        {
            OnPause();
            BCIInterfaceUtils.ShowCalibrationResultWindow(BCIInterfaceUtils.CALIBRATIONSTATUS, message, this);
            OnResume();
            EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
        }

        /// <summary>
        /// Show the main three options for BCI
        /// </summary>
        /// <param name="bciCalibrationStatus"></param>
        /// <returns></returns>
        private BCIMenuOptions.MainMenuOptions BCIShowMainOptionsWindow(BCICalibrationStatus bciCalibrationStatus)
        {
            OnPause();
            BCIMenuOptions.MainMenuOptions result = BCIMenuOptions.MainMenuOptions.ExitApplication;
            switch (bciCalibrationStatus.OverallStatus)
            {
                case BCIClassifierStatus.Ok:
                    result = BCIInterfaceUtils.ShowMainOptionsWindow(this, BCIInterfaceUtils.RECALIBRATEIF, BCIInterfaceUtils.IMPROVECALIBRATION, bciCalibrationStatus.OkToGoToTyping);
                    break;
                case BCIClassifierStatus.Expired:
                    result = BCIInterfaceUtils.ShowMainOptionsWindow(this, BCIInterfaceUtils.CALIBRATIONNEEDED, BCIInterfaceUtils.CALIBRATIONEXPIRED, bciCalibrationStatus.OkToGoToTyping);
                    break;
                case BCIClassifierStatus.NotFound:
                    result = BCIInterfaceUtils.ShowMainOptionsWindow(this, BCIInterfaceUtils.CALIBRATIONNEEDED, BCIInterfaceUtils.CALIBRATIONNOTFOUND, bciCalibrationStatus.OkToGoToTyping);
                    break;
            }
            OnResume();
            return result;
        }

        /// <summary>
        /// Show message box - Calibration Prompt when calibration failed
        /// </summary>
        private void BCIShowRecalibrationWindowMessage()
        {
            float auc = animationSharpManager.GetAUC();
            SoundManager.playSound(SoundManager.SoundType.CaregiverAttention);
            BCIMenuOptions.MainMenuOptions ConfirmBoxRes = BCIMenuOptions.MainMenuOptions.CalibrateOrShowCalibrationModes;
            try
            {
                OnPause();
                ConfirmBoxRes = BCIInterfaceUtils.ShowRecalibrationWindow(this, auc);
                OnResume();
                switch (ConfirmBoxRes)
                {
                    case BCIMenuOptions.MainMenuOptions.ExitApplication://Exit
                        bool confirmRes = BCIInterfaceUtils.ShowExitAppWindow(this);
                        if (confirmRes)
                            ExitApplication();
                        else
                            BCIShowRecalibrationWindowMessage();
                        break;
                    case BCIMenuOptions.MainMenuOptions.CalibrateOrShowCalibrationModes://Show Calibration modes
                        EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                        break;
                    case BCIMenuOptions.MainMenuOptions.TypingOrRecalibrate://Re-calibrate
                        BCIInterfaceUtils.ShowTimedMessageBox(this);
                        animationSharpManager.CalibrationRequest(_ScanningSection);
                        break;
                }
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | " + es.Message.ToString());
            }
        }

        /// <summary>
        /// Handler for Triggered events
        /// </summary>
        /// <param name="bCIState"></param>
        private void BCIUpdateBCI(BCIState bCIState)
        {
            try
            {
                switch (bCIState)
                {
                    case BCIState.None:
                        break;
                    case BCIState.UIRefresh:
                        Log.Debug("BCI LOG | BCI Init state: " + BCIState.UIRefresh);
                        _BCIState = BCIState.UIRefresh;
                        _ = ControlsUIAdjustment().ConfigureAwait(false);
                        break;
                    case BCIState.Initializing:
                        Log.Debug("BCI LOG | BCI Init state: " + BCIState.Initializing);
                        _BCIState = BCIState.Initializing;
                        _ = InitializeBCI().ConfigureAwait(false);
                        break;
                    case BCIState.ReqCalibrationStatus:
                        _BCIState = BCIState.ReqCalibrationStatus;
                        Log.Debug("BCI LOG | BCI Init state: " + BCIState.ReqCalibrationStatus);
                        _ = BCIRequestCalibrationStatus().ConfigureAwait(false);
                        break;
                    case BCIState.StartBCIReqParams:
                        //STEP - 4
                        Log.Debug("BCI LOG | BCI Init state: " + BCIState.StartBCIReqParams);
                        _BCIState = BCIState.StartBCIReqParams;
                        _ = BCIStartBCIReqParams().ConfigureAwait(false);
                        break;
                    case BCIState.BCIStartSession:
                        Log.Debug("BCI LOG | BCI Init state: " + BCIState.BCIStartSession);
                        _BCIState = BCIState.BCIStartSession;
                        _ = BCIStartSession().ConfigureAwait(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error in BCI Init state: " + BCIState.UIRefresh + "Messagge: " + ex.Message);
            }
        }

        /// <summary>
        /// Event handler for when a button gets focus. Set focus back to the
        /// text box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Button_GotFocus(object sender, EventArgs e)
        {
            _textBoxTalkWindow.Focus();
        }

        /// <summary>
        /// To know if is necessary to change The Texbox UC and avoid adding the same, avoid bug where cursor change place if the same UC is added
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="panelControl"></param>
        /// <returns></returns>
        private bool ChangeTextBoxUserControl(UserControl userControl, Control panelControl)
        {
            if (panelControl.Controls[0].Name.Equals(userControl.Name))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Change User Control to the Main keyboard
        /// </summary>
        private void ChangeToMainKeyboard()
        {
            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
                AddTextBoxUserControl(_textBoxUserControl);
            else
                AddTextBoxUserControl(_textBoxPhraseModeUserControl);
            if (_prevUserControl != null)
            {
                _prevUserControl = null;
                Windows.SetText(_textBoxTalkWindow, _prevText);
                Windows.SetCaretPosition(_textBoxTalkWindow, _prevCaretPosition);
                _prevText = string.Empty;
                _prevCaretPosition = 0;
            }
            List<Widget>[] widgets;
            selectedOption = BCIInterfaceUtils.NA;
            _scannerCommon.UserControlManager.PushUserControlByKeyOrName(scannerPanelKeyboard, null, "KeyboardABCUserControlBCI", true);
            widgets = SaveUserControlWidgets();
            SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), KeyboardABCUserControlBCI.getpathConfigFile());
        }

        /// <summary>
        /// Change User Control to the Yes/No keyboard
        /// </summary>
        private void ChnageToYesNoKeyboard()
        {
            List<Widget>[] widgets;
            _scannerCommon.UserControlManager.PushUserControlByKeyOrName(scannerPanelKeyboard, null, "KeyboardYesNoUserControlBCI", true);
            widgets = SaveUserControlWidgets();
            SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), KeyboardYesNoUserControlBCI2.getpathConfigFile());
        }

        /// <summary>
        /// Crop the text to a desired size
        /// </summary>
        /// <param name="size">Amount of characters </param>
        /// <param name="text">Text to crop</param>
        /// <returns></returns>
        private string CropText(int size, string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                text = TextUtils.EmbedEllipses(text, size);
            }
            return text;
        }

        /// <summary>
        /// Close the application
        /// </summary>
        private void ExitApplication()
        {
            try
            {
                Windows.CloseForm(this);
            }
            catch (Exception e)
            {
                Log.Debug("BCI LOG | Error in ExitApplication() BCI: " + e.Message);
            }
        }

        /// <summary>
        /// Gets the string for the CRG Prompt
        /// </summary>
        /// <returns></returns>
        private string GetCRGText()
        {
            string crgText = string.Empty;
            var mode = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode();
            switch (mode)
            {
                case WordPredictionModes.None:
                    crgText = " ";
                    break;
                case WordPredictionModes.Sentence:
                    crgText = "Mode: Sentence";
                    break;
                case WordPredictionModes.Shorthand:
                    crgText = "Mode: Shorthand";
                    break;
                case WordPredictionModes.CannedPhrases:
                    crgText = "Mode: Canned Phrases";
                    break;
            }
            return crgText;
        }

        /// <summary>
        /// Returns the previous para of text from where the cursor is
        /// </summary>
        /// <returns>text of previous para</returns>
        private String GetPreviousPara()
        {
            int index = _textBoxTalkWindow.SelectionStart;
            var text = _textBoxTalkWindow.Text;

            if (text.Length == 0)
            {
                return String.Empty;
            }

            if (index >= text.Length)
            {
                index = text.Length - 1;
            }

            while (index > 0 && (text[index] == '\r' || text[index] == '\n'))
            {
                index--;
            }

            int endPos = index;

            while (index > 0 && text[index] != '\r' && text[index] != '\n')
            {
                index--;
            }

            if (index > 0 && (text[index] == '\r' || text[index] == '\n'))
            {
                index++;
            }

            int startPos = index;

            return text.Substring(startPos, endPos - startPos + 1);
        }

        /// <summary>
        /// Gets the text used to save to Canned Phrases DB
        /// </summary>
        /// <returns></returns>
        private string GetTextCannedPhrases()
        {
            var text = _textBoxTalkWindow.Text;
            if (String.IsNullOrEmpty(_textBoxTalkWindow.Text.Trim()))
            {
                return string.Empty;
            }
            String textToLearn = String.Empty;
            using (var context = Context.AppAgentMgr.ActiveContext())
            {
                int caretPos = context.TextAgent().GetCaretPos();
                var start = TextUtils.GetStartIndexCurrOrPrevSentence(text, caretPos);
                int end = -1;
                if (start >= 0)
                {
                    end = TextUtils.GetIndexNextSentence(text, start);
                }
                if (start >= 0 && end >= 0 && (end - start) > 0)
                {
                    textToLearn = text.Substring(start, end - start);
                }
            }
            return textToLearn;
        }

        /// <summary>
        /// Request to Pressagio to learn a text
        /// </summary>
        /// <param name="textToLearn"></param>
        private void LearnCannedPhrases(string textToLearn)
        {
            if (!String.IsNullOrEmpty(textToLearn))
            {
                WordPredictionManager.Instance.ActiveWordPredictor.Learn(textToLearn, WordPredictorMessageTypes.LearnCanned);
            }
        }

        /// <summary>
        /// Logs the assembly version
        /// </summary>
        private void LogAssemblyVersion()
        {
            var version = ACATPreferences.ApplicationAssembly.GetName().Version.Major + "." + ACATPreferences.ApplicationAssembly.GetName().Version.Minor;
            Log.Debug("BCI LOG | ACAT - Assembly version info");
            Log.Debug("BCI LOG | AssemblyVersion: " + version);
        }

        /// <summary>
        /// Pause animations and requests for BCI 
        /// </summary>
        private void PauseAnimations()
        {
            _bciActuator.IoctlRequest((int)OpCodes.Pause, "true");
            animationSharpManager.SuspendAnimations = true;
        }

        /// <summary>
        /// Removes all the watchdogs
        /// </summary>
        private void RemoveWatchdogs()
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
                _windowActiveWatchdog = null;
            }
        }

        /// <summary>
        /// Resume animations and requests for BCI
        /// </summary>
        private void ResumeAnimations()
        {
            if (animationSharpManager.SuspendAnimations)
            {
                _bciActuator.IoctlRequest((int)OpCodes.Pause, "false");
            }
            BCIInterfaceUtils.ShowTimedMessageBox(this);
            animationSharpManager.ResumeAfterPause();
        }

        /// <summary>
        /// Saves the caret position for the current TextBox UserControl
        /// </summary>
        /// <param name="panelControl"></param>
        private void SaveCaretPositionForTextBoxUC(Control panelControl)
        {
            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
            {
                if (panelControl.Controls.Count > 0 && panelControl.Controls[0].Name.Equals("TalkWindowTextBoxUserControl"))
                {
                    BCIInterfaceUtils.SaveCaretPositionOfTextBoxUC(Windows.GetCaretPosition(_textBoxTalkWindow));
                }
            }
            else
            {
                if (panelControl.Controls.Count > 0 && panelControl.Controls[0].Name.Equals("TalkWindowTextBoxPhraseModeUserControl"))
                {
                    BCIInterfaceUtils.SaveCaretPhrasePositionOfTextBoxUC(Windows.GetCaretPosition(_textBoxTalkWindow));
                }
            }
        }

        /// <summary>
        /// Sets the objects for the trigger of buttons BCI
        /// </summary>
        /// <returns>List of Widgets</returns>
        private List<Widget>[] SaveUserControlWidgets()
        {
            List<Widget>[] allWidgets = new List<Widget>[3];
            List<IUserControl> listWords = new List<IUserControl>();
            UserControlManager.FindAllUserControls(scannerPanelWordPredictions, listWords);
            List<IUserControl> listSentences = new List<IUserControl>();
            UserControlManager.FindAllUserControls(scannerPanelSentences, listSentences);
            List<IUserControl> listKeyboard = new List<IUserControl>();
            UserControlManager.FindAllUserControls(scannerPanelKeyboard, listKeyboard);


            allWidgets[0] = UserControlManager.findAllWidgets(listWords);
            allWidgets[1] = UserControlManager.findAllWidgets(listSentences);
            allWidgets[2] = UserControlManager.findAllWidgets(listKeyboard);
            return allWidgets;
        }

        /// <summary>
        /// Event handler when changing to quiting Lock screen UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenLockTextBoxUserControl_EvtScreenUnlocked(object sender, EventArgs e)
        {
            animationSharpManager.CRGText = GetCRGText();
            animationSharpManager.LockAnimation(false);
            ChangeToMainKeyboard();
        }

        /// <summary>
        /// Sets the caret position for the main TextBox UserControl
        /// </summary>
        /// <param name="userControl">User control to be added</param>
        /// <param name="panelControl">Current User control placed in panel</param>
        /// <param name="currentCaretPosition">Curretn caret position</param>
        private void SetCaretPositionForTextBoxUC(UserControl userControl, Control panelControl, int currentCaretPosition)
        {

            try
            {
                if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
                {
                    if (panelControl.Controls.Count > 0 && panelControl.Controls[0].Name.Equals("TalkWindowTextBoxUserControl") && userControl.Name.Equals("TalkWindowTextBoxUserControl"))
                    {
                        if (BCIInterfaceUtils.GetCaretPositionOfTextBoxUC() != currentCaretPosition)
                        {
                            Windows.SetCaretPosition(_textBoxTalkWindow, BCIInterfaceUtils.GetCaretPositionOfTextBoxUC());
                            Log.Debug("BCI LOG | Caret position reestablish | Textbox user control changed");
                        }
                    }
                }
                else
                {
                    if (panelControl.Controls.Count > 0 && panelControl.Controls[0].Name.Equals("TalkWindowTextBoxPhraseModeUserControl") && userControl.Name.Equals("TalkWindowTextBoxPhraseModeUserControl"))
                    {
                        if (BCIInterfaceUtils.GetCaretPhrasePositionOfTextBoxUC() != currentCaretPosition)
                        {
                            Windows.SetCaretPosition(_textBoxTalkWindow, BCIInterfaceUtils.GetCaretPhrasePositionOfTextBoxUC());
                            Log.Debug("BCI LOG | Caret position reestablish | Textbox user control changed");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Debug("BCI LOG | Error | SetCaretPositionForTextBoxUC: " + ex.Message);
            }

        }

        /// <summary>
        /// Sets the objects used by SharpDX 
        /// </summary>
        /// <param name="panel1Path">User Control Config file Path</param>
        /// <param name="panel2Path">User Control Config file Path</param>
        /// <param name="panel3Path">User Control Config file Path</param>
        /// <param name="init">First time setting the objects</param>
        private void SetUserControlData(List<Widget>[] widgets, string panel1Path, string panel2Path, string panel3Path, bool init = false)
        {
            //The order of the panels and boxesData matters is the order of the box ID that will carry on to the class that handles the animations sequences
            var panelWordPredictions = GetControls("ScannerButtonControl", scannerPanelWordPredictions.Controls[0]);
            var panelSentences = GetControls("ScannerButtonControl", scannerPanelSentences.Controls[0]);
            var panelKeyboard = GetControls("ScannerButtonControl", scannerPanelKeyboard.Controls[0]);
            Dictionary<List<Control>, string> boxesData = new Dictionary<List<Control>, string>()
                    {
                        { panelWordPredictions, panel1Path },
                        { panelSentences, panel2Path },
                        { panelKeyboard, panel3Path }
                    };
            if (init)
                animationSharpManager.SetDataObjectsSharpDX(boxesData, widgets);
            else
                animationSharpManager.ChangeUserControllayout(boxesData, widgets);
        }

        /// <summary>
        /// Displays the about box
        /// </summary>
        /// <param name="parentForm">scanner form</param>
        private void ShowAboutBox(Form parentForm)
        {
            object[] attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var appName = (attributes.Length != 0) ? ((AssemblyTitleAttribute)attributes[0]).Title : String.Empty;
            var version = ACATPreferences.ApplicationAssembly.GetName().Version.Major + "." + ACATPreferences.ApplicationAssembly.GetName().Version.Minor;
            var versionInfo = String.Format(R.GetString("Version"), version);
            attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var copyrightInfo = (attributes.Length != 0) ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : String.Empty;
            attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            var companyName = (attributes.Length != 0) ? ((AssemblyCompanyAttribute)attributes[0]).Company : String.Empty;
            DialogUtils.ShowAboutBox(parentForm, appName, versionInfo, companyName, copyrightInfo, Attributions.GetAll());
        }

        /// <summary>
        /// Task to have a delay before showing the timed message box
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private async Task ShowTimedMessageBoxAsync()
        {
            await Task.Delay(150);//Wait until AnimationSharpmanager has already received the parameters from his side so the value is from the configuration
            await Task.Delay(animationSharpManager.GetDelayToGetReady());
            BCIInterfaceUtils.ShowTimedMessageBox(this);
            EvtBCIInitState?.Invoke(BCIState.BCIStartSession);
        }
        /// <summary>
        /// Use Text to spech 
        /// </summary>
        private void Speak()
        {
            if (String.IsNullOrEmpty(_textBoxTalkWindow.Text.Trim()))
            {
                return;
            }
            string textToSpeak = GetPreviousPara();
            TtsAndLearn(textToSpeak);
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void SubscribeToEvents()
        {
            Load += TalkApplicationScanner_Load;
            Shown += TalkApplicationScannerQwerty_Shown;
            FormClosing += TalkApplicationScanner_FormClosing;
            EvtBCIInitState += BCIUpdateBCI;
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void TalkApplicationScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveWatchdogs();
            UnsubscribeToEvents();
            animationSharpManager.OnFormClossing();
            LEDStatusUserControl.OnFormClossing();
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// The form has loaded.  Perform initialization.
        /// </summary>
        private void TalkApplicationScanner_Load(object sender, EventArgs e)
        {
            var icon = ImageUtils.GetEntryAssemblyIcon();
            if (icon != null)
                Icon = icon;
            _bciActuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            if (_bciActuator != null)
                _bciActuator.EvtIoctlResponse += BciActuator_EvtIoctlResponse;
            _textBoxTalkWindow.Focus();
            WordPredictionManager.Instance.ActiveWordPredictor.PredictionWordCount = 9;
            _scannerCommon.OnLoad();
            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);
            _textBoxTalkWindow.BackColor = colorScheme.Background;
            _textBoxTalkWindow.ForeColor = colorScheme.Foreground;
            _scannerCommon.ResizeToFitDesktop(this);
            _windowActiveWatchdog = new WindowActiveWatchdog(this);
            animationSharpManager = new AnimationSharpManagerV2();
            animationSharpManager.EvtBCIResumeWatchDog += BCIEvent;
            animationSharpManager.EvtBCIExitApplication += ExitApplication;
            animationSharpManager.EvtBCIStartCalibration += BCIShowRecalibrationWindowMessage;
            animationSharpManager.EvtBCICalibrationComplete += BCIShowCalibrationResult;
            animationSharpManager.EvtBCIUpdateTexttBox += UpdatetextBoxEvt;
            LEDStatusUserControl userControlLED = new LEDStatusUserControl  {  Dock = DockStyle.Fill  };
            panelLEDStatus.Controls.Add(userControlLED);
            BCIR.InitResourceManagerBCI();
        }

        /// <summary>
        /// Event handler for when form is shown
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TalkApplicationScannerQwerty_Shown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
            var mainColorScheme = AnimationManagerUtils.GetMainColorScheme("BCIColorCodedRegionDefault");
            _textBoxTalkWindow.BackColor = mainColorScheme.HighlightForeground;
            panelTextbox.BackColor = mainColorScheme.Background;
        }

        /// <summary>
        /// Key press event for the text box.  If user hit enter,
        /// convert text to speech
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="keyPressEventArgs">event args</param>
        private void TextBoxTalkWindowOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            try
            {
                if (keyPressEventArgs.KeyChar == '\r')
                {
                    var para = GetPreviousPara();

                    if (String.IsNullOrEmpty(_textBoxTalkWindow.Text.Trim()))
                    {
                        return;
                    }

                    String textToSpeak;

                    using (var context = Context.AppAgentMgr.ActiveContext())
                    {
                        context.TextAgent().GetParagraphAtCaret(out textToSpeak);
                    }

                    if (String.IsNullOrEmpty(textToSpeak) && !String.IsNullOrEmpty(para))
                    {
                        keyPressEventArgs.Handled = true;
                        TextToSpeech(para);
                    }
                    else
                    {
                        TtsAndLearn(textToSpeak);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Converts the specified text to speech
        /// </summary>
        /// <param name="text">text to convert</param>
        private void TextToSpeech(String text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                Log.Debug("*** TTS *** : " + text);
                TTSManager.Instance.ActiveEngine.Speak(text);
                Log.Debug("*** TTS *** : sent text!");

                AuditLog.Audit(new AuditEventTextToSpeech(TTSManager.Instance.ActiveEngine.Descriptor.Name));
            }
        }

        /// <summary>
        /// Converts the current para to speech and notify app about this
        /// </summary>
        /// <param name="text">text to convert</param>
        private void TtsAndLearn(String text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                TextToSpeech(text);
                if (WordPredictionManager.Instance.ActiveWordPredictor.SupportsLearning)
                {
                    switch (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode())
                    {
                        case WordPredictionModes.Sentence:
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnWords);
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnSentence);
                            break;
                        case WordPredictionModes.Shorthand:
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnShorthand);
                            break;
                        case WordPredictionModes.CannedPhrases:
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnCanned);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Unsubscribe from all events
        /// </summary>
        private void UnsubscribeToEvents()
        {
            Load -= TalkApplicationScanner_Load;
            Shown -= TalkApplicationScannerQwerty_Shown;
            FormClosing -= TalkApplicationScanner_FormClosing;
            _screenLockTextBoxUserControl.EvtScreenUnlocked -= ScreenLockTextBoxUserControl_EvtScreenUnlocked;

            EvtBCIInitState -= BCIUpdateBCI;
            _bciActuator.EvtIoctlResponse -= BciActuator_EvtIoctlResponse;
            animationSharpManager.EvtBCIResumeWatchDog -= BCIEvent;
            animationSharpManager.EvtBCIExitApplication -= ExitApplication;
            animationSharpManager.EvtBCIStartCalibration -= BCIShowRecalibrationWindowMessage;
            animationSharpManager.EvtBCICalibrationComplete -= BCIShowCalibrationResult;
            animationSharpManager.EvtBCIUpdateTexttBox -= UpdatetextBoxEvt;

            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged -= ActiveWordPredictor_EvtModeChanged;
            _textBoxTalkWindow.KeyPress -= TextBoxTalkWindowOnKeyPress;
            Paint -= (s, args) => { _formShown = true; };
        }
        private void UpdateTextBox(string message)
        {
            if (_getCaretPosition)
            {
                _getCaretPosition = false;
                _prevCaretPosition = Windows.GetCaretPosition(_textBoxTalkWindow);
            }
            AddTextBoxUserControl(_textBoxPromptUserControlLabel);
            _textBoxPromptUserControlLabel.SetText(message);
        }

        /// <summary>
        /// Event from BCI actuator to update the text from of the TextBox
        /// </summary>
        /// <param name="message"></param>
        private void UpdatetextBoxEvt(string message)
        {
            UpdateTextBox(message);
        }
        /// <summary>
        /// Handles yes/no command, sets the choice and then
        /// closes the scanner
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">the command to execute</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">true if it was handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                var form = Dispatcher.Scanner.Form as TalkApplicationBCIScanner;
                if (!form.animationSharpManager.IsCalibrationActive())
                {
                    handled = true;
                    List<Widget>[] widgets;
                    Log.Debug("BCI LOG | Command | selected | Pressed | " + Command.ToString());
                    switch (Command)
                    {
                        case "CmdEditScanner":
                            form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.scannerPanelKeyboard, "edit", "KeyboardEditUserControlBCI", true);
                            widgets = form.SaveUserControlWidgets();
                            form.SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), KeyboardEditUserControlBCI.getpathConfigFile());
                            break;
                        case "CmdKeyboardScannerNum":
                            form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.scannerPanelKeyboard, null, "NumericUserControlBCI", true);
                            widgets = form.SaveUserControlWidgets();
                            form.SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), NumericUserControlBCI.getpathConfigFile());
                            break;
                        case "CmdModesScanner":
                            form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.scannerPanelKeyboard, null, "KeyboardModesUserControlBCI", true);
                            form.animationSharpManager.LockAnimation(true);
                            widgets = form.SaveUserControlWidgets();
                            form.SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), KeyboardModesUserControlBCI.getpathConfigFile());
                            break;
                        case "CmdMenuScanner":
                            form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.scannerPanelKeyboard, null, "MenuUserControlBCI", true);
                            form.animationSharpManager.LockAnimation(true);
                            widgets = form.SaveUserControlWidgets();
                            form.SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), MenuUserControlBCI.getpathConfigFile());
                            break;
                        case "CmdKeyboardMain":
                            form.animationSharpManager.LockAnimation(false);
                            form.ChangeToMainKeyboard();
                            break;
                        case "CmdYes":
                            form.AddTextBoxUserControl(form._textBoxUserControl);
                            switch (form.selectedOption)
                            {
                                case BCIInterfaceUtils.EXIT_APP:
                                    Windows.CloseForm(form);
                                    break;
                                case BCIInterfaceUtils.CLEAR:
                                    form.animationSharpManager.LockAnimation(false);
                                    form.ChangeToMainKeyboard();
                                    Windows.SetText(form._textBoxTalkWindow, string.Empty);
                                    break;
                                case BCIInterfaceUtils.SAVE_TO_CANNED:
                                    form.LearnCannedPhrases(form._TextToLearnCanned);
                                    form._TextToLearnCanned = string.Empty;
                                    form.animationSharpManager.LockAnimation(false);
                                    form.ChangeToMainKeyboard();
                                    break;
                                case BCIInterfaceUtils.MODE_SENTENCE:
                                    Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(WordPredictionModes.Sentence);
                                    form.animationSharpManager.CRGText = form.GetCRGText();
                                    form.animationSharpManager.LockAnimation(false);
                                    form.ChangeToMainKeyboard();
                                    break;
                                case BCIInterfaceUtils.MODE_SHORTHAND:
                                    form.animationSharpManager.LockAnimation(false);
                                    form.ChangeToMainKeyboard();
                                    Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(WordPredictionModes.Shorthand);
                                    form.animationSharpManager.CRGText = form.GetCRGText();
                                    break;
                                case BCIInterfaceUtils.MODE_CANNED:
                                    form.animationSharpManager.LockAnimation(false);
                                    form.ChangeToMainKeyboard();
                                    Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(WordPredictionModes.CannedPhrases);
                                    form.animationSharpManager.CRGText = form.GetCRGText();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "CmdNo":
                            form.animationSharpManager.LockAnimation(false);
                            form.ChangeToMainKeyboard();
                            break;
                        case "CmdExitApp":
                            form.selectedOption = BCIInterfaceUtils.EXIT_APP;
                            form.AddTextBoxUserControl(form._textBoxPromptUserControlLabel);
                            form.UpdateTextBox(BCIInterfaceUtils.EXITPROMPT);
                            form.animationSharpManager.LockAnimation(true);
                            form.ChnageToYesNoKeyboard();
                            break;
                        case "CmdTalkWindowClear":
                            form.selectedOption = BCIInterfaceUtils.CLEAR;
                            form.AddTextBoxUserControl(form._textBoxPromptUserControlLabel);
                            form.UpdateTextBox(BCIInterfaceUtils.CLEARPROMPT);
                            form.animationSharpManager.LockAnimation(true);
                            form.ChnageToYesNoKeyboard();
                            break;
                        case "CmdActuatorToggleCalibrationWindow":
                            form.isSignalMonitorCalledFromMenu = true;
                            if (form._bciActuator != null)
                                form.animationSharpManager.RequestSignalMonitor(false);
                            break;
                        case "CmdEntryModeTyping":
                            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.Sentence)
                            {
                                form.selectedOption = BCIInterfaceUtils.MODE_SENTENCE;
                                form.AddTextBoxUserControl(form._textBoxPromptUserControlLabel);
                                form.UpdateTextBox(BCIInterfaceUtils.MODEROMPT + "Sentence?");
                                form.animationSharpManager.LockAnimation(true);
                                form.ChnageToYesNoKeyboard();
                            }
                            break;
                        case "CmdEntryModeShortHand":
                            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.Shorthand)
                            {
                                form.selectedOption = BCIInterfaceUtils.MODE_SHORTHAND;
                                form.AddTextBoxUserControl(form._textBoxPromptUserControlLabel);
                                form.UpdateTextBox(BCIInterfaceUtils.MODEROMPT + "Short Hand?");
                                form.animationSharpManager.LockAnimation(true);
                                form.ChnageToYesNoKeyboard();
                            }
                            break;
                        case "CmdEntryModePhrase":
                            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
                            {
                                form.selectedOption = BCIInterfaceUtils.MODE_CANNED;
                                form.AddTextBoxUserControl(form._textBoxPromptUserControlLabel);
                                form.UpdateTextBox(BCIInterfaceUtils.MODEROMPT + "Canned Phrases?");
                                form.animationSharpManager.LockAnimation(true);
                                form.ChnageToYesNoKeyboard();
                            }
                            break;
                        case "CmdSaveToCanned":
                            form._TextToLearnCanned = form.GetTextCannedPhrases();
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append(BCIInterfaceUtils.SAVEPROMPT);
                            stringBuilder.AppendLine();
                            string val = form.CropText(36, form._TextToLearnCanned);
                            stringBuilder.Append(val);
                            form.selectedOption = BCIInterfaceUtils.SAVE_TO_CANNED;
                            form.AddTextBoxUserControl(form._textBoxPromptUserControlLabel);
                            form.UpdateTextBox(stringBuilder.ToString());
                            form.animationSharpManager.LockAnimation(true);
                            form.ChnageToYesNoKeyboard();
                            break;
                        case "CmdTTSYesNoScanner":
                            form.AddTextBoxUserControl(form._textBoxPromptUserControl);
                            Windows.SetText(form._textBoxTalkWindow, string.Empty);
                            form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.scannerPanelKeyboard, null, "TTSYesNoUserControlBCI", true);
                            form.animationSharpManager.LockAnimation(true);
                            widgets = form.SaveUserControlWidgets();
                            form.SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), TTSYesNoUserControlBCI2.getpathConfigFile());
                            break;
                        case "CmdTTSYes":
                            Windows.SetText(form._textBoxTalkWindow, "Yes");
                            break;
                        case "CmdTTSNo":
                            Windows.SetText(form._textBoxTalkWindow, "No");
                            break;
                        case "CmdLockScanner":
                            form._prevText = Windows.GetText(form._textBoxTalkWindow);
                            form._prevCaretPosition = Windows.GetCaretPosition(form._textBoxTalkWindow);
                            if (form.panelTextbox.Controls.Count > 0)
                            {
                                form._prevUserControl = form.panelTextbox.Controls[0] as UserControl;
                            }
                            form.AddTextBoxUserControl(form._screenLockTextBoxUserControl);
                            form.animationSharpManager.CRGText = "LOCKED";
                            form.animationSharpManager.LockAnimation(true);
                            form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.scannerPanelKeyboard, null, "KeyboardLockUserControlBCI", true);
                            widgets = form.SaveUserControlWidgets();
                            form.SetUserControlData(widgets, WordPredictionUserControlBCI.getpathConfigFile(), PhrasesUserControlBCI.getpathConfigFile(), KeyboardLockUserControlBCI.getpathConfigFile());
                            break;
                        case "CmdShowAboutBox":
                            if (form._bciActuator != null)
                            {
                                form.PauseAnimations();
                                form.ShowAboutBox(form);
                                form.ResumeAnimations();
                            }
                            break;
                        case "CmdSpeak":
                            form.Speak();
                            break;
                        case "CmdRecalibrate":
                            while (!form.animationSharpManager.RequestRecalibration()) { }
                            form.ChangeToMainKeyboard();
                            form.EvtBCIInitState?.Invoke(BCIState.ReqCalibrationStatus);
                            break;
                        default:
                            break;
                    }

                }else
                    Log.Debug("BCI LOG | Command | selected | Pressed in calibration | No action | " + Command.ToString());
                return true;
            }
        }
        /// <summary>
        /// Command dispatcher
        /// </summary>
        private class Dispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="panel">the scanner object</param>
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
                Commands.Add(new CommandHandler("CmdEditScanner"));
                Commands.Add(new CommandHandler("CmdEntryModeSelect"));
                Commands.Add(new CommandHandler("CmdMenuScanner"));
                Commands.Add(new CommandHandler("CmdNumberScanner"));
                Commands.Add(new CommandHandler("CmdKeyboardMain"));
                Commands.Add(new CommandHandler("CmdModesScanner"));
                Commands.Add(new CommandHandler("CmdTTSYesNoScanner"));
                Commands.Add(new CommandHandler("CmdLockScanner"));
                Commands.Add(new CommandHandler("CmdExitApp"));
                Commands.Add(new CommandHandler("CmdSpeak"));
                Commands.Add(new CommandHandler("CmdAutocompleteWithFirstWord"));
                Commands.Add(new CommandHandler("CmdMainKeyboard"));
                Commands.Add(new CommandHandler("CmdShowAboutBox"));
                Commands.Add(new CommandHandler("CmdKeyboardScannerNum"));
                Commands.Add(new CommandHandler("CmdYes"));
                Commands.Add(new CommandHandler("CmdNo"));
                Commands.Add(new CommandHandler("CmdTTSYes"));
                Commands.Add(new CommandHandler("CmdTTSNo"));
                Commands.Add(new CommandHandler("CmdTalkWindowClear"));
                Commands.Add(new CommandHandler("CmdActuatorToggleCalibrationWindow"));
                Commands.Add(new CommandHandler("CmdSaveToCanned"));
                Commands.Add(new CommandHandler("CmdEntryModeTyping"));
                Commands.Add(new CommandHandler("CmdEntryModePhrase"));
                Commands.Add(new CommandHandler("CmdEntryModeShortHand"));
                Commands.Add(new CommandHandler("CmdRecalibrate"));
            }
        }
    }
}