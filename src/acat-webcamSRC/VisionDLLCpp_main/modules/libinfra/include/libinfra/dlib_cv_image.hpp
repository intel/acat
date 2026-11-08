/*
 * dlib_cv_image.hpp
 *
 * OpenCV 5 removed the C API (IplImage, types_c.h), which the stock
 * dlib/opencv/cv_image.h depends on. This header provides a drop-in
 * dlib::cv_image<pixel_type> built directly on cv::Mat so that the vendored
 * dlib sources do not need to be modified.
 *
 * Like the original, cv_image does not own the pixel data: the cv::Mat it was
 * created from must outlive it.
 */
#ifndef LIBINFRA_DLIB_CV_IMAGE_HPP_
#define LIBINFRA_DLIB_CV_IMAGE_HPP_

#include <opencv2/core.hpp>
#include <dlib/algs.h>
#include <dlib/pixel.h>
#include <dlib/matrix/matrix_mat.h>
#include <dlib/image_processing/generic_image.h>

namespace dlib
{
    template <typename pixel_type>
    class cv_image
    {
    public:
        typedef pixel_type type;
        typedef default_memory_manager mem_manager_type;

        cv_image(const cv::Mat& img)
        {
            DLIB_CASSERT(img.depth() == cv::DataType<typename pixel_traits<pixel_type>::basic_pixel_type>::depth &&
                         img.channels() == pixel_traits<pixel_type>::num,
                         "The pixel type you gave doesn't match pixel used by the open cv Mat object."
                         << "\n\t img.depth():    " << img.depth()
                         << "\n\t img.channels(): " << img.channels()
                         << "\n\t pixel_traits<pixel_type>::num: " << pixel_traits<pixel_type>::num);

            _data = reinterpret_cast<char*>(img.data);
            _widthStep = static_cast<long>(img.step);
            _nr = img.rows;
            _nc = img.cols;
        }

        cv_image() : _data(0), _widthStep(0), _nr(0), _nc(0) {}

        size_t size() const { return static_cast<size_t>(_nr * _nc); }

        inline pixel_type* operator[](const long row)
        {
            DLIB_ASSERT(0 <= row && row < nr(), "out of bounds row " << row << ", nr(): " << nr());
            return reinterpret_cast<pixel_type*>(_data + _widthStep * row);
        }

        inline const pixel_type* operator[](const long row) const
        {
            DLIB_ASSERT(0 <= row && row < nr(), "out of bounds row " << row << ", nr(): " << nr());
            return reinterpret_cast<const pixel_type*>(_data + _widthStep * row);
        }

        inline const pixel_type& operator()(const long row, const long column) const
        {
            DLIB_ASSERT(0 <= column && column < nc(), "out of bounds column " << column << ", nc(): " << nc());
            return (*this)[row][column];
        }

        inline pixel_type& operator()(const long row, const long column)
        {
            DLIB_ASSERT(0 <= column && column < nc(), "out of bounds column " << column << ", nc(): " << nc());
            return (*this)[row][column];
        }

        long nr() const { return _nr; }
        long nc() const { return _nc; }
        long width_step() const { return _widthStep; }

    private:
        char* _data;
        long _widthStep;
        long _nr;
        long _nc;
    };

    template <typename T>
    const matrix_op<op_array2d_to_mat<cv_image<T> > > mat(const cv_image<T>& m)
    {
        typedef op_array2d_to_mat<cv_image<T> > op;
        return matrix_op<op>(op(m));
    }

    template <typename T>
    struct image_traits<cv_image<T> >
    {
        typedef T pixel_type;
    };

    template <typename T>
    inline long num_rows(const cv_image<T>& img) { return img.nr(); }
    template <typename T>
    inline long num_columns(const cv_image<T>& img) { return img.nc(); }

    template <typename T>
    inline void* image_data(cv_image<T>& img)
    {
        return img.size() != 0 ? &img[0][0] : 0;
    }

    template <typename T>
    inline const void* image_data(const cv_image<T>& img)
    {
        return img.size() != 0 ? &img[0][0] : 0;
    }

    template <typename T>
    inline long width_step(const cv_image<T>& img) { return img.width_step(); }
}

#endif // LIBINFRA_DLIB_CV_IMAGE_HPP_
