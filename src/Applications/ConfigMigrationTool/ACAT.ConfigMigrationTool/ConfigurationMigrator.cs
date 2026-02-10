////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationMigrator.cs
//
// Main migration logic for converting XML configurations to JSON
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool.Configuration;
using ACAT.ConfigMigrationTool.Validation;
using FluentValidation;
using NJsonSchema;
using Spectre.Console;
using System.Text.Json;

namespace ACAT.ConfigMigrationTool
{
    /// <summary>
    /// Handles migration of XML configuration files to JSON format
    /// </summary>
    public class ConfigurationMigrator
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationMigrator()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Migrates XML configuration files to JSON
        /// </summary>
        public async Task<MigrationResult> MigrateAsync(
            string inputDir,
            string outputDir,
            bool dryRun,
            bool backup)
        {
            var result = new MigrationResult
            {
                StartTime = DateTime.Now,
                DryRun = dryRun
            };

            if (!Directory.Exists(inputDir))
            {
                throw new DirectoryNotFoundException($"Input directory not found: {inputDir}");
            }

            if (!dryRun && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Discover all XML files
            var xmlFiles = Directory.GetFiles(inputDir, "*.xml", SearchOption.AllDirectories);
            result.TotalFiles = xmlFiles.Length;

            if (xmlFiles.Length == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No XML files found in the input directory.[/]");
                result.EndTime = DateTime.Now;
                return result;
            }

            AnsiConsole.MarkupLine($"[green]Found {xmlFiles.Length} XML file(s) to process.[/]");
            if (dryRun)
            {
                AnsiConsole.MarkupLine("[yellow]DRY RUN MODE - No files will be modified[/]");
            }
            AnsiConsole.WriteLine();

            // Process each file with progress bar
            await AnsiConsole.Progress()
                .AutoClear(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask("[green]Migrating files[/]", maxValue: xmlFiles.Length);

                    foreach (var xmlFile in xmlFiles)
                    {
                        task.Description = $"[green]Processing {Path.GetFileName(xmlFile)}[/]";
                        
                        try
                        {
                            await ProcessFileAsync(xmlFile, inputDir, outputDir, dryRun, backup, result);
                            result.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            result.FailureCount++;
                            result.Errors.Add((xmlFile, ex.Message));
                            AnsiConsole.MarkupLine($"[red]✗ Failed: {Path.GetFileName(xmlFile)} - {ex.Message}[/]");
                        }

                        task.Increment(1);
                        await Task.Delay(10); // Small delay for visual effect
                    }

                    task.Description = "[green]Migration complete[/]";
                });

            result.EndTime = DateTime.Now;
            return result;
        }

        private async Task ProcessFileAsync(
            string xmlFile,
            string inputDir,
            string outputDir,
            bool dryRun,
            bool backup,
            MigrationResult result)
        {
            // Detect schema type
            var schemaType = DetectSchemaType(xmlFile);
            
            if (schemaType == SchemaType.Unknown)
            {
                result.SkippedCount++;
                result.Warnings.Add((xmlFile, "Unknown schema type, skipped"));
                return;
            }

            // Convert XML to POCO
            object poco;
            string json;
            
            switch (schemaType)
            {
                case SchemaType.ActuatorSettings:
                    poco = XmlDeserializers.DeserializeActuatorSettings(xmlFile);
                    json = JsonSerializer.Serialize((ActuatorSettingsJson)poco, _jsonOptions);
                    break;
                    
                case SchemaType.Theme:
                    poco = XmlDeserializers.DeserializeTheme(xmlFile);
                    json = JsonSerializer.Serialize((ThemeJson)poco, _jsonOptions);
                    break;
                    
                case SchemaType.PanelConfig:
                    poco = XmlDeserializers.DeserializePanelConfig(xmlFile);
                    json = JsonSerializer.Serialize((PanelConfigJson)poco, _jsonOptions);
                    break;
                    
                default:
                    throw new InvalidOperationException($"Unsupported schema type: {schemaType}");
            }

            // Validate POCO
            ValidatePoco(poco, schemaType);

            if (!dryRun)
            {
                // Backup if requested
                if (backup)
                {
                    var backupPath = xmlFile + ".backup";
                    File.Copy(xmlFile, backupPath, true);
                    result.BackedUpFiles.Add(backupPath);
                }

                // Calculate output path preserving directory structure
                var relativePath = Path.GetRelativePath(inputDir, xmlFile);
                var jsonFileName = Path.GetFileNameWithoutExtension(relativePath) + ".json";
                var jsonDir = Path.GetDirectoryName(relativePath);
                var outputPath = string.IsNullOrEmpty(jsonDir) 
                    ? Path.Combine(outputDir, jsonFileName)
                    : Path.Combine(outputDir, jsonDir, jsonFileName);

                // Ensure output directory exists
                var outputFileDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputFileDir) && !Directory.Exists(outputFileDir))
                {
                    Directory.CreateDirectory(outputFileDir);
                }

                // Write JSON file
                await File.WriteAllTextAsync(outputPath, json);
                result.SuccessfulFiles.Add(outputPath);
                
