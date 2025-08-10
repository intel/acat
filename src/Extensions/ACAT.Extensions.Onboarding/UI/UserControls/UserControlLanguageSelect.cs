////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

//using ACATResources;
using ACATResources;
using ACAT.Core.Onboarding;
using ACAT.Extensions.Onboarding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.Onboarding
{
    public class LanguageItem
    {
        public String DisplayName { get; set; }
        public CultureInfo CultureInfo { get; set; }

        public override String ToString()
        {
            return DisplayName;
        }

        public LanguageItem(string name, CultureInfo info)
        {
            DisplayName = name;
            CultureInfo = info;
        }
    }

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
            var _currentCultureId = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            listBoxLanguages.SelectedIndexChanged += listBoxLanguages_SelectedIndexChanged;

            var currentCulture = CultureInfo.CurrentUICulture;

            List<CultureInfo> installedCultures = ResourceHelper.GetAvailableResourceCultures(Assembly.GetExecutingAssembly());

            foreach (var culture in installedCultures)
            {
                int index = listBoxLanguages.Items.Add(new LanguageItem(culture.DisplayName, culture));
                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals(culture.TwoLetterISOLanguageName))
                {
                    currentCulture = culture;
                }
            }
        }

        private void listBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = listBoxLanguages.SelectedItem as LanguageItem;
            if (selectedItem != null)
            {
                Thread.CurrentThread.CurrentUICulture = selectedItem.CultureInfo;
                Thread.CurrentThread.CurrentCulture = selectedItem.CultureInfo;

                //var resourcesAssembly = typeof(ACATResources.ResourceHelper).Assembly;
                _ = new ComponentResourceManager(typeof(UserControlLanguageSelect));
                //ApplyResourcesToControls(this, resourceManager);
            }
        }

        private void ApplyResourcesToControls(Control control, ComponentResourceManager resources)
        {
            foreach (Control childControl in control.Controls)
            {
                resources.ApplyResources(childControl, childControl.Name, Thread.CurrentThread.CurrentUICulture);
                ApplyResourcesToControls(childControl, resources);
            }
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
            //if (listBoxLanguages.Items.Count > 0)
            //{
            //    listBoxLanguages.SelectedIndex = listBoxLanguages.Items.IndexOf(currentCulture);
            //}
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