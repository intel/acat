///////////////////////////////////////////////////////////////////////////
// <copyright file="performance_test.cpp" company="Intel Corporation">
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

#include "ivcp.hpp"
#include "performance_environment.hpp"

using namespace std;
using namespace cv;
using namespace ivcp;

SPtr<Performance> performance;

void simpleFeatureTest(FeatureType fType, RegionCreatorType rType, ClassifierType clType)
{
    SPtr<IRepository> repository = Factory::getRepository(TRAIN_SESSION);
    SPtr<Session> trainSession = repository->loadSession();
    SPtr<IRegionDataSource> trainGtRegions = trainSession->getDataSource("gt_regions");
    SPtr<VideoPlaylist> trainPlaylist = trainSession->getVideoPlaylist();
    SPtr<IClassifierResultDataSource> trainResults = trainSession->getDataSource("class_res");

    repository = Factory::getRepository(TEST_SESSION);
    SPtr<Session> testSession = repository->loadSession();
    SPtr<IRegionDataSource> testGtRegions = testSession->getDataSource("gt_regions");
    SPtr<VideoPlaylist> testPlaylist = testSession->getVideoPlaylist();
    SPtr<IClassifierResultDataSource> testResults = testSession->getDataSource("class_res");

    SPtr<IFeature> feature;
    switch (fType)
    {
    case NORMALFLOW:
        feature = Factory::createFeature(NORMALFLOW, "NormalflowFeature");
        break;
    case STLBP:
        feature = Factory::createFeature(STLBP, "STLBPFeature");
        break;
    case VLBP:
        feature = Factory::createVLBPFeature(VLBPFeature::Settings(3, 1, 1, 1, 4, 2, 2, 1), "VLBPFeature");
        break;
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Type of feature is not NORMALFLOW or STLBP or VLBP");
    }

    SPtr<IRegionCreator> trainRegion, testRegion;
    switch (rType)
    {
    case BBOX:
        trainRegion = Factory::createBBoxRegionCreator(Rect(0, 0, 100, 100), "BBoxRegionCreator");
        testRegion = trainRegion;
        break;
    case GROUNDTRUTH:
        trainRegion = Factory::createGTRegionCreator(trainGtRegions, 0, "GTRegionCreator");
        testRegion = Factory::createGTRegionCreator(testGtRegions, 0, "GTRegionCreator");
        trainRegion->setPlaylist(trainPlaylist);
        testRegion->setPlaylist(testPlaylist);
        break;
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unknown type of region creator");
    }

    string trainClName, testClName;
    switch (clType)
    {
    case ivcp::SVM:
        trainClName = "SVMClassifier_train";
        testClName = "SVMClassifier_test";
        break;
    default: 
        IVCP_EXCEPTION(IncorrectParamException, "Unknown type of classifier");
    }
    SPtr<IClassifier> classifier = Factory::createClassifier(clType, trainClName);

    SPtr<Session> session = Factory::createSession();
    trainPlaylist->seek(FIRST_SOURCE, FIRST_FRAME);
    session->setVideoPlaylist(trainPlaylist);
    session->addRegionCreator(trainRegion);
    session->addFeature(feature);    
    session->addClassifier(classifier);
    session->addDataSource(trainResults);
    session->addPerformanceFilter(performance);

    SPtr<Pipeline> pipeline = new Pipeline(session, TRAIN_PIPELINE);

    pipeline->connectAll();
    pipeline->run();
    pipeline.release();

    session->getClassifier(0)->setId(testClName);    
    session->delDataSource(trainResults);
    session->delRegionCreator(trainRegion);

    testPlaylist->seek(FIRST_SOURCE, FIRST_FRAME);
    session->setVideoPlaylist(testPlaylist);
    session->addRegionCreator(testRegion);
    session->addDataSource(testResults);

    pipeline = new Pipeline(session, TEST_PIPELINE);
    pipeline->connectAll();
    pipeline->run();
}

void aggregatorTest()
{
    SPtr<IRepository> repository = Factory::getRepository(TRAIN_SESSION);
    SPtr<Session> trainSession = repository->loadSession();
    SPtr<IRegionDataSource> trainGtRegions = trainSession->getDataSource("gt_regions");
    SPtr<VideoPlaylist> trainPlaylist = trainSession->getVideoPlaylist();
    SPtr<IClassifierResultDataSource> trainResults = trainSession->getDataSource("class_res");

    SPtr<IFeature> feature1 = Factory::createFeature(NORMALFLOW, "feature1");
    SPtr<IFeature> feature2 = Factory::createFeature(NORMALFLOW, "feature2");
    SPtr<AggregatorFeature> aggregator = Factory::createAggregatorFeature(2, 1, "AggregatorFeature");
    aggregator->addFeature(feature1);
    aggregator->addFeature(feature2);
    SPtr<IRegionCreator> region = Factory::createBBoxRegionCreator(Rect(0, 0, 100, 100), "BBoxRegionCreator");
    SPtr<IClassifier> classifier = Factory::createClassifier(ivcp::SVM, "SVMClassifier_train");

    SPtr<Session> session = Factory::createSession();
    trainPlaylist->seek(FIRST_SOURCE, FIRST_FRAME);
    session->setVideoPlaylist(trainPlaylist);
    session->addFeature(aggregator);
    session->addRegionCreator(region);
    session->addClassifier(classifier);
    session->addDataSource(trainResults);
    session->addPerformanceFilter(performance);

    SPtr<Pipeline> pipeline = new Pipeline(session, TRAIN_PIPELINE);
    pipeline->connectAll();
    pipeline->run();
}

void concatenatorTest()
{
    SPtr<IRepository> repository = Factory::getRepository(TRAIN_SESSION);
    SPtr<Session> trainSession = repository->loadSession();
    SPtr<IRegionDataSource> trainGtRegions = trainSession->getDataSource("gt_regions");
    SPtr<VideoPlaylist> trainPlaylist = trainSession->getVideoPlaylist();
    SPtr<IClassifierResultDataSource> trainResults = trainSession->getDataSource("class_res");

    SPtr<ConcatenatorFeature> concatenator = Factory::createConcatenatorFeature(2, "ConcatenatorFeature");
    concatenator->setFeature(Factory::createFeature());
    SPtr<IRegionCreator> region1 = Factory::createBBoxRegionCreator(Rect(0, 0, 100, 100), "bbox1");
    SPtr<IRegionCreator> region2 = Factory::createBBoxRegionCreator(Rect(50, 50, 100, 100), "bbox2");
    SPtr<IClassifier> classifier = Factory::createClassifier(ivcp::SVM, "SVMClassifier_train");

    SPtr<Session> session = Factory::createSession();
    trainPlaylist->seek(FIRST_SOURCE, FIRST_FRAME);
    session->setVideoPlaylist(trainPlaylist);
    session->addRegionCreator(region1);
    session->addRegionCreator(region2);
    session->addFeature(concatenator);
    session->addClassifier(classifier);
    session->addDataSource(trainResults);
    session->addPerformanceFilter(performance);

    SPtr<Pipeline> pipeline = new Pipeline(session, TRAIN_PIPELINE);
    pipeline->connectAll();
    pipeline->run();
}

int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    ivcpInit();
    performance = Factory::createPerformance(TEXT_REPORT, "performance");

    simpleFeatureTest(STLBP, BBOX, ivcp::SVM);
    simpleFeatureTest(NORMALFLOW, BBOX, ivcp::SVM);
    simpleFeatureTest(VLBP, BBOX, ivcp::SVM);
    aggregatorTest();
    concatenatorTest();

    performance->generatePerformanceReport("performance_report.txt");

    return 0;
}
