///////////////////////////////////////////////////////////////////////////
// <copyright file="factory.cpp" company="Intel Corporation">
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


#include "libivcp/factory.hpp"
#include "libivcp/performance/textfile_performance.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
SPtr<Session> Factory::createSession(string id)
{
    if (id.empty())
    {
        id = "session";
    }

    SPtr<Session> session = new Session;
    session->setId(id);
    return session;
}

SPtr<IVideoPlayer> Factory::createVideoPlayer(string id)
{
    if (id.empty())
    {
        id = "player";
    }

    SPtr<IVideoPlayer> player = new VideoPlayer();
    player->setId(id);
    player->setPlaylist(Factory::createVideoPlaylist());
    return player;
}

SPtr<VideoPlaylist> Factory::createVideoPlaylist(std::string id)
{
    if (id.empty())
    {
        id = "playlist";
    }

    SPtr<VideoPlaylist> playlist = new VideoPlaylist();
    playlist->setId(id);
    return playlist;
}

SPtr<CameraVideoSource> Factory::createCameraVideoSource(int cameraIndex, string id)
{
    SPtr<CameraVideoSource> cameraSource = createVideoSource(CAMERA, id);
    cameraSource->setCameraIndex(cameraIndex);

    return cameraSource;
}

#ifdef USE_REALSENSE
SPtr<RealSenseVideoSource> Factory::createRealSenseVideoSource(int cameraIndex, string id)
{
    SPtr<RealSenseVideoSource> rsSource = createVideoSource(RS_CAMERA, id);
    rsSource->setCameraIndex(cameraIndex);

    return rsSource;
}
#endif


SPtr<VideofileVideoSource> Factory::createVideofileVideoSource(string path, string id, bool multipleVideoFiles, bool is12Bit)
{
    SPtr<VideofileVideoSource> videoFileSource = new VideofileVideoSource(path, multipleVideoFiles, is12Bit);
    videoFileSource->setId(id);

    return videoFileSource;
}

SPtr<RawfileVideoSource> Factory::createRawfileVideoSource(std::string path, std::string id)
{
    SPtr<RawfileVideoSource> rawfileSource = new RawfileVideoSource(path);
    rawfileSource->setId(id);

    return rawfileSource;
}
    
SPtr<IVideoSource> Factory::createVideoSource(VideoSourceType type, string id)
{
    if (id.empty())
    {
        switch(type)
        {
        case VIDEOFILE:
            id = "videofile";
            break;
        case CAMERA:
            id = "camera";
            break;
        case RAWFILE:
            id = "rawfile";
            break;
        case RS_CAMERA:
            id = "rs_camera";
            break;
        default:
            IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
            break;
        }
    }

    SPtr<IVideoSource> videoSource = 0;
    switch(type)
    {
    case VIDEOFILE:
        videoSource = new VideofileVideoSource();
        break;
    case CAMERA:
        videoSource = new CameraVideoSource();
        break;
#ifdef USE_REALSENSE
    case RS_CAMERA:
        videoSource = new RealSenseVideoSource();
        break;
#endif
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
        break;
    }

    videoSource->setId(id);
    return videoSource;
}
SPtr<GTRegionCreator> Factory::createGTRegionCreator(SPtr<IRegionDataSource> source, ClassType regClass, string id)
{    
    if (id.empty())
    {
        id = "gt";
    }
    SPtr<GTRegionCreator> regionCreator = new GTRegionCreator(source, regClass);
    regionCreator->setId(id);
    return regionCreator;
}

SPtr<GTRegionCreator> Factory::createGTRegionCreator(std::string dataSourceFilepath, ClassType regClass, std::string id)
{
    SPtr<IRepository> repository = Factory::getRepository(dataSourceFilepath);
    SPtr<IRegionDataSource> regions = repository->loadRegionDataSource();
    return createGTRegionCreator(regions, regClass, id);
}

