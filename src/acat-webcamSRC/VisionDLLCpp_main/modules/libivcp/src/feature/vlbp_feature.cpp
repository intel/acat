///////////////////////////////////////////////////////////////////////////
// <copyright file="vlbp_feature.cpp" company="Intel Corporation">
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

#include "libivcp/feature/vlbp_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
VLBPFeature::~VLBPFeature()
{
}

VLBPFeature::VLBPFeature(VLBPFeature::Settings settings): 
    Feature(settings.sizeT)
{
    setSettings(settings);
}

Mat VLBPFeature::getFeature(const vector<RegionSequence> &regSeqs)
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

    vector<Mat> lbp(matSizes[0]);

    for(unsigned long k = 0; k < matSizes[0]; k++)
    {
        lbp[k] = Mat::zeros(matSizes[1], matSizes[2], CV_32SC1);
    }

    matSizes[0] = (unsigned long int)((int)regSeq.size() - radiusT);
    matSizes[1] = (unsigned long int)(regSeq[0].rows - radiusY);
    matSizes[2] = (unsigned long int)(regSeq[0].cols - radiusX);

    if(regSeq[0].type() == CV_16UC1)
    {
        calculateLBP<unsigned short>(regSeq, lbp, matSizes);
    }
    else
    {
        calculateLBP<unsigned char>(regSeq, lbp, matSizes);
    }
    
    if(rotateInvariant)
    {
        makeRotateInvatiant(lbp, matSizes);  
    }

    if(uniformPatterns)
    {
        makeUniform(lbp, matSizes);
    }

    if(rotateInvariant && uniformPatterns)
    {
        makeWholeRotateInvatiant(lbp, matSizes);  
    }

    Mat result = calculateResultHist(regSeq, lbp);

    return result;
}

void VLBPFeature::makeRotateInvatiant(vector<Mat> &lbp, long unsigned int matSizes[3])
{
    for(unsigned long k = 0; k < matSizes[0] - radiusT; k++)
    {
        for(unsigned long i = 0; i < matSizes[1] - radiusY; i++)
        {
            for(unsigned long j = 0; j < matSizes[2] - radiusX; j++)
            {
                int32_t minValue, currentInvariant;
                minValue = currentInvariant = lbp[k].at<int32_t>(i,j);
                int32_t middle1, middle2, middle3;

                int32_t middleMask1 = (numPatterns - 1) << 1;
                middle1 = currentInvariant & middleMask1;
                currentInvariant &= ~middleMask1;
                int32_t middleMask2 = middleMask1 << neighbours;
                middle2 = currentInvariant & middleMask2;
                currentInvariant &= ~middleMask2;
                int32_t middleMask3 = middleMask2 << neighbours;
                middle3 = currentInvariant & middleMask3;
                currentInvariant &= ~middleMask3;

                for(int n = 1; n < neighbours; n++)
                {
                    middle1 = ( (middle1 >> 1) | (middle1 << (neighbours - 1) ) ) & middleMask1;
                    middle2 = ( (middle2 >> 1) | (middle2 << (neighbours - 1) ) ) & middleMask2;
                    middle3 = ( (middle3 >> 1) | (middle3 << (neighbours - 1) ) ) & middleMask3;

                    minValue = min(minValue, currentInvariant | middle1 | middle2 | middle3);
                }
                lbp[k].at<int32_t>(i,j) = minValue;
            }
        }
    }
}

void VLBPFeature::makeUniform(vector<Mat> &lbp, long unsigned int matSizes[3])
{
    for(unsigned long k = 0; k < matSizes[0] - radiusT; k++)
    {
        for(unsigned long i = 0; i < matSizes[1] - radiusY; i++)
        {
            for(unsigned long j = 0; j < matSizes[2] - radiusX; j++)
            {
                int32_t currentValue = lbp[k].at<int32_t>(i,j);
                int32_t xorValue = currentValue ^ ((currentValue >> 1) | ((currentValue << (neighboursAll - 1)) % numPatternsAllPlanes) );

                int transitions = 0;
                for(int n = 0; n < neighboursAll; n++, xorValue >>= 1)
                {
                    transitions += xorValue & 1;
                }
                if(transitions > 2)
                {
                    lbp[k].at<int32_t>(i,j) = (int32_t)minNonUniformValue;
                }
            }
        }
    }
}

