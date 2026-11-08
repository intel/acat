///////////////////////////////////////////////////////////////////////////
// <copyright file="aggregator_feature.cpp" company="Intel Corporation">
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
#include "libivcp/feature/aggregator_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
AggregatorFeature::AggregatorFeature(int nRegions, int nSequences): 
    Feature(nRegions, nSequences) 
{
}

AggregatorFeature::~AggregatorFeature()
{
}

FeatureType AggregatorFeature::getType()
{
    return AGGREGATOR;
}

Mat AggregatorFeature::getFeature(const vector<RegionSequence> &regSeqs)
{
    if (regSeqs.size() != getNumberOfSequences())
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

    if (features.empty())
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no feature for sequence processing" );
    }


    Mat result(1, featureLength, CV_32FC1);
    int partStart = 0;
    for(auto iter = features.begin(); iter != features.end(); iter++)
    {
        (*iter)->getFeature( regSeqs ).convertTo(result.colRange( partStart, partStart + (*iter)->getFeatureLength() ), CV_32FC1);
        partStart += (*iter)->getFeatureLength();
    }

    return result;
}

void AggregatorFeature::setNumberOfRegions(unsigned int newNumberOfRegions)
{
    if(numberOfRegions != newNumberOfRegions)
    {
        numberOfRegions = newNumberOfRegions;
        featureLength = 0;
        features.clear();
    }
}

void AggregatorFeature::addFeature(SPtr<IFeature> feature)
{
    if (0 == feature)
    {
        IVCP_EXCEPTION(IncorrectParamException, "New feature is a NULL pointer");
    }
    
    if (0 == numberOfRegions) 
    {
        //Auto adjust expected region sequence in case it wasn't preconfigured
        numberOfRegions = feature->getNumberOfRegions();
    }

    if (feature->getNumberOfSequences() != getNumberOfSequences())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Provided feature could not be evaluated due to incorrect number of sequences" );
    }

    if (feature->getNumberOfRegions() != numberOfRegions)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Provided feature could not be evaluated due to incorrect frame sequence size" );
    }

    featureLength += feature->getFeatureLength();
    features.push_back(feature);
}

void AggregatorFeature::delFeature(SPtr<IFeature> feature)
{
    auto iter = find(features.begin(), features.end(), feature);
    if(iter != features.end())
    {
        featureLength -= feature->getFeatureLength();
        features.erase(iter);
    }
}

list<SPtr<IFeature> >& AggregatorFeature::getFeatureList()
{
    return features;
}

}
