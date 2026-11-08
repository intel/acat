///////////////////////////////////////////////////////////////////////////
// <copyright file="stlbp_feature.cpp" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

#include "libivcp/feature/stlbp_feature.hpp"

using namespace std;
using namespace cv;

//#define ROTATION_AS_IN_REFERENCE_CODE

namespace ivcp
{
STLBPFeature::~STLBPFeature()
{
}

STLBPFeature::STLBPFeature(STLBPFeature::Settings settings): 
    Feature(settings.sizeT)
{
    setSettings(settings);
}

Mat STLBPFeature::getFeature(const vector<RegionSequence> &regSeqs)
{
    if (regSeqs.size() != 1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of sequences is incorrect" );
    }

    if (regSeqs[0].size() != numberOfRegions)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of regions is incorrect" );
    }

    RegionSequence regSeq;
    if(regSeqs[0][0].type() == CV_8UC3)
    {
        for(auto iter = regSeqs[0].begin(); iter != regSeqs[0].end(); iter++)
        {
            Mat region;
            cvtColor(*iter, region, cv::COLOR_BGR2GRAY);
            regSeq.push_back(region);
        }
    }
    else
    {
        if(regSeqs[0][0].type() == CV_8UC1 || regSeqs[0][0].type() == CV_16UC1)
        {
            //TODO: possibly shall copy?
            regSeq = regSeqs[0];
        }
        else
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unsupported matrix data type" );
        }
    }

    //TODO: possibly there is a better solution?
    //Resize all regions to the same size
    Size maxRegion = regSeq[0].size();
    for (unsigned int r = 1; r < numberOfRegions; r++)
    {
        if (maxRegion.width < regSeq[r].cols)
        {
            maxRegion.width = regSeq[r].cols;
        }

        if (maxRegion.height < regSeq[r].rows)
        {
            maxRegion.height = regSeq[r].rows;
        }
    }

    for (unsigned int r = 0; r < numberOfRegions; r++)
    {
        resize(regSeq[r], regSeq[r], maxRegion);
    }

    long unsigned int matSizes[3];
    matSizes[0] = (unsigned long int)((int)regSeq.size() - 2 * radiusT);
    matSizes[1] = (unsigned long int)(regSeq[0].rows - 2 * radiusY);
    matSizes[2] = (unsigned long int)(regSeq[0].cols - 2 * radiusX);

    vector<Mat> lbpXY(matSizes[0]);
    vector<Mat> lbpXT(matSizes[0]);
    vector<Mat> lbpYT(matSizes[0]);

    for(unsigned long k = 0; k < matSizes[0]; k++)
    {
        lbpXY[k] = Mat::zeros(matSizes[1], matSizes[2], CV_32FC1);
        lbpXT[k] = Mat::zeros(matSizes[1], matSizes[2], CV_32FC1);
        lbpYT[k] = Mat::zeros(matSizes[1], matSizes[2], CV_32FC1);
    }

    matSizes[0] = (unsigned long int)((int)regSeq.size() - radiusT);
    matSizes[1] = (unsigned long int)(regSeq[0].rows - radiusY);
    matSizes[2] = (unsigned long int)(regSeq[0].cols - radiusX);

    calculateLBP(regSeq, lbpXY, lbpXT, lbpYT, matSizes);
    
    if(uniformPatterns)
    {
        makeUniform(lbpXY, lbpXT, lbpYT, matSizes);
    }

    Mat result = calculateResultHist(regSeq, lbpXY, lbpXT, lbpYT);

    return result;
}

void STLBPFeature::makeUniformForOnePlane(vector<Mat> &lbp, int numNeighbours, int numPatterns, const long unsigned int matSizes[3])
{
    if (numNeighbours >= 4)
    {
        for(unsigned long k = 0; k < matSizes[0] - radiusT; k++)
        {
            for(unsigned long i = 0; i < matSizes[1] - radiusY; i++)
            {
                for(unsigned long j = 0; j < matSizes[2] - radiusX; j++)
                {
                    int32_t currentValue = (int32_t)lbp[k].at<float>(i,j);
                    int32_t xorValue = currentValue ^ ((currentValue >> 1) | ((currentValue << (numNeighbours - 1)) % numPatterns) );

                    int transitions = 0;
                    for(int n = 0; n < numNeighbours; n++, xorValue >>= 1)
                    {
                        transitions += xorValue & 1;
                    }
                    if(transitions > 2)
                    {
                        lbp[k].at<float>(i,j) = (float)minNonUniformValue;
                    }
                }
            }
        }
    }
}