void VLBPFeature::makeWholeRotateInvatiant(vector<Mat> &lbp, long unsigned int matSizes[3])
{
    for(unsigned long k = 0; k < matSizes[0] - radiusT; k++)
    {
        for(unsigned long i = 0; i < matSizes[1] - radiusY; i++)
        {
            for(unsigned long j = 0; j < matSizes[2] - radiusX; j++)
            {
                int32_t minValue, currentValue;
                minValue = currentValue = lbp[k].at<int32_t>(i,j);

                for(int n = 1; n < neighboursAll; n++)
                {
                    currentValue = ( (currentValue >> 1) | ((currentValue << (neighboursAll - 1)) % numPatternsAllPlanes) );
                    minValue = min(minValue, currentValue);
                }

                lbp[k].at<int32_t>(i,j) = minValue;
            }
        }
    }
}

void VLBPFeature::calculateInterpolationWeights(float value1, float value2, float &w1, float &w2, float &w3, float &w4)
{
    w1 = (1 - value1) * (1 - value2);
    w2 =      value1  * (1 - value2);
    w3 = (1 - value1) *      value2;
    w4 =      value1  *      value2;
}

/*!
 * \brief Calculate LBP features in volume
 */
template<typename matrixData> void VLBPFeature::calculateLBP(const RegionSequence &regSeq, vector<Mat> &lbp, const long unsigned int matSizes[3])
{
    const float epsilon = numeric_limits<float>::epsilon();
    for(int n = 0; n < neighbours; n++)
    {
        float x;
        float y;

        // sample points
#ifndef ROTATION_AS_IN_REFERENCE_CODE
        x = static_cast<float>( -radiusX * sin( 2.0 * CV_PI * n / static_cast<float>(neighbours) ) );
        y = static_cast<float>( radiusY * cos( 2.0 * CV_PI * n / static_cast<float>(neighbours) ) );
#else
        x = static_cast<float>( radiusX * cos( 2.0 * CV_PI * n / static_cast<float>(neighbours) ) );
        y = static_cast<float>( -radiusY * sin( 2.0 * CV_PI * n / static_cast<float>(neighbours) ) );
#endif

        // relative indices
        int fx = static_cast<int>(floor(x));
        int fy = static_cast<int>(floor(y));
        int cx = static_cast<int>(ceil(x));
        int cy = static_cast<int>(ceil(y));
        
        // fractional part
        float ty = y - fy;
        float tx = x - fx;

        // set interpolation weights for 4 neighbouring pixels
        float wf1f2 = 0.0f, wc1f2 = 0.0f, wf1c2 = 0.0, wc1c2 = 0.0f;
        calculateInterpolationWeights(tx, ty, wf1f2, wc1f2, wf1c2, wc1c2);

        // iterate through your data
        for(unsigned long k = radiusT; k < matSizes[0]; k++)
        {
            Mat regSeqT = regSeq[k];
            Mat regSeqAT = regSeq[k + radiusT];
            Mat regSeqPT = regSeq[k - radiusT];

            for(unsigned long i = radiusY; i < matSizes[1]; i++)
            {
                for(unsigned long j = radiusX; j < matSizes[2]; j++)
                {
                    matrixData centerValue = regSeqT.at<matrixData>(i, j);

                    // calculate interpolated value
                    float interpolatedValue = static_cast<float>(
                            wf1f2 * regSeqT.at<matrixData>(i + fy, j + fx) + 
                            wc1f2 * regSeqT.at<matrixData>(i + fy, j + cx) +
                            wf1c2 * regSeqT.at<matrixData>(i + cy, j + fx) +
                            wc1c2 * regSeqT.at<matrixData>(i + cy, j + cx));
                    // calculate interpolated value
                    float interpolatedValueA = static_cast<float>(
                            wf1f2 * regSeqAT.at<matrixData>(i + fy, j + fx) + 
                            wc1f2 * regSeqAT.at<matrixData>(i + fy, j + cx) +
                            wf1c2 * regSeqAT.at<matrixData>(i + cy, j + fx) +
                            wc1c2 * regSeqAT.at<matrixData>(i + cy, j + cx));
                    // calculate interpolated value
                    float interpolatedValueP = static_cast<float>(
                            wf1f2 * regSeqPT.at<matrixData>(i + fy, j + fx) + 
                            wc1f2 * regSeqPT.at<matrixData>(i + fy, j + cx) +
                            wf1c2 * regSeqPT.at<matrixData>(i + cy, j + fx) +
                            wc1c2 * regSeqPT.at<matrixData>(i + cy, j + cx));

                    int32_t value = 0;
                    if (( interpolatedValueP > centerValue ) || ( abs( interpolatedValueP - centerValue ) < epsilon ))
                    {
                        value += 2;
                    }
                    if (( interpolatedValue > centerValue ) || ( abs( interpolatedValue - centerValue ) < epsilon ))
                    {
                        value += 2 << neighbours;
                    }
                    if (( interpolatedValueA > centerValue ) || ( abs( interpolatedValueA - centerValue ) < epsilon ))
                    {
                        value += 2 << (2 * neighbours);
                    }
                    
                    lbp[k-radiusT].at<int32_t>(i-radiusY,j-radiusX) += (value << n);
                }
            }
        }
    }
    // iterate through your data
    for(unsigned long k = radiusT; k < matSizes[0]; k++)
    {
        Mat regSeqT = regSeq[k];
        Mat regSeqAT = regSeq[k + radiusT];
        Mat regSeqPT = regSeq[k - radiusT];

        for(unsigned long i = radiusY; i < matSizes[1]; i++)
        {
            for(unsigned long j = radiusX; j < matSizes[2]; j++)
            {
                matrixData centerValue = regSeqT.at<matrixData>(i, j);
                matrixData centerValueA = regSeqAT.at<matrixData>(i, j);
                matrixData centerValueP = regSeqPT.at<matrixData>(i, j);

                int32_t value = 0;
                if (( centerValueP > centerValue ) || ( abs( centerValueP - centerValue ) < epsilon ))
                {
                    value += 1;
                }
                if (( centerValueA > centerValue ) || ( abs( centerValueA - centerValue ) < epsilon ))
                {
                    value += 1 << (neighboursAll - 1);
                }
                    
                lbp[k-radiusT].at<int32_t>(i-radiusY,j-radiusX) += value;
            }
        }
    }
}

