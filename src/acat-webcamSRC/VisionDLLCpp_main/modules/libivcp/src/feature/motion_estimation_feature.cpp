///////////////////////////////////////////////////////////////////////////
// <copyright file="motion_estimation_feature.cpp" company="Intel Corporation">
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

#include "libivcp/feature/motion_estimation_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
MotionEstimationFeature::MotionEstimationFeature(int nSequences)
    : Feature(2, nSequences)
{
    featureLength = nSequences;
}

MotionEstimationFeature::~MotionEstimationFeature()
{
}

void MotionEstimationFeature::getIntersectionOfRegionSequences(const vector<RegionSequence> &regSeqs, vector<vector<Mat> > &intersectionOfRegionSequences)
{
    vector<unsigned int> minWidth;
    vector<unsigned int> minHeight;

    for (size_t i = 0; i < regSeqs.size(); i++)
    {
        const RegionSequence &regSeq = regSeqs[i];
        minWidth.push_back (min(regSeq[0].cols, regSeq[1].cols));
        minHeight.push_back(min(regSeq[0].rows, regSeq[1].rows));
    }

    for (size_t i = 0; i < regSeqs.size(); i++)
    {        
        vector<Mat> intersection;
        intersection.resize(numberOfRegions);
        if (regSeqs[i][0].type() == CV_8UC3)
        {            
            cvtColor(regSeqs[i][0](Rect(0, 0, minWidth[i], minHeight[i])), intersection[0], cv::COLOR_BGR2GRAY);
            cvtColor(regSeqs[i][1](Rect(0, 0, minWidth[i], minHeight[i])), intersection[1], cv::COLOR_BGR2GRAY);
        }
        else
        {
            regSeqs[i][0](Rect(0, 0, minWidth[i], minHeight[i])).copyTo(intersection[0]);
            regSeqs[i][1](Rect(0, 0, minWidth[i], minHeight[i])).copyTo(intersection[1]);
        }
        intersectionOfRegionSequences.push_back(intersection);
    }
}

Mat MotionEstimationFeature::getFeature(const vector<RegionSequence> &regSeqs)
{
    if (regSeqs.size() != featureLength)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of sequences is incorrect" );
    }
    const RegionSequence &regSeq = regSeqs[0];

    if (regSeq.size() != numberOfRegions)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of regions is incorrect" );
    }

    if (regSeq[0].type() != CV_8UC1 && regSeq[0].type() != CV_8UC3
        && regSeq[0].type() != CV_16UC1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Type of regions is incorrect" );
    }

    vector<vector<Mat> > intersectionOfRegionSequences;
    getIntersectionOfRegionSequences(regSeqs, intersectionOfRegionSequences);

    Mat motionEstimation(1, featureLength, CV_32FC1);
    for (unsigned int i = 0; i < featureLength; i++)
    {
        motionEstimation.at<float>(i) = estimate(intersectionOfRegionSequences[i]);
    }

    return motionEstimation;
}

}
