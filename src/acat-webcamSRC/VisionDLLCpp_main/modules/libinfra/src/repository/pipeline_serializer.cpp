///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_serializer.cpp" company="Intel Corporation">
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

#include "libivcp/exceptions/io_exception.hpp"
#include "libinfra/repository/pipeline_serializer.hpp"
#include "libivcp/factory.hpp"
#include "libpipeline/pipeline.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
PipelineXMLSerializer::PipelineXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

PipelineXMLSerializer::~PipelineXMLSerializer()
{
}

SPtr<Filter> PipelineXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto filterMap = loadMap(fs, fn);

    vector<string> linksFrom = filterMap["links_from"].strVec;
    vector<string> linksTo = filterMap["links_to"].strVec;
    int mode = filterMap["mode"].ival;
    string sessionId = filterMap["session_id"].str;

    SPtr<IPipeline> pipeline = filter;
    if (pipeline == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to IPipeline for serialization");
        }
        pipeline = Factory::createPipeline((PipelineMode)mode);
        filter = pipeline;
    }

    if(pipeline->getMode() != mode)
    {
        IVCP_EXCEPTION(IOException, "Could not change pipeline mode");
    }

    FilterXMLSerializer::load(fs, fn, pipeline);

    pipeline->setSession(rep->loadSession(sessionId));
    //Add links
    if (linksFrom.size() == linksTo.size())
    {
        for (unsigned int i = 0; i < linksFrom.size(); i++)
        {
            pipeline->addLink(linksFrom[i], linksTo[i]);
        }
    }
    else
    {
        stringstream msg;
        msg << "Links sizes are incorrect: size of linksFrom is " << linksFrom.size() << ", size of linksTo is " << linksTo.size();
        IVCP_EXCEPTION(IncorrectParamException, msg.str());
    }

    return pipeline;
}

void PipelineXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<Pipeline> pipeline = filter;
    if  (pipeline == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to Pipeline for serialization");
    }

    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);

    rep->save(pipeline->getSession());
}

map<string, IXMLSerializer::SerializeData> PipelineXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    int mode;
    vector<string> linksFrom(fn["links"].size()), linksTo(fn["links"].size());
    string sessionId;

    if (fn["mode"].empty() || fn["session_id"].empty())
    {
        IVCP_EXCEPTION(IOException, "Unable to read \"mode\" and \"session_id\" tags. Data storage is possibly broken");
    }

    mode = fn["mode"];
    sessionId = (string)fn["session_id"];
    FileNodeIterator it;
    int counter = 0;
    for (it = fn["links"].begin(); it != fn["links"].end(); it++, counter++)
    {
        if (!(*it)["from"].empty() && !(*it)["to"].empty())
        {
            string from, to;
            (*it)["from"] >> from;
            (*it)["to"]  >> to;
            linksFrom[counter] = from;
            linksTo[counter] = to;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read links in PipelineXMLSerializer. Data is possibly broken");
        }
    }

    filterMap["mode"] = mode;
    filterMap["links_from"] = linksFrom;
    filterMap["links_to"] = linksTo;
    filterMap["session_id"] = sessionId;
    return filterMap;
}

void PipelineXMLSerializer::saveMap(map<string, SerializeData> &filterData, FileStorage &fs)
{
    int mode = filterData["mode"].ival;
    string sessionId = filterData["session_id"].str;
    vector<string> linksFrom = filterData["links_from"].strVec;
    vector<string> linksTo = filterData["links_to"].strVec;

    fs << "mode" << mode;
    if ((linksFrom.size() == linksTo.size()) && (!linksFrom.empty()))
    {
        fs << "links" << "[";
        for (unsigned int i = 0; i < linksFrom.size(); i++)
        {
            fs << "{";
            fs << "from" << linksFrom[i];
            fs << "to" << linksTo[i];
            fs << "}";
        }
        fs << "]";
    }
    fs << "session_id" << sessionId;

    map<string, SerializeData> filterDataCopy = filterData;
    filterDataCopy.erase("mode");
    filterDataCopy.erase("session_id");
    filterDataCopy.erase("links_from");
    filterDataCopy.erase("links_to");

    FilterXMLSerializer::saveMap(filterDataCopy, fs);
}

map<string, IXMLSerializer::SerializeData> PipelineXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);
    
    SPtr<IPipeline> pipeline = filter;
    if (pipeline == NULL)
    {
        return filterMap;
    }
    
    int numberOfLinks = pipeline->getNumberOfLinks();
    string sessionId = pipeline->getSession()->getId();
    vector<string> linksFrom(numberOfLinks);
    vector<string> linksTo(numberOfLinks);

    for (int l = 0; l < numberOfLinks; l++)
    {
        pair<string, string> link = pipeline->getLink(l);
        linksFrom[l] = link.first;
        linksTo[l] = link.second;
    }

    filterMap["mode"] = pipeline->getMode();
    filterMap["links_from"] = linksFrom;
    filterMap["links_to"] = linksTo;
    filterMap["session_id"] = sessionId;
    return filterMap;
}

}
