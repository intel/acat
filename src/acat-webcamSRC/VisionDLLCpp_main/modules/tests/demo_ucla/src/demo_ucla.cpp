///////////////////////////////////////////////////////////////////////////
// <copyright file="demo_ucla.cpp" company="Intel Corporation">
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

#include <vector>
#include <string>
#include "demo_environment.hpp"
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

namespace
{
const char *trainVideos[] =
{
"ucladb_boiling-b-near_2s1.avi",
"ucladb_boiling-b-near_2s2.avi",
"ucladb_boiling-b-near_3s1.avi",
"ucladb_boiling-c-mid_1s1.avi",
"ucladb_boiling-c-mid_1s2.avi",
"ucladb_boiling-c-mid_2s1.avi",
"ucladb_candle_4s1.avi",
"ucladb_candle_4s2.avi",
"ucladb_candle_5s1.avi",
"ucladb_fire_3s1.avi",
"ucladb_fire_3s2.avi",
"ucladb_fire_7s1.avi",
"ucladb_flowers-c-mid_2s1.avi",
"ucladb_flowers-c-mid_2s2.avi",
"ucladb_flowers-c-mid_3s1.avi",
"ucladb_flowers-d-mid_1s1.avi",
"ucladb_flowers-d-mid_1s2.avi",
"ucladb_flowers-d-mid_2s1.avi",
"ucladb_flowers-d-near_1s1.avi",
"ucladb_flowers-d-near_1s2.avi",
"ucladb_flowers-d-near_2s1.avi",
"ucladb_fountain-a-far_1s1.avi",
"ucladb_fountain-a-far_1s2.avi",
"ucladb_fountain-a-far_2s1.avi",
"ucladb_fountain-a-mid_1s1.avi",
"ucladb_fountain-a-mid_1s2.avi",
"ucladb_fountain-a-mid_2s1.avi",
"ucladb_fountain-a-near_1s1.avi",
"ucladb_fountain-a-near_1s2.avi",
"ucladb_fountain-a-near_2s1.avi",
"ucladb_fountain-b-near_1s1.avi",
"ucladb_fountain-b-near_1s2.avi",
"ucladb_fountain-b-near_2s1.avi",
"ucladb_fountain-c-far_2s1.avi",
"ucladb_fountain-c-far_2s2.avi",
"ucladb_fountain-c-far_3s1.avi",
"ucladb_plant-a-far_1s1.avi",
"ucladb_plant-a-far_1s2.avi",
"ucladb_plant-a-far_2s1.avi",
"ucladb_plant-a-near_1s1.avi",
"ucladb_plant-a-near_1s2.avi",
"ucladb_plant-a-near_2s1.avi",
"ucladb_plant-c-mid_1s1.avi",
"ucladb_plant-c-mid_1s2.avi",
"ucladb_plant-c-mid_2s1.avi",
"ucladb_plant-d-mid_1s1.avi",
"ucladb_plant-d-mid_1s2.avi",
"ucladb_plant-d-mid_2s1.avi",
"ucladb_plant-d-near_1s1.avi",
"ucladb_plant-d-near_1s2.avi",
"ucladb_plant-d-near_2s1.avi",
"ucladb_plant-e-mid_1s1.avi",
"ucladb_plant-e-mid_1s2.avi",
"ucladb_plant-e-mid_3s1.avi",
"ucladb_plant-e-near_1s1.avi",
"ucladb_plant-e-near_1s2.avi",
"ucladb_plant-e-near_2s1.avi",
"ucladb_plant-f-far_1s1.avi",
"ucladb_plant-f-far_1s2.avi",
"ucladb_plant-f-far_2s1.avi",
"ucladb_plant-f-near_1s1.avi",
"ucladb_plant-f-near_1s2.avi",
"ucladb_plant-f-near_2s1.avi",
"ucladb_plant-g-mid_1s1.avi",
"ucladb_plant-g-mid_1s2.avi",
"ucladb_plant-g-mid_2s1.avi",
"ucladb_plant-g-near_1s1.avi",
"ucladb_plant-g-near_1s2.avi",
"ucladb_plant-g-near_2s1.avi",
"ucladb_plant-h-mid_1s1.avi",
"ucladb_plant-h-mid_1s2.avi",
"ucladb_plant-h-mid_2s1.avi",
"ucladb_plant-h-near_1s1.avi",
"ucladb_plant-h-near_1s2.avi",
"ucladb_plant-h-near_2s1.avi",
"ucladb_plant-i-mid_1s1.avi",
"ucladb_plant-i-mid_1s2.avi",
"ucladb_plant-i-mid_2s1.avi",
"ucladb_plant-i-near_1s1.avi",
"ucladb_plant-i-near_1s2.avi",
"ucladb_plant-i-near_2s1.avi",
"ucladb_plant-l-mid_1s1.avi",
"ucladb_plant-l-mid_1s2.avi",
"ucladb_plant-l-mid_2s1.avi",
"ucladb_plant-m-mid_1s1.avi",
"ucladb_plant-m-mid_1s2.avi",
"ucladb_plant-m-mid_2s1.avi",
"ucladb_plant-m-near_1s1.avi",
"ucladb_plant-m-near_1s2.avi",
"ucladb_plant-m-near_2s1.avi",
"ucladb_plant-n-mid_1s1.avi",
"ucladb_plant-n-mid_1s2.avi",
"ucladb_plant-n-mid_2s1.avi",
"ucladb_plant-o-far_1s1.avi",
"ucladb_plant-o-far_1s2.avi",
"ucladb_plant-o-far_2s1.avi",
"ucladb_plant-q-far_1s1.avi",
"ucladb_plant-q-far_1s2.avi",
"ucladb_plant-q-far_2s1.avi",
"ucladb_plant-r-mid_1s1.avi",
"ucladb_plant-r-mid_1s2.avi",
"ucladb_plant-r-mid_2s1.avi",
"ucladb_plant-s-far_1s1.avi",
"ucladb_plant-s-far_1s2.avi",
"ucladb_plant-s-far_2s1.avi",
"ucladb_plant-s-mid_1s1.avi",
"ucladb_plant-s-mid_1s2.avi",
"ucladb_plant-s-mid_2s1.avi",
"ucladb_plant-t-mid_1s1.avi",
"ucladb_plant-t-mid_1s2.avi",
"ucladb_plant-t-mid_2s1.avi",
"ucladb_plant-t-near_1s1.avi",
"ucladb_plant-t-near_1s2.avi",
"ucladb_plant-t-near_2s1.avi",
"ucladb_plant-u-far_1s1.avi",
"ucladb_plant-u-far_1s2.avi",
"ucladb_plant-u-far_2s1.avi",
"ucladb_see-a-mid_1s1.avi",
"ucladb_see-a-mid_1s2.avi",
"ucladb_see-a-mid_2s1.avi",
"ucladb_see-e-near_3s1.avi",
"ucladb_see-e-near_3s2.avi",
"ucladb_see-e-near_4s1.avi",
"ucladb_see-i-far_2s1.avi",
"ucladb_see-i-far_2s2.avi",
"ucladb_see-i-far_3s1.avi",
"ucladb_smoke-b_4s1.avi",
"ucladb_smoke-b_4s2.avi",
"ucladb_smoke-b_6s1.avi",
"ucladb_water-a-near_1s1.avi",
"ucladb_water-a-near_1s2.avi",
"ucladb_water-a-near_2s1.avi",
"ucladb_water-b-mid_1s1.avi",
"ucladb_water-b-mid_1s2.avi",
"ucladb_water-b-mid_2s1.avi",
"ucladb_water-b-near_1s1.avi",
"ucladb_water-b-near_1s2.avi",
"ucladb_water-b-near_2s1.avi",
"ucladb_wfalls-a-near_1s1.avi",
"ucladb_wfalls-a-near_1s2.avi",
"ucladb_wfalls-a-near_2s1.avi",
"ucladb_wfalls-b-near_1s1.avi",
"ucladb_wfalls-b-near_1s2.avi",
"ucladb_wfalls-b-near_2s1.avi",
"ucladb_wfalls-c-near_1s1.avi",
"ucladb_wfalls-c-near_1s2.avi",
"ucladb_wfalls-c-near_2s1.avi",
"ucladb_wfalls-d-near_1s1.avi",
"ucladb_wfalls-d-near_1s2.avi",
"ucladb_wfalls-d-near_2s1.avi"
};

int trainMarking[] = 
{
0,
0,
0,
0,
0,
0,
1,
1,
1,
2,
2,
2,
3,
3,
3,
3,
3,
3,
3,
3,
3,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
6,
6,
6,
6,
6,
6,
6,
6,
6,
7,
7,
7,
8,
8,
8,
8,
8,
8,
8,
8,
8,
9,
9,
9,
9,
9,
9,
9,
9,
9,
9,
9,
9
};

const char *testVideos[] =
{
"ucladb_boiling-b-near_3s2.avi",
"ucladb_boiling-c-mid_2s2.avi",
"ucladb_candle_5s2.avi",
"ucladb_fire_7s2.avi",
"ucladb_flowers-c-mid_3s2.avi",
"ucladb_flowers-d-mid_2s2.avi",
"ucladb_flowers-d-near_2s2.avi",
"ucladb_fountain-a-far_2s2.avi",
"ucladb_fountain-a-mid_2s2.avi",
"ucladb_fountain-a-near_2s2.avi",
"ucladb_fountain-b-near_2s2.avi",
"ucladb_fountain-c-far_3s2.avi",
"ucladb_plant-a-far_2s2.avi",
"ucladb_plant-a-near_2s2.avi",
"ucladb_plant-c-mid_2s2.avi",
"ucladb_plant-d-mid_2s2.avi",
"ucladb_plant-d-near_2s2.avi",
"ucladb_plant-e-mid_3s2.avi",
"ucladb_plant-e-near_2s2.avi",
"ucladb_plant-f-far_2s2.avi",
"ucladb_plant-f-near_2s2.avi",
"ucladb_plant-g-mid_2s2.avi",
"ucladb_plant-g-near_2s2.avi",
"ucladb_plant-h-mid_2s2.avi",
"ucladb_plant-h-near_2s2.avi",
"ucladb_plant-i-mid_2s2.avi",
"ucladb_plant-i-near_2s2.avi",
"ucladb_plant-l-mid_2s2.avi",
"ucladb_plant-m-mid_2s2.avi",
"ucladb_plant-m-near_2s2.avi",
"ucladb_plant-n-mid_2s2.avi",
"ucladb_plant-o-far_2s2.avi",
"ucladb_plant-q-far_2s2.avi",
"ucladb_plant-r-mid_2s2.avi",
"ucladb_plant-s-far_2s2.avi",
"ucladb_plant-s-mid_2s2.avi",
"ucladb_plant-t-mid_2s2.avi",
"ucladb_plant-t-near_2s2.avi",
"ucladb_plant-u-far_2s2.avi",
"ucladb_see-a-mid_2s2.avi",
"ucladb_see-e-near_4s2.avi",
"ucladb_see-i-far_3s2.avi",
"ucladb_smoke-b_6s2.avi",
"ucladb_water-a-near_2s2.avi",
"ucladb_water-b-mid_2s2.avi",
"ucladb_water-b-near_2s2.avi",
"ucladb_wfalls-a-near_2s2.avi",
"ucladb_wfalls-b-near_2s2.avi",
"ucladb_wfalls-c-near_2s2.avi",
"ucladb_wfalls-d-near_2s2.avi"
};

int testMarking[] =
{
0,
0,
1,
2,
3,
3,
3,
4,
4,
4,
4,
4,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
5,
6,
6,
6,
7,
8,
8,
8,
9,
9,
9,
9
};

class UclaTrainClassifierResultDataSource: public IClassifierResultDataSource
{
public:
    virtual ClassType getClassifyResult(VideoPlaylistFrame frame)
    {
        return trainMarking[frame.sourceIndex];
    };

