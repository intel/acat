# Example: Adding DISABLE_TOPMOST to a Debug Configuration

## Quick Start

Add this to your `Directory.Build.props` file (at solution root):

```xml
<!-- Add DISABLE_TOPMOST for Debug builds only -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DefineConstants>DEBUG;TRACE;DISABLE_TOPMOST</DefineConstants>
</PropertyGroup>
```

Or create a new build configuration specifically for testing:

```xml
<!-- Create a new "Debug_TestMove" configuration -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug_TestMove'">
  <DefineConstants>DEBUG;TRACE;DISABLE_TOPMOST</DefineConstants>
  <DebugType>full</DebugType>
  <Optimize>false</Optimize>
</PropertyGroup>
```

Then in Visual Studio:
1. Configuration Manager → New Solution Configuration
2. Name: "Debug_TestMove" (copy from Debug)
3. Build the solution
4. Windows will now be movable and not always-on-top

## Testing the Feature

After building with `DISABLE_TOPMOST`:

1. **Launch ACATTalk:**
   ```
   ./build/bin/Debug/ACATTalk.exe
   ```

2. **Try these actions:**
   - ✅ Drag the window with your mouse
   - ✅ Open another application - it can appear in front
   - ✅ Use Alt+Tab to switch away from ACAT
   - ✅ Move windows between monitors

3. **Verify it works:**
   ```csharp
   // TopMostManager should report disabled:
   Console.WriteLine($"TopMost enabled: {TopMostManager.Enabled}");
   // Output: "TopMost enabled: False"
   ```

## Before/After Comparison

### Without DISABLE_TOPMOST (Default):
- ❌ Cannot drag windows
- ❌ Windows always stay on top
- ❌ Clicking title bar does nothing
- ✅ Consistent positioning for accessibility

### With DISABLE_TOPMOST:
- ✅ Can drag windows normally
- ✅ Other apps can appear in front
- ✅ Normal Windows behavior
- ⚠️ Not suitable for production

## Automated Testing Example

```csharp
#if DISABLE_TOPMOST
[TestMethod]
public void TestWindowMovement()
{
    // Create a scanner form
    var scanner = new TalkApplicationScanner();
    scanner.Show();
    
    // Should NOT be TopMost
    Assert.IsFalse(scanner.TopMost);
    
    // Should be movable - simulate WM_SYSCOMMAND/SC_MOVE
    // In normal builds, this would be blocked
    // With DISABLE_TOPMOST, it should succeed
}
#endif
```

## Reverting to Normal Behavior

Simply rebuild without the symbol:

```powershell
dotnet clean
dotnet build /p:Configuration=Debug
```

Or remove `DISABLE_TOPMOST` from your project properties.