                AnsiConsole.MarkupLine($"[green]✓ Converted: {Path.GetFileName(xmlFile)} → {Path.GetFileName(outputPath)}[/]");
            }
            else
            {
                result.SuccessfulFiles.Add(xmlFile);
                AnsiConsole.MarkupLine($"[blue]✓ Would convert: {Path.GetFileName(xmlFile)}[/]");
            }
        }

        private SchemaType DetectSchemaType(string xmlFile)
        {
            try
            {
                var doc = System.Xml.Linq.XDocument.Load(xmlFile);
                var root = doc.Root;

                if (root == null)
                    return SchemaType.Unknown;

                // Check for ActuatorConfig
                if (root.Name.LocalName == "ActuatorConfig")
                    return SchemaType.ActuatorSettings;

                // Check for Theme
                if (root.Name.LocalName == "ACAT")
                {
                    if (root.Element("Theme") != null)
                        return SchemaType.Theme;
                    
                    // Check for PanelConfig (has Layout or WidgetAttributes)
                    if (root.Element("Layout") != null || root.Element("WidgetAttributes") != null)
                        return SchemaType.PanelConfig;
                }

                return SchemaType.Unknown;
            }
            catch
            {
                return SchemaType.Unknown;
            }
        }

        private void ValidatePoco(object poco, SchemaType schemaType)
        {
            IValidator? validator = null;

            switch (schemaType)
            {
                case SchemaType.ActuatorSettings:
                    validator = new ActuatorSettingsValidator();
                    break;
                case SchemaType.Theme:
                    validator = new ThemeValidator();
                    break;
                case SchemaType.PanelConfig:
                    validator = new PanelConfigValidator();
                    break;
            }

            if (validator != null)
            {
                var context = new ValidationContext<object>(poco);
                var validationResult = validator.Validate(context);
                
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException($"Validation failed: {errors}");
                }
            }
        }

        /// <summary>
        /// Validates JSON files against their schemas
        /// </summary>
        public async Task<MigrationResult> ValidateAsync(string inputDir)
        {
            var result = new MigrationResult
            {
                StartTime = DateTime.Now,
                DryRun = false
            };

            if (!Directory.Exists(inputDir))
            {
                throw new DirectoryNotFoundException($"Input directory not found: {inputDir}");
            }

            var jsonFiles = Directory.GetFiles(inputDir, "*.json", SearchOption.AllDirectories);
            result.TotalFiles = jsonFiles.Length;

            AnsiConsole.MarkupLine($"[green]Found {jsonFiles.Length} JSON file(s) to validate.[/]");
            AnsiConsole.WriteLine();

            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(jsonFile);
                    
                    // Try to determine type and validate
                    if (json.Contains("\"actuatorSettings\""))
                    {
                        var settings = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);
                        if (settings != null)
                        {
                            ValidatePoco(settings, SchemaType.ActuatorSettings);
                            AnsiConsole.MarkupLine($"[green]✓ Valid: {Path.GetFileName(jsonFile)} (ActuatorSettings)[/]");
                            result.SuccessCount++;
                        }
                    }
                    else if (json.Contains("\"colorSchemes\""))
                    {
                        var theme = JsonSerializer.Deserialize<ThemeJson>(json);
                        if (theme != null)
                        {
                            ValidatePoco(theme, SchemaType.Theme);
                            AnsiConsole.MarkupLine($"[green]✓ Valid: {Path.GetFileName(jsonFile)} (Theme)[/]");
                            result.SuccessCount++;
                        }
                    }
                    else if (json.Contains("\"widgetAttributes\"") || json.Contains("\"layout\""))
                    {
                        var panel = JsonSerializer.Deserialize<PanelConfigJson>(json);
                        if (panel != null)
                        {
                            ValidatePoco(panel, SchemaType.PanelConfig);
                            AnsiConsole.MarkupLine($"[green]✓ Valid: {Path.GetFileName(jsonFile)} (PanelConfig)[/]");
                            result.SuccessCount++;
                        }
                    }
                    else
                    {
                        result.SkippedCount++;
                        AnsiConsole.MarkupLine($"[yellow]⚠ Skipped: {Path.GetFileName(jsonFile)} (Unknown type)[/]");
                    }
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add((jsonFile, ex.Message));
                    AnsiConsole.MarkupLine($"[red]✗ Invalid: {Path.GetFileName(jsonFile)} - {ex.Message}[/]");
                }
            }

            result.EndTime = DateTime.Now;
            return result;
        }

        /// <summary>
        /// Rolls back migration by restoring backup files
        /// </summary>
        public async Task<MigrationResult> RollbackAsync(string backupDir)
        {
            var result = new MigrationResult
            {
                StartTime = DateTime.Now,
                DryRun = false
            };

            if (!Directory.Exists(backupDir))
            {
                throw new DirectoryNotFoundException($"Backup directory not found: {backupDir}");
            }

            var backupFiles = Directory.GetFiles(backupDir, "*.backup", SearchOption.AllDirectories);
            result.TotalFiles = backupFiles.Length;

            AnsiConsole.MarkupLine($"[green]Found {backupFiles.Length} backup file(s) to restore.[/]");
            AnsiConsole.WriteLine();

            foreach (var backupFile in backupFiles)
            {
                try
                {
                    var originalFile = backupFile.Replace(".backup", "");
                    File.Copy(backupFile, originalFile, true);
                    File.Delete(backupFile);
                    
                    result.SuccessCount++;
                    AnsiConsole.MarkupLine($"[green]✓ Restored: {Path.GetFileName(originalFile)}[/]");
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add((backupFile, ex.Message));
                    AnsiConsole.MarkupLine($"[red]✗ Failed to restore: {Path.GetFileName(backupFile)} - {ex.Message}[/]");
                }
            }

            result.EndTime = DateTime.Now;
            return result;
        }
    }

    /// <summary>
    /// Supported configuration schema types
    /// </summary>
    public enum SchemaType
    {
        Unknown,
        ActuatorSettings,
        Theme,
        PanelConfig
    }
}
