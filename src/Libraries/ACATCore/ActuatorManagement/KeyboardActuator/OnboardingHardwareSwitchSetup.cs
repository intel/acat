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

using ACAT.Lib.Core.Utility;
using System;
using ACAT.Lib.Core.Onboarding;

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// An onboarding extension to configure the hotkey for a switch
    /// </summary>
    [DescriptorAttribute("E5435A02-4F3E-43FB-9F3C-BA32C859870F",
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
        IOnboardingWizard _wizard;
        SwitchType _switchType;
        public OnboardingHardwareSwitchSetup(SwitchType switchType)
        {
            _switchType = switchType;
        }

        public override bool Initialize(IOnboardingWizard wizard)
        {
            _wizard = wizard;
            return true;
        }

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
            switch (stepId)
            {
                case Step1:
                    {
                        var uc = new UserControlHardwareSwitchSetup(_wizard, this, stepId, _switchType);
                        //uc.Initialize();
                        return uc;
                    }

                case Step2:
                    {
                        if (_switchType == SwitchType.SwitchInterface)
                        {
                            var uc = new UserControlHardwareSwitchTest(_wizard, this, stepId, _switchType);
                            //uc.Initialize();
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
            switch(currentStepID)
            {
                case Step1:
                    return GetStep(Step2);

                default:
                    return null;
            }
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

