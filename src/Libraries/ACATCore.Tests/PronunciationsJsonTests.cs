////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using System.Collections.Generic;
using Xunit;

namespace ACATCore.Tests
{
    /// <summary>
    /// Sample unit tests for PronunciationsJson demonstrating xUnit patterns.
    /// </summary>
    public class PronunciationsJsonTests
    {
        [Fact]
        public void CreateDefault_ReturnsNonNullInstance()
        {
            var result = PronunciationsJson.CreateDefault();

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateDefault_ReturnsPronunciationsList()
        {
            var result = PronunciationsJson.CreateDefault();

            Assert.NotNull(result.Pronunciations);
            Assert.Empty(result.Pronunciations);
        }

        [Fact]
        public void PronunciationJson_DefaultWord_IsEmptyString()
        {
            var pronunciation = new PronunciationJson();

            Assert.Equal(string.Empty, pronunciation.Word);
            Assert.Equal(string.Empty, pronunciation.Pronunciation);
        }

        [Theory]
        [InlineData("cache", "kash")]
        [InlineData("queue", "kyoo")]
        [InlineData("colonel", "ker-nel")]
        public void PronunciationJson_CanSetWordAndPronunciation(string word, string pronunciation)
        {
            var entry = new PronunciationJson
            {
                Word = word,
                Pronunciation = pronunciation
            };

            Assert.Equal(word, entry.Word);
            Assert.Equal(pronunciation, entry.Pronunciation);
        }

        [Fact]
        public void PronunciationsJson_CanAddEntries()
        {
            var pronunciations = new PronunciationsJson
            {
                Pronunciations = new List<PronunciationJson>
                {
                    new PronunciationJson { Word = "cache", Pronunciation = "kash" },
                    new PronunciationJson { Word = "queue", Pronunciation = "kyoo" }
                }
            };

            Assert.Equal(2, pronunciations.Pronunciations.Count);
        }
    }
}
