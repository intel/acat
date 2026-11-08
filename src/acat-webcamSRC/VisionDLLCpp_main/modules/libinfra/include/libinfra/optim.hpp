/*M///////////////////////////////////////////////////////////////////////////////////////
//
//  IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING.
//
//  By downloading, copying, installing or using the software you agree to this license.
//  If you do not agree to this license, do not download, install,
//  copy or use the software.
//
//
//                           License Agreement
//                For Open Source Computer Vision Library
//
// Copyright (C) 2013, OpenCV Foundation, all rights reserved.
// Third party copyrights are property of their respective owners.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//   * Redistribution's of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//   * Redistribution's in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
//   * The name of the copyright holders may not be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall the OpenCV Foundation or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//
//M*/

#ifndef __OPENCV_OPTIM_HPP__
#define __OPENCV_OPTIM_HPP__

#include "libivcp/sptr.hpp"

namespace cv{namespace optim
{

/*!
 * \brief Solver class
 */
class CV_EXPORTS Solver : public Algorithm
{
public:
    /*!
     * \brief Function class
     */
    class CV_EXPORTS Function
    {
    public:
        /*!
         * \brief Destructor
         */
        virtual ~Function() {}

        /*!
         * \brief Solve method
         */
        virtual double calc(const double* x) const = 0;

        /*!
         * \brief Get gradient
         */
        virtual void getGradient(const double* /*x*/,double* /*grad*/) {}
    };

    /*!
     * \brief Getter for the optimized function
     */
    virtual Ptr<Function> getFunction() const = 0;

    /*!
     * \brief Setter for the optimized function
     */
    virtual void setFunction(const Ptr<Function>& f) = 0;

    /*!
     * \brief Getter for the previously set terminal criteria for this algorithm
     */
    virtual TermCriteria getTermCriteria() const = 0;

    /*!
     * \brief Set terminal criteria for downhill simplex method
     */
    virtual void setTermCriteria(const TermCriteria& termcrit) = 0;

    /*!
     * \brief The main method of the DownhillSolver
     * \param[in] x initial point, centroid of an initial simplex
     */
    virtual double minimize(InputOutputArray x) = 0;
};

/*!
 * \brief Downhill simplex class
 */
class CV_EXPORTS DownhillSolver : public Solver
{
public:
    /*!
     * \brief Get initial step 
     * \return Row-vector, even if the column-vector was given
     */
    virtual void getInitStep(OutputArray step) const = 0;

    /*!
     * \brief Set initial step
     */
    virtual void setInitStep(InputArray step) = 0;
};

/*!
 * \brief Downhill simplex implementation class
 */
class DownhillSolverImpl : public DownhillSolver
    {
    public:
        void getInitStep(OutputArray step) const;
        void setInitStep(InputArray step);
        Ptr<Function> getFunction() const;
        TermCriteria getTermCriteria() const;
        DownhillSolverImpl();
        void setTermCriteria(const TermCriteria& termcrit);
        double minimize(InputOutputArray x);

        /*!
         * \brief Setter for the optimized function
         */
        void setFunction(const Ptr<Function>& f);
    protected:
        /*!
         * \brief The current optimized function
         */
        Ptr<Solver::Function> _Function;

        /*!
         * \brief The current terminal criteria for downhill simplex method
         */
        TermCriteria _termcrit;

        /*!
         * \brief The current step
         */
        Mat _step;

        /*!
         * \brief The current centroid of current simplex
         */
        Mat_<double> buf_x;

    private:
        inline void createInitialSimplex(Mat_<double>& simplex,Mat& step);
        inline double innerDownhillSimplex(cv::Mat_<double>& p,double MinRange,double MinError,int& nfunk,
                const Ptr<Solver::Function>& f,int nmax);
        inline double tryNewPoint(Mat_<double>& p,Mat_<double>& y,Mat_<double>& coord_sum,const Ptr<Solver::Function>& f,int ihi,
                double fac,Mat_<double>& ptry);
    };

// both minRange & minError are specified by termcrit.epsilon; In addition, user may specify the number of iterations that the algorithm does.
CV_EXPORTS_W Ptr<DownhillSolver> createDownhillSolver(const Ptr<Solver::Function>& f=Ptr<Solver::Function>(),
        InputArray initStep=Mat_<double>(1,1,0.0),
        TermCriteria termcrit=TermCriteria(TermCriteria::MAX_ITER+TermCriteria::EPS,5000,0.000001));

//! conjugate gradient method
class CV_EXPORTS ConjGradSolver : public Solver{
};

CV_EXPORTS_W Ptr<ConjGradSolver> createConjGradSolver(const Ptr<Solver::Function>& f=Ptr<ConjGradSolver::Function>(),
        TermCriteria termcrit=TermCriteria(TermCriteria::MAX_ITER+TermCriteria::EPS,5000,0.000001));

//!the return codes for solveLP() function
enum
{
    SOLVELP_UNBOUNDED    = -2, //problem is unbounded (target function can achieve arbitrary high values)
    SOLVELP_UNFEASIBLE    = -1, //problem is unfeasible (there are no points that satisfy all the constraints imposed)
    SOLVELP_SINGLE    = 0, //there is only one maximum for target function
    SOLVELP_MULTI    = 1 //there are multiple maxima for target function - the arbitrary one is returned
};

CV_EXPORTS_W int solveLP(const Mat& Func, const Mat& Constr, Mat& z);
CV_EXPORTS_W void denoise_TVL1(const std::vector<Mat>& observations,Mat& result, double lambda=1.0, int niters=30);
}}// cv

#endif
