///////////////////////////////////////////////////////////////////////////
// <copyright file="normal_flow_blurred_feature.cpp" company="Intel Corporation">
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


#include "libivcp/feature/normal_flow_blurred_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{

typedef Matx<float, 1, 5> Matx15f;
typedef Matx<float, 5, 1> Matx51f;

NormalFlowBlurredFeature::NormalFlowBlurredFeature(float _threshold, NFBFType _outputType, bool _robustFlag): 
    Feature(5)
{
    featureLength = 1;
    threshold = _threshold;
    outputType = _outputType;
    robustFlag = _robustFlag;
}

NormalFlowBlurredFeature::~NormalFlowBlurredFeature()
{
}

static Mat filterInT(const vector<Mat>& regions, const Matx15f& kernel)
{
    const size_t kernelSize = kernel.cols;
    CV_Assert(regions.size() == kernelSize);
    CV_Assert(regions[0].type() == CV_32FC1);

    Mat dst = Mat::zeros(regions[0].rows, regions[0].cols, CV_32FC1);
    for (int y = 0; y < regions[0].rows; y++)
    {
        vector<float*> srcRows;
        for (size_t i = 0; i < regions.size(); i++)
        {
            srcRows.push_back((float*)regions[i].row(y).data);
        }
        float* dstRow = (float*)dst.row(y).data;

        for (int x = 0; x < regions[0].cols; x++)
        {
            for (unsigned int i = 0; i < kernelSize; i++)
            {
                dstRow[x] += srcRows[i][x] * kernel(i);
            }
        }
    }
    return dst;
}

static vector<Mat> computeBlurredGradient(const vector<Mat>& regions)
{
    enum {X = 0, Y, T};
    vector<Mat> gradients(3);

    Matx15f blurKernelSpaceHor(0.03504f, 0.24878f, 0.43234f, 0.24878f, 0.03504f);
    Matx51f blurKernelSpaceVert(blurKernelSpaceHor.val);
    Matx15f blurKernelTime = blurKernelSpaceHor;
    Matx15f derKernelSpaceHor(-0.10689f, -0.28461f, 0.0f, 0.28461f, 0.10689f);
    Matx51f derKernelSpaceVert(derKernelSpaceHor.val);
    Matx15f derKernelTime(0.10689f, 0.28461f, 0.0f, -0.28461f, -0.10689f);

    Mat filterInTimeResult = filterInT(regions, blurKernelTime);
    const int width = regions[0].cols;
    const int height = regions[0].rows;

    const int borderSize = derKernelSpaceHor.cols / 2;
    int roiX = borderSize;
    int roiY = borderSize;
    int roiW = width  - borderSize*2;
    int roiH = height - borderSize*2;
    if (roiW <= 0)
    {
        roiX = max(0, (int)(width / 2));
        roiW = 1;
    }
    if (roiH <= 0)
    {
        roiY = max(0, (int)(height / 2));
        roiH = 1;
    }
    Rect roi(roiX, roiY, roiW, roiH);

    Mat filterInSpaceResult(height, width, CV_32FC1);
    filter2D(filterInTimeResult, filterInSpaceResult, CV_32F, derKernelSpaceHor);
    filter2D(filterInSpaceResult, gradients[X], CV_32F, blurKernelSpaceVert);
    gradients[X] = gradients[X](roi);

    filter2D(filterInTimeResult, filterInSpaceResult, CV_32F, blurKernelSpaceHor);
    filter2D(filterInSpaceResult, gradients[Y], CV_32F, derKernelSpaceVert);
    gradients[Y] = gradients[Y](roi);

    filterInTimeResult = filterInT(regions, derKernelTime);
    filter2D(filterInTimeResult, filterInSpaceResult, CV_32F, blurKernelSpaceHor);
    filter2D(filterInSpaceResult, gradients[T], CV_32F, blurKernelSpaceVert);
    gradients[T] = gradients[T](roi);

    return gradients;
}

