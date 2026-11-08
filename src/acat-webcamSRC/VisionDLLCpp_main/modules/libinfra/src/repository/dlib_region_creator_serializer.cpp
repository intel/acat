///////////////////////////////////////////////////////////////////////////
// <copyright file="dlib_region_creator_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/dlib_region_creator_serializer.hpp"
#include "libinfra/dlib.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
DlibRegionCreatorXMLSerializer::DlibRegionCreatorXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

DlibRegionCreatorXMLSerializer::~DlibRegionCreatorXMLSerializer()
{
}

SPtr<Filter> DlibRegionCreatorXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto filterMap = loadMap(fs, fn);

    DlibRegionCreator::Settings settings;
    settings.typeToSearch = (RegionType)(filterMap["region_type"].ival);
    settings.regionExpansionWScale = filterMap["region_w_scale"].fval;
    settings.regionExpansionHScale = filterMap["region_h_scale"].fval;
    settings.normalize = (filterMap["region_normalize"].ival != 0);
    settings.stabThreshold = filterMap["handler_stab_threshold"].fval;
    settings.stabFrames = filterMap["handler_stab_frames"].ival;
    settings.regionOffsetX = filterMap["offset_x"].ival;
    settings.regionOffsetY = filterMap["offset_y"].ival;
    settings.normFrames = filterMap["handler_normframes"].ival;
    settings.noFaceFrames = filterMap["handler_nofaceframes"].ival;
    settings.noFaceClockwiseRotation = (filterMap["handler_clockwise"].ival != 0);
    settings.glassesFixEnabled = (filterMap["handler_glassesfix"].ival != 0);
    settings.useTemplateRegions = (filterMap["handler_template"].ival != 0);
    settings.glassesFixStrength = filterMap["handler_glasses_strength"].fval;
    settings.useAutoGlassesSearch = (filterMap["handler_glasses_search"].ival != 0);
    settings.cutBackground = (filterMap["handler_cut_background"].ival != 0);
    settings.resizeDownscale = filterMap["handler_resize_downscale"].dval;

    SPtr<DlibRegionCreator> regionCreator = filter;
    if (regionCreator == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to DlibRegionCreator");
        }
        regionCreator = Factory::createDlibRegionCreator(settings, "", Factory::getCachedDlib(settings.stabThreshold, settings.stabFrames, settings.normFrames,
            settings.noFaceFrames, settings.noFaceClockwiseRotation, settings.glassesFixEnabled, settings.useTemplateRegions, settings.glassesFixStrength,
            settings.useAutoGlassesSearch, settings.cutBackground, settings.resizeDownscale));
        filter = regionCreator;
    }
    else
    {
        regionCreator->setSearchRegionSettings(settings);
    }

    FilterXMLSerializer::load(fs, fn, regionCreator);

    return regionCreator;
}

map<string, IXMLSerializer::SerializeData> DlibRegionCreatorXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    int typeToSearch = 0;
    float regionExpansionWScale = 0;
    float regionExpansionHScale = 0;
    bool normalize = false;
    float stabThreshold = 0.0f;
    int stabFrames = 10;
    int normFrames = -1;
    int noFaceFrames = -1;
    bool noFaceClockwiseRotation = false;
    bool glassesFixEnabled = false;
    bool useTemplateRegions = false;
    float glassesFixStrength = 0.0f;
    bool useAutoGlassesSearch = false;
    int offsetX = 0;
    int offsetY = 0;
    bool cutBackground = false;
    double resizeDownscale = 1.0;

    if (!fn["region_type"].empty())
    {
        fn["region_type"] >> typeToSearch;
    }
    if (!fn["region_w_scale"].empty())
    {
        fn["region_w_scale"] >> regionExpansionWScale;
    }
    if (!fn["region_h_scale"].empty())
    {
        fn["region_h_scale"] >> regionExpansionHScale;
    }
    if (!fn["region_normalize"].empty())
    {
        fn["region_normalize"] >> normalize;
    }
    if (!fn["handler_stab_threshold"].empty())
    {
        fn["handler_stab_threshold"] >> stabThreshold;
    }
    if (!fn["handler_stab_frames"].empty())
    {
        fn["handler_stab_frames"] >> stabFrames;
    }
    if (!fn["offset_x"].empty())
    {
        fn["offset_x"] >> offsetX;
    }
    if (!fn["offset_y"].empty())
    {
        fn["offset_y"] >> offsetY;
    }
    if (!fn["handler_normframes"].empty())
    {
        fn["handler_normframes"] >> normFrames;
    }
    if (!fn["handler_nofaceframes"].empty())
    {
        fn["handler_nofaceframes"] >> noFaceFrames;
    }
    if (!fn["handler_clockwise"].empty())
    {
        fn["handler_clockwise"] >> noFaceClockwiseRotation;
    }
    if (!fn["handler_glassesfix"].empty())
    {
        fn["handler_glassesfix"] >> glassesFixEnabled;
    }
    if (!fn["handler_template"].empty())
    {
        fn["handler_template"] >> useTemplateRegions;
    }
    if (!fn["handler_glasses_strength"].empty())
    {
        fn["handler_glasses_strength"] >> glassesFixStrength;
    }
    if (!fn["handler_glasses_search"].empty())
    {
        fn["handler_glasses_search"] >> useAutoGlassesSearch;
    }
    if (!fn["handler_cut_background"].empty())
    {
        fn["handler_cut_background"] >> cutBackground;
    }
    if (!fn["handler_resize_downscale"].empty())
    {
        fn["handler_resize_downscale"] >> resizeDownscale;
    }

    filterMap["region_type"] = typeToSearch;
    filterMap["region_w_scale"] = regionExpansionWScale;
    filterMap["region_h_scale"] = regionExpansionHScale;
    filterMap["region_normalize"] = normalize;
    filterMap["handler_stab_threshold"] = stabThreshold;
    filterMap["handler_stab_frames"] = stabFrames;
    filterMap["offset_x"] = offsetX;
    filterMap["offset_y"] = offsetY;
    filterMap["handler_normframes"] = normFrames;
    filterMap["handler_nofaceframes"] = noFaceFrames;
    filterMap["handler_clockwise"] = noFaceClockwiseRotation;
    filterMap["handler_glassesfix"] = glassesFixEnabled;
    filterMap["handler_template"] = useTemplateRegions;
    filterMap["handler_glasses_strength"] = glassesFixStrength;
    filterMap["handler_glasses_search"] = useAutoGlassesSearch;
    filterMap["handler_cut_background"] = cutBackground;
    filterMap["handler_resize_downscale"] = resizeDownscale;

    return filterMap;
}

void DlibRegionCreatorXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<DlibRegionCreator> regionCreator = filter;
    if (NULL == regionCreator)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to DlibRegionCreator for serialization");
    }

    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);
}

void DlibRegionCreatorXMLSerializer::saveMap(map<string, SerializeData> &filterData, FileStorage &fs)
{
    fs << "region_type" << filterData["region_type"].ival;
    fs << "region_w_scale" << filterData["region_w_scale"].fval;
    fs << "region_h_scale" << filterData["region_h_scale"].fval;
    fs << "region_normalize" << filterData["region_normalize"].ival;
    fs << "handler_stab_threshold" << filterData["handler_stab_threshold"].fval;
    fs << "handler_stab_frames" << filterData["handler_stab_frames"].ival;
    fs << "offset_x" << filterData["offset_x"].ival;
    fs << "offset_y" << filterData["offset_y"].ival;
    fs << "handler_normframes" << filterData["handler_normframes"].ival;
    fs << "handler_nofaceframes" << filterData["handler_nofaceframes"].ival;
    fs << "handler_clockwise" << filterData["handler_clockwise"].ival;
    fs << "handler_glassesfix" << filterData["handler_glassesfix"].ival;
    fs << "handler_template" << filterData["handler_template"].ival;
    fs << "handler_glasses_strength" << filterData["handler_glasses_strength"].fval;
    fs << "handler_glasses_search" << filterData["handler_glasses_search"].ival;
    fs << "handler_cut_background" << filterData["handler_cut_background"].ival;
    fs << "handler_resize_downscale" << filterData["handler_resize_downscale"].dval;

    map<string, SerializeData> filterDataCopy = filterData;
    filterDataCopy.erase("region_type");
    filterDataCopy.erase("region_w_scale");
    filterDataCopy.erase("region_h_scale");
    filterDataCopy.erase("region_normalize");
    filterDataCopy.erase("handler_stab_threshold");
    filterDataCopy.erase("handler_stab_frames");
    filterDataCopy.erase("offset_x");
    filterDataCopy.erase("offset_y");
    filterDataCopy.erase("handler_normframes");
    filterDataCopy.erase("handler_nofaceframes");
    filterDataCopy.erase("handler_clockwise");
    filterDataCopy.erase("handler_glassesfix");
    filterDataCopy.erase("handler_template");
    filterDataCopy.erase("handler_glasses_strength");
    filterDataCopy.erase("handler_glasses_search");
    filterDataCopy.erase("handler_cut_background");
    filterDataCopy.erase("handler_resize_downscale");

    FilterXMLSerializer::saveMap(filterDataCopy, fs);
}

map<string, IXMLSerializer::SerializeData> DlibRegionCreatorXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<DlibRegionCreator> regionCreator = filter;
    if (regionCreator == NULL)
    {
        IVCP_LOG(LOG_WARNING, "regionCreator is null");
        return filterMap;
    }

    DlibRegionCreator::Settings settings = regionCreator->getSearchRegionSettings();
    filterMap["region_type"] = (int)settings.typeToSearch;
    filterMap["region_w_scale"] = settings.regionExpansionWScale;
    filterMap["region_h_scale"] = settings.regionExpansionHScale;
    filterMap["region_normalize"] = settings.normalize;
    filterMap["handler_stab_threshold"] = settings.stabThreshold;
    filterMap["handler_stab_frames"] = (int)settings.stabFrames;
    filterMap["offset_x"] = settings.regionOffsetX;
    filterMap["offset_y"] = settings.regionOffsetY;
    filterMap["handler_normframes"] = settings.normFrames;
    filterMap["handler_nofaceframes"] = settings.noFaceFrames;
    filterMap["handler_clockwise"] = (int)settings.noFaceClockwiseRotation;
    filterMap["handler_glassesfix"] = (int)settings.glassesFixEnabled;
    filterMap["handler_template"] = (int)settings.useTemplateRegions;
    filterMap["handler_glasses_strength"] = settings.glassesFixStrength;
    filterMap["handler_glasses_search"] = (int)settings.useAutoGlassesSearch;
    filterMap["handler_cut_background"] = (int)settings.cutBackground;
    filterMap["handler_resize_downscale"] = settings.resizeDownscale;
    return filterMap;
}

}
