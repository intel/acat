///////////////////////////////////////////////////////////////////////////
// <copyright file="factory.hpp" company="Intel Corporation">
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


#ifndef __IVCP_FACTORY_HPP__
#define __IVCP_FACTORY_HPP__

#include <string>
#include "libivcp/irepository.hpp"
#include "libivcp/irepository_factory.hpp"
#include "libivcp/ipipeline_factory.hpp"
#include "libivcp/log.hpp"
#include "libivcp/session.hpp"
#include "libivcp/idata_source_factory.hpp"
#include "libivcp/idlib_factory.hpp"
#include "libivcp/irealsense_factory.hpp"
#include "libivcp/classifier/svm_classifier.hpp"
#include "libivcp/classifier/lr_classifier.hpp"
#include "libivcp/data_source/iclassifier_result_data_source.hpp"
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/feature/aggregator_feature.hpp"
#include "libivcp/feature/classifier_feature.hpp"
#include "libivcp/feature/concatenator_feature.hpp"
#include "libivcp/feature/normal_flow_blurred_feature.hpp"
#include "libivcp/feature/normal_flow_feature.hpp"
#include "libivcp/feature/motion_estimation_stddev_feature.hpp"
#include "libivcp/feature/stlbp_feature.hpp"
#include "libivcp/feature/summator_feature.hpp"
#include "libivcp/feature/vlbp_feature.hpp"
#include "libivcp/region_creator/bbox_region_creator.hpp"
#include "libivcp/region_creator/gt_region_creator.hpp"
#include "libivcp/region_creator/dlib_region_creator.hpp"
#include "libivcp/region_creator/realsense_region_creator.hpp"
#include "libivcp/video_source/videofile_video_source.hpp"
#include "libivcp/video_source/rawfile_video_source.hpp"
#include "libivcp/video_source/camera_video_source.hpp"
#include "libivcp/video_source/realsense_video_source.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file factory.hpp
 * \brief Factory class provides static methods for creating IVCP filters
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Factory provides static methods for creating IVCP filters
 */
class IVCP_EXPORT Factory
{
public:
    /*!
     * \brief Create Session
     * \param[in] id Id that will be assigned to a new Session. If id is empty default ID "session" will be assigned
     * \return Session object
     * \throw IncorrectParamException Unable to create a Session 
     */
    static SPtr<Session> createSession(std::string id = "");

    /*!
     * \brief Create Pipeline
     * \param[in] mode Pipeline execution mode
     * \param[in] settings Pipeline settings
     * \param[in] id Id that will be assigned to a new Pipeline. If id is empty default ID "pipeline" will be assigned     
     * \return Pipeline object
     * \throw IncorrectParamException Unable to create a Pipeline 
     */
    static SPtr<IPipeline> createPipeline(PipelineMode mode, IPipeline::Settings settings = IPipeline::Settings(), std::string id = "");

    /*!
     * \brief Create ParallelPipeline
     * \param[in] mode Pipeline execution mode (Note: you can't create parallel pipeline in train mode).
     * \param[in] settings Pipeline settings
     * \param[in] id Id that will be assigned to a new Pipeline. If id is empty default ID "parallel_pipeline" will be assigned     
     * \return ParallelPipeline object
     * \throw IncorrectParamException Unable to create a Pipeline 
     */
    static SPtr<IPipeline> createParallelPipeline(PipelineMode mode, IPipeline::Settings settings = IPipeline::Settings(), std::string id = "");

    /*!
     * \brief Create GUIFramework
     * \param[in] pipeline Pipeline that will be visualised
     * \param[in] framerate Quantity of pipeline processed frames to visualize per second.(Please note that real framerate could be lower in case the pipeline couldn't process frames fast enough)
     * \return GUIFramework object
     * \throw IncorrectParamException Unable to create a GUIFramework
     */
    static SPtr<IGUIFramework> createGUIFramework(SPtr<IPipeline> pipeline, float framerate = 30.303f);

