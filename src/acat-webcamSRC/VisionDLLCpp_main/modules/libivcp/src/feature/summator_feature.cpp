///////////////////////////////////////////////////////////////////////////
// <copyright file="summator_feature.cpp" company="Intel Corporation">
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


#include "libivcp/feature/summator_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
SummatorFeature::SummatorFeature(SummatorFeature::Settings settings): 
    Feature(settings.nRegions), feature(NULL), normalize(settings.normalize), countMode(settings.countMode), threshold(settings.threshold), numberOfEvaluatedSubsequences(0) 
{
}

SummatorFeature::~SummatorFeature()
{
}

FeatureType SummatorFeature::getType()
{
    return SUMMATOR;
}

Mat SummatorFeature::getFeature(const vector<RegionSequence> &regSeqs)
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

    Mat result = Mat::zeros(1, featureLength, CV_32FC1);
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
        if(countMode)
        {
            Mat tmp;
            cv::threshold(feature->getFeature( regionsSeq ), tmp, threshold, 1, THRESH_BINARY);
            result += tmp;
        }
        else
        {
            result += feature->getFeature( regionsSeq );
        }
    }
    if(normalize)
    {
        result /= numberOfEvaluatedSubsequences;
    }
    return result;
}

void SummatorFeature::setFeature(SPtr<IFeature> newFeature)
{
    if (0 == newFeature)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Feature is a NULL pointer");
    }

    if (newFeature->getNumberOfRegions() > numberOfRegions)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Region Sequence used by SummatorFeature is shorter than neccesary to evaluate the internal Feature" );
    }

    numberOfEvaluatedSubsequences = numberOfRegions - newFeature->getNumberOfRegions() + 1;
    featureLength = newFeature->getFeatureLength();
    feature = newFeature;

    this->numberOfSequences = newFeature->getNumberOfSequences();
}

SPtr<IFeature> SummatorFeature::getInternalFeature()
{
    return feature;
}

SummatorFeature::Settings SummatorFeature::getSettings()
{
    return SummatorFeature::Settings(numberOfRegions, countMode, threshold, normalize);
}

void SummatorFeature::setNumberOfRegions(int nRegions)
{
    if (0 != feature)
    {
        if (feature->getNumberOfRegions() > (unsigned int)nRegions)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Region Sequence used by SummatorFeature is shorter than neccesary to evaluate the internal Feature" );
        }
    }

    if (nRegions <= 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of regions must be a positive number");
    }

    numberOfRegions = nRegions;
}

}
