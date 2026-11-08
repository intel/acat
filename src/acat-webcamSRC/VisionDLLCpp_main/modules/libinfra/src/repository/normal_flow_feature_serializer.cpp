///////////////////////////////////////////////////////////////////////////
// <copyright file="normal_flow_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/normal_flow_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
NormalFlowFeatureXMLSerializer::NormalFlowFeatureXMLSerializer(SPtr<IRepository> _rep):
    FeatureXMLSerializer(_rep)
{
}

NormalFlowFeatureXMLSerializer::~NormalFlowFeatureXMLSerializer()
{
}

SPtr<Filter> NormalFlowFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto featureMap = loadMap(fs, fn);

    SPtr<NormalFlowFeature> feature = filter;
    if (feature == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to NormalFlowFeature");
        }
        feature = Factory::createNormalFlowFeature(featureMap["threshold"].fval);
        filter = feature;
    }

    FeatureXMLSerializer::load(fs, fn, feature);
    return feature;
}

void NormalFlowFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<NormalFlowFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to NormalFlowFeature for serialization");
    }

    auto nfMap = createMap(filter);
    saveMap(nfMap, fs);
}

map<string, IXMLSerializer::SerializeData> NormalFlowFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto nfMap = FeatureXMLSerializer::loadMap(fs, fn);

    float threshold = 0.f;
    if (!fn["threshold"].empty())
    {
        fn["threshold"] >> threshold;        
    }
    nfMap["threshold"] = threshold;

    return nfMap;
}

map<string, IXMLSerializer::SerializeData> NormalFlowFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto nfMap = FeatureXMLSerializer::createMap(filter);

    SPtr<NormalFlowFeature> feature = filter;
    if (feature == NULL)
    {
        return nfMap;
    }

    nfMap["threshold"] = feature->getThreshold();
    return nfMap;
}

}
