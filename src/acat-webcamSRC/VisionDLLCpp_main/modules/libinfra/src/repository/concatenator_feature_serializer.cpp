////////////////////////////////////////////////////////////////////////////
// <copyright file="concatenator_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/concatenator_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
ConcatenatorFeatureXMLSerializer::ConcatenatorFeatureXMLSerializer(SPtr<IRepository> _rep):
    FeatureXMLSerializer(_rep)
{
}

ConcatenatorFeatureXMLSerializer::~ConcatenatorFeatureXMLSerializer()
{
}

SPtr<Filter> ConcatenatorFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto featureMap = loadMap(fs, fn);

    SPtr<ConcatenatorFeature> feature = filter;
    if (feature == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to ConcatenatorFeature");
        }
        feature = Factory::createConcatenatorFeature(featureMap["number_of_regions"].ival);
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

void ConcatenatorFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<ConcatenatorFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to ConcatenatorFeature");
    }

    auto concatenatorMap = createMap(filter);
    saveMap(concatenatorMap, fs); 

    auto internalFeature = feature->getInternalFeature();
    if (internalFeature != NULL)
    {
        rep->save(internalFeature);
    }
}

map<string, IXMLSerializer::SerializeData> ConcatenatorFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto concatenatorMap = FeatureXMLSerializer::loadMap(fs, fn);

    if (!fn["feature_id"].empty())
    {
        string featureId = "";
        fn["feature_id"] >> featureId;
        concatenatorMap["feature_id"] = featureId;
    }

    return concatenatorMap;
}

map<string, IXMLSerializer::SerializeData> ConcatenatorFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto concatenatorMap = FeatureXMLSerializer::createMap(filter);
    
    SPtr<ConcatenatorFeature> feature = filter;
    if (feature == NULL)
    {
        return concatenatorMap;
    }

    auto internalFeature = feature->getInternalFeature();
    if (internalFeature != NULL)
    {
        concatenatorMap["feature_id"] = internalFeature->getId();
    }

    return concatenatorMap;
}

}
