# Video Classification Pipeline

## Introduction
Video Classification Pipeline (IVCP) is a library for creating machine learning models for the classification of portions of video data and comparing different features and classification algorithms.
This User Guide gives a quick introduction of the library, its organization, main functionality and the best usage practices.
The User Guide is not intended to replace the Doxygen auto-generated documentation. The guide should be used to get a basic understanding of the library concepts. For detailed information of the function and their parameters, use the Doxygen documentation shipped with the library. 

## Dictionary
- Filter. Filter is the basic class for all entities of the library. Filter provides basic methods to set/get ids
- Filter ID. Each Filter instance is assigned a unique string ID. For example, if you have 2 Filters of the same type in a Session (for example, in case you have 2 Bounding Box Region Creators for the left and the right eyes in the Session, the Region Creators will have different IDs: for example, “left_eye” and “right_eye”). There are many functions in the Session and Pipeline classes that use the IDs to reference the underlying Filters
- Region. Region is a ROI (Region of Interest) in a video frame. Regions are detected by the Region Creator Filters. Regions could be accompanied by perspective transformation matrix. Examples of regions are the following: left eye, right eye, mouth, nose etc. In non-facial images example of regions could be: bounding box at (x=100, y = 100) with (width = 200, height = 200)
- Region sequence. Region sequence is a list of the same regions detected on a sequence of frames (for example, mouth region on 10 sequential frames)
- Serializer. In order to save Filters while using a Repository it is necessary to implement Serializers – special classes responsible for creating a XML representation of an object and vice versa: creating an object from XML representation
- Root Filter type. Each Filter implements a function IFilter::getRootType() which returns root type of the Filter – an enumeration which specifies a basic type of this Filter (whether it is a Video Player or Region Creator or Classifier or Performance etc.). The root type is used by Pipeline and by Repository (to select an appropriate Serializer)
- Filter type. Most of the Filters implement a function getType(). Return type of this function is dependent on the root type of a Filter – it is an enumeration which specifies exact type of a Filter (i.e. a Filter of root type “Feature” could be either NormalFlow, NormalFlowBlurred, MotionEstimation, STLBP or VLBP). This type is used mainly by the Repository (to select an appropriate Serializer)