SPtr<BBoxRegionCreator> Factory::createBBoxRegionCreator(const Rect &bboxRect, string id)
{
    SPtr<BBoxRegionCreator> regionCreator = createRegionCreator(BBOX, id);
    regionCreator->setBBox(bboxRect);
    return regionCreator;
}
SPtr<DlibRegionCreator> Factory::createDlibRegionCreator(DlibRegionCreator::Settings settings, string id, SPtr<IDlib> handler)
{
    if (NULL == handler)
    {
        handler = getCachedDlib(settings.stabThreshold, settings.stabFrames, settings.normFrames, settings.noFaceFrames, settings.noFaceClockwiseRotation,
            settings.glassesFixEnabled, settings.useTemplateRegions, settings.glassesFixStrength, settings.useAutoGlassesSearch, settings.cutBackground, settings.resizeDownscale);
    }

    if (NULL == handler)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Dlib hanler is invalid (NULL pointer)");
    }

    SPtr<DlibRegionCreator> regionCreator = new DlibRegionCreator();
    regionCreator->setSearchRegionSettings(settings);
    regionCreator->setDlibHandler(handler);
    if (id.empty())
    {
        id = "dlib";
    }
    regionCreator->setId(id);

    return regionCreator;
}

SPtr<RealSenseRegionCreator> Factory::createRealSenseRegionCreator(RealSenseRegionCreator::Settings settings, string id, SPtr<IRealSense> handler)
{
    if (NULL == handler)
    {
        handler = getCachedRealSense(settings.stabThreshold, settings.stabFrames, settings.glassesFixEnabled, settings.useTemplateRegions, settings.glassesFixStrength,
            settings.useAutoGlassesSearch, settings.cutBackground);
    }

    if (NULL == handler)
    {
        IVCP_EXCEPTION(IncorrectParamException, "RealSense hanler is invalid (NULL pointer)");
    }

    SPtr<RealSenseRegionCreator> regionCreator = new RealSenseRegionCreator();
    regionCreator->setSearchRegionSettings(settings);
    regionCreator->setRealSenseHandler(handler);
    if (id.empty())
    {
        id = "realsense";
    }
    regionCreator->setId(id);

    return regionCreator;
}

SPtr<IRegionCreator> Factory::createRegionCreator(RegionCreatorType type, string id)
{
    if (id.empty())
    {
        switch(type)
        {
        case BBOX:
            id = "bbox";
            break;
        case GROUNDTRUTH:
            id = "gt";
            break;
        case DLIB_REGION:
            id = "dlib";
            break;
        case RS_REGION:
            id = "realsense";
            break;
        default:
            IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
            break;
        }
    }

    SPtr<IRegionCreator> regionCreator = 0;
    switch(type)
    {
    case BBOX:
        regionCreator = new BBoxRegionCreator(0, 0, 0, 0);
        break;
    case GROUNDTRUTH:
        IVCP_LOG_WARNING("GTRegionCreator was created with no Data Source assigned. It is recommended to use createGTRegionCreator instead");
        regionCreator = new GTRegionCreator();
        break;
    case DLIB_REGION:
        regionCreator = new DlibRegionCreator(getCachedDlib());
        break;
    case RS_REGION:
        regionCreator = new RealSenseRegionCreator(getCachedRealSense());
        break;
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
        break;
    }

    regionCreator->setId(id);
    return regionCreator;
}

SPtr<AggregatorFeature> Factory::createAggregatorFeature(int numberOfRegions, int numberOfSequences, string id)
{
    if (id.empty())
    {
        id = "aggregator";
    }

    SPtr<AggregatorFeature> aggregatorFeature = new AggregatorFeature(numberOfRegions, numberOfSequences);
    aggregatorFeature->setId(id);
    return aggregatorFeature;
}

SPtr<ConcatenatorFeature> Factory::createConcatenatorFeature(int numberOfRegions, string id)
{
    SPtr<ConcatenatorFeature> concatenatorFeature = new ConcatenatorFeature(numberOfRegions);
    if (id.empty())
    {
        id = "concatenator";
    }

    concatenatorFeature->setId(id);
    return concatenatorFeature;
}

