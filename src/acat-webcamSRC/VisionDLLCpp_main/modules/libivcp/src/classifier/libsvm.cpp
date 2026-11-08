///////////////////////////////////////////////////////////////////////////
// <copyright file="libsvm.cpp" company="Intel Corporation">
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


/****************************************************************************************\
                                COPYRIGHT NOTICE
                                ----------------

  The code has been derived from libsvm library (version 3.16)
  (http://www.csie.ntu.edu.tw/~cjlin/libsvm).

  Here is the orignal copyright:
------------------------------------------------------------------------------------------
    Copyright (c) 2000-2013 Chih-Chung Chang and Chih-Jen Lin
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:

    1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.

    3. Neither name of copyright holders nor the names of its contributors
    may be used to endorse or promote products derived from this software
    without specific prior written permission.


    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
\****************************************************************************************/

#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <float.h>
#include <string>
#include <stdarg.h>
#include <limits.h>
#include <locale.h>
#include <list>
#include "libivcp/classifier/libsvm.hpp"

using namespace ivcp;
using namespace std;
using namespace cv;

namespace libsvm
{
void svmCrossValidation(const SVMTrainData* prob, const ivcp::SVMParameters* param, int nr_fold, double* target);
double svmPredictValues(const LibSVMModel* model, const SVMVector& feature, double* dec_values);

/*!
 * \brief Swap to values
 * \param[in] x The first value
 * \param[in] y The second value
 */
template <class T> static inline void swap(T& x, T& y) 
{ 
    T t = x; 
    x = y; 
    y = t; 
}

/*!
 * \brief Allocate memory of the given size and copy content of source
 * \param[in] src Pointer to the buffer from which data is copied
 * \param[in] n The size of the copied data
 * \param[out] dst Pointer to the new memory buffer where copy will stored
 */
template <class S, class T> static inline void clone(T*& dst, S* src, int n)
{
    dst = new T[n];
    memcpy((void *)dst, (void *)src, sizeof(T)*n);
}

/*!
 * \brief Raising to a power
 * \param[in] base Number that should be raised to a power
 * \param[in] pow Power
 * \return Result of raising to a power
 */
static inline double powi(double base, int pow)
{
    double tmp = base, ret = 1.0;

    for(int p = pow; p > 0; p /= 2)
    {
        if (p % 2 == 1) 
        {
            ret *= tmp;
        }
        tmp = tmp * tmp;
    }
    return ret;
}

#define INF HUGE_VAL
#define TAU 1e-12



SVMVector::SVMVector(unsigned int _size)
{
    varCount = _size;
    data = NULL;
    if (varCount > 0)
    {
        data = new double[varCount];
    }
}

void SVMVector::release()
{
    if (data != NULL)
    {
        delete[] data;
    }
    varCount = 0;
    data = NULL;
}

void SVMVector::resize(unsigned int _size)
{
    release();
    varCount = _size;
    if (varCount > 0)
    {
        data = new double[varCount];
    }
}

void SVMVector::swap(SVMVector &v)
{
    int _size = varCount;
    varCount = v.varCount;
    v.varCount = _size;

    double* _data = data;
    data = v.data;
    v.data = _data;
}

void SVMVector::setData(const SVMVector &v)
{
    resize(v.varCount);
    for (unsigned int i = 0; i < varCount; i++)
    {
        data[i] = v.data[i];
    }
}

int SVMVector::size() const
{
    return varCount;
}


SVMTrainData::SVMTrainData(int numOfFeatures)
{
    numberOfFeatures = numOfFeatures;
    if (numOfFeatures > 0)
    {
        responses = new double[numberOfFeatures];
        features = new SVMVector[numberOfFeatures];
    }
    else
    {
        numOfFeatures = 0;
        responses = NULL;
        features = NULL;
    }
}

SVMTrainData::~SVMTrainData()
{
    if (responses != NULL)
    {
        delete[] responses;
    }
    if (features != NULL)
    {
        delete[] features;
    }
}

void SVMTrainData::release()
{
    if (responses != NULL)
    {
        delete[] responses;
    }
    if (features != NULL)
    {
        for (unsigned int i = 0; i < numberOfFeatures; i++)
        {
            features[i].release();
        }
        delete[] features;
    }
    numberOfFeatures = 0;
    responses = NULL;
    features = NULL;
}

void SVMTrainData::set(const list<Mat>& _features, const vector<double>& _responses)
{
    if (_features.empty())
    {
        IVCP_EXCEPTION(NoDataException, "No features to train the model");
    }
    if (_responses.empty())
    {
        IVCP_EXCEPTION(NoDataException, "No responses to train the model");
    }
    if (_features.size() != _responses.size())
    {
        IVCP_EXCEPTION(IncorrectParamException, "Number of features and number of responses mismatch");
    }  
    if (_responses.size() != numberOfFeatures)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Size of actual train data does not match the size of expected train data");
    }

    auto fIter = _features.begin();
    int varCount = (*fIter).cols;
    for (unsigned int i = 0; i < numberOfFeatures; i++, fIter++)
    {
        responses[i] = _responses[i];
        features[i].resize(varCount);
        for (int j = 0; j < varCount; j++)
        {
            features[i].data[j] = (*fIter).row(0).at<float>(j);
        }
    }
}

LibSVMModel::LibSVMModel() : class_count(0), sv_count(0),
        SV(NULL), sv_coef(NULL), rho(NULL), 
        probA(NULL), probB(NULL), sv_indices(NULL),
        labels(NULL), numSVForClasses(NULL) 
{
}

LibSVMModel::~LibSVMModel()
{
    if (sv_coef != NULL)
    {
        for (int i = 0; i < class_count - 1; i++)
        {
            delete[] sv_coef[i];
        }
    }

    for (int i = 0; i < sv_count; i++)
    {
        SV[i].release();
    }
    if (SV != NULL)
    {
        delete[] SV;
    }

    if (sv_coef != NULL)
    {
        delete[] sv_coef;
    }

    if (rho != NULL)
    {
        delete[] rho;
    }

    if (labels != NULL)
    {
        delete[] labels;
    }

    if (probA != NULL)
    {
        delete[] probA;
    }

    if (probB != NULL)
    {
        delete[] probB;
    }

    if (sv_indices != NULL)
    {
        delete[] sv_indices;
    }

    if (numSVForClasses != NULL)
    {
        delete[] numSVForClasses;
    }
}
/*!
 * \brief Kernel Cache
 */
class Cache
{
public:
    /*!
     * \brief Constructor
     * \param[in] dataSize The number of total data items
     * \param[in] cacheSize The cache size limit in bytes
     */
    Cache(int dataSize, long int cacheSize);
    /*!
     * \brief Destructor
     */
    ~Cache();

    /*!
     * \brief Request data [0, len)
     * \return Some position p where [p,len) need to be filled (p >= len if nothing needs to be filled)
     */
    int get_data(const int index, float** data, int len);

    /*!
     * \brief Swap two indexes
     */
    void swap_index(int i, int j);    
private:
    /*!
     * \brief The number of total data items
     */
    int data_size;
    /*!
     * \brief The cache size limit in bytes
     */
    long int size;

    struct head_t
    {
        head_t *prev, *next;    // a circular list
        float *data;
        int len;        // data[0,len) is cached in this entry
    };

    head_t *head;
    head_t lru_head;
    void lru_delete(head_t *h);
    void lru_insert(head_t *h);
};

Cache::Cache(int dataSize, long int cacheSize): data_size(dataSize), size(cacheSize)
{
    head = reinterpret_cast<head_t*>(calloc(data_size, sizeof(head_t)));    // initialized to 0;
    size /= sizeof(float);
    size -= data_size * sizeof(head_t) / sizeof(float);
    size = max(size, 2 * (long int) data_size);    // cache must be large enough for two columns
    lru_head.next = lru_head.prev = &lru_head;
}

Cache::~Cache()
{
    for (head_t* h = lru_head.next; h != &lru_head; h = h->next)
    {
        free(h->data);
    }
    free(head);
}

void Cache::lru_delete(head_t *h)
{
    // delete from current location
    h->prev->next = h->next;
    h->next->prev = h->prev;
}

void Cache::lru_insert(head_t *h)
{
    // insert to last position
    h->next = &lru_head;
    h->prev = lru_head.prev;
    h->prev->next = h;
    h->next->prev = h;
}

int Cache::get_data(const int index, float **data, int len)
{
    head_t* h = &head[index];
    if (h->len) 
    {
        lru_delete(h);
    }
    int more = len - h->len;

    if (more > 0)
    {
        // free old space
        while (size < more)
        {
            head_t* old = lru_head.next;
            lru_delete(old);
            free(old->data);
            size += old->len;
            old->data = 0;
            old->len = 0;
        }

        // allocate new space
        h->data = (float *)realloc(h->data, sizeof(float) * len);
        size -= more;
        swap(h->len, len);
    }

    lru_insert(h);
    *data = h->data;
    return len;
}

void Cache::swap_index(int i, int j)
{
    if (i == j) 
    {
        return;
    }

    if (head[i].len) 
    {
        lru_delete(&head[i]);
    }
    if (head[j].len) 
    {
        lru_delete(&head[j]);
    }
    swap(head[i].data, head[j].data);
    swap(head[i].len, head[j].len);
    if (head[i].len) 
    {
        lru_insert(&head[i]);
    }
    if (head[j].len) 
    {
        lru_insert(&head[j]);
    }

    if (i > j) 
    {
        swap(i, j);
    } 
    for (head_t *h = lru_head.next; h != &lru_head; h = h->next)
    {
        if (h->len > i)
        {
            if (h->len > j)
            {
                swap(h->data[i], h->data[j]);
            }
            else
            {
                // give up
                lru_delete(h);
                free(h->data);
                size += h->len;
                h->data = 0;
                h->len = 0;
            }
        }
    }
}

