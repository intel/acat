////////////////////////////////////////////////////////////////////////////
// <copyright file="aggregator_feature_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/aggregator_feature_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
AggregatorFeatureXMLSerializer::AggregatorFeatureXMLSerializer(SPtr<IRepository> _rep):
    FeatureXMLSerializer(_rep)
{
}

AggregatorFeatureXMLSerializer::~AggregatorFeatureXMLSerializer()
{
}

SPtr<Filter> AggregatorFeatureXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto aggregatorMap = loadMap(fs, fn);
    int numberOfRegions = aggregatorMap["number_of_regions"].ival;

    SPtr<AggregatorFeature> feature = filter;
    if (NULL == feature)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to AggregatorFeature");
        }

        feature = Factory::createAggregatorFeature(numberOfRegions);
        filter = feature;
    }

    auto ids = aggregatorMap["ids"].strVec;
    for (unsigned int i = 0; i < ids.size(); i++)
    {
        feature->addFeature(rep->loadFeature(ids[i]));
    }

    FeatureXMLSerializer::load(fs, fn, feature);

    return feature;
}

void AggregatorFeatureXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<AggregatorFeature> feature = filter;
    if (feature == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to AggregatorFeature for serialization");
    }

    auto featuresList = feature->getFeatureList();
    fs << "ids" << "[";
    for (auto it = featuresList.begin(); it != featuresList.end(); it++)
    {
        fs << "{";
        fs << "id" << (*it)->getId();
        fs << "}";
        rep->save(*it);
    }
    fs << "]";

    FeatureXMLSerializer::save(filter, fs);
}

map<string, IXMLSerializer::SerializeData> AggregatorFeatureXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    map<string, IXMLSerializer::SerializeData> aggregatorMap = FeatureXMLSerializer::loadMap(fs, fn);

    size_t numberOfFeatures = fn["ids"].size();
    vector<string> data(numberOfFeatures);

    int counter  = 0;
    for (FileNodeIterator it = fn["ids"].begin(); it != fn["ids"].end(); it++, counter++)
    {
        string id = "";
        if (!(*it)["id"].empty())
        {
            (*it)["id"] >> id;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read tag \"id\"");
        }

        data[counter] = id;
    }
    aggregatorMap["ids"] = data;

    return aggregatorMap;
}

void AggregatorFeatureXMLSerializer::saveMap(map<string, SerializeData> &filterData, FileStorage &fs)
{
    if (filterData.find("ids") != filterData.end())
    {
        fs << "ids" << "[";
        auto ids = filterData["ids"].strVec;
        for (unsigned int i = 0; i < ids.size(); i++)
        {
            fs << "{";
            fs << "id" << ids[i];
            fs << "}";
        }
        fs << "]";
    }

    map<string, SerializeData> featureData = filterData;
    featureData.erase("ids");

    FeatureXMLSerializer::saveMap(featureData, fs);
}

}
