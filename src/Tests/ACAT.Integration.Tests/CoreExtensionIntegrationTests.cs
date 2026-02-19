////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Extension;
using System.Collections.Generic;
using Xunit;

namespace ACAT.Integration.Tests
{
    /// <summary>
    /// Sample integration tests demonstrating xUnit patterns for
    /// interactions between ACATCore and ACATExtension.
    /// </summary>
    public class CoreExtensionIntegrationTests
    {
        [Fact]
        public void ACATPreferences_LoadDefaultSettings_ReturnsValidPreferences()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.NotNull(preferences);
        }

        [Fact]
        public void AbbreviationsJson_CreateDefault_IsCompatibleWithExtensionPreferences()
        {
            var abbreviations = AbbreviationsJson.CreateDefault();
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.NotNull(abbreviations);
            Assert.NotNull(preferences);
            Assert.Empty(abbreviations.Abbreviations);
        }

        [Fact]
        public void PronunciationsJson_CreateDefault_IsCompatibleWithExtensionPreferences()
        {
            var pronunciations = PronunciationsJson.CreateDefault();
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.NotNull(pronunciations);
            Assert.NotNull(preferences);
            Assert.Empty(pronunciations.Pronunciations);
        }

        [Fact]
        public void AbbreviationsJson_And_PronunciationsJson_CanBeUsedTogether()
        {
            var abbreviations = new AbbreviationsJson
            {
                Abbreviations = new List<AbbreviationJson>
                {
                    new AbbreviationJson { Word = "brb", ReplaceWith = "be right back" }
                }
            };

            var pronunciations = new PronunciationsJson
            {
                Pronunciations = new List<PronunciationJson>
                {
                    new PronunciationJson { Word = "ACAT", Pronunciation = "ay-kat" }
                }
            };

            Assert.Single(abbreviations.Abbreviations);
            Assert.Single(pronunciations.Pronunciations);
        }
    }
}
