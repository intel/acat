////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

//using ACATResources;
using ACATResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ACAT.Core.CoreInterfaces;

namespace ACAT.Extensions.Onboarding.UI.UserControls
{
    /// <summary>
    /// User control that allows the user to select the input switch
    /// </summary>
    public partial class UserControlLanguageSelect : UserControl, IOnboardingUserControl
    {
        public CultureInfo currentCulture = CultureInfo.CurrentUICulture;
        private readonly IOnboardingExtension _onboardingExtension;
        private readonly String _stepId;
        public UserControlLanguageSelect(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId)
        {
            InitializeComponent();

            _onboardingExtension = onboardingExtension;
            _stepId = stepId;
            var _currentCultureId = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            listBoxLanguages.SelectedIndexChanged += listBoxLanguages_SelectedIndexChanged;

            List<CultureInfo> installedCultures = ResourceHelper.GetAvailableResourceCultures(Assembly.GetExecutingAssembly());

            foreach (CultureInfo culture in installedCultures)
            {
                int index = listBoxLanguages.Items.Add(new LanguageItem(culture.DisplayName, culture));
                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals(culture.TwoLetterISOLanguageName))
                {
                    currentCulture = culture;
                }
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

        private void ApplyResourcesToControls(Control control, ComponentResourceManager resources)
        {
            foreach (Control childControl in control.Controls)
            {
                resources.ApplyResources(childControl, childControl.Name, Thread.CurrentThread.CurrentUICulture);
                ApplyResourcesToControls(childControl, resources);
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
                currentCulture = selectedItem.CultureInfo;
            }
        }
    }

    public class LanguageItem
    {
        public LanguageItem(string name, CultureInfo info)
        {
            DisplayName = name;
            CultureInfo = info;
        }

        public CultureInfo CultureInfo { get; set; }
        public String DisplayName { get; set; }
        public override String ToString()
        {
            return DisplayName;
        }
    }
}