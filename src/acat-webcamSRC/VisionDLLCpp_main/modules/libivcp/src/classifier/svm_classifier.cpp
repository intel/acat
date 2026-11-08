///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_classifier.cpp" company="Intel Corporation">
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
#include "libivcp/classifier/svm_classifier.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
SVMClassifier::SVMClassifier(SVMParameters _classifierParams)
{
    svm = new SVMModel();
    setClassifierParams(_classifierParams);
}

SVMClassifier::~SVMClassifier()
{
}

ClassType SVMClassifier::clasify(Mat feature)
{
    ClassType c = (ClassType)svm->predict(feature);
    return c;
}

void SVMClassifier::trainModel()
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
    svm->train(accumulatedFeatures, responses, classifierParams);
}

ClassifierType SVMClassifier::getType()
{
    return SVM;
}

SPtr<SVMModel> SVMClassifier::getModel()
{
    return svm;
}

void SVMClassifier::setClassifierParams(SVMParameters _classifierParams)
{
    classifierParams = _classifierParams;
    classifierParams.estimateProbability = true;
}

SVMParameters SVMClassifier::getClassifierParams()
{
    return classifierParams;
}

vector<float> SVMClassifier::getDistribution(Mat feature)
{
    return svm->getProbabilities(feature);
}

bool SVMClassifier::isTrained()
{
    if (svm->getSupportVectorsCount() > 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}

int SVMClassifier::getVariablesCount()
{
    if (svm->getSupportVectorsCount() > 0)
    {
        varCount = svm->getVarCount();
    }
    return varCount;
}

int SVMClassifier::getClassCount()
{
    return svm->getClassCount();
}

void SVMClassifier::setClassifierParameters(const char *, double) {}

}