SPtr<SummatorFeature> Factory::createSummatorFeature(SummatorFeature::Settings settings, std::string id)
{
    SPtr<SummatorFeature> summatorFeature = new SummatorFeature(settings);
    if (id.empty())
    {
        id = "summator";
    }

    summatorFeature->setId(id);
    return summatorFeature;
}

SPtr<STLBPFeature> Factory::createSTLBPFeature(STLBPFeature::Settings settings, string id)
{
    SPtr<STLBPFeature> stlbpFeature = new STLBPFeature(settings);
    if (id.empty())
    {
        id = "stlbp";
    }

    stlbpFeature->setId(id);
    return stlbpFeature;
}

SPtr<VLBPFeature> Factory::createVLBPFeature(VLBPFeature::Settings settings, string id)
{
    SPtr<VLBPFeature> vlbpFeature = new VLBPFeature(settings);
    if (id.empty())
    {
        id = "vlbp";
    }

    vlbpFeature->setId(id);
    return vlbpFeature;
}

SPtr<ClassifierFeature> Factory::createClassifierFeature(int numberOfSeq, ClassifierOutputType type, string id)
{
    if (numberOfSeq <= 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of sequences should be more than 0");
    }

    if (type != PROBABILISTIC && type != CATEGORICAL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Unknown classifier output type");
    }

    SPtr<ClassifierFeature> clFeature = new ClassifierFeature(numberOfSeq, type);
    if (id.empty())
    {
        id = "classifier_feature";
    }
    
    clFeature->setId(id);
    return clFeature;
}

SPtr<NormalFlowBlurredFeature> Factory::createNormalFlowBlurredFeature(float threshold, NFBFType outputType, bool robustFlag, string id)
{
    if (id.empty())
    {
        id = "nfb";
    }
    SPtr<NormalFlowBlurredFeature> nfFeature = new NormalFlowBlurredFeature(threshold, outputType, robustFlag);
    nfFeature->setId(id);
    return nfFeature;
}

SPtr<NormalFlowFeature> Factory::createNormalFlowFeature(float threshold, string id)
{
    if (id.empty())
    {
        id = "nf";
    }
    SPtr<NormalFlowFeature> nfFeature = new NormalFlowFeature(threshold);
    nfFeature->setId(id);
    return nfFeature;
}

SPtr<MotionEstimationStdDevFeature> Factory::createMotionEstimationStdDevFeature(int numberOfSec, std::string id)
{
    if (id.empty())
    {
        id = "mestddev";
    }
    SPtr<MotionEstimationStdDevFeature> mestddevFeature = new MotionEstimationStdDevFeature(numberOfSec);
    mestddevFeature->setId(id);
    return mestddevFeature;
}

SPtr<IFeature> Factory::createFeature(FeatureType type, string id)
{
    if (id.empty())
    {
        switch(type)
        {
        case NORMALFLOW:
            id = "nf";
            break;
        case NORMALFLOWBLURRED:
            id = "nfb";
            break;
        case STLBP:
            id = "stlbp";
            break;
        case AGGREGATOR:
            id = "aggregator";
            break;
        case CONCATENATOR:
            id = "concatenator";
            break;
        case VLBP:
            id = "vlbp";
            break;
        case CLASSIFIER_FEATURE:
            id = "classifier_feature";
            break;
        case SUMMATOR:
            id = "summator";
            break;
        case MOTIONESTIMATIONSTDDEV:
            id = "mestddev";
            break;
        default:
            IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
            break;
        }
    }

    SPtr<IFeature> feature = 0;
    switch(type)
    {
    case NORMALFLOW:
        feature = new NormalFlowFeature;
        break;
    case NORMALFLOWBLURRED:
        feature = new NormalFlowBlurredFeature;
        break;
    case STLBP:
        feature = new STLBPFeature;
        break;
    case AGGREGATOR:
        feature = new AggregatorFeature;
        break;
    case CONCATENATOR:
        feature = new ConcatenatorFeature;
        break;
    case VLBP:
        feature = new VLBPFeature;
        break;
    case CLASSIFIER_FEATURE:
        feature = new ClassifierFeature;
        break;
    case SUMMATOR:
        feature = new SummatorFeature;
        break;
    case MOTIONESTIMATIONSTDDEV:
        feature = new MotionEstimationStdDevFeature;
        break;
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
        break;
    }

    feature->setId(id);
    return feature;
}

