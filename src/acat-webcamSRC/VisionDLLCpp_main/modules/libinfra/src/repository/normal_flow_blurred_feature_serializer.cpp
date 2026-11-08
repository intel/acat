///////////////////////////////////////////////////////////////////////////
// <copyright file="normal_flow_blurred_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/normal_flow_blurred_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
NormalFlowBlurredFeatureXMLSerializer::NormalFlowBlurredFeatureXMLSerializer(SPtr<IRepository> rep):
    NormalFlowFeatureXMLSerializer(rep)
{
}

NormalFlowBlurredFeatureXMLSerializer::~NormalFlowBlurredFeatureXMLSerializer()
{
}

SPtr<Filter> NormalFlowBlurredFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto featureMap = loadMap(fs, fn);

    SPtr<NormalFlowBlurredFeature> feature = filter;
    if (feature == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to NormalFlowBlurredFeature");
        }
        feature = Factory::createNormalFlowBlurredFeature(featureMap["threshold"].fval);
        filter = feature;
    }

    FeatureXMLSerializer::load(fs, fn, feature);
    return feature;
}

void NormalFlowBlurredFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<NormalFlowBlurredFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to NormalFlowBlurredFeature for serialization");
    }

    auto nfMap = createMap(filter);
    saveMap(nfMap, fs);
}

map<string, IXMLSerializer::SerializeData> NormalFlowBlurredFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto nfMap = FeatureXMLSerializer::createMap(filter);

    SPtr<NormalFlowBlurredFeature> feature = filter;
    if (feature == NULL)
    {
        return nfMap;
    }

    nfMap["threshold"] = feature->getThreshold();
    return nfMap;
}

}