    /*!
     * \brief Create Video Player
     * \param[in] id Id that will be assigned to a new Video Player. If id is empty default ID "player" will be assigned
     * \return Video Player object
     * \throw IncorrectParamException Unable to create a Video Player 
     */
    static SPtr<IVideoPlayer> createVideoPlayer(std::string id = "");

    /*!
     * \brief Create Video Playlist
     * \param[in] id Id that will be assigned to a new Video Playlist. If id is empty default ID "playlist" will be assigned
     * \return Video Playlist object
     * \throw IncorrectParamException Unable to create a Video Playlist
     */
    static SPtr<VideoPlaylist> createVideoPlaylist(std::string id = "");

    /*!
     * \brief Create CameraVideoSource
     * \param[in] cameraIndex Index of a camera to use
     * \param[in] id Id that will be assigned to a new CameraVideoSource. If id is empty default ID "camera" will be assigned 
     * \return CameraVideoSource object
     * \throw IncorrectParamException Unable to create a CameraVideoSource
     */
    static SPtr<CameraVideoSource> createCameraVideoSource(int cameraIndex = cv::CAP_ANY, std::string id = "");

#ifdef USE_REALSENSE
    /*!
     * \brief Create RealSenseVideoSource
     * \param[in] cameraIndex Index of a RealSense camera to use
     * \param[in] id Id that will be assigned to a new RealSenseVideoSource. If id is empty default ID "rs_camera" will be assigned 
     * \return RealSenseVideoSource object
     * \throw IncorrectParamException Unable to create a RealSenseVideoSource
     */
    static SPtr<RealSenseVideoSource> createRealSenseVideoSource(int cameraIndex = 0, std::string id = "");
#endif



    /*!
     * \brief Create VideofileVideoSource
     * \param[in] path Path to a video file
     * \param[in] id Id that will be assigned to a new VideofileVideoSource. If id is empty default ID "videofile" will be assigned
     * \param[in] multipleVideoFiles If this flag is true then several files will be used
     * \param[in] is12Bit whether or not we convert 24 bit RGB values to 16 bit b/w
     * (appending basic name with numbers 1, 2, etc)
     * \return VideofileVideoSource object
     * \throw IncorrectParamException Unable to create a VideofileVideoSource
     */
    static SPtr<VideofileVideoSource> createVideofileVideoSource(std::string path, std::string id = "videofile", bool multipleVideoFiles = false, bool is12Bit = false);

    /*!
     * \brief Create RawfileVideoSource
     * \param[in] path Path to a video file
     * \param[in] id Id that will be assigned to a new RawfileVideoSource. If id is empty default ID "rawfile" will be assigned
     * \return RawfileVideoSource object
     * \throw IncorrectParamException Unable to create a RawfileVideoSource
     */
    static SPtr<RawfileVideoSource> createRawfileVideoSource(std::string path, std::string id = "rawfile");

    /*!
     * \brief Create VideofileVideoSource or CameraVideoSource with no real device (camera) or file (video file) attached. The filter can be attached to a file or a camera later
     * \param[in] type Type of a Video Source to create
     * \param[in] id Id that will be assigned to a new VideofileVideoSource. If id is empty default ID ("videofile" or "camera") will be assigned 
     * \return VideofileVideoSource or CameraVideoSource object
     * \throw IncorrectParamException Unable to create a Video Source
     */
    static SPtr<IVideoSource> createVideoSource(VideoSourceType type = VIDEOFILE, std::string id = "");

	/*!
     * \brief Get IDlib object previously used to construct DlibRegionCreator objects
     * \details In case there is no previously used IDlib object it would be created using provided parameters.
     * \param[in] stabThreshold Movement threshold to start change regions on face movement
     * \param[in] stabFrames Frame number to check for face movement during face stabilization
     * \param[in] normFrames Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames).
     * In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed
     * \param[in] noFaceFrames Frequency of applying the "no face" Dlib workaround
     * \param[in] noFaceClockwiseRotation If true in the "no face" workaround we rotate the image clockwise, otherwise we rotate counter-clockwise
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true Dlib improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \param[in] resizeDownscale Downscale for every frame image to decrease face detection time
     * \throw IOException Dlib internal error
     */
    static SPtr<IDlib> getCachedDlib(float stabThreshold = 0.0f,
        unsigned int stabFrames = 10,
        int normFrames = -1,
        int noFaceFrames = -1,
        bool noFaceClockwiseRotation = false,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false,
        double resizeDownscale = 1.0);