SPtr<IClassifier> Factory::createClassifier(ClassifierType type, string id)
{
    if (id.empty())
    {
        switch(type)
        {
        case SVM:
            id = "svm";
            break;
        case LOGISTIC_REGRESSION:
            id = "logistic_regression";
            break;
        default:
            IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
            break;
        }
    }

    SPtr<IClassifier> classifier = 0;
    switch(type)
    {
    case SVM:
        classifier = new SVMClassifier();
        break;
    case LOGISTIC_REGRESSION:
        classifier = new LRClassifier();
        break;
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
        break;
    }

    classifier->setId(id);
    return classifier;
}


SPtr<SVMClassifier> Factory::createSVMClassifier(SVMParameters classifierParams, string id)
{
    SPtr<SVMClassifier> classifier = createClassifier(SVM, id);
    classifier->setClassifierParams(classifierParams);
    return classifier;
}

SPtr<LRClassifier> Factory::createLRClassifier(LRParameters classifierParams, string id)
{
    SPtr<LRClassifier> classifier = createClassifier(LOGISTIC_REGRESSION, id);
    classifier->setClassifierParams(classifierParams);
    return classifier;
}

SPtr<IPerformance> Factory::createPerformance(PerformanceType type, string id)
{
    if (id.empty())
    {
        switch(type)
        {
        case TEXT_REPORT:
            id = "txt_report";
            break;
        default:
            IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
            break;
        }
    }

    SPtr<IPerformance> performance = 0;
    switch(type)
    {
    case TEXT_REPORT:
        performance = new TextPerformance;
        break;
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");
        break;
    }

    performance->setId(id);
    return performance;
}

SPtr<IClassifierResultDataSource> Factory::createClassifierResultDataSource(string id)
{
    if (dataSourceFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "dataSourceFactory is null");
    }

    return dataSourceFactory->getClassifierResultDataSource(id);
}

SPtr<IRegionDataSource> Factory::createRegionDataSource(string id)
{
    if (dataSourceFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "dataSourceFactory is null");
    }

    return dataSourceFactory->getRegionDataSource(id);
}

SPtr<Filter> Factory::createDataSource(FilterType type, string id)		
{		
    SPtr<IFilter> dataSource = 0;		
    switch(type)		
    {		
    case CLASSIFIER_RESULTS_FILTER:		
        dataSource = createClassifierResultDataSource(id);	
        break;		
    case GROUNDTRUTH_REGIONS_FILTER:		
        dataSource = createRegionDataSource(id);		
        break;		
    default:	
        IVCP_EXCEPTION(IncorrectParamException, "Unknown filter type");		
        break;	
    }

    return dataSource;		
}

SPtr<IPipeline> Factory::createPipeline(PipelineMode mode, IPipeline::Settings settings, string id)
{
    if (pipelineFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "pipelineFactory is null");
    }

    return pipelineFactory->getPipeline(mode, id, settings);
}

SPtr<IPipeline> Factory::createParallelPipeline(PipelineMode mode, IPipeline::Settings settings, string id)
{
    if (pipelineFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "pipelineFactory is null");
    }

    return pipelineFactory->getParallelPipeline(mode, id, settings);
}

SPtr<IGUIFramework> Factory::createGUIFramework(SPtr<IPipeline> pipeline, float framerate)
{
    if (pipelineFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "pipelineFactory is null");
    }

    return pipelineFactory->getGUIFramework(pipeline, framerate);
}

