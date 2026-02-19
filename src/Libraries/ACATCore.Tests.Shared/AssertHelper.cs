////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AssertHelper.cs
//
// Enhanced assertion utilities for common test scenarios
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACATCore.Tests.Shared
{
    /// <summary>
    /// Provides enhanced assertion utilities beyond standard MSTest assertions
    /// </summary>
    public static class AssertHelper
    {
        /// <summary>
        /// Asserts that a collection contains exactly the expected items in any order
        /// </summary>
        public static void CollectionContainsExactly<T>(IEnumerable<T> actual, params T[] expected)
        {
            var actualList = actual.ToList();
            var expectedList = expected.ToList();

            if (actualList.Count != expectedList.Count)
            {
                Assert.Fail($"Expected collection to have {expectedList.Count} items but found {actualList.Count}");
            }

            foreach (var item in expectedList)
            {
                if (!actualList.Contains(item))
                {
                    Assert.Fail($"Expected collection to contain {item} but it was not found");
                }
            }
        }

        /// <summary>
        /// Asserts that a collection contains all expected items (may have additional items)
        /// </summary>
        public static void CollectionContainsAll<T>(IEnumerable<T> actual, params T[] expected)
        {
            var actualList = actual.ToList();
            foreach (var item in expected)
            {
                if (!actualList.Contains(item))
                {
                    Assert.Fail($"Expected collection to contain {item} but it was not found");
                }
            }
        }

        /// <summary>
        /// Asserts that a collection does not contain any of the specified items
        /// </summary>
        public static void CollectionDoesNotContain<T>(IEnumerable<T> actual, params T[] notExpected)
        {
            var actualList = actual.ToList();
            foreach (var item in notExpected)
            {
                if (actualList.Contains(item))
                {
                    Assert.Fail($"Expected collection not to contain {item} but it was found");
                }
            }
        }

        /// <summary>
        /// Asserts that a string contains a substring (case insensitive)
        /// </summary>
        public static void StringContains(string actual, string expectedSubstring, bool ignoreCase = true)
        {
            if (actual == null)
            {
                Assert.Fail("Actual string is null");
            }

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (actual.IndexOf(expectedSubstring, comparison) < 0)
            {
                Assert.Fail($"Expected string to contain '{expectedSubstring}' but actual was '{actual}'");
            }
        }

        /// <summary>
        /// Asserts that a string does not contain a substring
        /// </summary>
        public static void StringDoesNotContain(string actual, string notExpectedSubstring, bool ignoreCase = true)
        {
            if (actual == null)
            {
                return; // Null string doesn't contain anything
            }

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (actual.IndexOf(notExpectedSubstring, comparison) >= 0)
            {
                Assert.Fail($"Expected string not to contain '{notExpectedSubstring}' but it was found in '{actual}'");
            }
        }

        /// <summary>
        /// Asserts that a string starts with expected prefix
        /// </summary>
        public static void StringStartsWith(string actual, string expectedPrefix, bool ignoreCase = true)
        {
            if (actual == null)
            {
                Assert.Fail("Actual string is null");
            }

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!actual.StartsWith(expectedPrefix, comparison))
            {
                Assert.Fail($"Expected string to start with '{expectedPrefix}' but actual was '{actual}'");
            }
        }

        /// <summary>
        /// Asserts that a string ends with expected suffix
        /// </summary>
        public static void StringEndsWith(string actual, string expectedSuffix, bool ignoreCase = true)
        {
            if (actual == null)
            {
                Assert.Fail("Actual string is null");
            }

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!actual.EndsWith(expectedSuffix, comparison))
            {
                Assert.Fail($"Expected string to end with '{expectedSuffix}' but actual was '{actual}'");
            }
        }

        /// <summary>
        /// Asserts that a value is within a specified range
        /// </summary>
        public static void InRange<T>(T actual, T min, T max) where T : IComparable<T>
        {
            if (actual.CompareTo(min) < 0 || actual.CompareTo(max) > 0)
            {
                Assert.Fail($"Expected value to be between {min} and {max} but was {actual}");
            }
        }

        /// <summary>
        /// Asserts that a DateTime is close to expected within tolerance
        /// </summary>
        public static void DateTimeClose(DateTime actual, DateTime expected, TimeSpan tolerance)
        {
            var diff = (actual - expected).Duration();
            if (diff > tolerance)
            {
                Assert.Fail($"Expected DateTime to be within {tolerance} of {expected} but was {actual} (difference: {diff})");
            }
        }

        /// <summary>
        /// Asserts that a double is close to expected within tolerance
        /// </summary>
        public static void DoubleClose(double actual, double expected, double tolerance = 0.0001)
        {
            var diff = Math.Abs(actual - expected);
            if (diff > tolerance)
            {
                Assert.Fail($"Expected double to be within {tolerance} of {expected} but was {actual} (difference: {diff})");
            }
        }

        /// <summary>
        /// Asserts that all items in collection satisfy a predicate
        /// </summary>
        public static void All<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            var items = collection.ToList();
            var failedItems = items.Where(item => !predicate(item)).ToList();

            if (failedItems.Any())
            {
                var errorMessage = message ?? $"Expected all items to satisfy predicate but {failedItems.Count} items failed";
                Assert.Fail(errorMessage);
            }
        }

        /// <summary>
        /// Asserts that any item in collection satisfies a predicate
        /// </summary>
        public static void Any<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            if (!collection.Any(predicate))
            {
                var errorMessage = message ?? "Expected at least one item to satisfy predicate but none did";
                Assert.Fail(errorMessage);
            }
        }

        /// <summary>
        /// Asserts that no items in collection satisfy a predicate
        /// </summary>
        public static void None<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            var matchingItems = collection.Where(predicate).ToList();
            if (matchingItems.Any())
            {
                var errorMessage = message ?? $"Expected no items to satisfy predicate but {matchingItems.Count} items did";
                Assert.Fail(errorMessage);
            }
        }
    }
}
