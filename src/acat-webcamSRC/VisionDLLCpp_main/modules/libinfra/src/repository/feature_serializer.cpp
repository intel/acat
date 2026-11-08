///////////////////////////////////////////////////////////////////////////
// <copyright file="feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/feature_serializer.hpp"
#include "libivcp/feature/feature.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
FeatureXMLSerializer::FeatureXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

FeatureXMLSerializer::~FeatureXMLSerializer()
{
}

map<string, IXMLSerializer::SerializeData> FeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    map<string, IXMLSerializer::SerializeData> featureMap = FilterXMLSerializer::loadMap(fs, fn);

    if (!fn["number_of_regions"].empty())
    {
        int numberOfRegions = 0;
        fn["number_of_regions"] >> numberOfRegions;
        featureMap["number_of_regions"] = numberOfRegions;
    }

    if (!fn["number_of_sequences"].empty())
    {
        int numberOfSeq = 0;
        fn["number_of_sequences"] >> numberOfSeq;
        featureMap["number_of_sequences"] = numberOfSeq;
    }

    return featureMap;
}

map<string, IXMLSerializer::SerializeData> FeatureXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto featureMap = FilterXMLSerializer::createMap(filter);
    SPtr<Feature> feature = filter;
    if (feature != NULL)
    {
        featureMap["number_of_regions"] = (int)feature->getNumberOfRegions();
        featureMap["number_of_sequences"] = (int)feature->getNumberOfSequences();
    }

    return featureMap;
}

}