SPtr<IRepository> Factory::getRepository(string url)
{
    if (fromUrlToRepository.find(url) != fromUrlToRepository.end())
    {
        SPtr<IRepository> repository(fromUrlToRepository[url]);
        if (!repository.isEmpty())
        {
            return repository;
        }
        else
        {
            //The object is already deallocated
            fromUrlToRepository.erase(url);
        }
    }

    if (repositoryFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "repositoryFactory is null");
    }

    SPtr<IRepository> repository = repositoryFactory->getRepository(url);
    fromUrlToRepository[url] = repository;
    return repository;
}
SPtr<IDlib> Factory::getCachedDlib(float stabThreshold, unsigned int stabFrames, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled, bool useTemplateRegions, 
                                 float glassesFixStrength, bool useAutoGlassesSearch, bool cutBackground, double resizeDownscale)
{
    SPtr<IDlib> dlibH = dlibHandler;
    if (dlibH.isEmpty())
    {
        dlibH = createDlib(stabThreshold, stabFrames, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled, useTemplateRegions, 
            glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);
    }
    return dlibH;
}

bool Factory::isDlibHandlerCreated()
{
    SPtr<IDlib> dlibH = dlibHandler;
    if (dlibH == NULL)
    {
        return false;
    }
    return true;
}

SPtr<IDlib> Factory::createDlib(float stabThreshold, unsigned int stabFrames, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled, bool useTemplateRegions, 
                              float glassesFixStrength, bool useAutoGlassesSearch, bool cutBackground, double resizeDownscale)
{
    if (dlibFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "dlibFactory is null");
    }
    SPtr<IDlib> dlibH = dlibFactory->getDlib(stabThreshold, stabFrames, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled, useTemplateRegions, 
        glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);
    dlibHandler = dlibH;

    return dlibH;
}

SPtr<IRealSense> Factory::getCachedRealSense(float stabThreshold, unsigned int stabFrames, bool glassesFixEnabled, bool useTemplateRegions, float glassesFixStrength,
                                             bool useAutoGlassesSearch, bool cutBackground)
{
    SPtr<IRealSense> rsH = rsHandler;
    if (rsH.isEmpty())
    {
        rsH = createRealSense(stabThreshold, stabFrames, glassesFixEnabled, useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground);
    }
    return rsH;
}

bool Factory::isRealSenseHandlerCreated()
{
    SPtr<IRealSense> rsH = rsHandler;
    if (rsH == NULL)
    {
        return false;
    }
    return true;
}

SPtr<IRealSense> Factory::createRealSense(float stabThreshold, unsigned int stabFrames, bool glassesFixEnabled, bool useTemplateRegions, 
                              float glassesFixStrength, bool useAutoGlassesSearch, bool cutBackground)
{
    if (rsFactory == 0)
    {
        IVCP_EXCEPTION(IncorrectParamException, "rsFactory is null");
    }
    SPtr<IRealSense> rsH = rsFactory->getRealSense(stabThreshold, stabFrames, glassesFixEnabled, useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground);
    rsHandler = rsH;

    return rsH;
}

void Factory::setRepositoryFactory(SPtr<IRepositoryFactory> _repositoryFactory)
{
    repositoryFactory = _repositoryFactory;
}

void Factory::setDataSourceFactory(SPtr<IDataSourceFactory> _dataSourceFactory)
{
    dataSourceFactory = _dataSourceFactory;
}

void Factory::setPipelineFactory(SPtr<IPipelineFactory> _pipelineFactory)
{
    pipelineFactory = _pipelineFactory;
}
void Factory::setDlibFactory(SPtr<IDlibFactory> _dlibFactory)
{
    dlibFactory = _dlibFactory;
}

void Factory::setRealSenseFactory(SPtr<IRealSenseFactory> _rsFactory)
{
    rsFactory = _rsFactory;
}

SPtr<IRepositoryFactory> Factory::repositoryFactory = 0;
std::map<string, WPtr<IRepository> > Factory::fromUrlToRepository;
SPtr<IDataSourceFactory> Factory::dataSourceFactory = 0;
SPtr<IPipelineFactory> Factory::pipelineFactory = 0;
WPtr<IDlib> Factory::dlibHandler;
SPtr<IDlibFactory> Factory::dlibFactory = 0;
WPtr<IRealSense> Factory::rsHandler;
SPtr<IRealSenseFactory> Factory::rsFactory = 0;
}