//
// Kernel evaluation
//
// the static method k_function is for doing single kernel evaluation
// the constructor of Kernel prepares to calculate the l*l kernel matrix
// the member function get_Q is for getting one column from the Q Matrix
//
class QMatrix {
public:
    virtual float* get_Q(int column, int len) const = 0;
    virtual double* get_QD() const = 0;
    virtual void swap_index(int i, int j) const = 0;
    virtual ~QMatrix() {}
};

class Kernel: public QMatrix {
public:
    Kernel(int l, const SVMVector* x, const SVMParameters& param);
    virtual ~Kernel();

    static double k_function(const SVMVector& x, const SVMVector& y, const SVMParameters& param);
    virtual float* get_Q(int column, int len) const = 0;
    virtual double* get_QD() const = 0;
    virtual void swap_index(int i, int j) const    // no so const...
    {
        x[i].swap(x[j]);
        if (x_square) 
        {
            swap(x_square[i], x_square[j]);
        }
    }
protected:

    double (Kernel::*kernel_function)(int i, int j) const;

private:
    SVMVector* x;
    double* x_square;

    // SVM parameters
    int kernel_type;
    int degree;
    double gamma;
    double coef0;

    static double dot(const SVMVector& px, const SVMVector& py);

    double kernel_linear(int i, int j) const
    {
        return dot(x[i], x[j]);
    }
    double kernel_poly(int i, int j) const
    {
        return powi(gamma * dot(x[i], x[j]) + coef0, degree);
    }
    double kernel_rbf(int i, int j) const
    {
        return exp(-gamma * (x_square[i] + x_square[j] - 2 * dot(x[i], x[j])));
    }
    double kernel_sigmoid(int i, int j) const
    {
        return tanh(gamma * dot(x[i], x[j]) + coef0);
    }
};

Kernel::Kernel(int l, const SVMVector* x_, const SVMParameters& param)
:kernel_type(param.kernelType), degree(param.degree),
 gamma(param.gamma), coef0(param.coef0)
{
    switch (kernel_type)
    {
        case LINEAR:
            kernel_function = &Kernel::kernel_linear;
            break;
        case POLY:
            kernel_function = &Kernel::kernel_poly;
            break;
        case RBF:
            kernel_function = &Kernel::kernel_rbf;
            break;
        case SIGMOID:
            kernel_function = &Kernel::kernel_sigmoid;
            break;
    }

    clone(x, x_, l);
    
    if (kernel_type == RBF)
    {
        x_square = new double[l];
        for (int i = 0; i < l; i++)
        {
            x_square[i] = dot(x[i], x[i]);
        }
    }
    else
    {
        x_square = NULL;
    }
}

Kernel::~Kernel()
{
    if (x != NULL)
    {
        delete[] x;
    }
    if (x_square != NULL)
    {
        delete[] x_square;
    }
}

double Kernel::dot(const SVMVector& px, const SVMVector& py)
{
    double sum = 0;
    for (int i = 0; i < px.size(); i++)
    {
        sum += px.data[i] * py.data[i];
    }
    return sum;
}

double Kernel::k_function(const SVMVector& x, const SVMVector& y, const SVMParameters& param)
{
    switch (param.kernelType)
    {
        case LINEAR:
            return dot(x, y);
        case POLY:
            return powi(param.gamma * dot(x, y) + param.coef0, param.degree);
        case RBF:
        {
            double sum = 0;
            for (int i = 0; i < x.size(); i++)
            {
                double d = x.data[i] - y.data[i];
                sum += d * d;
            }
            
            return exp(-param.gamma * sum);
        }
        case SIGMOID:
            return tanh(param.gamma * dot(x, y) + param.coef0);
        default:
            return 0;  // Unreachable 
    }
}

// An SMO algorithm in Fan et al., JMLR 6(2005), p. 1889--1918
// Solves:
//
//    min 0.5(\alpha^T Q \alpha) + p^T \alpha
//
//        y^T \alpha = \delta
//        y_i = +1 or -1
//        0 <= alpha_i <= Cp for y_i = 1
//        0 <= alpha_i <= Cn for y_i = -1
//
// Given:
//
//    Q, p, y, Cp, Cn, and an initial feasible point \alpha
//    l is the size of vectors and matrices
//    eps is the stopping tolerance
//
// solution will be put in \alpha, objective value will be put in obj
//
class Solver {
public:
    Solver();
    virtual ~Solver() {};

    struct SolutionInfo {
        double obj;
        double rho;
        double upper_bound_p;
        double upper_bound_n;
        double r;    // for Solver_NU

        SolutionInfo()
        {
            obj = 0;
            rho = 0;
            upper_bound_p = 0;
            upper_bound_n = 0;
            r = 0;
        }
    };

    void Solve(int l, const QMatrix& Q, const double *p_, const signed char *y_,
           double *alpha_, double Cp, double Cn, double eps,
           SolutionInfo* si, bool useShrinking);
protected:
    int active_size;
    signed char *y;
    double *G;        // gradient of objective function
    enum { LOWER_BOUND, UPPER_BOUND, FREE };
    char *alpha_status;    // LOWER_BOUND, UPPER_BOUND, FREE
    double *alpha;
    const QMatrix *Q;
    const double *QD;
    double eps;
    double Cp,Cn;
    double *p;
    int *active_set;
    double *G_bar;        // gradient, if we treat free variables as 0
    int l;
    bool unshrink;    // XXX

    double get_C(int i)
    {
        return (y[i] > 0)? Cp : Cn;
    }

    void update_alpha_status(int i)
    {
        if (alpha[i] >= get_C(i))
        {
            alpha_status[i] = UPPER_BOUND;
        }
        else 
        {
            if (alpha[i] <= 0)
            {
                alpha_status[i] = LOWER_BOUND;
            }
            else
            {
                alpha_status[i] = FREE;
            }
        }
    }
    bool is_upper_bound(int i) 
    { 
        return alpha_status[i] == UPPER_BOUND; 
    }
    bool is_lower_bound(int i) 
    { 
        return alpha_status[i] == LOWER_BOUND; 
    }
    bool is_free(int i) 
    { 
        return alpha_status[i] == FREE; 
    }
    void swap_index(int i, int j);
    void reconstruct_gradient();
    virtual bool select_working_set(int &i, int &j);
    virtual double calculate_rho();
    virtual void do_shrinking();
private:
    bool be_shrunk(int i, double Gmax1, double Gmax2);    
};

Solver::Solver()
{
    active_size = 0;
    y = NULL;
    G = NULL;
    alpha_status = NULL;
    alpha = NULL;
    Q  = NULL;
    QD = NULL;
    eps = 0;
    Cp = 0;
    Cn = 0;
    p = NULL;
    active_set = NULL;
    G_bar = NULL; 
    l = 0;
    unshrink = false;
}

void Solver::swap_index(int i, int j)
{
    Q->swap_index(i, j);
    swap(y[i], y[j]);
    swap(G[i], G[j]);
    swap(alpha_status[i], alpha_status[j]);
    swap(alpha[i], alpha[j]);
    swap(p[i], p[j]);
    swap(active_set[i], active_set[j]);
    swap(G_bar[i], G_bar[j]);
}

void Solver::reconstruct_gradient()
{
    // reconstruct inactive elements of G from G_bar and free variables

    if (active_size == l) 
    {
        return;
    }

    int nr_free = 0;

    for (int j = active_size; j < l; j++)
    {
        G[j] = G_bar[j] + p[j];
    }

    for (int j = 0; j < active_size; j++)
    {
        if (is_free(j))
        {
            nr_free++;
        }
    }

    if (2 * nr_free < active_size)
    {
        IVCP_LOG_WARNING("Using without shrinking may be faster");
    }

    if (nr_free * l > 2 * active_size * (l - active_size))
    {
        for (int i = active_size; i < l; i++)
        {
            const float *Q_i = Q->get_Q(i, active_size);
            for (int j = 0; j < active_size; j++)
            {
                if (is_free(j))
                {
                    G[i] += alpha[j] * Q_i[j];
                }
            }
        }
    }
    else
    {
        for (int i = 0; i < active_size; i++)
            if (is_free(i))
            {
                const float *Q_i = Q->get_Q(i,l);
                double alpha_i = alpha[i];
                for (int j = active_size; j < l; j++)
                {
                    G[j] += alpha_i * Q_i[j];
                }
            }
    }
}

