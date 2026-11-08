///////////////////////////////////////////////////////////////////////////
// <copyright file="demo.cpp" company="Intel Corporation">
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

#include <ctime>
#include "demo_environment.hpp"
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

namespace 
{
class MockVideoSource: public VideoSource
{
public:
    MockVideoSource(int type, int duration, int count)
    {
        time_t t;
        rnd( (unsigned int) time(&t) );
        this->type = type;
        this->duration = duration;
        this->count = count;
        current = -1;
        i = 0; 
        j = 0;

        lsx = 0;
        lsy = 0;
        psx = 0;
        psy = 0;
        ldx = 0;
        ldy = 0;
        lfdx = 0;
        lfdy = 0;
    };

    virtual ~MockVideoSource() {};

    virtual Mat getNextFrame()
    {
        return generate_frame();
    };

    virtual void seek(int frame)
    {
        current = frame - 1;
    };

    virtual VideoSourceType getType()
    {
        return VIDEOFILE;
    };

    virtual void open() {}
    virtual void release() {}

protected:
    int current;
    int i, j;
    RNG rnd;
    int type, duration, count;
    unsigned int lsx, lsy, psx, psy;
    int ldx, ldy;
    float lfdx, lfdy;

    //0 - random, 1 - point, 2 - line
    Mat generate_frame()
    {
        Mat img;
        current++;
        if (j < count)
        {
            int genline = (type == 0) ? (1 - j % 2) : (type - 1);
            if(genline)
            {
                if (i % duration == 0)
                {
                    lsx = rnd.uniform(60, 140);
                    lsy = rnd.uniform(60, 140);
                    ldx = rnd.uniform(20, 40);
                    ldy = rnd.uniform(20, 40);
                    int s = 1;
                    float lnrm = sqrt((float)ldy * ldy + ldx * ldx);
                    lfdx = (ldy * s / lnrm);
                    lfdy = (-ldx * s / lnrm);
                    i = 0;
                }
                unsigned int x = (int)(lsx + i * lfdx);
                unsigned int y = (int)(lsy + i * lfdy);
                img = Mat::zeros(200, 200, CV_8UC3);
                line(img, Point(x, y), Point(x+ldx, y+ldy), Scalar(255, 255, 255));
                i++;
            }
            else
            {
                if (i % duration == 0)
                {
                    psx = rnd.uniform(20, 180);
                    psy = rnd.uniform(20, 180);
                    i = 0;
                }
                float dx = 1;
                float dy = 1;
                unsigned int x = (int)(psx + i * dx);
                unsigned int y = (int)(psy + i * dy);
                img = Mat::zeros(200, 200, CV_8UC3);
                circle(img, Point(x, y), 2, Scalar(255, 255, 255));
                i++;
            }

            if (!img.empty() && img.channels() == 1)
            {
                cvtColor(img, img, cv::COLOR_GRAY2BGR);
            }

            if (i % duration == 0)
            {
                j++;
            }
        }
        return img;
    }
};
}

namespace
{
class MockClassifierResultDataSourceDemo: public IClassifierResultDataSource
{
public:
    MockClassifierResultDataSourceDemo(bool interleaveClasses)
    {
        interleave = interleaveClasses;
    }

    virtual ClassType getClassifyResult(VideoPlaylistFrame frame)
    {
        return interleave ? (frame.frameIndex / 60 % 2) : frame.sourceIndex;
    }

    virtual bool isClassified(VideoPlaylistFrame frame)
    {
        return (frame.frameIndex % 60) != 0;
    }

    virtual ClassifierResultType getType()
    {
        return NEW_CLASSIFIER_RESULTS;
    };
    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType mark) {(void)frame; (void)mark;};
    virtual void removeClassifyResult(VideoPlaylistFrame frame) {(void)frame;};

protected:
    bool interleave;
};
}

Mat drawImage;
VideoPlaylistFrame currentFrame;
IClassifierResultDataSource* markup;
const VideoPlaylist* playlist;

