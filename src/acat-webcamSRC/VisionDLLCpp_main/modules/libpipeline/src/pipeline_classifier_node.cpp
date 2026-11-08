///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_classifier_node.cpp" company="Intel Corporation">
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


#include "libpipeline/pipeline_classifier_node.hpp"
#include <fstream>
#include <sstream>

using namespace std;
using namespace cv;

namespace ivcp
{
PipelineClassifierNode::PipelineClassifierNode(SPtr<IClassifier> _classifier, PipelineMode _mode):
    PipelineNode(_classifier), classifier(_classifier)
{
    resetReady();
    type = CLASSIFICATOR_NODE;
    mode = _mode;
    classMarkup = NULL;
}

PipelineClassifierNode::~PipelineClassifierNode()
{
    if (csv_output.is_open())
    {
        csv_output.close();
    }
}

void PipelineClassifierNode::addDestinationLink(SPtr<IPipelineNode> to)
{
    (void) to;
    IVCP_LOG_ERROR("PipelineClassifierNode::addDestinationLink method is not supposed to be called");
    return;
}

void PipelineClassifierNode::removeDestinationLink(SPtr<IPipelineNode> to)
{
    (void) to;
    IVCP_LOG_ERROR("PipelineClassifierNode::removeDestinationLink method is not supposed to be called");
    return;
}

void PipelineClassifierNode::addSourceLink(string from)
{
    if( sources.empty() || mode == TRAIN_PIPELINE)
    {
        PipelineNode::addSourceLink(from);
    }
    else
    {
         IVCP_LOG_INFO("Several features could be linked to classifier in training mode only");
         IVCP_EXCEPTION(IncorrectParamException, "Could not link more than one feature to classifier");
    }
}

void PipelineClassifierNode::removeSourceLink(string from)
{
    PipelineNode::removeSourceLink(from);
}

void PipelineClassifierNode::run()
{
    ClassType classifiedType = NOT_CLASSIFIED;
    vector<float> prob;
    resetReady();
    switch(mode)
    {
    case TRAIN_PIPELINE:
        classifiedType = classMarkup->getClassifyResult( activePipeline->getCurrentFrame() );
        break;
    case TEST_PIPELINE:
    case RUNTIME_PIPELINE:
        if (!isClassifierReady(feature))
        {
            break;
        }
        try
        {
            reportStart();
            classifiedType = classifier->clasify(feature);
            prob = classifier->getDistribution(feature);
            reportStop();
        }
        catch (Exception &e)
        {
            reportStop();
            IVCP_LOG_ERROR(e.getMessage());
            throw;
        }

        if ((TEST_PIPELINE == mode) && performanceCounter)
        {
            performanceCounter->addClassifyResult(activePipeline->getCurrentFrame(), classifiedType);
        }

        if(callbacks.size())
        {
            Mat res(1, 1 + (int)prob.size(), CV_32FC1);
            res.at<float>(0, 0) = (float)classifiedType;
            for (unsigned int i = 0; i < prob.size(); i++)
            {
                res.at<float>(0, i+1) = prob[i];
            }

            for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
            {
                (*callback->first)( res, callback->second);
            }
        }
        break;
    }
    if (csv_output.is_open())
    {
        csv_output << cv::format(feature, cv::Formatter::FMT_CSV) << csv_delimeter << ' ' << classifiedType << endl;
    }
}

void PipelineClassifierNode::finalize()
{
    switch( mode )
    {
    case TRAIN_PIPELINE:
        try
        {
            reportStart();
            classifier->trainModel();
            reportStop();
        }
        catch (Exception &e)
        {
            reportStop();
            IVCP_EXCEPTION(IncorrectParamException, e.getMessage());
        }
        break;
    case TEST_PIPELINE:
    case RUNTIME_PIPELINE:
        break;
    default:
        break;
    }
}

void PipelineClassifierNode::transmitMessage(Mat message, string caller)
{
    ClassType framemark;
    auto iter = find(sources.begin(), sources.end(), caller);
    if (iter != sources.end())
    {
        switch( mode )
        {
        case TRAIN_PIPELINE:
            if( classMarkup && activePipeline && classMarkup->isClassified( activePipeline->getCurrentFrame() ) )
            {
                framemark = classMarkup->getClassifyResult( activePipeline->getCurrentFrame() );
            }
            else
            {
                //If there is no data source provided it's impossibly to find out the right class mark
                //so just skip the feature;
                break;
            }
            try
            {
                classifier->addTrainFeature(message, framemark);
                feature = message;
                setReady();
            }
            catch (MismatchFeatureSizeException &e)
            {
                IVCP_LOG_ERROR(e.getMessage());
                throw;
            }

            break;
        case TEST_PIPELINE:
        case RUNTIME_PIPELINE:
            feature = message;
            setReady();
            break;
        default:
            break;
        }
    }
}

void PipelineClassifierNode::setMarkup(SPtr<IClassifierResultDataSource> markup)
{
    classMarkup = markup;
}

bool PipelineClassifierNode::isClassifierReady(Mat candidateFeature)
{
    bool isClassifierReady = true;
    if (!classifier->isReady())
    {
        classifier->clasify(candidateFeature);
        isClassifierReady = false;
    }
    return isClassifierReady;
}

void PipelineClassifierNode::setOutputFilename(string filename)
{
    if (filename.empty())
    {
        filename = getId() + ".csv";
    }

    csv_output.open(filename.c_str(), ios::out);
    if (!csv_output.is_open())
    {
        stringstream msg;
        msg << "Could not open file " << filename << " for tracking features";
        IVCP_EXCEPTION(IncorrectParamException, msg.str());
    }
}

};