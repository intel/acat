///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_classifier_xml_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/svm_classifier_xml_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
SVMClassifierXMLSerializer::SVMClassifierXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

SVMClassifierXMLSerializer::~SVMClassifierXMLSerializer()
{
}

void SVMClassifierXMLSerializer::saveSVMParams(FileStorage &fs, const string name, const SVMParameters &params)
{
    if (name.empty())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Name of FileNode is empty");
    }
    fs.startWriteStruct(name.c_str(), FileNode::MAP, "svm-parameters");

    fs << "svm_type" << params.svmType;
    fs << "kernel_type" << params.kernelType;
    fs << "degree" << params.degree;
    fs << "gamma" << params.gamma;
    fs << "coef0" << params.coef0;
    fs << "epsilon" << params.eps;
    fs << "C" << params.C;
    fs << "nu" << params.nu;
    fs << "p" << params.p;
    fs << "weight_count" << params.weightCount;
    if (!params.weightLabels.empty())
    {
        fs << "weight_labels" << params.weightLabels;
    }
    if (!params.weights.empty())
    {
        fs << "weights" << params.weights;
    }
    fs << "use_shrinking" << params.useShrinking;
    fs << "cache_size" << params.cacheSize;

    fs.endWriteStruct();
}

SVMParameters SVMClassifierXMLSerializer::loadSVMParams(FileStorage &fs, FileNode fn)
{
    SVMParameters params;
    (void)fs;

    if (!fn["svm_type"].empty())
    {
        int type = 0;
        fn["svm_type"] >> type;
        params.svmType = (SVMType) type;
    }
    if (!fn["kernel_type"].empty())
    {
        int kernel = 0;
        fn["kernel_type"] >> kernel;
        params.kernelType = (SVMKernelType) kernel;
    }
    if (!fn["degree"].empty())
    {
        fn["degree"] >> params.degree;
    }
    if (!fn["gamma"].empty())
    {
        fn["gamma"] >> params.gamma;
    }
    if (!fn["coef0"].empty())
    {
        fn["coef0"] >> params.coef0;
    }
    if (!fn["epsilon"].empty())
    {
        fn["epsilon"] >> params.eps;
    }
    if (!fn["C"].empty())
    {
        fn["C"] >> params.C;
    }
    if (!fn["nu"].empty())
    {
        fn["nu"] >> params.nu;
    }
    if (!fn["p"].empty())
    {
        fn["p"] >> params.p;
    }
    if (!fn["weight_count"].empty())
    {
        fn["weight_count"] >> params.weightCount;
    }
    if (!fn["weight_labels"].empty())
    {
        params.weightLabels.resize(params.weightCount);
        fn["weight_labels"] >> params.weightLabels;
    }
    if (!fn["weights"].empty())
    {
        params.weights.resize(params.weightCount);
        fn["weights"] >> params.weights;
    }
    if (!fn["use_shrinking"].empty())
    {
        fn["use_shrinking"] >> params.useShrinking;
    }
    if (!fn["cache_size"].empty())
    {
        fn["cache_size"] >> params.cacheSize;
    }
    return params;
}

SPtr<Filter> SVMClassifierXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<SVMClassifier> classifier = filter;
    if (classifier == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to SVMClassifier for serialization");
        }
        classifier = Factory::createSVMClassifier();
        filter = classifier;
    }

    auto classifierMap = loadMap(fs, fn);
    SVMParameters svmParams = classifierMap["parameters"].svmParams;
    classifier->setClassifierParams(svmParams);

    FilterXMLSerializer::load(fs, fn, filter);
    if (!fn["model"].empty())
    {
        SPtr<SVMModel> svm = classifier->getModel();
        svm->read(fs, fn["model"]);
    }

    return classifier;
}

void SVMClassifierXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<SVMClassifier> classifier = filter; 
    if (classifier == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to SVMClassifier for serialization");
    }

    auto classifierMap = createMap(filter);
    saveMap(classifierMap, fs);
}

map<string, IXMLSerializer::SerializeData> SVMClassifierXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    map<string, IXMLSerializer::SerializeData> svmMap = FilterXMLSerializer::loadMap(fs, fn); 

    SVMParameters params;
    if (!fn["parameters"].empty())
    {
        params = loadSVMParams(fs, fn["parameters"]);
    }
    svmMap["parameters"] = params;

    SPtr<SVMModel> model = new SVMModel();
    if (!fn["model"].empty())
    {
        model->read(fs, fn["model"]);
    }
    svmMap["model"] = model;

    return svmMap;
}
map<string, IXMLSerializer::SerializeData> SVMClassifierXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto classifierMap = FilterXMLSerializer::createMap(filter);
    SPtr<SVMClassifier> classifier = filter;
    if (classifier == NULL)
    {
        return classifierMap;    
    }

    classifierMap["parameters"] = classifier->getClassifierParams();
    SPtr<SVMModel> model = classifier->getModel();
    if (model != NULL)
    {
        classifierMap["model"] = model;
    }
    return classifierMap;
}

}