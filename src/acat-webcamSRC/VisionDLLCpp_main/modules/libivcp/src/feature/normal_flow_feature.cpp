///////////////////////////////////////////////////////////////////////////
// <copyright file="normal_flow_feature.cpp" company="Intel Corporation">
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


#include "libivcp/feature/normal_flow_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
NormalFlowFeature::NormalFlowFeature(float _threshold): 
    Feature(2)
{
    featureLength = 1;
    threshold = _threshold;
}

NormalFlowFeature::~NormalFlowFeature()
{
}

Mat NormalFlowFeature::getFeature(const vector<RegionSequence> &regSeqs)
{
    if (regSeqs.size() != 1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of sequences is incorrect" );
    }
    const RegionSequence &regSeq = regSeqs[0];

    if (regSeq.size() != numberOfRegions)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of regions is incorrect" );
    }

    unsigned int minWidth = min(regSeq[0].cols, regSeq[1].cols);
    unsigned int minHeight = min(regSeq[0].rows, regSeq[1].rows);

    if (minWidth == 0 || minHeight == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Minimal region size is 0");
    }

    Mat reg1, reg2;
    if (regSeq[0].type() == CV_8UC3)
    {
        cvtColor(regSeq[0](Rect(0, 0, minWidth, minHeight)), reg1, cv::COLOR_BGR2GRAY);
        cvtColor(regSeq[1](Rect(0, 0, minWidth, minHeight)), reg2, cv::COLOR_BGR2GRAY);
    }
    else
    {
        regSeq[0](Rect(0, 0, minWidth, minHeight)).copyTo(reg1);
        regSeq[1](Rect(0, 0, minWidth, minHeight)).copyTo(reg2);
    }
    reg1.convertTo(reg1, CV_32FC1);
    reg2.convertTo(reg2, CV_32FC1);

    Mat dt = reg2 - reg1;

    Mat dx, dy;
    Sobel(reg2, dx, CV_32FC1, 1, 0);
    Sobel(reg2, dy, CV_32FC1, 0, 1);

    Mat nf = Mat::zeros(dx.rows, dx.cols, CV_32FC1);
    float *dxData = (float *)dx.data;
    float *dyData = (float *)dy.data;
    float *dtData = (float *)dt.data;
    float *nfData = (float *)nf.data;

    for (int y = 0; y < dx.rows; y++)
    {
        for (int x = 0; x < dx.cols; x++)
        {
            float dxv = dxData[x + dx.step1() * y];
            float dyv = dyData[x + dy.step1() * y];
            float dxdyQSum = dxv * dxv + dyv * dyv;
            if (dxdyQSum > numeric_limits<float>::epsilon())
            {
                float dtv = dtData[x + dt.step1() * y];
                float value = dtv * dtv / dxdyQSum;
                if (value <= threshold)
                {
                    value = 0;
                }
                nfData[x + nf.step1() * y] = value;
            }
        }
    }

    Mat nfSum(1, 1, CV_32FC1);
    nfSum.at<float>(0,0) = (float)cv::sum(nf)(0);

    return nfSum;
}

FeatureType NormalFlowFeature::getType()
{
    return NORMALFLOW;
}

float NormalFlowFeature::getThreshold()
{
    return threshold;
}

}