void STLBPFeature::makeUniform(vector<Mat> &lbpXY, vector<Mat> &lbpXT, vector<Mat> &lbpYT, long unsigned int matSizes[3])
{
    makeUniformForOnePlane(lbpXY, neighborsXY, numPatternsXY, matSizes);
    makeUniformForOnePlane(lbpXT, neighborsXT, numPatternsXT, matSizes);
    makeUniformForOnePlane(lbpYT, neighborsYT, numPatternsYT, matSizes);
}

void STLBPFeature::calculateInterpolationWeights(float value1, float value2, float &w1, float &w2, float &w3, float &w4)
{
    w1 = (1 - value1) * (1 - value2);
    w2 =      value1  * (1 - value2);
    w3 = (1 - value1) *      value2;
    w4 =      value1  *      value2;
}

/*!
 * \brief Calculate LBP features in volume
 */
template<typename matrixData> void STLBPFeature::calculateLBPForOnePlane(const RegionSequence &regSeq, vector<Mat> &lbp, int numNeighbours, Plane plane, const long unsigned int matSizes[3])
{
#ifndef ROTATION_AS_IN_REFERENCE_CODE
    float radius1 = 0, radius2 = 0;
    switch (plane)
    {
    case XY:
        radius1 = (float)radiusX;
        radius2 = (float)radiusY;
        break;
    case XT:
        radius1 = (float)radiusX;
        radius2 = (float)radiusT;
        break;
    case YT:
        radius1 = (float)radiusY;
        radius2 = (float)radiusT;
        break;
    }
#endif

    for(int n = 0; n < numNeighbours; n++)
    {
        float x = 0.0f, y = 0.0f, t = 0.0f;

        // sample points
#ifndef ROTATION_AS_IN_REFERENCE_CODE
        float value1 = static_cast<float>( -radius1 * sin( 2.0 * CV_PI * n / static_cast<float>(numNeighbours) ) );
        float value2 = static_cast<float>( radius2 * cos( 2.0 * CV_PI * n / static_cast<float>(numNeighbours) ) );

        switch (plane)
        {
        case XY:
            x = value1;
            y = value2;
            break;
        case XT:
            x = value1;
            t = value2;
            break;
        case YT:
            y = value1;
            t = value2;
            break;
        }
#else
        switch (plane)
        {
        case XY:
            x = static_cast<float>( radiusX * cos( 2.0 * CV_PI * n / static_cast<float>(neighborsXY) ) );
            y = static_cast<float>( -radiusY * sin( 2.0 * CV_PI * n / static_cast<float>(neighborsXY) ) );
            break;
        case XT:
            x = static_cast<float>( radiusX * cos( 2.0 * CV_PI * n / static_cast<float>(neighborsXT) ) );
            t = static_cast<float>( radiusT * sin( 2.0 * CV_PI * n / static_cast<float>(neighborsXT) ) );            
            break;
        case YT:
            y = static_cast<float>( -radiusY * sin( 2.0 * CV_PI * n / static_cast<float>(neighborsYT) ) );
            t = static_cast<float>( radiusT * cos( 2.0 * CV_PI * n / static_cast<float>(neighborsYT) ) );
            break;
        }
#endif

        // relative indices
        int fx = static_cast<int>(floor(x));
        int fy = static_cast<int>(floor(y));
        int ft = static_cast<int>(floor(t));
        int cx = static_cast<int>(ceil(x));
        int cy = static_cast<int>(ceil(y));
        int ct = static_cast<int>(ceil(t));
        
        // fractional part
        float ty = y - fy;
        float tx = x - fx;
        float tt = t - ft;

        // set interpolation weights for 4 neighbouring pixels
        float wf1f2 = 0.0f, wc1f2 = 0.0f, wf1c2 = 0.0, wc1c2 = 0.0f;
        switch (plane)
        {
        case XY:
            calculateInterpolationWeights(tx, ty, wf1f2, wc1f2, wf1c2, wc1c2);
            break;
        case XT:
            calculateInterpolationWeights(tx, tt, wf1f2, wc1f2, wf1c2, wc1c2);
            break;
        case YT:
            calculateInterpolationWeights(ty, tt, wf1f2, wc1f2, wf1c2, wc1c2);
            break;
        }

        // iterate through your data
        for(unsigned long k = radiusT; k < matSizes[0]; k++)
        {
            Mat regSeqK = regSeq[k];
            Mat regSeqFT = regSeq[k + ft];
            Mat regSeqCT = regSeq[k + ct];

            for(unsigned long i = radiusY; i < matSizes[1]; i++)
            {
                for(unsigned long j = radiusX; j < matSizes[2]; j++)
                {
                    matrixData centerValue = regSeqK.at<matrixData>(i, j);

                    // calculate interpolated value
                    float interpolatedValue = 0.0f;
                    switch (plane)
                    {
                    case XY:
                    case XT:
                        interpolatedValue = static_cast<float>(
                            wf1f2 * regSeqFT.at<matrixData>(i + fy, j + fx) + 
                            wc1f2 * regSeqFT.at<matrixData>(i + fy, j + cx) +
                            wf1c2 * regSeqCT.at<matrixData>(i + cy, j + fx) +
                            wc1c2 * regSeqCT.at<matrixData>(i + cy, j + cx));
                        break;
                    case YT:
                        interpolatedValue = static_cast<float>(
                            wf1f2 * regSeqFT.at<matrixData>(i + fy, j) + 
                            wc1f2 * regSeqFT.at<matrixData>(i + cy, j) +
                            wf1c2 * regSeqCT.at<matrixData>(i + fy, j) +
                            wc1c2 * regSeqCT.at<matrixData>(i + cy, j));
                        break;
                    }

                    unsigned int value = 0;
                    const float epsilon = numeric_limits<float>::epsilon();
                    if (( interpolatedValue > centerValue ) || ( abs( interpolatedValue - centerValue ) < epsilon ))
                    {
                        value = 1;
                    }
                    
                    lbp[k-radiusT].at<float>(i-radiusY,j-radiusX) += (value << n);
                }
            }
        }
    }
}

