////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.CoreInterfaces
{
    /// <summary>
    /// Interface for the user control that is the GUI for
    /// a single step during the onboarding process
    /// </summary>
    public interface IOnboardingUserControl
    {
        IOnboardingExtension OnboardingExtension { get; }

        string StepId { get; }

        bool Initialize();

        void OnAdded();

        bool OnPreAdd();

        void OnRemoved();

        bool QueryCancelOnboarding();

        bool QueryGoToNextStep();

        bool QueryGoToPrevStep();
    }
}