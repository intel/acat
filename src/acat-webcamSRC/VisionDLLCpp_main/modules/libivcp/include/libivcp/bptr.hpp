///////////////////////////////////////////////////////////////////////////
// <copyright file="bptr.hpp" company="Intel Corporation">
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


#ifndef __BPTR_HPP__
#define __BPTR_HPP__

#include "libivcp/types.hpp"
#include "libivcp/log.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file bptr.hpp
 * \brief Basic smart pointer class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic smart pointer
 */
template<typename Type> class IVCP_EXPORT BPtr
{
public:
    /*!
     * \brief Default constructor
     */
    BPtr();

    /*!
     * \brief Default destructor
     */
    virtual ~BPtr() {};

    /*!
     * \brief A copy constructor which assignes a pointer of an object of the same type
     */
    BPtr(const BPtr& ptr);

    /*!
     * \brief A copy constructor which assignes a pointer of an object of different type
     * \details When assigning a pointer it is first casted to the type of our pointer. If the cast is failed
     * the object will contain a zero-pointer
     */
    template<typename _Tp2> BPtr(const BPtr<_Tp2>& ptr);

    /*!
     * \brief An assign operator which assignes a pointer of an object of the same type
     */
    BPtr& operator = (const BPtr& ptr);

    /*!
     * \brief Compare two pointers
     * \details The smart pointers are equal if they point to the same object
     */
    bool operator == (const BPtr& ptr) const;

    /*!
     * \brief Compare two pointers of different types
     * \details The smart pointers are equal if they point to the same object. 
     * The second pointer is casted to the first pointer. If dynamic_cast fails exeption is thrown
     * \throw IncorrectParamException Unable to cast the second pointer to the first pointer
     */
    template<typename _Tp2> bool operator == (const BPtr<_Tp2>& ptr) const;

    /*!
     * \brief An assign operator which assignes a pointer of an object of the same type
     */
    virtual void release() = 0;

    /*!
     * \brief Check if the smart pointer has active (non-weak) references
     */
    bool isEmpty() const;

protected:

    /*!
     * \brief Pointer to the object the smart pointer stores
     */
    Type* obj; 

    /*!
     * \brief Number of references to the object
     */
    int* refCount; 

    /*!
     * \brief Number of passive (weak) references to the object
     * \sa WPtr
     */
    int* refPassiveCount;

    /*!
     * \brief Increase value stored at the addr address by delta
     * \param[in,out] addr Pointer to integer variable which shall be updatted
     * \param delta Value (possibly negative) which shall be added 
     */
    static int _XADD(int* addr, int delta);

    /*!
     * \brief Increase number of references to the object by one
     */
    virtual void addRef() = 0;

    template<typename _Tp2> friend class BPtr;
};

template<typename Type> 
inline int BPtr<Type>::_XADD(int* addr, int delta) 
{ 
    int tmp = *addr; 
    *addr += delta; 
    return tmp; 
}

template<typename Type> 
inline BPtr<Type>::BPtr() : 
    obj(0), refCount(0), refPassiveCount(0)
{
}

template<typename Type> 
inline BPtr<Type>::BPtr(const BPtr& _ptr) : 
    obj(0), refCount(0), refPassiveCount(0)
{ 
    if(!_ptr.isEmpty())
    {
        obj = _ptr.obj; 
        refCount = _ptr.refCount; 
        refPassiveCount = _ptr.refPassiveCount;
    }
}

template<typename Type> 
inline BPtr<Type>& BPtr<Type>::operator = (const BPtr<Type>& _ptr)
{
    release();

    if(!_ptr.isEmpty())
    {
        obj = _ptr.obj; 
        refCount = _ptr.refCount; 
        refPassiveCount = _ptr.refPassiveCount;
        addRef();
    }
    return *this;
}

template<typename Type> 
inline bool BPtr<Type>::operator == (const BPtr& _ptr) const
{
    if(_ptr.isEmpty())
    {
        return isEmpty();
    }

    if(isEmpty() || obj != _ptr.obj)
    {
        return false;
    }

    if(refCount != _ptr.refCount || refPassiveCount != _ptr.refPassiveCount)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Heap memory handling error detected in smart pointers. Looks like the heap object is shared by separate smart pointers");
    }

    return true;
}
template<typename Type> template<typename _Tp2>
inline bool BPtr<Type>::operator == (const BPtr<_Tp2>& _ptr) const
{
    if(_ptr.isEmpty())
    {
        return isEmpty();
    }

    Type* p_casted = dynamic_cast<Type*>(_ptr.obj);
    if(isEmpty() || !p_casted || obj != p_casted)
    {
        return false;
    }

    if(refCount != _ptr.refCount || refPassiveCount != _ptr.refPassiveCount)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Heap memory handling error detected in smart pointers. Looks like the heap object is shared by separate smart pointers");
    }

    return true;
}

template<typename Type> template<typename _Tp2> 
BPtr<Type>::BPtr(const BPtr<_Tp2>& p)
    : obj(0), refCount(0), refPassiveCount(0)
{
    if (p.isEmpty())
    {
        return;
    }

    Type* p_casted = dynamic_cast<Type*>(p.obj);
    if (!p_casted)
    {
        IVCP_LOG_WARNING("Unable to cast a pointer of '%s' to a pointer of type '%s'", 
            typeid(p.obj).name(), typeid(Type*).name());
        return;
    }

    obj = p_casted;
    refCount = p.refCount;
    refPassiveCount = p.refPassiveCount;
}

template<typename Type> 
bool BPtr<Type>::isEmpty() const
{
    return !((refCount) && (*refCount > 0));
}

}
/** @}*/
#endif
