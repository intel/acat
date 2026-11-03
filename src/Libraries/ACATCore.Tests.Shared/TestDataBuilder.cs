////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TestDataBuilder.cs
//
// Utilities for building test data with builder pattern
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACATCore.Tests.Shared
{
    /// <summary>
    /// Base class for test data builders using fluent builder pattern
    /// </summary>
    /// <typeparam name="T">Type of object to build</typeparam>
    public abstract class TestDataBuilder<T>
    {
        /// <summary>
        /// Builds the object with configured values
        /// </summary>
        public abstract T Build();

        /// <summary>
        /// Builds the object and resets builder to default state
        /// </summary>
        public T BuildAndReset()
        {
            var result = Build();
            Reset();
            return result;
        }

        /// <summary>
        /// Resets builder to default state
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Builds multiple objects with configured values
        /// </summary>
        public List<T> BuildMany(int count)
        {
            var results = new List<T>();
            for (int i = 0; i < count; i++)
            {
                results.Add(Build());
            }
            return results;
        }
    }

    /// <summary>
    /// Provides common test data generation utilities
    /// </summary>
    public static class TestDataGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a random string of specified length
        /// </summary>
        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }
            return new string(result);
        }

        /// <summary>
        /// Generates a random integer within specified range
        /// </summary>
        public static int RandomInt(int min = 0, int max = 100)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Generates a random boolean
        /// </summary>
        public static bool RandomBool()
        {
            return _random.Next(2) == 1;
        }

        /// <summary>
        /// Generates a random GUID string
        /// </summary>
        public static string RandomGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generates a random date within specified range
        /// </summary>
        public static DateTime RandomDate(DateTime? start = null, DateTime? end = null)
        {
            var startDate = start ?? new DateTime(2020, 1, 1);
            var endDate = end ?? DateTime.Now;
            int range = (endDate - startDate).Days;
            return startDate.AddDays(_random.Next(range));
        }

        /// <summary>
        /// Selects a random item from an array
        /// </summary>
        public static T RandomItem<T>(params T[] items)
        {
            if (items == null || items.Length == 0)
                throw new ArgumentException("Items array cannot be null or empty");
            return items[_random.Next(items.Length)];
        }

        /// <summary>
        /// Generates a list of random items using a generator function
        /// </summary>
        public static List<T> RandomList<T>(Func<T> generator, int count)
        {
            var results = new List<T>();
            for (int i = 0; i < count; i++)
            {
                results.Add(generator());
            }
            return results;
        }
    }
}
