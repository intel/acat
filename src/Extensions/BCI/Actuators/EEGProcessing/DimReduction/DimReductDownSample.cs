////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DimReductDownsample.cs
//
// handles downsampling of data
//
////////////////////////////////////////////////////////////////////////////

using Accord.Math;
using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class DimReductDownSample
    {
        /// <summary>
        /// Downsample rate
        /// </summary>
        public readonly int downsampleRate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pSampleRate"></param>
        /// <param name="pDownsampleRate"></param>
        public DimReductDownSample(int pDownsampleRate)
        {
            downsampleRate = pDownsampleRate;
        }

        /// <summary>
        /// Learn: for this class, learn does not require any operation
        /// </summary>
        /// <param name="inputData"></param>
        public bool Learn()
        {
            // Do nothing
            return true;
        }

        /// <summary>
        /// Reduce: Otputs the data for the selected channels
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="channelSubset"></param>
        /// <param name="outputData"></param>
        public bool Reduce(List<double[,]> inputData, out List<double[,]> outputData)
        {
            int numTrials = inputData.Count;
            outputData = new List<double[,]>();

            double[,] thisTrialData;

            if (downsampleRate >= 1 && numTrials > 0)
            {
                double[,] reducedData;

                // Generate vector of subsamples
                for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                {
                    thisTrialData = inputData[trialIdx];
                    int numSamples = thisTrialData.GetLength(0);
                    int numChannels = thisTrialData.GetLength(1);

                    int[] indicesChannels = Vector.Range(0, numChannels);

                    List<int> indicesSamples = new List<int>();
                    for (int i = 0; i < numSamples; i += downsampleRate)
                        indicesSamples.Add(i);

                    reducedData = Matrix.Get(thisTrialData, indicesSamples.ToArray(), indicesChannels);
                    outputData.Add(reducedData);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}