    /*!
     * \brief Check if Dlib handler is created
     */
    static bool isDlibHandlerCreated();

    /*!
     * \brief Get IRealSense object previously used to construct RealSenseRegionCreator objects
     * \details In case there is no previously used IRealSense object it would be created using provided parameters.
     * \param[in] stabThreshold Movement threshold to start change regions on face movement
     * \param[in] stabFrames Frame number to check for face movement during face stabilization
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true RealSense improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \throw IOException RealSense internal error
     */
    static SPtr<IRealSense> getCachedRealSense(float stabThreshold = 0.0f,
        unsigned int stabFrames = 10,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false);

    /*!
     * \brief Check if RealSense handler is created
     */
    static bool isRealSenseHandlerCreated();
    /*!
     * \brief Create GTRegionCreator
     * \param[in] source Data marking for this region
     * \param[in] regClass Index of this Region (ie IRegionDataSource can contain markings for multiple regions)
     * \param[in] id Id that will be assigned to a new GTRegionCreator. If id is empty default ID "gt" will be assigned 
     * \return GTRegionCreator
     * \throw IncorrectParamException Unable to create a GTRegionCreator
     */
    static SPtr<GTRegionCreator> createGTRegionCreator(SPtr<IRegionDataSource> source, ClassType regClass, std::string id = "");

    /*!
     * \brief Create GTRegionCreator
     * \param[in] dataSourceFilepath Filepath to the marking for this region
     * \param[in] regClass Index of this Region (ie IRegionDataSource can contain markings for multiple regions)
     * \param[in] id Id that will be assigned to a new GTRegionCreator. If id is empty default ID "gt" will be assigned 
     * \return GTRegionCreator
     * \throw IncorrectParamException Unable to create a GTRegionCreator
     */
    static SPtr<GTRegionCreator> createGTRegionCreator(std::string dataSourceFilepath, ClassType regClass, std::string id = "");

    /*!
     * \brief Create BBoxRegionCreator
     * \param[in] bboxRect Rectangle that will be used as a region box on all frames
     * \param[in] id Id that will be assigned to a new BBoxRegionCreator. If id is empty default ID "bbox" will be assigned 
     * \return BBoxRegionCreator
     * \throw IncorrectParamException Unable to create a BBoxRegionCreator
     */
    static SPtr<BBoxRegionCreator> createBBoxRegionCreator(const cv::Rect &bboxRect, std::string id = "");
    /*!
     * \brief Create DlibRegionCreator
     * \param[in] settings Dlib Region Creator specific settings
     * \param[in] id Id that will be assigned to a new DlibRegionCreator. If id is empty default ID "dlib" will be assigned 
     * \param[in] dlibHandler Pointer to the interface implementing low-level integration with Dlib (this will be only necessary when we wil track for multiple faces, leave the NULL for now)
     * \return DlibRegionCreator
     * \throw Dlib handler is invalid
     */
    static SPtr<DlibRegionCreator> createDlibRegionCreator(DlibRegionCreator::Settings settings = DlibRegionCreator::Settings(), std::string id = "", SPtr<IDlib> dlibHandler = NULL);

     /*!
     * \brief Create RealSenseRegionCreator
     * \param[in] settings RealSense Region Creator specific settings
     * \param[in] id Id that will be assigned to a new RealSenseRegionCreator. If id is empty default ID "realsense" will be assigned 
     * \param[in] rsHandler Pointer to the interface implementing low-level integration with RealSense (this will be only necessary when we wil track for multiple faces, leave the NULL for now)
     * \return RealSenseRegionCreator
     * \throw RealSense handler is invalid
     */
    static SPtr<RealSenseRegionCreator> createRealSenseRegionCreator(RealSenseRegionCreator::Settings settings = RealSenseRegionCreator::Settings(), std::string id = "", SPtr<IRealSense> rsHandler = NULL);

