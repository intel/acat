////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DisplayPosteriorProbsForm.cs
//
// Form to display posterior probabilities in a column chart
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public partial class DisplayPosteriorProbsForm : Form
    {
        public DisplayPosteriorProbsForm()
        {
            InitializeComponent();
        }

        public void AddXYToChart(string x, float y)
        {
            chProbs.Series[0].Points.AddXY(x, y);
        }

        private delegate void AddPointsToChartCallback(float[] yValues, string title);

        private delegate void AddPointsToWithSeriesChartCallback(float[] yValues, int series, string title);

        /// <summary>
        /// Add points (yValues) to char
        /// </summary>
        /// <param name="yValues"> y values to add to char</param>
        /// <param name="title">title</param>
        public void AddPointsToChart(float[] yValues, string title)
        {
            if (this.chProbs.InvokeRequired)
            {
                AddPointsToChartCallback d = new AddPointsToChartCallback(AddPointsToChart);
                this.Invoke(d, new object[] { yValues, title });
            }
            else
            {
                chProbs.Series[0].Points.Clear();

                // Set max and minimum
                chProbs.ChartAreas[0].AxisY.Maximum = 1.2; //Set at 1.2 (max prob = 1)
                chProbs.ChartAreas[0].AxisY.Minimum = 0;

                // Add datapoints
                for (int i = 0; i < yValues.Length; i++)
                    chProbs.Series[0].Points.AddXY(i, yValues[i]);

                // Add title
                if (chProbs.Titles.Count == 0)
                    chProbs.Titles.Add(" ");
                chProbs.Titles[0].Text = title;

                // Update chart
                chProbs.Update();
            }
        }

        /// <summary>
        /// Add new series to chart
        /// </summary>
        /// <param name="yValues"> y values to display </param>
        /// <param name="seriesID">ID of the series </param>
        /// <param name="title">title</param>
        public void AddPointsToChart(float[] yValues, int seriesID, string title)
        {
            if (this.chProbs.InvokeRequired)
            {
                AddPointsToWithSeriesChartCallback d = new AddPointsToWithSeriesChartCallback(AddPointsToChart);
                this.Invoke(d, new object[] { yValues, seriesID, title });
            }
            else
            {
                if (chProbs.Series.Count <= seriesID || chProbs.Series[seriesID] == null)
                    chProbs.Series.Add(" ");

                // Remove previous points for series
                chProbs.Series[seriesID].Points.Clear();

                //Set max and minimum
                chProbs.ChartAreas[0].AxisY.Maximum = 1.2; //max prob = 1, maximum set at 1.2
                chProbs.ChartAreas[0].AxisY.Minimum = 0;

                // Addd points to char
                for (int i = 0; i < yValues.Length; i++)
                    chProbs.Series[seriesID].Points.AddXY(i, yValues[i]);

                // Add title
                if (chProbs.Titles.Count == 0)
                    chProbs.Titles.Add(" ");
                chProbs.Titles[0].Text = title;

                // UPdate chart
                chProbs.Update();
            }
        }

        /// <summary>
        /// Set target text
        /// </summary>
        /// <param name="text"></param>
        public void SetTargetText(String text)
        {
            lblTarget.Text = text;
        }

        /// <summary>
        ///  Set repetition text
        /// </summary>
        /// <param name="text"></param>
        public void SetRepetitionText(String text)
        {
            lblRepetition.Text = text;
        }
    }
}