void STLBPFeature::calculateLBP(const RegionSequence &regSeq, vector<Mat> &lbpXY, vector<Mat> &lbpXT, vector<Mat> &lbpYT, long unsigned int matSizes[3])
{
    if(regSeq[0].type() == CV_16UC1)
    {
        calculateLBPForOnePlane<unsigned short>(regSeq, lbpXY, neighborsXY, XY, matSizes);
        calculateLBPForOnePlane<unsigned short>(regSeq, lbpXT, neighborsXT, XT, matSizes);
        calculateLBPForOnePlane<unsigned short>(regSeq, lbpYT, neighborsYT, YT, matSizes);
    }
    else
    {
        calculateLBPForOnePlane<unsigned char>(regSeq, lbpXY, neighborsXY, XY, matSizes);
        calculateLBPForOnePlane<unsigned char>(regSeq, lbpXT, neighborsXT, XT, matSizes);
        calculateLBPForOnePlane<unsigned char>(regSeq, lbpYT, neighborsYT, YT, matSizes);
    }
}

void STLBPFeature::calculateResultHistForOnePlane(Mat cellHist, int numNeighbours, int numPatterns, Mat resultPart)
{
    int32_t resultIndex = 0;
    Mat histPart = cellHist.reshape(1, 1);
    for(int32_t k = 0; k < numPatterns; k++)
    {
        int32_t xorValue = k ^ ( (k >> 1) | ((k << (numNeighbours - 1)) % numPatterns) );

        int transitions = 0;
        for(int n = 0; n < numNeighbours; n++, xorValue >>= 1)
        {
            transitions += xorValue & 1;
        }

        if(uniformPatterns && transitions > 2)
        {   
            continue;
        }
        resultPart.at<float>(0,resultIndex++) = histPart.at<float>(0,k);
    }
    if(uniformPatterns && numNeighbours >= 4)
    {
        resultPart.at<float>(0,resultIndex) = histPart.at<float>(0, minNonUniformValue);
    }
}

