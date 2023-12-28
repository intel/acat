////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
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
    [DescriptorAttribute("301DBC87-C98C-491A-A2EE-D17863EAB831",
                    "OnboardingSwitchSelect",
                    "Switch select onboarding")]
    public class OnboardingSwitchSelect : OnboardingExtensionBase
    {
        private const String Step1 = "STEP 1";
        private IOnboardingWizard _wizard;
        private IActuator actuatorSelected = null;

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
            if (actuatorSelected.Name == "Keyboard Actuator" ||
                actuatorSelected.Name == "Switch Interface")
            {
                //MessageBox.Show("Actuator selected: " + actuatorSelected);
                IOnboardingExtension retVal = actuatorSelected.GetOnboardingExtension();
                if (retVal != null)
                {
                    retVal.Initialize(_wizard);
                }
                return retVal;
            }
            /*
            else if (actuatorSelected.Name == "Vision Actuator")
            {
                //var retVal = new OnboardingBCIActuator();
                //retVal.Initialize(_wizard);
                return retVal;
            }
            else if (actuatorSelected.Name == "BCI EEG Actuator")
            {
                //var retVal = new OnboardingBCIActuator();
                //retVal.Initialize(_wizard);
                return retVal;
            }
            */
            return null;
        }

        public override IOnboardingUserControl GetStep(String stepId)
        {
            IOnboardingUserControl userControl;

            switch (stepId)
            {
                case Step1:
                    userControl = new UserControlSwitchSelect(_wizard, this, stepId);
                    userControl.Initialize();
                    return userControl;

                default:
                    return null;
            }
        }

        public override bool Initialize(IOnboardingWizard wizard)
        {
            _wizard = wizard;

            Context.AppActuatorManager.LoadExtensions(Context.ExtensionDirs, true);

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
                    actuatorSelected = (userControl as UserControlSwitchSelect).ActuatorSelected;
                    var actuatorConfig = Context.AppActuatorManager.GetActuatorConfig();
                    var keyboardActuator = Context.AppActuatorManager.GetKeyboardActuator();

                    foreach (var actuatorSetting in actuatorConfig.ActuatorSettings)
                    {
                        if (keyboardActuator != null && actuatorSetting.Id.Equals(keyboardActuator.Descriptor.Id))
                        {
                            actuatorSetting.Enabled = true;
                        }
                        else
                        {
                            actuatorSetting.Enabled = actuatorSelected.Descriptor.Id.Equals(actuatorSetting.Id);
                        }

                        foreach (var actuator in Context.AppActuatorManager.Actuators)
                        {
                            if (actuator.Descriptor.Id.Equals(actuatorSetting.Id))
                            {
                                actuator.Enabled = actuatorSetting.Enabled;
                            }
                        }
                    }

                    actuatorConfig.Save();

                    break;
            }
        }
    }
}