void Solver::Solve(int l, const QMatrix& Q, const double *p_, const signed char *y_,
           double *alpha_, double Cp, double Cn, double eps,
           SolutionInfo* si, bool useShrinking)
{
    this->l = l;
    this->Q = &Q;
    QD = Q.get_QD();
    clone(p, p_, l);
    clone(y, y_, l);
    clone(alpha, alpha_, l);
    this->Cp = Cp;
    this->Cn = Cn;
    this->eps = eps;
    unshrink = false;

    // initialize alpha_status
    alpha_status = new char[l];
    for (int i = 0; i < l; i++)
    {
        update_alpha_status(i);
    }

    // initialize active set (for shrinking)
    active_set = new int[l];
    for (int i = 0; i < l; i++)
    {
        active_set[i] = i;
    }
    active_size = l;

    // initialize gradient
    G = new double[l];
    G_bar = new double[l];
    for (int i = 0; i < l; i++)
    {
        G[i] = p[i];
        G_bar[i] = 0;
    }
    for (int i = 0; i < l; i++)
    {
        if (!is_lower_bound(i))
        {
            const float *Q_i = Q.get_Q(i, l);
            double alpha_i = alpha[i];
            for (int j = 0; j < l; j++)
            {
                G[j] += alpha_i*Q_i[j];
            }
            if (is_upper_bound(i))
            {
                for (int j = 0; j < l; j++)
                {
                    G_bar[j] += get_C(i) * Q_i[j];
                }
            }
        }
    }

    // optimization step
    int iter = 0;
    int max_iter = max(10000000, l > INT_MAX / 100 ? INT_MAX : 100 * l);
    int counter = min(l, 1000) + 1;
    
    while(iter < max_iter)
    {
        // do shrinking
        if(--counter == 0)
        {
            counter = min(l, 1000);
            if (useShrinking) 
            {
                do_shrinking();
            }
        }

        int i = 0, j = 0;
        if (select_working_set(i, j))
        {
            // reconstruct the whole gradient
            reconstruct_gradient();
            // reset active set size and check
            active_size = l;
            if (select_working_set(i, j))
            {
                break;
            }
            else
            {
                counter = 1;    // do shrinking next iteration
            }
        }
        
        ++iter;

        // update alpha[i] and alpha[j], handle bounds carefully
        
        const float *Q_i = Q.get_Q(i, active_size);
        const float *Q_j = Q.get_Q(j, active_size);

        double C_i = get_C(i);
        double C_j = get_C(j);

        double old_alpha_i = alpha[i];
        double old_alpha_j = alpha[j];

        if (y[i] != y[j])
        {
            double quad_coef = QD[i] + QD[j] + 2 * Q_i[j];
            if (quad_coef <= 0)
            {
                quad_coef = TAU;
            }
            double delta = (-G[i] - G[j]) / quad_coef;
            double diff = alpha[i] - alpha[j];
            alpha[i] += delta;
            alpha[j] += delta;
            
            if (diff > 0)
            {
                if (alpha[j] < 0)
                {
                    alpha[j] = 0;
                    alpha[i] = diff;
                }
            }
            else
            {
                if (alpha[i] < 0)
                {
                    alpha[i] = 0;
                    alpha[j] = -diff;
                }
            }
            if (diff > C_i - C_j)
            {
                if(alpha[i] > C_i)
                {
                    alpha[i] = C_i;
                    alpha[j] = C_i - diff;
                }
            }
            else
            {
                if (alpha[j] > C_j)
                {
                    alpha[j] = C_j;
                    alpha[i] = C_j + diff;
                }
            }
        }
        else
        {
            double quad_coef = QD[i] + QD[j] - 2 * Q_i[j];
            if (quad_coef <= 0)
            {
                quad_coef = TAU;
            }
            double delta = (G[i] - G[j]) / quad_coef;
            double sum = alpha[i] + alpha[j];
            alpha[i] -= delta;
            alpha[j] += delta;

            if (sum > C_i)
            {
                if (alpha[i] > C_i)
                {
                    alpha[i] = C_i;
                    alpha[j] = sum - C_i;
                }
            }
            else
            {
                if (alpha[j] < 0)
                {
                    alpha[j] = 0;
                    alpha[i] = sum;
                }
            }
            if (sum > C_j)
            {
                if(alpha[j] > C_j)
                {
                    alpha[j] = C_j;
                    alpha[i] = sum - C_j;
                }
            }
            else
            {
                if (alpha[i] < 0)
                {
                    alpha[i] = 0;
                    alpha[j] = sum;
                }
            }
        }

        // update G

        double delta_alpha_i = alpha[i] - old_alpha_i;
        double delta_alpha_j = alpha[j] - old_alpha_j;
        
        for(int k = 0; k < active_size; k++)
        {
            G[k] += Q_i[k] * delta_alpha_i + Q_j[k] * delta_alpha_j;
        }

        // update alpha_status and G_bar
        bool ui = is_upper_bound(i);
        bool uj = is_upper_bound(j);
        update_alpha_status(i);
        update_alpha_status(j);
        if (ui != is_upper_bound(i))
        {
            Q_i = Q.get_Q(i, l);
            if (ui)
            {
                for (int k = 0; k < l; k++)
                {
                    G_bar[k] -= C_i * Q_i[k];
                }
            }
            else
            {
                for(int k = 0; k < l; k++)
                {
                    G_bar[k] += C_i * Q_i[k];
                }
            }
        }

        if (uj != is_upper_bound(j))
        {
            Q_j = Q.get_Q(j, l);
            if (uj)
            {
                for (int k = 0; k < l; k++)
                {
                    G_bar[k] -= C_j * Q_j[k];
                }
            }
            else
            {
                for (int k = 0; k < l; k++)
                {
                    G_bar[k] += C_j * Q_j[k];
                }
            }
        }
    }

    if (iter >= max_iter)
    {
        if(active_size < l)
        {
            // reconstruct the whole gradient to calculate objective value
            reconstruct_gradient();
            active_size = l;
        }
        IVCP_LOG_WARNING("Reaching max number of iterations");
    }

    // calculate rho
    si->rho = calculate_rho();

    // calculate objective value
    double v = 0;
    for (int i = 0; i < l; i++)
    {
        v += alpha[i] * (G[i] + p[i]);
    }
    si->obj = v/2;

    // put back the solution
    for (int i = 0; i < l; i++)
    {
        alpha_[active_set[i]] = alpha[i];
    }

    si->upper_bound_p = Cp;
    si->upper_bound_n = Cn;

    delete[] p;
    delete[] y;
    delete[] alpha;
    delete[] alpha_status;
    delete[] active_set;
    delete[] G;
    delete[] G_bar;
}

// return true if already optimal, return false otherwise
bool Solver::select_working_set(int &out_i, int &out_j)
{
    // return i,j such that
    // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
    // j: minimizes the decrease of obj value
    //    (if quadratic coefficeint <= 0, replace it with tau)
    //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)
    
    double Gmax = -INF;
    double Gmax2 = -INF;
    int Gmax_idx = -1;
    int Gmin_idx = -1;
    double obj_diff_min = INF;

    for (int t = 0; t < active_size; t++)
    {
        if (y[t] == 1)    
        {
            if (!is_upper_bound(t))
            {
                if(-G[t] >= Gmax)
                {
                    Gmax = -G[t];
                    Gmax_idx = t;
                }
            }
        }
        else
        {
            if (!is_lower_bound(t))
            {
                if (G[t] >= Gmax)
                {
                    Gmax = G[t];
                    Gmax_idx = t;
                }
            }
        }
    }

    int i = Gmax_idx;
    const float *Q_i = NULL;
    if (i != -1) // NULL Q_i not accessed: Gmax=-INF if i=-1
    {
        Q_i = Q->get_Q(i,active_size);
    }

    for (int j = 0; j < active_size; j++)
    {
        if (y[j] == 1)
        {
            if (!is_lower_bound(j))
            {
                double grad_diff = Gmax + G[j];
                if (G[j] >= Gmax2)
                {
                    Gmax2 = G[j];
                }
                if (grad_diff > 0)
                {
                    double obj_diff; 
                    double quad_coef = QD[i] + QD[j] - 2.0 * y[i] * Q_i[j];
                    if (quad_coef > 0)
                    {
                        obj_diff = -(grad_diff * grad_diff) / quad_coef;
                    }
                    else
                    {
                        obj_diff = -(grad_diff * grad_diff) / TAU;
                    }

                    if (obj_diff <= obj_diff_min)
                    {
                        Gmin_idx = j;
                        obj_diff_min = obj_diff;
                    }
                }
            }
        }
        else
        {
            if (!is_upper_bound(j))
            {
                double grad_diff= Gmax - G[j];
                if (-G[j] >= Gmax2)
                {
                    Gmax2 = -G[j];
                }
                if (grad_diff > 0)
                {
                    double obj_diff; 
                    double quad_coef = QD[i] + QD[j] + 2.0 * y[i] * Q_i[j];
                    if (quad_coef > 0)
                    {
                        obj_diff = -(grad_diff * grad_diff) / quad_coef;
                    }
                    else
                    {
                        obj_diff = -(grad_diff * grad_diff) / TAU;
                    }

                    if (obj_diff <= obj_diff_min)
                    {
                        Gmin_idx = j;
                        obj_diff_min = obj_diff;
                    }
                }
            }
        }
    }

    if (Gmax + Gmax2 < eps)
    {
        return true;
    }

    out_i = Gmax_idx;
    out_j = Gmin_idx;
    return false;
}

bool Solver::be_shrunk(int i, double Gmax1, double Gmax2)
{
    if (is_upper_bound(i))
    {
        if (y[i] == +1)
        {
            return (-G[i] > Gmax1);
        }
        else
        {
            return (-G[i] > Gmax2);
        }
    }
    else 
    {
        if (is_lower_bound(i))
        {
            if (y[i] == 1)
            {
                return (G[i] > Gmax2);
            }
            else
            {
                return(G[i] > Gmax1);
            }
        }
        else
        {
            return false;
        }
    }
}

void Solver::do_shrinking()
{
    double Gmax1 = -INF;        // max { -y_i * grad(f)_i | i in I_up(\alpha) }
    double Gmax2 = -INF;        // max { y_i * grad(f)_i | i in I_low(\alpha) }

    // find maximal violating pair first
    for (int i = 0; i < active_size; i++)
    {
        if (y[i] == +1)    
        {
            if (!is_upper_bound(i))    
            {
                if (-G[i] >= Gmax1)
                {
                    Gmax1 = -G[i];
                }
            }
            if (!is_lower_bound(i))    
            {
                if (G[i] >= Gmax2)
                {
                    Gmax2 = G[i];
                }
            }
        }
        else    
        {
            if (!is_upper_bound(i))    
            {
                if (-G[i] >= Gmax2)
                {
                    Gmax2 = -G[i];
                }
            }
            if (!is_lower_bound(i))    
            {
                if (G[i] >= Gmax1)
                {
                    Gmax1 = G[i];
                }
            }
        }
    }

    if (unshrink == false && Gmax1 + Gmax2 <= eps * 10) 
    {
        unshrink = true;
        reconstruct_gradient();
        active_size = l;
    }

    for (int i = 0; i < active_size; i++)
    {
        if (be_shrunk(i, Gmax1, Gmax2))
        {
            active_size--;
            while (active_size > i)
            {
                if (!be_shrunk(active_size, Gmax1, Gmax2))
                {
                    swap_index(i, active_size);
                    break;
                }
                active_size--;
            }
        }
    }
}

