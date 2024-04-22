////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AnimationSharpManager.cs
//
/// Manages the visual states of widgets, starts and stops
/// animations and handles transitions between animations.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Extensions.BCI.Common.BCIInterfaceUtilities;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using Newtonsoft.Json;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System;

namespace ACAT.Extensions.BCI.Common.AnimationSharp
{
    /// <summary>
    /// Manages the display states of the various widgets, starts and stops
    /// animations and and also handles transitions between animations.
    /// </summary>
    public class AnimationSharpManagerV2
    {
        /// <summary>
        /// Current active Keyboard Layout
        /// </summary>
        private volatile int _activeKeyboard;

        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _actuator;

        /// <summary>
        /// Used to know that the actuator is active and is responsive
        /// </summary>
        private bool _actuatorAnswer;

        /// <summary>
        /// Amount of layouts in the UI
        /// </summary>
        private int _amountOfKeyboards = 0;

        /// <summary>
        /// BCI calibration succes rate
        /// </summary>
        private float _AUC = 0;

        /// <summary>
        /// Border thickness
        /// </summary>
        private float _BorderWidth = 1;

        private Dictionary<List<Control>, string> _BoxesData;
        /// <summary>
        /// Contains ID, text, action, name and border setting for each button
        /// </summary>
        private List<ButtonsData>[] _ButtonDataList;

        /// <summary>
        /// List of strings store in each button
        /// </summary>
        private List<string>[] _buttonsStringsList;

        private TextFormat _buttonTextFormatCRG;
        /// <summary>
        /// Format for each of the buttons for each user control
        /// </summary>
        private List<TextFormat>[] _buttonTextFormatList;

        /// <summary>
        /// Log object
        /// </summary>
        private CachedLogBCI _cachedLog;

        /// <summary>
        /// Index for the box used for the calibration layout
        /// </summary>
        private int _CalibrationBox = 0;

        /// <summary>
        /// Iterations per sequence
        /// </summary>
        private int _CalibrationIterationsPerTarget = 1;

        /// <summary>
        /// Calibration text string target 
        /// </summary>
        private string _CalibrationStringtarget = string.Empty;

        /// <summary>
        /// Amount of repetitions per full sequences
        /// </summary>
        private int _CalibrationTargetCount = 60;

        /// <summary>
        /// Change the interval of the animation timer
        /// </summary>
        private volatile bool _ChangeTimings = false;

        /// <summary>
        /// Used to know when the change of objects for BCI need to be changed
        /// </summary>
        private volatile bool _changeUserControl;

        /// <summary>
        /// Button to be selected as a focus in the UI
        /// </summary>
        private ButtonsData _currentCalibrationTarget;

        private int _currEpochCount = 1;
        /// <summary>
        /// Counters for the animations
        /// </summary>
        private int _currSeqCount = 1;

        private int _currtButtonCount = 0;
        private int _currtButtonCount2 = 0;
        private int _customLockBoxAnimation = 0;
        /// <summary>
        /// ID of the decided button to be triggered
        /// </summary>
        private int _decidedID;

        /// <summary>
        /// Default target as lock animation box
        /// </summary>
        private bool _defaultLockBoxanimation = true;

        /// <summary>
        /// Time used to display the decision
        /// </summary>
        private int _delayDisplayDecision = 3500;

        /// <summary>
        /// Delay used before going into typing
        /// </summary>
        private int _DelayToGetReady = 3000;
        /// <summary>
        /// SharpDX Main UI object
        /// </summary>
        private SharpDX.DirectWrite.Factory _directWriteFactory;

        /// <summary>
        /// Option to keep the text to not overflow horizontally
        /// </summary>
        private DrawTextOptions _drawTextOptions = DrawTextOptions.Clip;

        /// <summary>
        /// Flag used to know when Calibration mode has ended
        /// </summary>
        private bool _endCalibration = false;

        private MicroTimer _endEpochTimer;

        /// <summary>
        /// Flag to skip the loop and end the task
        /// </summary>
        private bool _finishTask = false;

        /// <summary>
        /// Used when is the first iteration being played 
        /// </summary>
        private bool _firstSequence;

        /// <summary>
        /// Sequences of which row or columns will be highlighted (Box level)
        /// </summary>
        private List<int>[] _flashingSequenceBoxList;

        /// <summary>
        /// ID of the button in the Sequences of which row or columns will be highlighted (Box level)
        /// </summary>
        private List<int>[] _flashingSequenceIDBoxList;

        /// <summary>
        /// ID of the button in the Sequences of which row or columns will be highlighted 
        /// </summary>
        private List<int[]>[] _flashingSequenceIDList;

        /// <summary>
        /// Sequences of which row or columns will be highlighted 
        /// </summary>
        private List<int[]>[] _flashingSequenceList;

        /// <summary>
        /// Color used for circles in focal point
        /// </summary>
        private string _FocalCircleColor;

        /// <summary>
        /// Index in the array of the target to be display based on the calibration text string
        /// </summary>
        private int _indexTargetCalibration = 0;

        /// <summary>
        /// if box level active
        /// </summary>
        private bool _isBoxScannig;

        /// <summary>
        /// Flag when is required to trigger the button
        /// </summary>
        private bool _isDecisionMade;

        /// <summary>
        /// To fill the circles used as focal point in box
        /// </summary>
        private bool _IsFocalCircleFilled;
        /// <summary>
        /// Lock the animation to a single box
        /// </summary>
        private bool _lockAnimation = false;

        /// <summary>
        /// Object of the Form used for the UI and Animations
        /// </summary>
        private Form _mainForm;

        /// <summary>
        /// Minimum Value allowed to be visible for the progres bars
        /// </summary>
        private double _MinimumProgressBarsValue;

        /// <summary>
        /// Next button to have focus on
        /// </summary>
        private ButtonsData _nextTarget;

        /// <summary>
        /// Amount of animations (Rows and Columns)
        /// </summary>
        private int _numberOfSequences;

        private int _offsetCRG;

        /// <summary>
        /// Default values for the position of the letters/images inside the rectangle
        /// </summary>
        private List<int>[] _offsetStrings;

        /// <summary>
        /// value used to add pading when drawing text right and left
        /// </summary>
        private int _PaddingText = 20;

        /// <summary>
        /// value used to add pading when drawing text at the bottom
        /// </summary>
        private int _PaddingTextBottom = 10;

        private MicroTimer _pauseBetweenEpochsTimer;

        private int _pauseTime = 0;

        /// <summary>
        /// Probability bars for each of the buttons from each layout
        /// </summary>
        private SortedDictionary<int, double> _progressBarsProbs = null;

        /// <summary>
        /// Radious for each corner of each button
        /// </summary>
        private float _RadiusCornersButtons = 5;

        private bool _readStartSesionResult = false;

        /// <summary>
        /// Rectangles for the Trigger button BtnCRG
        /// </summary>
        private SharpDX.Mathematics.Interop.RawRectangleF _rectangleExtraButton;

        /// <summary>
        /// Rectangle for the CRG Text
        /// </summary>
        private SharpDX.Mathematics.Interop.RawRectangleF _rectangleExtraButtonCRG;

        /// <summary>
        /// Rectangles for each button in the Form
        /// </summary>
        private List<SharpDX.Mathematics.Interop.RawRectangleF>[] _rectanglesButtonsList;

        /// <summary>
        /// Rounded Rectangles for each button in the Form
        /// </summary>
        private List<SharpDX.Direct2D1.RoundedRectangle>[] _rectanglesButtonsRoundList;

        /// <summary>
        /// Rectangles for each progress Bar in the Form
        /// </summary>
        private Dictionary<int, SharpDX.Mathematics.Interop.RawRectangleF>[] _rectanglesProbBars;

        /// <summary>
        /// Rectangles for each progress Bar in the Form (Box level)
        /// </summary>
        private Dictionary<int, SharpDX.Mathematics.Interop.RawRectangleF> _rectanglesProbBarsBox;

        /// <summary>
        /// Flag used to know when Calibration mode need to be reestarted 
        /// </summary>
        private bool _repeatCalibration = false;

        /// <summary>
        /// Flag ro request probabilities even if they are the same
        /// </summary>
        private volatile bool _requestProbabilities = false;

        /// <summary>
        /// No decision made return to box scanning
        /// </summary>
        private bool _ReturnToBoxScanning = false;

        /// <summary>
        /// Mode in which the calibration sesion is set
        /// </summary>
        private BCIScanSections _ScanningSection = BCIScanSections.KeyboardR;

        /// <summary>
        /// When a sequence is done highlighting
        /// </summary>
        private bool _sequenceDone = false;

        /// <summary>
        /// Object of the mode of the UI
        /// </summary>
        private BCIModes _sessionMode = BCIModes.TYPING;

        private SharpDX.Direct2D1.Factory _sharpDX_d2dFactory;

        private RenderTarget _sharpDX_d2dRenderTarget;

        /// <summary>
        /// SharpDX objects
        /// </summary>
        private SharpDX.Direct3D11.Device _sharpDX_device;

        private SwapChain _sharpDX_swapChain;

        /// <summary>
        /// Time between epoch
        /// </summary>
        private int _shortPauseTime = 0;

        /// <summary>
        /// Seconds to pause for the decision made
        /// </summary>
        private int _showDecisionTime = 0;

        /// <summary>
        /// Timer for the decision made
        /// </summary>
        private MicroTimer _showDecisionTimer;

        /// <summary>
        /// Show probabilities bars?
        /// </summary>
        private bool _showProbabilityIndicator = true;

        /// <summary>
        /// 
        /// </summary>
        private int _slotTrialTimerCount = 0;

        /// <summary>
        /// After reseting the timers start the timer?
        /// </summary>
        private bool _StartSequences = true;

        /// <summary>
        /// Stop animations sequences?
        /// </summary>
        private bool _stopAnimation;

        private MicroTimer _targetOffTimer;

        /// <summary>
        /// List of the Controls from the widgets if main UI
        /// </summary>
        private Dictionary<List<Control>, string> _TempBoxesData;

        private List<Widget>[] _tempwidgets;

        /// <summary>
        /// Timers for Animations sequences
        /// </summary>
        private int _trialITI = 0;

        /// <summary>
        /// Timer object fot the sequences animations for BCI
        /// </summary>
        private MicroTimer _trialTimer;

        /// <summary>
        /// Timer object for the trigger test
        /// </summary>
        private MicroTimer _TriggerTest_Timer;

        /// <summary>
        /// Is the trigger test active
        /// </summary>
        private bool _triggerTestActive = false;

        /// <summary>
        /// Interval for the trigger test timer
        /// </summary>
        private int _TriggerTestInterval = 200;

        /// <summary>
        /// Repetitions for the duty cycle in trigger test
        /// </summary>
        private int _TriggerTestRepetitions = 10;

        /// <summary>
        /// The kind of box (Words, keyboard, menu, sentences)
        /// </summary>
        private string[] _typeOfBox;

        /// <summary>
        /// Iterations per sequence
        /// </summary>
        private int _TypingIterationsPerTarget = 1;

        /// <summary>
        /// Amount of repetitions per full sequences ( 0 = infinite )
        /// </summary>
        private int _TypingTargetCount = 0;

        /// <summary>
        /// Flag to alow updating the strings from buttons
        /// </summary>
        private bool _UpdateButtonsStrings = true;
        /// <summary>
        /// If using random value for calibration target
        /// </summary>
        private bool _useRandomSelectionTargetCalibration = true;

        /// <summary>
        /// Widhets for each of the User Control boxes
        /// </summary>
        private List<Widget>[] _widgets;

        /// <summary>
        /// Object with methods used by BCI class
        /// </summary>
        private BCIUtils BCIUtils;
        /// <summary>
        /// Object containing the Colors used by SharpDX
        /// </summary>
        private SharpDXColors SharpDXColors;
        public AnimationSharpManagerV2()
        {
            
        }
        /// <summary>
        /// Event objects
        /// </summary>
        public event BCIEvents.BCICalibrationComplete EvtBCICalibrationComplete;
        public event BCIEvents.BCIExitApplication EvtBCIExitApplication;
        public event BCIEvents.BCIResumeWatchdog EvtBCIResumeWatchDog;
        public event BCIEvents.BCIStartCalibration EvtBCIStartCalibration;
        public event BCIEvents.BCIUpdateTextBox EvtBCIUpdateTexttBox;

        public event BCIEvents.BCIParametersResult EvtParametersResult;

        /// <summary>
        /// Text used by the CRG area
        /// </summary>
        public string CRGText { get; set; }

        /// <summary>
        /// Error code for each mode
        /// </summary>
        public BCIError SensorErrorState { get; private set; }
        /// <summary>
        /// Animations requested to stop?
        /// </summary>
        public bool SuspendAnimations { get; set; }

        /// <summary>
        /// List of the controls from the Main UI
        /// </summary>
        private List<ScannerButtonControl>[] _ControlsBtns { get; set; }

        public void CalibrationEyesCloseRequest()
        {
            _readStartSesionResult = false;
        }

