///////////////////////////////////////////////////////////////////////////
// <copyright file="summator_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/summator_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
SummatorFeatureXMLSerializer::SummatorFeatureXMLSerializer(SPtr<IRepository> _rep):
    FeatureXMLSerializer(_rep)
{
}

SummatorFeatureXMLSerializer::~SummatorFeatureXMLSerializer()
{
}

SPtr<Filter> SummatorFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto featureMap = loadMap(fs, fn);

    SPtr<SummatorFeature> feature = filter;
    if (feature == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to SummatorFeature");
        }
        bool countMode = featureMap["count_mode"].ival ? true : false;
        bool normalize = featureMap["normalize"].ival ? true : false;
        feature = Factory::createSummatorFeature( SummatorFeature::Settings(featureMap["number_of_regions"].ival, countMode, 
                                                                            featureMap["threshold"].fval, normalize) );
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

    return feature;
}

void SummatorFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<SummatorFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to SummatorFeature");
    }

    auto summatorMap = createMap(filter);
    saveMap(summatorMap, fs); 

    auto internalFeature = feature->getInternalFeature();
    if (internalFeature != NULL)
    {
        rep->save(internalFeature);
    }
}

map<string, IXMLSerializer::SerializeData> SummatorFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto summatorMap = FeatureXMLSerializer::loadMap(fs, fn);

    if (!fn["count_mode"].empty())
    {
        bool countMode = false;
        fn["count_mode"] >> countMode;
        summatorMap["count_mode"] = countMode;
    }

    if (!fn["normalize"].empty())
    {
        bool normalize = false;
        fn["normalize"] >> normalize;
        summatorMap["normalize"] = normalize;
    }

    if (!fn["threshold"].empty())
    {
        float threshold = 0;
        fn["threshold"] >> threshold;
        summatorMap["threshold"] = threshold;
    }

    if (!fn["feature_id"].empty())
    {
        string featureId = "";
        fn["feature_id"] >> featureId;
        summatorMap["feature_id"] = featureId;
    }

    return summatorMap;
}

map<string, IXMLSerializer::SerializeData> SummatorFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto summatorMap = FeatureXMLSerializer::createMap(filter);
    
    SPtr<SummatorFeature> feature = filter;
    if (feature == NULL)
    {
        return summatorMap;
    }

    auto settings = feature->getSettings();
    summatorMap["count_mode"] = settings.countMode;
    summatorMap["normalize"] = settings.normalize;
    summatorMap["threshold"] = settings.threshold;

    auto internalFeature = feature->getInternalFeature();
    if (internalFeature != NULL)
    {
        summatorMap["feature_id"] = internalFeature->getId();
    }

    return summatorMap;
}

}