double Solver::calculate_rho()
{
    double r;
    int nr_free = 0;
    double ub = INF, lb = -INF, sum_free = 0;
    for (int i = 0; i < active_size; i++)
    {
        double yG = y[i] * G[i];

        if (is_upper_bound(i))
        {
            if (y[i] == -1)
            {
                ub = min(ub, yG);
            }
            else
            {
                lb = max(lb, yG);
            }
        }
        else 
        {
            if (is_lower_bound(i))
            {
                if (y[i] == 1)
                {
                    ub = min(ub, yG);
                }
                else
                {
                    lb = max(lb, yG);
                }
            }
            else
            {
                ++nr_free;
                sum_free += yG;
            }
        }
    }

    if (nr_free > 0)
    {
        r = sum_free / nr_free;
    }
    else
    {
        r = (ub + lb) / 2;
    }

    return r;
}

//
// Solver for nu-svm classification and regression
//
// additional constraint: e^T \alpha = constant
//
class Solver_NU: public Solver
{
public:
    Solver_NU() : si(NULL) {}
    void Solve(int l, const QMatrix& Q, const double *p, const signed char *y,
           double *alpha, double Cp, double Cn, double eps,
           SolutionInfo* si, bool useShrinking)
    {
        this->si = si;
        Solver::Solve(l, Q, p, y, alpha, Cp, Cn, eps, si, useShrinking);
    }
private:
    SolutionInfo *si;
    bool select_working_set(int &i, int &j);
    double calculate_rho();
    bool be_shrunk(int i, double Gmax1, double Gmax2, double Gmax3, double Gmax4);
    void do_shrinking();
};

// return true if already optimal, return false otherwise
bool Solver_NU::select_working_set(int &out_i, int &out_j)
{
    // return i,j such that y_i = y_j and
    // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
    // j: minimizes the decrease of obj value
    //    (if quadratic coefficeint <= 0, replace it with tau)
    //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

    double Gmaxp = -INF;
    double Gmaxp2 = -INF;
    int Gmaxp_idx = -1;

    double Gmaxn = -INF;
    double Gmaxn2 = -INF;
    int Gmaxn_idx = -1;

    int Gmin_idx = -1;
    double obj_diff_min = INF;

    for (int t = 0; t < active_size; t++)
    {
        if (y[t] == +1)
        {
            if (!is_upper_bound(t))
            {
                if (-G[t] >= Gmaxp)
                {
                    Gmaxp = -G[t];
                    Gmaxp_idx = t;
                }
            }
        }
        else
        {
            if (!is_lower_bound(t))
            {
                if (G[t] >= Gmaxn)
                {
                    Gmaxn = G[t];
                    Gmaxn_idx = t;
                }
            }
        }
    }

    int ip = Gmaxp_idx;
    int in = Gmaxn_idx;
    const float *Q_ip = NULL;
    const float *Q_in = NULL;
    if (ip != -1) // NULL Q_ip not accessed: Gmaxp=-INF if ip=-1
    {
        Q_ip = Q->get_Q(ip, active_size);
    }
    if (in != -1)
    {
        Q_in = Q->get_Q(in, active_size);
    }

    for (int j = 0; j < active_size; j++)
    {
        if (y[j] == 1)
        {
            if (!is_lower_bound(j))    
            {
                double grad_diff = Gmaxp + G[j];
                if (G[j] >= Gmaxp2)
                {
                    Gmaxp2 = G[j];
                }
                if (grad_diff > 0)
                {
                    double obj_diff; 
                    double quad_coef = QD[ip] + QD[j] - 2 * Q_ip[j];
                    if (quad_coef > 0)
                    {
                        obj_diff = -(grad_diff * grad_diff) / quad_coef;
                    }
                    else
                    {
                        obj_diff = -(grad_diff * grad_diff) / TAU;
                    }

                    if (obj_diff <= obj_diff_min)
                    {
                        Gmin_idx=j;
                        obj_diff_min = obj_diff;
                    }
                }
            }
        }
        else
        {
            if (!is_upper_bound(j))
            {
                double grad_diff = Gmaxn - G[j];
                if (-G[j] >= Gmaxn2)
                {
                    Gmaxn2 = -G[j];
                }
                if (grad_diff > 0)
                {
                    double obj_diff; 
                    double quad_coef = QD[in] + QD[j] - 2 * Q_in[j];
                    if (quad_coef > 0)
                    {
                        obj_diff = -(grad_diff * grad_diff) / quad_coef;
                    }
                    else
                    {
                        obj_diff = -(grad_diff * grad_diff) / TAU;
                    }

                    if (obj_diff <= obj_diff_min)
                    {
                        Gmin_idx = j;
                        obj_diff_min = obj_diff;
                    }
                }
            }
        }
    }

    if (max(Gmaxp + Gmaxp2, Gmaxn + Gmaxn2) < eps)
    {
        return true;
    }

    if (y[Gmin_idx] == 1)
    {
        out_i = Gmaxp_idx;
    }
    else
    {
        out_i = Gmaxn_idx;
    }
    out_j = Gmin_idx;

    return false;
}

bool Solver_NU::be_shrunk(int i, double Gmax1, double Gmax2, double Gmax3, double Gmax4)
{
    if (is_upper_bound(i))
    {
        if (y[i] == +1)
        {
            return (-G[i] > Gmax1);
        }
        else    
        {
            return (-G[i] > Gmax4);
        }
    }
    else 
    {
        if (is_lower_bound(i))
        {
            if (y[i] == +1)
            {
                return (G[i] > Gmax2);
            }
            else    
            {
                return(G[i] > Gmax3);
            }
        }
        else
        {
            return false;
        }
    }
}

void Solver_NU::do_shrinking()
{
    double Gmax1 = -INF;    // max { -y_i * grad(f)_i | y_i = +1, i in I_up(\alpha) }
    double Gmax2 = -INF;    // max { y_i * grad(f)_i | y_i = +1, i in I_low(\alpha) }
    double Gmax3 = -INF;    // max { -y_i * grad(f)_i | y_i = -1, i in I_up(\alpha) }
    double Gmax4 = -INF;    // max { y_i * grad(f)_i | y_i = -1, i in I_low(\alpha) }

    // find maximal violating pair first
    for (int i = 0; i < active_size; i++)
    {
        if (!is_upper_bound(i))
        {
            if (y[i] == +1)
            {
                if (-G[i] > Gmax1) 
                {
                    Gmax1 = -G[i];
                }
            }
            else
            {
                if (-G[i] > Gmax4) 
                {
                    Gmax4 = -G[i];
                }
            }
        }
        if (!is_lower_bound(i))
        {
            if (y[i] == +1)
            {    
                if (G[i] > Gmax2) 
                {
                    Gmax2 = G[i];
                }
            }
            else    
            {
                if (G[i] > Gmax3) 
                {
                    Gmax3 = G[i];
                }
            }
        }
    }

    if (unshrink == false && max(Gmax1 + Gmax2, Gmax3 + Gmax4) <= eps * 10) 
    {
        unshrink = true;
        reconstruct_gradient();
        active_size = l;
    }

    for (int i = 0; i < active_size; i++)
    {
        if (be_shrunk(i, Gmax1, Gmax2, Gmax3, Gmax4))
        {
            active_size--;
            while (active_size > i)
            {
                if (!be_shrunk(active_size, Gmax1, Gmax2, Gmax3, Gmax4))
                {
                    swap_index(i, active_size);
                    break;
                }
                active_size--;
            }
        }
    }
}

double Solver_NU::calculate_rho()
{
    int nr_free1 = 0, nr_free2 = 0;
    double ub1 = INF, ub2 = INF;
    double lb1 = -INF, lb2 = -INF;
    double sum_free1 = 0, sum_free2 = 0;

    for (int i = 0; i < active_size; i++)
    {
        if (y[i] == +1)
        {
            if (is_upper_bound(i))
            {
                lb1 = max(lb1, G[i]);
            }
            else 
            {
                if (is_lower_bound(i))
                {
                    ub1 = min(ub1, G[i]);
                }
                else
                {
                    nr_free1++;
                    sum_free1 += G[i];
                }
            }
        }
        else
        {
            if (is_upper_bound(i))
            {
                lb2 = max(lb2, G[i]);
            }
            else 
            {
                if (is_lower_bound(i))
                {
                    ub2 = min(ub2, G[i]);
                }
                else
                {
                    nr_free2++;
                    sum_free2 += G[i];
                }
            }
        }
    }

    double r1, r2;
    if (nr_free1 > 0)
    {
        r1 = sum_free1 / nr_free1;
    }
    else
    {
        r1 = (ub1 + lb1) / 2;
    }
    
    if (nr_free2 > 0)
    {
        r2 = sum_free2 / nr_free2;
    }
    else
    {
        r2 = (ub2 + lb2) / 2;
    }
    
    si->r = (r1 + r2) / 2;
    return (r1 - r2) / 2;
}

//
// Q matrices for various formulations
//
class SVC_Q: public Kernel
{ 
public:
    SVC_Q(const SVMTrainData& problem, const SVMParameters& params, const signed char *y_)
    :Kernel(problem.size(), problem.features, params)
    {
        clone(y, y_, problem.size());
        cache = new Cache(problem.size(), (long int)(params.cacheSize*(1 << 20)));
        QD = new double[problem.size()];
        for(unsigned int i = 0; i < problem.size(); i++)
        {
            QD[i] = (this->*kernel_function)(i, i);
        }
    }
    
