////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extension;
using Xunit;

namespace ACATExtension.Tests
{
    /// <summary>
    /// Sample unit tests for ACATPreferences demonstrating xUnit patterns.
    /// </summary>
    public class ACATPreferencesTests
    {
        [Fact]
        public void LoadDefaultSettings_ReturnsNonNullInstance()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.NotNull(preferences);
        }

        [Fact]
        public void LoadDefaultSettings_ClearTalkWindowOnTypeModeChange_DefaultIsTrue()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.True(preferences.ClearTalkWindowOnTypeModeChange);
        }

        [Fact]
        public void LoadDefaultSettings_SpeakOnEnterKey_DefaultIsTrue()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.True(preferences.SpeakOnEnterKey);
        }

        [Fact]
        public void LoadDefaultSettings_WordPredictionCount_DefaultIsTen()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.Equal(10, preferences.WordPredictionCount);
        }

        [Fact]
        public void LoadDefaultSettings_ScreenLockPin_DefaultIsSet()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.Equal("5143", preferences.ScreenLockPin);
        }

        [Fact]
        public void LoadDefaultSettings_StripScannerColumnIterations_DefaultIsTwo()
        {
            var preferences = ACATPreferences.LoadDefaultSettings();

            Assert.Equal(2, preferences.StripScannerColumnIterations);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(20)]
        public void ACATPreferences_WordPredictionCount_CanBeSetToValidValues(int count)
        {
            var preferences = ACATPreferences.LoadDefaultSettings();
            preferences.WordPredictionCount = count;

            Assert.Equal(count, preferences.WordPredictionCount);
        }
    }
}
