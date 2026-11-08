///////////////////////////////////////////////////////////////////////////
// <copyright file="motion_estimation_stddev_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/motion_estimation_stddev_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
MotionEstimationStdDevFeatureXMLSerializer::MotionEstimationStdDevFeatureXMLSerializer(SPtr<IRepository> rep):
    FeatureXMLSerializer(rep)
{
}

MotionEstimationStdDevFeatureXMLSerializer::~MotionEstimationStdDevFeatureXMLSerializer()
{
}

SPtr<Filter> MotionEstimationStdDevFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto featureMap = loadMap(fs, fn);

    SPtr<MotionEstimationStdDevFeature> feature = filter;
    if (feature == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to MotionEstimationStdDevFeature");
        }
        feature = Factory::createMotionEstimationStdDevFeature(featureMap["nSequences"].ival);
        filter = feature;
    }

    FeatureXMLSerializer::load(fs, fn, feature);
    return feature;
}

void MotionEstimationStdDevFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<MotionEstimationStdDevFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to MotionEstimationStdDevFeature for serialization");
    }

    auto featureMap = createMap(filter);
    saveMap(featureMap, fs);
}

map<string, IXMLSerializer::SerializeData> MotionEstimationStdDevFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto featureMap = FeatureXMLSerializer::loadMap(fs, fn);

    int nSequences = 1;
    if (!fn["nSequences"].empty())
    {
        fn["nSequences"] >> nSequences;        
    }
    featureMap["nSequences"] = nSequences;

    return featureMap;
}

map<string, IXMLSerializer::SerializeData> MotionEstimationStdDevFeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto featureMap = FeatureXMLSerializer::createMap(filter);

    SPtr<MotionEstimationStdDevFeature> feature = filter;
    if (feature == NULL)
    {
        return featureMap;
    }

    featureMap["nSequences"] = (int)feature->getFeatureLength();
    return featureMap;
}

}