    float *get_Q(int i, int len) const
    {
        float *data;
        int start;
        if ((start = cache->get_data(i, &data, len)) < len)
        {
            for (int j = start; j < len; j++)
            {
                data[j] = (float)(y[i] * y[j] * (this->*kernel_function)(i, j));
            }
        }
        return data;
    }

    double *get_QD() const
    {
        return QD;
    }

    void swap_index(int i, int j) const
    {
        cache->swap_index(i, j);
        Kernel::swap_index(i, j);
        swap(y[i], y[j]);
        swap(QD[i], QD[j]);
    }

    ~SVC_Q()
    {
        delete[] y;
        delete cache;
        delete[] QD;
    }
private:
    signed char *y;
    Cache *cache;
    double *QD;
};

class ONE_CLASS_Q: public Kernel
{
public:
    ONE_CLASS_Q(const SVMTrainData& problem, const SVMParameters& params)
    :Kernel(problem.size(), problem.features, params)
    {
        cache = new Cache(problem.size(), (long int)(params.cacheSize * (1 << 20)));
        QD = new double[problem.size()];
        for (unsigned int i = 0; i < problem.size(); i++)
        {
            QD[i] = (this->*kernel_function)(i, i);
        }
    }
    
    float *get_Q(int i, int len) const
    {
        float *data;
        int start;
        if ((start = cache->get_data(i, &data, len)) < len)
        {
            for (int j = start; j < len; j++)
            {
                data[j] = (float)(this->*kernel_function)(i, j);
            }
        }
        return data;
    }

    double *get_QD() const
    {
        return QD;
    }

    void swap_index(int i, int j) const
    {
        cache->swap_index(i, j);
        Kernel::swap_index(i, j);
        swap(QD[i], QD[j]);
    }

    ~ONE_CLASS_Q()
    {
        delete cache;
        delete[] QD;
    }
private:
    Cache *cache;
    double *QD;
};

class SVR_Q: public Kernel
{ 
public:
    SVR_Q(const SVMTrainData& problem, const SVMParameters& params)
    :Kernel(problem.size(), problem.features, params)
    {
        l = problem.size();
        cache = new Cache(l, (long int)(params.cacheSize * (1 << 20)));
        QD = new double[2*l];
        sign = new signed char[2*l];
        index = new int[2*l];
        for (int k = 0; k < l; k++)
        {
            sign[k] = 1;
            sign[k+l] = -1;
            index[k] = k;
            index[k+l] = k;
            QD[k] = (this->*kernel_function)(k, k);
            QD[k+l] = QD[k];
        }
        buffer[0] = new float[2*l];
        buffer[1] = new float[2*l];
        next_buffer = 0;
    }

    void swap_index(int i, int j) const
    {
        swap(sign[i], sign[j]);
        swap(index[i], index[j]);
        swap(QD[i], QD[j]);
    }
    
    float *get_Q(int i, int len) const
    {
        float *data;
        int real_i = index[i];
        if (cache->get_data(real_i, &data, l) < l)
        {
            for (int j = 0; j < l; j++)
            {
                data[j] = (float)(this->*kernel_function)(real_i, j);
            }
        }

        // reorder and copy
        float *buf = buffer[next_buffer];
        next_buffer = 1 - next_buffer;
        signed char si = sign[i];
        for (int j = 0; j < len; j++)
        {
            buf[j] = (float) si * (float) sign[j] * data[index[j]];
        }
        return buf;
    }

    double *get_QD() const
    {
        return QD;
    }

    ~SVR_Q()
    {
        delete cache;
        delete[] sign;
        delete[] index;
        delete[] buffer[0];
        delete[] buffer[1];
        delete[] QD;
    }
private:
    int l;
    Cache *cache;
    signed char *sign;
    int *index;
    mutable int next_buffer;
    float *buffer[2];
    double *QD;
};

//
// construct and solve various formulations
//
static void solve_c_svc(
    const SVMTrainData *problem, const SVMParameters* params,
    double *alpha, Solver::SolutionInfo* si, double Cp, double Cn)
{
    int l = problem->size();
    double *minus_ones = new double[l];
    signed char *y = new signed char[l];

    for(int i = 0; i < l; i++)
    {
        alpha[i] = 0;
        minus_ones[i] = -1;
        if (problem->responses[i] > 0) 
        {
            y[i] = +1;
        }
        else
        {
            y[i] = -1;
        }
    }

    Solver s;
    s.Solve(l, SVC_Q(*problem, *params, y), minus_ones, y,
        alpha, Cp, Cn, params->eps, si, params->useShrinking);

    for (int i = 0; i < l; i++)
    {
        alpha[i] *= y[i];
    }

    delete[] minus_ones;
    delete[] y;
}

static void solve_nu_svc(
    const SVMTrainData* problem, const SVMParameters* params,
    double* alpha, Solver::SolutionInfo* si)
{
    int l = problem->size();
    double nu = params->nu;

    signed char *y = new signed char[l];

    for (int i = 0; i < l; i++)
    {
        if (problem->responses[i] > 0)
        {
            y[i] = +1;
        }
        else
        {
            y[i] = -1;
        }
    }

    double sum_pos = nu * l / 2;
    double sum_neg = sum_pos;

    for (int i = 0; i < l; i++)
    {
        if (y[i] == +1)
        {
            alpha[i] = min(1.0, sum_pos);
            sum_pos -= alpha[i];
        }
        else
        {
            alpha[i] = min(1.0, sum_neg);
            sum_neg -= alpha[i];
        }
    }

    double *zeros = new double[l];

    for (int i = 0; i < l; i++)
    {
        zeros[i] = 0;
    }

    Solver_NU s;
    s.Solve(l, SVC_Q(*problem, *params, y), zeros, y,
        alpha, 1.0, 1.0, params->eps, si,  params->useShrinking);
    double r = si->r;

    for (int i = 0; i < l; i++)
    {
        alpha[i] *= y[i] / r;
    }

    si->rho /= r;
    si->obj /= (r * r);
    si->upper_bound_p = 1 / r;
    si->upper_bound_n = 1 / r;

    delete[] y;
    delete[] zeros;
}

static void solve_one_class(
    const SVMTrainData *problem, const SVMParameters *params,
    double *alpha, Solver::SolutionInfo* si)
{
    int l = problem->size();
    double *zeros = new double[l];
    signed char *ones = new signed char[l];

    unsigned int n = (int)(params->nu * problem->size());    // # of alpha's at upper bound

    for (unsigned int i = 0; i < n; i++)
    {
        alpha[i] = 1;
    }
    if (n < problem->size())
    {
        alpha[n] = params->nu * problem->size() - n;
    }
    for (int i = n + 1; i < l; i++)
    {
        alpha[i] = 0;
    }

    for (int i = 0; i < l; i++)
    {
        zeros[i] = 0;
        ones[i] = 1;
    }

    Solver s;
    s.Solve(l, ONE_CLASS_Q(*problem, *params), zeros, ones,
        alpha, 1.0, 1.0, params->eps, si, params->useShrinking);

    delete[] zeros;
    delete[] ones;
}

static void solve_epsilon_svr(
    const SVMTrainData *problem, const SVMParameters *params, double *alpha, Solver::SolutionInfo* si)
{
    int l = problem->size();
    double* alpha2 = new double[2*l];
    double* linear_term = new double[2*l];
    signed char* y = new signed char[2*l];

    for (int i = 0; i < l; i++)
    {
        alpha2[i] = 0;
        linear_term[i] = params->p - problem->responses[i];
        y[i] = 1;

        alpha2[i+l] = 0;
        linear_term[i+l] = params->p + problem->responses[i];
        y[i+l] = -1;
    }

    Solver s;
    s.Solve(2*l, SVR_Q(*problem, *params), linear_term, y,
        alpha2, params->C, params->C, params->eps, si, params->useShrinking);

    for(int i = 0; i < l; i++)
    {
        alpha[i] = alpha2[i] - alpha2[i+l];
    }

    delete[] alpha2;
    delete[] linear_term;
    delete[] y;
}

static void solve_nu_svr(
    const SVMTrainData *problem, const SVMParameters* params,
    double* alpha, Solver::SolutionInfo* si)
{
    int l = problem->size();
    double C = params->C;
    double* alpha2 = new double[2*l];
    double* linear_term = new double[2*l];
    signed char* y = new signed char[2*l];

    double sum = C * params->nu * l / 2;
    for (int i = 0; i < l; i++)
    {
        alpha2[i] = alpha2[i+l] = min(sum, C);
        sum -= alpha2[i];

        linear_term[i] = - problem->responses[i];
        y[i] = 1;

        linear_term[i+l] = problem->responses[i];
        y[i+l] = -1;
    }

    Solver_NU s;
    s.Solve(2*l, SVR_Q(*problem, *params), linear_term, y,
        alpha2, C, C, params->eps, si, params->useShrinking);

    for (int i = 0; i < l; i++)
    {
        alpha[i] = alpha2[i] - alpha2[i+l];
    }

    delete[] alpha2;
    delete[] linear_term;
    delete[] y;
}

//
// decision_function
//
struct decision_function
{
    double *alpha;
    double rho;    
};

