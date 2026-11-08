///////////////////////////////////////////////////////////////////////////
// <copyright file="feature.cpp" company="Intel Corporation">
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


#include <algorithm>
#include "libivcp/feature/feature.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
Feature::Feature(int nRegions, int nSequences):
    numberOfRegions(nRegions), numberOfSequences(nSequences)
{
    featureLength = 0;
}

unsigned int Feature::getNumberOfRegions()
{
    return numberOfRegions;
}

unsigned int Feature::getNumberOfSequences()
{
    return numberOfSequences;
}

unsigned int Feature::getFeatureLength()
{
    if(!featureLength)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Feature length is undefined. Please verify feature implementation" );
    }
    return featureLength;
}

FilterType Feature::getRootType() const
{
    return FEATURE_FILTER;
}

}