        /// <summary>
        /// Request to the actuator to start a calibration session
        /// </summary>
        /// <param name="bCIScanSections"></param>
        public void CalibrationRequest(BCIScanSections bCIScanSections)
        {
            _ScanningSection = bCIScanSections;
            switch (_ScanningSection)
            {
                case BCIScanSections.Word:
                    _CalibrationBox = 0;
                    break;
                case BCIScanSections.Sentence:
                    _CalibrationBox = 1;
                    break;
                case BCIScanSections.KeyboardL:
                    _CalibrationBox = 2;
                    break;
                case BCIScanSections.KeyboardR:
                    _CalibrationBox = 3;
                    break;
            }
            if (_ScanningSection == BCIScanSections.Box)
                _isBoxScannig = true;
            else
                _isBoxScannig = false;
            _readStartSesionResult = true;
            _sessionMode = BCIModes.CALIBRATION;
            _currEpochCount = 1;
            _sequenceDone = false;
            BCIMode bCIMode = new BCIMode();
            bCIMode.BciCalibrationMode = bCIScanSections;
            bCIMode.BciMode = BCIModes.CALIBRATION;
            if (bCIScanSections == BCIScanSections.Box)//Sets the amount of objects into an array so calibration does not repeat targets
                BCIUtils.SetTargetValuesForCalibration(_flashingSequenceBoxList.Length);
            else
                BCIUtils.SetTargetValuesForCalibration(_ButtonDataList[_CalibrationBox].Count);
            var strBciMode = JsonConvert.SerializeObject(bCIMode);
            _actuator.IoctlRequest((int)OpCodes.StartSession, strBciMode);
        }
        /// <summary>
        /// Cancels the active calibration session
        /// </summary>
        public void CancelCalibration()
        {
            if( _sessionMode == BCIModes.CALIBRATION)
            {
                try
                { AbortTimers(); }
                catch (Exception ex)
                { Log.Debug("BCI LOG | Error in BCI CancelCalibration(): " + ex.Message); }
                CloseSequencesLog();
                _sessionMode = BCIModes.TYPING;
                ChangeColorButtons(_flashingSequenceBoxList[0].ToList(), false, true, 0);
                var bciCalibrationEnd = new BCICalibrationEnd { DiscardCalibrationData = true };
                var strCalEnd = JsonConvert.SerializeObject(bciCalibrationEnd);
                _actuator.IoctlRequest((int)OpCodes.CalibrationEnd, strCalEnd);
                _mainForm.Invoke(new MethodInvoker(delegate { EvtBCICalibrationComplete?.Invoke("Calibration cancelled \nSelect calibration mode to start again"); }));
            }
        }
        /// <summary>
        /// To change all the objects used by SharpDX when a User Control is changed
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="widgets"></param>
        /// <returns></returns>
        public async Task ChangeUserControllayout(Dictionary<List<Control>, string> boxesData, List<Widget>[] widgets)
        {
            await Task.Delay(20);
            _changeUserControl = true;//like this if a timer uses SharpDx it wont proceed
            _stopAnimation = true;//like this if a timer uses SharpDx it wont proceed
            StopTimers();
            await Task.Delay(20);
            AbortTimers();//Ensure that timers are stoped
            await Task.Delay(100);
            if (!VerifyIfTimersAreStoped())//Doeble check timers are stoped, if a change is made while SharpDx is working falls into and exception and will break the app
            {
                StopTimers();
                await Task.Delay(20);
                AbortTimers();
                await Task.Delay(100);
            }
            if (_ChangeTimings && _sessionMode == BCIModes.TYPING)//If a change of user control occur and has to fall into a specific box
                RequestParameters(new BCIMode { BciMode = _sessionMode, BciCalibrationMode = _ScanningSection });
            _TempBoxesData = boxesData;//Save data so after SharpDx is not in used can change the objects used by it
            _tempwidgets = widgets;
            _currEpochCount = 0;
            _isBoxScannig = true;
            _numberOfSequences = _amountOfKeyboards;
            Reset(true);
        }
        /// <summary>
        /// Saves and close the object log
        /// </summary>
        public void CloseSequencesLog()
        {
            _cachedLog?.Save();
            _cachedLog = null;
        }

        /// <summary>
        /// Create sequences log for BCI
        /// </summary>
        public void CreateSequencesLog(string path, string name = null)
        {
            try
            {
                string baseFileName = string.Empty;
                if (_cachedLog != null)
                    CloseSequencesLog();
                if (String.IsNullOrEmpty(name))
                {
                    //This is to avoid if a log is created right after another and share the same name but only changes the time it was created by a few seconds, avoid append the new log into the old log since the name is the same 
                    //Part of the name goes up to minutes not seconds so it have the possibility that the name is the same only applies in the log folder
                    //This is to when the log is saved in the same folder
                    Random rand = new Random();
                    int randomIndex = rand.Next(1, 300);
                    baseFileName = _sessionMode.ToString() + "_" + (_triggerTestActive ? "" : _ScanningSection.ToString()) + "_" + randomIndex;
                    _cachedLog = new CachedLogBCI(baseFileName, path);
                }
                else
                    _cachedLog = new CachedLogBCI(name, path);
            }
            catch(Exception ex)
            {
                Log.Debug("BCI LOG | Exception in CreateSequencesLog() " + ex.Message);
            }

        }

        /// <summary>
        /// Draw the rectangles, background and text of the UI
        /// </summary>
        public void DrawMainLayout()
        {
            try
            {//The (if) flags is a meassure to ensure SharpDx will not be in used while changing an user control or objects used by it
                if (!_stopAnimation && !_changeUserControl)
                {
                    if (!_stopAnimation && !_changeUserControl)
                        _sharpDX_d2dRenderTarget.BeginDraw();
                    if (!_stopAnimation && !_changeUserControl)
                        _sharpDX_d2dRenderTarget.Clear(SharpDXColors.ColorBackground);
                    if (!_stopAnimation && !_changeUserControl)
                        DrawMatrix();
                    if (!_stopAnimation && !_changeUserControl)
                        _sharpDX_d2dRenderTarget.EndDraw();
                    if (!_stopAnimation && !_changeUserControl)
                        _sharpDX_swapChain.Present(1, PresentFlags.None);
                }
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception sharpDX in DrawMainLayout() " + es.Message);
            }
        }

        public float GetAUC()
        {
            return _AUC;
        }

        /// <summary>
        /// gets the delay used to wait before staring to type
        /// </summary>
        /// <returns></returns>
        public int GetDelayToGetReady()
        {
            return _DelayToGetReady;
        }

        /// <summary>
        /// Initialices all the variables used by SharpDX 
        /// </summary>
        /// <param name="form">Main Form</param>
        /// <param name="guid">Form ID</param>
        /// <param name="textBox"></param>
        public void Init(Form form)
        {
            SensorErrorState = new BCIError();
            _activeKeyboard = 0;
            SuspendAnimations = false;
            bool success;
            try
            {
                success = InitSwapChain(form);
                _mainForm = form;
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception occurred during initilization: " + ex.Message);
                return;
            }
            SharpDXColors = new SharpDXColors(_sharpDX_d2dRenderTarget);
            BCIUtils = new BCIUtils();
            AnimationManagerUtils.SetDefaultParameters();
            _RadiusCornersButtons = AnimationManagerUtils.GetParameter("cornerRadius");
            _TypingTargetCount = AnimationManagerUtils.GetParameter("TypingTargetCount");
            _TypingIterationsPerTarget = AnimationManagerUtils.GetParameter("TypingIterationsPerTarget");
            _BorderWidth = AnimationManagerUtils.GetParameter("borderWidth");
            _directWriteFactory = new SharpDX.DirectWrite.Factory();
            _amountOfKeyboards = 0;
            _nextTarget = GetButtomFromIndex(1);
            _ = UpdateStringsFromButtons().ConfigureAwait(false);//Task to update strings for SharpDX since it uses strings and not the Windows butons properties 
            _actuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            if (_actuator != null)
                _actuator.EvtIoctlResponse += Evt_ResponseActuator;
        }

        /// <summary>
        /// Gets if is in calibration mode
        /// </summary>
        /// <returns></returns>
        public bool IsCalibrationActive()
        {
            return _sessionMode == BCIModes.CALIBRATION;
        }

        /// <summary>
        /// To maintain the animation in a single box
        /// </summary>
        /// <param name="lockAnimation">Maintain animation in a single box</param>
        /// <param name="defaultLockBoxanimation">Default box is last box</param>
        /// <param name="customLockBoxAnimation">Custom Box to lock animation</param>
        public void LockAnimation(bool lockAnimation, bool defaultLockBoxanimation = true, int customLockBoxAnimation = 0)
        {
            _lockAnimation = lockAnimation;
            if (lockAnimation)
                _ScanningSection = BCIScanSections.KeyboardR;
            else
                _ScanningSection = BCIScanSections.Box;
            if (_sessionMode == BCIModes.TYPING)
                _ChangeTimings = true;
            _defaultLockBoxanimation = defaultLockBoxanimation;
            _customLockBoxAnimation = customLockBoxAnimation;
        }

        /// <summary>
        /// On Form clossing dispose objects
        /// </summary>
        public void OnFormClossing()
        {
            _stopAnimation = true;
            _changeUserControl = true;
            try
            {
                CloseSequencesLog();
                AbortTimers();
            }
            catch (Exception es)
            {
                AbortTimers();
                Log.Debug("BCI LOG | Exception occurred during closing SharpDX: " + es.Message);
            }
            DisposeObjects();
        }
        /// <summary>
        /// Request to the Actuator to get the parameters for the session
        /// </summary>
        /// <param name="bCIMode"></param>
        /// <param name="bCISimpleParameters"></param>
        public void RequestParameters(BCIMode bCIMode, BCISimpleParameters bCISimpleParameters = null)
        {
            _ScanningSection = bCIMode.BciCalibrationMode;
            if (bCISimpleParameters == null)
            {
                bCISimpleParameters = new BCISimpleParameters();
            }
            BCIUserInputParameters bCIUserInputParameters = new BCIUserInputParameters
            {
                BciMode = bCIMode.BciMode,
                BciCalibrationMode = bCIMode.BciCalibrationMode,
                ScanTime = bCISimpleParameters.ScannTime,
                NumTargets = bCISimpleParameters.Targets,
                NumIterationsPerTarget = bCISimpleParameters.IterationsPertarget,
                MinScoreRequired = bCISimpleParameters.MinScore,
            };
            var strBciModeParams = JsonConvert.SerializeObject(bCIUserInputParameters);
            _actuator.IoctlRequest((int)OpCodes.RequestParameters, strBciModeParams);
        }
        /// <summary>
        /// Request to start calibration manually
        /// </summary>
        public bool RequestRecalibration()
        {
            _StartSequences = false;
            SuspendAnimations = true;
            _ChangeTimings = false;
            _sessionMode = BCIModes.CALIBRATION;
            EnsureTimersAreStoped();
            SuspendAnimations = false;
            LockAnimation(false);
            return true;
        }
        /// <summary>
        /// Request for the Signal monitor
        /// </summary>
        public void RequestSignalMonitor(bool startSequences)
        {
            _StartSequences = startSequences;
            SuspendAnimations = true;
            _ChangeTimings = false;
            _sessionMode = BCIModes.CALIBRATION;
            EnsureTimersAreStoped();
            _actuator.IoctlRequest((int)OpCodes.ToggleCalibrationWindow, String.Empty);
        }

        /// <summary>
        /// To resume animations after dialog box is called
        /// </summary>
        public void ResumeAfterPause()
        {
            if (SuspendAnimations)
            {
                _requestProbabilities = true;
                try { AbortTimers(); } catch (Exception ex) { Log.Debug("BCI LOG | Exception ResumeAfterPause: " + ex.Message); }
                Reset();
            }
        }

