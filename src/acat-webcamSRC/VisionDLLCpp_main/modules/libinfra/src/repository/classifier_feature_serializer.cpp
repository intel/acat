////////////////////////////////////////////////////////////////////////////
// <copyright file="classifier_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/classifier_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
ClassifierFeatureXMLSerializer::ClassifierFeatureXMLSerializer(SPtr<IRepository> _rep):
    FeatureXMLSerializer(_rep)
{
}

ClassifierFeatureXMLSerializer::~ClassifierFeatureXMLSerializer()
{
}

SPtr<Filter> ClassifierFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto featureMap = loadMap(fs, fn);

    SPtr<ClassifierFeature> feature = filter;
    if (feature == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to ClassifierFeature");
        }
        ClassifierOutputType outputType = (ClassifierOutputType)featureMap["classifier_output_type"].ival;
        feature = Factory::createClassifierFeature(featureMap["number_of_sequences"].ival, outputType);
        filter = feature;
    }

    FeatureXMLSerializer::load(fs, fn, feature);

    if (!fn["feature_id"].empty())
    {
        string featureId = "";
        fn["feature_id"] >> featureId;
        SPtr<IFeature > internalFeature = rep->loadFeature(featureId);
        feature->setFeature(internalFeature);
    }

    if (!fn["classifier_id"].empty())
    {
        string classifierId = "";
        fn["classifier_id"] >> classifierId;
        SPtr<IClassifier> internalClassifier = rep->loadClassifier(classifierId);
        feature->setClassifier(internalClassifier);
    }

    return feature;
}

void ClassifierFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<ClassifierFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to ClassifierFeature");
    }

    auto featureMap = createMap(filter);
    saveMap(featureMap, fs); 

    auto internalFeature = feature->getInternalFeature();
    if (internalFeature != NULL)
    {
        rep->save(internalFeature);
    }

    auto internalClassifier = feature->getInternalClassifier();
    if (internalClassifier != NULL)
    {
        rep->save(internalClassifier);
    }
}

map<string, IXMLSerializer::SerializeData> ClassifierFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto featureMap = FeatureXMLSerializer::loadMap(fs, fn);

    if (!fn["classifier_output_type"].empty())
    {
        int outputType = PROBABILISTIC;
        fn["classifier_output_type"] >> outputType;
        featureMap["classifier_output_type"] = outputType;
    }

    if (!fn["feature_id"].empty())
    {
        string featureId = "";
        fn["feature_id"] >> featureId;
        featureMap["feature_id"] = featureId;
    }

    if (!fn["classifier_id"].empty())
    {
        string classifierId = "";
        fn["classifier_id"] >> classifierId;
        featureMap["classifier_id"] = classifierId;
    }

    return featureMap;
}

map<string, IXMLSerializer::SerializeData> ClassifierFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto featureMap = FeatureXMLSerializer::createMap(filter);
    
    SPtr<ClassifierFeature> feature = filter;
    if (feature == NULL)
    {
        return featureMap;
    }

    featureMap["classifier_output_type"] = (int) feature->getOutputType();

    auto internalFeature = feature->getInternalFeature();
    if (internalFeature != NULL)
    {
        featureMap["feature_id"] = internalFeature->getId();
    }

    auto internalClassifier = feature->getInternalClassifier();
    if (internalClassifier != NULL)
    {
        featureMap["classifier_id"] = internalClassifier->getId();
    }

    return featureMap;
}

}
