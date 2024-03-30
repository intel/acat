////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DimReductRDA.cs
//
// Regularized Discriminant Analysis to reduce the
// dimensionality of a data set.
//
////////////////////////////////////////////////////////////////////////////

using Accord.Math;
using Accord.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class DimReductRDA
    {
        /// <summary>
        /// [numFeatures x numClasses] projection matrix
        /// </summary>
        public List<double[,]> W;

        /// <summary>
        /// [numFeatures x 1] Linear term for the projection
        /// </summary>
        public List<double[]> w;

        /// <summary>
        /// Scalar, bias term needed to compute Bayesian discriminant function
        /// w0 = -(1/2)*mu_i'*sigma^-1*mu_i + ln(P(w_i))
        /// </summary>
        public List<double> w0;

        /// <summary>
        /// scalar, parameter for shrinkage: gets closer to common covariance
        /// Ssi = (1 - shrinkage)*Si + shrinkage*Stotal;
        /// </summary>
        public double shrinkParam;

        /// <summary>
        /// Scalar, parameter for regularization: gets closer to diagonal covariance
       // Ci = (1 - regularize)*Ci + regularize * diag(Ci);
        /// </summary>
        public double regularizeParam;

        /// <summary>
        /// [numClasses x1] distributions of classes a priori
        /// </summary>
        //public double[] priors;

        /// <summary>
        /// Scalar, number of trained classses
        /// </summary>
        public int numTrainedClasses;

        private double[][] means; //[class][means D dimensions]
        private double[][,] inverseCovariances; //[class] [covarianceMatrix DxD dimensions]
        private double[] logDeterminantOfCovariances; //[class]
        private double[] priors; //[class]

        /// <summary>
        /// Constructor,  no params
        /// </summary>
        public DimReductRDA()
        {
            Clear();
            shrinkParam = 1F;
            regularizeParam = 1F;
        }

        /// <summary>
        /// Constructor with reduced mode, shrinkage and regularization
        /// </summary>
        /// <param name="pReduceMode"></param>
        /// <param name="pShrinkParam"></param>
        /// <param name="pRegularizeParam"></param>
        public DimReductRDA(double pShrinkParam, double pRegularizeParam)
        {
            Clear();

            shrinkParam = pShrinkParam;
            regularizeParam = pRegularizeParam;
        }

        /// <summary>
        /// Computes the quadratic projection y = x^tWi*x + wi^t*x + wi0 with class dependent
        ///  Wi, wi, and wi0 according to the quadratic discriminant analysis formula
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="priorMode"></param>
        /// <param name="pPriors"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public Boolean Learn(List<double[]> inputData, List<int> labels)//, string priorMode, float[] pPriors, int k)
        {
            //Debug.WriteLine("DimReductRDA k=" + k + "learn");

            // Clear any param
            Clear();

            // Initialize params
            int numClasses = 2;
            this.numTrainedClasses = numClasses;
            int[] numTrialsPerClass = new int[numClasses];
            means = new double[numClasses][];
            inverseCovariances = new double[numClasses][,];
            logDeterminantOfCovariances = new double[numClasses];
            priors = new double[numClasses];

            int numFeatures = inputData[0].Length;
            List<double[]> meank = new List<double[]>(); //mean for class k
            List<double[,]> Sk = new List<double[,]>(); //covariance for class k
            double[,] S = new double[numFeatures, numFeatures]; // All classes

            this.priors = new double[numClasses];
            int numTotalTrials = 0;
            for (int classIdx = 0; classIdx < numClasses; classIdx++)
            {
                //Separate in targets and non-targets
                int[] indClass = Matrix.Find(labels.ToArray(), element => element == classIdx);
                List<double[]> trialClassData = Matrix.Get(inputData, indClass); //<trial0>[sample0, sample1, sample2...], <trial1[sample0, sample1, sample2]...
                numTrialsPerClass[classIdx] = indClass.Length;
                numTotalTrials += indClass.Length;

                Stopwatch sw = new Stopwatch();
                sw.Start();
                //var tmpClassData = trialClassData.; // (1, ;
                double[,] classData = trialClassData.ToArray().ToMatrix();
                //double[] classData = tmpClassData2.GetColumn(1);
                sw.Stop();
                long t = sw.ElapsedMilliseconds;

                double[] classMean = classData.Mean(0);
                double[,] classZeroMeanData = classData.Center(classMean, false);
                double[,] classS = Matrix.Dot(classZeroMeanData.Transpose(), classZeroMeanData);

                // Save class means and covariances
                Sk.Add(classS); // Append to list
                means[classIdx] = classMean;
                //meank.Add(classMean); //Append to list

                // Calculate total covariance
                S = S.Add(classS);//matrix addition
            }

            int N = Matrix.Sum(numTrialsPerClass); // num total samples

            // Calculate priors (proportional to number of samples of each class)
            if (N == 0)
            {
                for (int index = 0; index < numClasses; index++)
                {
                    this.priors[index] = 0.0f;
                }
            }
            else
            {
                this.priors = numTrialsPerClass.Divide(N);
            }

            double[] Nk = new double[numClasses];//= numTrialsPerClass.Multiply(1 - this.shrinkParam).Add(this.shrinkParam*N);
            inverseCovariances = new double[numClasses][,];
            logDeterminantOfCovariances = new double[numClasses];

            for (int classIdx = 0; classIdx < numClasses; classIdx++)
            {
                // Calculate Nk term
                Nk[classIdx] = (1 - shrinkParam) * numTrialsPerClass[classIdx] + shrinkParam * numTotalTrials;

                // Calculate shrinked covariance
                double[,] tmpS1 = Sk[classIdx].Multiply(1 - this.shrinkParam); //Matrix.Multiply(S[classIdx],  1-shrinkParam);
                double[,] tmpS2 = S.Multiply(this.shrinkParam);
                double[,] shrinkedCov = tmpS1.Add(tmpS2);
                shrinkedCov = shrinkedCov.Divide(Nk[classIdx]);

                // Calculate matrix for QR decomposition
                double[,] tmpR1 = shrinkedCov.Multiply(1 - this.regularizeParam);
                double coeff = (shrinkedCov.Trace() * this.regularizeParam) / numFeatures;
                double[,] tmpR2 = Accord.Math.Matrix.Identity(numFeatures).Multiply(coeff);
                double[,] regCov = tmpR1.Add(tmpR2);

                // Calculate inverse covariance (methd B)
                double[,] classInvSigma = Matrix.Inverse(regCov); //(Qt, R);
                this.inverseCovariances[classIdx] = classInvSigma;

                // Calculate log determinant using built-in function
                this.logDeterminantOfCovariances[classIdx] = Matrix.LogDeterminant(regCov);
            }

            return true;
        }

        /// <summary>
        /// Reduce dimensions via quadratic projection using the learned params.
        /// This returns the vector in "full mode"
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="outputData"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public Boolean Reduce(List<double[]> inputData, out List<double[]> outputData)
        {
            //int numTrials = inputData.Count;
            //int numSamples = inputData[0].Length;
            int numClasses = this.means.Length;

            List<double[]> classProjections = new List<double[]>();
            for (int classIdx = 0; classIdx < numClasses; classIdx++)
            {
                double[,] classData = inputData.ToArray().ToMatrix();
                double[] classMean = this.means[classIdx];
                double[,] classInvCov = this.inverseCovariances[classIdx];
                double classLogDeterminantOfCov = this.logDeterminantOfCovariances[classIdx];
                double classPrior = priors[classIdx]; ;

                // Substract mean
                double[,] classZeroMeanData = classData.Center(classMean, false);

                // Calculate terms for projection
                double[,] tmp1 = Matrix.Dot(classZeroMeanData, classInvCov);
                tmp1 = Matrix.ElementwiseMultiply(tmp1, classZeroMeanData);
                double[] term1 = Matrix.Sum(tmp1, 1);

                term1 = term1.Add(classLogDeterminantOfCov);
                term1 = term1.Divide(-2);

                double logPrior = Math.Log(classPrior);

                double[] classProjection = term1.Add(logPrior);

                classProjections.Add(classProjection);
            }
            outputData = classProjections;
            return true;
        }

        /// <summary>
        /// Reduce data in "reduce" mode (only valid for two classes as positive class and negative and substracted)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="outputData"></param>
        /// <returns></returns>
        public Boolean Reduce(List<double[]> inputData, out List<double> outputData)
        {
            Reduce(inputData, out List<double[]> fullOutput); //[class][trial]
            outputData = new List<double>();

            int numClasses = this.means.Length;
            if (numClasses == 2)
            {
                outputData = fullOutput[1].Subtract(fullOutput[0]).ToList();
                return true;
            }
            else
            {
                throw new Exception("RDA: Function only available for 'reduced mode' and number of classes=2");
            }
        }

        /// <summary>
        /// Clear all params
        /// </summary>
        public Boolean Clear()
        {
            W = new List<double[,]>();
            w = new List<double[]>();
            w0 = new List<double>();
            numTrainedClasses = 0;
            priors = null;

            return true;
        }

        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}