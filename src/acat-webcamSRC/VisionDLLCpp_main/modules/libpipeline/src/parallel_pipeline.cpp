///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline.cpp" company="Intel Corporation">
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


#include <chrono> 
#include <numeric>
#include <iostream>
#include <iomanip>
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/region_creator/gt_region_creator.hpp"
#include "libpipeline/parallel_pipeline.hpp"
#include "libpipeline/parallel_pipeline_source_node.hpp"
#include "libpipeline/parallel_pipeline_region_creator_node.hpp"
#include "libpipeline/parallel_pipeline_feature_node.hpp"
#include "libpipeline/parallel_pipeline_classifier_node.hpp"

using namespace cv;
using namespace std;
using namespace std::chrono;

#define PARALLEL_PIPELINE_TEXT_NAME "parallel pipeline"
#define DEFAULT_PERFORMANCE_RATE 30.0f
#define SLOW_PIPELINE_WARN_COEFF 1.15f

namespace ivcp
{
ParallelPipeline::ParallelPipeline(PipelineMode mode, Settings settings): Pipeline(mode, settings) 
{
    if (mode == TRAIN_PIPELINE)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Parallel pipeline settings are incorrect. Could not create parallel pipeline in train mode");
    }
    setPerformanceRate(DEFAULT_PERFORMANCE_RATE);
}

ParallelPipeline::ParallelPipeline(SPtr<Session> session, PipelineMode mode, Settings settings): Pipeline(session, mode, settings)
{
    if (mode == TRAIN_PIPELINE)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Parallel pipeline settings are incorrect. Could not create parallel pipeline in train mode");
    }
    setPerformanceRate(DEFAULT_PERFORMANCE_RATE);
}

void ParallelPipeline::run()
{
    if (getPerformanceRate() == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Parallel pipeline must have performance rate greater than 0");
    }

    pipelineDelayCount = 0;

    createNodes();

    for (auto iter = nodes.begin(); iter != nodes.end(); ++iter)
    {
        (*iter)->init();
        std::map<string, IPipeline::ProcessingPriority>::iterator priorIt;
        priorIt = priorities.find((*iter)->getId());
        if(priorIt != priorities.end())
            (*iter)->setPriority(priorIt->second);
    }

    duration<int, milli> cyclePeriod((int)(1000.0f / performanceRate));
    try
    {
        bool Acted;
        do
        {
            high_resolution_clock::time_point start = high_resolution_clock::now();
            Acted = false;
            for (auto iter = nodes.begin(); iter != nodes.end(); ++iter)
            {
                if((*iter)->isReady())
                {
                    Acted = true;
                    high_resolution_clock::time_point nodeStart = high_resolution_clock::now();
                    (*iter)->run();
                    high_resolution_clock::time_point nodeEnd = high_resolution_clock::now();

                    //Save time pipeline waits for the filter result
                    //In case this is video player replace name with "parallel pipeline" as they work at the same FPS
                    string id = ((*iter)->getType() == SOURCE_NODE)?(PARALLEL_PIPELINE_TEXT_NAME):((*iter)->getId());
                    filtersWaitTime[id] = (int)std::chrono::duration_cast<std::chrono::milliseconds>(nodeEnd - nodeStart).count();
                }
            }
            high_resolution_clock::time_point end = high_resolution_clock::now();

            if (currentMode == TEST_PIPELINE || currentMode == RUNTIME_PIPELINE )
            {
                if ((end - start) >= cyclePeriod * SLOW_PIPELINE_WARN_COEFF)
                {
                    pipelineDelayCount++;
                    updateRecomendations();
                }
                else if ((end - start) < cyclePeriod)
                {
                    this_thread::sleep_for(cyclePeriod - (end - start));                    
                }
            }
        }
        while(Acted);
    }
    catch (Exception& e)
    {
        finalize();
        clearNodes();
        IVCP_LOG_ERROR(e.getMessage());
        throw;
    }

    finalize();
    clearNodes();
}

