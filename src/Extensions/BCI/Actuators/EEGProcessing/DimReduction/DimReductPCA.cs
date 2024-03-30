////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DimReductPCA.cs
//
// Principle Component Analysis to reduce the
// dimensionality of a data set.  The level of reduction is determined
// by the user.  The numerical computation of the eigenvalues and
// vectors is done using the svd method.
//
////////////////////////////////////////////////////////////////////////////

using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class DimReductPCA
    {
        #region Public properties

        /// <summary>
        /// Determines whether to reduce by specified number of principle components or
        /// by specified level of their associated variances.
        /// Accepted values: 'threshold', 'firstNcomponents', 'minRelativeEigenvalue'
        /// </summary>
        public String componentSortMethod;

        /// <summary>
        /// number of principle components to reduce dimensionality with
        /// </summary>
        public int nComponents;

        /// <summary>
        /// Value to threshold variances by
        /// </summary>
        public float threshold;

        /// <summary>
        /// Value of the min eigenvalues (this determine how many compoenents are selected)
        /// </summary>
        public double minEigenValue;

        /// <summary>
        /// PCA object from Accord that contains the eigenvectors, eigenvalues, etc...
        /// </summary>
        private readonly List<PrincipalComponentAnalysis> chPCAObj;

        /// <summary>
        /// Projection matrix (for each channel)
        /// </summary>
        private List<double[,]> projectionMatrix;

        #endregion Public properties

        /// <summary>
        /// Constructor for DimReductPCA object using first N number of components
        /// </summary>
        /// <param name="pComponentSortedMethod"></param>
        /// <param name="numberOfComponents"></param>
        public DimReductPCA(String pComponentSortedMethod, int numberOfComponents)
        {
            // Create PCA object with numberOfComponents as a method
            if (String.Equals(pComponentSortedMethod, "firstNcomponents"))
            {
                chPCAObj = new List<PrincipalComponentAnalysis>();
                componentSortMethod = pComponentSortedMethod;
                nComponents = numberOfComponents;
            }
            else
            {
                throw new Exception(" incorrect input for this PCA method");
            }
        }

        /// <summary>
        /// Constructor for DimReductPCA object using threshold to select the first N components
        /// </summary>
        /// <param name="componentSortedMethod"></param>
        /// <param name="threshold"></param>
        public DimReductPCA(String pComponentSortedMethod, float pThreshold)
        {
            if (String.Equals(pComponentSortedMethod, "threshold"))
            {
                chPCAObj = new List<PrincipalComponentAnalysis>();
                componentSortMethod = pComponentSortedMethod;
                threshold = pThreshold;
            }
            else
            {
                throw new Exception(" incorrect input for this PCA method");
            }
        }

        /// <summary>
        /// Constructor for DimReductPCA object using threshold to select the first N components
        /// </summary>
        /// <param name="componentSortedMethod"></param>
        /// <param name="threshold"></param>
        public DimReductPCA(String pComponentSortedMethod, double pMinEigenValue)
        {
            if (String.Equals(pComponentSortedMethod, "minRelativeEigenvalue"))
            {
                chPCAObj = new List<PrincipalComponentAnalysis>();
                componentSortMethod = pComponentSortedMethod;
                minEigenValue = pMinEigenValue;
            }
            else
            {
                throw new Exception(" incorrect input for this PCA method");
            }
        }

        /// <summary>
        /// Learn PCA
        /// </summary>
        /// <param name="inputData"></param>
        public void Learn(List<double[,]> inputData)
        {
            int numTrials = inputData.Count();
            int numFeatures = inputData[0].GetLength(0);
            int numChannels = inputData[0].GetLength(1);

            double[][] chData = new double[numTrials][]; //[trials][samples]
            this.projectionMatrix = new List<double[,]>();
            // Apply PCA to each channel
            for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
            {
                // Get data for the channel
                for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                {
                    chData[trialIdx] = Matrix.GetColumn(inputData[trialIdx], channelIdx); //chData[trialIdx][featureIdx]
                }

                // Remove mean
                double[] means = chData.Mean(0);
                double[,] zeroMeanChData = chData.Center(means).ToMatrix();
                //double[,] zeroMeanChData = Elementwise.S chData Subtract(means).ToMatrix();

                var m = Matrix.Dot(zeroMeanChData.Transpose(), zeroMeanChData); //covariance

                // Calculate eigenvalues and eigenvectors
                var EigenValueDecompObj = new EigenvalueDecomposition(m, false, false);
                double[,] eigenVectors = EigenValueDecompObj.Eigenvectors;
                double[] eigenValues = EigenValueDecompObj.RealEigenvalues;

                // Calculate the number of compoenents depending on method
                int nStartComponent = 0;
                switch (componentSortMethod)
                {
                    case "firstNcomponents":
                        nStartComponent = numFeatures - nComponents;
                        break;

                    case "threshold": // This case has not been tested.
                        double[] cumProp = eigenValues.CumulativeSum(); //can be divided by 100 (not tested)
                        double value = Array.Find(cumProp, element => element > threshold);
                        nComponents = Array.IndexOf(cumProp, value);
                        nStartComponent = 1;
                        break;

                    case "minRelativeEigenvalue":
                        double thresholdEigen = minEigenValue * eigenValues[eigenValues.Length - 1];
                        double minValue = Array.Find(eigenValues, element => element >= thresholdEigen);
                        nStartComponent = Array.IndexOf(eigenValues, minValue);
                        break;
                }

                // Calculate projectionMatrix
                int[] indSelectedEigenValues = Vector.Range(nStartComponent, numFeatures);
                //indSelectedEigenValues = indSelectedEigenValues.Subtract(-numFeatures + 1);
                double[,] chProjectionMatrix = eigenVectors.GetColumns(indSelectedEigenValues).Transpose();

                this.projectionMatrix.Add(chProjectionMatrix);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="reducedData"></param>
        public void Reduce(List<double[,]> inputData, out List<double[]> reducedData)
        {
            int numTrials = inputData.Count();
            int numChannels = inputData[0].GetLength(1);
            reducedData = new List<double[]>();
            double[][] matrixReducedData = new double[numTrials][];

            for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
            {
                double[][] chData = new double[numTrials][]; //[trials][samples]
                // Get data for the channel
                for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                {
                    chData[trialIdx] = Matrix.GetColumn(inputData[trialIdx], channelIdx); //chData[trialIdx][featureIdx]
                }

                // Calculate reduced features for the channel
                var tmp = chData.ToMatrix().Transpose();
                double[,] reducedFeaturesChannel = this.projectionMatrix[channelIdx].Dot(tmp); //[numOutputComponents, numTrials]

                for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                {
                    // Concatenate data;
                    double[] chDataConcatenated = reducedFeaturesChannel.GetColumn(trialIdx).Concatenate();

                    if (channelIdx == 0)
                        matrixReducedData[trialIdx] = chDataConcatenated;
                    else
                        matrixReducedData[trialIdx] = Matrix.Concatenate(matrixReducedData[trialIdx], chDataConcatenated);
                }
            }
            reducedData = matrixReducedData.ToList();
        }
    }
}