# VisionExternalInstalls

This folder holds third-party components that the vision library depends on.
They are **not distributed with this repository**; download each one from its
original source and place it as described below. Everything in this folder
except this README is git-ignored.

Expected layout after setup:

```
VisionExternalInstalls/
  README.md
  dlib-20.0/
    dlib/                                  <- dlib source tree
    ...
  shape_predictor_68_face_landmarks.dat    <- dlib face landmark model
```

OpenCV is **not** placed here; see below.

## dlib 20.0 (required)

- Used for face detection and 68-point landmark estimation. The sources are
  compiled directly into `libinfra`; nothing is built or installed separately.
- Version: **20.0** (other versions are not tested).
- Download: <http://dlib.net/files/dlib-20.0.tar.bz2>, or the `v20.0` tag at
  <https://github.com/davisking/dlib/releases>.
- Extract so that `VisionExternalInstalls/dlib-20.0/dlib/` exists
  (the build adds `VisionExternalInstalls/dlib-20.0` to the include path).
- License: Boost Software License 1.0.

## Face landmark model (required)

- File: `shape_predictor_68_face_landmarks.dat`
- Download: <http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2>
- Decompress the `.bz2` (for example with 7-Zip) and place the resulting `.dat`
  directly in `VisionExternalInstalls/`.
- The build embeds this file's absolute path into the native DLL
  (`DLIB_FACE_LANDMARKS_FILE` in `msbuild/acat.native.props`), so it must be in
  place before building.
- License: the model was trained on the iBUG 300-W dataset, whose terms
  restrict use to non-commercial research. Review the terms at
  <http://dlib.net/files/> and <https://ibug.doc.ic.ac.uk/resources/300-W/>
  before using or redistributing it, particularly in a commercial product.

## OpenCV 5.x (required, stored outside this folder)

- Used for video capture and image processing. Tested with **OpenCV 5.0.0**;
  5.1.0 is selectable with `-p:OpenCVVersion=510`. Requires C++17.
- Download the Windows package from
  <https://github.com/opencv/opencv/releases>. The folder you need is
  `<extract dir>\build`, which contains `include` and `x64\vc16` (`lib`, `bin`).
  Prebuilt packages are `x64` only; `x86` requires building OpenCV from source.
- Point the build at it using either:

  ```powershell
  # environment variable (honored only if <dir>\x64\vc16\lib\opencv_world<ver>.lib exists)
  $env:OPENCV_DIR = 'C:\path\to\opencv-5.0.0\build'

  # or an MSBuild property
  msbuild acat_webcam.sln /p:OpenCVDir='C:\path\to\opencv-5.0.0\build' ...
  ```

- At runtime the OpenCV DLLs (`opencv_world500.dll`, `opencv_world500d.dll` for
  Debug, plus `opencv_videoio_*` plugins) are copied next to the built binaries.
- License: Apache 2.0.
## Optional: Doxygen

Only needed to regenerate the API documentation. Download Doxygen from
<https://www.doxygen.nl/download.html>; it is not used by the build.
