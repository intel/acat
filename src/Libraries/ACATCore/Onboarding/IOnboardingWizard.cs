////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;

namespace ACAT.Lib.Core.Onboarding
{
    public enum OnboardingButtonTypes
    {
        ButtonNext,
        ButtonBack,
        ButtonCancel
    }

    /// <summary>
    /// The interface to the onboarding wizard that the extensions
    /// and usercontrols can use to communicate with the wizard
    /// like navigating, hiding/showing buttons etc.
    /// </summary>
    public interface IOnboardingWizard
    {
        void AddCustomButton(Control control, OnboardingButtonTypes buttonType);

        void GoBack(IOnboardingExtension source);

        void GotoNext(IOnboardingExtension source);

        void Quit(IOnboardingExtension source, Reason reason, bool userConfirm);

        void SetButtonEnable(OnboardingButtonTypes button, bool state);

        void SetButtonText(OnboardingButtonTypes button, string text);

        void SetButtonVisible(OnboardingButtonTypes button, bool visible);
    }
}