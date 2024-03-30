////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Plots.cs
//
// Handles plotting of data in graphs
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using Accord.Math;
using Accord.Statistics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    internal static class Plots
    {
        /// <summary>
        /// List of colors to be used when plotting
        /// </summary>
        private static string[] ColorValues = new string[] { "#000000",  "#00FFFF", "#808080",
             "#FF00FF", "#800000", "#FF0000", "#808000", "#808080", "#FFFF00", "#008000",
             "#008080", "#000080", "#0000FF"," #800080", "#00FF00",  "#606000", "#000000"};

        /// <summary>
        /// Plot signal for specific channel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channelIdx"></param>
        /// <param name="sampleRate"></param>
        public static void plotSignal(double[,] data, int channelIdx, int sampleRate = 250)
        {
            GraphDisplayerForm2x1 graphFormSignal = new GraphDisplayerForm2x1();
            GraphPane graphPaneBottom = graphFormSignal.graphControlBottom.GraphPane;
            GraphPane graphPaneTop = graphFormSignal.graphControlTop.GraphPane;

            double[] chData = data.GetRow(channelIdx);
            int numChannels = data.GetLength(0);
            int numSamples = data.GetLength(1);
            double[] timeVector = new double[numSamples];

            for (int i = 0; i < numSamples; i++)
                timeVector[i] = i;

            graphPaneTop.AddCurve(channelIdx.ToString(), timeVector, chData, Color.Blue, SymbolType.None);
            graphPaneTop.AxisChange();

            graphFormSignal.ShowDialog();

            graphFormSignal.Dispose();
        }

        /// <summary>
        /// Plot trigger signal
        /// </summary>
        /// <param name="triggerSignal"></param>
        public static void plotTriggerSignal(int[] triggerSignal)
        {
            GraphDisplayerForm2x1 graphFormSignal = new GraphDisplayerForm2x1();
            GraphPane graphPaneBottom = graphFormSignal.graphControlBottom.GraphPane;
            GraphPane graphPaneTop = graphFormSignal.graphControlTop.GraphPane;

            int numSamples = triggerSignal.Length;
            double[] timeVector = new double[numSamples];

            for (int i = 0; i < numSamples; i++)
                timeVector[i] = i;

            graphPaneTop.AddCurve("Trigger", timeVector, triggerSignal.ToDouble(), Color.Blue, SymbolType.None);
            graphPaneTop.AxisChange();

            graphFormSignal.ShowDialog();

            graphFormSignal.Dispose();
        }

        /// <summary>
        /// Plot ERP data
        /// </summary>
        /// <param name="trialData"></param>
        /// <param name="trialTargetness"></param>
        /// <param name="channels"></param>
        /// <param name="auc"></param>
        /// <param name="sampleRate"></param>
        public static void plotERPs(List<double[,]> trialData, int[] trialTargetness, int[] channels, int sampleRate, string auc = "N/A")
        {
            // Separate data in targets and non-targets
            int[] indNontargets = Matrix.Find(trialTargetness, element => element == 0);
            int[] indTargets = Matrix.Find(trialTargetness, element => element == 1);

            List<double[,]> targets = Matrix.Get(trialData, indTargets);
            List<double[,]> nontargets = Matrix.Get(trialData, indNontargets);

            // Create Form
            GraphDisplayerForm2x1 graphFormERPs = new GraphDisplayerForm2x1();
            GraphPane graphPaneBottom = graphFormERPs.graphControlBottom.GraphPane;
            GraphPane graphPaneTop = graphFormERPs.graphControlTop.GraphPane;

            string title = "targets AUC:" + auc;

            // Calculate averages and display in graph
            diplaySignalInGraph(targets, channels, graphPaneTop, title, sampleRate);
            title = "non-targets AUC:" + auc;
            diplaySignalInGraph(nontargets, channels, graphPaneBottom, title, sampleRate);

            // Change y Axis to same values
            if (graphPaneTop.YAxis.Scale.Max > graphPaneBottom.YAxis.Scale.Max)
                graphPaneBottom.YAxis.Scale.Max = graphPaneTop.YAxis.Scale.Max;
            else
                graphPaneTop.YAxis.Scale.Max = graphPaneBottom.YAxis.Scale.Max;

            if (graphPaneTop.YAxis.Scale.Min < graphPaneBottom.YAxis.Scale.Min)
                graphPaneBottom.YAxis.Scale.Min = graphPaneTop.YAxis.Scale.Min;
            else
                graphPaneTop.YAxis.Scale.Min = graphPaneBottom.YAxis.Scale.Min;

            var form = Context.AppPanelManager.GetCurrentForm() as Form;
            graphFormERPs.ShowDialog(Context.AppPanelManager.GetCurrentForm() as Form);

            graphFormERPs.Dispose();
        }

        /// <summary>
        /// Plot class distributions
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="trialTargetness"></param>
        /// <param name="graphPane"></param>
        public static void plotClassDistributions(List<double> scores, int[] trialTargetness, GraphPane graphPane = null)
        {
            GraphDisplayerForm1x2 graphForm = null;
            if (graphPane == null)
            {
                graphForm = new GraphDisplayerForm1x2();
                graphPane = graphForm.graphControlLeft.GraphPane;
            }

            //Separate in targets and non-targets
            int[] indNonTarget = Matrix.Find(trialTargetness, element => element == 0);
            List<double> nonTargetScores = Matrix.Get(scores, indNonTarget); //<trial0>[sample0, sample1, sample2...], <trial1[sample0, sample1, sample2]...

            int[] indTarget = Matrix.Find(trialTargetness, element => element == 1);
            List<double> targetScores = Matrix.Get(scores, indTarget); //<trial0>[sample0, sample1, sample2...], <trial1[sample0, sample1, sample2]...

            // Find lim KDEs
            int nKDEsamples = 200;
            double bufferx = 2 * Measures.StandardDeviation(scores.ToArray());
            double xMin = scores.Min() - bufferx;
            double xMax = scores.Max() + bufferx;
            double stepSize = (xMax - xMin) / nKDEsamples;
            double[] xVector = Vector.Range(xMin, xMax, stepSize);

            // Construct KDEs for target and non-target
            NormalKDE KDENonTarget = new NormalKDE();
            KDENonTarget.BuildKDE(nonTargetScores.ToArray());
            double[] nonTargetPDF = KDENonTarget.CalculateProbabilities(xVector);

            NormalKDE KDETarget = new NormalKDE();
            KDETarget.BuildKDE(targetScores.ToArray());
            double[] targetPDF = KDETarget.CalculateProbabilities(xVector);

            // Plot KDEs
            graphPane.AddCurve("non-target", xVector, nonTargetPDF, Color.Blue, SymbolType.None);
            graphPane.AddCurve("target", xVector, targetPDF, Color.Red, SymbolType.None);
            graphPane.Title.Text = "Class-conditional distributions";
            graphPane.AxisChange();

            if (graphForm != null)
            {
                graphForm.ShowDialog();
                graphForm.Dispose();
            }
        }

        /// <summary>
        /// Plot ROC curve
        /// </summary>
        /// <param name="TPrate"></param>
        /// <param name="FPrate"></param>
        /// <param name="auc"></param>
        /// <param name="graphPane"></param>
        public static void plotRoC(double[] TPrate, double[] FPrate, float auc = 0, GraphPane graphPane = null)
        {
            GraphDisplayerForm1x2 graphForm = null;
            if (graphPane == null)
            {
                graphForm = new GraphDisplayerForm1x2();
                graphPane = graphForm.graphControlLeft.GraphPane;
            }

            graphPane.AddCurve("RoC curve", FPrate, TPrate, Color.Blue, SymbolType.None);
            graphPane.XAxis.Scale.Min = 0;
            graphPane.XAxis.Scale.Max = 1;
            graphPane.YAxis.Scale.Min = 0;
            graphPane.YAxis.Scale.Max = 1;
            graphPane.Title.Text = "AUC:" + auc;
            graphPane.XAxis.Title.Text = "FP";
            graphPane.YAxis.Title.Text = "TP";
            graphPane.AxisChange();

            if (graphForm != null)
            {
                graphForm.ShowDialog();
                graphForm.Dispose();
            }
        }

        /// <summary>
        /// Display signals in graph
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channels"></param>
        /// <param name="graphPaneObj"></param>
        /// <param name="title"></param>
        /// <param name="sampleRate"></param>
        private static void diplaySignalInGraph(List<double[,]> data, int[] channels, GraphPane graphPaneObj, string title, int sampleRate)
        {
            List<PointPairList> channelPoints = new List<PointPairList>();
            List<double[]> channelAverages = new List<double[]>();

            int numTrials = data.Count;
            int numChannels = data[0].GetLength(1);
            int numSamples = data[0].GetLength(0);

            if (numChannels != channels.Length)
            {
                for (int i = 1; i <= numChannels; i++)
                    channels[i - 1] = i;
            }
            //double[] averages = new double[numSamples];

            double maxV = 0;
            double minV = 0;
            float maxTime = 0;
            for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
            {
                PointPairList graphPoints = new PointPairList();
                double[] averages = new double[numSamples];

                for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                {
                    double sample = 0;
                    for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
                    {
                        double v = (double)data[trialIdx][sampleIdx, channelIdx];
                        sample += v;//.Get(sampleIdx).Get(channelIdx);
                    }

                    // Calculate corresponding time (X value) and average (Y value) for the sampleIdx
                    sample = (float)sample / (float)numTrials;
                    float time = (float)sampleIdx / sampleRate;

                    if (sample > maxV)
                        maxV = sample;
                    if (sample < minV)
                        minV = sample;
                    maxTime = time;

                    // Append to create the vector or averaged values
                    graphPoints.Add(time, sample);
                    averages[sampleIdx] = sample;
                }

                // Add to plot
                String channelLegend = "";
                if (channels.Length >= channelIdx)
                    channelLegend = "channel " + channels[channelIdx].ToString();
                else
                    channelLegend = "channel idx " + channelIdx;

                graphPaneObj.AddCurve(channelLegend, graphPoints, System.Drawing.ColorTranslator.FromHtml(ColorValues[channelIdx]), SymbolType.None);
                graphPaneObj.AxisChange();

                // Append to lists
                channelPoints.Add(graphPoints);
                channelAverages.Add(averages);
            }

            //double r = maxV - minV;
            graphPaneObj.XAxis.Title.Text = "time (s)";
            graphPaneObj.YAxis.Title.Text = "uV";
            graphPaneObj.YAxis.Scale.Max = maxV;
            graphPaneObj.YAxis.Scale.Min = minV;
            graphPaneObj.XAxis.Scale.Min = 0;
            graphPaneObj.XAxis.Scale.Max = maxTime;
            graphPaneObj.Title.Text = title;
        }
    }
}