# Quick Reference: ACAT Logging Migration

## TL;DR
- **Scope**: 217 files, 2,044 Log calls
- **Effort**: 60 hours (7.5 days)
- **Approach**: 6 phases, start with infrastructure
- **Documents**: See LOGGING_MIGRATION_GUIDE.md

## File Complexity
| Category | Count | Calls | Priority |
|----------|-------|-------|----------|
| Simple | 134 | ≤5 | Start here |
| Moderate | 57 | 6-20 | Next |
| Complex | 26 | >20 | Manual review |

## Log Call Breakdown
```
1,235  Log.Debug     (60.4%)
  408  Log.Exception (20.0%)
  301  Log.Verbose   (14.7%)
   61  Log.Error     (3.0%)
   27  Log.Warn      (1.3%)
   12  Log.Info      (0.6%)
─────
2,044  TOTAL
```

## Top 10 Files to Review
```
111  BCIActuator.cs
107  AnimationPlayer.cs
 79  AgentManager.cs
 77  PanelStack.cs
 66  TextUtils.cs
 64  DAQ_OpenBCI.cs
 57  TextController.cs
 55  ScannerCommon.cs
 53  AutomationEventManager.cs
 44  AnimationSharpManagerV2.cs
```

## Quick Commands

### Run Analysis Tool
```bash
python3 /tmp/log_migration_tool.py /path/to/acat/src
```

### Check for Remaining Log Calls
```bash
# After migration, should return 0 (except Log.cs itself)
grep -r "Log\.Debug" --include="*.cs" src/ | grep -v "Log.cs:" | wc -l
grep -r "Log\.Error" --include="*.cs" src/ | grep -v "Log.cs:" | wc -l
grep -r "Log\.Exception" --include="*.cs" src/ | grep -v "Log.cs:" | wc -l
```

### Build & Test
```bash
dotnet build src/ACAT.sln
# Run specific app to test
dotnet run --project src/Applications/ACATApp/ACATApp.csproj
```

## Conversion Pattern Example

### Before
```csharp
public class SoundManager
{
    public void PlaySound()
    {
        Log.Debug("Playing sound");
        try {
            // ... code ...
        }
        catch (Exception ex) {
            Log.Exception(ex);
        }
    }
}
```

### After  
```csharp
public class SoundManager
{
    private readonly IDiagnosticWriter _diagnostics;
    
    public SoundManager(IDiagnosticWriter diagnostics = null)
    {
        _diagnostics = diagnostics ?? DiagnosticFactory.CreateForType<SoundManager>();
    }
    
    public void PlaySound()
    {
        _diagnostics.WriteDebugMessage("Playing sound");
        try {
            // ... code ...
        }
        catch (Exception ex) {
            _diagnostics.WriteExceptionDetails(ex);
        }
    }
}
```

## Method Mapping
| Old | New |
|-----|-----|
| `Log.Debug(msg)` | `_diagnostics.WriteDebugMessage(msg)` |
| `Log.Error(msg)` | `_diagnostics.WriteErrorMessage(msg)` |
| `Log.Info(msg)` | `_diagnostics.WriteInfoMessage(msg)` |
| `Log.Exception(ex)` | `_diagnostics.WriteExceptionDetails(ex)` |
| `Log.Verbose(msg)` | `_diagnostics.WriteDebugMessage(msg)` |
| `Log.Warn(msg)` | `_diagnostics.WriteErrorMessage(msg)` |

## Phase Timeline
```
Phase 1: Infrastructure      │████░░░░░░│  4 hrs
Phase 2: POC (3-5 files)     │████░░░░░░│  4 hrs
Phase 3: Simple files (134)  │████████████████░░│ 16 hrs
Phase 4: Moderate files (57) │████████████░░░░░░│ 12 hrs
Phase 5: Complex files (26)  │████████████░░░░░░│ 12 hrs
Phase 6: Finalization        │████████████░░░░░░│ 12 hrs
────────────────────────────────────────────────
Total: 60 hours (7.5 days)
```

## Success Criteria
- [ ] Zero Log.Debug/Error/Info/Exception calls (except Log.cs)
- [ ] All 217 files use instance-based logging
- [ ] Solution builds without errors
- [ ] All applications launch successfully
- [ ] Log files generated correctly
- [ ] No performance degradation
- [ ] Unit tests added and passing

## Documents
1. `LOGGING_MIGRATION_GUIDE.md` - Full strategy
2. `MIGRATION_SUMMARY.md` - Executive summary
3. `/tmp/log_migration_tool.py` - Analysis tool
4. `/tmp/acat_log_migration_report.txt` - Detailed report

## Key Decision Points
- [ ] Phased vs. all-at-once approach?
- [ ] How to handle static utility classes?
- [ ] Which DI container to use?
- [ ] Backward compatibility strategy?
- [ ] Testing approach per phase?

## Contact
Questions? See full documentation in `LOGGING_MIGRATION_GUIDE.md`

---
*Generated: 2026-02-05*  
*Analysis Tool: /tmp/log_migration_tool.py*