void VLBPFeature::calculateResultHistReduced(Mat cellHist, Mat resultPart)
{
    int32_t resultIndex = 0;
    Mat histPart = cellHist.reshape(1, 1);
    for(int32_t k = 0; k < numPatternsAllPlanes; k++)
    {
        int32_t minValue, currentInvariant, minFullValue;
        minFullValue = minValue = currentInvariant = k;
        int32_t middle1, middle2, middle3;

        int32_t middleMask1 = (numPatterns - 1) << 1;
        middle1 = currentInvariant & middleMask1;
        currentInvariant &= ~middleMask1;
        int32_t middleMask2 = middleMask1 << neighbours;
        middle2 = currentInvariant & middleMask2;
        currentInvariant &= ~middleMask2;
        int32_t middleMask3 = middleMask2 << neighbours;
        middle3 = currentInvariant & middleMask3;
        currentInvariant &= ~middleMask3;

        int32_t xorValue = k ^ ( (k >> 1) | ((k << (neighboursAll - 1)) % numPatternsAllPlanes) );
        int transitions = 0;

        for(int n = 0; n < neighboursAll; n++, xorValue >>= 1)
        {
            middle1 = ( (middle1 >> 1) | (middle1 << (neighbours - 1) ) ) & middleMask1;
            middle2 = ( (middle2 >> 1) | (middle2 << (neighbours - 1) ) ) & middleMask2;
            middle3 = ( (middle3 >> 1) | (middle3 << (neighbours - 1) ) ) & middleMask3;
            minValue = min(minValue, currentInvariant | middle1 | middle2 | middle3);
            transitions += xorValue & 1;
            minFullValue = min(minFullValue, ((k >> n) | ((k << (neighboursAll - n)) % numPatternsAllPlanes) ) );
        }

        if(rotateInvariant && minValue != k)
        {
            continue;
        }

        if(uniformPatterns && transitions > 2)
        {   
            continue;
        }

        if(rotateInvariant && uniformPatterns && minFullValue != k)
        {
            continue;
        }

        resultPart.at<float>(0,resultIndex++) = histPart.at<float>(0,k);
    }
    if(uniformPatterns)
    {
        resultPart.at<float>(0,resultIndex) = histPart.at<float>(0, minNonUniformValue);
    }
}

