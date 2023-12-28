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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// User control that allows the user to select the keyboard type like
    /// alphbetical and qwerty
    /// </summary>
    public partial class UserControlKeyboardConfigSelect : UserControl, IOnboardingUserControl
    {
        private readonly IOnboardingExtension _onboardingExtension;
        private PanelClassConfig _panelClassConfigForApp;
        private readonly List<PanelClassConfigMap> _panelClassConfigMaps = new List<PanelClassConfigMap>();
        private readonly String _stepId;

        public UserControlKeyboardConfigSelect(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId)
        {
            InitializeComponent();

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

            var actuatorConfig = Context.AppActuatorManager.GetActuatorConfig();
            var keyboardActuator = Context.AppActuatorManager.GetKeyboardActuator();

            if(keyboardActuator == null)
            {
                return false;
            }

            var keyboardActuatorConfigs = keyboardActuator.GetSupportedKeyboardConfigs();
            List<String> keyboardConfigs = new List<string>();

            var actuators = Context.AppActuatorManager.Actuators;
            foreach (var actuator in actuators)
            {
                if (!actuator.Enabled)
                {
                    continue;
                }

                if (!actuator.Descriptor.Id.Equals(keyboardActuator.Descriptor.Id))
                {
                    var configs = actuator.GetSupportedKeyboardConfigs();
                    if (configs != null)
                    {
                        keyboardConfigs.AddRange(configs);
                    }
                }
            }

            if (keyboardConfigs.Count == 0)
            {
                keyboardConfigs.AddRange(keyboardActuatorConfigs);
            }

            listBoxKeyboardConfigs.SelectedIndexChanged += ListBoxKeyboardConfigs_SelectedIndexChanged;

            _panelClassConfigForApp = PanelConfigMap.GetPanelClassConfigForApp();

            foreach (var panelClassConfigMap in _panelClassConfigForApp.PanelClassConfigMaps)
            {
                foreach (var name in keyboardConfigs)
                {
                    if (String.Compare(panelClassConfigMap.Name, name, true) == 0)
                    {
                        _panelClassConfigMaps.Add(panelClassConfigMap);
                        listBoxKeyboardConfigs.Items.Add(panelClassConfigMap.DisplayNameShort);
                    }
                }
            }

            if (listBoxKeyboardConfigs.Items.Count > 1)
            {
                listBoxKeyboardConfigs.SelectedIndex = 0;
            }
            else
            {
                if (listBoxKeyboardConfigs.Items.Count == 1)
                {
                    _panelClassConfigForApp.SetDefaultClassConfigMap(_panelClassConfigMaps[0].Name);
                    PanelConfigMap.SavePanelClassConfig();
                }

                retVal = false;
            }

            return retVal;
        }

        public void OnAdded()
        {
            listBoxKeyboardConfigs.Focus();
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

        private void ListBoxKeyboardConfigs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxKeyboardConfigs.SelectedIndex;
            Image image = null;
            if (index >= 0)
            {
                _panelClassConfigForApp.SetDefaultClassConfigMap(_panelClassConfigMaps[index].Name);
                labelDescription.Text = _panelClassConfigMaps[index].Description;
                if (!String.IsNullOrEmpty(_panelClassConfigMaps[index].ScreenshotFileName))
                {
                    image = ImageUtils.LoadImage(Path.Combine(FileUtils.GetImagePath(_panelClassConfigMaps[index].ScreenshotFileName)));
                }

                pictureBoxScreenShot.BackgroundImage = image;
            }
        }

        private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}