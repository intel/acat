////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SwitchInterfaceActuator.cs
//
// The ACAT interface for an external switch switch (off-the-shelf)
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// The ACAT interface for an external switch switch (off-the-shelf)
    /// </summary>
    [DescriptorAttribute("790324B9-C733-49CA-9A28-C49357BB7C14",
                        "Switch Interface",
                        "External hardware switch")]
    public class SwitchInterfaceActuator : KeyboardActuator
    {
        public override String OnboardingImageFileName
        {
            get
            {
                return FileUtils.GetImagePath("HardwareSwitch.png");
            }
        }

        public override bool ShowTryoutOnStartup
        {
            get
            {
                return CoreGlobals.AppPreferences.ShowSwitchTryoutOnStartup;
            }
        }

        public override bool SupportsScanTimingsConfigureDialog
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsTryout
        {
            get
            {
                return true;
            }
        }

        public override IOnboardingExtension GetOnboardingExtension()
        {
            return new OnboardingHardwareSwitchSetup(OnboardingHardwareSwitchSetup.SwitchType.SwitchInterface);
        }

        public override bool ShowScanTimingsConfigureDialog()
        {
            return ShowDefaultScanTimingsConfigureDialog();
        }

        public override bool ShowTryoutDialog()
        {
            return ShowDefaultTryoutDialog();
        }
    }
}