void ParallelPipeline::updateRecomendations()
{
    /*
    - we shall not output warnings in case filter is not ready in time
    - we shall output recommended fps only if the parallel pipeline is not able to run at 90% of given fps
    - pipeline shall not output warnings more often than once in 10 seconds and only if it has at least 10 times being late
    - pipeline shall aggregate recommendations from difference cycles and output averaged recommendations (see details below)

    Algorithm for making recommendations on per iteration basis:
    - for each filter save time pipeline had to wait
    - sort the wait time from the largest to the lowest
    - decrease fps of the filters until total wait time is less than necessary for the pipeline to work at given fps
    - decrease fps of the filter which has top wait time
    - resort the filters by the updated wait time

    Algorithm of decreasing fps:
    - decrease fps of the filter so that value "(fps of pipeline)/(fps of filter)" is increased by 1

    Algorithm of aggregating recommendations for a set of iterations:
    - For each filter we need to calculate the following statistics: percent of times when fps of the filter is recommended for decreasing among all times when pipeline was not able to be in time
    */

    int cyclePeriod = (int)(1000.0f / performanceRate);
    int totalWaitTime = std::accumulate(filtersWaitTime.begin(), filtersWaitTime.end(), pair<string, int> ("", 0), 
        [](const pair<string, int> &a, const pair<string, int> &b) -> pair<string, int> {return pair<string, int>("", a.second + b.second);}).second;
    map<string, float> advices;

    while (totalWaitTime > cyclePeriod)
    {
        auto maxWaitTime = std::max_element (filtersWaitTime.begin(), filtersWaitTime.end(), 
            [](const pair<string, int> &a, const pair<string, int> &b) -> bool {return a.second < b.second;});

        int ratio = (int) (performanceRate / ((advices.count(maxWaitTime->first))?(advices[maxWaitTime->first]):(filtersFPS[maxWaitTime->first])));
        float newFPS = performanceRate / (ratio + 1);
        maxWaitTime->second = std::max<int>  (0, maxWaitTime->second - cyclePeriod);
        advices[maxWaitTime->first] = newFPS;

        totalWaitTime = std::accumulate(filtersWaitTime.begin(), filtersWaitTime.end(), pair<string, int> ("", 0), 
            [](const pair<string, int> &a, const pair<string, int> &b) -> pair<string, int> {return pair<string, int>("", a.second + b.second);}).second;
    }

    std::for_each(advices.begin(), advices.end(), 
        [this](const pair<string, float> &a){fpsAdvices[a.first][a.second] += 1; });

    filtersWaitTime.clear();

    high_resolution_clock::time_point nowTime = high_resolution_clock::now();
    if (nowTime - lastFPSWarning > seconds(10) && pipelineDelayCount > 10)
    {
        stringstream ss;
        ss << "Parallel pipeline works slower than expected\n";

        advices.clear();
        for (auto it = fpsAdvices.begin(); it != fpsAdvices.end(); it++)
        {
            auto maxVotesAdvice = std::max_element(it->second.begin(), it->second.end(), 
                [](const pair<float, int> &a, const pair<float, int> &b) -> bool {return a.second < b.second;});

            advices[it->first] = maxVotesAdvice->first;
        }

        if (!advices.empty())
        {
            ss << "We recommended to decrease FPS values of the following filters:\n";
            for_each(advices.begin(), advices.end(), 
                [&ss](const pair<string, float> &a)
                {
                    ss << std::setw(20) << a.first << ":\t" << std::fixed << std::setprecision(2) << a.second << " FPS \n";
                });
        }

        IVCP_LOG_WARNING(ss.str());

        pipelineDelayCount = 0;
        lastFPSWarning = nowTime;
    }
}

void ParallelPipeline::finalize()
{
    for (auto iter = nodes.begin(); iter != nodes.end(); ++iter)
    {
        (*iter)->finalize();
    }
}

