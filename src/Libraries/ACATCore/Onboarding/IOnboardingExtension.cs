////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.Onboarding
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
    /// the onboarding process and can have multipe steps during
    /// onboarding.
    /// </summary>
    public interface IOnboardingExtension
    {
        IDescriptor Descriptor { get; }

        bool StartOverOnBackwardNavigation { get; }

        IOnboardingUserControl GetFirstStep();

        IOnboardingExtension GetNextOnboardingExtension();

        IOnboardingUserControl GetNextStep(String currentStepID);

        IOnboardingUserControl GetStep(String currentStepId);

        bool Initialize(IOnboardingWizard wizard);

        bool IsFirstStep(String stepId1);

        bool IsLastStep(String stepId);

        bool OnBeginOnboarding();

        void OnBeginStep(IOnboardingUserControl userControl);

        void OnEndOnboarding(Reason reason);

        void OnEndStep(IOnboardingUserControl userControl, Reason reason);
    }
}