static decision_function svm_train_one(
    const SVMTrainData *problem, const SVMParameters *params,
    double Cp, double Cn)
{
    double* alpha = new double[problem->size()];
    for (unsigned int i = 0; i < problem->size(); i++)
    {
        alpha[i] = 0;
    }
    Solver::SolutionInfo si;
    switch (params->svmType)
    {
        case C_SVC:
            solve_c_svc(problem, params, alpha, &si, Cp, Cn);
            break;
        case NU_SVC:
            solve_nu_svc(problem, params, alpha, &si);
            break;
        case ONE_CLASS:
            solve_one_class(problem, params, alpha, &si);
            break;
        case EPSILON_SVR:
            solve_epsilon_svr(problem, params, alpha, &si);
            break;
        case NU_SVR:
            solve_nu_svr(problem, params, alpha, &si);
            break;
    }

    // output SVs
    int nSV = 0;
    int nBSV = 0;
    for (unsigned int i = 0; i < problem->size(); i++)
    {
        if (fabs(alpha[i]) > 0)
        {
            nSV++;
            if(problem->responses[i] > 0)
            {
                if (fabs(alpha[i]) >= si.upper_bound_p)
                {
                    nBSV++;
                }
            }
            else
            {
                if (fabs(alpha[i]) >= si.upper_bound_n)
                {
                    nBSV++;
                }
            }
        }
    }

    decision_function f;
    f.alpha = alpha;
    f.rho = si.rho;
    return f;
}

// Platt's binary SVM Probablistic Output: an improvement from Lin et al.
static void sigmoid_train(
    int l, const double* dec_values, const double* labels, 
    double& A, double& B)
{
    double prior1 = 0, prior0 = 0;

    for (int i = 0; i < l; i++)
    {
        if (labels[i] > 0) 
        {
            prior1++;
        }
        else 
        {
            prior0++;
        }
    }
    
    int max_iter = 100;    // Maximal number of iterations
    double min_step = 1e-10;    // Minimal step taken in line search
    double sigma = 1e-12;    // For numerically strict PD of Hessian
    double eps = 1e-5;
    double hiTarget = (prior1 + 1.0) / (prior1 + 2.0);
    double loTarget = 1.0 / (prior0 + 2.0);
    double* t = new double[l];
    double fApB, p, q, h11, h22, h21, g1, g2, det, dA, dB, gd, stepsize;
    double newA, newB, newf, d1, d2;
    
    // Initial Point and Initial Fun Value
    A = 0.0; B = log((prior0 + 1.0) / (prior1 + 1.0));
    double fval = 0.0;

    for (int i = 0; i < l; i++)
    {
        if (labels[i] > 0) 
        {
            t[i] = hiTarget;
        }
        else 
        {
            t[i] = loTarget;
        }
        fApB = dec_values[i] * A + B;
        if (fApB >= 0)
        {
            fval += t[i] * fApB + log(1 + exp(-fApB));
        }
        else
        {
            fval += (t[i] - 1) * fApB + log(1 + exp(fApB));
        }
    }

    int iter;
    for (iter = 0; iter < max_iter; iter++)
    {
        // Update Gradient and Hessian (use H' = H + sigma I)
        h11 = sigma; // numerically ensures strict PD
        h22 = sigma;
        h21 = 0.0;
        g1 = 0.0;
        g2 = 0.0;
        for (int i = 0; i < l; i++)
        {
            fApB = dec_values[i] * A + B;
            if (fApB >= 0)
            {
                p=exp(-fApB) / (1.0 + exp(-fApB));
                q = 1.0 / (1.0 + exp(-fApB));
            }
            else
            {
                p = 1.0 / (1.0 + exp(fApB));
                q = exp(fApB) / (1.0 + exp(fApB));
            }
            d2 = p * q;
            h11 += dec_values[i] * dec_values[i] * d2;
            h22 += d2;
            h21 += dec_values[i] * d2;
            d1 = t[i] - p;
            g1 += dec_values[i] * d1;
            g2 += d1;
        }

        // Stopping Criteria
        if (fabs(g1) < eps && fabs(g2) < eps)
        {
            break;
        }

        // Finding Newton direction: -inv(H') * g
        det = h11 * h22 - h21 * h21;
        dA = -(h22 * g1 - h21 * g2) / det;
        dB = -(-h21 * g1 + h11 * g2) / det;
        gd = g1 * dA + g2 * dB;

        stepsize = 1;        // Line Search
        while (stepsize >= min_step)
        {
            newA = A + stepsize * dA;
            newB = B + stepsize * dB;

            // New function value
            newf = 0.0;
            for (int i = 0; i < l; i++)
            {
                fApB = dec_values[i] * newA + newB;
                if (fApB >= 0)
                {
                    newf += t[i] * fApB + log(1 + exp(-fApB));
                }
                else
                {
                    newf += (t[i] - 1) * fApB +log(1 + exp(fApB));
                }
            }
            // Check sufficient decrease
            if (newf < fval + 0.0001 * stepsize * gd)
            {
                A = newA;
                B = newB;
                fval = newf;
                break;
            }
            else
            {
                stepsize = stepsize / 2.0;
            }
        }

        if (stepsize < min_step)
        {
            IVCP_LOG_INFO("Line search fails in two-class probability estimates");
            break;
        }
    }

    if (iter >= max_iter)
    {
        IVCP_LOG_INFO("Reaching maximal iterations in two-class probability estimates");
    }
   delete[] t;
}

static double sigmoid_predict(double decision_value, double A, double B)
{
    double fApB = decision_value * A + B;
    // 1-p used later; avoid catastrophic cancellation
    if (fApB >= 0)
    {
        return exp(-fApB) / (1 + exp(-fApB));
    }
    else
    {
        return 1.0 / (1 + exp(fApB)) ;
    }
}

// Method 2 from the multiclass_prob paper by Wu, Lin, and Weng
static void multiclass_probability(int k, double **r, double *p)
{
    int iter = 0;
    int max_iter = max(100, k);
    double** Q = new double*[k];
    double* Qp = new double[k];
    double pQp, eps = 0.005 / k;
    
    for (int t = 0; t < k; t++)
    {
        p[t] = 1.0 / k;  // Valid if k = 1
        Q[t] = new double[k];
        Q[t][t] = 0;
        for (int j = 0; j < t; j++)
        {
            Q[t][t] += r[j][t] * r[j][t];
            Q[t][j] = Q[j][t];
        }
        for (int j = t + 1; j < k; j++)
        {
            Q[t][t] += r[j][t] * r[j][t];
            Q[t][j] = -r[j][t] * r[t][j];
        }
    }
    for (iter = 0; iter < max_iter; iter++)
    {
        // stopping condition, recalculate QP,pQP for numerical accuracy
        pQp = 0;
        for (int t = 0; t < k; t++)
        {
            Qp[t] = 0;
            for (int j = 0; j < k; j++)
            {
                Qp[t] += Q[t][j] * p[j];
            }
            pQp += p[t] * Qp[t];
        }
        double max_error = 0;
        for (int t = 0; t < k; t++)
        {
            double error = fabs(Qp[t] - pQp);
            if (error > max_error)
            {
                max_error = error;
            }
        }
        if (max_error < eps) 
        {
            break;
        }
        
        for (int t = 0; t < k; t++)
        {
            double diff= (-Qp[t] + pQp) / Q[t][t];
            p[t] += diff;
            pQp = (pQp + diff * (diff * Q[t][t] + 2 * Qp[t])) / (1 + diff) / (1 + diff);
            for (int j = 0; j < k; j++)
            {
                Qp[j] = (Qp[j] + diff * Q[t][j]) / (1 + diff);
                p[j] /= (1 + diff);
            }
        }
    }
    for (int t = 0; t < k; t++) 
    {
        delete[] Q[t];
    }
    delete[] Q;
    delete[] Qp;
}

// Cross-validation decision values for probability estimates
static void svm_binary_svc_probability(
    const SVMTrainData* problem, const SVMParameters* params,
    double Cp, double Cn, double& probA, double& probB)
{
    int nr_fold = 5;
    int* perm = new int[problem->size()];
    double* dec_values = new double[problem->size()];

    // random shuffle
    for (unsigned int i = 0; i < problem->size(); i++)
    {
        perm[i] = i;
    }
    for (unsigned int i = 0; i <problem->size(); i++)
    {
        int j = i + rand() % (problem->size() - i);
        swap(perm[i], perm[j]);
    }
    for (int i = 0; i < nr_fold; i++)
    {
        int begin = i * problem->size() / nr_fold;
        int end = (i + 1) * problem->size() / nr_fold;

        SVMTrainData subprob(problem->size() - (end - begin));            
        int k = 0;
        for (int j = 0; j < begin; j++)
        {
            subprob.features[k] = problem->features[perm[j]];
            subprob.responses[k] = problem->responses[perm[j]];
            k++;
        }
        for (unsigned int j = end; j <problem->size(); j++)
        {
            subprob.features[k] = problem->features[perm[j]];
            subprob.responses[k] = problem->responses[perm[j]];
            k++;
        }
        int p_count = 0, n_count = 0;
        for (int j = 0; j < k; j++)
        {
            if (subprob.responses[j] > 0)
            {
                p_count++;
            }
            else
            {
                n_count++;
            }
        }
        if (p_count == 0 && n_count == 0)
        {
            for (int j = begin; j < end; j++)
            {
                dec_values[perm[j]] = 0;
            }
        }
        else
        {
            if (p_count > 0 && n_count == 0)
            {
                for (int j = begin; j < end; j++)
                {
                    dec_values[perm[j]] = 1;
                }
            }
            else 
            {
                if (p_count == 0 && n_count > 0)
                {
                    for (int j = begin; j < end; j++)
                    {
                        dec_values[perm[j]] = -1;
                    }
                }
                else
                {
                    SVMParameters subparam = *params;
                    subparam.estimateProbability = false;
                    subparam.C = 1.0;
                    subparam.weightCount = 2;
                    subparam.weightLabels.resize(2);
                    subparam.weights.resize(2);
                    subparam.weightLabels[0] = +1;
                    subparam.weightLabels[1] = -1;
                    subparam.weights[0] = Cp;
                    subparam.weights[1] = Cn;
                    LibSVMModel* submodel = svmTrain(&subprob, &subparam);
                    for (int j = begin; j < end; j++)
                    {
                        svmPredictValues(submodel, problem->features[perm[j]], &(dec_values[perm[j]])); 
                        // ensure +1 -1 order; reason not using CV subroutine
                        dec_values[perm[j]] *= submodel->labels[0];
                    }        
                    if (submodel != NULL)
                    {
                        delete submodel;
                    }
                }
            }
        }
    }        
    sigmoid_train(problem->size(), dec_values, problem->responses, probA, probB);
    delete[] dec_values;
    delete[] perm;
}

