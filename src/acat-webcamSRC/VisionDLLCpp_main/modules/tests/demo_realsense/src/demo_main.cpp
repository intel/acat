// Copyright (C) 2015, Intel Corporation, all rights reserved.
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

float stabThreshold = 0.0F;
unsigned int stabFrames = 10U;
bool glassesFixEnabled = false;
bool useTemplateRegions = true;
float glassesFixStrength = 0.0F;
bool useAutoGlassesSearch = true;
bool cutBackground = true;
bool tracking3d = true;

int main(int argc, char *argv[])
{
    (void) argc;
    (void) argv;

    ivcpInit();

    SPtr<Session> session = Factory::createSession("session");

    SPtr<VideoPlayer> videoPlayer = Factory::createVideoPlayer();
    SPtr<VideoPlaylist> videoPlaylist = Factory::createVideoPlaylist();

    SPtr<RealSenseVideoSource> camera = Factory::createRealSenseVideoSource();
    //vector<pair<int, string>> allDevicesRS = RealSenseVideoSource::listDevices();
    //vector<pair<int, string>> allDevicesCV = CameraVideoSource::listDevices();
    camera->setOutputResolution(640, 480);
    camera->set3dTracking(tracking3d);
    camera->openCamera(0);
    FrameRange videorange;
    videoPlaylist->addSourceRange(camera, videorange);
    session->setVideoPlaylist(videoPlaylist);

    SPtr<IRealSense> rsHandler = Factory::createRealSense(stabThreshold, stabFrames, glassesFixEnabled, useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground);

    bool normalize = true;
    SPtr<RealSenseRegionCreator> regionFace = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_FACE, 1.0, 1.0, normalize));
        regionFace->setId("regionFace");
        session->addRegionCreator(regionFace);
    SPtr<RealSenseRegionCreator> regionLEyebrow = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_LEYEBROW, 1.0, 1.0, normalize));
        regionLEyebrow->setId("regionLEyebrow");
        session->addRegionCreator(regionLEyebrow);
    SPtr<RealSenseRegionCreator> regionREyebrow = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_REYEBROW, 1.0, 1.0, normalize));
        regionREyebrow->setId("regionREyebrow");
        session->addRegionCreator(regionREyebrow);
    SPtr<RealSenseRegionCreator> regionMouth = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_MOUTH, 1.0, 1.0, normalize));
        regionMouth->setId("regionMouth");
        session->addRegionCreator(regionMouth);
    SPtr<RealSenseRegionCreator> regionNose = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_NOSE, 1.0, 1.0, normalize));
        regionNose->setId("regionNose");
        session->addRegionCreator(regionNose);
    SPtr<RealSenseRegionCreator> regionLEye = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_LEYE, 1.0, 1.0, normalize));
        regionLEye->setId("regionLEye");
        session->addRegionCreator(regionLEye);
    SPtr<RealSenseRegionCreator> regionREye = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_REYE, 1.0, 1.0, normalize));
        regionREye->setId("regionREye");
        session->addRegionCreator(regionREye);
    SPtr<RealSenseRegionCreator> regionLCheek = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_LCHEEK, 1.0, 1.0, normalize));
        regionLCheek->setId("regionLCheek");
        session->addRegionCreator(regionLCheek);
    SPtr<RealSenseRegionCreator> regionRCheek = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_RCHEEK, 1.0, 1.0, normalize));
        regionRCheek->setId("regionRCheek");
        session->addRegionCreator(regionRCheek);

    SPtr<TextPerformance> performance = Factory::createPerformance();
        performance->setId("performance");
        session->addPerformanceFilter(performance);

   SPtr<Pipeline> pipeline = Factory::createPipeline(RUNTIME_PIPELINE);
        pipeline->setSession(session);

    pipeline->addLink(session->getVideoPlayer()->getId(), "regionFace");
    pipeline->addLink("regionFace", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionLEyebrow");
    pipeline->addLink("regionLEyebrow", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionREyebrow");
    pipeline->addLink("regionREyebrow", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionMouth");
    pipeline->addLink("regionMouth", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionNose");
    pipeline->addLink("regionNose", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionLEye");
    pipeline->addLink("regionLEye", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionREye");
    pipeline->addLink("regionREye", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionLCheek");
    pipeline->addLink("regionLCheek", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionRCheek");
    pipeline->addLink("regionRCheek", "performance");

    SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
    pipeline->run();

    return 0;
}