    virtual bool isClassified(VideoPlaylistFrame frame)
    {
        (void)frame;
        return true;
    };

    virtual ClassifierResultType getType()
    {
        return NEW_CLASSIFIER_RESULTS;
    };
    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType mark) {(void)frame; (void)mark;};
    virtual void removeClassifyResult(VideoPlaylistFrame frame) {(void)frame;};
};

class UclaTestClassifierResultDataSource: public IClassifierResultDataSource
{
public:
    virtual ClassType getClassifyResult(VideoPlaylistFrame frame)
    {
        return testMarking[frame.sourceIndex];
    };

    virtual bool isClassified(VideoPlaylistFrame frame)
    {
        (void)frame;
        return true;
    };

    virtual ClassifierResultType getType()
    {
        return NEW_CLASSIFIER_RESULTS;
    };
    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType mark) {(void)frame; (void)mark;};
    virtual void removeClassifyResult(VideoPlaylistFrame frame) {(void)frame;};
};

Mat drawImage;
string window;
SPtr<IClassifierResultDataSource> markup;
SPtr<const VideoPlaylist> demo_playlist;

void sourceCallback(Mat mat, void* param)
{
    (void)param;

    VideoPlaylistFrame currentFrame = demo_playlist->getCurrentFrame();
    drawImage = mat.clone();
    if( markup->isClassified(currentFrame) )
    {
        stringstream mark;
        mark << "mark: " << markup->getClassifyResult(currentFrame);
        putText(drawImage, mark.str(), Point(1, 10), FONT_ITALIC, 0.35, Scalar(255) );
    }
};

