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
    /// Sample unit tests for AbbreviationsJson demonstrating xUnit patterns.
    /// </summary>
    public class AbbreviationsJsonTests
    {
        [Fact]
        public void CreateDefault_ReturnsNonNullInstance()
        {
            var result = AbbreviationsJson.CreateDefault();

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateDefault_ReturnsEmptyAbbreviationsList()
        {
            var result = AbbreviationsJson.CreateDefault();

            Assert.NotNull(result.Abbreviations);
            Assert.Empty(result.Abbreviations);
        }

        [Fact]
        public void AbbreviationJson_DefaultMode_IsWrite()
        {
            var abbreviation = new AbbreviationJson();

            Assert.Equal("Write", abbreviation.Mode);
        }

        [Theory]
        [InlineData("brb", "be right back")]
        [InlineData("omw", "on my way")]
        [InlineData("thx", "thanks")]
        public void AbbreviationJson_WordAndReplaceWith_CanBeSet(string word, string replaceWith)
        {
            var abbreviation = new AbbreviationJson
            {
                Word = word,
                ReplaceWith = replaceWith
            };

            Assert.Equal(word, abbreviation.Word);
            Assert.Equal(replaceWith, abbreviation.ReplaceWith);
        }

        [Fact]
        public void AbbreviationsJson_CanAddAbbreviations()
        {
            var abbreviations = new AbbreviationsJson
            {
                Abbreviations = new List<AbbreviationJson>
                {
                    new AbbreviationJson { Word = "brb", ReplaceWith = "be right back" },
                    new AbbreviationJson { Word = "omw", ReplaceWith = "on my way" }
                }
            };

            Assert.Equal(2, abbreviations.Abbreviations.Count);
        }
    }
}
