////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// The onboarding extension that lets the user select the input
    /// trigger switch
    /// </summary>
    [Descriptor("F2803F8A-D639-459C-9F27-5742BAD4E405",
                    "OnboardingLanguageSelect",
                    "Language select onboarding")]
    public class OnboardingLanguageSelect : OnboardingExtensionBase
    {
        // TODO - Localize Me
        private const String Step1 = "STEP 1";
        private IOnboardingWizard _wizard;

        public override IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        public override IOnboardingUserControl GetFirstStep()
        {
            return GetStep(Step1);
        }

        public override IOnboardingExtension GetNextOnboardingExtension()
        {
            return null;
        }

        public override IOnboardingUserControl GetStep(String stepId)
        {
            IOnboardingUserControl userControl;

            switch (stepId)
            {
                case Step1:
                    userControl = new UserControlLanguageSelect(_wizard, this, stepId);
                    userControl.Initialize();
                    return userControl;

                default:
                    return null;
            }
        }

        public override bool Initialize(IOnboardingWizard wizard)
        {
            _wizard = wizard;
            return true;
        }

        public override bool IsFirstStep(string stepId)
        {
            return stepId == Step1;
        }

        public override bool IsLastStep(string stepId)
        {
            return stepId == Step1;
        }

        public override void OnEndOnboarding(Reason reason)
        {
        }

        public override void OnEndStep(IOnboardingUserControl userControl, Reason reason)
        {
            switch (userControl.StepId)
            {
                case Step1:
                    Log.Debug("Not Implemented");

                    break;
            }
        }
    }
}