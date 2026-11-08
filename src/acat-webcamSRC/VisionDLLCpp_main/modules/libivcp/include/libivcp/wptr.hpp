///////////////////////////////////////////////////////////////////////////
// <copyright file="wptr.hpp" company="Intel Corporation">
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


#ifndef __WPTR_HPP__
#define __WPTR_HPP__

#include "libivcp/sptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file wptr.hpp
 * \brief Weak shared smart pointer class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Weak smart pointer
 * \details The weak smmart pointer is used in internal classes such as Repository where
 * it is necessary to store reference to created object (cached references). Weak pointers do not prevent object
 * from deleting (ie an object will be deleted once it has no references of SPtr, no matter how many WPtr references it has)
 */
template<typename Type> class IVCP_EXPORT WPtr: public BPtr<Type>
{
public:
    /*!
     * \brief Default constructor
     */
    WPtr(){};

    /*!
     * \brief Default destructor
     */
    virtual ~WPtr();

    /*!
     * \brief A copy constructor which assignes a pointer of an object of the same type
     */
    WPtr(const WPtr& ptr) : BPtr<Type>(ptr) { addRef(); };

    /*!
     * \brief A copy constructor which assignes a pointer of an object of different type
     * \details When assigning a pointer it is first casted to the type of our pointer. If the cast is failed
     * the object will contain a zero-pointer
     */
    template<typename _Tp2> WPtr(const BPtr<_Tp2>& ptr) : BPtr<Type>(ptr) { addRef(); };

    /*!
     * \brief Cast pointer to the stored object to a new type and return a weak pointer of that type
     * \details In case cast fails pointer to a NULL pointer will be returned
     */
    template<typename _Tp2> WPtr<_Tp2> ptr();

    /*!
     * \brief Cast pointer to the stored object to a new type and return a weak pointer of that type
     * \details In case cast fails pointer to a NULL pointer will be returned
     */
    template<typename _Tp2> const WPtr<_Tp2> ptr() const;

    virtual void release();

    /*!
     * \brief Get smart pointer to stored type
     */
    SPtr<Type> operator -> ();

    /*!
     * \brief Get const smart pointer to stored type
     */
    const SPtr<Type> operator -> () const;

    /*!
     * \brief Convert to smart pointer
     */
    operator SPtr<Type> ();

    /*!
     * \brief Convert to const smart pointer
     */
    operator const SPtr<Type> () const;

protected:
    virtual void addRef();
};

template<typename Type> 
inline void WPtr<Type>::addRef()
{ 
    if( this->refPassiveCount ) 
    {
        this->_XADD(this->refPassiveCount, 1); 
    }
}

template<typename Type> 
inline void WPtr<Type>::release()
{
    if (this->refPassiveCount && this->_XADD(this->refPassiveCount, -1) == 1)
    {
        delete this->refCount;
        delete this->refPassiveCount;
    }

    this->refCount = 0;
    this->refPassiveCount = 0;
    this->obj = 0;
}

template<typename Type> 
inline WPtr<Type>::~WPtr() 
{ 
    release(); 
}

template<typename Type> 
inline SPtr<Type> WPtr<Type>::operator -> () 
{ 
    return SPtr<Type>(*this); 
}

template<typename Type> 
inline const SPtr<Type> WPtr<Type>::operator -> () const
{ 
    return SPtr<Type>(*this); 
}

template<typename Type> 
inline WPtr<Type>::operator SPtr<Type> () 
{ 
    return SPtr<Type>(*this); 
}

template<typename Type> 
inline WPtr<Type>::operator const SPtr<Type>() const 
{ 
    return SPtr<Type>(*this); 
}

template<typename Type> template<typename _Tp2> 
inline WPtr<_Tp2> WPtr<Type>::ptr()
{
    return WPtr<_Tp2>(*this);
}

template<typename Type> template<typename _Tp2> 
inline const WPtr<_Tp2> WPtr<Type>::ptr() const
{
    return WPtr<_Tp2>(*this);
}

};
/** @}*/
#endif
