////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.ActuatorManagement.Settings;
using ACAT.Core.CoreInterfaces;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Extensions.Onboarding.UI;
using ACAT.Extensions.Onboarding.UI.UserControls;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Extensions.Onboarding.Onboarding
{
    /// <summary>
    /// The onboarding extension that lets the user select the input
    /// trigger switch
    /// </summary>
    [ClassDescriptor("301DBC87-C98C-491A-A2EE-D17863EAB831",
                    "OnboardingSwitchSelect",
                    "Switch select onboarding")]
    public class OnboardingSwitchSelect : OnboardingExtensionBase
    {
        // TODO - Localize Me
        private const string Step1 = "STEP 1";

        private IOnboardingWizard _wizard;
        private IActuator actuatorSelected = null;

        public override ClassDescriptorAttribute Descriptor
        {
            get { return ClassDescriptorAttribute.GetDescriptor(GetType()); }
        }

        public override IOnboardingUserControl GetFirstStep()
        {
            return GetStep(Step1);
        }

        public override IOnboardingExtension GetNextOnboardingExtension()
        {
            IOnboardingExtension extension = null;
            switch (actuatorSelected.Name)
            {
                case "Keyboard Actuator":
                    extension = new OnboardingHardwareSwitchSetup(OnboardingHardwareSwitchSetup.SwitchType.Keyboard);
                    break;
                case "Switch Interface":
                    extension = new OnboardingHardwareSwitchSetup(OnboardingHardwareSwitchSetup.SwitchType.SwitchInterface);
                    break;
            }

            extension?.Initialize(_wizard);
            return extension;
        }

        public override IOnboardingUserControl GetStep(string stepId)
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

            ILogger<OnboardingSwitchSelect> logger = LoggingConfiguration.CreateLogger<OnboardingSwitchSelect>();
            logger.LogDebug("OnboardingSwitchSelect.Initialize() called");
            logger.LogDebug($"Loading actuator extensions from: {Context.ExtensionDirs}");

            Context.AppActuatorManager.LoadExtensions(Context.ExtensionDirs, true);

            IEnumerable<IActuator> actuators = Context.AppActuatorManager.ActuatorsList;
            logger.LogDebug($"After LoadExtensions, ActuatorsList contains {actuators.Count()} actuators");

            foreach (IActuator actuator in actuators)
            {
                logger.LogDebug($"  Actuator loaded: {actuator.Name} (ID: {actuator.Descriptor.Id})");
            }

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
                    ActuatorConfig actuatorConfig = Context.AppActuatorManager.GetActuatorConfig();
                    IActuator keyboardActuator = Context.AppActuatorManager.GetKeyboardActuator();

                    foreach (ActuatorSetting actuatorSetting in actuatorConfig.ActuatorSettings)
                    {
                        if (keyboardActuator != null && actuatorSetting.Id.Equals(keyboardActuator.Descriptor.Id))
                        {
                            actuatorSetting.Enabled = true;
                        }
                        else
                        {
                            actuatorSetting.Enabled = actuatorSelected.Descriptor.Id.Equals(actuatorSetting.Id);
                        }

                        foreach (IActuator actuator in Context.AppActuatorManager.ActuatorsList)
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