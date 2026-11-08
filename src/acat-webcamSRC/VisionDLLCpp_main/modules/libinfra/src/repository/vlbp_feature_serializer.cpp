///////////////////////////////////////////////////////////////////////////
// <copyright file="vlbp_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/vlbp_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
VLBPFeatureXMLSerializer::VLBPFeatureXMLSerializer(SPtr<IRepository> _rep):
    FeatureXMLSerializer(_rep)
{
}

VLBPFeatureXMLSerializer::~VLBPFeatureXMLSerializer()
{
}

SPtr<Filter> VLBPFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<VLBPFeature> feature = filter;
    if (NULL == feature)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to VLBPFeature for serialization");
        }
        feature = filter = Factory::createVLBPFeature();
    }

    auto featureMap = this->loadMap(fs, fn);
    VLBPFeature::Settings settings;

    settings.sizeT = featureMap["size_t"].ival;
    settings.radiusX = featureMap["radius_x"].ival;
    settings.radiusY = featureMap["radius_y"].ival;
    settings.radiusT = featureMap["radius_t"].ival;
    settings.neighbours = featureMap["neighbours"].ival;
    settings.gridX = featureMap["grid_x"].ival;
    settings.gridY = featureMap["grid_y"].ival;
    settings.gridT = featureMap["grid_t"].ival;
    settings.normalizeFeature = (featureMap["normalize_feature"].ival) ? (true) : (false);
    settings.useRotateInvariant = (featureMap["use_rotate_invariant"].ival) ? (true) : (false);
    settings.useUniformPatterns = (featureMap["use_uniform_patterns"].ival) ? (true) : (false);

    feature->setSettings(settings);

    FeatureXMLSerializer::load(fs, fn, feature);
    return feature;
}

void VLBPFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<VLBPFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to VLBPFeature for serialization");
    }

    auto featureMap = createMap(filter);
    this->saveMap(featureMap, fs);
}

map<string, IXMLSerializer::SerializeData> VLBPFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto featureMap = FeatureXMLSerializer::loadMap(fs, fn);

    if (!fn["size_t"].empty())
    {
        fn["size_t"] >> featureMap["size_t"].ival;
    }

    if (!fn["radius_x"].empty())
    {
        fn["radius_x"] >> featureMap["radius_x"].ival;
    }

    if (!fn["radius_y"].empty())
    {
        fn["radius_y"] >> featureMap["radius_y"].ival;
    }

    if (!fn["radius_t"].empty())
    {
        fn["radius_t"] >> featureMap["radius_t"].ival;
    }

    if (!fn["neighbours"].empty())
    {
        fn["neighbours"] >> featureMap["neighbours"].ival;
    }

    if (!fn["grid_x"].empty())
    {
        fn["grid_x"] >> featureMap["grid_x"].ival;
    }

    if (!fn["grid_y"].empty())
    {
        fn["grid_y"] >> featureMap["grid_y"].ival;
    }

    if (!fn["grid_t"].empty())
    {
        fn["grid_t"] >> featureMap["grid_t"].ival;
    }

    if (!fn["normalize_feature"].empty())
    {
        fn["normalize_feature"] >> featureMap["normalize_feature"].ival;
    }

    if (!fn["use_rotate_invariant"].empty())
    {
        fn["use_rotate_invariant"] >> featureMap["use_rotate_invariant"].ival;
    }

    if (!fn["use_uniform_patterns"].empty())
    {
        fn["use_uniform_patterns"] >> featureMap["use_uniform_patterns"].ival;
    }

    return featureMap;
}

void VLBPFeatureXMLSerializer::saveMap(map<string, SerializeData> &filterData, FileStorage &fs)
{
    FeatureXMLSerializer::saveMap(filterData, fs);
}

map<string, IXMLSerializer::SerializeData> VLBPFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto featureMap = FeatureXMLSerializer::createMap(filter);

    SPtr<VLBPFeature> feature = filter;
    if (NULL == feature)
    {
        return featureMap;
    }

    VLBPFeature::Settings settings = feature->getSettings();
    featureMap["size_t"] = settings.sizeT;
    featureMap["radius_x"] = settings.radiusX;
    featureMap["radius_y"] = settings.radiusY;
    featureMap["radius_t"] = settings.radiusT;
    featureMap["neighbours"] = settings.neighbours;
    featureMap["grid_x"] = settings.gridX;
    featureMap["grid_y"] = settings.gridY;
    featureMap["grid_t"] = settings.gridT;
    featureMap["normalize_feature"] = (int)settings.normalizeFeature;
    featureMap["use_rotate_invariant"] = (int)settings.useRotateInvariant;
    featureMap["use_uniform_patterns"] = (int)settings.useUniformPatterns;

    return featureMap;
}

}

