////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Filter.cs
//
// Notch and Frontend filters
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition

{
    public class Filter
    {
        //Type of filter: Notch or Frontend
        public enum FilterTypes
        {
            Notch,
            Frontend,
        }

        /// <summary>
        ///  number of coeficients of the filter
        /// </summary>
        private const int nCoeffs = 5; //25;

        private double[,] prev_x_filt;
        private double[,] prev_y_filt;

        /// <summary>
        /// Coeficients of the filters
        /// </summary>
        private double[] b = null;

        private double[] a = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filterIdx">ID of the filter </param>
        /// <param name="filterType">type of filter (Notch or Passband)</param>
        /// <param name="maxChannels">max number of channels of the filter</param>
        public Filter(int filterIdx, FilterTypes filterType, int maxChannels = 17)
        {
            prev_x_filt = new double[maxChannels, nCoeffs];
            prev_y_filt = new double[maxChannels, nCoeffs];

            b = new double[nCoeffs] { 1, 1, 1, 1, 1 };
            a = new double[nCoeffs] { 1, 1, 1, 1, 1 };

            FiltersSelect(filterIdx, filterType);
        }

        /// <summary>
        /// Restart filter
        /// </summary>
        /// <param name="maxChannels"></param>
        public void Restart(int maxChannels = 17)
        {
            prev_x_filt = new double[maxChannels, nCoeffs];
            prev_y_filt = new double[maxChannels, nCoeffs];
        }

        /// <summary>
        /// Select filter
        /// </summary>
        /// <param name="filterIdx"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        private bool FiltersSelect(int filterIdx, FilterTypes filterType)
        {
            switch (filterType)
            {
                case FilterTypes.Notch:
                    switch (filterIdx)
                    {
                        case 1: // 50Hz
                            b = new double[nCoeffs] { 0.96508099, -1.19328255, 2.29902305, -1.19328255, 0.96508099 };
                            a = new double[nCoeffs] { 1, -1.21449347931898, 2.29780334191380, -1.17207162934772, 0.931381682126902 };
                            break;

                        case 2: // 60Hz
                            b = new double[nCoeffs] { 0.9650809863447347, -0.2424683201757643, 1.945391494128786, -0.2424683201757643, 0.9650809863447347 };
                            a = new double[nCoeffs] { 1, -0.2467782611297853, 1.944171784691352, -0.2381583792217435, 0.9313816821269039 };
                            break;

                        default: // no filter
                            b = new double[nCoeffs] { 1, 1, 1, 1, 1 };
                            a = new double[nCoeffs] { 1, 1, 1, 1, 1 };
                            break;
                    }
                    break;

                case FilterTypes.Frontend:
                    switch (filterIdx)
                    {
                        case 1:  //1-50Hz
                            b = new double[nCoeffs] { 0.2001387256580675, 0, -0.4002774513161350, 0, 0.2001387256580675 };
                            a = new double[nCoeffs] { 1, -2.355934631131582, 1.941257088655214, -0.7847063755334187, 0.1999076052968340 };
                            break;

                        case 2://7-13Hz
                            b = new double[nCoeffs] { 0.005129268366104263, 0, -0.01025853673220853, 0, 0.005129268366104263 };
                            a = new double[nCoeffs] { 1, -3.678895469764040, 5.179700413522124, -3.305801890016702, 0.8079495914209149 };
                            break;

                        case 3: //15-50Hz
                            b = new double[nCoeffs] { 0.1173510367246093, 0, -0.2347020734492186, 0, 0.1173510367246093 };
                            a = new double[nCoeffs] { 1, -2.137430180172061, 2.038578008108517, -1.070144399200925, 0.2946365275879138 };
                            break;

                        case 4: //5-50Hz
                            b = new double[nCoeffs] { 0.1750876436721012, 0, -0.3501752873442023, 0, 0.1750876436721012 };
                            a = new double[nCoeffs] { 1, -2.299055356038497, 1.967497759984450, -0.8748055564494800, 0.2196539839136946 };
                            break;

                        case 5: //highpass 20-250Hz
                            b = new double[nCoeffs] { 0.514246848996279, -2.056987395985115, 3.085481093977673, -2.056987395985115, 0.514246848996279 };
                            a = new double[nCoeffs] { 1, -2.692610987017433, 2.867399109111386, -1.403484671368138, 0.264454816443504 };
                            break;

                        case 6: // highpass 0.01Hz
                            b = new double[nCoeffs] { 0.999671678819234, -3.998686715276934, 5.998030072915401, -3.998686715276934, 0.999671678819234 };
                            a = new double[nCoeffs] { 1, -3.999343249822933, 5.998029965120605, -3.998030180730933, 0.999343465433265 };
                            break;

                        case 7: // lowpass 10Hz
                            b = new double[nCoeffs] { 0.000183216023369612, 0.000732864093478447, 0.00109929614021767, 0.000732864093478447, 0.000183216023369612 };
                            a = new double[nCoeffs] { 1, -3.34406783771188, 4.23886395088407, -2.40934285658632, 0.517478199788042 };
                            break;

                        case 8: //bandpass 0.01-10Hz (EOG)
                            b = new double[nCoeffs] { 0.0190392098141520, 0, -0.0380784196283039, 0, 0.0190392098141520 };
                            a = new double[nCoeffs] { 1, -4.35026430147529, 7.12700526502516, -5.20323116418853, 1.42649020184247 };
                            break;

                        case 9: //bandpass 2-15Hz (EOG blink removal)
                            b = new double[nCoeffs] { 0.00434585942518146, 0, -0.00869171885036292, 0, 0.00434585942518146 };
                            a = new double[nCoeffs] { 1, -3.78701160054245, 5.39822412124497, -3.43356661099357, 0.822435633245155 };
                            break;

                        default: // no filter
                            b = new double[nCoeffs] { 1, 1, 1, 1, 1 };
                            a = new double[nCoeffs] { 1, 1, 1, 1, 1 };
                            break;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Changes filter
        /// </summary>
        /// <param name="filterIdx"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public bool SetFilterIdx(int filterIdx, FilterTypes filterType)
        {
            Restart();
            FiltersSelect(filterIdx, filterType);
            return true;
        }

        /// <summary>
        /// Filter sample corresponding to channelIdx = 1
        /// </summary>
        /// <param name="frontendIdx"></param>
        /// <param name="notchIdx"></param>
        /// <param name="data"></param>
        /// <param name="channelIdx"></param>
        /// <returns></returns>
        public double FilterData(double sampleAtTimeT, int channelIdx)
        {
            //double  filteredData = filterFrontend(a, b, sampleAtTimeT, channelIdx);

            for (int j = nCoeffs - 1; j > 0; j--)
            {
                prev_x_filt[channelIdx, j] = prev_x_filt[channelIdx, j - 1];
                prev_y_filt[channelIdx, j] = prev_y_filt[channelIdx, j - 1];
            }
            prev_x_filt[channelIdx, 0] = sampleAtTimeT;

            // Apply notch filter
            double filteredData = 0;
            for (int j = 0; j < nCoeffs; j++)
            {
                filteredData += b[j] * prev_x_filt[channelIdx, j];
                if (j > 0)
                {
                    filteredData -= a[j] * prev_y_filt[channelIdx, j];
                }
            }
            prev_y_filt[channelIdx, 0] = filteredData;

            return filteredData;
        }

        /// <summary>
        /// Filters samples
        /// </summary>
        /// <param name="frontendIdx"></param>
        /// <param name="notchIdx"></param>
        /// <param name="samplesAtTimeT"></param> samples for all channels at t-..
        /// <returns></returns>
        public double[] FilterData(double[] samplesAtTimeT)
        {
            int numChannels = samplesAtTimeT.Length;

            // Filter data for all channels
            double[] filteredData = new double[numChannels];
            for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
            {
                filteredData[channelIdx] = FilterData(samplesAtTimeT[channelIdx], channelIdx);
            }
            return filteredData;
        }

        /// <summary>
        /// Filters samples in
        /// </summary>
        /// <param name="data"> in [numChannels, numSamples] </param>
        /// <returns></returns>
        public double[,] FilterData(double[,] unfilteredData, int[] indChannels = null)
        {
            int numChannels;
            int numSamples = unfilteredData.GetLength(1);

            if (indChannels == null)
            {
                numChannels = unfilteredData.GetLength(0);
            }
            else
            {
                numChannels = indChannels.Length;
            }

            double[,] filteredData = (double[,])unfilteredData.Clone();

            for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
            {
                for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                {
                    if (indChannels == null)
                        filteredData[channelIdx, sampleIdx] = FilterData(unfilteredData[channelIdx, sampleIdx], channelIdx);
                    else
                        filteredData[indChannels[channelIdx], sampleIdx] = FilterData(unfilteredData[indChannels[channelIdx], sampleIdx], channelIdx);
                }
            }
            return filteredData;
        }
    }
}