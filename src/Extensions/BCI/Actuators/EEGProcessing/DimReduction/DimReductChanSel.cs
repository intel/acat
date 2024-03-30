////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DimReductChanSel.cs
//
// Handles reducing number of channels from data
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class DimReductChanSel
    {
        // Subset of channels
        public int[] channelSubset;

        public DimReductChanSel(int[] pChannelSubset)
        {
            channelSubset = pChannelSubset;
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
            outputData = new List<double[,]>();

            try
            {
                int numTrials = inputData.Count();

                if (numTrials > 0)
                {
                    // If channelSubset > numChannels (Eg: receiving data from OpenBCI 8ch and susbset is for 16ch), disable extra channels
                    int maxChannels = inputData[0].GetLength(1);
                    List<int> newChannelSubset = new List<int>();
                    for (int channelIdx = 0; channelIdx < channelSubset.Length; channelIdx++)
                    {
                        if (channelSubset[channelIdx] <= maxChannels)
                            newChannelSubset.Add(channelSubset[channelIdx]);
                    }
                    channelSubset = newChannelSubset.ToArray();
                }

                int[] channelSubsetZero = channelSubset.Subtract(1); //data starts at idx 0 channelSubset 1

                double[,] thisTrialData;
                double[,] reducedData;

                // Generate vector of subset of channels
                for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                {
                    thisTrialData = inputData[trialIdx];
                    int numSamples = thisTrialData.GetLength(0);

                    int[] indicesSamples = Vector.Range(0, numSamples);

                    reducedData = Matrix.Get(thisTrialData, indicesSamples, channelSubsetZero);
                    outputData.Add(reducedData);
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
            return true;
        }
    }
}