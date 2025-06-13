////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Classifier Utils.cs
//
// Handles useful features for classifiers such as calculating AUC from ROC curve
//
////////////////////////////////////////////////////////////////////////////

using Accord.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public class ClassifierUtils
    {
        /// <summary>
        /// Calculates AUC given scores and true labels
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="trueLabels"></param>
        /// <param name="TPrate"></param>
        /// <param name="FPrate"></param>
        /// <returns></returns>
        public static float CalculateAUC(List<double> scores, int[] trueLabels, out double[] TPrate, out double[] FPrate)
        {
            float auc = 0;

            // Sort scores and rearrange labels according to sorted scores
            var sorted = scores.Select((x, i) => new KeyValuePair<double, int>(x, i))
                    .OrderBy(x => x.Key)
                    .Reverse()
                    .ToList();
            List<double> sortedScores = sorted.Select(x => x.Key).ToList();
            int[] indices = sorted.Select(x => x.Value).ToList().ToArray<int>();
            int[] sortedLabels = Matrix.Get(trueLabels, indices);

            // Calculated cumulative for true and false cases
            int[] cumTrue = sortedLabels.CumulativeSum();
#pragma warning disable CS0618
            int[] reversedLabels = Matrix.Abs(sortedLabels.Subtract(1)); //where 0 put 1 where 1 put 0
#pragma warning restore CS0618
            int[] cumFalse = reversedLabels.CumulativeSum();

            // Calculate true positive and false positive rates
            int numPoints = cumTrue.Length + 1;
            int[] tmpTPrate = Matrix.Concatenate(0, cumTrue);
            int[] tmpFPrate = Matrix.Concatenate(0, cumFalse);

            // Normalize true positives and negatives between 0 and 1
            int maxCumTrue = cumTrue[cumTrue.Length - 1];
            int maxCumFalse = cumFalse[cumFalse.Length - 1];
            TPrate = tmpTPrate.Divide(maxCumTrue);
            FPrate = tmpFPrate.Divide(maxCumFalse);

            // Calculate are under the curve (AUC) (equivalent to trapz(x,y) in Matlab)
            double integral = 0;
            for (int i = 1; i < TPrate.Length; i++)
                integral += (TPrate[i] + TPrate[i - 1]) / 2 * (FPrate[i] - FPrate[i - 1]);

            auc = (float)integral;
            auc = (float)Math.Round(auc * 100) / 100; // 2 digits

            return auc;
        }
    }
}