Mat NormalFlowBlurredFeature::getFeature(const vector<RegionSequence> &regSeqs)
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

    unsigned int minWidth = min( min(regSeq[0].cols, regSeq[1].cols), 
        min(regSeq[2].cols, regSeq[3].cols < regSeq[4].cols ? regSeq[3].cols : regSeq[4].cols) );
    unsigned int minHeight = min( min(regSeq[0].rows, regSeq[1].rows),
        min(regSeq[2].rows, regSeq[3].rows < regSeq[4].rows ? regSeq[3].rows : regSeq[4].rows) );

    if (minWidth == 0 || minHeight == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Minimal region size is 0");
    }

    vector<Mat> regions;
    regions.resize(numberOfRegions);

    for (size_t i = 0; i < numberOfRegions; i++)
    {
        if (regSeq[0].type() == CV_8UC3)
        {
            cvtColor(regSeq[i](Rect(0, 0, minWidth, minHeight)), regions[i], cv::COLOR_BGR2GRAY);
        }
        else
        {
            regSeq[i](Rect(0, 0, minWidth, minHeight)).copyTo(regions[i]);
        }
        regions[i].convertTo(regions[i], CV_32FC1);
    }

    vector<Mat> gradients = computeBlurredGradient(regions);

    const float eps = 0.1f;
    Mat normalFlow = Mat::zeros(gradients[0].rows, gradients[0].cols, gradients[0].type());

    int width = 2;
    int height = gradients[0].rows * gradients[0].cols;
    int realHeight = 0;

    Mat A = Mat::zeros(1, width * height, CV_32FC1);
    Mat b = Mat::zeros(1, height, CV_32FC1);
    float *pA = (float*)A.data;
    float *pb = (float*)b.data;

    for (int y = 0; y < gradients[0].rows; y++)
    {
        float* gxRow = (float*)gradients[0].row(y).data;
        float* gyRow = (float*)gradients[1].row(y).data;
        float* gtRow = (float*)gradients[2].row(y).data;
        float* nfRow = (float*)normalFlow.row(y).data;
        for (int x = 0; x < gradients[0].cols; x++)
        {
            float nf = abs(gtRow[x]) / (sqrt(gxRow[x]*gxRow[x] + gyRow[x]*gyRow[x]) + eps);
            if (nf >= threshold)
            {
				nfRow[x] = nf; 
            }
			if (outputType != NFBF_ENERGY) 
			{
				pA[2 * realHeight] = gxRow[x];
				pA[2 * realHeight + 1] = gyRow[x];
				pb[realHeight] = -gtRow[x];
				realHeight++;
			}
        }
    }

    Mat flow = Mat::zeros(2, 1, CV_32FC1);
    float dx = 0.0;
    float dy = 0.0;  

    if (outputType != NFBF_ENERGY)
    {  
        if (realHeight > 10)
        {
            A = A.reshape(1, height);
            b = b.reshape(1, height);
            //solve using opencv function
            bool isOk = solve(A, b, flow, DECOMP_SVD);
            if (isOk)
            {
                dx = flow.at<float>(0);
                dy = flow.at<float>(1);
            }        
        }
    }


    Mat nfSum;
    switch (outputType)
    {
    case NFBF_ENERGY:
        nfSum = (Mat_<float>(1,1) << (float)sum(normalFlow)(0));
        break;
    case NFBF_DX_DY:
        nfSum = (Mat_<float>(1,2) << dx, dy);
        break;
    case NFBF_ENERGY_DX_DY:
        nfSum = (Mat_<float>(1,3) << (float)mean(normalFlow)(0), dx, dy); //test with mean instead of sum
        break;
    default:
        break;
    }
    return nfSum;
}

FeatureType NormalFlowBlurredFeature::getType()
{
    return NORMALFLOWBLURRED;
}

float NormalFlowBlurredFeature::getThreshold()
{
    return threshold;
}

unsigned int NormalFlowBlurredFeature::getFeatureLength()
{
    unsigned int length = 1;
    switch (outputType)
    {
    case NFBF_ENERGY:
        length = 1;
        break;
    case NFBF_DX_DY:
        length = 2;
        break;
    case NFBF_ENERGY_DX_DY:
        length = 3;
        break;
    default:
        break;
    }
    return length;
}
}