void callSource(Mat mat, void *param)
{
    (void)param;

    currentFrame = playlist->getCurrentFrame();
    drawImage = mat.clone();
    if( markup->isClassified(currentFrame) )
    switch( markup->getClassifyResult(currentFrame) )
    {
    case 0:
        putText(drawImage, "mark: line", Point(5, 15), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
        break;
    case 1:
        putText(drawImage, "mark: point", Point(5, 15), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
        break;
    default:
        putText(drawImage, "mark: unknown", Point(5, 15), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
        break;
    }
};

void callSourceTrain(Mat mat, void *param)
{
    callSource(mat, param);
    putText(drawImage, "training", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
    if (!drawImage.empty())
    {
        imshow("Demo", drawImage);
    }
    cv::waitKey(33);
};

void callClassifier(Mat mat, void *param)
{
    (void)param;

    ClassType callData = (ClassType)mat.at<float>(0, 0);
    switch(callData)
    {
    case 0:
        putText(drawImage, "pred: line", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
        break;
    case 1:
        putText(drawImage, "pred: point", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
        break;
    default:
        putText(drawImage, "pred: unknown", Point(5, 30), FONT_HERSHEY_COMPLEX, 0.5, Scalar(255) );
        break;
    }
    imshow("Demo",drawImage);
    cv::waitKey(33);
};

int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    ivcpInit();

    SPtr<TextPerformance> test_performance = Factory::createPerformance(TEXT_REPORT, "performance");
    SPtr<MockClassifierResultDataSourceDemo > classifier_result = new MockClassifierResultDataSourceDemo(false);
    classifier_result->addClassifyResult(VideoPlaylistFrame(), 0);
    classifier_result->removeClassifyResult(VideoPlaylistFrame());
    SPtr<MockClassifierResultDataSourceDemo > classifier_test_result = new MockClassifierResultDataSourceDemo(true);
    classifier_result->setId("classifier result");
    classifier_test_result->setId("classifier test result");
    SPtr<MockVideoSource> videoline = new MockVideoSource( 2, 60, 100);
    SPtr<MockVideoSource> videopoint = new MockVideoSource( 1, 60, 100);
    SPtr<MockVideoSource> videotest = new MockVideoSource( 0, 60, 100);
    SPtr<VideoPlaylist> train_playlist = Factory::createVideoPlayer("train")->getPlaylist();
    SPtr<VideoPlaylist> test_playlist = Factory::createVideoPlayer("test")->getPlaylist();
    FrameRange videorange;

    videorange.firstFrame = 0;
    videorange.endFrame = 600;
    train_playlist->addSourceRange( videoline, videorange );
    train_playlist->addSourceRange( videopoint, videorange );
    test_playlist->addSourceRange( videotest, videorange );

    SPtr<IRepository> repository = Factory::getRepository(SESSION_FILE);
    SPtr<Session> session = repository->loadSession(); 

//Pipeline train mode
    session->addDataSource(classifier_result);
    session->setVideoPlaylist(train_playlist);
    playlist = session->getVideoPlaylist();

    SPtr<Pipeline> train_pipeline = Factory::createPipeline(TRAIN_PIPELINE);
    train_pipeline->setSession(session);   
    train_pipeline->connectAll();

    markup = classifier_result;
    train_pipeline->addNodeCallback("player", callSourceTrain, NULL);
    train_pipeline->run();

    //Generation test svm model
    SPtr<IRepository> svmRepository = Factory::getRepository("svm_model.xml");
    svmRepository->save(session->getClassifier(0));

//Pipeline test mode
    session->setVideoPlaylist(test_playlist);
    session->delDataSource(classifier_result);
    session->addDataSource(classifier_test_result);
    session->addPerformanceFilter(test_performance);
    playlist = session->getVideoPlaylist();

    SPtr<Pipeline> test_pipeline = Factory::createPipeline(TEST_PIPELINE);
    test_pipeline->setSession(session);
    test_pipeline->connectAll();

    markup = classifier_test_result;
    test_pipeline->addNodeCallback("player", &callSource, NULL);
    test_pipeline->addNodeCallback("classifier", &callClassifier, NULL);
    test_pipeline->run();
    test_performance->generateQualityReport("recognition_quality.txt");

    return 0;
}