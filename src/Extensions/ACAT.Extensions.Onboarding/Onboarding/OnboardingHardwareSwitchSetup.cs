////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// OnboardingHardwareSwitchSetup.cs
//
// An onboarding extension to configure the hotkey for a switch
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Onboarding;
using ACAT.Core.Utility;
using ACAT.Extensions.Onboarding.UI.UserControls;
using System;

namespace ACAT.Extensions.Onboarding
{
    /// <summary>
    /// An onboarding extension to configure the hotkey for a switch
    /// </summary>
    [ClassDescriptor("E5435A02-4F3E-43FB-9F3C-BA32C859870F",
                        "OnboardingHardwareSwitchSetup",
                        "Hardware switch select onboarding")]
    public class OnboardingHardwareSwitchSetup : OnboardingExtensionBase
    {
        public enum SwitchType
        {
            Keyboard,
            SwitchInterface
        }

        private const String Step1 = "STEP 1";
        private const String Step2 = "STEP 2";
        private IOnboardingWizard _wizard;
        private readonly SwitchType _switchType;

        public OnboardingHardwareSwitchSetup(SwitchType switchType)
        {
            _switchType = switchType;
        }

        public override bool Initialize(IOnboardingWizard wizard)
        {
            _wizard = wizard;
            return true;
        }

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
            switch (stepId)
            {
                case Step1:
                    {
                        var uc = new UserControlHardwareSwitchSetup(_wizard, this, stepId, _switchType);
                        uc.Initialize();
                        return uc;
                    }

                case Step2:
                    {
                        if (_switchType == SwitchType.SwitchInterface)
                        {
                            var uc = new UserControlHardwareSwitchTest(_wizard, this, stepId, _switchType);
                            uc.Initialize();
                            return uc;
                        }

                        return null;
                    }

                default:
                    return null;
            }
        }

        public override IOnboardingUserControl GetNextStep(string currentStepID)
        {
            return currentStepID switch
            {
                Step1 => GetStep(Step2),
                _ => null,
            };
        }

        public override bool IsLastStep(string stepId)
        {
            //return stepId == Step1;

            return _switchType == SwitchType.SwitchInterface ? stepId == Step2 : stepId == Step1;
        }

        public override bool IsFirstStep(string stepId)
        {
            return stepId == Step1;
        }

        public override void OnBeginStep(IOnboardingUserControl userControl)
        {
            userControl.Initialize();
        }
    }
}