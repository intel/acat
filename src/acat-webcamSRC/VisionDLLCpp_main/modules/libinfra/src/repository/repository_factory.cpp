///////////////////////////////////////////////////////////////////////////
// <copyright file="repository_factory.cpp" company="Intel Corporation">
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

#include "libinfra/repository/repository_factory.hpp"
#include "libinfra/repository/xml_repository.hpp"
#include "libinfra/repository/svm_classifier_xml_serializer.hpp"
#include "libinfra/repository/lr_classifier_xml_serializer.hpp"
#include "libinfra/repository/classifier_result_data_source_serializer.hpp"
#include "libinfra/repository/gt_region_data_source_serializer.hpp"
#include "libinfra/repository/normal_flow_feature_serializer.hpp"
#include "libinfra/repository/normal_flow_blurred_feature_serializer.hpp"
#include "libinfra/repository/aggregator_feature_serializer.hpp"
#include "libinfra/repository/concatenator_feature_serializer.hpp"
#include "libinfra/repository/motion_estimation_stddev_feature_serializer.hpp"
#include "libinfra/repository/videofile_video_source_serializer.hpp"
#include "libinfra/repository/camera_video_source_serializer.hpp"
#include "libinfra/repository/realsense_video_source_serializer.hpp"
#include "libinfra/repository/rawfile_video_source_serializer.hpp"
#include "libinfra/repository/video_player_serializer.hpp"
#include "libinfra/repository/video_playlist_serializer.hpp"
#include "libinfra/repository/text_performance_serializer.hpp"
#include "libinfra/repository/gt_region_creator_serializer.hpp"
#include "libinfra/repository/bbox_region_creator_serializer.hpp"
#include "libinfra/repository/session_serializer.hpp"
#include "libinfra/repository/pipeline_serializer.hpp"
#include "libinfra/repository/parallel_pipeline_serializer.hpp"
#include "libinfra/repository/stlbp_feature_serializer.hpp"
#include "libinfra/repository/vlbp_feature_serializer.hpp"
#include "libinfra/repository/summator_feature_serializer.hpp"
#include "libinfra/repository/classifier_feature_serializer.hpp"
#include "libinfra/repository/dlib_region_creator_serializer.hpp"
#include "libinfra/repository/realsense_region_creator_serializer.hpp"
using namespace std;

namespace ivcp
{
SPtr<IRepository> RepositoryFactory::getRepository(string url)
{
    SPtr<XMLRepository> repository = new XMLRepository(url);

    repository->registerSerializer(
        repository->getFilterTypeId(CLASSIFIER_FILTER, ivcp::SVM),
        new SVMClassifierXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(CLASSIFIER_FILTER, ivcp::LOGISTIC_REGRESSION),
        new LRClassifierXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(DATA_SOURCE_OBSOLETE, ivcp::CLASSIFIER_RESULT_OBSOLETE),
        new ClassifierResultDataSourceXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(DATA_SOURCE_OBSOLETE, ivcp::GROUNDTRUTH_REGIONS_OBSOLETE),
        new GTRegionDataSourceXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(CLASSIFIER_RESULTS_FILTER, ivcp::XML_CLASSIFIER_RESULTS),
        new ClassifierResultDataSourceXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(GROUNDTRUTH_REGIONS_FILTER, ivcp::XML_GROUNDTRUTH_REGIONS),
        new GTRegionDataSourceXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::NORMALFLOW),
        new NormalFlowFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::NORMALFLOWBLURRED),
        new NormalFlowBlurredFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::AGGREGATOR),
        new AggregatorFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::CONCATENATOR),
        new ConcatenatorFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::SUMMATOR),
        new SummatorFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::MOTIONESTIMATIONSTDDEV),
        new MotionEstimationStdDevFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(VIDEO_SOURCE_FILTER, ivcp::VIDEOFILE),
        new VideofileVideoSourceSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(VIDEO_SOURCE_FILTER, ivcp::CAMERA),
        new CameraVideoSourceXMLSerializer(repository));

#ifdef USE_REALSENSE
    repository->registerSerializer(
        repository->getFilterTypeId(VIDEO_SOURCE_FILTER, ivcp::RS_CAMERA),
        new RealSenseVideoSourceXMLSerializer(repository));
#endif


    repository->registerSerializer(
        repository->getFilterTypeId(VIDEO_SOURCE_FILTER, ivcp::RAWFILE),
        new RawfileVideoSourceSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(VIDEO_PLAYER_FILTER, 0),
        new VideoPlayerXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(VIDEO_PLAYLIST_FILTER, 0),
        new VideoPlaylistXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(PERFORMANCE_FILTER, 0),
        new TextPerformanceXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(REGION_CREATOR_FILTER, ivcp::BBOX),
        new BBoxRegionCreatorXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(REGION_CREATOR_FILTER, ivcp::GROUNDTRUTH),
        new GTRegionCreatorXMLSerializer(repository));

#ifdef USE_DLIB
    repository->registerSerializer(
        repository->getFilterTypeId(REGION_CREATOR_FILTER, ivcp::DLIB_REGION),
        new DlibRegionCreatorXMLSerializer(repository));
#endif
#ifdef USE_REALSENSE
    repository->registerSerializer(
        repository->getFilterTypeId(REGION_CREATOR_FILTER, ivcp::RS_REGION),
        new RealSenseRegionCreatorXMLSerializer(repository));
#endif
    repository->registerSerializer(
        repository->getFilterTypeId(SESSION_FILTER, 0),
        new SessionXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(PIPELINE_FILTER, 0),
        new PipelineXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(PARALLEL_PIPELINE_FILTER, 0),
        new ParallelPipelineXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::STLBP),
        new STLBPFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::VLBP),
        new VLBPFeatureXMLSerializer(repository));

    repository->registerSerializer(
        repository->getFilterTypeId(FEATURE_FILTER, ivcp::CLASSIFIER_FEATURE),
        new ClassifierFeatureXMLSerializer(repository));

    return repository;
}
}

