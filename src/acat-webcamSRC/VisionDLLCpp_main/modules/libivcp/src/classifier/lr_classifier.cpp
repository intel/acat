///////////////////////////////////////////////////////////////////////////
// <copyright file="lr_classifier.cpp" company="Intel Corporation">
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
#include <sstream>
#include <time.h>
#include "libivcp/classifier/lr_classifier.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
LRClassifier::LRClassifier(LRParameters _classifierParams)
{
    lr = new LRModel();
    setClassifierParams(_classifierParams);
}

LRClassifier::~LRClassifier()
{
}

ClassType LRClassifier::clasify(Mat feature)
{
    if (!isTrained())
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }
    
    ClassType c = (ClassType)lr->predict(feature);
    return c;
}

void LRClassifier::trainModel()
{
    if (accumulatedFeatures.empty())
    {
        IVCP_EXCEPTION(NoDataException, "No features to train the model");
    }

    if (accumulatedFeatures.size() != accumulatedResponses.size())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of features and number of responses mismatch");
    }

    vector<double> responses(accumulatedResponses.size());
    auto iter = accumulatedResponses.begin();
    for (int i = 0; iter != accumulatedResponses.end(); iter++, i++)
    {
        responses[i] = (*iter);
    }
    
    lr->train(accumulatedFeatures, responses, classifierParams);
}

ClassifierType LRClassifier::getType()
{
    return LOGISTIC_REGRESSION;
}

SPtr<LRModel> LRClassifier::getModel()
{
    return lr;
}

void LRClassifier::setModel(const SPtr<LRModel>& model)
{
    this->lr = model;
}

void LRClassifier::setClassifierParams(LRParameters _classifierParams)
{
    classifierParams = _classifierParams;
}

LRParameters LRClassifier::getClassifierParams()
{
    return classifierParams;
}

vector<float> LRClassifier::getDistribution(Mat feature)
{
    if (!isTrained())
    {
        IVCP_EXCEPTION(NoModelException, "The model is not yet trained");
    }

    return lr->getProbabilities(feature);
}

bool LRClassifier::isTrained()
{
    return lr->isTrained();
}

int LRClassifier::getVariablesCount()
{
    return varCount;
}

int LRClassifier::getClassCount()
{
    if (!isTrained())
    {
        return 0;
    }
    return lr->getClassCount();
}
void LRClassifier::setClassifierParameters(const char *, double) {}
}