Mat STLBPFeature::calculateResultHist(const RegionSequence &regSeq, vector<Mat> &lbpXY, vector<Mat> &lbpXT, vector<Mat> &lbpYT)
{
    int rangeT=((int)regSeq.size() - 2 * radiusT) / gridT;
    int rangeY=(regSeq[0].rows - 2 * radiusY) / gridY;
    int rangeX=(regSeq[0].cols - 2 * radiusX) / gridX;

    // allocate memory for the spatial histogram
    Mat result = Mat::zeros(gridX * gridY * gridT, numResultPatternsXY + numResultPatternsXT + numResultPatternsYT, CV_32FC1);

    // Set the ranges.
    float rangeXY[] = { 0, static_cast<float>(numPatternsXY) };
    const float* histRangeXY = { rangeXY };
    float rangeXT[] = { 0, static_cast<float>(numPatternsXT) };
    const float* histRangeXT = { rangeXT };
    float rangeYT[] = { 0, static_cast<float>(numPatternsYT) };
    const float* histRangeYT = { rangeYT };

    // iterate through grid
    int resultRowIdx = 0;
    Range ranges[3];
    ranges[0].start = 0;
    ranges[0].end = rangeT;
    for(int k = 0; k < gridT; k++, ranges[0].start += rangeT, ranges[0].end += rangeT)
    {
        ranges[1].start = 0;
        ranges[1].end = rangeY;
        for(int i = 0; i < gridY; i++, ranges[1].start += rangeY, ranges[1].end += rangeY)
        {
            ranges[2].start = 0;
            ranges[2].end = rangeX;
            for(int j = 0; j < gridX; j++, ranges[2].start += rangeX, ranges[2].end += rangeX)
            {
                Mat cellHistXY, cellHistXT, cellHistYT;
                for(int l = ranges[0].start; l < ranges[0].end; l++)
                {
                    // XY histogram
                    Mat src_cellXY(lbpXY[l], ranges + 1);

                    // calc histogram
                    calcHist(&src_cellXY, 1, 0, Mat(), cellHistXY, 1, &numPatternsXY, &histRangeXY, true, true);

                    // XT histogram
                    Mat src_cellXT(lbpXT[l], ranges + 1);

                    // calc histogram
                    calcHist(&src_cellXT, 1, 0, Mat(), cellHistXT, 1, &numPatternsXT, &histRangeXT, true, true);

                    // YT histogram
                    Mat src_cellYT(lbpYT[l], ranges + 1);

                    // calc histogram
                    calcHist(&src_cellYT, 1, 0, Mat(), cellHistYT, 1, &numPatternsYT, &histRangeYT, true, true);
                }

                // normalize
                if(normalize)
                {
                    cellHistXY /= rangeX * rangeY * rangeT;
                    cellHistXT /= rangeX * rangeY * rangeT;
                    cellHistYT /= rangeX * rangeY * rangeT;
                }

                // copy to the result matrix
                if(uniformPatterns && neighborsXY >= 4)
                {
                    Mat resultPartXY = result.row(resultRowIdx).colRange(0, numResultPatternsXY);
                    calculateResultHistForOnePlane(cellHistXY, neighborsXY, numPatternsXY, resultPartXY);
                }
                else
                {
                    cellHistXY.reshape(1, 1).convertTo(
                        result.row(resultRowIdx).colRange(0, numResultPatternsXY), CV_32FC1);
                }

                if(uniformPatterns && neighborsXT >= 4)
                {
                    Mat resultPartXT = result.row(resultRowIdx).colRange(numResultPatternsXY, numResultPatternsXY + numResultPatternsXT);
                    calculateResultHistForOnePlane(cellHistXT, neighborsXT, numPatternsXT, resultPartXT);
                }
                else
                {
                    cellHistXT.reshape(1, 1).convertTo(
                        result.row(resultRowIdx).colRange(numResultPatternsXY, numResultPatternsXY + numResultPatternsXT), CV_32FC1);
                }

                if(uniformPatterns && neighborsYT >= 4)
                {
                    Mat resultPartYT = result.row(resultRowIdx).colRange(numResultPatternsXY + numResultPatternsXT, numResultPatternsXY + numResultPatternsXT + numResultPatternsYT);
                    calculateResultHistForOnePlane(cellHistYT, neighborsYT, numPatternsYT, resultPartYT);
                }
                else
                {
                    cellHistYT.reshape(1, 1).convertTo(
                        result.row(resultRowIdx).colRange(
                        numResultPatternsXY + numResultPatternsXT, 
                        numResultPatternsXY + numResultPatternsXT + numResultPatternsYT), CV_32FC1);
                }

                // increase row count in result matrix
                resultRowIdx++;
            }
        }
    }

    // return result as reshaped feature vector
    return result.reshape(1, 1);
}

