///////////////////////////////////////////////////////////////////////////
// <copyright file="classifier_feature.cpp" company="Intel Corporation">
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


#include "libivcp/feature/classifier_feature.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
ClassifierFeature::ClassifierFeature(int numberOfSeq, ClassifierOutputType type): 
    Feature(2, numberOfSeq), feature(NULL), classifier(NULL), outputType(type)
{
    if (outputType == CATEGORICAL)
    {
        featureLength = 1;
    }
}

ClassifierFeature::~ClassifierFeature()
{
}

FeatureType ClassifierFeature::getType()
{
    return CLASSIFIER_FEATURE;
}

Mat ClassifierFeature::getFeature(const vector<RegionSequence> &regSeqs)
{
    if (!feature)
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no feature defined for sequence processing" );
    }

    if (!classifier)
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no classifier defined for sequence processing");
    }

    if (regSeqs.size() != numberOfSequences)
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

    Mat internalFeature;
    if (numberOfSequences == feature->getNumberOfSequences())
    {
        internalFeature = feature->getFeature(regSeqs);
    }
    else
    {
        internalFeature.create(1, numberOfSequences * feature->getFeatureLength(), CV_32FC1);
        int partStart = 0;
        for (unsigned int i = 0; i < numberOfSequences; i++)
        {
            vector<RegionSequence> tmp;
            tmp.resize(1);
            tmp[0] = regSeqs[i];
            feature->getFeature(tmp).convertTo(internalFeature.colRange(partStart, partStart + feature->getFeatureLength() ), CV_32FC1);
            partStart += feature->getFeatureLength();
        }
    }
    
    Mat result(1, featureLength, CV_32FC1);
    if (outputType == CATEGORICAL)
    {
        result.at<float>(0, 0) = (float) classifier->clasify(internalFeature);
    }
    else
    {
        vector<float> prob = classifier->getDistribution(internalFeature);
        for (unsigned int i = 0; i < featureLength; i++)
        {
            result.row(0).at<float>(i) = prob[i];
        }
    }

    return result;
}

void ClassifierFeature::setFeature(SPtr<IFeature> newFeature)
{
    if (newFeature == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Feature is a NULL pointer");
    }

    if (newFeature->getNumberOfSequences() == 1)
    {
        if (classifier != NULL && (int)(numberOfSequences * newFeature->getFeatureLength()) != classifier->getVariablesCount())
        {
            IVCP_EXCEPTION(IncorrectParamException, "Number of sequences * length of new feature != classifier variables count");
        }
    }
    else
    {
        if (newFeature->getNumberOfSequences() != numberOfSequences)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Number of region sequences of internal feature should be 1 or number of sequences of classifier feature");
        }

        if (classifier != NULL && (int)(newFeature->getFeatureLength()) != classifier->getVariablesCount())
        {
            IVCP_EXCEPTION(IncorrectParamException, "Internal feature length is not equal to the number of variables in the classifier model");
        }
    }

    feature = newFeature;
    numberOfRegions = feature->getNumberOfRegions();
}

SPtr<IFeature> ClassifierFeature::getInternalFeature()
{
    return feature;
}

void ClassifierFeature::setClassifier(SPtr<IClassifier> newClassifier)
{
    if (newClassifier == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Classifier is a NULL pointer");
    }

    if (!newClassifier->isTrained())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Classifier is not trained");
    }

    if (feature != NULL)
    {
        if (feature->getNumberOfSequences() == 1 && (int)(numberOfSequences * feature->getFeatureLength()) != newClassifier->getVariablesCount())
        {
            IVCP_EXCEPTION(IncorrectParamException, "Number of sequences * length of internal feature != new classifier variables count");
        }
        if (feature->getNumberOfSequences() == numberOfSequences && (int)(feature->getFeatureLength()) != newClassifier->getVariablesCount())
        {
            IVCP_EXCEPTION(IncorrectParamException, "The number of variables in the classifier model is not equal to internal feature length");
        }
    }

    classifier = newClassifier;
    if (outputType == PROBABILISTIC)
    {
        featureLength = classifier->getClassCount();
    }
    else
    {
        featureLength = 1;
    }
}

SPtr<IClassifier> ClassifierFeature::getInternalClassifier()
{
    return classifier;
}

void ClassifierFeature::setOutputType(ClassifierOutputType type)
{
    if (type != PROBABILISTIC && type != CATEGORICAL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Unknown output type");
    }

    outputType = type;
    if (type == CATEGORICAL)
    {
        featureLength = 1;
    }
    else
    {
        if (classifier != NULL)
        {
            featureLength = classifier->getClassCount();
        }
        else
        {
            featureLength = 0;
        }
    }
}

ClassifierOutputType ClassifierFeature::getOutputType()
{
    return outputType;
}

}