// Return parameter of a Laplace distribution 
static double svm_svr_probability(const SVMTrainData* problem, const SVMParameters* params)
{
    int nr_fold = 5;
    double* ymv = new double[problem->size()];
    double mae = 0;

    SVMParameters newparam = *params;
    newparam.estimateProbability = false;
    svmCrossValidation(problem, &newparam, nr_fold, ymv);
    for (unsigned int i = 0; i < problem->size(); i++)
    {
        ymv[i] = problem->responses[i] - ymv[i];
        mae += fabs(ymv[i]);
    }        
    mae /= problem->size();
    double std = sqrt(2 * mae * mae);
    mae = 0;
    int count = 0;
    for (unsigned int i = 0; i < problem->size(); i++)
    {
        if (fabs(ymv[i]) > 5 * std) 
        {
            count++;
        }
        else 
        {
            mae += fabs(ymv[i]);
        }
    }
    mae /= (problem->size() - count);
    delete[] ymv;
    return mae;
}


// label: label name, start: begin of each class, count: #data of classes, perm: indices to the original data
// perm, length l, must be allocated before calling this subroutine
static void svm_group_classes(const SVMTrainData* problem, int* nr_class_ret, int** label_ret, int** start_ret, int** count_ret, int* perm)
{
    int l = problem->size();
    int max_nr_class = 16;
    int class_count = 0;
    int* label = new int[max_nr_class];
    int* count = new int[max_nr_class];
    int* data_label = new int[l];   

    for (int i = 0; i < l; i++)
    {
        int this_label = (int)problem->responses[i];
        int j;
        for (j = 0; j < class_count; j++)
        {
            if (this_label == label[j])
            {
                count[j]++;
                break;
            }
        }
        data_label[i] = j;
        if (j == class_count)
        {
            if (class_count == max_nr_class)
            {
                int* tmp1 = new int[max_nr_class * 2];
                int* tmp2 = new int[max_nr_class * 2];
                for (int p = 0; p < max_nr_class; p++)
                {
                    tmp1[p] = label[p];
                    tmp2[p] = count[p];
                }
                delete[] label;
                delete[] count;
                label = tmp1;
                label = tmp2;
                max_nr_class *= 2;
            }
            label[class_count] = this_label;
            count[class_count] = 1;
            class_count++;
        }
    }

    int* start = new int[class_count];
    start[0] = 0;
    for (int i = 1; i < class_count; i++)
    {
        start[i] = start[i-1] + count[i-1];
    }
    for (int i = 0; i < l; i++)
    {
        perm[start[data_label[i]]] = i;
        start[data_label[i]]++;
    }
    start[0] = 0;
    for(int i = 1; i < class_count; i++)
    {
        start[i] = start[i-1] + count[i-1];
    }

    *nr_class_ret = class_count;
    *label_ret = label;
    *start_ret = start;
    *count_ret = count;
    delete[] data_label;
}

//
// Interface functions
//
LibSVMModel* svmTrain(const SVMTrainData *problem, const SVMParameters *params)
{
    LibSVMModel* model = new LibSVMModel();
    model->params = (*params);

    if(params->svmType == ONE_CLASS || params->svmType == EPSILON_SVR || params->svmType == NU_SVR)
    {
        // regression or one-class-svm
        model->class_count = 2;
        model->labels = NULL;
        model->numSVForClasses = NULL;
        model->probA = NULL; 
        model->probB = NULL;
        model->sv_coef = new double*[1];

        if(params->estimateProbability && (params->svmType == EPSILON_SVR || params->svmType == NU_SVR))
        {
            model->probA = new double[1];
            model->probA[0] = svm_svr_probability(problem, params);
        }

        decision_function f = svm_train_one(problem, params, 0, 0);
        model->rho = new double[1];
        model->rho[0] = f.rho;

        int nSV = 0;
        for (unsigned int i = 0; i < problem->size(); i++)
        {
            if (fabs(f.alpha[i]) > 0) 
            {
                nSV++;
            }
        }
        model->sv_count = nSV;
        model->SV = new SVMVector[nSV];
        model->sv_coef[0] = new double[nSV];
        model->sv_indices = new int[nSV];
        for (unsigned int i = 0, j = 0; i < problem->size(); i++)
        {
            if (fabs(f.alpha[i]) > 0)
            {
                model->SV[j].setData( problem->features[i] );
                model->sv_coef[0][j] = f.alpha[i];
                model->sv_indices[j] = i + 1;
                j++;
            }        
        }
        delete[] f.alpha;
    }
    else
    {
        // classification
        int l = problem->size();
        int class_count;
        int* label = NULL;
        int* start = NULL;
        int* count = NULL;
        int* perm = new int[l];

        // group training data of the same class
        svm_group_classes(problem, &class_count, &label, &start, &count, perm);
        if (class_count == 1) 
        {
            IVCP_LOG_WARNING("Training data in only one class");
        }
        
        SVMVector* features = new SVMVector[l];
        for (int i = 0; i < l; i++)
        {
            features[i] = problem->features[perm[i]];
        }

        // calculate weighted C

        double* weighted_C = new double[class_count];
        for (int i = 0; i < class_count; i++)
        {
            weighted_C[i] = params->C;
        }
        for (int i = 0; i < params->weightCount; i++)
        {    
            int j;
            for (j = 0; j < class_count; j++) 
            {
                if (params->weightLabels[i] == label[j])
                    break;
            }
            if (j == class_count)
            {
                IVCP_LOG_WARNING("Class label %d specified in weights is not found", params->weightLabels[i]);
            }
            else
            {
                weighted_C[j] *= params->weights[i];
            }
        }

        // train k*(k-1)/2 models
        
        bool* nonzero = new bool[l];
        for (int i = 0; i < l; i++)
        {
            nonzero[i] = false;
        }

        int combCount = class_count * (class_count - 1) / 2;
        decision_function* f = new decision_function[combCount];

        double* probA = NULL;
        double* probB = NULL;
        if (params->estimateProbability && params->svmType != ONE_CLASS)
        {
            probA = new double[combCount];
            probB = new double[combCount];
        }

        int p = 0;
        for (int i = 0; i < class_count; i++)
        {
            for (int j = i + 1; j < class_count; j++)
            {
                int si = start[i], sj = start[j];
                int ci = count[i], cj = count[j];
                SVMTrainData sub_prob(ci + cj);
                for (int k = 0; k < ci; k++)
                {
                    sub_prob.features[k] = features[si+k];
                    sub_prob.responses[k] = 1;
                }
                for (int k = 0; k < cj; k++)
                {
                    sub_prob.features[ci+k] = features[sj+k];
                    sub_prob.responses[ci+k] = -1;
                }

                if (params->estimateProbability && params->svmType != ONE_CLASS)
                {
                    svm_binary_svc_probability(&sub_prob, params, weighted_C[i], weighted_C[j], probA[p], probB[p]);
                }

                f[p] = svm_train_one(&sub_prob, params, weighted_C[i], weighted_C[j]);
                for (int k = 0; k < ci; k++)
                {
                    if (!nonzero[si+k] && fabs(f[p].alpha[k]) > 0)
                    {
                        nonzero[si+k] = true;
                    }
                }
                for (int k = 0; k < cj; k++)
                {
                    if (!nonzero[sj+k] && fabs(f[p].alpha[ci+k]) > 0)
                    {
                        nonzero[sj+k] = true;
                    }
                }
                p++;
            }
        }

        // build output
        model->class_count = class_count;        
        model->labels = new int[class_count];

        for (int i = 0; i < class_count; i++)
        {
            model->labels[i] = label[i];
        }
        
        model->rho = new double[combCount];
        for (int i = 0; i < combCount; i++)
        {
            model->rho[i] = f[i].rho;
        }

        if(params->estimateProbability && params->svmType != ONE_CLASS)
        {
            model->probA = new double[combCount];
            model->probB = new double[combCount];
            for (int i = 0; i < combCount; i++)
            {
                model->probA[i] = probA[i];
                model->probB[i] = probB[i];
            }
        }
        else
        {
            model->probA = NULL;
            model->probB = NULL;
        }

        int total_sv = 0;
        int* nz_count = new int[class_count];
        model->numSVForClasses = new int[class_count];
        for (int i = 0; i < class_count; i++)
        {
            int nSV = 0;
            for (int j = 0; j < count[i]; j++)
            {
                if (nonzero[start[i] + j])
                {    
                    nSV++;
                    total_sv++;
                }
            }
            model->numSVForClasses[i] = nSV;
            nz_count[i] = nSV;
        }
        
        model->sv_count = total_sv;
        model->SV = new SVMVector[total_sv];
        model->sv_indices = new int[total_sv];
        p = 0;
        for (int i = 0; i < l; i++)
        {
            if(nonzero[i])
            {
                model->SV[p].setData( features[i] );
                model->sv_indices[p++] = perm[i] + 1;
            }
        }

        int* nz_start = new int[class_count];
        nz_start[0] = 0;
        for (int i = 1; i < class_count; i++)
        {
            nz_start[i] = nz_start[i-1] + nz_count[i-1];
        }

        model->sv_coef = new double*[class_count-1];
        for (int i = 0; i < class_count - 1; i++)
        {
            model->sv_coef[i] = new double[total_sv];
        }

        p = 0;
        for (int i = 0; i < class_count; i++)
        {
            for (int j = i + 1; j < class_count; j++)
            {
                // classifier (i,j): coefficients with
                // i are in sv_coef[j-1][nz_start[i]...],
                // j are in sv_coef[i][nz_start[j]...]

                int si = start[i];
                int sj = start[j];
                int ci = count[i];
                int cj = count[j];
                
                int q = nz_start[i];
                for (int k = 0; k < ci; k++)
                {
                    if (nonzero[si + k])
                    {
                        model->sv_coef[j-1][q++] = f[p].alpha[k];
                    }
                }
                q = nz_start[j];
                for (int k = 0; k < cj; k++)
                {
                    if (nonzero[sj + k])
                    {
                        model->sv_coef[i][q++] = f[p].alpha[ci+k];
                    }
                }
                p++;
            }
        }

        delete[] label;
        delete[] count;
        delete[] perm;
        delete[] start;
        delete[] probA;
        delete[] probB;        
        delete[] features;
        delete[] weighted_C;
        delete[] nonzero;
        for (int i = 0; i < combCount; i++)
        {
            delete[] f[i].alpha;
        }
        delete[] f;
        delete[] nz_count;
        delete[] nz_start;
    }
    return model;
}