        /// <summary>
        /// Sets the main objects used by the Drawing process from SharpDX
        /// </summary>
        /// <param name="boxesData">Data from the User controls</param>
        /// <param name="widgetsData">Controls in the main form data</param>
        public void SetDataObjectsSharpDX(Dictionary<List<Control>, string> boxesData, List<Widget>[] widgetsData)
        {
            int amountBoxes = 0;
            int indexBox = 0;
            for (int index = 0; index < boxesData.Count; index++)
            {
                var val = boxesData.ElementAt(index);
                amountBoxes += AnimationManagerUtils.GetAmountBoxes(val.Value);
            }
            SetInitialObjectsSize(amountBoxes, _amountOfKeyboards);//Sets the size for all the objects used by the class and mostly SharpDX based on the user controls buttons and amount of controls
            int amountBoxesPerUserControl = 0;
            for (int indexBoxData = 0; indexBoxData < boxesData.Count; indexBoxData++)//populate the objects so they are ready to be used in the UI, is like a second layer of UI but manually fill instead of Windows Controls do the work
            {
                var boxData = boxesData.ElementAt(indexBoxData);
                amountBoxesPerUserControl = AnimationManagerUtils.GetAmountBoxes(boxData.Value);
                var temp_flashingSequenceIDList = new List<int[]>[amountBoxesPerUserControl];
                var temp_flashingSequenceList = new List<int[]>[amountBoxesPerUserControl];
                var temp_typeOfBox = new string[amountBoxesPerUserControl];
                var temp_rectanglesButtonsList = SharpDXUtils.GetRectanglesButtonsList(boxData, amountBoxesPerUserControl);
                var temp_rectanglesButtonsRoundList = SharpDXUtils.GetRectanglesButtonsRoundList(boxData, amountBoxesPerUserControl, _RadiusCornersButtons);
                var temp_ButtonDataList = AnimationManagerUtils.GetButtonDataList(boxData, amountBoxesPerUserControl, _sharpDX_d2dRenderTarget);
                var temp_ControlsBtns = AnimationManagerUtils.GetControlsBtns(boxData, amountBoxesPerUserControl, out temp_flashingSequenceIDList, out temp_flashingSequenceList);
                var temp_buttonTextFormatList = SharpDXUtils.GetListButtonTextFormat(boxData, widgetsData[indexBoxData], _directWriteFactory, amountBoxesPerUserControl, out temp_typeOfBox);
                var temp_widgets = AnimationManagerUtils.GetBoxWidgetsList(boxData, widgetsData[indexBoxData], amountBoxesPerUserControl);
                var temp_offsetStrings = AnimationManagerUtils.GetButtonsOffsetList(boxData, widgetsData[indexBoxData], amountBoxesPerUserControl);
                var temprectProbBars = SharpDXUtils.GetRecProbabilityBars(boxData.Key, boxData.Value, amountBoxesPerUserControl);
                var temprectProbBarsBox = SharpDXUtils.GetRecProbabilityBarsBox(boxData.Key, boxData.Value);
                for (int boxIndexUserControl = 0; boxIndexUserControl < amountBoxesPerUserControl; boxIndexUserControl++)
                {
                    _rectanglesButtonsList[indexBox] = temp_rectanglesButtonsList[boxIndexUserControl];
                    _rectanglesButtonsRoundList[indexBox] = temp_rectanglesButtonsRoundList[boxIndexUserControl];
                    _ButtonDataList[indexBox] = temp_ButtonDataList[boxIndexUserControl];
                    _ControlsBtns[indexBox] = temp_ControlsBtns[boxIndexUserControl];
                    _flashingSequenceIDList[indexBox] = temp_flashingSequenceIDList[boxIndexUserControl];
                    _flashingSequenceList[indexBox] = temp_flashingSequenceList[boxIndexUserControl];
                    _buttonTextFormatList[indexBox] = temp_buttonTextFormatList[boxIndexUserControl];
                    _typeOfBox[indexBox] = temp_typeOfBox[boxIndexUserControl];
                    _widgets[indexBox] = temp_widgets[boxIndexUserControl];
                    _offsetStrings[indexBox] = temp_offsetStrings[boxIndexUserControl];
                    _rectanglesProbBars[indexBox] = temprectProbBars[boxIndexUserControl];
                    try
                    {
                        for (int pbindex = 0; pbindex < temprectProbBarsBox.Count; pbindex++)
                        {
                            var pbBox = temprectProbBarsBox.ElementAt(pbindex);
                            _rectanglesProbBarsBox.Add(indexBox + 1 + pbindex, pbBox.Value);
                        }
                    }
                    catch (Exception)
                    {
                        //Log.Debug("BCI LOG | Exception _rectProbBarsBox: " + es.Message);
                    }
                    indexBox += 1;
                }
            }
            _flashingSequenceIDBoxList = AnimationManagerUtils.GetFlashingSequenceIDBoxList(_widgets, _amountOfKeyboards, out _flashingSequenceBoxList);
            _rectangleExtraButton = SharpDXUtils.GetRectanglesTriggerBox(boxesData);
            _rectangleExtraButtonCRG = SharpDXUtils.GetRectanglesCRG(boxesData, widgetsData, _directWriteFactory, out _offsetCRG, out _buttonTextFormatCRG);
            _BoxesData = boxesData;
            _numberOfSequences = _amountOfKeyboards;
            _currtButtonCount = 0;
            _numberOfSequences = _flashingSequenceBoxList.Length;
        }

        public void SetParametersTriggerTest(int interval, int repetitions)
        {
            _TriggerTestInterval = interval;//Save the parameters so when the timer of the trigger test starts it contains the values from the config form
            _TriggerTestRepetitions = repetitions;
        }

        /// <summary>
        /// Starts the Animations sequences
        /// </summary>
        /// <param name="actuator"></param>
        public void StartSequences(bool actuator = true)
        {
            if (!actuator)
                _actuator = null;
            _currEpochCount = 1;
            _currSeqCount = 0;
            CreateTimers();
            StartEpoch();
        }

        /// <summary>
        /// Request to start a trigger test
        /// </summary>
        public void TriggerTestRequest()
        {
            _sessionMode = BCIModes.TRIGGERTEST;
            _triggerTestActive = true;
            BCIMode bCIMode = new BCIMode { BciMode = BCIModes.TRIGGERTEST };
            var strBciMode = JsonConvert.SerializeObject(bCIMode);
            _actuator.IoctlRequest((int)OpCodes.StartSession, strBciMode);
            _actuator.IoctlRequest((int)OpCodes.TriggerTestStart, string.Empty);
        }

        /// <summary>
        /// Request to the actuator to start typing session
        /// </summary>
        public void TypingRequest()
        {
            _readStartSesionResult = true;
            _sessionMode = BCIModes.TYPING;
            _currEpochCount = 0;
            _isBoxScannig = true;
            _numberOfSequences = _amountOfKeyboards;
            BCIMode bCIMode = new BCIMode { BciMode = BCIModes.TYPING };
            var strBciMode = JsonConvert.SerializeObject(bCIMode);
            _actuator.IoctlRequest((int)OpCodes.StartSession, strBciMode);
        }

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        /// <summary>
        /// force to stop all timers
        /// </summary>
        private void AbortTimers()
        {
            _trialTimer?.Abort();
            _targetOffTimer?.Abort();
            _endEpochTimer?.Abort();
            _showDecisionTimer?.Abort();
            _pauseBetweenEpochsTimer?.Abort();
            _TriggerTest_Timer?.Abort();
        }