Mat VLBPFeature::calculateResultHist(const RegionSequence &regSeq, vector<Mat> &lbp)
{
    int rangeT = ((int)regSeq.size() - 2 * radiusT) / gridT;
    int rangeY = (regSeq[0].rows - 2 * radiusY) / gridY;
    int rangeX = (regSeq[0].cols - 2 * radiusX) / gridX;

    // allocate memory for the spatial histogram
    Mat result = Mat::zeros(gridX * gridY * gridT, numResultPatterns, CV_32FC1);

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
                Mat cellHist;
                {
                    Mat icellHist(1,numPatternsAllPlanes,CV_32SC1);
                    icellHist = cv::Scalar(0);
                    for(int l = ranges[0].start; l < ranges[0].end; l++)
                    {
                        // calc histogram
                        for(int y = ranges[1].start; y < ranges[1].end; y++ )
                        {
                            for(int x = ranges[2].start; x < ranges[2].end; x++ )
                            {
                                int32_t idx = lbp[l].at<int32_t>(y, x);
                                if( (unsigned)idx < (unsigned)numPatternsAllPlanes )
                                {
                                    icellHist.at<int32_t>(0, idx)++;
                                }
                            }
                        }
                    }
                    icellHist.convertTo(cellHist, CV_32FC1);
                }

                // normalize
                if(normalize)
                {
                    cellHist /= rangeX * rangeY * rangeT;
                }

                // copy to the result matrix
                if(rotateInvariant || uniformPatterns)
                {
                    Mat resultPart = result.row(resultRowIdx).colRange(0, numResultPatterns);
                    calculateResultHistReduced(cellHist, resultPart);
                }
                else
                {
                    cellHist.reshape(1, 1).convertTo(
                        result.row(resultRowIdx).colRange(0, numResultPatterns), CV_32FC1);
                }

                // increase row count in result matrix
                resultRowIdx++;
            }
        }
    }

    // return result as reshaped feature vector
    return result.reshape(1, 1);
}

FeatureType VLBPFeature::getType()
{
    return VLBP;
}

VLBPFeature::Settings VLBPFeature::getSettings()
{
    return settingsCopy;
}

