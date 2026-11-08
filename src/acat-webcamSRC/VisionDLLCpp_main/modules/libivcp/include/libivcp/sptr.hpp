///////////////////////////////////////////////////////////////////////////
// <copyright file="sptr.hpp" company="Intel Corporation">
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


#ifndef __SPTR_HPP__
#define __SPTR_HPP__

#include "libivcp/log.hpp"
#include "libivcp/bptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file sptr.hpp
 * \brief Shared smart pointer class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Shared smart pointer
 */
template<typename Type> class IVCP_EXPORT SPtr: public BPtr<Type>
{
public:
    /*!
     * \brief Default constructor
     */
    SPtr() {};

    /*!
     * \brief Default destructor
     */
    virtual ~SPtr();

    /*!
     * \brief A copy constructor which assignes a pointer of an object of the same type
     */
    SPtr(const SPtr& ptr) : BPtr<Type>(ptr) { addRef(); };

    /*!
     * \brief A copy constructor which assignes a pointer of an object of different type
     * \details When assigning a pointer it is first casted to the type of our pointer. If the cast is failed
     * the object will contain a zero-pointer
     */
    template<typename _Tp2> SPtr(const BPtr<_Tp2>& ptr) : BPtr<Type>(ptr) { addRef(); };

    /*!
     * \brief Cast pointer to the stored object to a new type and return a smart pointer of that type
     * \details In case cast fails pointer to a NULL pointer will be returned
     */
    template<typename _Tp2> SPtr<_Tp2> ptr();

    /*!
     * \brief Cast pointer to the stored object to a new type and return a smart pointer of that type
     * \details In case cast fails pointer to a NULL pointer will be returned
     */
    template<typename _Tp2> const SPtr<_Tp2> ptr() const;

    virtual void release();

    /*!
     * \brief Constructor
     * \param[in] _obj Pointer to the object that this smart pointer will contain reference to 
     */
    SPtr(Type* _obj);

    /*!
     * \brief Get pointer to stored type
     */
    Type* operator -> ();

    /*!
     * \brief Get const pointer to stored type
     */
    const Type* operator -> () const;

    /*!
     * \brief Convert to stored type
     */
    operator Type* ();

    /*!
     * \brief Convert to const stored type
     */
    operator const Type* () const;

protected:
    virtual void addRef();
};

template<typename Type> 
inline void SPtr<Type>::addRef()
{ 
    if( this->refPassiveCount ) 
    {
        this->_XADD(this->refPassiveCount, 1); 
    }

    if ( this->refCount )
    {
        this->_XADD(this->refCount, 1);
    }
}

template<typename Type> 
inline void SPtr<Type>::release()
{
    if ( this->refCount )
    {
        if(this->_XADD(this->refCount, -1) == 1 )
        {
            if( this->obj ) 
            {
                delete this->obj; 
            }
        }
    }

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
inline SPtr<Type>::~SPtr() 
{ 
    release(); 
}

template<typename Type> 
inline SPtr<Type>::SPtr(Type* _obj)
{
    this->obj = _obj;
    if (this->obj)
    {
        this->refCount = new int;
        this->refPassiveCount = new int;
        *(this->refCount) = 1;
        *(this->refPassiveCount) = 1;
    }
    else
    {
        this->refCount = 0;
        this->refPassiveCount = 0;
    }
}

template<typename Type> 
inline Type* SPtr<Type>::operator -> () 
{ 
    return this->obj; 
}

template<typename Type> 
inline const Type* SPtr<Type>::operator -> () const
{ 
    return this->obj; 
}

template<typename Type> 
inline SPtr<Type>::operator Type* () 
{ 
    return this->obj; 
}

template<typename Type> 
inline SPtr<Type>::operator const Type* () const 
{ 
    return this->obj; 
}

template<typename Type> template<typename _Tp2> 
inline SPtr<_Tp2> SPtr<Type>::ptr()
{
    return SPtr<_Tp2>(*this);
}

template<typename Type> template<typename _Tp2> 
inline const SPtr<_Tp2> SPtr<Type>::ptr() const
{
    return SPtr<_Tp2>(*this);
}

};
/** @}*/
#endif
