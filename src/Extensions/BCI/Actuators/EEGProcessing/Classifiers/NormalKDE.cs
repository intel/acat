////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// NormalKDE.cs
//
// Normal Kernel-Densisty Estimation
//
////////////////////////////////////////////////////////////////////////////

using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Distributions.Univariate;
using System;
using System.Drawing;
using System.Linq;
using ZedGraph;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class NormalKDE
    {
        // Sigma of normal distribution fitted on each data point for KDE
        public double kernelWidth;

        /// <summary>
        /// [numSamples] data to calculate its kernel density distribution (eg: vector of scores)
        /// </summary>
        public double[] data2fit;

        /// <summary>
        /// Constructor, no params needed
        /// </summary>
        public NormalKDE()
        {
        }

        /// <summary>
        /// Builds KDE from data
        /// </summary>
        /// <param name="data"></param>
        public void BuildKDE(double[] data)
        {
            // Calculate Silverman's bandwidth “silverman” - .9 * A * nobs ** (-1/5.), where A is min(std(X),IQR/1.34)

            // Calculate iqr: range from the 25th percentile to the 75th percentile, or midlle 50 percent of a data set
            double q1, q3;
            Measures.Quartiles(data, out q1, out q3, false);
            double iqr = q3 - q1;

            double minValue = Math.Min(data.StandardDeviation(), iqr / 1.34);

            int numObs = data.Length;
            kernelWidth = 1.06 * minValue * Math.Pow(numObs, -0.2);

            data2fit = data;
        }

        /// <summary>
        /// Calcualtes the probabilities using the kde fitted on the data2fit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] CalculateProbabilities(double[] data)
        {
            int numSamples = data.Length;
            double[] probs = new double[numSamples];

            for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
            {
                //Calculate p(xi_=sum(normpdf(data(sampleIdx], data2fit, kernelWidth);
                double sum = 0.0;
                for (int fitSampleIdx = 0; fitSampleIdx < data2fit.Length; fitSampleIdx++)
                {
                    // Build NormPDF for each sample
                    NormalDistribution NormalDistributionObj = new NormalDistribution(mean: data2fit[fitSampleIdx], stdDev: kernelWidth);
                    double v = NormalDistributionObj.ProbabilityDensityFunction(data[sampleIdx]);
                    sum += v;
                }

                // Normalize and save
                probs[sampleIdx] = sum / data2fit.Length;
            }

            return probs;
        }

        /// <summary>
        /// Generates samples from learned KDEs
        /// </summary>
        /// <param name="numSamples"></param>
        /// <returns></returns>
        public double[] generateSamples(int numSamples)
        {
            NormalDistribution NormalDistributionObj = new NormalDistribution(mean: 0, stdDev: 1);
            double[] gaussianRandomSamples = NormalDistributionObj.Generate(numSamples);

            int[] indices = UniformDiscreteDistribution.Random(0, data2fit.Length, numSamples);

            return gaussianRandomSamples.Multiply(kernelWidth).Add(data2fit.Get(indices));
        }

        /// <summary>
        /// Generates a single sample from the learned KDEs
        /// </summary>
        /// <returns></returns>
        public double generateSample()
        {
            return generateSamples(1)[0];
        }

        public void plotKDE(double[] data, int nSamples)
        {
            nSamples = 200;
            double bufferx = 2 * Measures.StandardDeviation(data);
            double xMin = data.Min() - bufferx;
            double xMax = data.Max() + bufferx;
            double stepSize = (xMax - xMin) / nSamples;
            double[] xVector = Vector.Range(xMin, xMax, stepSize);

            BuildKDE(data);
            double[] nonTargetPDF = CalculateProbabilities(xVector);
        }

        /// <summary>
        /// Test function: generates data from two gaussian distributions, learn KDEs and display them.
        /// </summary>
        /// <param name="args"></param>
        public static void RunTest()
        {
            // Generate normal gaussian samples
            int numSamples = 100;
            NormalDistribution normalPDFObj1 = new NormalDistribution(mean: -2, stdDev: 0.5);
            NormalDistribution normalPDFObj2 = new NormalDistribution(mean: 2, stdDev: 1.5);
            double[] samplesPDF1 = normalPDFObj1.Generate(numSamples);
            double[] samplesPDF2 = normalPDFObj2.Generate(numSamples);

            double[] x = Vector.Range(-10.0, 10.0, 0.2);

            // Build KDE Objects
            NormalKDE KDEObj1 = new NormalKDE();
            NormalKDE KDEObj2 = new NormalKDE();
            KDEObj1.BuildKDE(samplesPDF1);
            KDEObj2.BuildKDE(samplesPDF2);

            // Calculate probabilities
            double[] pdfT = KDEObj1.CalculateProbabilities(x);
            double[] pdfNT = KDEObj2.CalculateProbabilities(x);

            GraphDisplayerForm1x2 graphForm = new GraphDisplayerForm1x2();
            GraphPane graphPaneObjRight = graphForm.graphControlRight.GraphPane;

            // Add datapoints for display
            PointPairList graphTargetPoints = new PointPairList();
            PointPairList graphNontargetPoints = new PointPairList();
            for (int i = 0; i < pdfT.Length; i++)
            {
                graphNontargetPoints.Add(x[i], pdfNT[i]);
                graphTargetPoints.Add(x[i], pdfT[i]);
            }

            // For plotting: Add datapoints to curve
            graphPaneObjRight.AddCurve("Class 0", graphNontargetPoints, Color.Blue, SymbolType.None);
            graphPaneObjRight.AddCurve("Class 1", graphTargetPoints, Color.Red, SymbolType.None);

            // Set graph params
            graphPaneObjRight.AxisChange();
            graphPaneObjRight.Title.Text = "KDE estimates";
            graphPaneObjRight.XAxis.Title.Text = "x";
            graphPaneObjRight.YAxis.Title.Text = "p(x)";

            graphForm.ShowDialog();

            graphForm.Dispose();
        }
    }
}