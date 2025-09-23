////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Extensions.Onboarding.UI.UserControls;
using ACAT.Extensions.Onboarding.UI;
using ACAT.Core.CoreInterfaces;
using System.Globalization;

namespace ACAT.Extensions.Onboarding.Onboarding
{
    /// <summary>
    /// The onboarding extension that lets the user select the input
    /// trigger switch
    /// </summary>
    [ClassDescriptor("F2803F8A-D639-459C-9F27-5742BAD4E405",
                    "OnboardingLanguageSelect",
                    "Language select onboarding")]
    public class OnboardingLanguageSelect : OnboardingExtensionBase
    {
        // TODO - Localize Me
        private const string Step1 = "STEP 1";

        private IOnboardingWizard _wizard;

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
            return null;
        }

        public override IOnboardingUserControl GetStep(string stepId)
        {
            IOnboardingUserControl userControl;

            switch (stepId)
            {
                case Step1:
                    userControl = new UserControlLanguageSelect(_wizard, this, stepId);
                    userControl.Initialize();
                    return userControl;

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

        public override void OnEndOnboarding(Reason reason)
        {
        }

        public override void OnEndStep(IOnboardingUserControl userControl, Reason reason)
        {
            using var userControlLang = (UserControlLanguageSelect)userControl;
            switch (userControl.StepId)
            {
                case Step1:
                    var cultureInfo = userControlLang.currentCulture;
                    Log.Debug ("User selected language: " + cultureInfo.DisplayName);

                    CoreGlobals.AppPreferences.Language = cultureInfo.TwoLetterISOLanguageName;
                    CoreGlobals.AppPreferences.Save();

                    break;
            }
        }
    }
}