void classifierCallback(Mat mat, void* param)
{
    (void)param;

    ClassType callData = (ClassType)mat.at<float>(0, 0);
    stringstream pred;
    pred << "pred: " << callData;
    putText(drawImage, pred.str(), Point(1, 20), FONT_ITALIC, 0.35, Scalar(255) );
    imshow(window, drawImage);
    cv::waitKey(33);
};
}

void uclaTest()
{
    SPtr<Session> session = Factory::createSession();
    
    SPtr<VideoPlayer > videoPlayerTrain = Factory::createVideoPlayer();
    SPtr<VideoPlaylist > videoPlaylistTrain = videoPlayerTrain->getPlaylist();

    //Create train playlist
    stringstream sourceFilename;
    int trainSize = sizeof(trainMarking) / sizeof(trainMarking[0]);

    for (int i = 0; i < trainSize; i++)
    {
        sourceFilename << UCLA2_FOLDER << trainVideos[i];
        SPtr<IVideoSource > source = Factory::createVideofileVideoSource(sourceFilename.str());
        sourceFilename.str("");
        videoPlaylistTrain->addSourceRange(source, FrameRange(0, END_FRAME));
    }

    SPtr<IFeature> feature = Factory::createSTLBPFeature(STLBPFeature::Settings(4, 1, 1, 1, 4, 4, 4, 4, 4, 1));
    feature->setId("feature");

    SPtr<BBoxRegionCreator > bbox1 = Factory::createBBoxRegionCreator(Rect(0, 0, 48, 48), "bbox1");

    SPtr<IClassifierResultDataSource> classifierResultTrain = new UclaTrainClassifierResultDataSource;
    classifierResultTrain->addClassifyResult(VideoPlaylistFrame(), 0);
    classifierResultTrain->removeClassifyResult(VideoPlaylistFrame());
    SPtr<IClassifierResultDataSource> classifierResultTest = new UclaTestClassifierResultDataSource;
    classifierResultTest->addClassifyResult(VideoPlaylistFrame(), 0);
    classifierResultTest->removeClassifyResult(VideoPlaylistFrame());

    SPtr<IClassifier > classifier = Factory::createClassifier(ivcp::SVM, "classifier");
    classifierResultTrain->setId("classifier_result_train");
    classifierResultTest->setId("classifier_result_test");

    session->setVideoPlaylist(videoPlaylistTrain);

    session->addFeature(feature);
    session->addRegionCreator(bbox1);
    session->addClassifier(classifier);
    session->addDataSource(classifierResultTrain);
    SPtr<IPerformance> perf = Factory::createPerformance();
    session->addPerformanceFilter(perf);

    //Create Pipeline for train
    cout << "Training..." << endl;
    SPtr<Pipeline> pipelineTrain = Factory::createPipeline(TRAIN_PIPELINE);
    pipelineTrain->setSession(session);
    pipelineTrain->connectAll();
    pipelineTrain->run();

    //Set test video player
    SPtr<VideoPlayer > videoPlayerTest = Factory::createVideoPlayer();
    SPtr<VideoPlaylist > videoPlaylistTest = videoPlayerTest->getPlaylist();

    int testSize = sizeof(testMarking) / sizeof(testMarking[0]);
    for (int i = 0; i < testSize; i++)
    {
        sourceFilename << UCLA2_FOLDER << testVideos[i];
        SPtr<IVideoSource > source = Factory::createVideofileVideoSource(sourceFilename.str());
        sourceFilename.str("");
        videoPlaylistTest->addSourceRange(source, FrameRange(0, END_FRAME));
    }

    session->setVideoPlaylist(videoPlaylistTest);

    //Create Pipeline for test
    session->getVideoPlayer()->getPlaylist()->seek(0, 0);
    session->delDataSource(classifierResultTrain);
    session->addDataSource(classifierResultTest);

    cout << "Training finished" << endl;
    cout << "Test..." << endl;
    SPtr<Pipeline> pipelineTest = Factory::createPipeline(TEST_PIPELINE);
    pipelineTest->setSession(session);
    pipelineTest->connectAll();

    demo_playlist = session->getVideoPlaylist();
    markup = classifierResultTest;
    namedWindow(window, cv::WINDOW_NORMAL);
    pipelineTest->addNodeCallback("player", &sourceCallback, NULL);
    pipelineTest->addNodeCallback(classifier->getId(), &classifierCallback, NULL);

    pipelineTest->run();

    cout << "Test finished" << endl;

    perf->generateQualityReport("./utest2_quality_1_1.txt");
    cout << "Quality report was generated" << endl;
}


int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    ivcpInit();
    IVCP_LOG_TO_FILE("ucla_log.txt");

    window = "Ucla demo";
    uclaTest();
    destroyWindow(window);

    return 0;
}
