///////////////////////////////////////////////////////////////////////////
// <copyright file="classifier.cpp" company="Intel Corporation">
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

#include "libivcp/classifier/classifier.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
Classifier::Classifier()
{
    varCount = 0;
}

Classifier::~Classifier() 
{
}

void Classifier::addTrainFeature(Mat feature, ClassType response)
{
    //Check feature size
    if (!accumulatedFeatures.empty())
    {
        if (accumulatedFeatures.front().cols != feature.cols)
        {
            IVCP_EXCEPTION(MismatchFeatureSizeException, "New feature size is not equal to size of previous features");
        }
    }       

    if (feature.rows != 1)
    {
        IVCP_EXCEPTION(MismatchFeatureSizeException, "Number of rows in feature matrix shall be equal to 1");
    }
    
    if (varCount == 0)
    {
        varCount = feature.cols;
    }
    accumulatedFeatures.push_back(feature);
    accumulatedResponses.push_back(response);
}

FilterType Classifier::getRootType() const 
{
    return CLASSIFIER_FILTER;
}

bool Classifier::isReady() const
{
    return true;
}

}
