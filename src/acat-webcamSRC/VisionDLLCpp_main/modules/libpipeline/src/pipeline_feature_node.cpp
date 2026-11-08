///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_feature_node.cpp" company="Intel Corporation">
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


#include <algorithm>
#include "libpipeline/pipeline_feature_node.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
PipelineFeatureNode::PipelineFeatureNode(SPtr<IFeature> _feature, IPipeline::Settings settings):
    PipelineNode(_feature), feature(_feature)
{
    resetReady();
    type = FEATURE_NODE;
    needFrames = feature->getNumberOfRegions();
    needFrames = needFrames + (needFrames - 1) * (settings.distanceBetweenFrames - 1);
    currentFrame = VIDEO_PLAYLIST_FRAME_EOF;
    pipelineSettings = settings;
    needSkipFrames = 0;
}

void PipelineFeatureNode::addSourceLink(string from)
{
    if( sourceToRegionSequence.count(from) == 0 )
    {
        PipelineNode::addSourceLink(from);
        sourceToRegionSequence[from];
        framesBuffer[from];
    }
}

void PipelineFeatureNode::removeSourceLink(string from)
{
    auto iterSource = sourceToRegionSequence.find(from);
    if(iterSource != sourceToRegionSequence.end())
    {
        PipelineNode::removeSourceLink(from);
        sourceToRegionSequence.erase(iterSource);
    }

    auto iterFrames = framesBuffer.find(from);
    if(iterFrames != framesBuffer.end())
    {
        framesBuffer.erase(iterFrames);
    }
}

vector<VideoPlaylistFrame> PipelineFeatureNode::getLastProcessedFrames()
{
    return lastProcessedFrames;
}
        
void PipelineFeatureNode::run()
{
    resetReady();
    Mat evaluatedFeatureValue;
    try
    {
        reportStart();
        //Check whether feature is capable to handle multiple region sequence
        if (feature->getNumberOfSequences() == 1)
        {
            //Evaluate feature for each input region sequence and concatenate results
            evaluatedFeatureValue.create(1, (int)sources.size() * feature->getFeatureLength(), CV_32FC1);
            int partStart = 0;            
            for (list<string>::iterator iter = sources.begin(); iter != sources.end(); ++iter)
            {
                vector<RegionSequence> tmp;
                tmp.resize(1);
                tmp[0] = sourceToRegionSequence[*iter];
                bool regNotEmpty = true;
                for (auto regionIter = tmp[0].begin(); regionIter != tmp[0].end() && regNotEmpty; ++regionIter)
                {
                    if (regionIter->empty())
                    {
                        regNotEmpty = false;
                    }
                }
                if (regNotEmpty)
                {
                    feature->getFeature( tmp ).convertTo(evaluatedFeatureValue.colRange( partStart, partStart + feature->getFeatureLength() ), CV_32FC1);
                }
                else
                {
                    evaluatedFeatureValue.colRange( partStart, partStart + feature->getFeatureLength() ) = Mat::zeros(1, feature->getFeatureLength(), CV_32FC1);
                }
                partStart += feature->getFeatureLength();
            }
        }
        else
        {
            //Evaluate feature once for all input region sequences simultaneously if we have enough sequences
            if (feature->getNumberOfSequences() == sources.size())
            {
                vector<RegionSequence> tmp;
                tmp.resize(sources.size());
                bool regNotEmpty = true;
                int i = 0;
                for (list<string>::iterator iter = sources.begin(); iter != sources.end(); ++iter, ++i)
                {
                    tmp[i] = sourceToRegionSequence[*iter];
                    for (auto regionIter = tmp[i].begin(); regionIter != tmp[i].end() && regNotEmpty; ++regionIter)
                    {
                        if (regionIter->empty())
                        {
                            regNotEmpty = false;
                        }
                    }
                }

                //At the moment only frames with all necessary regions detected are used
                //ToDo Other frame storing strategies are possible
                if (regNotEmpty)
                {
                    evaluatedFeatureValue = feature->getFeature(tmp);
                }
                else
                {
                    evaluatedFeatureValue = Mat::zeros(1, feature->getFeatureLength(), CV_32FC1);
                }
            }
            else
            {
                IVCP_EXCEPTION(IncorrectParamException, "Number of sequences is incorrect" );
            }
        }
        reportStop();
    }
    catch (IncorrectParamException &e)
    {
        reportStop();
        IVCP_LOG_ERROR(e.getMessage());
        throw;
    }

    lastProcessedFrames.clear();
    for (unsigned int i = 0; i < feature->getNumberOfRegions(); i++)
    {
        lastProcessedFrames.push_back(frameNumbersForProcessing.front());
        frameNumbersForProcessing.pop_front();
    }

    for_each(destinations.begin(), destinations.end(), [&](SPtr<IPipelineNode> to)
    {
        to->transmitMessage(evaluatedFeatureValue, getId());
    });

    if (callbacks.size())
    {
        for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
        {
            (*callback->first)( evaluatedFeatureValue, callback->second);
        }
    }
}

