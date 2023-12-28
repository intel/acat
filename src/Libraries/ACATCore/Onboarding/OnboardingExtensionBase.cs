////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;

namespace ACAT.Lib.Core.Onboarding
{
    /// <summary>
    /// The base class for onboarding extensions
    /// </summary>
    public abstract class OnboardingExtensionBase : IOnboardingExtension
    {
        public abstract IDescriptor Descriptor { get; }
        public virtual bool StartOverOnBackwardNavigation => false;

        public virtual IOnboardingUserControl GetFirstStep()
        {
            return null;
        }

        public virtual IOnboardingExtension GetNextOnboardingExtension()
        {
            return null;
        }

        public virtual IOnboardingUserControl GetNextStep(string currentStepID)
        {
            return null;
        }

        public virtual IOnboardingUserControl GetStep(string currentStepId)
        {
            return null;
        }

        public virtual bool Initialize(IOnboardingWizard wizard)
        {
            return true;
        }

        public virtual bool IsFirstStep(string stepId1)
        {
            return false;
        }

        public virtual bool IsLastStep(string stepId)
        {
            return false;
        }

        public virtual bool OnBeginOnboarding()
        {
            return true;
        }

        public virtual void OnBeginStep(IOnboardingUserControl userControl)
        {
        }

        public virtual void OnEndOnboarding(Reason reason)
        {
        }

        public virtual void OnEndStep(IOnboardingUserControl userControl, Reason reason)
        {
        }
    }
}