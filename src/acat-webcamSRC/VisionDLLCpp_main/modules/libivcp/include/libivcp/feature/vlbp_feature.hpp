///////////////////////////////////////////////////////////////////////////
// <copyright file="vlbp_feature.hpp" company="Intel Corporation">
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

#ifndef __FEATURE_VLBP_HPP__
#define __FEATURE_VLBP_HPP__

#include <cstdint>
#include "libivcp/feature/feature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief LBP-TOP feature implementation
 * \details Read "Dynamic Texture Recognition Using Local Binary Patterns with an Application to Facial Expressions"
 * by Guoying Zhao and Matti Pietikainen for more details
 * @{
 */

namespace ivcp
{
/*!
* \brief LBP-TOP feature implementation
* \details Histogram of VLBP features in 3 ortogonal planes XY, XT, YT.
* Read "Dynamic Texture Recognition Using Local Binary Patterns with an Application to Facial Expressions"
* by Guoying Zhao and Matti Pietikainen for more details
*/
class IVCP_EXPORT VLBPFeature: public Feature
{
public:
    /*!
    * \brief VLBPFeature settings structure
    */
    struct Settings
    {
    public:
        /*!
        * \brief Constructor
        */
        Settings(int _sizeT = 3,
            int _radiusX = 1, int _radiusY = 1, int _radiusT = 1,
            int _neighbours = 4,
            int _gridX = 1, int _gridY = 1, int _gridT = 1,
            bool _normalizeFeature = true, bool _useRotateInvariant = false, bool _useUniformPatterns = false)
        {
            sizeT = _sizeT;
            radiusX = _radiusX;
            radiusY = _radiusY;
            radiusT = _radiusT;
            neighbours = _neighbours;
            gridX = _gridX;
            gridY = _gridY;
            gridT = _gridT;
            normalizeFeature = _normalizeFeature;
            useRotateInvariant = _useRotateInvariant;
            useUniformPatterns = _useUniformPatterns;
        }

        /*!
        * \brief Number of time slices (equal to number of regions)
        */
        int sizeT;

        /*!
        * \brief Radius of an ellipse on X axis
        */
        int radiusX;
        /*!
        * \brief Radius of an ellipse on Y axis
        */
        int radiusY;
        /*!
        * \brief Radius of an ellipse on T axis
        */
        int radiusT;

        /*!
        * \brief Number of neighbours on image plane
        */
        int neighbours;

        /*!
        * \brief Number of grid cells on X axis
        * \details The XYT space is divided into equal sized cells. Histograms for each cell are concatenated together
        */
        int gridX;
        /*!
        * \brief Number of grid cells on Y axis
        * \details The XYT space is divided into equal sized cells. Histograms for each cell are concatenated together
        */
        int gridY;
        /*!
        * \brief Number of grid cells on T axis
        * \details The XYT space is divided into equal sized cells. Histograms for each cell are concatenated together
        */
        int gridT;

        /*!
        * \brief Whether or not to normalize the histograms by volume (X x Y x T)
        */
        bool normalizeFeature;

        /*!
        * \brief Whether or not to make the LBP features rotation invariat
        * \details If true the LBP-TOP features are rotated in 3 planes and the minimal LBP code is selected (for each plane separately)
        */
        bool useRotateInvariant;

        /*!
        * \brief Whether or not to use only uniform patterns
        * \details The pattern is uniform if there is no more than 2 transitions between 0 and 1 and vice versa
        * in cyclic pattern
        */
        bool useUniformPatterns;
    };

    /*!
    * \brief Constructor
    */
    VLBPFeature(VLBPFeature::Settings settings = VLBPFeature::Settings());

    /*!
    * \brief Destructor
    */
    virtual ~VLBPFeature();

    /*!
    * \brief The function calculates LBP-TOP feature for given region sequence
    * \param[in] regSeqs Sequence of regions (number of sequences is expected to be equal to 1)
    * \return Feature
    */
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);

    /*!
    * \brief Get type of this feature
    * \return STLBP value - feature type
    */
    virtual FeatureType getType();

    /*!
    * \brief Get copy of Settings structure this object was created with
    * \return Settings this object was created with
    */
    virtual Settings getSettings();

    /*!
    * \brief Update settings of the STLBP feature
    * \param[in] settings new Settings 
    */
    virtual void setSettings(Settings settings);

protected:
    /*!
    * \brief Make features rotation invariant
    */
    void makeRotateInvatiant(std::vector<cv::Mat> &lbp, 
        long unsigned int matSizes[3]);

    /*!
    * \brief Make features uniform
    */
    void makeUniform(std::vector<cv::Mat> &lbp, 
        long unsigned int matSizes[3]);

    /*!
    * \brief Make features as a whole rotation invariant
    */
    void makeWholeRotateInvatiant(std::vector<cv::Mat> &lbp, 
        long unsigned int matSizes[3]);

    /*!
    * \brief Calculate resulting histogram
    */
    cv::Mat calculateResultHist(const RegionSequence &regSeq, 
        std::vector<cv::Mat> &lbpXY);

    /*!
    * \brief Radius of an ellipse on X axis
    */
    int radiusX;
    /*!
    * \brief Radius of an ellipse on Y axis
    */
    int radiusY;
    /*!
    * \brief Radius of an ellipse on T axis
    */
    int radiusT;

    /*!
    * \brief Number of neighbours on XY plane
    */
    int neighbours;
    /*!
    * \brief Overall number of neighbours
    */
    int neighboursAll;

    /*!
    * \brief Number of grid cells on X axis
    * \details The XYT space is divided into equal sized cells. Histograms for each cell are concatenated together
    */
    int gridX;
    /*!
    * \brief Number of grid cells on Y axis
    * \details The XYT space is divided into equal sized cells. Histograms for each cell are concatenated together
    */
    int gridY;
    /*!
    * \brief Number of grid cells on T axis
    * \details The XYT space is divided into equal sized cells. Histograms for each cell are concatenated together
    */
    int gridT;

    /*!
    * \brief Whether or not to normalize the histograms by volume (X x Y x T)
    */
    bool normalize;

    /*!
    * \brief Whether or not to make the features rotation invariat
    * \details If true the LBP-TOP features are rotated in 3 planes and the minimal LBP code is selected (for each plane separately)
    */
    bool rotateInvariant;
    /*!
    * \brief Whether or not to use only uniform patterns
    * \details The pattern is uniform if there is no more than 2 transitions between 0 and 1 and vice versa
    * in cyclic pattern
    */
    bool uniformPatterns;

    /*!
    * \brief Copy of Settings structure this object was created with
    */
    Settings settingsCopy;
private:
    //precalculated parameters
    int32_t numPatterns;
    int32_t numPatternsAllPlanes;
    int32_t numResultPatterns;

    /*!
     * \brief Calculate LBP features in volume
     */
    template<typename matrixData> void calculateLBP(const RegionSequence &regSeq, std::vector<cv::Mat> &lbp, const long unsigned int matSizes[3]);

    /*!
     * \brief Calculate 4-neighbours interpolation weights
     */
    void calculateInterpolationWeights(float value1, float value2, float &w1, float &w2, float &w3, float &w4);

    /*!
    * \brief Calculate resulting histogram
    */
    void calculateResultHistReduced(cv::Mat cellHist, cv::Mat resultPart);

    /*!
    * \brief Calculate number patterns in resulting histogram taking all settings into account (ie rotate invatiant, uniform etc)
    */
    int32_t getNumResultPatterns();

    static const int minNonUniformValue;
};
};

/** @}*/
#endif