    /*!
     * \brief Create BBoxRegionCreator, GTRegionCreator, DlibRegionCreator or RealSenseRegionCreator
     * \param[in] type Type of RegionCreator
     * \param[in] id Id that will be assigned to a new RegionCreator. If id is empty default ID ("bbox", "gt",  "dlib", "realsense") will be assigned 
     * \return Region Creator
     * \throw IncorrectParamException 1) Unknown region creator type 2) handler is not found
     */
    static SPtr<IRegionCreator> createRegionCreator( RegionCreatorType type = BBOX, std::string id = "");

    /*!
     * \brief Create AggregatorFeature
     * \param[in] numberOfRegions Number of regions used by all Features aggregated by the AggregatorFeature
     * \param[in] numberOfSequences Number of region sequences
     * \param[in] id Id that will be assigned to a new AggregatorFeature. If id is empty default ID "aggregator" will be assigned 
     * \return AggregatorFeature
     * \throw IncorrectParamException Unable to create a AggregatorFeature
     */
    static SPtr<AggregatorFeature> createAggregatorFeature(int numberOfRegions, int numberOfSequences = 1, std::string id = "");

    /*!
     * \brief Create ConcatenatorFeature
     * \param[in] numberOfRegions Number of regions used by the underlaying feature passed to ConcatenatorFeature
     * \param[in] id Id that will be assigned to a new ConcatenatorFeature. If id is empty default ID "concatenator" will be assigned 
     * \return ConcatenatorFeature
     * \throw IncorrectParamException Unable to create a ConcatenatorFeature
     */
    static SPtr<ConcatenatorFeature> createConcatenatorFeature(int numberOfRegions, std::string id = "");

    /*!
     * \brief Create SummatorFeature
     * \param[in] settings Summator feature configuration
     * \param[in] id Id that will be assigned to a new ConcatenatorFeature. If id is empty default ID "concatenator" will be assigned 
     * \return SummatorFeature
     * \throw IncorrectParamException Unable to create a SummatorFeature
     */
    static SPtr<SummatorFeature> createSummatorFeature(SummatorFeature::Settings settings = SummatorFeature::Settings(), std::string id = "");

    /*!
     * \brief Create STLBPFeature
     * \param[in] settings STLBP feature settings 
     * \param[in] id Id that will be assigned to a new STLBPFeature. If id is empty default ID "stlbp" will be assigned 
     * \return STLBPFeature
     * \throw IncorrectParamException Unable to create a STLBPFeature
     */
    static SPtr<STLBPFeature> createSTLBPFeature(STLBPFeature::Settings settings = STLBPFeature::Settings(), std::string id = "");

    /*!
     * \brief Create VLBPFeature
     * \param[in] settings VLBP feature settings 
     * \param[in] id Id that will be assigned to a new VLBPFeature. If id is empty default ID "vlbp" will be assigned 
     * \return VLBPFeature
     * \throw IncorrectParamException Unable to create a VLBPFeature
     */
    static SPtr<VLBPFeature> createVLBPFeature(VLBPFeature::Settings settings = VLBPFeature::Settings(), std::string id = "");

    /*!
     * \brief Create ClassifierFeature
     * \param[in] numberOfSeq Number of region sequences
     * \param[in] type Type of classifier output (probabilistic or categorical)
     * \param[in] id Id that will be assigned to a new ClassifierFeature. If id is empty default ID "classifier_feature" will be assigned 
     * \return ClassifierFeature object
     * \throw IncorrectParamException Unable to create a ClassifierFeature
     */
    static SPtr<ClassifierFeature> createClassifierFeature(int numberOfSeq = 1, ClassifierOutputType type = PROBABILISTIC, std::string id = "");

    /*!
     * \brief Create NormalFlowBlurredFeature
     * \param[in] threshold Threshold value
     * \param[in] outputType NormalFlowBlurredFeature ouput type
     * \param[in] robustFlag If true, solve system using robust estimator
     * \param[in] id Id that will be assigned to a new NormalFlowBlurredFeature. If id is empty default ID "nfb" will be assigned
     * \return NormalFlowBlurredFeature object
     */
    static SPtr<NormalFlowBlurredFeature> createNormalFlowBlurredFeature(float threshold = 0.f, NFBFType outputType = NFBF_ENERGY, bool robustFlag = false, std::string id = "");