int32_t VLBPFeature::getNumResultPatterns()
{
    int32_t resultPatternCount = 0;

    if(rotateInvariant || uniformPatterns)
    {
        for(int32_t k = 0; k < numPatternsAllPlanes; k++)
        {
            int32_t minValue, currentInvariant, minFullValue;
            minFullValue = minValue = currentInvariant = k;
            int32_t middle1, middle2, middle3;

            int32_t middleMask1 = (numPatterns - 1) << 1;
            middle1 = currentInvariant & middleMask1;
            currentInvariant &= ~middleMask1;
            int32_t middleMask2 = middleMask1 << neighbours;
            middle2 = currentInvariant & middleMask2;
            currentInvariant &= ~middleMask2;
            int32_t middleMask3 = middleMask2 << neighbours;
            middle3 = currentInvariant & middleMask3;
            currentInvariant &= ~middleMask3;

            int32_t xorValue = k ^ ( (k >> 1) | ((k << (neighboursAll - 1)) % numPatternsAllPlanes) );
            int transitions = 0;

            for(int n = 0; n < neighboursAll; n++, xorValue >>= 1)
            {
                middle1 = ( (middle1 >> 1) | (middle1 << (neighbours - 1) ) ) & middleMask1;
                middle2 = ( (middle2 >> 1) | (middle2 << (neighbours - 1) ) ) & middleMask2;
                middle3 = ( (middle3 >> 1) | (middle3 << (neighbours - 1) ) ) & middleMask3;
                minValue = min(minValue, currentInvariant | middle1 | middle2 | middle3);
                transitions += xorValue & 1;
                minFullValue = min(minFullValue, ((k >> n) | ((k << (neighboursAll - n)) % numPatternsAllPlanes) ) );
            }

            if(rotateInvariant && minValue != k)
            {
                continue;
            }

            if(uniformPatterns && transitions > 2)
            {
                continue;
            }
            
            if(rotateInvariant && uniformPatterns && minFullValue != k)
            {
                continue;
            }

            resultPatternCount++;
        }
        if(uniformPatterns)
        {
            resultPatternCount++;
        }
    }
    else
    {
        resultPatternCount = numPatternsAllPlanes;
    }

    return resultPatternCount;
}

void VLBPFeature::setSettings(VLBPFeature::Settings settings)
{
    if( !(settings.radiusX > 0 && settings.radiusY > 0 && settings.radiusT > 0) )
    {
        IVCP_EXCEPTION(IncorrectParamException, "VLBP Feature radius should be greater than 0" );
    }

    if( settings.neighbours < 1 )
    {
        IVCP_EXCEPTION(IncorrectParamException, "There should be at least 1 neighbour on each plane to calculate VLBP" );
    }

    if( settings.neighbours > 9 )     //Otherwise LBP could not be calculated precisely
    {
        IVCP_EXCEPTION(IncorrectParamException, "Could not calculate VLBP with more than 9 neighbours on a plane" );
    }

    if( !(settings.gridX > 0 && settings.gridY > 0 && settings.gridT > 0) )
    {
        IVCP_EXCEPTION(IncorrectParamException, "VLBP grid size should be greater than 0 for each dimension" );
    }

    if( (settings.sizeT - 2 * settings.radiusT) < settings.gridT )
    {
        IVCP_EXCEPTION(IncorrectParamException, "Used VLBP feature frame sequence is too short for evaluation of the configured grid" );
    }

    radiusX = settings.radiusX;
    radiusY = settings.radiusY;
    radiusT = settings.radiusT;

    neighbours = settings.neighbours;
    neighboursAll = 3 * neighbours + 2;

    gridX = settings.gridX;
    gridY = settings.gridY;
    gridT = settings.gridT;

    normalize = settings.normalizeFeature;
    rotateInvariant = settings.useRotateInvariant;
    uniformPatterns = settings.useUniformPatterns;

    //populate precalculated parameters
    numPatterns = 1 << neighbours;
    numPatternsAllPlanes = 1 << neighboursAll;
    
    numResultPatterns = getNumResultPatterns();

    //inherited property configuration
    featureLength = gridX * gridY * gridT * ( numResultPatterns );

    settingsCopy = settings;
}

const int VLBPFeature::minNonUniformValue = 5;
}
