////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Onboarding
{
    /// <summary>
    /// The container form for a single step in the onboarding process.
    /// It has buttons for naviation and hosts the usercontrol for
    /// an onboarding step
    /// </summary>
    public partial class OnboardingForm : Form
    {
        public String OnboardingFileName { get; set; }
        public OnboardingSequence Sequence { get; set; }

        public bool QuitOnboarding = false;
        private const string buttonBackText = "Back";
        private const string buttonCancelText = "Exit";
        private const String buttonNextText = "Next";
        private Dictionary<OnboardingButtonTypes, Tuple<Color, Color>> _buttonColors = new Dictionary<OnboardingButtonTypes, Tuple<Color, Color>>();
        private Dictionary<Control, OnboardingButtonTypes> _buttonMap = new Dictionary<Control, OnboardingButtonTypes>();
        private IOnboardingExtension _currentOnboardExtension = null;
        private IOnboardingUserControl _currentStep = null;
        private OnboardingWizard _onboardingWizard;

        public OnboardingForm()
        {
            InitializeComponent();

            Load += OnboardingForm_Load;

            FormClosing += OnboardingForm_FormClosing;
        }
        private void _onboardingWizard_EvtGoBack(IOnboardingExtension source)
        {
            goBack();
        }

        private void _onboardingWizard_EvtGotoNext(IOnboardingExtension source)
        {
            gotoNext();
        }

        private void _onboardingWizard_EvtQuit(IOnboardingExtension source, Reason reason, bool confirm)
        {
            quit(reason, confirm);
        }

        private void _onboardingWizard_EvtSetButtonEnabled(OnboardingButtonTypes buttonType, bool enable)
        {
            enableButton(buttonType, enable);
        }

        private void _onboardingWizard_EvtSetButtonText(OnboardingButtonTypes buttonType, string text)
        {
            var control = getButton(buttonType);
            if (control != null)
            {
                control.Text = text;
            }
        }

        private void _onboardingWizard_EvtSetButtonVisible(OnboardingButtonTypes buttonType, bool visible)
        {
            var control = getButton(buttonType);
            if (control != null)
            {
                control.Visible = visible;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            quit(Reason.CancelOnboarding, true);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            gotoNext();
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            goBack();
        }

        private void enableButton(OnboardingButtonTypes buttonType, bool enable)
        {
            var control = getButton(buttonType);
            if (control == null)
            {
                return;
            }

            control.Enabled = enable;

            var tuple = _buttonColors[buttonType];

            if (tuple != null)
            {
                if (!control.Enabled)
                {
                    control.BackColor = Color.Gray;
                    control.ForeColor = Color.DimGray;
                }
                else
                {
                    control.BackColor = tuple.Item1;
                    control.ForeColor = tuple.Item2;
                }
            }
        }

        private Control getButton(OnboardingButtonTypes buttonType)
        {
            foreach (var key in _buttonMap.Keys)
            {
                if (_buttonMap[key] == buttonType)
                {
                    return key;
                }
            }

            return null;
        }

        private void goBack()
        {
            setButtonDefaults();

            if (_currentOnboardExtension == null)
            {
                return;
            }

            if (!_currentStep.QueryGoToPrevStep())
            {
                return;
            }

            var historyEntry = _onboardingWizard.GetPrevious();
            if (historyEntry == null)
            {
                return;
            }

            _currentOnboardExtension = historyEntry.OnboardingExtension;
            _currentStep = _currentOnboardExtension.GetStep(historyEntry.StepId);

            replaceUserControl(_currentStep, null, Reason.GotoPrev);

            setButtonStates();

            _currentStep.OnAdded();
        }

        private void gotoNext()
        {
            setButtonDefaults();

            if (_onboardingWizard.IsLastOnboardingExtension(_currentOnboardExtension) &&
                _currentOnboardExtension.IsLastStep(_currentStep.StepId))
            {
                saveOnboardingComplete();

                Close();
                return;
            }

            if (_currentOnboardExtension == null)
            {
                return;
            }

            if (!_currentStep.QueryGoToNextStep())
            {
                return;
            }

            _currentOnboardExtension.OnEndStep(_currentStep, Reason.GotoNext);

            var step = _currentOnboardExtension.GetNextStep(_currentStep.StepId);

            IOnboardingExtension nextExtension;
            IOnboardingExtension prevExtension = null;

            if (step == null)
            {
                prevExtension = _currentOnboardExtension;

                prevExtension.OnEndOnboarding(Reason.GotoNext);

                nextExtension = prevExtension.GetNextOnboardingExtension();
                if (nextExtension == null)
                {
                    nextExtension = _onboardingWizard.GetNextOnboardingExtension();
                }
                else
                {
                    //_onboardingWizard.InsertAfterOnboardingExtension(prevExtension, nextExtension);
                }
                if (nextExtension != null)
                {
                    _currentOnboardExtension = nextExtension;

                    step = _currentOnboardExtension.GetFirstStep();
                }
            }

            if (step != null)
            {
                _currentStep = step;
                replaceUserControl(_currentStep, prevExtension, Reason.GotoNext);
            }

            setButtonStates();

            if (step != null)
            {
                _currentStep.OnAdded();
            }
            else // recurse. to skip the step
            {
                gotoNext();
            }
        }

        private void OnboardingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!QuitOnboarding)
            {
                e.Cancel = true;
            }
        }

        private void OnboardingForm_Load(object sender, EventArgs e)
        {
            _onboardingWizard = new OnboardingWizard();

            if (!String.IsNullOrEmpty(OnboardingFileName))
            {
                if (!File.Exists(OnboardingFileName))
                {
                    ConfirmBoxSingleOption.ShowDialog("Onboarding file " + OnboardingFileName + " does not exist", "OK");
                    QuitOnboarding = true;
                    Close();
                    return;
                }

                OnboardingSequence.SettingsFilePath = OnboardingFileName;
                Sequence = OnboardingSequence.Load();
            }

            if (Sequence == null || Sequence.OnboardingSequenceItems.Count == 0)
            {
                ConfirmBoxSingleOption.ShowDialog("Onboarding sequence is null or empty", "OK");
                QuitOnboarding = true;
                Close();
                return;
            }

            _buttonMap[buttonBack] = OnboardingButtonTypes.ButtonBack;
            _buttonMap[buttonNext] = OnboardingButtonTypes.ButtonNext;
            _buttonMap[buttonCancel] = OnboardingButtonTypes.ButtonCancel;

            foreach (OnboardingButtonTypes buttonType in (OnboardingButtonTypes[])Enum.GetValues(typeof(OnboardingButtonTypes)))
            {
                var control = getButton(buttonType);
                if (control != null)
                {
                    _buttonColors.Add(buttonType, Tuple.Create(control.BackColor, control.ForeColor));
                }
            }

            setButtonDefaults();

            ///Loads actuators dll's
            bool retVal = _onboardingWizard.Initialize(Sequence);
            if (!retVal)
            {
                QuitOnboarding = true;
                FormClosing -= OnboardingForm_FormClosing;
                removeOnboardingUserControl();
                Close();
            }

            _onboardingWizard.EvtSetButtonEnabled += _onboardingWizard_EvtSetButtonEnabled;
            _onboardingWizard.EvtSetButtonText += _onboardingWizard_EvtSetButtonText;
            _onboardingWizard.EvtSetButtonVisible += _onboardingWizard_EvtSetButtonVisible;
            _onboardingWizard.EvtQuit += _onboardingWizard_EvtQuit;
            _onboardingWizard.EvtGoBack += _onboardingWizard_EvtGoBack;
            _onboardingWizard.EvtGotoNext += _onboardingWizard_EvtGotoNext;

            _currentOnboardExtension = _onboardingWizard.GetNextOnboardingExtension();

            if (_currentOnboardExtension != null)
            {
                _currentOnboardExtension.OnBeginOnboarding();

                _currentStep = _currentOnboardExtension.GetFirstStep();

                var userControl = _currentStep as UserControl;

                replaceUserControl(_currentStep);

                setButtonStates();

                _currentStep.OnAdded();
            }
        }

        private void quit(Reason reason, bool confirm)
        {
            if (_currentStep == null || _currentStep.QueryCancelOnboarding())
            {
                if (reason == Reason.CancelOnboarding && _onboardingWizard.IsFirstOnboardingExtension(_currentOnboardExtension))
                {
                    confirm = false;
                    QuitOnboarding = true;
                }

                if (confirm)
                {
                    if (!ConfirmBox.ShowDialog("ACAT configuration incomplete. Quit anyway?", "Quit"))
                    {
                        return;
                    }
                    else
                    {
                        QuitOnboarding = true;
                    }
                }

                FormClosing -= OnboardingForm_FormClosing;
                removeOnboardingUserControl();

                if (reason == Reason.OnboardingComplete)
                {
                    saveOnboardingComplete();
                }

                Close();
            }
        }

        private void removeOnboardingUserControl()
        {
            Control control = null;
            if (panelContainer.Controls.Count > 0)
            {
                control = panelContainer.Controls[0];
            }

            this.panelContainer.Controls.Clear();

            if (control is IOnboardingUserControl)
            {
                (control as IOnboardingUserControl).OnRemoved();
            }
        }

        private void replaceUserControl(IOnboardingUserControl iuc, IOnboardingExtension prevExtension = null, Reason replaceReason = Reason.None)
        {
            SuspendLayout();

            removeOnboardingUserControl();

            var uc = iuc as UserControl;

            if (prevExtension != null && prevExtension != iuc.OnboardingExtension)
            {
                iuc.OnboardingExtension.OnBeginOnboarding();
            }

            iuc.OnPreAdd();

            iuc.OnboardingExtension.OnBeginStep(iuc);

            uc.Dock = DockStyle.Fill;
            this.panelContainer.Controls.Add(uc);

            //this.panelContainer.BorderStyle = BorderStyle.FixedSingle;
            this.panel1.BorderStyle = BorderStyle.FixedSingle;

            ResumeLayout();

            if (replaceReason == Reason.GotoNext || replaceReason == Reason.None)
            {
                _onboardingWizard.AddToHistory(iuc.OnboardingExtension, iuc.StepId);
            }
        }

        private void saveOnboardingComplete()
        {
            CoreGlobals.AppPreferences.OnboardingComplete = true;

            CoreGlobals.AppPreferences.Save();
        }

        private void setButtonDefaults()
        {
            buttonNext.Text = buttonNextText;
            buttonBack.Text = buttonBackText;
            buttonCancel.Text = buttonCancelText;

            enableButton(_buttonMap[buttonNext], true);
            enableButton(_buttonMap[buttonBack], true);
            enableButton(_buttonMap[buttonCancel], true);

            buttonNext.Visible = true;
            buttonBack.Visible = true;
            buttonCancel.Visible = true;
        }

        private void setButtonStates()
        {
            if (_currentOnboardExtension != null)
            {
                if (_onboardingWizard.IsFirstOnboardingExtension(_currentOnboardExtension))
                {
                    buttonBack.Visible = false;
                }
                else
                {
                    buttonBack.Visible = true;
                }
            }

            buttonCancel.Visible = true;

            if (_currentOnboardExtension != null && _onboardingWizard.IsLastOnboardingExtension(_currentOnboardExtension) &&
                _currentOnboardExtension.IsLastStep(_currentStep.StepId))
            {
                buttonNext.Text = "Finish";
            }
        }
    }
}