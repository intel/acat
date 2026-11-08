///////////////////////////////////////////////////////////////////////////
// <copyright file="lr_classifier_xml_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/lr_classifier_xml_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
LRClassifierXMLSerializer::LRClassifierXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

LRClassifierXMLSerializer::~LRClassifierXMLSerializer()
{
}

void LRClassifierXMLSerializer::saveLRParams(FileStorage &fs, const string name, const LRParameters &params)
{
    if (name.empty())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Name of FileNode is empty");
    }
    fs.startWriteStruct(name.c_str(), FileNode::MAP, "lr-parameters");

    fs << "lr_type" << params.solver_type;
    fs << "epsilon" << params.eps;
    fs << "C" << params.C;
    fs << "weight_count" << params.nr_weight;
    if (NULL != params.weight_label)
    {
        fs.startWriteStruct("weight_labels", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("i", params.weight_label, static_cast<size_t>(params.nr_weight) * sizeof(int));
        fs.endWriteStruct();
    }
    if (NULL != params.weight)
    {
        fs.startWriteStruct("weights", FileNode::SEQ + FileNode::FLOW);
        fs.writeRaw("d", params.weight, static_cast<size_t>(params.nr_weight) * sizeof(double));
        fs.endWriteStruct();
    }

    fs.endWriteStruct();
}

LRParameters LRClassifierXMLSerializer::loadLRParams(FileStorage &fs, FileNode fn)
{
    LRParameters params;

    if (!fn["lr_type"].empty())
    {
        int type = 0;
        fn["lr_type"] >> type;
        params.solver_type = (LRType) type;
    }
    if (!fn["epsilon"].empty())
    {
        fn["epsilon"] >> params.eps;
    }
    if (!fn["C"].empty())
    {
        fn["C"] >> params.C;
    }
    if (!fn["weight_count"].empty())
    {
        fn["weight_count"] >> params.nr_weight;
    }
    if (params.nr_weight != 0 && !fn["weight_labels"].empty())
    {
        params.weight_label = new int[params.nr_weight];
        fn["weight_labels"].readRaw("i", params.weight_label, static_cast<size_t>(params.nr_weight) * sizeof(int));
    }
    if (params.nr_weight != 0 && !fn["weights"].empty())
    {
        params.weight = new double[params.nr_weight];
        fn["weights"].readRaw("d", params.weight, static_cast<size_t>(params.nr_weight) * sizeof(double));
    }
    return params;
}

SPtr<Filter> LRClassifierXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<LRClassifier> classifier = filter;
    if (classifier == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to LRClassifier for serialization");
        }
        classifier = Factory::createLRClassifier();
        filter = classifier;
    }

    auto classifierMap = loadMap(fs, fn);
    LRParameters lrParams = classifierMap["parameters"].lrParams;
    classifier->setClassifierParams(lrParams);

    FilterXMLSerializer::load(fs, fn, filter);
    if (!fn["model"].empty())
    {
        SPtr<LRModel> lr = classifier->getModel();
        lr->read(fs, fn["model"]);
    }
    return classifier;
}

void LRClassifierXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<LRClassifier> classifier = filter; 
    if (classifier == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to SVMClassifier for serialization");
    }

    auto classifierMap = createMap(filter);
    saveMap(classifierMap, fs);
}

map<string, IXMLSerializer::SerializeData> LRClassifierXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    map<string, IXMLSerializer::SerializeData> lrMap = FilterXMLSerializer::loadMap(fs, fn); 

    LRParameters params;
    if (!fn["parameters"].empty())
    {
        params = loadLRParams(fs, fn["parameters"]);
    }
    lrMap["parameters"] = params;

    SPtr<LRModel> model = new LRModel();
    if (!fn["model"].empty())
    {
        model->read(fs, fn["model"]);
    }
    lrMap["model"] = model;

    return lrMap;
}
map<string, IXMLSerializer::SerializeData> LRClassifierXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto classifierMap = FilterXMLSerializer::createMap(filter);
    SPtr<LRClassifier> classifier = filter;
    if (classifier == NULL)
    {
        return classifierMap;    
    }

    classifierMap["parameters"] = classifier->getClassifierParams();
    SPtr<LRModel> model = classifier->getModel();
    if (model != NULL)
    {
        classifierMap["model"] = model;
    }
    return classifierMap;
}

}
