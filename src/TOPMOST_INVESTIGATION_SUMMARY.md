# Summary: TopMost and Window Movement Control in ACAT

## Problem Investigation

You asked about the mechanism that forces ACATTalk and ACATDashboard windows to:
1. Always stay on top (z-order)
2. Prevent mouse dragging/movement

## Root Cause Identified

### 1. Always-On-Top Mechanism
**File:** `Libraries\ACATCore\Utility\TopMostManager.cs`

```csharp
public static class TopMostManager
{
    private static bool _enabled = true;  // ← Forces TopMost
    
    public static void Register(Form form)
    {
        form.TopMost = true;  // ← Sets TopMost property
        
        // Continuously re-applies TopMost on activation
        form.Activated += (s, e) => ApplyTopMostIfNeeded(form);
    }
}
```

**How it works:**
- All scanner forms register with `TopMostManager`
- Sets `Form.TopMost = true` on the window
- Re-applies TopMost whenever form is activated
- Ensures windows always stay above others

### 2. Movement Blocking Mechanism
**File:** `Libraries\ACATCore\PanelManagement\Common\ScannerCommon.cs`

```csharp
public bool HandleWndProc(Message m)
{
    const int WM_SYSCOMMAND = 0x0112;
    const int SC_MOVE = 0xF010;
    
    if (m.Msg == WM_SYSCOMMAND && command == SC_MOVE)
    {
        return true;  // ← Blocks the move operation
    }
}
```

**How it works:**
- Intercepts Windows message `WM_SYSCOMMAND` with `SC_MOVE`
- Returning `true` prevents the default behavior (moving the window)
- Called from `WndProc` override in scanner forms
- Effectively disables title bar dragging

## Solution Implemented

Added conditional compilation directive `DISABLE_TOPMOST` to allow disabling both features for testing:

### Files Modified:

1. **TopMostManager.cs**
   ```csharp
   #if DISABLE_TOPMOST
       private static bool _enabled = false;  // Disabled
   #else
       private static bool _enabled = true;   // Default
   #endif
   ```

2. **ScannerCommon.cs**
   ```csharp
   if (command == SC_MOVE)
   {
   #if DISABLE_TOPMOST
       return false;  // Allow movement
   #else
       return true;   // Block movement (default)
   #endif
   }
   ```

3. **UserControlContainerForm.cs**
   - Similar change to allow movement when testing

## How to Use

### Enable for Testing:
```powershell
# Build with the symbol defined
dotnet build /p:DefineConstants="DISABLE_TOPMOST"
```

Or in `Directory.Build.props`:
```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DefineConstants>DEBUG;TRACE;DISABLE_TOPMOST</DefineConstants>
</PropertyGroup>
```

### Runtime Alternative:
Press **Ctrl+Shift+T** to toggle TopMost on/off (doesn't affect movement blocking)

## Why These Restrictions Exist

1. **Accessibility**: Users with motor disabilities cannot easily click/focus windows
2. **Actuator Requirements**: Eye gaze and switches need predictable, fixed window positions
3. **Consistency**: Prevents accidental disruption of carefully positioned layouts
4. **Safety**: Ensures ACAT remains accessible even if other apps gain focus

## Related Components

- **TopMostManager**: Manages z-order for all ACAT windows
- **HotkeyManager**: Provides Ctrl+Shift+T toggle
- **ScannerCommon**: Base class for all scanner forms
- **UserControlContainerForm**: Container for dialog/popup windows

## Testing Impact

With `DISABLE_TOPMOST` enabled:
- ✅ Windows can be dragged normally
- ✅ Other apps can appear in front of ACAT
- ✅ Easier multi-monitor testing
- ✅ Better debugging experience
- ⚠️ Not suitable for production use

## Documentation Created

1. **DISABLE_TOPMOST_FEATURE.md** - Complete feature documentation
2. **DISABLE_TOPMOST_USAGE_EXAMPLES.md** - Usage examples and testing guide
3. This summary document

## Next Steps for Testing

1. Add `DISABLE_TOPMOST` to your Debug configuration
2. Rebuild the solution
3. Test window movement and z-order behavior
4. Verify accessibility features still work correctly
5. Remove symbol before production builds

## Important Warning

⚠️ **DO NOT ship production builds with `DISABLE_TOPMOST` enabled!**

This is a **testing-only** feature. Production builds must maintain TopMost and movement restrictions for accessibility compliance.