        /// <summary>
        /// Actuate the control given a ID or Tag seted in the control
        /// </summary>
        /// <param name="id">ID or Tag defined in the control</param>
        /// <returns>success action</returns>
        private async Task ActuateWidgetFromID(int id)
        {
            await Task.Delay(1);//Used another thead to ensure if a button that changes to another User control wont interfere to the change and slow the process and where this method was called continue normally 
            try
            {
                ScannerButtonControl btn = AnimationManagerUtils.GetControlFromID(id, _ControlsBtns, _activeKeyboard);
                foreach (Widget widget in _widgets[_activeKeyboard].Where(widget => Int32.Parse(widget.UIControl.Tag.ToString()) == id && widget.Name.Equals(btn.Name) && widget.Enabled))
                {
                    widget.Actuate();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error actuating Button: " + ex.Message);
            }
            await Task.Delay(1);
        }

        private void ActuatorRequestEndCalibration()
        {
            Log.Debug("BCI LOG | EndCalibration | Section " + _ScanningSection);
            CloseSequencesLog();
            RequestToUpdateTextBox(AnimationManagerUtils.StatusMessageAnalyzingCalibrationData + " . .");
            var bciCalibrationEnd = new BCICalibrationEnd();
            try
            {
                if (_ScanningSection == BCIScanSections.Box)
                {
                    for (int id = 0; id < _flashingSequenceIDBoxList.Length; id++)
                    {
                        List<int> seq = new List<int>() { id + 1 };
                        bciCalibrationEnd.FlashingSequence.Add(id + 1, seq.ToArray());
                    }
                }
                else
                {
                    for (int id = 0; id < _flashingSequenceIDList[_CalibrationBox].Count; id++)
                    {
                        bciCalibrationEnd.FlashingSequence.Add(id + 1, _flashingSequenceIDList[_CalibrationBox][id]);
                    }
                }
                bciCalibrationEnd.DiscardCalibrationData = false;
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception occurred during request to Actuator in ENDCAL: " + es.Message);
            }
            var strCalEnd = JsonConvert.SerializeObject(bciCalibrationEnd);
            _actuator?.IoctlRequest((int)OpCodes.CalibrationEnd, strCalEnd);
        }

        private void ActuatorRequestEndRepetition()
        {
            switch (_sessionMode)
            {
                case BCIModes.CALIBRATION:
                    ActuatorRequestEndRepetitionCalibration();
                    break;
                case BCIModes.TYPING:
                    if(!SuspendAnimations)
                        ActuatorRequestEndRepetitionTyping();
                    break;
            }
        }

        private void ActuatorRequestEndRepetitionCalibration()
        {
            var bciCalibrationInput = new BCICalibrationInput();
            try
            {
                if (_ScanningSection == BCIScanSections.Box)
                {
                    if (_firstSequence) //Needed only in first sequence
                    {
                        _firstSequence = false;
                        bciCalibrationInput.RowColumnIDs.Add(_currentCalibrationTarget.id + 1000); // add target+1000 followed by ids of rows and columns
                    }
                    for (int RowColID = 1; RowColID <= _flashingSequenceBoxList.Length; RowColID++)
                    {
                        bciCalibrationInput.RowColumnIDs.Add(RowColID);
                    }
                }
                else
                {
                    if (_firstSequence) //Needed only in first sequence
                    {
                        _firstSequence = false;
                        bciCalibrationInput.RowColumnIDs.Add(_currentCalibrationTarget.id + 1000); // add target+1000 followed by ids of rows and columns
                    }
                    for (int RowColID = 1; RowColID <= _flashingSequenceList[_CalibrationBox].Count; RowColID++)
                    {
                        bciCalibrationInput.RowColumnIDs.Add(RowColID);
                    }
                }
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception occurred during request to Actuator in CAl: " + es.Message);
            }
            var strCalRepEnd = JsonConvert.SerializeObject(bciCalibrationInput);
            _actuator?.IoctlRequest((int)OpCodes.CalibrationEndRepetition, strCalRepEnd);
        }

        private void ActuatorRequestEndRepetitionTyping()
        {
            var bciTypingRepetitionEnd = new BCITypingRepetitionEnd();
            try
            {
                if (!_isBoxScannig)
                {
                    for (int RowColID = 1; RowColID <= _flashingSequenceList[_activeKeyboard].Count; RowColID++)
                    {
                        bciTypingRepetitionEnd.RowColumnIDs.Add(RowColID);
                    }
                    for (int id = 0; id < _flashingSequenceIDList[_activeKeyboard].Count; id++)
                    {
                        bciTypingRepetitionEnd.FlashingSequence.Add(id + 1, _flashingSequenceIDList[_activeKeyboard][id]);
                    }
                    try
                    {
                        for (int index = 0; index < _ButtonDataList[_activeKeyboard].Count; index++)
                        {
                            bciTypingRepetitionEnd.ButtonTextValues.Add(_ButtonDataList[_activeKeyboard][index].id, _widgets[_activeKeyboard][index].Value);
                        }
                    }
                    catch (Exception es)
                    {
                        Log.Debug("BCI LOG | Exception occurred during request to Actuator in TYPE 3: " + es.Message);
                    }
                }
                else
                {
                    for (int RowColID = 1; RowColID <= _flashingSequenceBoxList.Length; RowColID++)
                    {
                        bciTypingRepetitionEnd.RowColumnIDs.Add(RowColID);
                    }
                    for (int id = 0; id < _flashingSequenceIDBoxList.Length; id++)
                    {
                        List<int> seq = new List<int>() { id + 1 };
                        bciTypingRepetitionEnd.FlashingSequence.Add(id + 1, seq.ToArray());
                    }
                    try
                    {
                        for (int index = 1; index <= _flashingSequenceBoxList.Length; index++)
                        {
                            bciTypingRepetitionEnd.ButtonTextValues.Add(index, "Box " + index.ToString());
                        }
                    }
                    catch (Exception es)
                    {
                        Log.Debug("BCI LOG | Exception occurred during request to Actuator in TYPE 1: " + es.Message);
                    }
                }
                bciTypingRepetitionEnd.ScanningSection = _ScanningSection;
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception occurred during request to Actuator in TYPE: " + es.Message);
            }
            var strTypRepEnd = JsonConvert.SerializeObject(bciTypingRepetitionEnd);
            _actuator?.IoctlRequest((int)OpCodes.TypingEndRepetition, strTypRepEnd);
        }

        private void ActuatorRequestMarker(string data)
        {
            _actuator?.IoctlRequest((int)OpCodes.HighlightOnOff, data);
        }

        private void ActuatorRequestTriggerTestFinish()
        {
            Log.Debug("BCI LOG | TriggerTestFinish ");
            _triggerTestActive = false;
            Thread.Sleep(200);
            _actuator?.IoctlRequest((int)OpCodes.TriggerTestStop, string.Empty);
        }

        private void ActuatorRequestValueProbs()
        {
            var bciLanguageModelProbabilities = new BCILanguageModelProbabilities();
            try
            {
                Dictionary<int, double> nextProbs = new Dictionary<int, double>();
                if (_typeOfBox[_activeKeyboard] != null)
                {
                    switch (_typeOfBox[_activeKeyboard].ToLower())
                    {
                        case "keyboard":
                            if (!_requestProbabilities)
                                nextProbs = AnimationManagerUtils.GetLettersProbs(_widgets[_activeKeyboard]);
                            else
                            {
                                nextProbs = AnimationManagerUtils.GetLettersProbs(_widgets[_activeKeyboard], true);
                                _requestProbabilities = false;
                            }
                            bciLanguageModelProbabilities.LanguageModelProbabilityType = ProbabilityType.NextCharacterProbabilities;
                            break;
                        case "menu":
                            bciLanguageModelProbabilities.LanguageModelProbabilityType = ProbabilityType.Other;
                            break;
                        case "sentences":
                            bciLanguageModelProbabilities.LanguageModelProbabilityType = ProbabilityType.NextSentenceProbabilities;
                            break;
                        case "words":
                            if (!_requestProbabilities)
                                nextProbs = AnimationManagerUtils.GetWordsProbs(_ControlsBtns[_activeKeyboard]);
                            else
                            {
                                nextProbs = AnimationManagerUtils.GetWordsProbs(_ControlsBtns[_activeKeyboard], true);
                                _requestProbabilities = false;
                            }
                            bciLanguageModelProbabilities.LanguageModelProbabilityType = ProbabilityType.NextWordProbabilities;
                            break;
                    }
                    if (nextProbs.Count != 0)
                    {
                        bciLanguageModelProbabilities.LanguageModelProbabilities = nextProbs;
                        var strNextProb = JsonConvert.SerializeObject(bciLanguageModelProbabilities);
                        _actuator?.IoctlRequest((int)OpCodes.LanguageModelProbabilities, strNextProb);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception in ActuatorRequestValueProbs: " + ex.Message);
            }
        }
        private void ActuatorResponseCalibrationEndRepetitionResult(String response)
        {
            try
            {
                var bciCalibrationEndRepResult = JsonConvert.DeserializeObject<BCISensorStatus>(response);
                SensorErrorState = bciCalibrationEndRepResult.Error;
                AnimationManagerUtils.StatusSignal = (SignalStatus)bciCalibrationEndRepResult.StatusSignal;
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception in ActuatorResponseCalibrationEndRepetitionResult: " + ex.Message);
                SensorErrorState = new BCIError(BCIErrorCodes.OpticalSensorError_UnknownException, BCIMessages.SensorError);
                AnimationManagerUtils.StatusSignal = SignalStatus.SIGNAL_KO;
            }
        }

        private void ActuatorResponseCalibrationResult(String response)
        {
            Log.Debug("BCI LOG | CalibrationResult | AUC " + _AUC + " | Section " + _ScanningSection);
            var bciCalibrationResult = JsonConvert.DeserializeObject<BCICalibrationResult>(response);
            _AUC = bciCalibrationResult.AUC;
            if (bciCalibrationResult.CalibrationSuccessful)//Flags to let know the timer thread once is at the final process to do either one or the other event trigger
                _endCalibration = true;
            else
                _repeatCalibration = true;
            SensorErrorState = bciCalibrationResult.Error;
        }

        private void ActuatorResponseSendParameters(String response)
        {
            try
            {
                Log.Debug("BCI LOG | SendParameters | Section " + _ScanningSection);
                var bciParameters = JsonConvert.DeserializeObject<BCIParameters>(response);
                _DelayToGetReady = bciParameters.Scanning_DelayToGetReady;
                _MinimumProgressBarsValue = (bciParameters.MinProbablityToDisplayBarOnTyping);
                _CalibrationTargetCount = bciParameters.CalibrationParameters[_ScanningSection].TargetCount;
                _CalibrationIterationsPerTarget = bciParameters.CalibrationParameters[_ScanningSection].IterationsPerTarget;
                SensorErrorState = bciParameters.Error;
                _delayDisplayDecision = bciParameters.Scanning_DelayAfterDecision;
                _useRandomSelectionTargetCalibration = bciParameters.CalibrationParameters[_ScanningSection].UseRandomTargetsFlag;
                _CalibrationStringtarget = bciParameters.CalibrationParameters[_ScanningSection].Sequence;
                _trialITI = bciParameters.CalibrationParameters[_ScanningSection].ScanTime;
                _pauseTime = bciParameters.Scanning_PauseTime;
                _shortPauseTime = bciParameters.Scanning_ShortPauseTime;
                _showDecisionTime = bciParameters.Scanning_ShowDecisionTime;
                _IsFocalCircleFilled = bciParameters.Scanning_IsFocalCircleFilled;
                _FocalCircleColor = bciParameters.Scanning_FocalCircleColor;
                if (_ChangeTimings && !_changeUserControl)//This is a trigger when changing to different keyboards that have different scannig speeds, for example going from Box to Keyboard Right and have different scanning time it starts a timer with the correct speed
                {
                    _trialTimer.Stop();
                    Thread.Sleep(20);
                    _trialTimer.Abort();
                    Thread.Sleep(100);
                    if (_trialTimer.Enabled)
                    {
                        _trialTimer.Stop();
                        Thread.Sleep(20);
                        _trialTimer.Abort();
                        Thread.Sleep(100);
                    }
                    _ChangeTimings = false;
                    UnsubscribeTimerEvent(ref _trialTimer, TrialTimer_Tick);
                    _trialTimer = CreateTimer(_trialITI * 1000 / 4, TrialTimer_Tick);
                    if (!_changeUserControl)
                        StartEpoch();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | ActuatorResponseSendParameters | Exception | " + ex.Message);
                _CalibrationTargetCount = 60;
                _CalibrationIterationsPerTarget = 2;
            }
        }

        private void ActuatorResponseStartSessionResult(String response)
        {
            Log.Debug("BCI LOG | StartSessionResult | Section " + _ScanningSection);
            var bciSessionResult = JsonConvert.DeserializeObject<BCIStartSessionResult>(response);
            SensorErrorState = bciSessionResult.Error;
            CreateSequencesLog(bciSessionResult.SessionDirectory);
            Log.Debug("BCI LOG | Scanning Log created in: | Path " + bciSessionResult.SessionDirectory);
            Log.Debug("BCI LOG | bciSessionResult.Error | Status " + bciSessionResult.Error);
            //_readStartSesionResult flag is for Eyes/Clsoed Calibration form that also has the start session request with this we avoid the event handler to run scanning when Eyes Form is active and here has the event subscribed active
            if (_readStartSesionResult && !_triggerTestActive)
            {
                _StartSequences = true;
                if (bciSessionResult.SensorReady && (_endCalibration || _repeatCalibration))
                {
                    _repeatCalibration = false;
                    _endCalibration = false;
                    Reset();
                }
                else if (bciSessionResult.SensorReady)
                    StartSequences();
            }
        }

        private void ActuatorResponseTriggerTestResult(String response)
        {
            var bciTriggerTestResult = JsonConvert.DeserializeObject<BCITriggerTestResult>(response);
            CloseSequencesLog();
            Log.Debug("BCI LOG | TriggerTestResult | DutyCycleAvg " + bciTriggerTestResult.DutyCycleAvg.ToString());
            //Trigger event to call a UI message box from a higher level
            _mainForm.Invoke(new MethodInvoker(delegate
            {
                EvtBCICalibrationComplete?.Invoke("Average Duty Cycle: " + bciTriggerTestResult.DutyCycleAvg.ToString());
            }));
        }

        private void ActuatorResponseTriggerTestStartReady()
        {
            _TriggerTest_Timer = CreateTimer(_TriggerTestInterval * 1000 / 4, TriggerTestTimer_Tick);
            _TriggerTest_Timer.Start();
        }

        private void ActuatorResponseTypingEndRepetitionResult(String response)
        {
            var bciTypingResult = JsonConvert.DeserializeObject<BCITypingRepetitionResult>(response);
            _isDecisionMade = bciTypingResult.DecidedFlag;
            _progressBarsProbs = bciTypingResult.PosteriorProbs;
            SensorErrorState = bciTypingResult.Error;
            _ReturnToBoxScanning = bciTypingResult.ReturnToBoxScanningFlag;
            AnimationManagerUtils.StatusSignal = (SignalStatus)bciTypingResult.StatusSignal;
            if (bciTypingResult.DecidedFlag)
                _decidedID = bciTypingResult.DecidedId;
        }

        /// <summary>
        /// Change colors of the buttons and boxes in the UI
        /// </summary>
        /// <param name="buttonID">ID of the control</param>
        /// <param name="highlightOn">Highlight the decision</param>
        /// <param name="useExtraBox">Change color of trigger box</param>
        /// <param name="type">Status type of the decision</param>
        /// <param name="turnOnNow">Highlight the trigger box</param>
        private void ChangeColorButtons(int buttonID, bool highlightOn, bool useExtraBox, int type, bool turnOnNow = false)
        {
            List<int> l = new List<int> { buttonID };
            if (!_stopAnimation && !_changeUserControl)
                ChangeColorButtons(l, highlightOn, useExtraBox, type, turnOnNow);
        }

        /// <summary>
        /// Change colors of the buttons and boxes in the UI
        /// </summary>
        /// <param name="buttonIDs">List of ID of the buttons</param>
        /// <param name="highlightOn">Highlight the decision</param>
        /// <param name="useExtraBox">Change color of trigger box</param>
        /// <param name="type">Status type of the decision</param>
        /// <param name="turnOnNow">Highlight the trigger box</param>
        private void ChangeColorButtons(List<int> buttonIDs, bool highlightOn, bool useExtraBox, int type, bool turnOnNow = false)
        {
            try
            {
                if (!_changeUserControl)
                {
                    if (_sessionMode == BCIModes.CALIBRATION)
                    {
                        if (_ScanningSection == BCIScanSections.Box)
                            _numberOfSequences = _flashingSequenceBoxList.Length;
                        else
                        {
                            _activeKeyboard = _CalibrationBox;
                            _numberOfSequences = _flashingSequenceList[_CalibrationBox].Count();
                        }
                    }
                    SolidColorBrush colorRectExtraBox, colorBorder, colorRect, colorBorderExtraBox;
                    if (highlightOn)
                    {
                        colorBorderExtraBox = SharpDXColors.SolidColorBrushExtraBoxOn;
                        colorRectExtraBox = SharpDXColors.SolidColorWhite;
                        colorBorder = SharpDXColors.SolidColorBrushOn;
                        colorRect = SharpDXColors.SolidColorBrushOn;
                        if (type == 0) // trial
                        {
                            colorBorder = SharpDXColors.SolidColorBrushOn; //solidColorBrushOff
                            colorRect = SharpDXColors.SolidColorBrushOn;
                        }
                        if (type == 1)
                        {
                            colorBorder = SharpDXColors.SolidColorBrushOn; //solidColorBrushOff
                            colorRect = SharpDXColors.SolidColorBrushTarget;
                        }
                        if (type == 2) // decision
                        {
                            colorBorder = SharpDXColors.SolidColorBrushOn; //solidColorBrushOff
                            colorRect = SharpDXColors.SolidColorBrushDecision;
                        }
                        if (type == 3) //correct decision
                        {
                            colorBorder = SharpDXColors.SolidColorBrushOn;
                            colorRect = SharpDXColors.SolidColorBrushDecisionCorrect;
                        }
                        if (type == 4) // incorrect decision
                        {
                            colorBorder = SharpDXColors.SolidColorBrushOn;
                            colorRect = SharpDXColors.SolidColorBrushDecisionIncorrect;
                        }
                        if (type == 5) //correct decision
                        {
                            colorBorder = SharpDXColors.SolidColorBrushOn;
                            colorRect = SharpDXColors.SolidColorBrushDecisionActuate;
                        }
                        if (type == 6) // highlight edges off
                        {
                            colorBorder = SharpDXColors.SolidColorBrushDisabledButton;
                            colorRect = SharpDXColors.SolidColorBrushOff;
                            colorBorderExtraBox = SharpDXColors.SolidColorBrushExtraBoxOff;
                            colorRectExtraBox = SharpDXColors.SolidColorBrushExtraBoxOff;
                        }
                    }
                    else
                    {
                        colorBorder = SharpDXColors.SolidColorBrushOn;
                        colorRect = SharpDXColors.SolidColorBrushOff;
                        colorBorderExtraBox = SharpDXColors.SolidColorBrushExtraBoxOff;
                        colorRectExtraBox = SharpDXColors.SolidColorBrushExtraBoxOff;
                    }
                    SharpDX.DirectWrite.Factory directWriteFactory = new SharpDX.DirectWrite.Factory();
                    if (!_stopAnimation && !_changeUserControl)
                    {
                        _sharpDX_d2dRenderTarget.BeginDraw();
                        _sharpDX_d2dRenderTarget.Clear(SharpDXColors.ColorBackground);
                    }
                    if (_sessionMode == BCIModes.TYPING && !_stopAnimation && !_changeUserControl)
                        SetValueProbabilityBars();
                    DrawMatrix();//draw the base UI, all elements in the UI all buttons and text
                    if (_sessionMode != BCIModes.TRIGGERTEST)
                    {
                        ChangeColorButtonsSequence(buttonIDs, colorRect, highlightOn, _activeKeyboard);
                        if (_sessionMode == BCIModes.CALIBRATION)
                        {
                            if (!_isBoxScannig)//Draw the elements of a keyboard
                                _sharpDX_d2dRenderTarget.DrawRoundedRectangle(_rectanglesButtonsRoundList[_activeKeyboard][FindIndexFromID(_currentCalibrationTarget.id)], SharpDXColors.SolidColorBrushTarget, _BorderWidth);
                            else
                            {//To draw the buttons that being used as target in Box calibration
                                if (highlightOn)
                                {
                                    if ((_currentCalibrationTarget.id - 1) != _activeKeyboard)
                                        ChangeColorButtonsSequence(_flashingSequenceBoxList[(_currentCalibrationTarget.id - 1)].ToList(), SharpDXColors.SolidColorBrushOff, false, (_currentCalibrationTarget.id - 1), true);
                                }
                                else
                                    ChangeColorButtonsSequence(_flashingSequenceBoxList[(_currentCalibrationTarget.id - 1)].ToList(), SharpDXColors.SolidColorBrushOff, highlightOn, (_currentCalibrationTarget.id - 1), true);
                                DrawFocalPointsFor(BCIModes.CALIBRATION);
                            }
                        }
                        else if (_sessionMode == BCIModes.TYPING && _isBoxScannig && !_lockAnimation)
                        {
                            DrawFocalPointsFor(BCIModes.TYPING);
                        }
                    }
                    if (useExtraBox) //(for optical sensor)
                    {
                        if (!_stopAnimation && !_changeUserControl)
                        {
                            _sharpDX_d2dRenderTarget.DrawRectangle(_rectangleExtraButton, (_triggerTestActive ? SharpDXColors.SolidColorBrushExtraBoxOff : colorBorderExtraBox));
                            _sharpDX_d2dRenderTarget.FillRectangle(_rectangleExtraButton, (_triggerTestActive ? SharpDXColors.SolidColorBrushExtraBoxOff : colorRectExtraBox));
                            if (turnOnNow)
                            {
                                _sharpDX_d2dRenderTarget.DrawRectangle(_rectangleExtraButton, (_triggerTestActive ? SharpDXColors.SolidColorWhite : SharpDXColors.SolidColorBrushExtraBoxOff));
                                _sharpDX_d2dRenderTarget.FillRectangle(_rectangleExtraButton, (_triggerTestActive ? SharpDXColors.SolidColorWhite : SharpDXColors.SolidColorBrushExtraBoxOff));
                            }
                        }
                    }
                    if (!_stopAnimation && !_changeUserControl)
                    {
                        _sharpDX_d2dRenderTarget.EndDraw();
                        _sharpDX_swapChain.Present(1, PresentFlags.None);
                    }
                }
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception sharpDX ChangeColorButtons: " + es.Message);
            }

        }

        /// <summary>
        /// Change the colors of the sequence row/column or box
        /// </summary>
        /// <param name="buttonIDs">List of controls to change colors</param>
        /// <param name="colorRect">Color of the rectangle</param>
        /// <param name="highlightOn">is highlighted target/sequence</param>
        /// <param name="activekeyboard">current box being highlighted</param>
        /// <param name="maintainColortarget">Keep the target color in the sequence</param>
        private void ChangeColorButtonsSequence(List<int> buttonIDs, SolidColorBrush colorRect, bool highlightOn, int activekeyboard, bool maintainColortarget = false)
        {
            try
            {
                foreach (int buttonID in buttonIDs)
                {
                    SolidColorBrush solidColorBrush;
                    if (maintainColortarget)
                        solidColorBrush = SharpDXColors.SolidColorBrushTarget;
                    else
                    {
                        if (_widgets[activekeyboard][buttonID].Enabled)
                            solidColorBrush = _ButtonDataList[activekeyboard][buttonID].borderColor;
                        else
                            solidColorBrush = SharpDXColors.SolidColorBrushDisabledButton;
                    }
                    if (!_stopAnimation && !_changeUserControl)
                    {
                        _sharpDX_d2dRenderTarget.FillRoundedRectangle(_rectanglesButtonsRoundList[activekeyboard][buttonID], colorRect);
                        _sharpDX_d2dRenderTarget.DrawRoundedRectangle(_rectanglesButtonsRoundList[activekeyboard][buttonID], (highlightOn ? colorRect : solidColorBrush), _BorderWidth);
                    }
                    SharpDX.Mathematics.Interop.RawRectangleF rect = new SharpDX.Mathematics.Interop.RawRectangleF(_rectanglesButtonsList[activekeyboard][buttonID].Left + _PaddingText, _rectanglesButtonsList[activekeyboard][buttonID].Top + _offsetStrings[activekeyboard][buttonID], _rectanglesButtonsList[activekeyboard][buttonID].Right - _PaddingText, _rectanglesButtonsList[activekeyboard][buttonID].Bottom - _PaddingTextBottom);
                    if (!_stopAnimation && !_changeUserControl)
                        _sharpDX_d2dRenderTarget.DrawText(_buttonsStringsList[activekeyboard][buttonID], _buttonTextFormatList[activekeyboard][buttonID], rect, highlightOn ? SharpDXColors.SolidColorBrushButtonTextOn : solidColorBrush, _drawTextOptions);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception sharpDX ChangeColorButtonsSequence: " + ex.Message);
            }

        }

        /// <summary>
        /// Create SharpDX object
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        private SwapChainDescription CreateSwapChainDesc(Form frm)
        {
            return new SwapChainDescription()
            {
                BufferCount = 3,                                 //how many buffers are used for writing. it's recommended to have at least 2 buffers but this is an example
                Flags = SwapChainFlags.None,
                IsWindowed = true,                               //it's windowed
                ModeDescription = new ModeDescription(
              frm.ClientSize.Width,                       //windows veiwable width
              frm.ClientSize.Height,                      //windows veiwable height
              new Rational(60, 1),                          //refresh rate
              Format.R8G8B8A8_UNorm),                      //pixel format, need to research this
                OutputHandle = frm.Handle,                      //the magic 
                SampleDescription = new SampleDescription(1, 0), //the first number is how many samples to take, anything above one is multisampling.
                SwapEffect = SwapEffect.FlipSequential, //Discard
                Usage = Usage.RenderTargetOutput
            };
        }

        /// <summary>
        /// Get the initialized Timer object
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        private MicroTimer CreateTimer(int interval, MicroTimer.MicroTimerElapsedEventHandler eventHandler)
        {
            var timer = new MicroTimer();
            timer.MicroTimerElapsed += eventHandler;
            timer.Interval = interval;
            return timer;
        }

        /// <summary>
        /// Sets the timers for different task to play the sequences or stop them
        /// </summary>
        private void CreateTimers()
        {
            _trialTimer = CreateTimer(_trialITI * 1000 / 4, TrialTimer_Tick);
            _targetOffTimer = CreateTimer(_pauseTime * 1000, TargetOffTimer_Tick);
            _endEpochTimer = CreateTimer(_pauseTime * 1000, EndEpochTimer_Tick);
            _showDecisionTimer = CreateTimer(_showDecisionTime * 1000, ShowDecisionTimer_Tick);
            _pauseBetweenEpochsTimer = CreateTimer(_shortPauseTime * 1000, PauseBetweenEpochs_Tick);
        }

        /// <summary>
        /// Dispose objects used during session
        /// </summary>
        private void DisposeObjects()
        {
            _sharpDX_device.Dispose();
            _sharpDX_swapChain.Dispose();
            _sharpDX_d2dFactory.Dispose();
            _sharpDX_d2dRenderTarget.Dispose();
            _finishTask = true;
            _endEpochTimer = null;
            _trialTimer = null;
            _targetOffTimer = null;
            _showDecisionTimer = null;
            _pauseBetweenEpochsTimer = null;
            _TriggerTest_Timer = null;
        }

        /// <summary>
        /// Draw focal points when is box scanning
        /// </summary>
        /// <param name="bCIModes"></param>
        private void DrawFocalPointsFor(BCIModes bCIModes)
        {
            try
            {
                switch (bCIModes)
                {
                    case BCIModes.CALIBRATION:
                        var roundRectc = SharpDXUtils.GetFocalAreaForBox(_rectanglesButtonsRoundList[_currentCalibrationTarget.id - 1], 100, 100, 50, 50, BCIFocalAreaRegion.Center);
                        _sharpDX_d2dRenderTarget.DrawRoundedRectangle(roundRectc, _FocalCircleColor.ToLower().Equals("yellow") ? SharpDXColors.SolidColorBrushOn : SharpDXColors.SolidColorBrushDecisionCorrect, 8);
                        if (_IsFocalCircleFilled)
                            _sharpDX_d2dRenderTarget.FillRoundedRectangle(roundRectc, _FocalCircleColor.ToLower().Equals("yellow") ? SharpDXColors.SolidColorBrushOn : SharpDXColors.SolidColorBrushDecisionCorrect);
                        break;
                    case BCIModes.TYPING:
                        foreach (var rects in _rectanglesButtonsRoundList)
                        {
                            var roundRectt = SharpDXUtils.GetFocalAreaForBox(rects, 100, 100, 50, 50, BCIFocalAreaRegion.Center);
                            _sharpDX_d2dRenderTarget.DrawRoundedRectangle(roundRectt, _FocalCircleColor.ToLower().Equals("yellow") ? SharpDXColors.SolidColorBrushOn : SharpDXColors.SolidColorBrushDecisionCorrect, 8);
                            if (_IsFocalCircleFilled)
                                _sharpDX_d2dRenderTarget.FillRoundedRectangle(roundRectt, _FocalCircleColor.ToLower().Equals("yellow") ? SharpDXColors.SolidColorBrushOn : SharpDXColors.SolidColorBrushDecisionCorrect);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception sharpDX DrawFocalPointsFor: " + ex.Message);
            }
        }

        /// <summary>
        /// Draw Borders of rectangles
        /// </summary>
        private void DrawMatrix()
        {
            try
            {
                bool drawArea;
                for (int ii = 0; ii < _amountOfKeyboards; ii++)
                {
                    drawArea = true;
                    int i = 0;
                    foreach (SharpDX.Mathematics.Interop.RawRectangleF rect in _rectanglesButtonsList[ii])
                    {
                        if (_sessionMode == BCIModes.CALIBRATION && ii != _CalibrationBox && _ScanningSection != BCIScanSections.Box && !_triggerTestActive)//When in calibration if is needed to draw the other boxes Ex: if is sentence cal. then dont draw words and keyboard L and R
                        {
                            drawArea = false;
                        }
                        RoundedRectangle roundedRectangle = SharpDXUtils.GetRoundedRectangle(rect, _RadiusCornersButtons);
                        //to draw the buttons as a disable option (grey kind of color)
                        if (_widgets[ii][i].Enabled)
                        {
                            if (!_stopAnimation && drawArea && !_changeUserControl)
                                _sharpDX_d2dRenderTarget.DrawRoundedRectangle(roundedRectangle, _ButtonDataList[ii][i].borderColor, _BorderWidth);
                            SharpDX.Mathematics.Interop.RawRectangleF rect2 = new SharpDX.Mathematics.Interop.RawRectangleF(rect.Left + _PaddingText, rect.Top + _offsetStrings[ii][i], rect.Right - _PaddingText, rect.Bottom - _PaddingTextBottom);
                            if (!_stopAnimation && drawArea && !_changeUserControl)
                                _sharpDX_d2dRenderTarget.DrawText(_buttonsStringsList[ii][i], _buttonTextFormatList[ii][i], rect2, SharpDXColors.SolidColorBrushButtonTextOff, _drawTextOptions);// highli
                        }
                        else
                        {
                            if (!_stopAnimation && drawArea && !_changeUserControl)
                                _sharpDX_d2dRenderTarget.DrawRoundedRectangle(roundedRectangle, SharpDXColors.SolidColorBrushDisabledButton, _BorderWidth);
                            SharpDX.Mathematics.Interop.RawRectangleF rect2 = new SharpDX.Mathematics.Interop.RawRectangleF(rect.Left + _PaddingText, rect.Top + _offsetStrings[ii][i], rect.Right - _PaddingText, rect.Bottom - _PaddingTextBottom);
                            if (!_stopAnimation && drawArea && !_changeUserControl)
                                _sharpDX_d2dRenderTarget.DrawText(_buttonsStringsList[ii][i], _buttonTextFormatList[ii][i], rect2, SharpDXColors.SolidColorBrushDisabledButton, _drawTextOptions);// highli
                        }
                        i++;
                    }
                    //For the CRG Section, here in the future for any upcoming chabe can be added here either the shape, text or color
                    if (_rectangleExtraButtonCRG.Bottom != 0 && _rectangleExtraButtonCRG.Top != 0 && _rectangleExtraButtonCRG.Left != 0 && _rectangleExtraButtonCRG.Right != 0 && _offsetCRG != 0 && _buttonTextFormatCRG != null && CRGText != null)
                    {
                        if (!_stopAnimation && !_changeUserControl)
                            _sharpDX_d2dRenderTarget.DrawRectangle(_rectangleExtraButtonCRG, SharpDXColors.SolidColorBrushExtraBoxOn);
                        SharpDX.Mathematics.Interop.RawRectangleF rectCRG = new SharpDX.Mathematics.Interop.RawRectangleF(_rectangleExtraButtonCRG.Left, _rectangleExtraButtonCRG.Top + _offsetCRG, _rectangleExtraButtonCRG.Right, _rectangleExtraButtonCRG.Bottom);
                        if (!_stopAnimation && !_changeUserControl)
                            _sharpDX_d2dRenderTarget.DrawText(CRGText, _buttonTextFormatCRG, rectCRG, SharpDXColors.SolidColorBrushButtonTextOff);
                    }
                    //This is for the trigger box
                    if (!_stopAnimation && !_changeUserControl)
                    {
                        _sharpDX_d2dRenderTarget.DrawRectangle(_rectangleExtraButton, SharpDXColors.SolidColorBrushExtraBoxOn);
                        _sharpDX_d2dRenderTarget.FillRectangle(_rectangleExtraButton, SharpDXColors.SolidColorBrushExtraBoxOff);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception sharpDX in DrawMatrix: " + ex.Message);
            }

        }

        /// <summary>
        /// Epoch timer handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndEpochTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            _endEpochTimer.Stop();
            if (!_changeUserControl)
                _pauseBetweenEpochsTimer.Start();
        }

        /// <summary>
        /// Ensures that the timers are stoped and not running
        /// </summary>
        private void EnsureTimersAreStoped()
        {
            try
            {
                //this measure is to avoid a timer still running and we are able to make changes to the objects used by SharpDX, there were times where the stope and/abort the timer remain active 
                // and makeing a change fall into an exception 
                StopTimers();
                Thread.Sleep(20);
                AbortTimers();
                Thread.Sleep(100);
                if (!VerifyIfTimersAreStoped())
                {
                    StopTimers();
                    Thread.Sleep(20);
                    AbortTimers();
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception  in EnsureTimersAreStoped: " + ex.Message);
            }
        }

        /// <summary>
        /// Event handler for the answer from the actuator
        /// </summary>
        /// <param name="opcode">Operation code result</param>
        /// <param name="response">Response data</param>
        private void Evt_ResponseActuator(int opcode, String response)
        {
            _actuatorAnswer = true;
            switch (opcode)
            {
                case (int)OpCodes.CalibrationResult:
                    ActuatorResponseCalibrationResult(response);
                    break;
                case (int)OpCodes.CalibrationEndRepetitionResult:
                    ActuatorResponseCalibrationEndRepetitionResult(response);
                    break;
                case (int)OpCodes.TypingEndRepetitionResult:
                    ActuatorResponseTypingEndRepetitionResult(response);
                    break;
                case (int)OpCodes.SendParameters:
                    ActuatorResponseSendParameters(response);
                    break;
                case (int)OpCodes.StartSessionResult:
                    ActuatorResponseStartSessionResult(response);
                    break;
                case (int)OpCodes.CalibrationWindowClose:
                    CloseSequencesLog();
                    break;
                case (int)OpCodes.TriggerTestStartReady:
                    ActuatorResponseTriggerTestStartReady();
                    break;
                case (int)OpCodes.TriggerTestResult:
                    ActuatorResponseTriggerTestResult(response);
                    break;
                default:
                    break;
            }
            ValidateSensorErrorState(SensorErrorState);
        }

        /// <summary>
        /// get the control given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ButtonsData FindButtomFromId(int id)
        {
            try
            {
                var button = _ButtonDataList[_activeKeyboard].FirstOrDefault(m => m.id == id);
                return button;
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception occurred findButtomFromId: " + es.Message);
                return new ButtonsData();
            }
        }

        /// <summary>
        /// Get the index from the controls from active keyboard given an ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int FindIndexFromID(int id)
        {
            int indexMode;
            if (!_changeUserControl)
            {
                try
                {
                    if (_sessionMode == BCIModes.CALIBRATION)
                        indexMode = _CalibrationBox;
                    else
                        indexMode = _activeKeyboard;
                    for (int ii = 0; ii < _ButtonDataList[indexMode].Count; ii++)
                    {
                        if (_ButtonDataList[indexMode][ii].id == id)
                            return ii;
                    }
                }
                catch (Exception es)
                {
                    Log.Debug("BCI LOG | Exception findIndexFromID: " + es.Message);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get the control given an Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ButtonsData GetButtomFromIndex(int index)
        {
            try
            {
                if (_sessionMode == BCIModes.TYPING)
                    return _ButtonDataList[_activeKeyboard][index];
                else if (_sessionMode == BCIModes.CALIBRATION)
                    return _ButtonDataList[_CalibrationBox][index];
            }
            catch (Exception)
            {
                return new ButtonsData();
            }
            return new ButtonsData();
        }

        /// <summary>
        /// Get the string from the calibration target process
        /// </summary>
        /// <returns></returns>
        private string GetCalibrationInProgressText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Clear();
            stringBuilder.Append(BCIR.GetString("CalibrationInProgrressPrompt"));
            stringBuilder.AppendLine();
            stringBuilder.Append(BCIR.GetString("IterationNumber") + " " + (_currEpochCount - 1) + " " + BCIR.GetString("Of") + " " + _CalibrationTargetCount);
            stringBuilder.AppendLine();
            stringBuilder.Append(BCIR.GetString("ScanningTime") + " " + _trialITI + " ms");
            return stringBuilder.ToString();
        }
        private bool InitSwapChain(Form form)
        {
            SwapChainDescription swpChn = CreateSwapChainDesc(form);
            //create device with swap chain handle set to the form
            SharpDX.Direct3D11.Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware,//hardware if you have a graphics card otherwise you can use software
                        DeviceCreationFlags.BgraSupport,
                        swpChn,                                 //the swapchain description made above
                                out _sharpDX_device, out _sharpDX_swapChain                        //our directx objects
                                );
            //create the 2D device 
            _sharpDX_d2dFactory = new SharpDX.Direct2D1.Factory();
            SharpDX.DXGI.Factory factory = _sharpDX_swapChain.GetParent<SharpDX.DXGI.Factory>();

            // New RenderTargetView from the backbuffer
            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(_sharpDX_swapChain, 0);
            Surface surface = backBuffer.QueryInterface<Surface>();
            _sharpDX_d2dRenderTarget = new RenderTarget(_sharpDX_d2dFactory, surface,
                                                                    new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            return true;
        }

        /// <summary>
        /// Add a new entry to the log used by BCI
        /// </summary>
        private void NewEntryLog(bool highlight, bool triggerbox, List<int> idButtons, int idRowColumn, bool targetdecision = false)
        {
            _cachedLog?.LogEntry("Highlight", BCIUtils.GetEntryLogStr(_sessionMode, _ScanningSection, highlight, triggerbox, idButtons, idRowColumn, targetdecision));
        }

        /// <summary>
        /// Timer handler for pause between epoch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseBetweenEpochs_Tick(object sender, MicroTimerEventArgs e)
        {
            if (_sessionMode == BCIModes.TYPING)
            {
                _ChangeTimings = true;
                StopTimers();
                RequestParameters(new BCIMode { BciMode = _sessionMode, BciCalibrationMode = _ScanningSection });
            }
            else
            {
                if (!_changeUserControl)
                    StartEpoch();
                _pauseBetweenEpochsTimer.Stop();
            }
        }

        /// <summary>
        /// Check is box has empty elements
        /// </summary>
        /// <returns>is box empty</returns>
        private bool PlayBoxSequences()
        {
            try
            {
                for (int index = 0; index < _widgets[_activeKeyboard].Count; index++)
                {
                    if (_widgets[_activeKeyboard][index].Value.Length > 0)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error in isBoxEmpty: " + ex.Message);
                return true;
            }
        }

        /// <summary>
        /// Evaluates the data at the end of the Sequence
        /// </summary>
        private void ProcessEndSequence()
        {
            switch (_sessionMode)
            {
                case BCIModes.CALIBRATION:
                    if (_currSeqCount < _CalibrationIterationsPerTarget) // Sequences in epoch
                    {
                        if (!_changeUserControl)
                            StartSequence();
                    }
                    else // All sequences in epoch finalized
                    {
                        if (_currEpochCount <= _CalibrationTargetCount)
                        {
                            _currSeqCount = 0;
                            _sequenceDone = false;
                            if (!_changeUserControl)
                                _endEpochTimer.Start();
                        }
                        else
                            ActuatorRequestEndCalibration();
                    }
                    break;
                case BCIModes.TYPING:
                    if (_actuator == null)
                    {
                        _currSeqCount = 1;
                        if (_currSeqCount < _TypingIterationsPerTarget) // No decision made, start new sequence
                        {
                            if (!_changeUserControl)
                                StartSequence();
                        }
                        else
                        {
                            int decisionIdx = 3;
                            ButtonsData decision = FindButtomFromId(decisionIdx);
                            Thread.Sleep(500);
                            ShowDecision(decision, false);
                            UpdateCopiedText();
                        }
                    }
                    else
                    {
                        if (_actuator != null)
                        {
                            while (true)//Infinite loop since is necessary to wait for the actuator answer
                            {
                                if (_actuatorAnswer)//The actuator answer
                                {
                                    _actuatorAnswer = false;//Restore the flag for the next iteration/decision made
                                    if (_isDecisionMade)
                                    {
                                        _isDecisionMade = false;//Restore the flag
                                        if (!_changeUserControl)//Flag of precautionary measure it a user control is triggered
                                        {
                                            if (!_isBoxScannig)//Do something if is box scanning or button scanning
                                            {
                                                ButtonsData decision = FindButtomFromId(_decidedID);
                                                UpdateCopiedText();
                                                ShowDecision(decision, true);//This method is the one that shows the decision made and start the next sequences 

                                                if (!_ReturnToBoxScanning && !_changeUserControl)
                                                    _ = ActuateWidgetFromID(_decidedID).ConfigureAwait(false);
                                                if (!_lockAnimation)//If not lock then continue with box scanning otherwise it will continue with keyboard scanning
                                                {
                                                    _numberOfSequences = _flashingSequenceBoxList.Length;
                                                    _isBoxScannig = true;
                                                    if (_sessionMode == BCIModes.TYPING)
                                                        _ScanningSection = BCIScanSections.Box;
                                                    _ReturnToBoxScanning = false;
                                                }
                                            }
                                            else
                                            {
                                                ShowDecisionBox(_decidedID, true);
                                                _numberOfSequences = _flashingSequenceList[_decidedID - 1].Count;
                                                if (_ReturnToBoxScanning || !PlayBoxSequences())//if a box was selected return to box scanning anyway
                                                {
                                                    _numberOfSequences = _flashingSequenceBoxList.Length;
                                                    _isBoxScannig = true;
                                                    _ReturnToBoxScanning = false;
                                                }
                                                else
                                                {
                                                    _isBoxScannig = false;
                                                    if (_sessionMode == BCIModes.TYPING)
                                                    {
                                                        switch (_decidedID)
                                                        {
                                                            case 1:
                                                                _ScanningSection = BCIScanSections.Word;
                                                                break;
                                                            case 2:
                                                                _ScanningSection = BCIScanSections.Sentence;
                                                                break;
                                                            case 3:
                                                                _ScanningSection = BCIScanSections.KeyboardL;
                                                                break;
                                                            case 4:
                                                                _ScanningSection = BCIScanSections.KeyboardR;
                                                                break;
                                                        }
                                                    }

                                                }

                                            }
                                        }
                                        _progressBarsProbs = null;
                                    }
                                    else//No decision made star over the iterations
                                    {
                                        if (_ReturnToBoxScanning)//If the actuator return to go back to box scanning
                                        {
                                            _numberOfSequences = _flashingSequenceBoxList.Length;
                                            _isBoxScannig = true;
                                            _ReturnToBoxScanning = false;
                                        }
                                        if (!_changeUserControl)
                                        {
                                            StartSequence();
                                        }

                                    }
                                    break;
                                }
                                Thread.Sleep(5);
                            }
                        }
                        _currSeqCount = 0;
                    }
                    break;
            }

        }

        /// <summary>
        /// Request to trigger the event to update the textBox from the TalkApp
        /// </summary>
        /// <param name="message"></param>
        private void RequestToUpdateTextBox(string message)
        {
            _mainForm.Invoke(new MethodInvoker(delegate { EvtBCIUpdateTexttBox?.Invoke(message); }));
        }

        /// <summary>
        /// Resets timers
        /// </summary>
        private void Reset(bool delayAfterLayoutChange = false)
        {//Restore flags and object so it can change them
            _nextTarget = FindButtomFromId(1);
            _currSeqCount = 0;
            UnsuscribeTimers();
            CreateTimers();
            if (_repeatCalibration)
                _repeatCalibration = false;
            try
            {
                if (!SuspendAnimations)//This is when a user control is being changed
                {
                    if (_TempBoxesData != null && _tempwidgets != null)
                    {
                        _UpdateButtonsStrings = false;
                        SetDataObjectsSharpDX(_TempBoxesData, _tempwidgets);
                        //null objects after is to avoid if another call the reset fall into here and change the SharpDX objects
                        _TempBoxesData = null;
                        _tempwidgets = null;
                    }
                }
                else
                {//This is when pause animations, no user control changes no Message box prompts so it can go back to scannning after is requested
                    SuspendAnimations = false;
                    _isBoxScannig = true;
                    _numberOfSequences = _amountOfKeyboards;
                }

            }
            catch (Exception er)
            {
                Log.Debug("BCI LOG | Exception setDataObjectsSharpDX: " + er.Message);
            }
            _UpdateButtonsStrings = true;
            Thread.Sleep(250);
            _changeUserControl = false;
            _stopAnimation = false;
            if (_StartSequences)
            {// if delay is to have a time before animations starts
                if (delayAfterLayoutChange)
                    _ = ResetWaitingDelay();
                else
                    StartEpoch();
            }
            /*else 
                DrawMainLayout();*/

        }

        /// <summary>
        /// Delay after User control changes
        /// </summary>
        /// <returns></returns>
        private async Task ResetWaitingDelay()
        {
            await Task.Delay(200);
            try
            {
                StopTimers();
                await Task.Delay(20);
                AbortTimers();
                await Task.Delay(100);
                if (!VerifyIfTimersAreStoped())
                {
                    StopTimers();
                    await Task.Delay(20);
                    AbortTimers();
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception  in ResetWaitingDelay: " + ex.Message);
            }
            await Task.Delay(200);
            DrawMainLayout();
            await Task.Delay(_delayDisplayDecision);
            ScanningNoticeBox();
            StartEpoch();
        }

        /// <summary>
        /// Short animation indicating box iterations are about to start, First box
        /// </summary>
        private void ScanningNoticeBox()
        {
            if (_isBoxScannig && !_lockAnimation)
            {
                int tempActiveKeyboard = _activeKeyboard;
                _activeKeyboard = 0;
                ChangeColorButtons(_flashingSequenceBoxList[0].ToList(), true, true, 6);
                Thread.Sleep(100);
                ChangeColorButtons(_flashingSequenceBoxList[0].ToList(), false, true, 0);
                Thread.Sleep(100);
                ChangeColorButtons(_flashingSequenceBoxList[0].ToList(), true, true, 6);
                Thread.Sleep(100);
                ChangeColorButtons(_flashingSequenceBoxList[0].ToList(), false, true, 0);
                _activeKeyboard = tempActiveKeyboard;
            }
        }

        /// <summary>
        /// Short animation indicating Row/Column iterations are about to start, first row or column
        /// </summary>
        private void ScanningNoticeRowColumn()
        {
            ChangeColorButtons(_flashingSequenceList[_activeKeyboard][0].ToList(), true, true, 6);
            Thread.Sleep(100);
            ChangeColorButtons(_flashingSequenceList[_activeKeyboard][0].ToList(), false, true, 0);
            Thread.Sleep(100);
            ChangeColorButtons(_flashingSequenceList[_activeKeyboard][0].ToList(), true, true, 6);
            Thread.Sleep(100);
            ChangeColorButtons(_flashingSequenceList[_activeKeyboard][0].ToList(), false, true, 0);
        }

        /// <summary>
        /// Sets the size of the list used in BCI
        /// </summary>
        /// <param name="recentAmountOfBoxes"></param>
        /// <param name="prevAmountofBoxes"></param>
        private void SetInitialObjectsSize(int recentAmountOfBoxes, int prevAmountofBoxes)
        {
            if (recentAmountOfBoxes != prevAmountofBoxes)
            {
                _amountOfKeyboards = recentAmountOfBoxes;
                _rectanglesButtonsList = new List<SharpDX.Mathematics.Interop.RawRectangleF>[recentAmountOfBoxes];
                _rectanglesButtonsRoundList = new List<SharpDX.Direct2D1.RoundedRectangle>[recentAmountOfBoxes];
                _buttonsStringsList = new List<string>[recentAmountOfBoxes];
                _ButtonDataList = new List<ButtonsData>[recentAmountOfBoxes];
                _ControlsBtns = new List<ScannerButtonControl>[recentAmountOfBoxes];
                _flashingSequenceIDList = new List<int[]>[recentAmountOfBoxes];
                _flashingSequenceList = new List<int[]>[recentAmountOfBoxes];
                _rectanglesProbBars = new Dictionary<int, SharpDX.Mathematics.Interop.RawRectangleF>[recentAmountOfBoxes];
                _BoxesData = new Dictionary<List<Control>, string>();
                _flashingSequenceIDBoxList = new List<int>[recentAmountOfBoxes];
                _flashingSequenceBoxList = new List<int>[recentAmountOfBoxes];
                _rectanglesProbBarsBox = new Dictionary<int, SharpDX.Mathematics.Interop.RawRectangleF>();
                _typeOfBox = new string[recentAmountOfBoxes];
                _widgets = new List<Widget>[recentAmountOfBoxes];
                _buttonTextFormatList = new List<TextFormat>[recentAmountOfBoxes];
                _offsetStrings = new List<int>[recentAmountOfBoxes];
            }
        }
        /// <summary>
        /// Draw the progress bars into the UI
        /// </summary>
        private void SetValueProbabilityBars()
        {
            var rectBtnsss = new List<SharpDX.Mathematics.Interop.RawRectangleF>();
            try
            {
                if (_progressBarsProbs != null && _actuator != null && SensorErrorState.ErrorCode == BCIErrorCodes.Status_Ok)
                {
                    foreach (var value in _progressBarsProbs)
                    {
                        try
                        {
                            if (value.Value > _MinimumProgressBarsValue)
                            {
                                SharpDX.Mathematics.Interop.RawRectangleF rect;
                                if (!_isBoxScannig)
                                {
                                    if (_rectanglesProbBars[_activeKeyboard].TryGetValue(value.Key, out rect))
                                    {
                                        rectBtnsss.Add(new SharpDX.Mathematics.Interop.RawRectangleF(rect.Left, rect.Top, (rect.Right - ((float)((rect.Right - rect.Left) * (1 - value.Value)))), rect.Bottom));
                                    }
                                }
                                else
                                {
                                    if (_rectanglesProbBarsBox.TryGetValue(value.Key, out rect))
                                    {
                                        rectBtnsss.Add(new SharpDX.Mathematics.Interop.RawRectangleF(rect.Left, rect.Top, (rect.Right - ((float)((rect.Right - rect.Left) * (1 - value.Value)))), rect.Bottom));
                                    }
                                }
                            }
                        }
                        catch (Exception es)
                        {
                            Log.Debug("BCI LOG | Exception occurred during Drawing progress bars probs: " + es.Message);
                        }
                    }
                    if (_showProbabilityIndicator)
                    {
                        foreach (var rect in rectBtnsss)
                        {
                            if (!_stopAnimation && !_changeUserControl)
                                _sharpDX_d2dRenderTarget.FillRectangle(rect, SharpDXColors.SolidColorBrushProgressBars);
                        }
                    }
                }
            }
            catch (Exception es)
            {
                Log.Debug("BCI LOG | Exception occurred during Drawing progress bars: " + es.Message);
            }
        }

        /// <summary>
        /// Animates and show the control chosen as the final decision 
        /// </summary>
        /// <param name="decision">control choose as decision</param>
        /// <param name="correct"></param>
        private void ShowDecision(ButtonsData decision, bool correct)
        {
            _currentCalibrationTarget = decision;
            bool highlight = true;
            int index = FindIndexFromID(decision.id);
            List<int> d = new List<int> { index };
            if (_ReturnToBoxScanning)
                highlight = false;
            if (!_stopAnimation && !_changeUserControl)
            {
                if (correct)
                    ChangeColorButtons(d, highlight, true, 5, true); //turn on
                else
                    ChangeColorButtons(d, highlight, true, 4); //turn on
            }
            List<int> seq = new List<int>() { decision.id - 1 };
            NewEntryLog(true, false, seq, 0, true);
            if (!_changeUserControl)
                _showDecisionTimer.Start();
        }

        /// <summary>
        /// Animates and show the box selected
        /// </summary>
        /// <param name="decision">control choose as decision</param>
        /// <param name="correct"></param>
        private void ShowDecisionBox(int decision, bool correct)
        {
            bool highlight = true;
            if (_ReturnToBoxScanning)
                highlight = false;
            List<int> d;
            try
            {
                d = _flashingSequenceBoxList[decision - 1].ToList();
                _activeKeyboard = decision - 1;
            }
            catch (Exception)
            {
                d = _flashingSequenceBoxList[1].ToList();
                _activeKeyboard = 1;
            }
            if (!_stopAnimation && !_changeUserControl)
            {
                if (correct)
                    ChangeColorButtons(d, highlight, true, 5, true); //turn on
                else
                    ChangeColorButtons(d, highlight, true, 4); //turn on
            }
            List<int> seq = new List<int>() { decision - 1 };
            NewEntryLog(true, false, seq, 0, true);
            if (!_changeUserControl)
                _showDecisionTimer.Start();
        }

        /// <summary>
        /// Timer handler for the decision display in animations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDecisionTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            if (!_stopAnimation)
            {

                var indexButton = FindIndexFromID(_currentCalibrationTarget.id);
                if (!_stopAnimation && !_changeUserControl)
                    ChangeColorButtons(indexButton, false, true, 2); //Stop displaying the decision
                List<int> seq = new List<int>() { _currentCalibrationTarget.id - 1 };
                NewEntryLog(false, false, seq, 0, true);
                if (!_changeUserControl && (_isBoxScannig || _lockAnimation) && !_stopAnimation)
                {
                    int counter = _delayDisplayDecision / 10;
                    int counterToRefresh = 0;
                    for (int i = 0; i < counter; i++)
                    {
                        counterToRefresh += 1;
                        if (counterToRefresh == 50 && !_changeUserControl)
                        {
                            DrawMainLayout();
                            counterToRefresh = 0;
                        }
                        Thread.Sleep(10);
                        if (_changeUserControl || SuspendAnimations)
                            break;
                    }
                }
                ScanningNoticeBox();
                _showDecisionTimer.Stop();
                if (!_changeUserControl && !SuspendAnimations)
                    _pauseBetweenEpochsTimer.Start();
            }
        }

        /// <summary>
        ///  Run Epoch
        /// </summary>
        private void StartEpoch()
        {
            int target = 1;
            switch (_sessionMode)
            {
                case BCIModes.CALIBRATION:
                    if (_sessionMode == BCIModes.CALIBRATION)
                    {
                        _numberOfSequences = _flashingSequenceList[_CalibrationBox].Count();
                    }
                    if (_currEpochCount <= _CalibrationTargetCount)
                    {
                        if (_useRandomSelectionTargetCalibration)
                        {
                            while (true)//loop until get a target ID 
                            {
                                target = BCIUtils.GetRandomID();//Id's were prev defined in the class so it can get an id that does not repeat 
                                if (_ScanningSection == BCIScanSections.Box)
                                {
                                    _currentCalibrationTarget = new ButtonsData { id = target + 1, text = "BOX " + (target + 1), name = "BOX" + (target + 1) };
                                    break;
                                }
                                else
                                {
                                    _currentCalibrationTarget = GetButtomFromIndex(target);
                                    if (!_currentCalibrationTarget.text.Equals("m"))//m is for space icon from custom font
                                    {
                                        target = FindIndexFromID(_currentCalibrationTarget.id);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            char[] result = _CalibrationStringtarget.ToUpper().ToCharArray();
                            int countertargets = 0;
                            while (true)
                            {
                                _currentCalibrationTarget = GetButtomFromIndex(countertargets);
                                if (!_currentCalibrationTarget.name.Contains("PWLItem") && _currentCalibrationTarget.text.Length > 0)
                                {

                                    if (string.IsNullOrWhiteSpace(result[_indexTargetCalibration].ToString()) && _currentCalibrationTarget.text.Equals("m"))
                                    {
                                        target = FindIndexFromID(_currentCalibrationTarget.id);
                                        _indexTargetCalibration += 1;
                                        if (_indexTargetCalibration > result.Length)
                                            _indexTargetCalibration = 0;
                                        break;
                                    }
                                    else if (_currentCalibrationTarget.text.Equals(result[_indexTargetCalibration].ToString()))
                                    {
                                        target = FindIndexFromID(_currentCalibrationTarget.id);
                                        _indexTargetCalibration += 1;
                                        if (_indexTargetCalibration == result.Length)
                                            _indexTargetCalibration = 0;
                                        break;
                                    }
                                }
                                countertargets += 1;
                                if (countertargets == _ButtonDataList[_CalibrationBox].Count)
                                    countertargets = 0;
                            }
                        }

                        if (!_stopAnimation && !_changeUserControl)// Display target in a color
                        {
                            List<int> intList = new List<int> { target };
                            if (_ScanningSection == BCIScanSections.Box)
                            {
                                _activeKeyboard = _currentCalibrationTarget.id - 1;
                                ChangeColorButtons(_flashingSequenceBoxList[target].ToList(), true, true, 1);
                            }
                            else
                                ChangeColorButtons(target, true, true, 1);
                            NewEntryLog(true, true, intList, 0, true);
                            ActuatorRequestMarker("1");
                        }
                        _currEpochCount += 1;
                        RequestToUpdateTextBox(GetCalibrationInProgressText());
                        if (!_changeUserControl)
                            _targetOffTimer.Start();
                    }
                    break;
                case BCIModes.TYPING:
                    if (_TypingTargetCount != 0)
                    {
                        if (_currEpochCount <= _TypingTargetCount)
                        {
                            _currEpochCount += 1;
                            if (!_changeUserControl)
                                _targetOffTimer.Start();
                        }
                    }
                    else
                    {
                        if (!_changeUserControl)
                            _targetOffTimer.Start();
                    }
                    _currentCalibrationTarget = _nextTarget;
                    break;
            }
        }

        /// <summary>
        /// Run sequence
        /// </summary>
        private void StartSequence()
        {
            int target = 1;
            switch (_sessionMode)
            {
                case BCIModes.CALIBRATION:
                    target = _CalibrationIterationsPerTarget;
                    break;
                case BCIModes.TYPING:
                    target = _TypingIterationsPerTarget;
                    break;
            }
            if (_currSeqCount <= target && !_changeUserControl) // Sequences in epoch
            {
                UnsubscribeTimerEvent(ref _trialTimer, TrialTimer_Tick);
                _trialTimer = CreateTimer(_trialITI * 1000 / 4, TrialTimer_Tick);
                _slotTrialTimerCount = 0;
                if (!_changeUserControl)
                    _trialTimer.Start();
                _currtButtonCount = 0;
                _currSeqCount += 1;
            }

        }

        /// <summary>
        /// Stops all timers
        /// </summary>
        private void StopTimers()
        {
            _trialTimer?.Stop();
            _targetOffTimer?.Stop();
            _endEpochTimer?.Stop();
            _showDecisionTimer?.Stop();
            _pauseBetweenEpochsTimer?.Stop();
        }

        /// <summary>
        /// Timer handler to Stop highlight target 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetOffTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            if (_sequenceDone)
            {
                _targetOffTimer.Stop();
                _sequenceDone = false;
                if (!_changeUserControl)
                    StartSequence();
            }
            else
            {
                int targetOff = FindIndexFromID(_currentCalibrationTarget.id);// target off
                List<int> t = new List<int> { targetOff };
                if (!_stopAnimation && !_changeUserControl)
                {
                    if (_sessionMode == BCIModes.CALIBRATION)
                        Thread.Sleep(800);
                    ChangeColorButtons(t, false, true, 1); //turn off target if calibration (settings variable change)
                    NewEntryLog(false, false, t, 0, true);
                    ActuatorRequestMarker("0");
                }
                if (_sessionMode == BCIModes.CALIBRATION)
                    Thread.Sleep(400);
                _targetOffTimer.Stop();
                if (!_changeUserControl)
                    StartSequence();
            }
        }

        /// <summary>
        /// Timer handler for the Animations states sequences
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrialTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            try
            {
                if (_lockAnimation)
                {
                    _isBoxScannig = false;
                    if (_defaultLockBoxanimation)
                    {
                        _activeKeyboard = _amountOfKeyboards - 1;
                        _numberOfSequences = _flashingSequenceList[_amountOfKeyboards - 1].Count();
                    }
                    else
                    {
                        _activeKeyboard = _customLockBoxAnimation - 1;
                        _numberOfSequences = _flashingSequenceList[_customLockBoxAnimation - 1].Count();
                    }
                }
                _slotTrialTimerCount++;
                if (_slotTrialTimerCount == 1)//Trigger box white, buttons highlighted (orange)
                {
                    if (_currtButtonCount < _numberOfSequences)
                    {
                        if (!_stopAnimation && !_changeUserControl)
                        {
                            if (!_isBoxScannig)
                            {
                                ChangeColorButtons(_flashingSequenceList[_activeKeyboard][_currtButtonCount].ToList(), true, true, 0); 
                                NewEntryLog(true, true, _flashingSequenceIDList[_activeKeyboard][_currtButtonCount].ToList(), _currtButtonCount);
                            }
                            else
                            {
                                _activeKeyboard = _currtButtonCount;
                                ChangeColorButtons(_flashingSequenceBoxList[_currtButtonCount].ToList(), true, true, 0); 
                                NewEntryLog(true, true, _flashingSequenceIDBoxList[_currtButtonCount].ToList(), _currtButtonCount);
                            }
                            ActuatorRequestMarker("1");
                        }
                    }
                }
                else if (_slotTrialTimerCount == 3)//Trigger box black, buttons highlighted (orange)
                {
                    if (!_stopAnimation && !_changeUserControl)
                    {
                        if (!_isBoxScannig)
                        {
                            ChangeColorButtons(_flashingSequenceList[_activeKeyboard][_currtButtonCount].ToList(), true, true, 0, true); 
                            NewEntryLog(true, false, _flashingSequenceIDList[_activeKeyboard][_currtButtonCount].ToList(), _currtButtonCount);
                        }
                        else
                        {
                            ChangeColorButtons(_flashingSequenceBoxList[_currtButtonCount].ToList(), true, true, 0, true); 
                            NewEntryLog(true, false, _flashingSequenceIDBoxList[_currtButtonCount].ToList(), _currtButtonCount);
                        }
                        ActuatorRequestMarker("0");
                    }
                }
                else if (_slotTrialTimerCount == 4)//Trigger box black, buttons not highlighted. 
                {

                    if (!_stopAnimation && !_changeUserControl)
                    {
                        if (!_isBoxScannig)
                        {
                            ChangeColorButtons(_flashingSequenceList[_activeKeyboard][_currtButtonCount].ToList(), false, true, 0, true); 
                            NewEntryLog(false, false, _flashingSequenceIDList[_activeKeyboard][_currtButtonCount].ToList(), _currtButtonCount);
                        }
                        else
                        {

                            ChangeColorButtons(_flashingSequenceBoxList[_currtButtonCount].ToList(), false, true, 0, true); 
                            NewEntryLog(false, false, _flashingSequenceIDBoxList[_currtButtonCount].ToList(), _currtButtonCount);
                        }
                    }
                    _currtButtonCount += 1;
                    _slotTrialTimerCount = 0;
                    if (_currtButtonCount == _numberOfSequences) // End of sequence,rotation
                    {
                        if (_currSeqCount == 1)// for the actuator request to add the marker latter as the first sequence
                            _firstSequence = true;
                        if (!_isBoxScannig && _sessionMode == BCIModes.TYPING)//in calibration this is not neccessary
                            ActuatorRequestValueProbs();
                        ActuatorRequestEndRepetition();
                        _sequenceDone = true;
                        _trialTimer.Stop();
                        if (SensorErrorState.ErrorCode != BCIErrorCodes.Status_Ok)//if and error was returned then it wont continue with animations 
                        {
                            Log.Debug("BCI LOG | Error entered in loop | ErrorCode: " + SensorErrorState.ErrorCode);
                            SoundManager.playSound(SoundManager.SoundType.CaregiverAttention);
                            Log.Debug("BCI LOG | SoundManager.playSound ");
                            /*while (true)
                            {
                                Thread.Sleep(500);
                            }*/
                        }
                        else
                        {
                            if (!SuspendAnimations)
                                ProcessEndSequence();

                            switch (_sessionMode)
                            {//this is to trigger events that need a UI iteraction so the forms are called from a higher level and not this class
                                case BCIModes.CALIBRATION://if apply it Need to be called at the end of the sequences so it ensures to be triggered when the timers is at the final step and not during 
                                    if (_endCalibration)
                                    {
                                        try
                                        {
                                            _sessionMode = BCIModes.TYPING;
                                            DrawMainLayout();
                                            _mainForm.Invoke(new MethodInvoker(delegate
                                            {
                                                EvtBCICalibrationComplete?.Invoke("Calibration succeeded \n Score: " + (GetAUC() * 100));
                                            }));
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Debug("BCI LOG | Error in _endCalibration: " + ex.Message);
                                        }
                                    }
                                    if (_repeatCalibration)
                                    {
                                        _mainForm.Invoke(new MethodInvoker(delegate
                                        {
                                            EvtBCIStartCalibration?.Invoke();
                                        }));
                                    }
                                    break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception TrialTimer_Tick: " + ex.Message);
            }
        }

        /// <summary>
        /// Timer handler for the Trigger test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TriggerTestTimer_Tick(object sender, MicroTimerEventArgs e)
        {
            try
            {
                _slotTrialTimerCount++;
                if (_slotTrialTimerCount == 1)
                {
                    if (_currtButtonCount2 < _TriggerTestRepetitions)
                    {
                        if (!_changeUserControl)
                        {
                            ChangeColorButtons(new List<int>(), true, true, 0);
                            NewEntryLog(true, false, new List<int>(), _TriggerTestRepetitions - 1);
                        }
                    }
                }
                else if (_slotTrialTimerCount == 3)
                {
                    if (!_changeUserControl)
                    {
                        ChangeColorButtons(new List<int>(), true, true, 0, true);
                        NewEntryLog(true, true, new List<int>(), _TriggerTestRepetitions - 1);
                    }
                }
                else if (_slotTrialTimerCount == 4)
                {
                    if (!_changeUserControl)
                    {
                        ChangeColorButtons(new List<int>(), false, true, 0, true);
                        NewEntryLog(false, true, new List<int>(), _TriggerTestRepetitions - 1);
                    }
                    _slotTrialTimerCount = 0;
                    _TriggerTestRepetitions -= 1;
                    if (_currtButtonCount2 == _TriggerTestRepetitions)
                    {
                        _TriggerTest_Timer.Stop();
                        DrawMainLayout();
                        ActuatorRequestTriggerTestFinish();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception TriggerTestTimer_Tick: " + ex.Message);
            }
        }

        /// <summary>
        /// Unsubscribe the timer from the event handler
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="eventHandler"></param>
        private void UnsubscribeTimerEvent(ref MicroTimer timer, MicroTimer.MicroTimerElapsedEventHandler eventHandler)
        {
            if (timer != null)
            {
                timer.MicroTimerElapsed -= eventHandler;
                timer = null;
            }
        }

        /// <summary>
        /// Unsubscribe the timers from their events
        /// </summary>
        private void UnsuscribeTimers()
        {
            UnsubscribeTimerEvent(ref _trialTimer, TrialTimer_Tick);
            UnsubscribeTimerEvent(ref _targetOffTimer, TargetOffTimer_Tick);
            UnsubscribeTimerEvent(ref _endEpochTimer, EndEpochTimer_Tick);
            UnsubscribeTimerEvent(ref _showDecisionTimer, ShowDecisionTimer_Tick);
            UnsubscribeTimerEvent(ref _pauseBetweenEpochsTimer, PauseBetweenEpochs_Tick);
            UnsubscribeTimerEvent(ref _TriggerTest_Timer, TriggerTestTimer_Tick);

        }

        /// <summary>
        /// In CopyPhrase mode, updates the text that has been copied
        /// </summary>
        /// <param name="decision"></param>
        /// <param name="correctDecision"></param>
        private void UpdateCopiedText()
        {
            _nextTarget = GetButtomFromIndex(1);
        }

        /// <summary>
        /// Thread to get the strings from the predicted words/letters buttons
        /// </summary>
        /// <returns></returns>
        private async Task UpdateStringsFromButtons()
        {//this is to get the latest string from the buttons since sharpDX uses a string to draw instead of the Windows control that updates automatically has to be manually obtained 
            await Task.Delay(10);
            while (!_finishTask)
            {
                try
                {
                    if (_BoxesData != null && _UpdateButtonsStrings)
                    {
                        int amountBoxes = 0;
                        int indexBox = 0;
                        int indexTypeBox = 0;
                        for (int ib = 0; ib < _BoxesData.Count; ib++)
                        {
                            var val = _BoxesData.ElementAt(ib);
                            amountBoxes = AnimationManagerUtils.GetAmountBoxes(val.Value);
                            var tempbtnsStringsAll = AnimationManagerUtils.ExtractButtonText(val.Key, val.Value, amountBoxes);
                            for (int ii = 0; ii < amountBoxes; ii++)
                            {
                                if (_typeOfBox[indexTypeBox] != null)
                                    _buttonsStringsList[indexBox] = tempbtnsStringsAll[ii];
                                indexBox += 1;
                            }
                        }
                    }
                    await Task.Delay(500);
                }
                catch (Exception es)
                {
                    Log.Debug("BCI LOG | Exception occurred during updating strings from buttons: " + es.Message);
                    await Task.Delay(2000);
                }
            }
        }

        /// <summary>
        /// Validates the error sensor state 
        /// </summary>
        private void ValidateSensorErrorState(BCIError bCIError)
        {
            if (bCIError.ErrorCode != BCIErrorCodes.Status_Ok)
            {
                RequestToUpdateTextBox(bCIError.ErrorMessage + " ( Error Code: " + (int)bCIError.ErrorCode + " )");
                Log.Debug("BCI LOG | ErrorCode received " + (int)bCIError.ErrorCode);
                Log.Debug("BCI LOG | ErrorMessage received " + bCIError.ErrorMessage);
            }
        }

        /// <summary>
        /// Checks if timers are stoped and not running
        /// </summary>
        /// <returns></returns>
        private bool VerifyIfTimersAreStoped()
        {
            return !((_trialTimer != null && _trialTimer.Enabled) && (_targetOffTimer != null && _targetOffTimer.Enabled) && (_endEpochTimer != null && _endEpochTimer.Enabled) && (_showDecisionTimer != null && _showDecisionTimer.Enabled) && (_pauseBetweenEpochsTimer != null && _pauseBetweenEpochsTimer.Enabled) && (_TriggerTest_Timer != null && _TriggerTest_Timer.Enabled));
        }
        public struct ButtonsData
        {
            public String action { get; set; }
            public SolidColorBrush borderColor { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public String text { get; set; }
        }
    }
}