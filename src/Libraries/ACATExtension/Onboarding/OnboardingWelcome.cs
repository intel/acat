////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// The onboarding extension that displays the Welcome screen
    /// </summary>
    [DescriptorAttribute("6D8DA00E-5035-4B7F-A646-ED9F840A13BF",
                    "OnboardingWelcome",
                    "Welcome onboarding")]
    public class OnboardingWelcome : OnboardingExtensionBase
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

        public override IOnboardingUserControl GetStep(String stepId)
        {
            IOnboardingUserControl userControl;

            switch (stepId)
            {
                case Step1:

                    userControl = new UserControlWelcome(_wizard, this, stepId);
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
    }
}