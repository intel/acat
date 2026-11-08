///////////////////////////////////////////////////////////////////////////
// <copyright file="xml_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/xml_serializer.hpp"
#include "libinfra/repository/svm_classifier_xml_serializer.hpp"
#include "libinfra/repository/lr_classifier_xml_serializer.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{

XMLSerializer::XMLSerializer(SPtr<IRepository> _rep) 
{
    if (_rep == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Repository in XMLSerializer constructor is NULL pointer");
    }

    rep = _rep;
}

XMLSerializer::~XMLSerializer()
{
}
  
void XMLSerializer::saveMap(map<string, SerializeData> &filterData, FileStorage &fs)
{
    for (auto it = filterData.begin(); it != filterData.end(); it++)
    {
        if (it->first == "type") continue;

        stringstream errorMsg;
        switch (it->second.type)
        {
        case IXMLSerializer::SerializeData::INT:
            fs << it->first << it->second.ival;
            break;
        case IXMLSerializer::SerializeData::FLOAT:
            fs << it->first << it->second.fval;
            break;
        case IXMLSerializer::SerializeData::DOUBLE:
            fs << it->first << it->second.dval;
            break;
        case IXMLSerializer::SerializeData::MAT:
            fs << it->first << it->second.mat;
            break;
        case IXMLSerializer::SerializeData::STRING:
            fs << it->first << it->second.str;
            break;
        case IXMLSerializer::SerializeData::ML_SVM:
            if (it->second.svm->getSupportVectorsCount() > 0)
            {
                it->second.svm->write(fs, "model");
            }
            break;
        case IXMLSerializer::SerializeData::ML_LR:
            if (it->second.lr->isTrained())
            {
                it->second.lr->write(fs, "model");
            }
            break;
        case IXMLSerializer::SerializeData::SVM_PARAMS:
            SVMClassifierXMLSerializer::saveSVMParams(fs, "parameters", it->second.svmParams);
            break;
        case IXMLSerializer::SerializeData::LR_PARAMS:
            LRClassifierXMLSerializer::saveLRParams(fs, "parameters", it->second.lrParams);
            break;
        case IXMLSerializer::SerializeData::STRING_VEC:
        default:
            errorMsg << "There is no standard serializer for type " << it->second.type;
            IVCP_LOG_ERROR(errorMsg.str());
            break;
        }
    }
}

void XMLSerializer::saveStrVec(const vector<string > &strVec, string groupName, string valueName, FileStorage &fs)
{
    if (strVec.size() > 0)
    {
        fs << groupName << "[";
        for (unsigned int i = 0; i < strVec.size(); i++)
        {
            fs << "{";
            fs << valueName << strVec[i];
            fs << "}";
        }
        fs << "]";
    }
}

void XMLSerializer::loadStrVec(vector<string> &strVec, string groupName, string valueName, FileStorage &fs, FileNode fn)
{
    (void) fs;

    strVec.resize(fn[groupName].size());
    FileNodeIterator it;
    int counter = 0;
    for (it = fn[groupName].begin(); it != fn[groupName].end(); it++, counter++)
    {
        string value;
        if (!(*it)[valueName].empty())
        {
            (*it)[valueName] >> value;
        }
        else
        {
            stringstream message;
            message << "Unable to read tag " << valueName << " in group " << groupName << " from the Repository";
            IVCP_EXCEPTION(IOException, message.str());
        }
        strVec[counter] = value;
    }
}

}
