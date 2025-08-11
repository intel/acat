////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using System;
using ACAT.Core.Onboarding;
using ACAT.Extensions.Onboarding.UI.UserControls;

namespace ACAT.Extensions.Onboarding
{
    /// <summary>
    /// The onboarding extension that signifies the end of onboarding
    /// </summary>
    [ClassDescriptor("E03754B3-85AF-4F43-855E-47E20F7400C2",
                        "OnboardingFinish",
                        "Final step in onboarding")]
    public class OnboardingFinish : OnboardingExtensionBase
    {
        // TODO - Localize Me
        private const String Step1 = "STEP 1";

        private IOnboardingWizard _wizard;

        public override ClassDescriptorAttribute Descriptor
        {
            get { return ClassDescriptorAttribute.GetDescriptor(GetType()); }
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
                    userControl = new UserControlFinish(_wizard, this, stepId);
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