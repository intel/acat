////////////////////////////////////////////////////////////////////////////
// <copyright file="data_source_factory.cpp" company="Intel Corporation">
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

#include "libinfra/data_source/data_source_factory.hpp"
#include "libinfra/data_source/classifier_result_data_source.hpp"
#include "libinfra/data_source/gt_region_data_source.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
SPtr<IClassifierResultDataSource> DataSourceFactory::getClassifierResultDataSource(string id)
{
    SPtr<IClassifierResultDataSource> classResult = new ClassifierResultDataSource();
    if (id.empty())
    {
        id = "class_res";
    }

    classResult->setId(id);

    return classResult;
}

SPtr<IRegionDataSource> DataSourceFactory::getRegionDataSource(string id)
{
    SPtr<IRegionDataSource> regionSource = new GTRegionDataSource();
    if (id.empty())
    {
        id = "gt_regions";
    }

    regionSource->setId(id);

    return regionSource;
}

}
