////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Onboarding
{
    /// <summary>
    /// Holds the ID of the extension that is a part of
    /// the onboarding process
    /// </summary>
    [Serializable]
    public class OnboardingSequenceItem
    {
        public OnboardingSequenceItem()
        {
            Id = Guid.Empty;
        }

        public OnboardingSequenceItem(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}