    /*!
     * \brief Create NormalFlowFeature
     * \param[in] threshold Threshold value
     * \param[in] id Id that will be assigned to a new NormalFlowFeature. If id is empty default ID "nf" will be assigned
     * \return NormalFlowFeature object
     */
    static SPtr<NormalFlowFeature> createNormalFlowFeature(float threshold = 0.f, std::string id = "");

    /*!
     * \brief Create MotionEstimationStdDevFeature
     * \param[in] numberOfSeq Number of region sequences
     * \param[in] id Id that will be assigned to a new MotionEstimationStdDevFeature. If id is empty default ID "mestddev" will be assigned
     * \return MotionEstimationStdDevFeature object
     */
    static SPtr<MotionEstimationStdDevFeature> createMotionEstimationStdDevFeature(int numberOfSeq = 1, std::string id = "");

    /*!
     * \brief Create Feature
     * \param[in] type Type of Feature to create
     * \param[in] id Id that will be assigned to a new ConcatenatorFeature. If id is empty default ID (depending on type) will be assigned 
     * \return Feature (AggregatorFeature, ConcatenatorFeature, STLBPFeature, VLBPFeature, NormalFlowFeature, NormalFlowBlurredFeature)
     * \throw IncorrectParamException Unable to create a Feature
     */
    static SPtr<IFeature> createFeature(FeatureType type = NORMALFLOW, std::string id = "");

    /*!
     * \brief Create SVM Classifier with given settings 
     * \param[in] classifierParams Parameters of SVM classifier 
     * \param[in] id Id that will be assigned to a new SVMClassifier. If id is empty default ID "svm" will be assigned 
     * \return SVMClassifier
     * \throw IncorrectParamException Unable to create a SVMClassifier
     */
    static SPtr<SVMClassifier> createSVMClassifier(SVMParameters classifierParams = SVMParameters(), std::string id = "");

    /*!
     * \brief Create LR Classifier with given settings 
     * \param[in] classifierParams Parameters of LR classifier 
     * \param[in] id Id that will be assigned to a new LRClassifier. If id is empty default ID "svm" will be assigned 
     * \return LRClassifier
     * \throw IncorrectParamException Unable to create a LRClassifier
     */
    static SPtr<LRClassifier> createLRClassifier(LRParameters classifierParams = LRParameters(), std::string id = "");

    /*!
     * \brief Create Classifier
     * \param[in] type Type of Classifier to create
     * \param[in] id Id that will be assigned to a new ConcatenatorFeature. If id is empty default ID (depending on type) will be assigned 
     * \return Classifier (SVMClassifier or LRClassifier)
     * \throw IncorrectParamException Unable to create a Classifier
     */
    static SPtr<IClassifier> createClassifier(ClassifierType type = SVM, std::string id = "");

    /*!
     * \brief Create Performance filter
     * \param[in] type Type of Performance to create
     * \param[in] id Id that will be assigned to a new Performance. If id is empty default ID (depending on type) will be assigned 
     * \return TextPerformance
     * \throw IncorrectParamException Unable to create a Classifier
     */
    static SPtr<IPerformance> createPerformance(PerformanceType type = TEXT_REPORT, std::string id = "");

    /*!
     * \brief Create classifier result data source
     * \param[in] id Id that will be assigned to a new ClassifierResultDataSource. If id is empty default ID "class_res" will be assigned 
     * \return ClassifierResultDataSource object
     * \throw IncorrectParamException Data source factory is null
     */
    static SPtr<IClassifierResultDataSource> createClassifierResultDataSource(std::string id = "");

    /*!
     * \brief Create region data source
     * \param[in] id Id that will be assigned to a new region data source. If id is empty default ID "gt_regions" will be assigned 
     * \return GTRegionDataSource object
     * \throw IncorrectParamException Data source factory is null
     */
    static SPtr<IRegionDataSource> createRegionDataSource(std::string id = "");