// Stratified cross validation
void svmCrossValidation(const SVMTrainData* problem, const SVMParameters* params, int nr_fold, double* target)
{
    int* fold_start = new int[nr_fold + 1];
    int numberOfFeatures = problem->size();
    int* perm = new int[numberOfFeatures];
    int class_count;

    // stratified cv may not give leave-one-out rate
    // Each class to l folds -> some folds may have zero elements
    if ((params->svmType == C_SVC || params->svmType == NU_SVC) && nr_fold < numberOfFeatures)
    {
        int* start = NULL;
        int* label = NULL;
        int* count = NULL;
        svm_group_classes(problem, &class_count, &label, &start, &count, perm);

        // random shuffle and then data grouped by fold using the array perm
        int* fold_count = new int[nr_fold];
        int* index = new int[numberOfFeatures];
        for (int i = 0; i < numberOfFeatures; i++)
        {
            index[i] = perm[i];
        }
        for (int c = 0; c < class_count; c++) 
        {
            for(int i = 0; i <count[c]; i++)
            {
                int j = i + rand() % (count[c] - i);
                swap(index[start[c] + j], index[start[c] + i]);
            }
        }
        for(int i = 0; i < nr_fold; i++)
        {
            fold_count[i] = 0;
            for (int c = 0; c < class_count; c++)
            {
                fold_count[i] += (i + 1) * count[c] / nr_fold - i * count[c] / nr_fold;
            }
        }
        fold_start[0] = 0;
        for (int i = 1; i <= nr_fold; i++)
        {
            fold_start[i] = fold_start[i-1] + fold_count[i-1];
        }
        for (int c = 0; c < class_count; c++)
        {
            for(int i = 0; i < nr_fold; i++)
            {
                int begin = start[c] + i * count[c] / nr_fold;
                int end = start[c] + (i + 1) * count[c] / nr_fold;
                for (int j = begin; j < end; j++)
                {
                    perm[fold_start[i]] = index[j];
                    fold_start[i]++;
                }
            }
        }
        fold_start[0] = 0;
        for (int i = 1; i <= nr_fold; i++)
        {
            fold_start[i] = fold_start[i-1] + fold_count[i-1];
        }
        delete[] start;    
        delete[] label;
        delete[] count;    
        delete[] index;
        delete[] fold_count;
    }
    else
    {
        for (int i = 0; i < numberOfFeatures; i++) 
        {
            perm[i] = i;
        }
        for (int i = 0; i < numberOfFeatures; i++)
        {
            int j = i + rand() % (numberOfFeatures - i);
            swap(perm[i], perm[j]);
        }
        for (int i = 0; i <= nr_fold; i++)
        {
            fold_start[i] = i * numberOfFeatures / nr_fold;
        }
    }

    for (int i = 0; i < nr_fold; i++)
    {
        int begin = fold_start[i];
        int end = fold_start[i+1];

        SVMTrainData subprob(numberOfFeatures - (end - begin));            
        int k = 0;
        for (int j = 0; j < begin; j++)
        {
            subprob.features[k] = problem->features[perm[j]];
            subprob.responses[k] = problem->responses[perm[j]];
            k++;
        }
        for (int j = end; j < numberOfFeatures; j++)
        {
            subprob.features[k] = problem->features[perm[j]];
            subprob.responses[k] = problem->responses[perm[j]];
            k++;
        }

        LibSVMModel* submodel = svmTrain(&subprob, params);
        if (params->estimateProbability && (params->svmType == C_SVC || params->svmType == NU_SVC))
        {
            double *prob_estimates = new double[submodel->class_count];
            for (int j = begin; j < end; j++)
            {
                target[perm[j]] = svmPredictProbability(submodel, problem->features[perm[j]], prob_estimates);
            }
            delete[] prob_estimates;
        }
        else
        {
            for(int j = begin; j < end; j++)
            {
                target[perm[j]] = svmPredict(submodel, problem->features[perm[j]]);
            }
        }
        if (submodel != NULL)
        {
            delete submodel;
        }
    }
    delete[] fold_start;
    delete[] perm;
}

double svmPredictValues(const LibSVMModel* model, const SVMVector& feature, double* dec_values)
{
    if(model->params.svmType == ONE_CLASS ||
       model->params.svmType == EPSILON_SVR ||
       model->params.svmType == NU_SVR)
    {
        double* sv_coef = model->sv_coef[0];
        double sum = 0;
        for (int i = 0; i < model->sv_count; i++)
        {
            sum += sv_coef[i] * Kernel::k_function(feature, model->SV[i], model->params);
        }
        sum -= model->rho[0];
        *dec_values = sum;

        if (model->params.svmType == ONE_CLASS)
        {
            return (sum > 0) ? 1 : -1;
        }
        else
        {
            return sum;
        }
    }
    else
    {
        int class_count = model->class_count;
        int sv_count = model->sv_count;
        
        double* kvalue = new double[sv_count];
        for (int i = 0; i < sv_count; i++)
        {
            kvalue[i] = Kernel::k_function(feature, model->SV[i], model->params);
        }

        int* start = new int[class_count];
        start[0] = 0;
        for (int i = 1; i < class_count; i++)
        {
            start[i] = start[i-1] + model->numSVForClasses[i-1];
        }

        int* vote = new int[class_count];
        for (int i = 0; i < class_count; i++)
        {
            vote[i] = 0;
        }

        int p = 0;
        for (int i = 0; i < class_count; i++)
        {
            for (int j = i + 1; j < class_count; j++)
            {
                double sum = 0;
                int si = start[i];
                int sj = start[j];
                int ci = model->numSVForClasses[i];
                int cj = model->numSVForClasses[j];
                
                double* coef1 = model->sv_coef[j-1];
                double* coef2 = model->sv_coef[i];
                for (int k = 0; k < ci; k++)
                {
                    sum += coef1[si + k] * kvalue[si + k];
                }
                for (int k = 0; k < cj; k++)
                {
                    sum += coef2[sj + k] * kvalue[sj + k];
                }
                sum -= model->rho[p];
                dec_values[p] = sum;

                if (dec_values[p] > 0)
                {
                    vote[i]++;
                }
                else
                {
                    vote[j]++;
                }
                p++;
            }
        }

        int vote_max_idx = 0;
        for (int i = 1; i < class_count; i++)
        {
            if (vote[i] > vote[vote_max_idx])
            {
                vote_max_idx = i;
            }
        }

        delete[] kvalue;
        delete[] start;
        delete[] vote;

        return model->labels[vote_max_idx];
    }
}

double svmPredict(const LibSVMModel* model, const SVMVector& feature)
{
    int class_count = model->class_count;
    double* dec_values;
    if(model->params.svmType == ONE_CLASS ||
       model->params.svmType == EPSILON_SVR ||
       model->params.svmType == NU_SVR)
    {
        dec_values = new double[1];
    }
    else 
    {
        dec_values = new double[class_count * (class_count - 1) / 2];
    }
    double pred_result = svmPredictValues(model, feature, dec_values);
    delete[] dec_values;
    return pred_result;
}

double svmPredictProbability(const LibSVMModel* model, const SVMVector& feature, double* prob_estimates)
{
    if ((model->params.svmType == C_SVC || model->params.svmType == NU_SVC) &&
        model->probA != NULL && model->probB != NULL)
    {
        int class_count = model->class_count;
        double* dec_values = new double[class_count * (class_count - 1) / 2];
        svmPredictValues(model, feature, dec_values);

        double min_prob = 1e-7;
        double** pairwise_prob = new double*[class_count];
        for (int i = 0; i < class_count; i++)
        {
            pairwise_prob[i] = new double[class_count];
        }
        int k = 0;
        for (int i = 0; i < class_count; i++)
        {
            for(int j = i + 1; j < class_count; j++)
            {
                pairwise_prob[i][j] = min(max(sigmoid_predict(dec_values[k], model->probA[k], model->probB[k]), min_prob), 1 - min_prob);
                pairwise_prob[j][i] = 1 - pairwise_prob[i][j];
                k++;
            }
        }
        multiclass_probability(class_count, pairwise_prob, prob_estimates);

        int prob_max_idx = 0;
        for (int i = 1; i < class_count; i++)
        {
            if (prob_estimates[i] > prob_estimates[prob_max_idx])
            {
                prob_max_idx = i;
            }
        }
        for (int i = 0; i < class_count; i++)
        {
            delete[] pairwise_prob[i];
        }

        delete[] dec_values;
        delete[] pairwise_prob;
        return model->labels[prob_max_idx];
    }
    else 
    {
        return svmPredict(model, feature);
    }
}
};