////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Onboarding;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// The usercontrol that is displayed at the end of onboarding
    /// </summary>
    public partial class UserControlFinish : UserControl, IOnboardingUserControl
    {
        private const int maxIterations = 3;
        private const int maxPeriods = 3;
        private int _count = 0;
        private int _iteration = 0;
        private readonly IOnboardingExtension _onboardingExtension;
        private readonly String _stepId;
        private Timer _timer;
        private readonly IOnboardingWizard _wizard;
        private readonly String message = "Launching ACAT.";

        public UserControlFinish(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId)
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

            labelStartingACAT.Text = message;
            _timer = new Timer
            {
                Interval = 1000
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();

            return retVal;
        }

        public void OnAdded()
        {
            _wizard.SetButtonVisible(OnboardingButtonTypes.ButtonBack, false);
            _wizard.SetButtonVisible(OnboardingButtonTypes.ButtonCancel, false);
            _wizard.SetButtonVisible(OnboardingButtonTypes.ButtonNext, false);
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

        private void _timer_Tick(object sender, EventArgs e)
        {
            String str;
            _count = (++_count % maxPeriods);
            if (_count == 0)
            {
                str = message;
            }
            else
            {
                str = labelStartingACAT.Text;
                str += ".";
            }

            labelStartingACAT.Text = str;

            _iteration++;

            if (_iteration > maxIterations)
            {
                _timer.Stop();
                _wizard.Quit(_onboardingExtension, Reason.OnboardingComplete, false);
            }
        }
    }
}