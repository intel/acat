# ACAT Logging Migration Guide

## Executive Summary

This document outlines the strategy for migrating ACAT's static `Log.*` calls to instance-based diagnostic logging across 217 files containing 2,044 logging calls.

## Current State Analysis

### Codebase Statistics
- **Total C# files**: 764
- **Files using Log**: 217
- **Total Log calls**: 2,044

### Log Call Distribution
| Method | Count | Percentage |
|--------|-------|------------|
| Log.Debug | 1,235 | 60.4% |
| Log.Exception | 408 | 20.0% |
| Log.Verbose | 301 | 14.7% |
| Log.Error | 61 | 3.0% |
| Log.Warn | 27 | 1.3% |
| Log.Info | 12 | 0.6% |

### File Complexity
- **Simple** (≤5 calls): 134 files - Quick wins
- **Moderate** (6-20 calls): 57 files - Standard effort  
- **Complex** (>20 calls): 26 files - Needs review

## Migration Strategy

### Phase 1: Foundation (Ticket #1 - Prerequisite)
**Goal**: Add logging infrastructure WITHOUT breaking existing code

**Tasks**:
1. Add NuGet packages to ACAT.Core.csproj:
   - Microsoft.Extensions.Logging (v8.0.0+)
   - Microsoft.Extensions.Logging.Debug
   - Microsoft.Extensions.Logging.Console
   - Microsoft.Extensions.DependencyInjection

2. Create `ACAT.Core.Utility.DiagnosticWriter` - Instance-based logging wrapper
3. Create `ACAT.Core.Utility.DiagnosticFactory` - Factory for creating loggers
4. Keep existing `Log.cs` unchanged (backward compatibility)
5. Add `[Obsolete]` attributes to `Log.cs` methods

### Phase 2: Proof of Concept (Ticket #2 - Sample Conversion)
**Goal**: Convert 3-5 representative files to establish pattern

**Recommended starter files** (simple, non-static classes):
1. `Libraries/ACATCore/Utility/SoundManager.cs` (3 calls)
2. `Libraries/ACATCore/Utility/ImageUtils.cs` (4 calls)
3. `Libraries/ACATCore/Utility/ResourceUtils.cs` (5 calls)

**Conversion Pattern**:
```csharp
// BEFORE
public class SoundManager
{
    public static void PlaySound()
    {
        Log.Debug("Playing sound");
    }
}

// AFTER
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
    }
}
```

### Phase 3: Batch Conversion (Ticket #2 - Mass Migration)
**Goal**: Convert remaining 134 simple files, then 57 moderate files

**Approach**: Semi-automated using custom tooling
1. Use Roslyn API or regex-based tool for mechanical conversion
2. Human review for each file
3. Test after each batch of 10-20 files

**Conversion Rules**:
| Old Pattern | New Pattern |
|------------|-------------|
| `Log.Debug(msg)` | `_diagnostics.WriteDebugMessage(msg)` |
| `Log.Error(msg)` | `_diagnostics.WriteErrorMessage(msg)` |
| `Log.Info(msg)` | `_diagnostics.WriteInfoMessage(msg)` |
| `Log.Exception(ex)` | `_diagnostics.WriteExceptionDetails(ex)` |
| `Log.Verbose(msg)` | `_diagnostics.WriteDebugMessage(msg)` |
| `Log.Warn(msg)` | `_diagnostics.WriteErrorMessage(msg)` |

### Phase 4: Complex Files (Ticket #2 - Manual Review)
**Goal**: Convert 26 complex files with >20 calls each

**Top 5 Priority Files**:
1. `BCIActuator.cs` (111 calls)
2. `AnimationPlayer.cs` (107 calls)
3. `AgentManager.cs` (79 calls)
4. `PanelStack.cs` (77 calls)
5. `TextUtils.cs` (66 calls - static utility class)

**Special Cases**:
- **Static utility classes**: Consider making them instantiable or using static factory
- **Singleton managers**: May need constructor parameter added
- **Legacy code**: May require architectural discussion

### Phase 5: Application Entry Points (Ticket #3)
**Goal**: Setup dependency injection in application startup

**Entry Points to Update**:
1. `Applications/ACATApp/Program.cs`
2. `Applications/ACATWatch/Program.cs`
3. `Applications/ACATTalk/Program.cs`
4. `Applications/ACATConfig/Program.cs`
5. `Applications/ACATConfigNext/Program.cs`
6. `Applications/ConvAssistTerminate/Program.cs`

**Startup Pattern**:
```csharp
static void Main(string[] args)
{
    var diagnosticsFactory = DiagnosticFactory.Initialize();
    
    // Existing initialization code...
    InitializeGlobals();
    InitializeLogging();  // Enhanced to use diagnosticsFactory
    
    // Rest of application...
}
```

### Phase 6: Testing & Validation (Ticket #4)
**Goal**: Ensure no regressions, all logs still work

**Validation Steps**:
1. Run grep validation:
   ```bash
   # Should show only Log.cs itself
   grep -r "Log\.Debug" --include="*.cs" | grep -v "Log.cs:"
   ```

2. Build solution: `dotnet build ACAT.sln`

3. Manual testing:
   - Launch each application
   - Verify log files created
   - Check log content matches old format

4. Create unit tests for DiagnosticWriter

5. Document edge cases and known issues

## Timeline Estimate

| Phase | Effort | Dependencies |
|-------|--------|--------------|
| Phase 1: Foundation | 4 hours | None |
| Phase 2: POC (3-5 files) | 4 hours | Phase 1 |
| Phase 3: Simple files (134) | 16 hours | Phase 2 |
| Phase 3: Moderate files (57) | 12 hours | Phase 3 (simple) |
| Phase 4: Complex files (26) | 12 hours | Phase 3 (moderate) |
| Phase 5: Entry points (6) | 4 hours | Phase 4 |
| Phase 6: Testing | 8 hours | Phase 5 |
| **Total** | **60 hours (7.5 days)** | |

## Risk Mitigation

### High Risk Items
1. **Static utility classes**: 60+ classes can't use constructor injection
   - **Mitigation**: Keep as static, use factory pattern internally

2. **Breaking changes**: Existing code expects static Log access
   - **Mitigation**: Keep Log.cs as thin wrapper during transition

3. **Test coverage**: No existing logging tests
   - **Mitigation**: Add tests before making changes

### Success Criteria
✅ All 2,044 Log calls converted  
✅ Solution builds without errors  
✅ All applications launch successfully  
✅ Log files generated correctly  
✅ No performance degradation  
✅ Zero remaining Log.Debug/Error/Info calls (except in Log.cs)

## Appendix: Automation Tool

A Python analysis tool has been created at `/tmp/log_migration_tool.py` that:
- Scans codebase for Log usage
- Generates statistics and reports
- Identifies files by complexity
- Can be extended for automated conversion

Usage:
```bash
python3 /tmp/log_migration_tool.py /path/to/acat/src
```

## Next Steps

1. Review and approve this migration strategy
2. Execute Phase 1 (Foundation setup)
3. Complete Phase 2 (POC with 3 files)
4. Review POC and adjust pattern if needed
5. Proceed with remaining phases

---
*Document Version: 1.0*  
*Last Updated: 2026-02-05*
