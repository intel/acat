////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// User control that displays the Welcome screen
    /// </summary>
    public partial class UserControlWelcome : UserControl, IOnboardingUserControl
    {
        private const int timeout = 5;
        private readonly IOnboardingExtension _onboardingExtension;
        private readonly String _stepId;
        private int _timeLeft = 5;
        private Timer _timer;
        private readonly IOnboardingWizard _wizard;
        private readonly String blurb = "\r\nEnable communication through keyboard simulation, word prediction, and speech synthesis for people with limited speech and typing capabilities.\r\n\r\nClick on Configure to change your settings.";
        private readonly String clickOnConfigure = "Click on Configure to change your settings";
        private readonly String startPrompt = "ACAT will start in xxx";

        public UserControlWelcome(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId)
        {
            InitializeComponent();

            _wizard = wizard;
            _onboardingExtension = onboardingExtension;
            _stepId = stepId;
        }

        public IOnboardingExtension OnboardingExtension
        {
            get
            {
                return _onboardingExtension;
            }
        }

        public String StepId
        {
            get
            {
                return _stepId;
            }
        }

        public UserControl GetUserControl()
        {
            return this;
        }

        public bool Initialize()
        {
            bool retVal = true;

            var prompt = startPrompt.Replace("xxx", timeout.ToString());

            if (Common.AppPreferences.OnboardingComplete)
            {
                labelBlurb.Text = clickOnConfigure;

                updateCountdown();

                var font = buttonConfigure.Font;
                buttonConfigure.Font = new Font(font.FontFamily, font.Size, FontStyle.Underline, GraphicsUnit.Point, ((byte)(0))); ;

                _timer = new Timer
                {
                    Interval = 1000
                };
                _timer.Tick += _timer_Tick;
            }
            else
            {
                labelBlurb.Text = blurb;
                buttonConfigure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
                buttonConfigure.ForeColor = System.Drawing.Color.Black;
            }

            return retVal;
        }

        public void OnAdded()
        {
            _wizard.SetButtonVisible(OnboardingButtonTypes.ButtonBack, false);
            _wizard.SetButtonVisible(OnboardingButtonTypes.ButtonNext, false);

            if (Common.AppPreferences.OnboardingComplete)
            {
                _timer.Start();
            }
        }

        public bool OnPreAdd()
        {
            return true;
        }

        public void OnRemoved()
        {
        }

        public bool QueryCancelOnboarding()
        {
            return true;
        }

        public bool QueryGoToNextStep()
        {
            return true;
        }

        public bool QueryGoToPrevStep()
        {
            return true;
        }

        public void updateCountdown()
        {
            var prompt = startPrompt.Replace("xxx", _timeLeft.ToString() + ((_timeLeft > 1) ? " secs" : " sec"));

            Windows.SetText(labelCountdown, "\r\n\r\n" + prompt);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timeLeft--;
            updateCountdown();

            if (_timeLeft <= 0)
            {
                _timer.Stop();
                _wizard.Quit(_onboardingExtension, Reason.OnboardingComplete, false);
            }
            else
            {
            }
        }

        private void buttonConfigure_Click(object sender, EventArgs e)
        {
            _timer?.Stop();

            CoreGlobals.AppPreferences.OnboardingComplete = false;

            CoreGlobals.AppPreferences.Save();

            _wizard.GotoNext(_onboardingExtension);
        }
    }
}