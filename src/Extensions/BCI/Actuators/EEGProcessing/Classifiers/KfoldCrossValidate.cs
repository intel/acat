////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KFoldCrossValidate.cs
//
// Handles cross validation operations
//
////////////////////////////////////////////////////////////////////////////

using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class KFoldCrossValidate
    {
        private readonly int nFolds = 10;
        private readonly String partitioningMethod = "sequential"; //Options: "sequential", "random"

        /// <summary>
        /// Constructor with default parameters (sequential, 10 folds)
        /// </summary>
        public KFoldCrossValidate()
        {
            nFolds = 10;
            partitioningMethod = "sequential";
        }

        /// <summary>
        /// Constructor with k folda and partitionmethod
        /// </summary>
        /// <param name="_K"></param>
        /// <param name="_partitioningMethod"></param>
        public KFoldCrossValidate(int _K, string _partitioningMethod)
        {
            nFolds = _K;

            if (_partitioningMethod.ToLower() == "sequential" || _partitioningMethod.ToLower() == "random")
                partitioningMethod = _partitioningMethod.ToLower();
            else
                throw new System.ArgumentException("Parameter" + _partitioningMethod + " not supported in crossValidation", "original");
        }

        /// <summary>
        /// Crossvalidate
        /// </summary>
        /// <param name="DimReductObj4Params"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>

        public List<double> CrossValidate(DimReductRDA DimReductObj4Params, List<double[]> data, List<int> labels)
        {
            int nTrials = labels.Count;

            // Split in folds
            int[] indicesFolds = SplitInFolds(labels);

            List<double> output = new List<double>();
            output.InsertRange(0, Vector.Zeros(nTrials));

            // Train & Test in parallel
            Parallel.For(0, nFolds, i =>
                {
                    // Generate train and test indices to split in folds
                    int[] indTrain = Matrix.Find(indicesFolds, element => element != i);
                    int[] indTest = Matrix.Find(indicesFolds, element => element == i);
                    List<double> kOutput;

                    DimReductRDA DimReductObj = new DimReductRDA(DimReductObj4Params.shrinkParam, DimReductObj4Params.regularizeParam);

                    //Train in K-1 folds (defined by indTrain)
                    DimReductObj.Learn(data.Get(indTrain), labels.Get(indTrain).ToList());

                    // Test in k fold (defined by indTest)
                    DimReductObj.Reduce(data.Get(indTest), out kOutput);

                    // Save test in output matrix
                    int j = 0;
                    for (int trialIdx = indTest[0]; trialIdx < indTest[indTest.Length - 1]; trialIdx++)
                    {
                        output[trialIdx] = kOutput[j];
                        j++;
                    }
                });

            return output.ToList();
        }

        /// <summary>
        /// Split in folds
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        private int[] SplitInFolds(List<int> labels)
        {
            //int numClasses = labels.ToArray().DistinctCount();
            int nTrials = labels.Count;

            // 1. Create partitions
            List<int> indicesFolds = new List<int>();
            switch (partitioningMethod)
            {
                case "sequential":
                    // generate indices
                    int nElementsFold = (int)Math.Floor((double)nTrials / (double)nFolds);
                    int extraElementFold = (int)nTrials % nFolds;

                    int endElementPrev = -1;
                    for (int k = 0; k < nFolds; k++)
                    {
                        int startElement = endElementPrev + 1;
                        int endElement = (k + 1) * nElementsFold - 1;

                        if (k < extraElementFold)
                            endElement = endElement + 1;

                        endElementPrev = endElement;

                        for (int i = startElement; i <= endElement; i++)
                            indicesFolds.Add(k);
                        //int[] indKfold = Vector.Interval(startElement, endElement);
                    }

                    break;

                case "random":
                    break;
            }
            return indicesFolds.ToArray();
        }
    }
}