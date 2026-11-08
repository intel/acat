# acat-webcam
Webcam actuator for ACAT (Assistive Context-aware Toolkit)

The project builds as a plain Visual Studio solution — **no CMake required**.
A user guide is available in Docs/acat_webcam_user_guide.doc.

## Prerequisites

- **Visual Studio 2026** with the **Desktop development with C++** workload and
  a Windows SDK. The native projects use the `v145` platform toolset.
- The **.NET Framework 4.8.1 targeting pack** for the C# projects
  (`VisionWindowsAppCS`, `VisionHost`, `VisionContracts`).
- **OpenCV 5.x** Windows prebuilt package (`opencv_world` libraries, `vc16`
  layout, `x64`). OpenCV 5.0.0 is the tested version; 5.1.0 is selectable with
  `-p:OpenCVVersion=510`. The native code requires C++17 and no longer uses
  the removed C API, `ml`, or Haar/HOG modules. The prebuilt packages ship
  `x64` only; an `x86` build needs OpenCV built from source.- The **dlib 20.0 sources** at `VisionExternalInstalls/dlib-20.0` and the
  `shape_predictor_68_face_landmarks.dat` model at
  `VisionExternalInstalls/`. dlib is compiled directly into `libinfra`;
  nothing needs to be built or installed separately.

These third-party components are not redistributed here; see
[VisionExternalInstalls/README.md](VisionExternalInstalls/README.md) for where
to download each one and where to place it.

Point the build at your OpenCV package (the `build` folder containing
`x64\vc16\lib\opencv_world500.lib`) in either of two ways:

```powershell
# Option 1: environment variable (only honored if the opencv_world lib exists there)
$env:OPENCV_DIR = 'C:\path\to\opencv-5.0.0\build'

# Option 2: MSBuild property
msbuild acat_webcam.sln /p:OpenCVDir='C:\path\to\opencv-5.0.0\build' ...

# Other version (world lib suffix, e.g. opencv_world510.lib)
msbuild acat_webcam.sln /p:OpenCVVersion=510 ...
```

If neither is set, the default path in `msbuild/acat.native.props` is used;
edit that default to match your machine. A missing or wrong OpenCV location
fails the build with an explanatory error.
## Build

Open **`acat_webcam.sln`** at the repository root, pick a configuration
(`Debug`/`Release`) and platform (`x64`/`x86`), and build. From the command
line:

```powershell
msbuild acat_webcam.sln /p:Configuration=Release /p:Platform=x64 /m
```

The default build produces the native libraries (`libivcp`, `libpipeline`,
`libinfra`), the gesture DLL (`acat_gestures_dll`), the native test executable
(`acat_gestures`), and the C# projects. Everything lands in a single shared
output directory:

```
bin\x64\Release\        (or bin\x64\Debug, bin\Win32\Release, bin\Win32\Debug)
```

The output is self-contained: native DLLs, the OpenCV runtime DLLs, the
landmark model, `VisionWindowsAppCS.exe`, and `VisionHost.exe` are all staged
there automatically, so the app runs directly from the output directory. Close
any running copy of the app before rebuilding; Windows locks the executable
while it is running. Debug binaries use a `_d` suffix
(for example `acat_gestures_dll_d.dll`).

To debug, set `VisionWindowsAppCS` as the startup project and press F5. No C#
assembly references to the native DLLs are needed; the vision pipeline runs in
the separate `VisionHost.exe` process, which loads the gesture DLL through
P/Invoke and talks to the app over a named pipe (`VisionContracts` defines the
protocol). `VisionHost` exits automatically if the app that launched it dies.

Shared native build settings (output directories, OpenCV paths, warning
levels, preprocessor defines) live in `msbuild/acat.native.props`.

The app loads the native DLL on startup and reports missing native dependencies
in its window. Supply the landmark model and any required session data.

## Demos, tests, and markup tools

The solution also contains the demo applications (`demo_dlib`,
`demo_emotion_classification`, `demo_lr_classification`,
`demo_motion_estimation_feature`, `demo_runtime_mode`, `demo_test_mode`,
`demo_ucla`), the `performance_test`, and the markup tools (`markup`,
`regions_markup`) in the `demos` solution folder. They are **not** part of the
default build; right-click a project and choose **Build** to build one on
demand (project references pull in the native libraries automatically).

## Legacy CMake build

The `CMakeLists.txt` files are retained for reference only and are no longer
the supported way to build the project. The standalone
`VisionWindowsAppCS/VisionWindowsAppCS.sln` builds the C# projects alone
(AnyCPU), but does not stage the native DLLs; use the root solution instead.