SPtr<IPipelineNode> ParallelPipeline::addNode(SPtr<IFilter> filter)
{
    string newFilterId = filter->getId();
    SPtr<IPipelineNode> pipelineNode = getNode(newFilterId);
    if(pipelineNode != NULL)
    {
        return pipelineNode;
    }

    float filterPerformanceRate;
    if (filter->getPerformanceRate() == 0)
    {
        filterPerformanceRate = performanceRate;
    }
    else
    {
        filterPerformanceRate = filter->getPerformanceRate();
    }

    if (filterPerformanceRate > performanceRate)
    {
        stringstream ss; 
        ss << "Performance rate of \"" << filter->getId() << "\" filter greater than pipeline performance rate";
        IVCP_EXCEPTION(IncorrectParamException, ss.str());
    }

    filtersFPS[filter->getId()] = filterPerformanceRate; 
    filtersFPS[PARALLEL_PIPELINE_TEXT_NAME] = performanceRate;

    switch(filter->getRootType())
    {
    case VIDEO_PLAYER_FILTER:
        if (playlist)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Source is already defined");
        }
        if (filterPerformanceRate != performanceRate)
        {
            IVCP_LOG_WARNING("Video player performance rate is not equal to the pipeline performance rate. The video player will use the parallel pipeline performance rate instead");
        }
        filtersFPS[filter->getId()] = performanceRate;
        pipelineNode = new ParallelPipelineSourceNode( SPtr<IVideoPlayer>( filter ), 1 );
        playlist = SPtr<IVideoPlayer>( filter )->getPlaylist();
        break;
    case REGION_CREATOR_FILTER:
        pipelineNode = new ParallelPipelineRegionCreatorNode( SPtr<IRegionCreator>(filter), (size_t)(performanceRate/filterPerformanceRate) );
        break;
    case FEATURE_FILTER:
        pipelineNode = new ParallelPipelineFeatureNode( SPtr<IFeature>(filter), (size_t)(performanceRate/filterPerformanceRate), pipelineSettings );
        break;
    case CLASSIFIER_FILTER:
        pipelineNode = new ParallelPipelineClassifierNode( SPtr<IClassifier>(filter), this->currentMode, (size_t)(performanceRate/filterPerformanceRate) );
        {
            auto outputFile = classifierToOutputFile.find(filter->getId());
            if (outputFile != classifierToOutputFile.end())
            {
                SPtr<PipelineClassifierNode>(pipelineNode)->setOutputFilename(outputFile->second);
            }
        }
        break;
    case CLASSIFIER_RESULTS_FILTER:
    case GROUNDTRUTH_REGIONS_FILTER:
    case PERFORMANCE_FILTER:
    case VIDEO_SOURCE_FILTER:
    case SESSION_FILTER:
        IVCP_EXCEPTION(IncorrectParamException, "Could not create node for such type of filter");
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Undefined filter type");
    }

    pipelineNode->setPipeline(this);

    std::map<NodeType, int> filterPos; 
    filterPos[SOURCE_NODE] = 0;
    filterPos[REGION_NODE] = 1;
    filterPos[FEATURE_NODE] = 2;
    filterPos[CLASSIFICATOR_NODE] = 3;

    //Find insert position (for Video Player this is a position of the first Region Creator, for a Region Creator this is a position of a Feature Node etc)
    auto insertPos = find_if(nodes.begin(), nodes.end(), [&filterPos, &pipelineNode](SPtr<IPipelineNode> node) -> bool 
    {
        if (filterPos[node->getType()] > filterPos[pipelineNode->getType()])
        {
            return true;
        }
        return false;
    });

    nodes.insert(insertPos, pipelineNode);
    if(nodeCallbacks.count(newFilterId))
    {
        for (auto callback = nodeCallbacks[newFilterId].begin(); callback != nodeCallbacks[newFilterId].end(); callback++)
        {
            pipelineNode->addCallback(callback->first, callback->second);
        }
    }
    return pipelineNode;
}

FilterType ParallelPipeline::getRootType() const
{
    return PARALLEL_PIPELINE_FILTER;
}

void ParallelPipeline::setFilterPriority(const std::string &filterName, ProcessingPriority priority)
{
    priorities[filterName] = priority;
}

};

