///////////////////////////////////////////////////////////////////////////
// <copyright file="ipipeline_node.hpp" company="Intel Corporation">
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



#ifndef __IPIPELINE_NODE_HPP__
#define __IPIPELINE_NODE_HPP__

#include <string>
#include <list>
#include "libivcp/performance/iperformance.hpp"
#include "libpipeline/pipeline_types.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"
#include "libivcp/ipipeline.hpp"
/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file ipipeline_node.hpp
 * \brief Declaration of pipeline filter interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic interface a pipeline node
 * \details The Pipeline node component is responsible for receiving information from previous levels of pipeline
 * processing of this information via incorporated filter object and sending results to the next level of pipeline.
 */
class IVCP_EXPORT IPipelineNode
{
public:
    /*!
     * \brief Destructor
     */
    virtual ~IPipelineNode() {};

    /*!
     * \brief Init incorporated filter and perform necessary preprocessing steps
     * \throw IncorrectParamException There are missing or incorrect parameters necessary for initialization
     */
    virtual void init() = 0;

    /*!
     * \brief Run incorporated filter to process stored information
     * \throw IncorrectParamException There are missing or incorrect parameters found
     */
    virtual void run() = 0;

    /*!
     * \brief Perform necessary postprocessing steps and finalize incorporated filter
     * \throw IncorrectParamException There are missing or incorrect parameters for finalize
     */
    virtual void finalize() = 0;

    /*!
     * \brief Receive and store information message from previous level of pipeline
     * \param[in] message received information message
     * \param[in] caller pipeline filter ID of the message sender
     */
    virtual void transmitMessage(cv::Mat message, std::string caller) = 0;

    /*!
     * \brief Add a link to higher level pipeline filter
     * \details Pipeline filter should work in accordance with pipeline configuration. It should check whether received messages are produced by intended source filters.
     * Therefore pipeline filter should be notified about these sources prior to message receiption.
     * \param[in] from pipeline filter ID of higher level filter
     */
    virtual void addSourceLink(std::string from) = 0;

    /*!
     * \brief Add a link to lower level pipeline filter
     * \details Pipeline filter should send produced results to lower level filters upon incoming information evaluation.
     * Therefore pipeline filter should be notified about these destinations.
     * \param[in] to pointer to lower level pipeline filter
     */
    virtual void addDestinationLink(SPtr<IPipelineNode > to) = 0;

    /*!
     * \brief Remove a link to higher level pipeline filter
     * \param[in] from pipeline filter ID of higher level filter
     * \sa addSourceLink
     */
    virtual void removeSourceLink(std::string from) = 0;

    /*!
     * \brief Remove a link to lower level pipeline filter
     * \param[in] to pointer to lower level pipeline filter
     * \sa addDestinationLink
     */
    virtual void removeDestinationLink(SPtr<IPipelineNode > to) = 0;

    /*!
     * \brief Set a link to performance filter
     * \details Pipeline node could be linked to single performance filter.
     * \param[in] performance pointer to performance filter
     */
    virtual void setPerformance(SPtr<IPerformance> performance) = 0;

    /*!
     * \brief Get list of links to connected lower level pipeline filters
     * \return List of references to connected lower level filters
     */
    virtual std::list<SPtr<IPipelineNode> > getDestinations() = 0;

    /*!
     * \brief Get ID of the pipeline filter
     * \return pipeline filter ID
     */
    virtual std::string getId() const = 0;

    /*!
     * \brief Get type of the pipeline filter(pipeline level)
     * \return pipeline filter type value
     */
    virtual NodeType getType() = 0;

    /*!
     * \brief Check whether enough information is provided to pipeline filter to perform evaluation
     * \return true in case enough information is provided for evaluation, false otherwise
     */
    virtual bool isReady() = 0;

    /*!
     * \brief Add a debugging callback
     * \param[in] func Pointer to callback function
     * \param[in] context Pointer to context data for callback function
     */
    virtual void addCallback(PipelineCallback func, void* context) = 0;

    /*!
     * \brief Set active pipeline
     * \param[in] pipeline Pointer to current pipeline
     */
    virtual void setPipeline(IPipeline* pipeline) = 0;

    /*!
     * \brief Set execution prority for the filter of the current node
     * \param[in] prior Priority to set (ivcp::IPipeline::LOW, ivcp::IPipeline::NORMAL, ivcp::IPipeline::HIGH)
     * \throw NotImplementedException This method could be used only for parallel pipeline nodes
     */
    virtual void setPriority(IPipeline::ProcessingPriority prior) = 0;

protected:
private:

};
};

/** @}*/
#endif 
