# Disabling TopMost and Window Movement Blocking for Testing

## Overview

By default, ACAT windows (ACATTalk, ACATDashboard, scanners, etc.) are:
1. **Always on top** - They stay above all other windows (z-order priority)
2. **Non-movable** - Users cannot drag them with the mouse

This behavior ensures ACAT remains accessible to users with disabilities. However, for **testing and development**, you may want to disable these restrictions.

## How to Disable

### Option 1: Build with Conditional Compilation (Recommended)

Add the `DISABLE_TOPMOST` conditional compilation symbol:

#### In Visual Studio:
1. Right-click on the **ACAT.sln** or individual project
2. Select **Properties** → **Build**
3. In **Conditional compilation symbols**, add: `DISABLE_TOPMOST`
4. Rebuild the solution

#### Via Command Line:
```powershell
dotnet build /p:DefineConstants="DISABLE_TOPMOST"
```

#### In Project Files (Permanent):
Add to `Directory.Build.props` for global effect:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DefineConstants>DEBUG;TRACE;DISABLE_TOPMOST</DefineConstants>
</PropertyGroup>
```

Or add to specific `.csproj` files:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DefineConstants>$(DefineConstants);DISABLE_TOPMOST</DefineConstants>
</PropertyGroup>
```

### Option 2: Runtime Toggle (Already Available)

Press **Ctrl+Shift+T** at runtime to toggle TopMost behavior on/off. 
(Note: This doesn't enable window movement, only TopMost behavior)

## What Gets Disabled

When `DISABLE_TOPMOST` is defined:

### 1. TopMost Behavior (`TopMostManager.cs`)
- Windows no longer force themselves to stay on top
- `TopMostManager._enabled` defaults to `false`
- Other applications can appear in front of ACAT windows

### 2. Window Movement Blocking (`ScannerCommon.cs`, `UserControlContainerForm.cs`)
- The `WM_SYSCOMMAND` / `SC_MOVE` message is no longer blocked
- Windows can be dragged with the mouse
- Normal window movement behavior is restored

## Affected Components

**Files Modified:**
- `Libraries\ACATCore\Utility\TopMostManager.cs`
- `Libraries\ACATCore\PanelManagement\Common\ScannerCommon.cs`
- `Libraries\ACATExtension\UI\UserControlContainerForm.cs`

**Affected Windows:**
- All scanners (TalkApplicationScanner, DashboardScanner, etc.)
- Dialog windows
- Menu panels
- Any window managed by `ScannerCommon`

## Usage Examples

### Testing Window Positioning
```powershell
# Build with movement enabled
dotnet build /p:Configuration=Debug /p:DefineConstants="DISABLE_TOPMOST"

# Run ACATTalk
./build/bin/Debug/ACATTalk.exe

# Now you can:
# - Drag windows with the mouse
# - Let other windows appear in front
# - Test multi-monitor scenarios more easily
```

### Debugging Z-Order Issues
When `DISABLE_TOPMOST` is defined, you can:
- Inspect windows with other debugging tools
- Test interactions with other applications
- Verify window positioning logic without TopMost interference

## Important Notes

⚠️ **DO NOT ship production builds with `DISABLE_TOPMOST` enabled!**

This feature is **for testing and development only**. Production builds must have TopMost and movement blocking enabled to ensure accessibility.

### Why These Restrictions Exist

1. **Accessibility**: Users with disabilities may not be able to click or focus windows normally. TopMost ensures ACAT remains accessible.

2. **Actuator Interaction**: Some actuators (eye gaze, switches) require ACAT to be in a fixed, predictable location.

3. **Consistency**: Fixed positioning prevents accidental window movement that could disrupt user workflows.

## Troubleshooting

**Q: I defined `DISABLE_TOPMOST` but windows still won't move**

A: Ensure you did a **clean rebuild**:
```powershell
dotnet clean
dotnet build /p:DefineConstants="DISABLE_TOPMOST"
```

**Q: Can I disable only TopMost but keep movement blocking?**

A: Yes, use the runtime hotkey **Ctrl+Shift+T** or modify `TopMostManager._enabled` directly.

**Q: Some windows still don't move**

A: Check if they override `WndProc` without calling `ScannerCommon.HandleWndProc()`. Those windows need individual updates.

## Related Code

- **TopMost Management**: `Libraries\ACATCore\Utility\TopMostManager.cs`
- **Movement Blocking**: Search for `WM_SYSCOMMAND` and `SC_MOVE` in `*.cs` files
- **Runtime Toggle**: `Libraries\ACATCore\Utility\HotkeyManager.cs` (Ctrl+Shift+T)

## See Also

- [ACAT Architecture Documentation](docs/Architecture.md)
- [Scanner Development Guide](docs/ScannerDevelopment.md)
- [Debugging ACAT Applications](docs/Debugging.md)
