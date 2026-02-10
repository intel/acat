////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MigrationResult.cs
//
// Represents the result of a configuration migration operation
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.ConfigMigrationTool
{
    /// <summary>
    /// Represents the result of a migration operation
    /// </summary>
    public class MigrationResult
    {
        public int TotalFiles { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int SkippedCount { get; set; }
        public List<(string File, string Error)> Errors { get; set; } = new();
        public List<(string File, string Warning)> Warnings { get; set; } = new();
        public List<string> SuccessfulFiles { get; set; } = new();
        public List<string> BackedUpFiles { get; set; } = new();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool DryRun { get; set; }

        /// <summary>
        /// Generates a formatted report of the migration results
        /// </summary>
        public string GenerateReport()
        {
            var duration = EndTime - StartTime;
            var sb = new System.Text.StringBuilder();
            
            sb.AppendLine();
            sb.AppendLine("========================================");
            sb.AppendLine("  MIGRATION REPORT");
            sb.AppendLine("========================================");
            sb.AppendLine();
            
            if (DryRun)
            {
                sb.AppendLine("Mode: DRY RUN (no changes made)");
                sb.AppendLine();
            }

            sb.AppendLine($"Duration: {duration.TotalSeconds:F2} seconds");
            sb.AppendLine($"Total Files: {TotalFiles}");
            sb.AppendLine($"Successful: {SuccessCount}");
            sb.AppendLine($"Failed: {FailureCount}");
            sb.AppendLine($"Skipped: {SkippedCount}");
            
            if (BackedUpFiles.Count > 0)
            {
                sb.AppendLine($"Backed Up: {BackedUpFiles.Count}");
            }
            
            sb.AppendLine();

            if (SuccessfulFiles.Count > 0)
            {
                sb.AppendLine("Successfully Converted:");
                foreach (var file in SuccessfulFiles)
                {
                    sb.AppendLine($"  ✓ {file}");
                }
                sb.AppendLine();
            }

            if (Warnings.Count > 0)
            {
                sb.AppendLine("Warnings:");
                foreach (var (file, warning) in Warnings)
                {
                    sb.AppendLine($"  ⚠ {file}: {warning}");
                }
                sb.AppendLine();
            }

            if (Errors.Count > 0)
            {
                sb.AppendLine("Errors:");
                foreach (var (file, error) in Errors)
                {
                    sb.AppendLine($"  ✗ {file}: {error}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("========================================");
            
            return sb.ToString();
        }
    }
}