    /*!
     * \brief Create a data source of specific type	
     * \param[in] type Type of data source to create (ClassifierResult Data Source or Region Data Source)	
     * \param[in] id Id that will be assigned to a new Region Data Source. If id is empty default ID (depending on type) will be assigned 		
     * \return Data Source (ClassifierResult Data Source or Region Data Source)		
     * \throw IncorrectParamException Data source factory is null or filter type is incorrect	
     */	
    static SPtr<Filter> createDataSource(FilterType type = CLASSIFIER_RESULTS_FILTER, std::string id = "");

    /*!
     * \brief Get Repository for given URL
     * \param[in] url URL of a Repository (filepath for XML Repository)
     * \throw IncorrectParamException Repository factory is null
     */
    static SPtr<IRepository> getRepository(std::string url);
    /*!
     * \brief Create new IDlib object used to construct DlibRegionCreator objects
     * \param[in] stabThreshold Movement threshold to start change regions on face movement
     * \param[in] stabFrames Frame number to check for face movement during face stabilization
     * \param[in] normFrames Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames).
     * In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed
     * \param[in] noFaceFrames Frequency of applying the "no face" Dlib workaround
     * \param[in] noFaceClockwiseRotation If true in the "no face" workaround we rotate the image clockwise, otherwise we rotate counter-clockwise
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true Dlib improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \param[in] resizeDownscale Downscale for every frame image to decrease face detection time
     * \return Dlib handler
     * \throw IncorrectParamException Dlib factory is null or Dlib is not found
     * \throw IOException Dlib internal error
     */
    static SPtr<IDlib> createDlib(float stabThreshold = 0.0,
        unsigned int stabFrames = 10,
        int normFrames = -1,
        int noFaceFrames = -1,
        bool noFaceClockwiseRotation = false,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false,
        double resizeDownscale = 1.0);

    /*!
     * \brief Create new IRealSense object used to construct RealSenseRegionCreator objects
     * \param[in] stabThreshold Movement threshold to start change regions on face movement
     * \param[in] stabFrames Frame number to check for face movement during face stabilization
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true RealSense improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \return RealSense handler
     * \throw IncorrectParamException RealSense factory is null or RealSense is not found
     * \throw IOException RealSense internal error
     */
    static SPtr<IRealSense> createRealSense(float stabThreshold = 0.0,
        unsigned int stabFrames = 10,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false);

    /*!
     * \brief Set Repository Factory
     * \param[in] repositoryFactory Repository Factory is responsible for creating a repository from given Repository URL
     */
    static void setRepositoryFactory(SPtr<IRepositoryFactory> repositoryFactory);

    /*!
     * \brief Set Data Source factory 
     * \param[in] dataSourceFactory Factory that create Region Data Source and Classifier Result Data Source
     */
    static void setDataSourceFactory(SPtr<IDataSourceFactory> dataSourceFactory);

    /*!
     * \brief Set Pipeline factory
     * \param[in] pipelineFactory Facotory responsible for creating a Pipeline
     */
    static void setPipelineFactory(SPtr<IPipelineFactory> pipelineFactory);
    /*!
     * \brief Dlib factory
     * \param[in] dlibFactory Factory responsible for creating low-level integration with Dlib interface
     */
    static void setDlibFactory(SPtr<IDlibFactory> dlibFactory);

    /*!
     * \brief RealSense factory
     * \param[in] rsFactory Factory responsible for creating low-level integration with RealSense interface
     */
    static void setRealSenseFactory(SPtr<IRealSenseFactory> rsFactory);

private:
    static SPtr<IRepositoryFactory> repositoryFactory;
    static std::map<std::string, WPtr<IRepository> > fromUrlToRepository;

    static SPtr<IDataSourceFactory> dataSourceFactory;

    static SPtr<IPipelineFactory> pipelineFactory;
    static SPtr<IDlibFactory> dlibFactory;
    static WPtr<IDlib> dlibHandler;

    static SPtr<IRealSenseFactory> rsFactory;
    static WPtr<IRealSense> rsHandler;

};

}
/** @}*/
#endif
