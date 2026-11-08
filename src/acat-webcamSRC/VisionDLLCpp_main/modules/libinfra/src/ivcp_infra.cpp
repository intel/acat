///////////////////////////////////////////////////////////////////////////
// <copyright file="ivcp_infra.cpp" company="Intel Corporation">
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
#include "libinfra/ivcp_infra.hpp"
#include "libinfra/repository/dlib_factory.hpp"
#include "libinfra/repository/realsense_factory.hpp"
#include "libinfra/repository/xml_repository.hpp"
#include "libinfra/repository/repository_factory.hpp"
#include "libinfra/data_source/data_source_factory.hpp"
#include "libpipeline/pipeline_factory.hpp"

namespace ivcp
{
IVCP_EXPORT void ivcpInit()
{
    static bool init = false;
    if (!init)
    {
        Factory::setRepositoryFactory(new RepositoryFactory);
        Factory::setDataSourceFactory(new DataSourceFactory);
        Factory::setPipelineFactory(new PipelineFactory);
        Factory::setDlibFactory(new DlibFactory);
        Factory::setRealSenseFactory(new RealSenseFactory);
        init = true;
    }
}

}
