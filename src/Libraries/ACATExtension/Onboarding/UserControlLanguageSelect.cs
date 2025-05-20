////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACATResources;
using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace ACAT.Lib.Extension.Onboarding
{
    /// <summary>
    /// User control that allows the user to select the input switch
    /// </summary>
    public partial class UserControlLanguageSelect : UserControl, IOnboardingUserControl
    {
        private readonly IOnboardingExtension _onboardingExtension;
        private readonly String _stepId;

        public UserControlLanguageSelect(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId)
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

        public bool Initialize()
        {
            return true;
        }

        public void OnAdded()
        {
            listBoxLanguages.Focus();

            var cultureInfos = ResourceUtils.EnumerateInstalledLanguages();
            string defaultLanguage = (CultureInfo.DefaultThreadCurrentUICulture != null)
                ? CultureInfo.DefaultThreadCurrentUICulture.DisplayName
                : "[Not set]";

            listBoxLanguages.Items.Add(defaultLanguage+"("+ CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName+")");

            foreach (var culture in cultureInfos)
             {
                 string text = culture.DisplayName + " (" + culture.TwoLetterISOLanguageName + ")";
                listBoxLanguages.Items.Add(text);
            }
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
   }
}