void PipelineFeatureNode::transmitMessage(Mat message, string caller)
{
    auto iter = sourceToRegionSequence.find(caller);
    if (iter != sourceToRegionSequence.end())
    {
        if (currentFrame != activePipeline->getCurrentFrame())
        {
            currentFrame = activePipeline->getCurrentFrame();
            currentRegions.clear();
        }

        currentRegions[caller] = message;
        //At the moment only frames with all necessary regions detected are used
        //ToDo Other frame storing strategies are possible
        if (currentRegions.size() == sourceToRegionSequence.size())
        {
            if (needSkipFrames)
            {
                needSkipFrames--;
            }
            else 
            {
                frameNumbersBuffer.push_back(currentFrame);
                iter = framesBuffer.begin();
                size_t bufferSize = 0;
                if (iter != framesBuffer.end())
                {
                    bufferSize = iter->second.size();
                    for (; iter != framesBuffer.end(); ++iter)
                    {
                        iter->second.push_back(currentRegions[iter->first].clone());
                    }
                    bufferSize++;
                }
                
                if (bufferSize >= needFrames)
                {
                    for (iter = framesBuffer.begin(); iter != framesBuffer.end(); iter++)
                    {
                        sourceToRegionSequence[iter->first].clear();
                        unsigned int skip = 0;
                        auto bufIter = iter->second.begin();
                        auto frameNumbersIter = frameNumbersBuffer.begin();
                        for (int i = needFrames; i > 0; i--, bufIter++, frameNumbersIter++)
                        {
                            if (skip)
                            {
                                skip--;                                
                            }
                            else
                            {
                                sourceToRegionSequence[iter->first].push_back(*bufIter);
                                skip = pipelineSettings.distanceBetweenFrames - 1;
                                if (iter == framesBuffer.begin())
                                {
                                    frameNumbersForProcessing.push_back(*frameNumbersIter);
                                }
                            }
                        }
                    }
                    needFrames = 0;
                }
            }
        }

        if ( !needFrames ) 
        {
            needFrames = feature->getNumberOfRegions();
            needFrames = needFrames + (needFrames - 1) * (pipelineSettings.distanceBetweenFrames - 1);
            needSkipFrames = pipelineSettings.distanceBetweenStartsFrameSeq;

            iter = framesBuffer.begin();
            size_t bufferSize = frameNumbersBuffer.size();
            int deletedCount = 0;
            for (unsigned int i = 0; i < needSkipFrames && i < bufferSize; i++)
            {
                frameNumbersBuffer.pop_front();
                deletedCount++;
            }
            for (; iter != framesBuffer.end(); iter++)
            {
                for (unsigned int i = 0; i < needSkipFrames && i < bufferSize; i++)
                {
                    iter->second.pop_front();
                }
            }
            needSkipFrames -= deletedCount;

            setReady();
        }
    }
}
   
};