The system consists of the following main components (also called Filters):
- Video Source. The component supports  2 video sources: a video file and a video camera
- Video Playlist. The Playlist component can be used to combine multiple video sources into a playlist. Each of the video sources added into a playlist has also an assigned interval (for example, one can specify to open frames from #40 to #50 from video file #1 and then open frames from #10 to #20 from video file #2 and then frame #77 from video file #2)
- Video Player. The Video Player is a Filter added into the Pipeline as source of Video frames. The Video Player uses Video Playlist internally. The Video Player component can return a link to its Video Playlist to share the playlist with different components (for example the classification result Data Sources uses the Video Playlist to synchronize classification results with the current frame being processed by the Pipeline)
- Region Creator. This component is responsible for creating regions of interest in a given image and providing image transformation necessary to get the optimal representation of the detected object (in case the given Filter is capable to detect suitable transformation). Detected regions are considered related to the transformed image in the case when Region Creator provides perspective image transformation matrix in addition to the region rectangle. Examples of such regions with regard to face images could be a face, a mouth, eyes, a nose etc.
- Feature. This component calculates features based on regions detected by the Region Creator component. By features we understand a short vector (a one-row cv::Mat) of numbers which can be used to describe the region. We will mainly use normalized flow and spatial-temporal local binary patches but the architecture assumes that later more features could be added
- Classifier. This component is responsible for creating classification models based on labeled video sequences in the train mode. The same component can be used for classification using previously created model in the test and real-time modes
- Performance. The Performance component is used for generating accuracy and performance reports. Examples of such reports include: confusion matrix, classification time.
- Session. This component is a storage for all users’ Filters. If you have a collection of Filters you want to use, just add them to a Session. That way you can save and load them at once (please read the “Object persistence - Repository” section for more details)
- Pipeline. The Pipeline component is responsible for connecting all the components together and for managing the message traffic between the components. The Pipelines works on top of a Session. Therefore it is not possible to add Filters to the Pipeline, you shall add all Filters to the Session and then create a Pipeline on top of the Session
- Data Source. The Data Source component provides interfaces for loading and storing of different settings and ground truth markings used by the components. There are 2 Data Sources in the library: a Region Data Source which is used together with GTRegionCreator class (the data source reads data from XML file and provides the data to the Region Creator) and a Classifier Result Data Source which provides ground truth classification data to the Classifier and Performance Filters (see the “Data Source” section for more details)
- Pipeline Node. This is a component in a Pipeline tree. The Pipeline Node is responsible for processing messages in the Pipeline. Each node contains an underlying Filter. Specific implementation of a Node knows which Filter type it contains and knows what to do with the messages (for example: to call feature calculation for a Feature Filter, call getRegion for a RegionCreator)
- Repository. The Repository class is responsible for object persistence. It can save Filters’ state to a file and read them back in the original state. The Repository is also used to read/save Data Sources

There are 2 special types of Features necessary to mention:
- Aggregator Feature. This feature can be used to construct advanced features based on simple primitives. The Aggregator Features takes a list of Features as input and aggregates their output into a single advanced feature. See more details in the “AggregatorFeature class” section
- Concatenator Feature. This feature as well as the Aggregator Feature can be used to construct advanced features based on simpler primitives. But instead of the Aggregator TheConcatenator Feature aggregates output of a single feature for a few sequential region sequences into a single feature. See more details in the “ConcatenatorFeature class” section

There are also 2 special types of Features (ClassifierFeature and SummatorFeature) related to the implementation of hierarchical machine learning described in the “Hierarchical Machine Learning” section.

## Functionality overview
The basic idea of the library is to provide simple and convenient tools for testing machine learning algorithms in the domain of dynamic texture classification. The library provides a set of tools that could be used as building blocks for creating a test environment for new ML (Machine Learning) functionality:
•	The library provides a set of simple interfaces that a new Filter can implement in order to be integrated into the machine learning pipeline and tested against different algorithms. So when testing new ML or CV (Computer Vision) functionality, the user can be focused on new functionality instead of developing test infrastructure from the scratch
•	The library provides interfaces for creating a classification pipeline based on implemented Filters (i.e. linking Filters in specific order, manipulating the pipeline graph)
•	The library provides a set of tools to create advanced features based on a  set of simpler features (see the “AggregatorFeature class” and “ConcatenatorFeature class” sections for more details)
•	The library provides a set of tools to create advanced features based on a set of other classifier results (see the “Hierarchical Machine Learning”, “ClassifierFeature class” and “SummatorFeature class” for more details)
•	State of all implemented Filters can be loaded/stored in xml files. The library provides a simple interface that user could implement in order to load/save newly added Filters
In addition to the test environment the library provides rich basic functionality which could be used for fast prototyping or creating simple applications:
•	Video Sources: support for capturing from web-cameras and video files
•	Video Player: combination of frames from different Video Sources into a single video sequence
•	Region Creators: bounding boxes (the ROI is set in the constructor anddoes not depend on image data), xml pre-marked data, facial regions recognized by, Dlib and RealSense (Dlib, RealSense Region Creators are available only under Windows)
•	Features: normal flow feature, normal flow blurred feature, motion estimation feature,  LBP-TOP features and VLBP features (refer to "Dynamic Texture Recognition Using Local Binary Patterns with an Application to Facial Expressions" by Guoying Zhao and Matti Pietikainen for more details), aggregation of simple features (read the “AggregatorFeature class” section for more details), concatenation of features for sequential frames (read the “Concatenator class” section for more details)
•	Classifiers: SVM, AdaBoost, and Logistic Regression classifiers
•	Performance: text report on recognition quality includes precision, recall, confusion matrix
## Project structure
The project source tree has the following folders:
•	data – files used by demos and test applications
•	Docs – all project documentation
•	include – API interface files
•	Markup – markup applications
•	modules – library source code
•	weka_converter – converter from Weka's saved classifiers to IVCP serialized classifiers
The library is divided into 4 modules (modules folder):
•	libivcp – main functionality
•	libpipeline – pipeline implementation
•	libinfra – infrastructure code (Repository implementation, serialization code)
•	tests – unit tests and demo applications
Pipeline is implemented in a separate library. So if user application uses only Filters from libivcp it is not required to link with libpipeline. 
All infrastructure code (XML processing etc.) is isolated in libinfra not to overload the domain libraries with auxiliary functionality, to make the domain code clean and easy to read and to eliminate unnecessary link to libinfra in case user does not use Repository.
Separate usage of the libraries is reserved for future. It is out of scope of the current User Guide. For the rets of the document,  the use of all 3 libraries (libivcp, libpipeline, libinfra), is assumed.
## Building instruction under Windows

### Dependencies
•	Install Microsoft Visual Studio 2015 with Update 3. Please note that this build will only work with Visual Studio 2015 – higher or lower versions are not currently supported.
◦	Go to the Visual Studio Downloads page and sign in to your account or create an account if you do not have it. This step is essential to download older versions of the Microsoft Visual Studio.
◦	 Go to the Download Older Visual Studio Software page https://my.visualstudio.com/Downloads?q=visual%20studio%202015&wt.mc_id=o~msft~vscom~older-downloads  
•	Expand the 2015 tab and click Download next to Visual Studio 2015 and other Products. 
 
•	You will be redirected to the Downloads page.  Select Visual Studio Professional 2015 with Update 3. Note Community version should also work, if you already have it installed.
◦	 
▪	An iso file en_visual_studio_professional_2015_with_update_3_x86_x64_dvd_8923272.iso will be downloaded to your Windows Downloads folder
▪	Right-click the iso file and select Mount
▪	Rightc-click on vs_professional.exe and select Run as an administrator
▪	During installation - Select “Custom” installation and click Next
▪	Select all the components with a checkmark from the following two screenshots
▪	  
	
◦	Proceed to install. Installation can take about 45 minutes to an hour
•	Install OpenCV 2.4.13.6 (http://opencv.org/downloads.html) (for e.g.,  C:/ACATVision/ExternalInstalls/OpenCV_2.4.13.6). Add  the environment variable “OPENCV_DIR” to your system and set it to the the build folder. For e.g., “C:/ACATVision/ExternalInstalls/opencv_2.4.13.6/opencv/build”
•	Install CMake 2.8.9 or higher (https://cmake.org/files/v3.4/cmake-3.4.3-win32-x86.exe)
 
•	 
•	Install Doxygen 1.8.2 (http://ftp.stack.nl/pub/users/dimitri/doxygen-1.8.2-setup.exe) 
•	Install Microsoft Visual Studio Express 2015
•	Reboot to make sure all environment variables are registered after installation of the software packages
•	Install Dlib C++ Library (http://sourceforge.net/projects/dclib/files/dlib/v18.14/) and shape_predictor_68_face_landmarks.dat file (http://sourceforge.net/projects/dclib/files/dlib/v18.10/shape_predictor_68_face_landmarks.dat.bz2)
- Install RealSense SDK (only for Microsoft Windows 8.1 OS 64-bit or higher, for more information see https://software.intel.com/en-us/intel-realsense-sdk «Required OS» section) https://software.intel.com/en-us/realsense-sdk-windows-eol (2016 R2 - Full SDK).  Drivers for the RealSense Camera, can be downloaded from https://software.intel.com/en-us/realsense-sdk-windows-eol (Intel RealSenseTM Camera F200 Firmware – Drivers)

### Building
•	Open CMake (cmake-gui)
•	In the text field “Where is the source code” enter full path to the source tree (for e.g., C: /ACATVision/VisionDLLCpp)
•	In the text field “Where to build the binaries” enter full path to the build tree (it is recommended to have the source folder and the build folder separate (for e.g., C:/ ACATVision/VisionDLLBuilds/MyBuild)
•	Press “Configure” button
•	Choose Visual Studio version which will be used for compilation (Visual Studio 14 Win64 is the recommended option) and make selections as shown below -
 
 

 
•	The configuration will produce an error because some paths need to be specified
•	In the Cmake, make sure the “OpenCV_DIR” folder detected by cmake is correct (for e.g., C:/ACATVision/ExternalInstalls/opencv_2.4.13.6/opencv/build. In case autodetection found incorrect OpenCV folder enter OpenCV_DIR folder manually, remove all other variables having “opencv” substring (please use the “search” field to Filter the variables containing the substring) and press “Configure” button
•	Make sure the path to the folder with OpenCV dlls is added to your system PATH environment variable (example of the path is C:/ACATVision/ExternalInstalls /OpenCV_2.4.13.6/opencv/build/x86/vc14/bin or OPENCV_DIR/x86/vc14/bin (assuming you have already defined the environment variable “OPENCV_DIR” as mentioned above during opencv installation. 
 

•	

•	If you want to use dlib face and landmarks detection select “WITH_DLIB” option and enter path to Dlib folder in DLIB_PATH field (for e.g.,  C:/ ACATVision/ExternalInstalls/dlib-18.14/dlib-18.14/) and enter path to file with landmarks in DLIB_FACE_LANDMARKS_FILE field (for e.g., C:/ACATVision/ExternalInstalls/shape_predictor_68_face_landmarks.dat). After that uncheck DLIB_LINK_WITH_LIBJPEG, DLIB_LINK_WITH_LIBPNG, DLIB_USE_BLAS and DLIB_USE_LAPACK options.
 
•	Finally, you can make dlib faster (select only one of the next described options). The face detector is fastest when compiled with at least SSE2 instructions enabled. So if you are using a PC with an Intel or AMD CPU then you should enable at least SSE2 instructions (set checked USE_SSE2_INSTRUCTIONS). Note that AVX is faster but requires a CPU from at least 2011 year (set checked USE_AVX_INSTRUCTIONS).  SSE4 is the fastest and is supported by most current machines (set checked USE_SSE4_INSTRUCTIONS)
•	If you want to use RealSense camera or/and regions select “WITH_REALSENSE” option. If CMake can't fine RealSense SDK by itself, enter path to include RealSense folder to RealSense_INCLUDE_DIR field (example of the path is C:/Program Files (x86)/Intel/RSSDK/include) and path to libpxc.lib file to RealSense_LIBRARY field (example of the path is C:/Program Files (x86)/Intel/RSSDK/lib/x64/libpxc.lib)
 
•	If you want to build converter from Weka model files to ivcp serialized classifiers select the “WITH_WEKA_CONVERTER” and “BUILD_SHARED_LIBS” option. Please note that Java JDK and Apache Ant must be installed on your PC (JDK can be downloaded from http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html (jdk-8u181-windows-x64.exe), install Apache Ant according to this instruction: http://ant.apache.org/manual/install.html; set JAVA_HOME and ANT_DIR environment variables are required)[SS1.1]
•	Specifiy the installation folder in the entry “INSTALL_PREFIX”, For e.g., C:/ACATVision/VisionDLLInstall
•	Press “Generate” button
•	In case all settings are correct CMake will  create a Visual Studio solution at the folder set in the “Where to build the binaries” text field (for e.g., C:/ ACATVision/VisionDLLBuilds/MyBuild) 
•	Open generated ivcp solution (ivcp.sln) in Visual Studio
•	Select the “Build”->”Batch Build” menu item  
•	Select Projects ALL_BUILD/ Debug Configuration, ALL_BUILD/Release Configuration, INSTALL/Release Configurations as shown below 
 
•	Select “Rebuild” 
•	Once Rebuild is successful, the bin folder under the installation folder (for e.g., C:/ACATVision/VisionDLLInstall/bin will contain all the test exes and the libraries


## Architectural notes
This section covers the basic ideas under IVCP design. For more details on different Filters implemented in the library you shall look in the respective sections of this guide.
The core functionality of the library is implemented as a set of Filters of different types: Data Source, Video Player, Region Creator, Feature, Classifier, Performance and a few others. Each type has an interface (i.e. an abstract class in terms of C++). So if you need to implement a new Filter you can easily derive from an appropriate interface (see the “Adding new Filters” section for more details). 
There are 2 more Filters that you need to use in your code: a Session and a Pipeline. Session is just a set of Filters grouped by their type. Pipeline contains processing and message transferring logic. 
So in order to use the library you need to perform the following steps:
•	Create Filters (see the “Objects creation” section for more details). You will need at least a Video Player, a Regions Creator, a Feature, a Classifier and a Performance Filters
•	Add all the Filters to a Session (see the “Session” section for more details)
•	Create a Pipeline in desired mode (i.e. train, test or real-time modes) based on the Session (see the “Pipeline” section for more details)
You can save your Pipeline to a file by using a Repository (see the “Object persistence – Repository” section for more details). The rest of the Filters (including a Session) can be saved to a Repository as well. When a Filter is saved to a file all embedded Filters are saved too (i.e. when you save a Session all the Filters added to the session are saved too). So when the Filter is loaded from the Repository it shall be in the same state as before the saving (for example if you save a trained Classifier Filter it will be loaded up in the trained state).
### Library initialization
The first function that must be called in all applications that utilize the IVCP library is ivcpInit. This function initializes internal data structures, configures repository and injects necessary dependencies from libinfra to libivcp and libpipeline libraries.
Declaration of ivcpInit:

```cpp
namespace ivcp
{
IVCP_EXPORT void ivcpInit();
}
```

All code of IVCP is implemented in ivcp C++ namespace. All API interfaces are declared in ivcp.hpp file. So the simplest application that uses IVCP is the following:

```cpp
#include "ivcp.hpp"

int main(int argc, char **argv)
{
    ivcp::ivcpInit();
    return 0;
}
```

### Filters
Filter class provides a basic interface for all entities of IVCP:

```cpp
namespace ivcp
{
class IVCP_EXPORT Filter: public IFilter
{
public:
    Filter() {};
    virtual ~Filter() {};

    virtual void setId(std::string id);
    virtual std::string getId();

protected:
    std::string FilterID;
};
}
```

The Filter class defines interfaces for settings/getting a unique ID assigned to all Filters in IVCP. The Filter IDs are used for identification of objects in Session, Pipeline and Repository interfaces. For example Pipeline could have 2 Normal Flow Features objects. In order to differentiate one from another the unique IDs shall be assigned to each object.
Type of the IDs is std::string, no special restrictions are set on content of the Filter IDs.
### Data Source
Data Source Filters are used as ground truth data provided for the region creator and the classifier Filters. The Data Source can be considered as helper classes for reading and modifying  XML files that contain manually marked data. The Data Source Filters hide complexity of parsing the XML files and encapsulate the XML formatting used internally. Also, it is possible to switch to a different format of data sources (for example, plain text, relational database etc.) by implementing specific Data Source Filters without modifying user code.
 There are 2 types of data source Filters in IVCP: 
•	Region Data Source. Provides information of regions for each frame of the Video Playlist. This information is used by the GTRegionCreator Filter
•	Classifier Result Data Source. For each frame in a Video Playlist the Classifier Result Data Source provides correct class identifier. This information is used by the Classifier Filter during training and by the Performance Filter during testing.
Let’s consider each of the two Data Sources in details.
#### Region Data Source
The Region Data Source Filter shall be derived from IRegionDataSource interface and implement the following methods for manipulation regions:
•	getRegion – for given frame get a region rectangle of a specified region id (examples of such id include: REGION_LEYE for a left eye, REGION_NOSE for a nose etc)
•	getRegionsByFrame  - for a given frame get all regions and their class ids
•	addRegion – add a new region and its class
•	delRegion – delete region of a specific class from a frame
•	getTransformation – get 3x3 homography transformation matrix related to the frame
•	setTransformation – set 3x3 homography transformation matrix related to the frame

A Region Data Source object can be created with using Factory::createDataSource(GROUNDTRUTH_REGIONS_FILTER) and Factory::createRegionDataSource method. The Factory will return an empty Region Data Source which shall be filled with Regions in your code. Later this Region Data Source can be saved to a file by using IRepository::save method. 
If you already have a file with saved Data Source (created by ivcp_region_markup application or saved with using IRepository::save method), you can load it with using IRepository::loadRegionDataSource or IRepository::loadDataSource methods. 
The most common way of using the Region Data Source Filter is to use it with GTRegionCreator Filter. GTRegionCreator Filter needs a Region Data Source assigned in order to extract regions in the Pipeline. You can assign a Region Data Source to GTRegionCreator by using GTRegionCreator::setParams method or during constructing the Region Creator by setting dataSourceFilepath parameters of the Factory::createGTRegionCreator method. 
Please note that GTRegionCreator shall have a Region Data Source Filter assigned in order to extract regions in the Pipeline. You shall assign a Data Source before starting Pipeline.
#### Classifier Result Data Source
The Classifier Result Data Source Filter shall be derived from IClassifierResultDataSource and implement the following methods for manipulating class marking data:
•	getClassifyResult – for given frame get class id
•	isClassified – check whether or not given frame is classified
•	addClassifyResult – add (or update) classification result of given frame
•	removeClassifyResult – delete classification result for given frame
A Classifier Result Data Source can be created with using Factory::createDataSource(CLASSIFIER_RESULTS_FILTER) and Factory::createClassifierResultDataSource methods. The Factory will return an empty Data Source which shall be filled in your code. Later this Data Source can be saved to a file with using IRepository::save method. If you already have a file with saved Data Source (created by ivcp_markup application or saved with using IRepository::save method) you can load it with using IRepository::loadClassifierResultDataSource or IRepository::loadDataSource methods.
A Classifier Result Data Source is used by a Classifier Filter in a Pipeline during training (i.e. when Classifier receives a feature vector it also asks the assigned Data Source for a classification result for the current frame). 
A Classifier Result Data Source is also used by a Performance Filter during test mode of a Pipeline (i.e. when a Performance Filter receives classification result from a Classifier Filter, the Performance Filter needs also a ground truth classification result from a Data Source Filter in order to check classification correctness and update statistics).
Please note that you shall add a Classifier Result Data Source Filter to a Session and then connect it (Pipeline::addLink) either to a Classifier Filter (in train mode) or to a Performance Filter (in test mode). 
### Video Player
Video Player is used as a source of Video frames in the Pipeline. Video Player can be used to combine multiple Video Sources (i.e. video files or/and cameras) into a sequence fed to the Pipeline. The typical usage of the Video Player is the following:

```cpp
//Create video player 
SPtr<VideoPlayer > videoPlayer = Factory::createVideoPlayer();
//Get a pointer of the video playlist assigned to the player 
SPtr<VideoPlaylist > videoPlaylist = videoPlayer->getPlaylist();

//Create train playlist – update the playlist
stringstream sourceFilename;
sourceFilename << UCLA1_FOLDER << "ucladb_wfalls-d-near_1s1.avi";
SPtr<IVideoSource > source1 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source1, FrameRange(0, 75));

sourceFilename << UCLA1_FOLDER << "ucladb_wfalls-d-near_1s2.avi";
SPtr<IVideoSource > source2 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source2, FrameRange(10, 75));
videoPlaylist->addSourceRange(source2, FrameRange(0, 9));


//Now the Video playlist has 2 video sources and 3 ranges:
//source1, from frame #0 to #75
//source2, from frame #10 to #75
//source2, from frame #0 to #9

//The videoPlayer with videoPlaylist can be assigned to a Session etc...

```

The VideoPlayer class is a container for VideoPlaylist. VideoPlayer is added to a Session and is used as a Video frames source in the Pipeline. But if the user wants to add more video sources they  need to get a pointer to the VideoPlaylist used by the player and update the playlist. The playlist could be shared between different objects (VideoPlayer and ClassifierResultDataSource – this way the ClassifierResultDataSource can be in sync with the current frame played by the VideoPlayer object).
VideoPlaylist uses IVideoSource interface. Currently there are 3 implementations of the interface (please use the Factory class to create a Video Source):
•	VideofileVideoSource. This class can read Video files. In order to create a VideofileVideoSource Filter, you shall call the following Factory method with the path to your video file:

```cpp
static SPtr<VideofileVideoSource> Factory::createVideofileVideoSource(std::string path, std::string id = "", bool multipleVideoFiles = false, bool is12Bit = false) throw (IncorrectParamException);
```

•	CameraVideoSource. This class careads from web-cameras. In order to create a CameraVideoSource Filter, you shall call the following Factory method with the index of your camera (for example, CV_CAP_ANY):

```cpp
static SPtr<CameraVideoSource> 
Factory::createCameraVideoSource(int cameraIndex, std::string id = "") throw (IncorrectParamException);
```

•	RealSenseVideoSourse. This class reads RealSense camera. In order to create a RealSenseVideoSource Filter you shall call the following Factory method with index of your RealSense camera:

```cpp
static SPtr<RealSenseVideoSource> createRealSenseVideoSource(int cameraIndex = 0, std::string id = "") throw (IncorrectParamException);
```

VideoSource can output frames in one of the following formats:
•	CV_BGR format (i.e. 3 8-bit channels in BGR order: blue, gray, red) – for color images;
•	CV_8UC1 – for black and white images from standard cameras and video files;
•	CV_16UC1 – for 12 bit black and white images from cameras such as the Ximea (if the camera supports the extended 12 bit range). In this case all pixels will be represented by an unsigned short value in the range from 0 to 4095. 
User can save output from a VideoSource filter to a file by using the GUIFramework (see the “GUI Framework” section for more details). In case of 12 bit video the resulting file will contain 24 bits per pixel (8 bits per color channel) but only first 12 bits of 24 will be used. Please note that when you save 12 bit video you shall not use encoders (i.e. save video in uncompressed format), otherwise data will be corrupted (use CV_FOURCC ('D','I','B',' ') codec). 
You can read the 12 bit data back with using VideofileVideoSource. Please use VideofileVideoSource::set12BitSource(bool is12Bit) method to switch between standard video files and video files containing 12 bit data. If you call VideofileVideoSource::set12BitSource(false) the video source will output data in CV_8UC3 format.  If you call VideofileVideoSource::set12BitSource(true) the video source will output data in CV_16UC1 format.
In order to use several files as one VideoSource Filter user can provide an additional flag multipleVideoFiles to Factory::createVideofileVideoSource(...). Being in the multiple files mode with “<filename>.<extension>” set as an original (ie the first) file VideofileVideoSource Filter uses consequently all video files with names “<filename>N.<extension>” where N is a positive integer number.
### Region Creators
The Region Creator Filters are used to extract special regions of interest from images. In case of facial emotion recognition such regions could be a mouth, eyes, nose etc. In case of general dynamic texture recognition an image could be split into uniform grid (overlapping or non-overlapping). 
In addition to the region rectangle, Region Creator Filter can provide perspective image transformation matrix (3x3 of doubles) that should be applied to the image prior to region extraction to get the optimal representation of the region of interest. Please note that in case the transformation matrix is provided region description is considered related to the transformed image representation. 
Region of interest description and a transformation matrix are returned as a structure:

```cpp
struct RegionDescription
{
public:
    cv::Rect regionFrame;
    cv::Mat regionRotation;
};
```
In case a Region Creator doesn’t provide the transformation matrix regionRotation is empty. Currently the library supports  4 Region Creators:
•	BBoxRegionCreator. This Filter extracts regions of equal size and position set in constructor:

```cpp
SPtr<IRegionCreator> regionCreator = Factory::createBBoxRegionCreator(cv::Rect(0, 0, 100, 100));
```

regionCreator will return Rect(0, 0, 100, 100) as regionFrame for all input images. This region creator doesn’t provide transformation matrixes.
•	GTRegionCreator. This Filter uses IRegionDataSource with labeled regions (please read the “Marking applications” section for more info on how to create marking data). In case the Data Source contains markings for multiple regions (left and right eyes for example) the Region Creator needs to know which class of regions it corresponds to. So the Factory method takes 3 parameters: IRegionDataSource, ClassType and id:

```cpp
static SPtr<IRegionCreator> Factory::createGTRegionCreator(
SPtr<IRegionDataSource> source, ClassType regClass, std::string id = "") throw (IncorrectParamException);
```
This region creator could provide homography image transformation matrixes in case the used region Data Source has such information for the processed frame. In this case provided region description is considered related to the transformed image. Otherwise the image transformation matrix is empty and region description is related to the original image. 
•	DlibRegionCreator. This Region Creator uses the dlib face detector to get regions of face, mouth and other facial regions. The regions shall be set as a part of configuration structure used by the DlibRegionCreator constructor. This region detector also can provide a rotation image transformation matrix intended to normalize representation of the face detected on the image. In order to enable this functionality the normalize flag of the configuration structure should be set to true. In case the face normalization is enabled region detection is performed on normalized images. If face normalization is disabled this region creator returns transformation matrix and processes unmodified image

```cpp
struct DlibRegionCreator::Settings
{
public:
    RegionType regionToSearch;
    float regionWidthExpansionMultiplier;
    float regionHeightExpansionMultiplier;
    bool normalize;
    float stabThreshold;
    unsigned int stabFrames;
    int regionOffsetX;
    int regionOffsetY;
    int normFrames;
    int noFaceFrames;
    bool noFaceClockwiseRotation;
    bool glassesFixEnabled;
    bool useTemplateRegions;
    float glassesFixStrength;
    bool useAutoGlassesSearch;
    bool cutBackground;
    double resizeDownscale;
};
static SPtr<DlibRegionCreator> createDlibRegionCreator(DlibRegionCreator::Settings settings = DlibRegionCreator::Settings(), std::string id = "", SPtr<IDlib> dlibHandler = NULL) throw (IncorrectParamException);
```
To use Dlib regions firstly you need to create Dlib handler using the following function:

```cpp
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
        double resizeDownscale = 1.0) throw (IncorrectParamException, IOException);
    ```
Dlib has the same workarounds as FaceAPI: face normalization, regions stabilization, auto glasses search workaround, template based regions. Also it has cutBackground as ASM.
Some of emotion classification algorithms require facial region positions to be stable in sequential frames. For example we can detect eye blink by analyzing amount of motion in eye region and using a simple threshold technic. But this will work if only the eye region does not move itself. In order to eliminate jittering in FaceAPI output IVCP provides an option to stabilize facial regions. If you want to enable this option you shall create FaceAPI handler manually with using the following function:

```cpp
static SPtr<IFaceAPI> createFaceAPI(int pollTimeout = 1, 
    bool realtime = false, 
    float framerate = 30.0f, 
    float stabThreshold = 0.0,
    unsigned int stabFrames = 10, 
    int normFrames = -1, 
    int noFaceFrames = -1, 
    bool noFaceClockwiseRotation = false, 
    bool glassesFixEnabled = true,
    bool useFaceAPITemplateRegions = false,
    float glassesFixStrength = 0.0f,
    TypeOfTransform transformType = PERSPECTIVE, 
    bool useAutoGlassesSearch = false) throw (IncorrectParamException, IOException);
```
Please set the following parameters in order to enable the stabilization option:
◦	stabThreshold - movement threshold to start updating regions on face movement
◦	stabFrames - number of frames to check for face movement during face stabilization
If the regions include any part of glasses, it also may cause problem for a gesture detection algorithm. Glasses fix option is used to avoid it. It is enabled by default, but you can deactivate it by setting the “glassesFixEnabled” flag to false. Strength of the “glasses fix” parameter can be tuned with using glassesFixStrength parameter. This coefficient should be from 0.0 to 1.0..
Also you can choose type of transformation for normalization (PERSPECTIVE or ROTATION). Perspective transformation has 8 degrees of freedom (including 3 angles: roll, pitch, yaw). ROTATION is a similarity transformation with 4 degrees of freedom (4 parameters: scale, roll angle, dx, dy); it's calculated using canonical landmarks (read about them in «Template based regions” section).
Also you can use template regions option to make the eyebrow regions more stable. If you want to activate it, set the “useFaceAPITemplateRegions” flag to true, but please note that glasses fix and template regions options can’t be enabled at the same time.
Sometimes eyebrow and cheek regions move outside of the face areas, including background. To avoid it, set cutBackground parameter to true. FaceDetector return some landmarks on the face border, using them regions are just resized. Face border is recalculated each time stabThreshold is reached.

To decrease face detection time change resizeDownscale parameter. If you set it lower than 1.0 (for example 0.25), every frame image will be rescale to lower size and face searching will be faster.
•	RealSenseRegionCreator. This Region Creator uses Intel RealSense face location detection and landmark detection to get regions of face, mouth and other facial regions. The regions shall be set as a part of configuration structure used by RealSenseRegionCreator constructor. This region detector also can provide a rotation image matrix intended to normalize representation of the face detected on the image. In order to enable this functionality the normalize flag of the configuration structure should be set to true. In case the face normalization is enabled region detection is performed on normalized images. If face normalization is disabled this region creator returns transformation matrix and processes unmodified image.
Note that RealSense regions can be used only with RealSense video source. And with RealSense video source you can create any provided type of regions.

```cpp
struct RealSenseRegionCreator::Settings
{
public:
    RegionType typeToSearch;
    float regionExpansionWScale;
    float regionExpansionHScale;
    bool normalize;
    float stabThreshold;
    unsigned int stabFrames;
    int regionOffsetX;
    int regionOffsetY;
    bool glassesFixEnabled;
    bool useTemplateRegions;
    float glassesFixStrength;
    bool useAutoGlassesSearch;
    bool cutBackground;
};

static Sptr<RealSenseRegionCreator> createRealSenseRegionCreator(RealSenseRegionCreator::Settings settings = RealSenseRegionCreator::Settings(), std::string id = "", SPtr<IRealSense> rsHandler = NULL) throw (IncorrectParamException);
```
To use RealSense regions firstly you need to create RealSense handler using the following function:

```cpp
static SPtr<IRealSense> createRealSense(float stabThreshold = 0.0,
        unsigned int stabFrames = 10,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false) throw (IncorrectParamException, IOException);
    ```

RealSense has the same workarounds as DlibI: face normalization, regions stabilization, auto glasses search workaround, template based regions and cutBackground.
### FaceAPI workarounds
Since FaceAPI landmarks are not always perfect we implemented a series of “workarounds” which try to improve quality of face tracking and gesture classification. The workarounds are not guaranteed to work on all videos so users are advised to test them in their target environment. 
Some sections below may repeat description from other sections of the User Guide. We just wanted to gather them all together.
#### Face normalization
In case detected face on the given frames is rotated, algorithm finds homography image transformation matrix intended to normalize representation of the face detected on the image. In order to enable this functionality the “normalize” flag of the FaceAPIRegionCreator::Settings configuration structure for each region should be set to true. In case face normalization is enabled region detection is performed on normalized images. Effect made by this functionality you can see on the pictures below. 
Without normalization:
  
With normalization:
 
#### Regions stabilization
Stabilization option uses following parameters of the FaceAPI class:
•	stabThreshold - movement threshold to start updating regions on face movement
•	stabFrames - number of frames to check for face movement during face stabilization
The option works as follows. It analyzes previous stabFrames number of frames and calculates average position of the nose region on those frames. Then the library compares nose position on the current frame with the average position. In case difference is greater than stabThreshold all facial regions are updated. Otherwise the regions from the previous frame are used. Please note that when FaceAPI handler calculates average of the previous stabFrames number of frames it uses raw unfiltered FaceAPI output (i.e. not the stabilized regions).
The average region is calculated by averaging positions of the 4 region points independently (as result the average region is not obligatory a rectangle). Difference between the average region and region on the current frame is calculated as a sum of square distances between the region corners. Therefore the stabThreshold parameter is not proportional to number of pixels between the regions, in average it is proportional to 4 * (number of pixels) ^ 2. So if you want to ignore 5 pixels of nose movements the stabThreshold parameter shall be set to 4*5^2=4*25=100.
#### Glasses fix
This option is enabled by default, but you can disable it by setting the “glassesFixEnabled” flag in the createFaceAPI method parameters to false. If it is active then eyebrow regions will be moved for every frame along the axis of symmetry of the nose (the red arrow on the image below) by a specially selected constant. It is performed to avoid intersection of parts of glasses and eyebrow regions. Strength of the move can be controlled by using the glassesFixStrength parameter.
Please note that the “glasses fix” does not take into account shape of the face. It simply moves the eyebrow region a little bit upper. Constant of this shift was chosen based on the set of our target videos and may work imprecise on your target videos. 
 
#### Auto glasses search workaround
Sometimes, when users wear glasses, eyebrow regions are detected incorrectly, and this workaround helps in such a case. In order to use it set useAutoGlassesSearch to true. In this case on the first frame (where FaceAPI detects landmarks with needed confidence) position of glasses is calculated. At first, the image is normalized. Then we calculate a histogram of the face region and get lightnessThreshold (with an assumption that 50% of the brightest pixels are on skin). After that we paint out all pixels I on the image where lightness(I) >  lightnessThreshold. For the resulting image we calculate a gradient image and search for glasses position (using glasses template and exhaustive search or downhill simplex method). Glasses position is saved as a ratio:
length between center of eyes and glasses frame / eyes width.
And on each next frame, with calculated current position of center of eyes glasses frame is estimated based on the ratio and eyebrow regions are moved so that they are above glasses.
#### Template based regions
Set the “useFaceAPITemplateRegions” flag of the Factory::createFaceAPI  method to true in order to activate this option. Option uses a-priory information about face proportions and the subset of canonical FaceAPI landmarks. The canonical landmarks are the most reliable landmarks, which FaceAPI detects correctly and stably in most cases. Now we use the following landmarks (for more details on landmarks available in FaceAPI see the “Face Landmark Standard” section of the FaceTrackingAPI document):
•	eye corners (points 0, 1, 2 and 3)
•	mouth corners (points 4 and 5)
•		tip of the nose (points 6, 7, 500 and 501)
The library calculates similarity transformation which needs to be applied to the template in order to best match it with the canonical regions on the normalized face.
Please note that the options can be enabled only when FaceAPI normalization option is enabled as well. 
Also, please note that in case the “useFaceAPITemplateRegions” option is enabled the “Glasses fix” workaround is not applied. 
#### FaceAPI is unable to detect a face
In case FaceAPI can’t detect a face on the given frames the following algorithm is used. After certain number of frames with no face detection the pipeline processing is stopped and it tries to rotate image until face is detected or until it make a full circle (360 degrees).  In case no face is detected after the rotation the algorithm performs another iteration (i.e. it tries to rotate the image after the same number of frames with no face detection). The following parameters of the FaceAPI class are used by this algorithm:
•	noFaceFrames -  In case this parameter is -1 the workaround is not applied. Otherwise it sets number of frames with no face detection after which image should be rotated in order to try fixing the issue
•	noFaceClockwiseRotation -  If true the image is rotated clockwise, otherwise it is rotated counter-clockwise
#### Region offsets
In order to calibrate placement and size of FaceAPI regions use the following parameters of the FaceAPIRegionCreator::Settings configuration structure of some region:
•		regionExpansionWScale -  region width scaling factor
•		regionExpansionHScale -  region height scaling factor
•		regionOffsetX -  region offset along X axis
•		regionOffsetY -  region offset along Y axis
### Features
Feature is a Filter responsible for generating a compact representation of a region sequence (for details on advanced features that take not just a single sequence but multiple sequences please read the “Advanced multi-sequential features” section). This output is a 1-row matrix of type CV_32FC1 (1 channel 32-bit float numbers):

```cpp
cv::Mat Feature::getFeature(const std::vector<RegionSequence> &regSeqs) throw (IncorrectParamException) = 0;
```

Size of a feature is feature-dependent. For example, the size of a normal flow feature is 1x1 matrix while the size of SLBP feature depends on feature parameters.
There are 4 basic features implemented in the library are:
•	NormalFlowFeature. This is a measure of the quantity of motion that is computed at every pixel as follows:
 
where  is the spatio-temporal gradient at the pixel. To measure the motion energy in a given region we take the sum of the norm of the normal flow for all pixels. If threshold parameter is set then this sum is calculated only for the pixels where the norm of the normal flow is more than threshold. The normal flow feature can be created with the default Factory method and with a special Factory method:

```cpp
static SPtr<IFeature> Factory::createFeature(FeatureType type = NORMALFLOW, std::string id = "") throw (IncorrectParamException);

static SPtr<NormalFlowFeature> createNormalFlowFeature(float threshold = 0.f, std::string id = "");
```

•	NormalFlowBlurredFeature. This feature is similar to NormalFlowFeature, but computed for blurred image according to the pipeline illustrated below:
 
The normal flow blurred feature can be created with the default Factory method and with a special Factory method:
```cpp
static SPtr<IFeature> createFeature(FeatureType type = NORMALFLOW, std::string id = "") throw (IncorrectParamException);

static SPtr<NormalFlowBlurredFeature> createNormalFlowBlurredFeature(float threshold = 0.f, NFBFType outputType = NFBF_ENERGY, bool robustFlag = false, std::string id = "");
```

Variable 'option' gives you the ability to choose output of the feature:  
1.	NFBF_ENERGY – energy value (1 feature/region):
 
2.	NFBF_DX_DY – dx and dy values (2 features/region), where dx and dy are calculated from the following system of linear equations (one equation per pixel):
`fx * dx + fy * dy = -ft`
…
3.	NFBF_ENERGY_DX_DY – energy, dx and dy values (3 features/region in a single feature vector)
The system of linear equations used to find dx and dy could be solved in different ways. In order to solve it using the robust M-estimator, set robustFlag to true. Otherwise, it will be computed by the least squared solver using method solve() from OpenCV. 
•	STLBPFeature. This feature is an implementation of the LBP-TOP features from "Dynamic Texture Recognition Using Local Binary Patterns with an Application to Facial Expressions" by Guoying Zhao and Matti Pietikainen. There are 2 implemented modifications of LBP-TOP features: 
◦	Normalized features. In case STLBPFeature::Settings::normalizeFeature is true the feature will be normalized by volume of the (X,Y,T) space so that feature is not dependent on image size and temporal interval
◦	Uniform patterns. In case STLBPFeature::Settings::useUniformPatterns is true the feature will use uniform patterns as it is described in the paper. A pattern is considered uniform if it contains at most two bitwise transitions from 0 to 1 or vice versa when the bit pattern is considered circular. When using uniform patterns, all non-uniform LBP patterns are stored in a single bin in the histogram computation. This makes the length of the feature vector much shorter.  Please note that uniformity of the feature is considered on a per plane basis.
For example let's consider the feature with 4 neighbors on each plane. During processing of XY plane we could get following LBP codes for different input data points: 0101, 0110, 0111, 1010. In this case, the values 0110 and 0111 would be considered uniform and counted separately while values 0101 and 1010 would be considered non-uniform and counted as a single histogram bin. The same evaluation would be performed for planes XT and YT. The overall result of the feature is a concatenation of histograms evaluated for each of the planes.
•	VLBPFeature. This feature is an implementation of VLBP features from "Dynamic Texture Recognition Using Local Binary Patterns with an Application to Facial Expressions" by Guoying Zhao and Matti Pietikainen. There are 3 implemented modifications of the VLBP features: 
◦	Normalized features. In case VLBPFeature::Settings::normalizeFeature is true the feature will be normalized by volume of the (X,Y,T) space so that feature is not dependent on image size and temporal interval
◦	Rotate invariant features. In case VLBPFeature::Settings::useRotateInvariant is true the feature will use rotation invariant LBP codes as it is described in the paper. Please note that in this case all three evaluated XY planes of the input data are rotated simultaneously while we search for the minimum of the overall feature value.
For example let's consider the VLBP feature with 2 neighbors. During processing of the input data we could get the following VLBP codes: 10110010 and 11001010. Here the first and last bits denote differences for central points of planes while internal pairs 01, 10, 01 and 10, 01, 01 denotes differences for each plane of the first and second value respectively. In this case the value 10110010 is rotation invariant since its rotation (since there are only 2 neighbors there is only one possible rotation to check) is 11001100 which is greater. The value 11001010 isn't rotation invariant since its rotation is 10110100 which is lesser (therefore the value 11001010 will sum up to the rotation non-invariant bin). 
◦	Uniform patterns. In case VLBPFeature::Settings::useUniformPatterns is true the feature will use uniform patterns as it is described in the paper. A pattern is considered uniform if it contains at most two bitwise transitions from 0 to 1 or vice versa when the bit pattern is considered circular. When using uniform patterns, all non-uniform LBP patterns are stored in a single bin in the histogram computation. This makes the length of the feature vector much shorter
Please note that uniformity is considered as property of the whole LBP feature representation unrelated to separation of considered temporal planes. Also in the case uniformity is used simultaneously with rotation invariance all uniform features would be considered as their rotation invariant alternatives evaluated as a whole LBP feature representation unrelated to separation of considered temporal planes.  So in case the rotation invariant uniform feature is used the VLBP code is checked for per plane rotation invariance and then per plane rotation invariant alternative of the code is checked as the whole for uniformity and rotation invariance.
For example let's consider the feature with 2 neighbors. Input data processing could yield the following values:
◦	10011001 – This value is not uniform. Regardless of each plane LBP code is uniform the overall VLBP code is not. This value should be counted in common non-uniform bin during histogram evaluation
◦	10001111 – This value is uniform. This value should be considered as a separate uniform value if we use uniform feature. In case we use rotation invariant uniform feature this value should be also checked for rotation variant as a whole value and therefore should be considered as 00011111 code
◦	11110001 – This value is uniform. However if we use rotation invariant uniform feature this value could not be checked for uniformity since it's not per plane rotation invariant. Therefore this code should be changed to per plane rotation invariant alternative 11101001 which isn't uniform prior to uniformity check. So this value should be counted as non-uniform histogram bin
◦	00111111 – This value is uniform. This value is also per plane rotation invariant as well as rotation invariant as a whole. So this value should be counted as dedicated histogram bin
•	MotionEstimationFeature.  This is a base class for features that estimates the presence of motion. Features motion estimation's type takes a vector of RegionSequence on input (multi-sequential features), and produce 1x<region sequences' number> matrix. All frames in each region sequence need to be the same size, so feature operates on intersection of region sequence's frames. Each intersection is passed in method estimate(const vector<Mat> &regions), that returns estimation of motion presence for corresponding region sequence. You need to implement the estimate method with own algorithm for motion estimation in derived class.  The output of this feature is 1x<region sequences' number> matrix. There is one implementation of MotionEstimationFeature currently:
◦	MotionEstimationStdDevFeature.  This feature estimates motion with the following algorithm:
▪	Calculates absolute difference for each pixel of 2 sequential frame regions
▪	Calculates standard deviation of this difference
▪	Standard deviation for every region sequence is inserted into corresponding position of output feature's vector

#### AggregatorFeature class
In case user wants to concatenate vectors from different Features into a single feature vector they can use the Aggregator Feature. The Aggregator Feature  concatenates outputs of different Features into a single vector (outputs of “Feature 1”, “Feature 2” and “Feature 3” on the picture above). For example, let’s have Aggregator Feature instance with two internal features: Feature1 and Feature2. The getFeature function takes a region sequence. This region sequence is processed by Feature1 and Feature2. Feature1 returns the vector (0, 1), Feature2 returns the vector (2, 3, 4, 5). Then the Aggregator Feature will return the vector (0, 1, 2, 3, 4, 5).
The pipeline framework works with the Aggregator Feature the same way as with all the other Features. For example, in the case shown above, a Pipeline Node which contains the Aggregator Feature shall call the Aggregator Feature twice: the first time with the region sequence from “Region 1”, the second time with the region sequence from “Region 2” and concatenate output vectors.
The Aggregator Feature can be created by two ways: with the default Factory method or with using a special Factory method. By default both methods create Aggregator Feature with “aggregator” id:
```cpp
static SPtr<IFeature> createFeature(FeatureType type = NORMALFLOW, std::string id = "") throw (IncorrectParamException);

static SPtr<AggregatorFeature> createAggregatorFeature(int numberOfRegions, int numberOfSequences = 1, std::string id = "") throw (IncorrectParamException);
```

If you use the default method you shall specify feature type as AGGREGATOR. In this case the number of regions of created Aggregator Feature will be two and the number of region sequences will be one:
```cpp
SPtr<AggregatorFeature> aggregatorFeature = 			Factory::createFeature(AGGREGATOR);
```

If you use the special Factory method you can set the number of regions and the number of sequences manually:
```cpp
SPtr<AggregatorFeature> aggregatorFeature = 			
	Factory:: createAggregatorFeature(3, 1);
```

Adding and removing of internal features are performed with using addFeature and delFeature functions. Note that all internal features should have the same number of regions and number of sequences as the Aggregator Feature. In the following example the number of regions is 3 and the number of region sequences is 1 for all features:
```cpp
SPtr<AggregatorFeature> aggregator = Factory::createAggregatorFeature(3, 1);
SPtr<Feature> feature1 = Factory::createFeature(STLBP, "feature1"); SPtr<Feature> feature2 = Factory::createSTLBPFeature(
		STLBPFeature::Settings(3, 1, 1, 1, 4, 4, 4, 4, 4), "feature2"); aggregator->addFeature(feature1);
aggregator->addFeature(feature2);

aggregator->delFeature(feature1);
```
#### ConcatenatorFeature class
 
Some features are not descriptive enough to build classification on top of them. But it is possible to build classification on output from a few sequential Region sequences. For example we might want to concatenate features calculated for 3 sequences as shown on the picture above. In case the feature takes as input 2 frames the concatenated feature shall be calculated for 3 sequences:
•	frame #N, frame #N+1
•	frame #N+1, frame #N+2
•	frame #N+2, frame #N+3
The ConcatenatorFeature is responsible for that.  As first we should set an internal feature for the Concatenator Feature. For example this internal feature takes 2 regions as input and returns a vector of length 2. Also we should set the total number of regions which will be processed by Concatenator Feature. This number should be not less than the number of regions for internal feature. For example let we create Concatenator Feature with 4 regions. Let's consider how the feature will be calculated in this case. The getFeature function takes a region sequence which consists of 4 regions (denote them as R1, R2, R3, R4).  As first the internal feature is calculated for regions R1 and R2. For example it returns vector (0, 1). Then  this feature processes the regions R2 and R3 and returns vector (2, 3). At last the internal feature processes the regions R3 and R4 and returns (4, 5). Then Concatenator Feature will return the vector (0, 1, 2, 3, 4, 5).
The Concatenator Feature can be created by two ways: with default Factory method and with special Factory method. By default both methods create Concatenator Feature with “concatenator” id:
```cpp
static SPtr<IFeature> createFeature(FeatureType type = NORMALFLOW, std::string id = "") throw (IncorrectParamException);
static SPtr<ConcatenatorFeature> createConcatenatorFeature(int numberOfRegions, std::string id = "") throw (IncorrectParamException);
```

If you use the default method you shall specify feature type as CONCATENATOR. In this case the number of regions of created Concatenator Feature will be two:
```cpp
SPtr<ConcatenatorFeature> concatenatorFeature = 			Factory::createFeature(CONCATENATOR);
```

If you use the special Factory method you can set the number of regions manually:
```cpp
SPtr<ConcatenatorFeature> concatenatorFeature = 			
	Factory:: createConcatenatorFeature(4);
```

You can change the number of regions for Concatenator Feature by using setNumberOfRegions function. Setting of internal feature is performed with using setFeature function:
```cpp
SPtr<ConcatenatorFeature> concatenator =
	Factory::createConcatenatorFeature(4);
SPtr<Feature> internalFeature = Factory::createFeature(NORMALFLOW); concatenator->setFeature(internalFeature);
```
#### ClassifierFeature class
The ClassifierFeature is applied in hierarchical machine learning (please, read “Hierarchical machine learning” section and “IVCP_Usage_samples” document for more information). This class is responsible for constructing a new feature based on output from another feature and a classifier.
Let's consider an example of how this class works. Let we have a Feature which takes one region sequence with 2 regions as input and returns a vector of length 3. Also let we have a Classifier and two Region Creators. At first we shall train this Classifier with this Feature and Region Creators (please, see “Classifiers”, “Session”, “Pipeline” sections and usage samples for more information). You can use a Classifier which was trained earlier and saved in Repository. But make sure that this Classifier was trained with the same Feature and with the same number of Region Creators (getVariablesCount function of Classifier returns size of vector that trained Classifier can take as input; (Feature length * number of Region Creators) should be equal to this size). 
Now we can create ClassifierFeature instance, set Feature as the ClassifierFeature internal feature and the trained Classifier as internal classifier. Then we need to set number of region sequences for the ClassifierFeature. It should be equal to the number of used Region Creators. In our case it is 2. Also we need to set a type of output: categorical or probabilistic. In the first case Classifier will return predicted class index (e.g. 2), in the second case Classifier will return a probability distribution (e.g. (0.25, 0.5, 0.25) if we have 3 classes). For more information read the “Classifiers” section and “IVCP_Usage_samples” document. In our example we will use a probabilistic output and 3 classes.
The getFeature function takes two Region Sequences (from two Region Creators), every one consists of two Regions. At first the internal Feature processes the first Region Sequence. For example it returns vector (0, 1, 2). Then Feature processes the second Region Sequence and returns vector (2, 1, 3). The ClassifierFeature concatenates these vectors in a single vector (0, 1, 2, 2, 1, 3). This concatenated vector is fed to the internal Classifier which returns probability vector (0.25, 0.5, 0.25). Then Classifier Feature returns the vector (0.25, 0.5, 0.25) as result.
The Classifier Feature can be created by two ways: with default Factory method and with a special Factory method. By default both methods create Classifier Feature with “classifier_feature” id, probabilistic output type and one region sequence:
```cpp
static SPtr<IFeature> createFeature(FeatureType type = NORMALFLOW, std::string id = "") throw (IncorrectParamException);
static SPtr<ClassifierFeature> createClassifierFeature(int numberOfSeq = 1, ClassifierOutputType type = PROBABILISTIC, std::string id = "") throw (IncorrectParamException);
```

If you use the default method you have to specify feature type as CLASSIFIER_FEATURE:
```cpp
SPtr<ClassifierFeature> classifierFeature = 			Factory::createFeature(CLASSIFIER_FEATURE);
```

If you use the special Factory method you can set the number of Region Sequences, output type and identifier manually:
```cpp
SPtr<ClassifierFeature> classifierFeature = 			
	Factory:: createClassifierFeature(2, PROBABILISTIC);
```

You can set internal Feature and Classifier for Classifier Feature by using  setFeature and setClassifier functions.  You can change output type with setOutputType function:
```cpp
SPtr<ClassifierFeature> classifierFeature =
	Factory::createClassifierFeature(2, CATEGORICAL);

SPtr<Feature> feature = Factory::createFeature(NORMALFLOW);
Sptr<IRepository> repository =
		Factory::getRepository(“trained_classifier.xml”);
Sptr<Classifier> classifier = repository->loadClassifier();
classifierFeature->setFeature(feature);
classifierFeature->setClassifier(classifier);
classifierFeature->setOutputType(PROBABILISTIC);
```
#### SummatorFeature class
The SummatorFeature class works similarly to the ConcatenatorFeature class but instead of concatenating features for sequential Region sequences it sums them up. This scenario is especially useful for the Hierarchical classification case (please read the “ClassifierFeature”, “Hierarchical machine learning” sections and the “IVCP_Usage_samples” document for more information).
Let's consider an example of how this class works. We should set an internal feature for the Summator Feature. For example this internal feature could be a ClassifierFeature which takes 2 Regions as input and returns a probability vector of length 4 (i.e. there are 4 classes). Also we should set the total number of Regions which will be processed by Summator Feature. This number should not be less than the number of regions for internal feature.  Also Summator Feature has parameter normalize. If normalize is true Summator Feature will sum up the output vector to 1. Additionally Summator Feature can be configured to sum up classes with percentage over a certain amount (parameters countMode and threshold are used for this purpose).
For example let we create a Summator Feature with 3 Regions and configure it with threshold 0.2. Let's consider how the feature will be calculated in this case. The getFeature function takes a region sequence which consists of 3 regions (denote them as R1, R2, R3).  As first the internal Classifier Feature is calculated for regions R1 and R2. For example it returns probability vector (0.1, 0.2, 0.3, 0.4), denote it V1. Then this feature processes the regions R2 and R3 and returns vector (0.12, 0.2, 0.5, 0.18), denote it V2.  
If countMode is false Summator Feature will return the following results:
•	If normalize is false the vectors V1 and V2 will be simply summarized.  We will have the following result vector: (0.22, 0.4, 0.8, 0.58)
•	If normalize is true then each element of the vector (0.22, 0.4, 0.8, 0.58) will be divided into 2 (the number of the vectors which was be calculated by internal Classifier Feature). We will have  the following result vector: (0.11, 0.2, 0.4, 0.29)
If countMode is true Summator Feature will sum up classes with percentage over a threshold (recall that threshold is 0.2 in our example):
•	If normalize is false we will have the following result vector: (0, 0, 2, 1)
•	If normalize is true then each element of the vector (0, 0, 2, 1) will be divided into 2. We will have  the following result vector: (0, 0, 1, 0.5)
The Summator Feature can be created by two ways: with default Factory method and with a special Factory method. By default both methods create Summator Feature with “summator” id and default settings (the number of regions is 2, countMode is false, threshold is 0.4, normalize is true):
```cpp
static SPtr<IFeature> createFeature(FeatureType type = NORMALFLOW, std::string id = "") throw (IncorrectParamException);
static SPtr<SummatorFeature> createSummatorFeature(SummatorFeature::Settings settings = SummatorFeature::Settings(), std::string id = "") throw (IncorrectParamException);
```

If you use the default method you have to specify feature type as SUMMATOR:
```cpp
SPtr<SummatorFeature> summator = 			Factory::createFeature(SUMMATOR);
```

If you use the special Factory method you can set the number of regions, countMode, threshold, normalize and identifier manually:
```cpp
SPtr<SummatorFeature> summator = Factory:: createSummatorFeature(
			SummatorFeature::Settings(2, true, 0.2, false));
```

You can set internal feature for Summator Feature by using  the setFeature function:
```cpp
SPtr<SummatorFeature> summator = Factory::createSummatorFeature();
SPtr<Feature> internalFeature = Factory::createNormalFlowFeature();
summator->setFeature(internalFeature);
```
#### Advanced multi-sequential features
Both implemented features (normal flow and STLBP) works with a single region sequence (an example of such sequence is an image sequence of opening left eye). However the interface IFeature::getFeature takes a vector of RegionSequence. It was done intentionally as later user may add advanced features that support for multiple regions (an example of such a feature could be a feature that takes sequences of both eyes).
The normal flow and STLBP features checks that size of a vector they receive is equal to 1 and generate IncorrectParamException otherwise.

### Classifiers
The Classifier Filter is responsible for predicting a class label  based on a feature matrix. Also the Classifier allows getting probability for each class that a feature corresponds to this class. The classifier model is build based on data – i.e. multiple samples of all classes shall be recorded with 
```cpp
virtual void IClassifier::addTrainFeature(cv::Mat feature, ClassType response) throw (MismatchFeatureSizeException);
```
where feature is a feature matrix, response is a class label.
After sufficient number of examples trainModel function must be called in order to generate a classification model:
```cpp
virtual void IClassifier::trainModel() throw (NoDataException, IncorrectParamException) = 0;
```

The trained classifier can predict new class of new features with the following function:
```cpp
virtual ClassType IClassifier::clasify(cv::Mat feature) throw (NoModelException, MismatchFeatureSizeException);
```

The trained classifier can evaluate probabilities for each class that new feature belongs to a class. For this the following function is used. It returns the probabilities  vector sorted by number of class:
```cpp
virtual std::vector<float> getDistribution(cv::Mat feature) throw 
  (NoModelException, MismatchFeatureSizeException) = 0;
```

There are 3 implemented classifiers in IVCP library:
•	AdaBoostClassifier. This classifier uses Adaptive Boosting classification algorithm
•	SVMClassifier. This classifier uses Support Vector Machines classification algorithm
•	LRClassifier. This classifier uses Logistic Regression classification algorithm
The classifiers can be created with the following Factory methods:
```cpp
static SPtr<IClassifier> Factory::createClassifier(ClassifierType type, std::string id = "") throw (IncorrectParamException);

static SPtr<SVMClassifier> createSVMClassifier(
    SVMParameters classifierParams = SVMParameters(), 
    std::string id = "") throw (IncorrectParamException);

static SPtr<AdaBoostClassifier> createAdaBoostClassifier(
    AdaBoostClassifier::Settings settings = AdaBoostClassifier::Settings(),
    std::string id = "") throw (IncorrectParamException);


static SPtr<LRClassifier> createLRClassifier(
    LRParameters classifierParams = LRParameters(), 
    std::string id = "") throw (IncorrectParamException);
```

You can read the description of SVMParameters, LRParameters, and AdaBoostClassifier::Settings in the Doxygen documentation.
### Performance
Performance Filter is used for generating performance and quality reports of classification pipeline execution. The Filter shall be added to a Session and connected to Filters being profiled in the pipeline. 
Here is an example of a quality report:
```text
Total precision (% correct): 0.994889
Results for each class:
----------------------------------------------
| Class Name   | Precision(%) | Recall(%)    |
----------------------------------------------
| 0            |     1.000000 |     0.988500 |
| 1            |     0.990884 |     1.000000 |
----------------------------------------------

Confusion matrix:
--------------------------------------------------------
|   Predicted Class Name | 0            | 1            |
| Correct Class Name     |              |              |
--------------------------------------------------------
| 0                      |     0.988500 |     0.011500 |
| 1                      |     0.000000 |     1.000000 |
--------------------------------------------------------
```

The report contains the following metrics:
•	Total precision. An overall estimation of prediction quality for all classes. This metric is calculated as (Total number of correct predictions)/(Total number of predictions)
•	Precision and recall for each class. For each class precision and recall metrics are calculated, where precision=(true positives) / (true positives + false positives), recall = (true positives) / (true positives + false negatives)
•	Confusion matrix. The matrix contains correct classes as rows and predicted classes as columns. Each cell (row index, col index) contains percent of class (row index) being classified as class (col index). So in case of a perfect prediction the matrix shall be equal to identity matrix
Here is an example of a performance report:
```text
---------------------------------------------------------------------------------
| Metric name                 | Avg time (ms) | Max time (ms) | Total time (ms) |
---------------------------------------------------------------------------------
| classifier                  |      0.013556 |      1.000000 |      244.000000 |
| feature                     |      0.981943 |      3.000000 |    17674.000000 |
| region                      |      0.007222 |      1.000000 |      130.000000 |
| source                      |      0.010889 |      1.000000 |      196.000000 |
---------------------------------------------------------------------------------
```
The performance report is generated for all Filters in pipeline being connected to the Performance Filter. Here “Metric name” contains ids of the profiled Filters. The table contains average, maximal and total time of Filters execution.
Currently only text reports are supported (TextPerformance class). In order to create a Text report Filter use the following Factory method:
```cpp
static SPtr<IPerformance> Factory::createPerformance(PerformanceType type = TEXT_REPORT, std::string id = "") throw (IncorrectParamException);
```

In order to generate reports Performance Filter needs to collect statistics of timings (for performance report) and classification results (for quality report). So the Performance Filter shall be connected with Filters being profiled. If you want to add a Filter to a performance report just add a link between this Filter and the Performance Filter with using Pipeline::addLink method. If you want to generate quality report for the classifier connect the classifier Filter to the Performance Filter and then connect a classification result data source with the Performance Filter with using Pipeline::addLink method:
```cpp
//Add feature timings to the performance report
SPtr<NormalFlowFeature > feature = Factory::createNormalFlowFeature();
SPtr<IPerformance > perf = Factory::createPerformance();
pipelineTest->addLink(feature->getId(), perf->getId());
```

Connect a Classifier Filter and a Classification Result Data Source to the Performance Filter to generate a quality report:
```cpp
SPtr<MockClassifierResultDataSource> classifierResult = 
	new MockClassifierResultDataSource();
SPtr<SVMClassifier > classifier = Factory::createSVMClassifier();
pipelineTest->addLink(classifierResult->getId(), perf->getId());
pipelineTest->addLink(classifier->getId(), perf->getId());
```

Instead of direct connecting the Filters you can use Pipeline::connectAll() function (please read the “Pipeline” section for more details).
In order to create a quality report call the following function (filename is a path to a new file that will contain the report):
```cpp
virtual void IPerformance::generateQualityReport(std::string filename);
```

In order to create a performance report call the following function (filename is a path to a new file that will contain the report):
```cpp
virtual void IPerformance::generatePerformanceReport(std::string filename);
```
### Hierarchical Machine Learning
Robust emotion classification shall use temporal information. However 2 frames are not enough to build a robust classification pipeline. One possible solution is to make a prediction based on classification of multiple features calculated on multiple subsequences. The process of making classification based on multiple results from another classifier is called Hierarchical Machine Learning.
In order to implement this functionality the following feature Filters are added to the library:
•	ClassifierFeature. This class is responsible for constructing a new feature based on output from another feature and a classifier. The ClassifierFeature shall take an image sequence as input, calculate the underlying feature on the images and use output of the underlying classifier as new feature output. Output of the underlying classifier could be either categorical or probabilistic. The classifier used by the ClassifierFeature shall be trained separately by using standard train mode of the Pipeline.
 
The ClassifierFeature can be used everywhere where the standard feature is used. For instance the ClassifierFeature can be used as underlying feature of ConcatenatorFeature and AggregatorFeature classes.
•	SummatorFeature. This class operates similarly to the ConcatenatorFeature class but instead of concatenating features for sequential Region sequences it aggregate evaluated results by either summing up feature vector elements or calculating amount of values over a certain threshold for each feature element. Evaluated result could be normalized by the amount of input feature vectors (by default) or provided as is.
The most generic usage example is to have a SummatorFeature with an underlying ClassifierFeature. This approach allows building a hierarchical classification with several classifiers by using the following pipeline:
 
where SummatorFeature uses a ClassifierFeature as underlying feature which in turn encapsulate classifiers from #1 to #(n-1).
In case hierarchical classification is built with just 2 classifiers the ClassifierFeature is built on top of some common feature (i.e. NormalFlowFeature) and Classifier #1:
 
In case hierarchical classification is built with 3 or more classifiers ClassifierFeature is recursively built on top of other ClassifierFeatures encapsulating classifiers from #1 to #(n-2) and classifier #(n-1). For example in case of 3 classifiers the structure should be:
 
You can find more details on using the ClassifierFeature and SummatorFeature classes in the “Features” and in the “hierarchical Machine Learning usage sample” sections.

### Session
Session is a container for all Filters to use by the classification Pipeline. The Pipeline class takes a pointer to Session. So the first step of creating a classification Pipeline is to add Filters you would like to use in Pipeline to a Session.
Session provides easy add/delete interface to operate with the Filters (there are similar add/delete functions for all Filters):
```cpp
void addFeature(SPtr<IFeature > feature);
void delFeature(SPtr<IFeature > feature);
```

You can get number of specific Filters in the Session and get a Filter by index or by ID:
```cpp
int getNumberOfFeatures() const;
SPtr<IFeature > getFeature(std::string featureId) throw(IncorrectParamException);
SPtr<IFeature > getFeature(int featureIndex) throw(IncorrectParamException);
```

When Session is constructed you can save all its content in a file with using Repository interface. It is important to note that when you save a Session object all Filters added to the Session are saved too so you don’t need to call the save method for all nested objects:
```cpp
SPtr<Session> session = Factory::createSession();
session->setVideoPlaylist( playlist);
session->addRegionCreator( region );
session->addFeature( feature );
session->addClassifier( classifier );

SPtr<IRepository> repository = Factory::getRepository("session_tmp.xml");
//All objects added to Session are saved too
repository->save( session );
```

### Pipeline
All Filters implemented in the library can be used separately. However, real power of the library is achieved by using the concept of Pipeline. Pipeline is a directed acyclic graph built on top of the Filters added to a Session. User can test different classification strategies by connecting and disconnecting different Filters added to the Session (for example user can investigate whether or not a nose region has influence on emotions classification by disconnecting the region creator responsible for nose detection from the following Feature Filter).
Pipeline supports 3 modes of operation:
•	Training mode. In training mode the classification model is created based on given video sequence and ground truth data provided by user
•	Testing mode. In testing mode the trained model is applied to a video sequence and accuracy report is generated based on ground truth data provided by user
•	Real-time mode. In real-time mode user can interactively observe results of classification based on a live video stream from a camera or other media
In all 3 modes all frames of Video Playlist are processed in a Pipeline. The main difference between the modes is that in train mode Features are collected into a queue and when all frames are processed the Classifier is trained on the collected data (i.e. Classifier::trainModel function is called). In test mode Features are classified on each frame and Performance Filter accuracy statistics is updated. In real-time mode no accuracy statistics is updated, only performance statistics is saved.
The Filters in the Pipeline are grouped into 4 layers. Each layer can have links to the Filters in neighboring layers only (Filters on layer #N can have outgoing links to layer #N+1 and ingoing links from layer #N-1):
•	Video player layer
•	Regions creators layer
•	Features layer
•	Classification Filter layer
Call function Pipeline::addLink to add a link between 2 Filters (reference the Filters by string IDs) and call  Pipeline:removeLink to remove the link:
```cpp
void addLink(std::string from, std::string to) throw (IncorrectParamException);
void removeLink(std::string from, std::string to) throw (IncorrectParamException);
```

In addition to the links between layers there are special types of links used for generating performance reports. Please read the “Performance” section for details on how to connect the Performance Filter with different Filters in Pipeline.
In case you have only 1 Filter on each layer (with the exception of Region Creators – all Region Creators will be connected to a Feature by the connectAll function) you can simply call the Pipeline::connectAll method to connect all Filters one-by-one. The connectAll function also makes connections between all Filters and the Performance Filter (these connections are necessary for the performance report) and a connection between a Classification Result Data Source Filter and the Performance Filter (this connection is necessary for generating a quality report). Read more details on performance and quality reports generation in the “Performance” section.
In case you have more than 1 Filter on each layer (with the exception of Region Creators) connectAll will raise an exception. In this case you shall connect the Filters manually (for more details read the “Manual Filters connecting” section).
Pipeline can be used to set callbacks for all Filters so that the callback function set by user is called each time the Filter generates new output in the Pipeline. For example callbacks from Video Player will contain the current frame being propagated in the Pipeline.  The callbacks from Region Creator will contain the matrix 1x4 or 4x4 with information about processed region. The first row of this matrix always contains the bounding rectangle: (x, y, width, height).  If size of this matrix is 4x4 then the rest rows contain the transformation matrix (3x3, i.e. the last column does not contain useful information). The callbacks from Feature contain one-row matrix with feature calculated for the current frame. In the train and runtime mode of Pipeline the callbacks from Classifier are available. They contain one-row matrix with classification results for the current frame. The first element of this matrix contains the predicted class, others elements contain probabilities for each class sorted by class label.
```cpp
typedef void (*PipelineCallback)(cv::Mat, void *);
virtual void Ipipeline::addNodeCallback(std::string FilterId, PipelineCallback func, void* context ) throw (IncorrectParamException);
```

Please note that filter output denoted by the cv::Mat parameter is shared between the pipeline and callback function and therefore modification of received data could cause pipeline evaluation result to change. So you have to clone received information in case you are going to modify it.
If you want to track features and responses for specified classifier to file in csv format you can use IPipeline::trackFeatureToFile method. In case pipeline runs in train mode it writes responses from data source, otherwise it writes responses from classifier. The typical usage of this method is following:
```cpp
SPtr<IClassifier> classifier = session->getClassifier(0);
 pipeline->trackFeatureToFile(classifier->getId(), "features.csv");
```
In case you do not specify file name, output file name will set to classifier ID. If it could not open file in write mode it throws IncorrectParamException.
#### Manual Filters connecting
In case your Session contains multiple Filters on each layer you will have to connect them manually. Please follow the rules below to build a correct Pipeline:
1.	There is exactly one Video Player Filter per Session, it shall be connected to all Region Creators you want to use
2.	There could be multiple Region Creators per Session. Please connect all Region Creators you want to use to a Feature
3.	In test and real-time modes there shall be exactly one Feature connected to a Classifier. So in case you have multiple Features in your Session make sure there is only one Feature connected to a Classifier per each Pipeline run. In train mode it is possible to connect multiple Features to a Classifier. Output of all Features will be used to train a Classifier. So the Classifier shall be able to correctly classify output of each Feature used in train mode (length of each Feature vectors shall be equal)
4.	In train mode you can use multiple Classifiers simultaneously. In this case you shall get multiple trained Classifier models on output. Just make sure that each Classifier has exactly one link from a Feature (so you can have either 2 link from a Feature to 2 different Classifiers or there could be 2 Features in a Pipeline connected to 2 different Classifiers). In test mode you can run a Pipeline with multiple Classifiers to get multiple accuracy reports (one per Classifier) – please make sure you have a separate Performance Filters for each Classifier
5.	In order to generate an accuracy report you shall connect a Classifier Filter to a Performance Filter in test mode. The Performance Filter shall also be connected to a Classifier Result Data Source for ground-truth classification results. In case you have multiple Classifiers in a Pipeline you shall have the same number of Performance Filters with corresponding connections (from a Classifier to a Performance Filter and from a Classification Result Data Source to a Performance Filter). A Classifier Result Data Source Filter can be shared between Performance Filters (a single Classifier Result Data Source can be connected to multiple Performance Filters)
6.	In train mode you shall have a connection between a Classification Result Data Source and a Classifier. In case of multiple Classifiers the same Data Source can be shared among the Classifiers (each Classifier shall have a link from the Data Source Filter)
7.	In order to generate a Performance report you shall make a connection from a Filter which performance statistics you want to collect to a Performance Filter. Each Filter can be connected to one Performance Filter only
8.	The Classifier Result Data Sources are used by a Classifier during train mode and by a Performance Filter during test mode. Please read the items 5-7 above for details on making connections with the Classifier Result Data Source Filters
9.	You don’t need to connect a GTRegionDataSource to other Filters in a Pipeline. The only Filter which utilizes GTRegionDataSource is GTRegionCreater which has a reference to the Data Source during construction (see Factory::createGTRegionCreator method)
#### Parallel Pipeline
Parallel pipeline provides similar functionality as sequential pipeline, but has methods which allow setting guarantee performance in test and real-time mode (in frames per second). Please note that parallel pipeline does support the train mode. In parallel pipeline all filters are executed in separated threads so-called “worker threads”. Also parallel pipeline has the main thread which synchronizes “worker threads” and transfers results between the filters. 
In order to use the parallel pipeline it is necessary to set its performance rate and performance rates of all the filters in the session.  Performance rate is a non-negative value which indicates frequency the filter can process new data (in frames per second).  There are 2 new methods added to the filter class to get/set performance rate.  
```cpp
virtual size_t getPerformanceRate();
virtual void setPerformanceRate(size_t rate);
```
Please follow the rules below to set up correct performance rate:
1.	By default, performance rate of all filters is equal to zero (which indicates to use the same rate as pipeline does) and performance rate of parallel pipeline equal to 30
2.	When you add a filter to a parallel pipeline with performance rate equal to zero then this filter will use performance rate set for the parallel pipeline
3.	Performance rate of parallel pipeline must be greater than zero, otherwise it will throw an exception
4.	Filter performance rate should be less or equal than performance rate of parallel pipeline 
5.	Video source shall have performance rate equal to parallel pipeline
If performance rate of a filter is less than performance rate of a parallel pipeline the pipeline can use results processed on the previous step. For example, if parallel pipeline have performance rate equal to 30 and region creator filter have performance rate equal to 15 then region creator provides new rectangle each second frame from video source. It can degrade the results of the classification, but provides better performance. Please see a more complex example below. Here different colors correspond to different stages of processing. In case the filter has the same color for a few sequential frames it uses results from the previous step.
 
* These region and feature calculated using frame which be sourced two steps ago.
** This classifier result start calculated using frame which be sourced four steps ago.

In case performance rate of a filter is greater than it can actually work with the parallel pipeline will wait for filter results and user will receive a warning message on the console. In case you receive this warning try to decrease performance rate of the filter.
To create a parallel pipeline use the following method:
```cpp
SPtr<IPipeline> createParallelPipeline(PipelineMode mode, IPipeline::Settings settings = IPipeline::Settings(), std::string id = "") throw (IncorrectParamException);
```
A typical usage example of parallel pipeline is the following:
```cpp
ivcpInit();
SPtr<Session> session = Factory::createSession("session");

//Create filters
SPtr<VideoPlayer > videoPlayer = Factory::createVideoPlayer();
SPtr<VideoPlaylist > videoPlaylist = videoPlayer->getPlaylist();
SPtr<VideofileVideoSource > videoFile = 
    Factory::createVideofileVideoSource("video.avi");
videoPlaylist->addSourceRange(videoFile, FrameRange(0, videoFile->getFrameCount()));

SPtr<BBoxRegionCreator > bboxRegion1 = 
    Factory::createBBoxRegionCreator(cv::Rect(0, 0, 48, 48), "bbox1");
SPtr<BBoxRegionCreator > bboxRegion2 = 
    Factory::createBBoxRegionCreator(cv::Rect(0, 0, 24, 24), "bbox2");

SPtr<NormalFlowFeature > feature = Factory::createFeature(NORMALFLOW);

SPtr<SVMClassifier > classifier = Factory::createClassifier(ivcp::SVM);

SPtr<IClassifierResultDataSource> classifier_results =
    Factory::createClassifierResultDataSource("classifier_results");
for (int i=0; i<videoFile->getFrameCount(); i++)
{
    classifier_results->addClassifyResult(VideoPlaylistFrame(0, i), i % 2);
}

//Set performance rate
videoPlayer->setPerformanceRate(50);
bboxRegion1->setPerformanceRate(25);
bboxRegion2->setPerformanceRate(50);
feature->setPerformanceRate(25);
classifier->setPerformanceRate(10);

//Add filters to session
session->setVideoPlaylist(videoPlaylist);
session->addRegionCreator(bboxRegion1);
session->addRegionCreator(bboxRegion2);
session->addFeature(feature);
session->addClassifier(classifier);
session->addDataSource(classifier_results);

//Create sequential pipeline for train classifier
SPtr<Pipeline> trainPipeline = Factory::createPipeline(TRAIN_PIPELINE);
trainPipeline->setSession(session);
trainPipeline->connectAll();
trainPipeline->run();

//Create parallel pipeline in runtime mode
SPtr<ParallelPipeline> runtimePipeline = 
    Factory::createParallelPipeline(RUNTIME_PIPELINE);
    
//Set pipeline performance rate (must be equal to video player performance rate)
runtimePipeline->setPerformanceRate(50);

runtimePipeline->setSession(session);
runtimePipeline->connectAll();
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(runtimePipeline);
runtimePipeline->run();
```



## Objects lifecycle

### Smart pointers
All Filters in IVCP shall be created in heap. In order to minimize probability of memory leaks and to simplify managing of objects lifecycle we use shared smart pointers (similar to std::smart_ptr):
```cpp
namespace ivcp
{
template<typename Type> class IVCP_EXPORT SPtr;
}
```

When user allocates memory and assigns the allocated pointer to a typed smart pointer it is now responsibility of the smart pointer to count for links to the memory and free it when there is no more references to the memory:
```cpp
int *ai = new int;

//The smart pointer now stores 1 reference to the ai memory region
SPtr<int> aiPtr = ai;

{
    //Now both smart pointers (aiPtr and aiPtr2) points to the same memory, therefore number of references is 2
    SPtr<int> aiPtr2 = aiPtr;
}
//aiPtr2 leaves its scope, therefore there is only 1 reference to ai

//aiPtr now is assigned with another pointer, ai is deallocated
aiPtr = 0;

//Runtime error:
//Now it is impossible to use ai since it is deallocated
//*ai = 0;  //Error!
```
 
When you assign SPtr<A> to SPtr<B>, internally the smart pointer dynamic_casts B* to A*. If cast fails the pointer on the left side will be assigned a NULL-pointer:
```cpp
class A
{
public:
    virtual ~A() {};
};

class B: public A
{
public:
    virtual ~B() {};
};

int main(int argc, char **argv)
{
    //OK
    SPtr<A> a = new B;

    //OK
    SPtr<B> b = a;

    //OK
    SPtr<A> a2 = new A;

    //b2 stores a NULL-pointer
    SPtr<B> b2 = a2;
    assert(b2 == NULL);

    return 0;
}
```

### Objects creation
It is recommended to create all Filters with using Factory class methods. Currently in most cases it is safe to create objects with direct call of new operator as the only difference in comparison with the Factory-created objects is that Factory guarantees the objects are assigned an id (if no id is set by user the Factory will assign a default id). However, later the Factory could be used to inject dependency to infrastructure code into IVCP Filters. So it is highly recommended to use only the Factory methods for creating Filters:
```cpp
namespace ivcp
{
class IVCP_EXPORT Factory
{
public:
    static SPtr<Session> createSession(std::string id = "") throw (IncorrectParamException);
    static SPtr<IPipeline> createPipeline(PipelineMode mode, std::string id = "") throw (IncorrectParamException);

    static SPtr<IVideoPlayer> createVideoPlayer(std::string id = "") throw (IncorrectParamException);

    static SPtr<CameraVideoSource> createCameraVideoSource(int cameraIndex, std::string id = "") throw (IncorrectParamException);
    static SPtr<VideofileVideoSource> createVideofileVideoSource(std::string path, std::string id = "") throw (IncorrectParamException);
    static SPtr<IVideoSource> createVideoSource(VideoSourceType type = VIDEOFILE, std::string id = "") throw (IncorrectParamException);

//…

};
}
```


Factory class provides 2 types of functions:
•	Generic create-functions. These functions take Filter type as a parameter and return generic type pointer. User shall cast the generic type pointer to specific type pointer and configure the Filter manually (if needed):
```cpp
//Implicitly cast SPtr<IVideoSource> to SPtr<VideofileVideoSource>
SPtr<VideofileVideoSource> videoSource = 	Factory::createVideoSource(VIDEOFILE, "my_file");
if (videoSource != NULL)
{
    //Filter-specific configuration
    videoSource->setFilename("c:/video.avi");
}
```

•	Specific create-functions. These functions take Filter-specific parameters and return pointer to a specific type:
```cpp
SPtr<VideofileVideoSource> videoSource = 	Factory::createVideofileVideoSource("c:/video.avi");
```

### Object persistence - Repository
Filters can be saved to a file and reloaded from the file in their original state. For example we can train a SVM model and load it in another application from a file instead of retraining. IRepository interface is used for loading and saving of Filters:
```cpp
namespace ivcp
{
class IVCP_EXPORT IRepository
{
public:
virtual ~IRepository() {};

virtual SPtr<Filter> load(std::string id);
virtual SPtr<Session> loadSession(std::string id = "");
virtual SPtr<IPipeline> loadPipeline(std::string id = "")
virtual SPtr<IClassifier> loadClassifier(std::string id = "");
virtual SPtr<IDataSource> loadDataSource(std::string id = "");
virtual SPtr<IClassifierResultDataSource>
			loadClassifierResultDataSource(std::string id = "");
virtual SPtr<IRegionDataSource> loadRegionDataSource(std::string id = "");
virtual SPtr<IFeature> loadFeature(std::string id = "");
virtual SPtr<IPerformance> loadPerformance(std::string id = "");
virtual SPtr<IRegionCreator> loadRegionCreator(std::string id = "");
virtual SPtr<IVideoSource> loadVideoSource(std::string id = "");
virtual SPtr<IVideoPlayer> loadVideoPlayer(std::string id = "");
virtual SPtr<VideoPlaylist> loadVideoPlaylist(std::string id = "");
virtual void save(SPtr<Filter> Filter);
};
}
```

Each Repository object is associated with a file (url parameter of a Factory method is a system path to the Repository file):
```cpp
static SPtr<IRepository> Factory::getRepository(std::string url);
```

If you save a few objects to a single Repository make sure the objects have different IDs – otherwise the last object will overwrite the previous. 
If you load an object the Repository searches for the first match. For example, if you call loadSession with no id (id = “”) the Repository will load the first session in the repository.
If you save a Filter that has nested references to other Filters (Session for example) all referenced objects will be saved too. Generally it is a good practice to save Filters with multiple references to different Filters (Pipeline or Session) to separate files to make sure no id-conflicts with objects previously saved to the Repository occurs.
## Usage samples

### Basic UCLA test

#### Overview
The UCLA dataset contains 200 video files (resolution of each video file is 48x48 pixels) of 10 dynamic texture classes. In this sample we train SVM classifier on 6 video files of 2 classes and then use the same video files to test the trained model.
#### Source code
As an alternative for creating an XML file with classification results as described in the Marking applications section you can implement IClassifierResultDataSource interface and you it as a data source.
```cpp
namespace
{
class MockClassifierResultDataSource: public IClassifierResultDataSource
{
    public:
        //Video files 0,1,2 -> class 0
        //Video files 3,4,5 -> class 1
        virtual ClassType getClassifyResult(VideoPlaylistFrame frame)
        {
            return frame.sourceIndex / 3;
        };

        //Miss the first frame, all the rest are assumed to be classified
        virtual bool isClassified(VideoPlaylistFrame frame)
        {
            return frame.frameIndex != 0;
        };

        virtual DataSourceType getType()
        {
            return CLASSIFIER_RESULT;
        };
};
}

SPtr<Session> session = Factory::createSession();
    
//Use Mock object 
SPtr<MockClassifierResultDataSource> classifierResult = new 
	MockClassifierResultDataSource;
classifierResult->setId("classifier_result");
    
//Create video player 
SPtr<VideoPlayer > videoPlayer = Factory::createVideoPlayer();
SPtr<VideoPlaylist > videoPlaylist = videoPlayer->getPlaylist();

//Create playlist, add video sources
stringstream sourceFilename;
sourceFilename << UCLA1_FOLDER << "ucladb_wfalls-d-near_1s1.avi";
SPtr<IVideoSource > source11 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source11, FrameRange(0, 75));

sourceFilename << UCLA1_FOLDER << "ucladb_wfalls-d-near_1s2.avi";
SPtr<IVideoSource > source12 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source12, FrameRange(0, 75));

sourceFilename << UCLA1_FOLDER << "ucladb_wfalls-d-near_2s1.avi";
SPtr<IVideoSource > source13 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source13, FrameRange(0, 75));

sourceFilename << UCLA1_FOLDER << "ucladb_candle_4s1.avi";
SPtr<IVideoSource > source21 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source21, FrameRange(0, 75));

sourceFilename << UCLA1_FOLDER << "ucladb_candle_4s2.avi";
SPtr<IVideoSource > source22 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source22, FrameRange(0, 75));

sourceFilename << UCLA1_FOLDER << "ucladb_candle_5s1.avi";
SPtr<IVideoSource > source23 = Factory::createVideofileVideoSource(sourceFilename.str());
sourceFilename.str("");
videoPlaylist->addSourceRange(source23, FrameRange(0, 75));

//Create normalized flow feature
SPtr<NormalFlowFeature > feature = Factory::createFeature(NORMALFLOW);

//Create BBox region 
SPtr<BBoxRegionCreator > bboxRegion = Factory::createBBoxRegionCreator(Rect(0, 0, 48, 48));

//Create SVM classifier 
SPtr<SVMClassifier > classifier = Factory::createClassifier(ivcp::SVM);

//Create a Performance Filter
SPtr<IPerformance> perf = Factory::createPerformance();

//Add all Filters to the Session
session->setVideoPlaylist(videoPlaylist);
session->addFeature(feature);
session->addRegionCreator(bboxRegion);
session->addClassifier(classifier);
session->addDataSource(classifierResult);
session->addPerformanceFilter(perf);
	
//Create Pipeline for training
SPtr<Pipeline> pipeline = Factory::createPipeline(TRAIN_PIPELINE);
pipeline->setSession(session);
pipeline->connectAll();
pipeline->run();

//Create Pipeline for test
//Seek to the start position
session->getVideoPlayer()->getPlaylist()->seek(0,0);
SPtr<Pipeline> pipelineTest = Factory::createPipeline(TEST_PIPELINE);
pipelineTest->setSession(session);
pipelineTest->connectAll();
pipelineTest->run();

//Output quality report to TXT file
perf->generateQualityReport("./utest1_quality.txt");
```

The generated utest1_quality.txt will contain the following result:
```text
Total precision (% correct): 0.993243

Results for each classes:
----------------------------------------------
| Class Name   | Precision(%) | Recall(%)    |
----------------------------------------------
| 0            |     0.991031 |     0.995495 |
| 1            |     0.995475 |     0.990991 |
----------------------------------------------

Confusion matrix:
--------------------------------------------------------
|   Predicted Class Name | 0            | 1            |
| Correct Class Name     |              |              |
--------------------------------------------------------
| 0                      |     0.995495 |     0.004505 |
| 1                      |     0.009009 |     0.990991 |
--------------------------------------------------------
```
### Emotion classification sample

#### Overview
This sample shows a simple scenario of emotion classification. It trains an AdaBoost classifier on 2 videos with 2 classes of emotions: a smile and a blink. The sample uses 4 facial regions detected with using FaceAPI library: left eye, right eye, mouth and face. When training is done the sample uses 2 different videos with the same emotion classes to classify emotions. 
Please note that this sample is in intended to demonstrate basic concepts of usage the IVCP library. Classification quality of the model trained on the training step is not sufficient for real world applications. 
#### Source code
In order to run the project you can create a module in IVCP source tree:
•	Open “SOURCES”/src/modules/tests folder and copy content of  demo_emotion_classification subfolder to a new folder sample_emotion_classification
•	Open the “SOURCES”/src/modules/tests/sample_emotion_classification/CMakeLists.txt file in an editor
•	Rename project “ivcp_demo_emotion_classification” to “ivcp_sample_emotion_classification”
•	Open “SOURCES”/src/modules/tests/CMakeLists.txt file in an editor and new  line “add_subdirectory(sample_emotion_classification)”
•	Now regenerate project in CMake as it is described in the “Building instruction” section
•	Open project in Visual Studio. If everything is done correctly you shall have a new project ivcp_sample_emotion_classification being added to the solution
•	Open demo_emotion.cpp in the ivcp_sample_emotion_classification project and replace it with the following code:

```cpp
#include <time.h>
#include "ivcp.hpp"
#include "demo_environment.hpp"
using namespace cv;
using namespace std;
using namespace ivcp;
namespace
{
    class MockEmotionsClassifier: public IClassifierResultDataSource
    {
    public:
        virtual ClassType getClassifyResult(VideoPlaylistFrame frame)
        {
            return frame.sourceIndex;
        }
        virtual bool isClassified(VideoPlaylistFrame frame)
        {
            if (frame.frameIndex > 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        virtual ClassifierResultType getType()
        {
            return XML_CLASSIFIER_RESULTS;
        }
        virtual void removeClassifyResult(VideoPlaylistFrame frame){};
        virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType mark){};
    };
}

void addFaceAPIRegions(SPtr<Session> session)
{
    while (session->getNumberOfRegionCreators())
    {
        session->delRegionCreator(session->getRegionCreator(0));
    }
    SPtr<IFaceAPI> faceAPIHandler = Factory::createFaceAPI();
    SPtr<IRegionCreator> leftEye = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_LEYE), "leftEye", faceAPIHandler);
    SPtr<IRegionCreator> rightEye = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_REYE), "rightEye", faceAPIHandler);
    SPtr<IRegionCreator> mouth = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_MOUTH), "mouth", faceAPIHandler);
    SPtr<IRegionCreator> face = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_FACE), "face", faceAPIHandler);

    session->addRegionCreator(leftEye);
    session->addRegionCreator(rightEye);
    session->addRegionCreator(mouth);
    session->addRegionCreator(face);
}

void addASMRegions(SPtr<Session> session) 
{ 
    while (session->getNumberOfRegionCreators()) 
    { 
        session->delRegionCreator(session->getRegionCreator(0)); 
    } 
    SPtr<IASM> asmHandler = Factory::createASM(); 
    SPtr<IRegionCreator> leftEye = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_LEYE), "leftEye", asmHandler); 
    SPtr<IRegionCreator> rightEye = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_REYE), "rightEye", asmHandler); 
    SPtr<IRegionCreator> mouth = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_MOUTH), "mouth", asmHandler); 
    SPtr<IRegionCreator> face = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_FACE), "face", asmHandler); 
    session->addRegionCreator(leftEye); 
    session->addRegionCreator(rightEye); 
    session->addRegionCreator(mouth); 
    session->addRegionCreator(face); 
}

int main(int argc, char **argv)
{
    //All applications shall call ivcpInit function first
    ivcpInit();
    ////////////TRAIN MODE///////////////
    //Session is an object that contain all Filters
    SPtr<Session> session = Factory::createSession();
    //Create a playlist used for training
    SPtr<VideoPlaylist> trainPlaylist = Factory::createVideoPlaylist();
    //Add 2 videos of 2 classes to the train playlist
    SPtr<VideofileVideoSource> trainVideoSourceClass1 = Factory::createVideofileVideoSource("res/blink/blink_1.avi");
    SPtr<VideofileVideoSource> trainVideoSourceClass2 = Factory::createVideofileVideoSource("res/smile/smile_1.avi");
    for (int i = 0; i < 5; i++)
    {
        trainPlaylist->addSourceRange(trainVideoSourceClass1, FrameRange(0, 5));
    }
    trainPlaylist->addSourceRange(trainVideoSourceClass1, FrameRange(0, 71));
    for (int i = 0; i < 5; i++)
    {
        trainPlaylist->addSourceRange(trainVideoSourceClass2, FrameRange(0,5));
    }
    trainPlaylist->addSourceRange(trainVideoSourceClass2, FrameRange(0, 91));
    session->setVideoPlaylist(trainPlaylist);
    //Create 3 FaceAPI region creators for eyes and mouth
    //Please note in the next code drop the FaceAPI region creator will be created via the Factory
    addFaceAPIRegions(session);
    //or
    //addASMRegions(session);

    //Create LBP-TOP features
    STLBPFeature::Settings settings;
    settings.useUniformPatterns = true;
    settings.normalizeFeature = true;
    SPtr<IFeature> feature = Factory::createSTLBPFeature(settings);
    session->addFeature(feature);
    //Create AdaBoost classifier
    SPtr<IClassifier> classifier = Factory::createClassifier(ivcp::ADABOOST);
    session->addClassifier(classifier);
    //On train mode we need a data source with classification results
    //You can use the marking application or create a small class that //implements IClassifierResultDataSource
    SPtr<IClassifierResultDataSource> trainClassificationResultsDataSource = new MockEmotionsClassifier;
    session->addDataSource(trainClassificationResultsDataSource);
    //Create pipeline that connects the Filters together
    SPtr<Pipeline> trainPipeline = Factory::createPipeline(TRAIN_PIPELINE);
    trainPipeline->setSession(session);
    //In simple cases connectAll just connects all Filters in a Session
    trainPipeline->connectAll();
    SPtr<IGUIFramework> guiTrain = Factory::createGUIFramework(trainPipeline);
    trainPipeline->run();
    //Now the classifier object is trained to classify smile vs blink
    ////////////RUNTIME MODE////////////////
    //Now Session shall play the test videos
    SPtr<VideoPlaylist> testPlaylist = Factory::createVideoPlaylist();
    session->setVideoPlaylist(testPlaylist);

    addFaceAPIRegions(session);
    //or
    //addASMRegions(session);

    //Add 2 videos of 2 classes to the test playlist
    SPtr<VideofileVideoSource> testVideoSourceClass1 = Factory::createVideofileVideoSource("res/blink/blink_2.avi");
    SPtr<VideofileVideoSource> testVideoSourceClass2 = Factory::createVideofileVideoSource("res/smile/smile_2.avi");
    testPlaylist->addSourceRange(testVideoSourceClass1, FrameRange(0, 62));
    for (int i = 0; i < 1; i++)
    {
        testPlaylist->addSourceRange(testVideoSourceClass2, FrameRange(0, 9));
    }
    testPlaylist->addSourceRange(testVideoSourceClass2, FrameRange(0, 52));
    session->setVideoPlaylist(testPlaylist);
    //Create test pipeline
    SPtr<Pipeline> testPipeline = Factory::createPipeline(ivcp::RUNTIME_PIPELINE);
    testPipeline->setSession(session);
    //In simple cases connectAll just connects all Filters in a Session
    testPipeline->connectAll();
    SPtr<IGUIFramework> gui = Factory::createGUIFramework(testPipeline);
    testPipeline->run();
    return 0;
}
```
#### Code walkthrough
The sample consists of 2 main parts: training and runtime. In the training we train the classification model. In the runtime mode we visualize the emotion classification on 2 different videos. 
As was described in the “Filters” section the Pipeline consists of the following Filters: Video Player, Region Creators, Feature and Classifier. The Filters shall be grouped in a Session which then is passed to a Pipeline object. Let’s explain how it is done in the sample in details.
#### Train mode
At first the session is created. The session is used to group all Filters being used:
```cpp
SPtr<Session> session = Factory::createSession();
```

Then we define what Video Sources are used in the train mode. To do it we first get a pointer to a newly created Video Playlist object:
```cpp
SPtr<VideoPlaylist> trainPlaylist = Factory::createVideoPlaylist();
```

Then we add Video files to the Playlist and specify the first and the last frames in the Video Source to play. Here we add all frames of 2 videos ("res/blink/blink_1.avi" and "res/smile/smile_1.avi"). Additionally we ask the playlist to add a few frames between the videos which are not used for training. This improves facial regions detection by FaceAPI (i.e. when Video Player switches between 2 videos FaceAPI cannot redetect new face position for a few frames):
```cpp
SPtr<VideofileVideoSource> trainVideoSourceClass1 = Factory::createVideofileVideoSource("res/blink/blink_1.avi");
SPtr<VideofileVideoSource> trainVideoSourceClass2 = Factory::createVideofileVideoSource("res/smile/smile_1.avi");
for (int i = 0; i < 5; i++) 
{
    trainPlaylist->addSourceRange(trainVideoSourceClass1, FrameRange(0, 5));
}
trainPlaylist->addSourceRange(trainVideoSourceClass1, FrameRange(0, 71));
for (int i = 0; i < 5; i++) 
{
    trainPlaylist->addSourceRange(trainVideoSourceClass2, FrameRange(0, 5));
}
trainPlaylist->addSourceRange(trainVideoSourceClass2, FrameRange(0, 91));
session->setVideoPlaylist(trainPlaylist);
```

Then we add 4 facial regions (face, left eye, right eye, mouth):
```cpp
void addFaceAPIRegions(SPtr<Session> session)
{
    while (session->getNumberOfRegionCreators())
    {
        session->delRegionCreator(session->getRegionCreator(0));
    }
    SPtr<IFaceAPI> faceAPIHandler = Factory::createFaceAPI();
    SPtr<IRegionCreator> leftEye = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_LEYE), "leftEye", faceAPIHandler);
    SPtr<IRegionCreator> rightEye = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_REYE), "rightEye", faceAPIHandler);
    SPtr<IRegionCreator> mouth = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_MOUTH), "mouth", faceAPIHandler);
    SPtr<IRegionCreator> face = Factory::createFaceAPIRegionCreator( FaceAPIRegionCreator::Settings(REGION_FACE), "face", faceAPIHandler);

    session->addRegionCreator(leftEye);
    session->addRegionCreator(rightEye);
    session->addRegionCreator(mouth);
    session->addRegionCreator(face);
}
```


Or from ASM:
void addASMRegions(SPtr<Session> session) 
{ 
    while (session->getNumberOfRegionCreators()) 
    { 
        session->delRegionCreator(session->getRegionCreator(0)); 
    } 
    SPtr<IASM> asmHandler = Factory::createASM(); 
    SPtr<IRegionCreator> leftEye = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_LEYE), "leftEye", asmHandler); 
    SPtr<IRegionCreator> rightEye = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_REYE), "rightEye", asmHandler); 
    SPtr<IRegionCreator> mouth = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_MOUTH), "mouth", asmHandler); 
    SPtr<IRegionCreator> face = Factory::createASMRegionCreator( ASMRegionCreator::Settings(REGION_FACE), "face", asmHandler); 
    session->addRegionCreator(leftEye); 
    session->addRegionCreator(rightEye); 
    session->addRegionCreator(mouth); 
    session->addRegionCreator(face); 
}

Then we add a LBP-TOP feature:
//Create LBP-TOP features
STLBPFeature::Settings settings;
settings.useUniformPatterns = true;
settings.useRotateInvariant = true;
SPtr<IFeature> feature = Factory::createSTLBPFeature(settings);
session->addFeature(feature);

And the AdaBoost classifier:
//Create AdaBoost classifier
SPtr<IClassifier> classifier = Factory::createClassifier(ivcp::ADABOOST);
session->addClassifier(classifier);

In order to train the AdaBoost class we need to have a source of ground truth classification data. For this purpose we added a simple class that implements IClassifierResultDataSource. getClassifyResult function takes a structure that describes position in Video playlist (i.e. source index and frame index). The function classifies frames of the first video source as “class 0”, frames of the second video source as “class 1”. isClassified function ignores frames with index less than 10 to give FaceAPI library time to detect facial regions robustly:
namespace 
{
class MockEmotionsClassifier: public IClassifierResultDataSource
{
public:
    virtual ClassType getClassifyResult(VideoPlaylistFrame frame)
    {
        return frame.sourceIndex;
    }

    virtual bool isClassified(VideoPlaylistFrame frame)
    {
        if (frame.frameIndex > 10)
        {
            return true;
        }
        else
        {
            return false;   
        }
    }

    virtual DataSourceType getType()
    {
        return CLASSIFIER_RESULT;
    }
};
}

Then the MockEmotionsClassifier data source is added to the Session:
session->addDataSource(trainClassificationResultsDataSource);

Then we construct a Pipeline object in Train mode and create a simple GUI visualizing the training process. In this simple scenario the links in the Pipeline can be created automatically with using connectAll function (please read more details on connecting Filters in the “Pipeline” section):
//Create pipeline that connects the Filters together
SPtr<Pipeline> trainPipeline = Factory::createPipeline(ivcp::TRAIN_PIPELINE);
trainPipeline->setSession(session);

//In simple cases connectAll just connects all Filters in a Session
trainPipeline->connectAll();
SPtr<IGUIFramework> guiTrain = Factory::createGUIFramework(trainPipeline);
trainPipeline->run();
#### Runtime mode
In the runtime mode we use the Classifier object trained on the train step. Feature object could be the same. So we simply update the Session object instead of recreating it. At first we create new playlist that contain 2 new videos with classes “blink” and “smile” and an auxiliary sequence of frame (testPlaylist->addSourceRange(testVideoSourceClass2, FrameRange(0, 9))) to improve FaceAPI regions detection:
SPtr<VideoPlaylist> testPlaylist = Factory::createVideoPlaylist();
session->setVideoPlaylist(testPlaylist);    

//Add 2 videos of 2 classes to the test playlist
SPtr<VideofileVideoSource> testVideoSourceClass1 = Factory::createVideofileVideoSource("res/blink/blink_2.avi");
SPtr<VideofileVideoSource> testVideoSourceClass2 = Factory::createVideofileVideoSource("res/smile/smile_2.avi");
testPlaylist->addSourceRange(testVideoSourceClass1, FrameRange(0, 62));
for (int i = 0; i < 1; i++) 
{
    testPlaylist->addSourceRange(testVideoSourceClass2, FrameRange(0, 9));
}
testPlaylist->addSourceRange(testVideoSourceClass2, FrameRange(0, 52));
session->setVideoPlaylist(testPlaylist);

Then we recreate FaceAPI regions:
addFaceAPIRegions(session);

Then we create a Pipeline object in runtime mode and add a visualization GUI:
//Create test pipeline
SPtr<Pipeline> testPipeline = Factory::createPipeline(ivcp::RUNTIME_PIPELINE);
testPipeline->setSession(session);

//In simple cases connectAll just connects all Filters in a Session
testPipeline->connectAll();
SPtr<IGUIFramework> gui = Factory::createGUIFramework(testPipeline);
testPipeline->run();
#### Alternative Region Creators
In case precision of FaceAPI regions detection is not sufficient you can use your own regions. Here are the possible options:
•	In case region position is constant on all frames same use BBoxRegionCreator class instead of FaceAPIRegionCreator (please find a sample on usage of BBoxRegionCreator in the “Basic UCLA test” section)
•	You can implement IRegionDataSource interface and feed it to GTRegionCreator class which could be used instead of FaceAPIRegionCreator:
class IVCP_EXPORT IRegionDataSource: public DataSource
{
public:
    /*!
    * \brief Get region
    * \param[in] frame Frame that contains region
    * \param[in] regionClass Class of region
    * \return Bounding rectangle of region
    */
    virtual cv::Rect getRegion(VideoPlaylistFrame frame, ClassType regionClass) = 0;

    /*!
    * \brief Get all regions in frame
    * \param[in] frame Frame that contains the regions
    * \return List of regions
    */
    virtual std::list <std::pair <ClassType, cv::Rect> > getRegionsByFrame(VideoPlaylistFrame frame) = 0;

    /*!
    * \brief Add new region in data source
    * \param[in] frame Frame that contains region
    * \param[in] rect Bounding rectangle of region
    * \param[in] regionClass Class of region
    */
    virtual void addRegion(VideoPlaylistFrame frame, cv::Rect rect, ClassType regionClass) = 0;

    /*!
    * \brief Get transformation of frame related to extraction
    * of ground truth regions
    * \param[in] frame Frame that contains regions
    * \return Transformation matrix for the frame
    */
    virtual cv::Mat getTransformation(VideoPlaylistFrame frame) = 0;

    /*!
    * \brief Set transformation of frame related to extraction
    * of ground truth regions
    * \details There should be at least one region related
    * to the frame to store transformation information.
    * If there is no region defined on the frame nothing would be done.
    * \param[in] frame Frame that contains regions
    * \param[in] transformation Transformation matrix for the frame
    */
    virtual void setTransformation(VideoPlaylistFrame frame, cv::Mat transformation) = 0;
}

You will need to implement getRegion and getRegionsByFrame methods that take the current frame index and a region class id set as a parameter in the constructor of GTRegionCreator. You also will need to implement getTransformation method that takes the current frame index as a parameter. This method could just return the matrix of zero size in case the image transformation functionality is redundant. You can leave addRegion, delRegion and setTransformation implementations blank as they aren’t used by the Pipeline.
Then you need to create GTRegionCreator class and set the newly added successor of IRegionDataSource as source parameter of GTRegionCreator constructor:
GTRegionCreator(SPtr<const VideoPlaylist> plist, SPtr<IRegionDataSource> source, ClassType regClass);

In case you have multiple Regions you shall create multiple GTRegionCreator with different regClass parameters (for example, 0 for left eye, 1 for right eye, 2 for mouth etc.). These values are used for identification of regions in IRegionDataSource::getRegion function.
•	You can use the ivcp_region_markup application to manually markup the regions (see the “Marking applications“ section for more details)
### Concatenator Feature usage sample

#### Overview
The LBP-TOP features used in the previous sample require 3 consecutive frames (i.e. Feature::getNumberOfRegions() = 3):
 
In this sample we show how to utilize the ConcatenatorFeature class to calculate a feature based on concatenation of LBP-TOP features for sequential frames (i.e. LBP-TOP feature for frames ##0-2 and LBP-TOP feature for frames ##1-3):
 
#### Source code
In the previous sample we constructed the LBP-TOP feature with the following code:
//Create VLBP-TOP features
STLBPFeature::Settings settings;
settings.useUniformPatterns = true;
settings.useRotateInvariant = true;
SPtr<IFeature> feature = Factory::createSTLBPFeature(settings);
session->addFeature(feature);

In order to concatenate the LBP-TOP features for consecutive frames the code shall be modified as following:
//Create VLBP-TOP features
STLBPFeature::Settings settings;
settings.useUniformPatterns = true;
settings.useRotateInvariant = true;
SPtr<IFeature> feature = Factory::createSTLBPFeature(settings);
SPtr<ConcatenatorFeature> concatenatorFeature = Factory::createConcatenatorFeature(4);
concatenatorFeature->setFeature(feature);
session->addFeature(concatenatorFeature);

Here we construct the object of type ConcatenatorFeature with parameter 4 – number of regions it requires (i.e. regions on frames ##0-3). The number of regions (i.e. number of consecutive frames) used by ConcatenatorFeature shall be greater or equal to the number of regions used by the underlying feature (i.e. the feature concatenated by ConcatenatorFeature). In case those numbers are equal (i.e. if ConcatenatorFeature is created with parameter 3) output of ConcatenatorFeature is equal to output of the underlying feature.
Please note that since length of the LBP-TOP feature is large it is not recommended to concatenate more than 2-3 features, otherwise training and classification time will increase drastically. For example, if you set number of regions used by the ConcatenatorFeature to 10 it will concatenate 10-3+1=8 features, so length of resulting vector will be 8x the original feature length.
### Hierarchical Machine Learning usage sample

#### Overview
In this sample we show how to utilize the SummatorFeature and ClassifierFeature classes to train and use for classification a hierarchical classifier set. The sample is partially similar to the emotion classification sample since the same regions and playlist would be used for the training of smile and blink emotions recognition. The other part of the sample is an addition to the code intended to train the second classifier and prepare the system for more complex classifiers usage during runtime.
#### Source code
The sample consists of 2 main parts: training and runtime. Since the sample is similar to the emotion classification sample we will use it as a base. The difference in the code will be related to training part of the samples. The original classifier training code would be used as a training of the first classifier. Training of the second classifier would be added as a new part of the code. Please note that there will be no changes in the runtime part because the structure of the used features and classifiers is the same between recognition and the training of the last used classifier. Therefore the runtime process requires no changes since it is just a generic recognition processes that uses SummatorFeature as a descriptor.
The description of the code that should be added to perform training and recognition using HML will follow. This code should be added after first classifier training: 
  //Train external classifier
  //Reset the playlist to start training from the beginning
  trainPlaylist->seek(0,0);

  //Remove previously used feature and classifier from the session
  session->delFeature(feature);
  session->delClassifier(classifier);

Here we initially prepare the system for the training of the second classifier. In order to do that we have to reset the playlist back to start (sine the training of the second classifier should be performed on the same playlist). Then we remove LBT-TOP feature and first classifier from the session. This step is not really necessary and is performed just to simplify the pipeline creation (it wouldn’t be possible to use connectAll method otherwise).
 The next step is to create ClassifierFeature and configure it to use as a sub objects feature and classifier that were previously removed from the session. Please note that classifier feature is created with the parameter numberOfSeq equal to 4. This is essential since the feature should be aware that classifier was trained on the output of features yielded on 4 separate regions and therefore information from all of these regions should be received in order the internal feature could be used to produce classifier input of suitable size.
  //Create Classifier and Sumator features
  SPtr<ClassifierFeature> clFeature = Factory::createClassifierFeature(4);
  clFeature->setFeature(feature);
  clFeature->setClassifier(classifier);

Result of classifier feature could be directly evaluated by the second classifier or could be combined using Aggregator, Concatenator or Summator Features with other descriptor information. In this sample we use the SummatorFeature. SummatorFeature is similar to ConcatenatorFeature and also process more frames that are necessary to compute its subfeature . The length of the evaluated frame sequence should be configured during SummatorFeature creation. We use previously created ClassifierFeature as a subfeature of the Summator.
  SPtr<SummatorFeature> sumFeature = Factory::createSummatorFeature(SummatorFeature::Settings(4));
  sumFeature->setFeature(clFeature);

Then we have to create the second classifier and add Summator feature and the second classifier to the session
  session->addFeature(sumFeature);
  //Create SVM classifier
  SPtr<IClassifier> classifierExt = Factory::createClassifier(ivcp::SVM);
  session->addClassifier(classifierExt);

The last step is to recreate the pipeline using updated session and perform training of the second classifier. This step is similar to the training of the first classifier. 
  //Create pipeline that connects the Filters together
  trainPipeline = Factory::createPipeline(ivcp::TRAIN_PIPELINE);
  trainPipeline->setSession(session);
  //In simple cases connectAll just connects all Filters in a Session
  trainPipeline->connectAll();
  guiTrain = Factory::createGUIFramework(trainPipeline);
  trainPipeline->run();

## Converter from Weka trained models to IVCP serialized classifiers

### Overview
This separate application is used to convert saved models of Weka's trained classifiers to serialized IVCP classifiers. Currently are supported models from LibSVM v1.0.5 and LibLINEAR v1.9.0 (logistic regression only) Weka's functions (see “Updating jars” section if you need more recent versions). These two functions can be downloaded through Weka's package manager since version 3.7.2. 
### Running converter from Install package
Prerequisites:
•	You need to have installed JRE. 
In command line window change current directory to directory with installed binary files (e.g. “C:\ivcp 0.17\bin”):
“cd C:\ivcp 0.17\bin” 
To convert Weka's model to IVCP format type in command line window:
“java -jar WekaToIvcpModelConverter.jar <classifier name> <saved_weka_classifier>.model  <output_ivcp>.xml”, (e.g. “java -jar WekaToIvcpModelConverter.jar svm svm_iris.model  svm_iris.xml”)
 where:
•	<classifier name> is “svm” or “lr”.
•	<saved_weka_classifier>.model is the name of file with saved models of Weka's trained classifier.
•	<output_ivcp>.xml is the name of output file with corresponding serialized IVCP classifier.
Notes for Linux:
In terminal window change change current directory to directory with installed jar files (e.g. “~/ivcp 0.17/bin/Release”):
“cd ~/ivcp 0.17/bin/Release”
To perform convertation you should additionally pass “java.library.path” with absolute paths to folder with IVCP libraries. For example, if IVCP “.so” libraries are located in “/home/<user name>/ivcp/build/lib” folder, then you can run converter as following: 
“java -Djava.library.path=/home/<user name>/ivcp/build/lib -jar WekaToIvcpModelConverter.jar svm svm_iris.model svm_iris.xml”

### Loading converted classifier into user project
The following code snippet demonstrates the way, how serialized classifier can be used into user project (suppose “svm_iris.xml” is obtained from “WekaToIvcpModelConverter.jar”, classifier id can be obtained from serialized file, by default for svm classifier it's “svm”, for logistic regression classifier it's “lr”):
const string id = "svm";
SPtr<IRepository> repository = Factory::getRepository("svm_iris.xml");
SPtr<LRClassifier> clasifier = repository->loadClassifier(id);
// Now classifier can be used as usual
### Building converter from sources
Converter from Weka's model files to IVCP serialized classifiers builds automatically after “libwekaconverter” project (if “WITH_WEKA_CONVERTER” option is selected during “cmake” step, see “Building instruction under Windows” section for more details). Please note that converter works with release libraries only.
“WekaToIvcpModelConverter” is a Java application (jar), that deserialize Weka's model file with saved trained classifier functions, then pass parsed data through JNI into c++ library (“libwekaconverter” project),  that calls IVCP's functions to construct proper classifier and serialize it. “WekaToIvcpModelConverter.jar” is built automatically, according to Apache Ant build script (you can find it in “IVCP_BUILD_DIRECTORY\build.xml”, e.g. “C:\ivcp\build\build.xml”).
### Updating jars
If you want to convert models from more recent versions of Weka's functions, you must update jars in the folder with converter (“C:\ivcp 0.17\bin”). Weka places downloaded classifiers into “WEKA_HOME” directory, by default it is “C:\Users\<User name>\wekafiles” (you can find “WEKA_HOME” value through “Weka GUI Chooser”: “Help->SystemInfo”). 
Steps to update jars:
•	Delete in “C:\ivcp 0.17\bin” following files: “libsvm.jar”, “libsvm_wrapper.jar”, “liblinear.jar”, “liblinear_wrapper.jar”.
•	Copy necessary jars from “WEKA_HOME” to “C:\ivcp 0.17\bin”:
◦	“WEKA_HOME\packages\LibSVM\LibSVM.jar”
◦	“WEKA_HOME\packages\LibSVM\lib\libsvm.jar”
◦	“WEKA_HOME\packages\LibLINEAR\LibLINEAR.jar”
◦	“WEKA_HOME\packages\LibLINEAR\lib\liblinear-<version>.jar”
•	Rename in folder “C:\ivcp 0.17\bin”:
◦	“LibSVM.jar” → “libsvm_wrapper.jar”
◦	“LibLINEAR.jar” → “liblinear_wrapper.jar”
◦	“liblinear-<version>.jar” → “liblinear.jar”
Now “WekaToIvcpModelConverter” is able to convert models from updated functions.
### Installing LibSVM and LibLINEAR in Weka
Since v3.7.2 Weka supports “package manager”, available through “Weka GUI Chooser” (select “Tools”->”Package manager”). LibSVM and LibLINEAR can be installed through it. After installation, you should be able to find “LibSVM” and “LibLINEAR” functions: 
1.	Select “Classify” tab in “Weka GUI Chooser”
2.	Push “Choose” button in “Classifier” panel
3.	“LibSVM” and “LibLINEAR” are located in “functions” folder


## GUI Framework
GUIFramework is a simple wrapper for basic visualization of pipeline execution. Visualization depends on Pipeline mode (training, testing or real-time):
•	In train mode the GUIFramework shows a video sequence and the labels used for training of the Classifier Filter
•	In test mode the GUIFramework shows a video sequence, predicted classes and ground-truth labels
•	In real-time mode the GUIFramework shows a video sequence and a predicted class. In real-time mode user can press the ‘v’ key to start/stop recording of the video sequence
To use GUIFranework just create an object with Pipeline as a parameter and run the Pipeline:
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
pipeline->run();

In order to generate output video file (consisted of frames used during Pipeline execution) user can choose one of 2 modes:
1.	Every frame is written to the file in realtime
Use the following code for this mode:
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
visualize->enableVideoOutput(outputFilename, fourcc, frameSize, fps, framesThreshold, outputType, convertTo8bit); 
pipeline->run();
visualize->disableVideoOutput();
  	where: 
•	outputFilename – output video file name
•	fourcc – output video file codec (use CV_FOURCC('D','I','B',' ') for uncompressed video – for 12 bit stream, CV_FOURCC(‘X’,’V’,’I’,’D’) is an example of compressed video code, -1 shows a selection dialog)
•	frameSize – output video file frame size
•	fps – output video file frame rate
•	framesThreshold – frame number limit per output file (if zero then it's ignored). This will limit frame number of every output file and will imply creating several files if threshold is achieved
•	outputType - output video file type (use VIDEOFILE_OUTPUT for .avi files - by default. Or RAWFILE_OUTPUT for .raw files - for this mode the fourcc parameter is ignored)
•	convertTo8bit - whether need to save video in 8bit format (for example, we need to set this parameter to true for writing compressed video from 12bit video source)
2.	At first frames are stored to a queue and when pipeline is finished they are written to a file
Use the following code for this mode:
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
pipeline->run();
visualize->generateVideoOutput(outputFilename, fourcc, frameSize, fps, outputType);
To hide a GUI press CTRL + ALT + SHIFT + W, and to show GUI press CTRL + ALT + SHIFT + W again. 
Also you can do it with using socket command. Create a listening socket connection as follows:
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline); 
//...
visualize->listenSocket(portNumber);
Default port number is 27015. 
The GUI is hidden when the following command is received: “action=HIDEGUI”, the GUI is shown when it receives the following command: “action=SHOWGUI”.
You have an option to change some parameters without restarting the pipeline. To show the “Settings” window press CTRL + ALT + SHIFT + K and to hide it press CTRL + ALT + SHIFT + K again. Currently this is implemented for Windows OS only. 
 

To specify settings use IGUIFramework::Settings structure which have following fields:
•	sensitivity – sensitivity level number (from 0 to 10)
•	autoExposure – enable\disable auto exposure
•	exposure – manual exposure(in milliseconds from 10 to 90), when auto exposure is switched off 
•	stabThreshold – face movement threshold used to stabilize resulting frames in case there is no motion detected
•	stabFrames – number of frames that are used to check for face motion (used only once on first creation of FaceAPI handler)
•	gestures – gestures settings (enabled\disabled for every gesture, for example cheek twitch, yes, no) 
•	regions – region settings which have these fields:
o	regionExpansionWScale – width scale of the region
o	regionExpansionHScale – height scale of the region
o	regionOffsetX – region offset along X axis
o	regionOffsetY – region offset along Y axis
By using the Settings GUI you can:
•	Set ROI-based auto exposure (please read the “Ximea camera auto exposure function” section for more details) or set manual exposure value (CV_CAP_PROP_XI_AEAG_LEVEL parameter in Ximea SDK).
•	Change gesture settings (enable or disable them). In order to add gestures (gesture “yes” in the code below) to the list use the following code: 
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);

IGUIFramework::Settings settings;
settings.gestures["yes"] = true; //or false	

visualize->setGesturesSettings(settings);
•	Change region settings (offset, scale, stabframe, stabthreshold)
•	Change sensitivity (use IGUIFramework::getCurrentSensitivity function in order to read the current setting)
•	Apply performed settings in next frame (“Apply” button)
•	Back to default settings (“Default” button)
The default settings can be defined as follows: 
SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
IGUIFramework::Settings settings;
//...
visualize->setDefaultSettings(settings);
//...
pipeline->run();
Use IGUIFramework::replaceSettingsWithDefaults() function to replace the current pipeline settings with default.
•	Save all settings (current and default) to a config file
•	Load all settings (current and default) from a config file
You have two possible variants for region selection:
•	by the Settings GUI
•	by mouse click in GUI
The selected ASM, FaceAPI, Dlib or RealSense region is painted in yellow color, unselected regions are painted in blue:
 
After selection, you can change region parameters via keyboard:
•	W - Move region up
•	S - Move region down 
•	A - Move region left 
•	D - Move region right
•	H - Increase width
•	Y - Increase height
•	G - Decrease width
•	T - Decrease height
•	R - Reset tracking
You can reset FaceAPI, ASM, Dlib or RealSemse tracking if the regions look slightly off. To reset press R or push the “Reset tracking” button in the Settings GUI. 
### Ximea camera auto exposure function
The ROI – based auto exposure feature lets you specify rectangles on frame for automating camera settings adjusting to achieve the optimal exposure value (instead of static rectangles, you can also specify Region Creators). Currently this is only implemented for the XIMEA cameras.   
To specify the settings use XimeParams structure which have following fields:
•	enableAutoExposure – enable\disable ROI-based auto exposure (disabled by default)
•	autoExposureLevel – auto exposure level (from 0 to 100 percent). The higher value is set the brighter resulting image is
•	autoExposureLevelTolerance – auto exposure level tolerance (from 0 to 100 percent). Increase this value in case the auto exposure feature is activated too often
•	autoExposureSpeed – auto exposure speed (float value from 0.0f to 1.0f)
•	enableAEAG – in case the ROI-based auto exposure (enableAutoExposure) is disabled this option enables or disables full image automatic exposure/gain selection controlled natively by Ximea camera (see defaulAEAFLevel for more details). In case the option is disabled exposure and gain parameters are used. This option is enabled by default
•	defaultAEAGLevel – exposure level to be used if the auto exposure feature is disabled (floating point value from 10.f to 90.f, 40.0f is the default)
•	maxlimitAE – defines the maximum limit of exposure in AEAG procedure (for Ximea native auto exposure). Default is 33
•	maxlimitAG – defines the maximum limit of gain in AEAG procedure (for Ximea native auto exposure). Default is -1. In case auto exposure is enabled or automatic exposure/gain is disabled this parameter is ignored. If value is -1 then default value for the current camera is used
•	exposurePriority – defines exposure priority for Auto Exposure / Auto Gain function (for Ximea native auto exposure)
•	badPixCorrection – defines if correction of bad pixels is enabled or disabled
•	exposure – exposure time in microseconds (for non-automatic exposure). Default is 60000
•	gain – gain in dB (for non-automatic exposure). Default is 0

So the following flowchart illustrates logic of applying the parameters:
 

To use auto exposure in the interface of XimeaVideoSource have following methods:

typedef void (*AutoExposureStatusCallback)(bool autoExposureOn, void* context);

class XimeaVideoSource: public VideoSource
{
public:
//...
    void setParameters(const XimeaParams& params);

    XimeaParams getAutoExposureParameters();

    void setAutoExposureStatusCallback(AutoExposureStatusCallback onStatus,  void* userdata = 0);

    void setAutoExposureROI(IPipeline* pipeline, std::vector<IRegionCreator*>& regionCreator, bool pausePipelineOnAdjusting = false);

    void setAutoExposureROI(std::vector<cv::Rect>& roi, bool pausePipelineOnAdjusting = false);

    void setAutoExposureROI(std::vector<cv::Rect>& roi, cv::Mat roiRotation, bool pausePipelineOnAdjusting = false);

//...
};

Method setAutoExposureStatusCallback allows to set callback function that is called each time when status of auto exposure changes. Callback function must have the same signature as AutoExposureStatusCallback type. Method setAutoExposureROI allows setting auto exposure ROI on frame (with additional rotation matrix) or specify region creator from pipeline (see “Auto exposure demo” section for more details). This method have three overloads and an additional Boolean parameter pausePipelineOnAdjusting (by default this parameter is false). If this flag is true then the Pipeline makes a pause when auto exposure is adjusting. In this way only correctly exposed frames will be transmitted to a classifier. This may increase quality of classifier results, but can add some performance overhead.
The typical usage of the auto exposure is the following:
    //Create Ximea camera 
    SPtr<XimeaVideoSource> camera = Factory::createXimeaVideoSource(0, XimeaAutoExposureParams(), "camera");

    //Configure auto exposure
    XimeaParams parameters;
    parameters.enableAutoExposure = true;
    parameters.autoExposureLevel = 50;
    parameters.autoExposureLevelTolerance = 10;
    parameters.autoExposureSpeed = 0.007;
    camera->setParameters(parameters);

    //Create vector to store auto exposure roi
    vector<Rect> roi;
    roi.push_back(Rect(0,0,100,100));
    camera->setAutoExposureROI(roi);

### Auto exposure demo

#### Overview
This demo shows a simple scenario of using auto exposure adjusting with specified Region Creator. In order to see how auto exposure works we create a simple pipeline with a Video Source and a single Region Creator. When create XimeaVideoSource we should set necessary auto exposure parameters. Then, it creates BBoxRegionCreator with rectangle with position on the frame center. By using setAutoExposureROI method set BBoxRegionCreator for use with auto exposure. The demo uses GUI Framework to visualize the pipeline. 
#### Source code
// Copyright (C) 2013, Intel Corporation, all rights reserved.
#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

int main(int argc, char **argv)
{
    (void)argc;
    (void)argv;

    ivcpInit();

    SPtr<XimeaVideoSource> camera = Factory::createXimeaVideoSource(0, XimeaAutoExposureParams(), "camera");

    XimeaAutoExposureParams params;

    //Enable auto exposure for camera
    params.enableAutoExposure = true;
    camera->setAutoExposureParameters(params);

    camera->openCamera(0);

    SPtr<VideoPlaylist> playlist = Factory::createVideoPlayer()->getPlaylist();
    FrameRange videorange;
    playlist->addSourceRange(camera, videorange);

    SPtr<Session> session = Factory::createSession("session");
    session->setVideoPlaylist(playlist);

    Size output_resolution = camera->getOutputResolution();
    SPtr<IRegionCreator> bb_rc = Factory::createBBoxRegionCreator(
	Rect(output_resolution.width/4, output_resolution.height/4, 		output_resolution.width/2, output_resolution.height/2), "bb_rc");
    session->addRegionCreator(bb_rc);
    
    //Create simple pipeline with one link from video source to one region creator
    SPtr<Pipeline> pipeline = Factory::createPipeline(RUNTIME_PIPELINE);
    pipeline->setSession(session);
    pipeline->addLink("player", "bb_rc");

    SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
    visualize->setWindowName("Auto exposure runtime demo (for XIMEA camera)");
    
    //Set auto exposure ROI from black box region creator
    vector<IRegionCreator*> region_creators;
    region_creators.push_back(bb_rc);
    source->setAutoExposureROI(pipeline, region_creators, true);

    pipeline->run();

    return 0;
}
## Marking applications

### Ivcp_region_markup application

#### Overview
GTRegionCreator extracts regions based on pre-created marking data. The data is stored in XML format (see the “Format of the XML file” section below for more details) that can be created manually or with using the marking application. It is important to note that the marking data is associated with a Video playlist object (not a Video source) which can contain multiple Video sources. 
The region marking application allows marking regions for frames in Video playlist.  This application loads a Video playlist object from input repository (for example, see res/region_markup_in.xml) and saves the region data source in output repository.   The paths to input and output repositories should be passed as the command-line options: 
ivcp_region_markup [--normalize] input.xml output.xml

Input repository should contain Video Playlist and Video Sources for playlist.
In case --normalize command line option is also provided all playlist frames will be processed using FaceAPI to estimate frame transformations necessary to make face images frontal. It will be possible to select this transformation during markup process. Please note that frames that already were marked in input repository will be displayed with saved transformation by default (used transformation could be changed during markup process) regardless of the –normalize option. In case currently displayed frame was not marked previously it would be displayed with FaceAPI provided transformation if --normalize option is used and without transformation otherwise.
Use the following keys for marking:
•	Left / Right – move to the previous/next video frame;
•	0-9 – selection class number for the next region begin marked;
•	Del + 0-9 – remove region with selected class number from the current frame;
•	n – change transformation used to display the frame. (Transformation will be circularly changed among original transformation already saved for the frame, transformation estimated using FaceAPI (if --normalize flag is provided) and original frame without transformation)
•	Esc – exit from application with saving region data source to the output repository.
To mark a new region, you should choose class number (press corresponding key) and select region of interest in the frame by the mouse. Every class have own color for drawing of region rectangle (0 – red, 1 – green, 2 – blue, 3 – yellow, 4 – pink, 5 – light blue, 6 – brown, 7 – violet, 8 – white, 9 – black). Transformation used during marking of the last region is saved as a transformation of the frame (even if all other regions were marked in different transformation mode).
#### Format of the XML file
Output repository contains region data source, corresponding video playlist and video sources. Region data source is saved as Filter with id “gt_regions” and has the following structure:
<_>
 <id>gt_regions</id> 
 <type>65537</type> 
 <internals>
   <frames>
 <!-- List of marked frames --> 
 <_> <!-- Description of the first marked frame --> 
  <sourceindex>0</sourceindex> <!-- Index of video source in video playlist --> 
  <frameindex>0</frameindex> <!-- Index of frame in video source →
  <transformation type_id="opencv-matrix"> <!-- Frame transformation matrix -->
    <rows>1</rows> <!-- Should be 3*3 transformation matrix -->
    <cols>9</cols> <!-- stored row by row as a 1 row and 9 columns matrix -->
    <dt>d</dt>
    <data>
      9. -1. 5. 2. 8. -1. 8. -2. 1.</data>
  </transformation>  <!-- End of frame transformation matrix -->
  <regions> <!-- List of regions of this frame --> 
    <_> <!-- Description of the first region --> 
      <class>1</class> <!-- Class of the current region --> 
      <x>100</x> <!-- x-coordinate of top-left corner --> 
      <y>100</y> <!-- y-coordinate of top-left corner--> 
      <width>300</width> <!-- Width of the region --> 
      <height>400</height> <!-- Height of region --> 
    </_> <!-- End of description of the first region --> 
    <!-- Descriptions of the rest regions of this frame --> 
    </regions>
  </_> <!-- End of description of the first marked frame --> 
  <!-- Descriptions of other marked frames --> 
  </frames> <!-- End of list of marked frames --> 
  <id>gt_regions</id> 
 </internals>
</_>
### Ivcp_markup application

#### Overview
Classification Result Markup application is intended to produce files containing information about per frame “ground truth” classification results for a given Video playlist. This application loads a Video player object from input repository (for example, see res/session_markup.xml) and saves produced frame classification marks in output repository.  The paths to input and output repositories should be passed as the command-line options: 
ivcp_markup input.xml output.xml

Input repository shall contain Video Playlist and Video Sources for its playlist. 
Application will show the first frame from the playlist after start up.  At the top of displayed frame you could see its current and new marks. Current frame mark is displayed as class number or "Unmarked" if the frame isn't marked yet. New frame mark is displayed as "None" while no new information is provided and is displayed as class number when you provide new class number or "Unmarked" when you state you want to reset class mark for this frame.
You could navigate between frames using key "Right" to go to the next frame and "Left" to go to the previous frame. If there is no next/previous frame in the playlist you will stay on the same frame.  To provide new class mark for displayed frame you should type corresponding number using numeric keys. You could clear last entered frame mark digit by pressing Backspace. You could clear frame mark completely by pressing "c" (it changes new frame mark back to "None"). In case you want to reset frame mark for this frame you should type "u".
Provided frame class mark will be stored (or cleared if you state you want it to reset to "Unmarked") automatically as soon as you navigate to other frame. Frame class mark will not be changed while new frame mark is displayed as "None". In this case it is considered that no new information is provided and frame class mark state should stay intact.
When you accomplish frame markup you should press Esc to export markup information from internal memory structures to markup file and exit from application.
#### Format of output XML file
Output file contains frame markup as a sequence of similarly marked frame ranges per source and has the following structure:
<_>
 <id>class_marks</id> 
 <type>65536</type> 
 <internals>
  <sourceranges>
   <!-- List of marked ranges --> 
   <_> <!-- Markup for the first video source --> 
    <sourceindex>0</sourceindex> <!-- Index of video source in playlist -->
    <rangemarks> <!-- Description of marked ranges for this video source -->
      <_> <!-- Description of the first range --> 
        <rangestart>0</rangestart> <!-- Start of the current range --> 
        <rangeend>15</rangeend> <!-- End of the current range --> 
        <rangeclass>0</rangeclass> <!-- Class of the current range --> 
      </_> <!-- End of description of the first range --> 
      <!-- Descriptions of the rest ranges of this video source --> 
     </rangemarks>
    </_> <!-- End of description of marked ranges for the first video source --> 
    <!-- Descriptions of other video sources --> 
  </sourceranges> <!-- End of list of marked ranges --> 
  <id>class_marks</id> 
 </internals>
</_>
## Adding new Filters
The IVCP library provides an infrastructure for building your classification pipeline based on a set of existing Filters or your own Filters. In case you want to add a new Filter (for example a new Feature or a new Classifer) here is a list of steps to integrate the new Filter:
•	Derive your class from one of existing abstract classes of corresponding root type (i.e. Feature, RegionCreator, Classifier, Performance, Classifier Result Data Source or Region Data Source)
•	Implement the abstract methods declared in the parent class (all methods necessary to implement will be shown by your compiler on first build attempt)
•	In case you’re planning to store your new Filter in a Repository add a new type of the enumeration returned by getType method to file types.hpp (for example in case of Feature you need to declare a new type of FeatureType enumeration). In case you are not planning to use Repository just use a pre-defined enumeration values (i.e. NEW_FEATURE, NEW_VIDEO_SOURCE, NEW_REGION_CREATOR etc.)
•	Please note that you shall add new enumeration values to the end of enumeration lists in order to keep compatibility with existing repositories
•	Please note that pipeline shares the same input data between all filters of the same level (usually that would be the filters with the same root type). So you have to retain input of your filter intact. Modification of input data could lead to incorrect input for all other filters and therefore invalidate the pipeline evaluation result.
•	In case you cannot modify source code of the library just use your own unique value and convert it to enumeration in getType function (for example, (FeatureType)10)
•	(Optional) Add a method for creating the Filter in Factory class (for example, see factory.hpp and factory.cpp) 
•	If you want to have possibility to save/load Filter it is necessary to implement a Serializer for you Filter – a special class responsible for creating a XML representation of an object and vice versa: creating an object from XML representation
In order to implement a Serializer for your filter you shall create a class derived from XMLSerializer class and implement its interface:
Public Member Functions	
		
virtual SPtr< Filter >
load (cv::FileStorage &fs, cv::FileNode fn, SPtr< Filter > filter=NULL)=0 throw (IOException, IncorrectParamException)

virtual void 	save (SPtr< Filter > filter, cv::FileStorage &fs)=0 throw (IOException, CastException, NoDataException)

 	Save filter to XML file. 	
virtual FilterState 
loadMap (cv::FileStorage &fs, cv::FileNode fn)=0 throw (IOException)

 	Load an auxiliary structure that fully describes state of a Filter. 

 	
virtual void 	saveMap (FilterState &filterData, cv::FileStorage &fs)=0 throw (IOException)

 	Saves the XML description of a Filter based on filterData auxiliary structure loaded by the loadMap function. 


Protected Member Functions
virtual FilterState 
createMap (SPtr< Filter > filter)=0 throw (NoDataException)

 	Create a structure that describes the Filter identical to the structure generated by loadMap function. 

Please check the doxygen generated documentation for more details on the input, output parameters of the Serializer functions and the implementation recommendations. 
Please note that in case you derive from an existing IVCP class (Feature for example) it is more likely that there is already an existing Serializer implemented for that class. Then it is recommended to derive from the existing Serializer (FeatureXMLSerializer for example) and use existing code to serialize members of the base class. 
The Serializers can be registered in Repository with using XMLRepository::registerSerializer function. All existing Serializers are registered in ivcpInit function. But user’s Serializers can be registered anywhere in the code.


## Adding new unit tests
For creating new unit tests the framework Google Test is used (for more information on using this testing framework see http://code.google.com/p/googletest/wiki). All unit tests are contained in the project ivcp_test. Initialization of the test environment occurs in the file test_main.cpp (see class TestEnvironment). Before running all tests the function SetUp() is called. Then the tests are run if there was no fatal failures. And finally TearDown() is called. Now only the call of ivcpInit() occurs in the function SetUp() . You can set additional actions in SetUp and TearDown functions.
To create a test, use the TEST(test_case_name, test_name) macro to define and name a test function. The first argument is the name of the test case, the second argument is the test’s name within the test case. The full name of the test function will be test_case_name.test_name. You can use the full name to run single or several tests by using --gtest_Filter command line option. 
In the test function you can use the various Google Test assertions to check values and conditions. For example: EXPECT_EQ(expected, actual) verifies that actual value is equal to expected value, EXPECT_TRUE(condition) verifies that condition is true. For more information about other assertions see  Google Test documentation.
In test_environment.hpp the various paths to the temporary files (these are in the working directory, for example “./pipeline_tmp.xml”) and files with test data (these are in the directory res, for example “res/session.xml”) are defined. The first are used for writing any data, the second are used for reading the test data. It is recommended to delete the temporary files in the test function before using them.
If you create new file with unit tests you should include “gtest/gtest.h”, “test_environment.hpp” and “ivcp.hpp”, namespaces cv and ivcp for correct compilation and work of the tests. Possibly it will be necessary to include the header files that are missing in ivcp.hpp.
