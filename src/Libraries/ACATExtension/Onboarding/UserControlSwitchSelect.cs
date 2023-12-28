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
using ACAT.ACATResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// User control that allows the user to select the input switch
    /// </summary>
    public partial class UserControlSwitchSelect : UserControl, IOnboardingUserControl
    {
        public IActuator ActuatorSelected = null;
        private const String bodyStyle = " background-color:#232433;";
        private const String headStyle = "a:link{color: rgb(255, 170, 0);}";
        private const String textStyle = "font-family:'Montserrat Medium'; font-size:24px; color:white";
        private ActuatorConfig _actuatorConfig;
        private readonly List<ActuatorSetting> _actuatorSettings = new List<ActuatorSetting>();
        private readonly String _htmlTemplate = "<!DOCTYPE html><html><head><style>{0}</style></head><body style=\"{1}\"><p style=\"{2}\">{3}<font></body></html>";
        private readonly IOnboardingExtension _onboardingExtension;
        private readonly String _stepId;

        public UserControlSwitchSelect(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId)
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

            var actuators = Context.AppActuatorManager.Actuators;

            _actuatorConfig = ActuatorConfig.Load();

            foreach (var actuator in actuators)
            {
                var actuatorSetting = getAcutatorSetting(actuator.Descriptor.Id);
                if (actuatorSetting != null)
                {
                    listBoxActuators.Items.Add(actuatorSetting.Name);
                    _actuatorSettings.Add(actuatorSetting);
                }
            }

            listBoxActuators.SelectedIndexChanged += ListBoxActuators_SelectedIndexChanged;

            if (listBoxActuators.Items.Count > 0)
            {
                listBoxActuators.SelectedIndex = 0;
            }

            webBrowserDesc.DocumentCompleted += WebBrowserDesc_DocumentCompleted;

            return retVal;
        }

        public void OnAdded()
        {
            listBoxActuators.Focus();
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

        private IActuator getActuator(int index)
        {
            var actuatorSetting = _actuatorSettings[index];
            foreach (var actuator in Context.AppActuatorManager.Actuators)
            {
                if (actuatorSetting.Id.Equals(actuator.Descriptor.Id))
                {
                    return actuator;
                }
            }

            return null;
        }

        private ActuatorSetting getAcutatorSetting(Guid guid)
        {
            foreach (var actuatorSetting in _actuatorConfig.ActuatorSettings)
            {
                if (guid.Equals(actuatorSetting.Id))
                {
                    return actuatorSetting;
                }
            }

            return null;
        }

        private void ListBoxActuators_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActuatorSelected = getActuator(listBoxActuators.SelectedIndex);
            var desc = _actuatorSettings[listBoxActuators.SelectedIndex].Description;
            if (!String.IsNullOrEmpty(desc))
            {
                String html = String.Format(_htmlTemplate, headStyle, bodyStyle, textStyle, desc);
                Log.Debug(html);

                html = html.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));

                webBrowserDesc.DocumentText = html;
            }

            var file = _actuatorSettings[listBoxActuators.SelectedIndex].ImageFileName;

            if (String.IsNullOrEmpty(file))
            {
                file = ActuatorSelected.OnboardingImageFileName;
            }

            Image image = null;
            if (!String.IsNullOrEmpty(file))
            {
                image = ImageUtils.LoadImage(Path.Combine(FileUtils.GetImagePath(file)));
            }

            pictureBoxActuator.BackgroundImage = image;
        }

        private void WebBrowserDesc_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowserDesc.Navigating -= WebBrowserDesc_Navigating;
            webBrowserDesc.Navigating += WebBrowserDesc_Navigating;
        }

        private void WebBrowserDesc_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var str = e.Url.ToString();

            Log.Debug("Url is [" + str + "]");

            if (str.ToLower().Contains("blank"))
            {
                return;
            }

            e.Cancel = true;

            String param1 = String.Empty;
            String param2 = String.Empty;

            if (str.Contains("about:"))
            {
                var index = str.IndexOf(':');

                str = str.Substring(index + 1);

                index = str.IndexOf('#');

                if (index > 0)
                {
                    param1 = str.Substring(0, index);
                    param2 = str.Substring(index + 1, str.Length - index -1);
                }
                else
                {
                    param1 = str;
                }
            }

            List<String> list = new List<String>();
            
            if (param2.ToLower().EndsWith(".mp4"))
            {
                list.Add("Video");
                list.Add(String.Empty);
                list.Add(String.Empty);
                list.Add((param2));
                list.Add(String.Empty);
            }
            else if (param1.ToLower().EndsWith(".pdf"))
            {
                list.Add("PDF");
                list.Add("true");
                list.Add(R.GetString("PDFLoaderHtml"));
                list.Add(param1);
                list.Add(param2);
            }

            try
            {
                HtmlUtils.LoadHtml(SmartPath.ApplicationPath, list.ToArray());
            }
            catch
            {

            }
            finally
            {

            }
        }
    }
}