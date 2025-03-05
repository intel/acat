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
    /// The onboarding extension that lets the user select the preferred
    /// keyboard configuration
    /// </summary>
    [DescriptorAttribute("65B95DE3-BF5A-4AE8-B44D-F5E7950AB8D6",
                        "OnboardingKeyboardConfigSelect",
                        "Keyboard coniguration select onboarding")]
    public class OnboardingKeyboardConfigSelect : OnboardingExtensionBase
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

        public override IOnboardingUserControl GetNextStep(String stepID)
        {
            switch (stepID)
            {
                default:
                    return null;
            }
        }

        public override IOnboardingUserControl GetStep(String stepId)
        {
            switch (stepId)
            {
                case Step1:
                    PanelConfigMap.LoadPanelClassConfig();
                    var retVal = new UserControlKeyboardConfigSelect(_wizard, this, stepId);
                    bool success = retVal.Initialize();

                    return (success) ? retVal : null;

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

        public override void OnEndStep(IOnboardingUserControl userControl, Reason reason)
        {
            switch (userControl.StepId)
            {
                case Step1:
                    PanelConfigMap.SavePanelClassConfig();
                    break;

                default:
                    break;
            }
        }
    }
}