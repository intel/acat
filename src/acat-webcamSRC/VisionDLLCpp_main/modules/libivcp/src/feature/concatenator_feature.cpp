///////////////////////////////////////////////////////////////////////////
// <copyright file="concatenator_feature.cpp" company="Intel Corporation">
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


#include "libivcp/feature/concatenator_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
ConcatenatorFeature::ConcatenatorFeature(int nRegions): 
    Feature(nRegions), feature(NULL), numberOfEvaluatedSubsequences(0)
{
}

ConcatenatorFeature::~ConcatenatorFeature()
{
}

FeatureType ConcatenatorFeature::getType()
{
    return CONCATENATOR;
}

Mat ConcatenatorFeature::getFeature(const vector<RegionSequence> &regSeqs)
{
    if (!feature)
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no feature defined for sequence processing" );
    }

    if (regSeqs.size() != feature->getNumberOfSequences())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of sequences is incorrect" );
    }

    for(unsigned int s = 0; s < regSeqs.size(); s++)
    {
        if (regSeqs[s].size() != numberOfRegions)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Number of regions is incorrect" );
        }
    }

    Mat result(1, featureLength, CV_32FC1);
    int partStart = 0;
    for (int n = 0; n < (int)numberOfEvaluatedSubsequences; n++)
    {
        size_t numberOfSeqs = regSeqs.size();
        vector<RegionSequence> regionsSeq(numberOfSeqs);
    
        for (size_t i = 0; i < numberOfSeqs; i++)
        {
            for (unsigned int j = 0; j < feature->getNumberOfRegions(); j++)
            {
                regionsSeq[i].push_back(regSeqs[i][n+j]);
            }
        }
        feature->getFeature( regionsSeq ).convertTo(result.colRange( partStart, partStart + feature->getFeatureLength() ), CV_32FC1);
        partStart += feature->getFeatureLength();
    }

    return result;
}

void ConcatenatorFeature::setFeature(SPtr<IFeature> newFeature)
{
    if (newFeature == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Feature is a NULL pointer");
    }

    if (newFeature->getNumberOfRegions() > numberOfRegions)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Frame sequence used by ConcatenatorFeature is shorter than neccesary to evaluate provided feature" );
    }

    numberOfEvaluatedSubsequences = numberOfRegions - newFeature->getNumberOfRegions() + 1;
    featureLength = newFeature->getFeatureLength() * numberOfEvaluatedSubsequences;
    feature = newFeature;

    this->numberOfSequences = newFeature->getNumberOfSequences();
}

SPtr<IFeature> ConcatenatorFeature::getInternalFeature()
{
    return feature;
}

void ConcatenatorFeature::setNumberOfRegions(int nRegions)
{
    if (0 != feature)
    {
        if (feature->getNumberOfRegions() > (unsigned int)nRegions)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Frame sequence used by ConcatenatorFeature is shorter than necessary to evaluate provided feature" );
        }
    }

    if (nRegions <= 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of regions must be a positive number");
    }

    numberOfRegions = nRegions;
}

}
