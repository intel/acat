////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MatrixManinipulations.cs
//
// Handles all matrix manipulations to convert jagged matrix to 2d array and viceversa,
// concatenate matrices, etc
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGUtils
{
    public class MatrixManipulations
    {

        /// <summary>
        /// Concatenate data (data is formated in classes or groups)
        /// </summary>
        /// <param name="dataInClasses"></param>
        /// <returns></returns>
        public static List<List<double[]>> ConcatenateFeatures(List<List<double[,]>> dataInClasses)
        {
            List<List<double[]>> concatenatedData = new List<List<double[]>>();
            int numGroups = dataInClasses.Count;
            for (int groupIdx = 0; groupIdx < numGroups; groupIdx++)
            {
                int numTrials = dataInClasses[groupIdx].Count;
                List<double[]> trialData = new List<double[]>();
                for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                {
                    int numChannels = dataInClasses[groupIdx][trialIdx].GetLength(1);
                    int numSamples = dataInClasses[groupIdx][trialIdx].GetLength(0);
                    List<double> trialConcatenatedData = new List<double>();
                    for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                        for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
                            trialConcatenatedData.Add(dataInClasses[groupIdx][trialIdx][sampleIdx, channelIdx]);
                    trialData.Add(trialConcatenatedData.ToArray());
                }
                concatenatedData.Add(trialData);
            }
            return concatenatedData;
        }

        /// <summary>
        /// Convert jagged array to 2D array
        /// </summary>
        /// <param name="jaggedArray"></param>
        /// <returns></returns>
        public static double[,] Convert2BidimensionalArray(double[][] jaggedArray)
        {
            double[,] bidimensionalArray = new double[jaggedArray.Length, jaggedArray[0].Length];
            for (int i = 0; i < jaggedArray.Length; i++)
                for (int j = 0; j < jaggedArray[0].Length; j++)
                    bidimensionalArray[i, j] = jaggedArray[i][j];

            return bidimensionalArray;
        }

        /// <summary>
        /// Convert jagged list to 2D array
        /// </summary>
        /// <param name="jaggedArray"></param>
        /// <returns></returns>
        public static double[,] Convert2BidimensionalArray(List<List<double>> jaggedList)
        {
            int dim1 = jaggedList.Count;
            int dim2 = jaggedList[0].Count;

            double[,] bidimensionalArray = new double[dim1, dim2];
            for (int i = 0; i < dim1; i++)
                for (int j = 0; j < dim2; j++)
                    bidimensionalArray[i, j] = jaggedList[i][j];

            return bidimensionalArray;
        }

        /// <summary>
        /// Convert jagged array to 2D array
        /// </summary>
        /// <param name="jaggedArray"></param>
        /// <returns></returns>
        public static double[,] Convert2BidimensionalArray(List<double[]> jaggedList)
        {
            int dim1 = jaggedList.Count;
            int dim2 = jaggedList[0].Length;

            double[,] bidimensionalArray = new double[dim1, dim2];
            for (int i = 0; i < dim1; i++)
                for (int j = 0; j < dim2; j++)
                    bidimensionalArray[i, j] = jaggedList[i][j];

            return bidimensionalArray;
        }
    }
}