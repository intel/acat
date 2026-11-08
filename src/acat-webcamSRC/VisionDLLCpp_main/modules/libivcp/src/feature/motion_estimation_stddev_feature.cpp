///////////////////////////////////////////////////////////////////////////
// <copyright file="motion_estimation_stddev_feature.cpp" company="Intel Corporation">
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


#include "libivcp/feature/motion_estimation_stddev_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
MotionEstimationStdDevFeature::MotionEstimationStdDevFeature(int nSequences)
    : MotionEstimationFeature(nSequences)
{
}

MotionEstimationStdDevFeature::~MotionEstimationStdDevFeature()
{
}

FeatureType MotionEstimationStdDevFeature::getType()
{
    return MOTIONESTIMATIONSTDDEV;
}

float MotionEstimationStdDevFeature::estimate(const vector<Mat> &regions)
{
    Mat diff;
    absdiff(regions[0], regions[1], diff);
    Scalar mean;
    Scalar stddev;
    meanStdDev(diff, mean, stddev);
    return (float)stddev(0);
}
}
