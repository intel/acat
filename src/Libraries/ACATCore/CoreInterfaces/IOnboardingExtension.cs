////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Extensions;
using ACAT.Core.Utility.TypeLoader;

namespace ACAT.Core.CoreInterfaces
{
    /// <summary>
    /// Reason for navigation in the onboarding process
    /// </summary>
    public enum Reason
    {
        None,
        GotoPrev,
        GotoNext,
        CancelOnboarding,
        OnboardingComplete
    }

    /// <summary>
    /// The interface for an onboarding extension. The onboarding
    /// extension represents module that needs to be included in
    /// the onboarding process and can have multiple steps during
    /// onboarding.
    /// </summary>
    public interface IOnboardingExtension : IPluginExtension, IExtension
    {
        //ClassDescriptorAttribute Descriptor { get; }

        bool StartOverOnBackwardNavigation { get; }

        IOnboardingUserControl GetFirstStep();

        IOnboardingExtension GetNextOnboardingExtension();

        IOnboardingUserControl GetNextStep(string currentStepID);

        IOnboardingUserControl GetStep(string currentStepId);

        bool Initialize(IOnboardingWizard wizard);

        bool IsFirstStep(string stepId1);

        bool IsLastStep(string stepId);

        bool OnBeginOnboarding();

        void OnBeginStep(IOnboardingUserControl userControl);

        void OnEndOnboarding(Reason reason);

        void OnEndStep(IOnboardingUserControl userControl, Reason reason);
    }
}