FeatureType STLBPFeature::getType()
{
    return STLBP;
}

STLBPFeature::Settings STLBPFeature::getSettings()
{
    return settingsCopy;
}

int32_t STLBPFeature::getNumResultPatterns(int numNeighbours, int numPatterns)
{
    if(uniformPatterns && numNeighbours >= 4)
    {
        return (numNeighbours*(numNeighbours-1) + 3);
    }
    else
    {
        return numPatterns;
    }
}

void STLBPFeature::setSettings(STLBPFeature::Settings settings)
{
    if( !(settings.radiusX > 0 && settings.radiusY > 0 && settings.radiusT > 0) )
    {
        IVCP_EXCEPTION(IncorrectParamException, "STLBP Feature radius should be greater than 0" );
    }

    if( !(settings.neighboursXY > 0 && settings.neighboursXT > 0 && settings.neighboursYT > 0) )
    {
        IVCP_EXCEPTION(IncorrectParamException, "There should be at least 1 neighbour on each plane to calculate STLBP" );
    }

    if( !(settings.neighboursXY < 24 && settings.neighboursXT < 24 && settings.neighboursYT < 24) )     //Otherwise LBP could not be calculated precisely
    {
        IVCP_EXCEPTION(IncorrectParamException, "Could not calculate STLBP with more than 24 neighbours on a plane" );
    }

    if( !(settings.gridX > 0 && settings.gridY > 0 && settings.gridT > 0) )
    {
        IVCP_EXCEPTION(IncorrectParamException, "STLBP grid size should be greater than 0 for each dimension" );
    }

    if( (settings.sizeT - 2 * settings.radiusT) < settings.gridT )
    {
        IVCP_EXCEPTION(IncorrectParamException, "Used STLBP feature frame sequence is too short for evaluation of the configured grid" );
    }

    radiusX = settings.radiusX;
    radiusY = settings.radiusY;
    radiusT = settings.radiusT;

    neighborsXY = settings.neighboursXY;
    neighborsXT = settings.neighboursXT;
    neighborsYT = settings.neighboursYT;

    gridX = settings.gridX;
    gridY = settings.gridY;
    gridT = settings.gridT;

    normalize = settings.normalizeFeature;
    uniformPatterns = settings.useUniformPatterns;

    //populate precalculated parameters
    numPatternsXY = static_cast<int>(pow(2.0, static_cast<double>(neighborsXY)));
    numPatternsXT = static_cast<int>(pow(2.0, static_cast<double>(neighborsXT)));
    numPatternsYT = static_cast<int>(pow(2.0, static_cast<double>(neighborsYT)));
    
    numResultPatternsXY = getNumResultPatterns(neighborsXY, numPatternsXY);
    numResultPatternsXT = getNumResultPatterns(neighborsXT, numPatternsXT);
    numResultPatternsYT = getNumResultPatterns(neighborsYT, numPatternsYT);

    //inherited property configuration
    featureLength = gridX * gridY * gridT * ( numResultPatternsXY + numResultPatternsXT + numResultPatternsYT );

    settingsCopy = settings;
}

const int STLBPFeature::minNonUniformValue = 5;

}
