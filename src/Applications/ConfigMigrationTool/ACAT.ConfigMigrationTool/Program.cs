////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// ACAT Configuration Migration Tool - Main entry point
//
////////////////////////////////////////////////////////////////////////////

using System.CommandLine;
using ACAT.ConfigMigrationTool;
using Spectre.Console;

// Create root command
var rootCommand = new RootCommand("ACAT Configuration Migration Tool - Convert XML configurations to JSON format");

// Create migrate command
var migrateCommand = new Command("migrate", "Migrate XML configuration files to JSON format");

var inputOption = new Option<string>(
    aliases: new[] { "--input", "-i" },
    description: "Input directory containing XML configuration files")
{
    IsRequired = true
};

var outputOption = new Option<string>(
    aliases: new[] { "--output", "-o" },
    description: "Output directory for JSON configuration files")
{
    IsRequired = true
};

var dryRunOption = new Option<bool>(
    aliases: new[] { "--dry-run", "-d" },
    getDefaultValue: () => false,
    description: "Preview changes without actually converting files");

var backupOption = new Option<bool>(
    aliases: new[] { "--backup", "-b" },
    getDefaultValue: () => false,
    description: "Create backup copies of original XML files");

migrateCommand.AddOption(inputOption);
migrateCommand.AddOption(outputOption);
migrateCommand.AddOption(dryRunOption);
migrateCommand.AddOption(backupOption);

migrateCommand.SetHandler(async (string input, string output, bool dryRun, bool backup) =>
{
    try
    {
        AnsiConsole.Write(
            new FigletText("ACAT")
                .LeftJustified()
                .Color(Color.Blue));
        
        AnsiConsole.MarkupLine("[bold]Configuration Migration Tool[/]");
        AnsiConsole.WriteLine();
        
        var migrator = new ConfigurationMigrator();
        var result = await migrator.MigrateAsync(input, output, dryRun, backup);
        
        Console.WriteLine(result.GenerateReport());
        
        if (result.FailureCount > 0)
        {
            Environment.ExitCode = 1;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        Environment.ExitCode = 1;
    }
}, inputOption, outputOption, dryRunOption, backupOption);

// Create validate command
var validateCommand = new Command("validate", "Validate JSON configuration files against schemas");

var validateInputOption = new Option<string>(
    aliases: new[] { "--input", "-i" },
    description: "Input directory containing JSON configuration files")
{
    IsRequired = true
};

validateCommand.AddOption(validateInputOption);

validateCommand.SetHandler(async (string input) =>
{
    try
    {
        AnsiConsole.Write(
            new FigletText("ACAT")
                .LeftJustified()
                .Color(Color.Blue));
        
        AnsiConsole.MarkupLine("[bold]Configuration Validation Tool[/]");
        AnsiConsole.WriteLine();
        
        var migrator = new ConfigurationMigrator();
        var result = await migrator.ValidateAsync(input);
        
        Console.WriteLine(result.GenerateReport());
        
        if (result.FailureCount > 0)
        {
            Environment.ExitCode = 1;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        Environment.ExitCode = 1;
    }
}, validateInputOption);

// Create rollback command
var rollbackCommand = new Command("rollback", "Rollback migration by restoring backup files");

var backupDirOption = new Option<string>(
    aliases: new[] { "--backup", "-b" },
    description: "Directory containing backup files")
{
    IsRequired = true
};

rollbackCommand.AddOption(backupDirOption);

rollbackCommand.SetHandler(async (string backup) =>
{
    try
    {
        AnsiConsole.Write(
            new FigletText("ACAT")
                .LeftJustified()
                .Color(Color.Blue));
        
        AnsiConsole.MarkupLine("[bold]Configuration Rollback Tool[/]");
        AnsiConsole.WriteLine();
        
        if (!AnsiConsole.Confirm($"Are you sure you want to rollback changes in '{backup}'?"))
        {
            AnsiConsole.MarkupLine("[yellow]Rollback cancelled.[/]");
            return;
        }
        
        var migrator = new ConfigurationMigrator();
        var result = await migrator.RollbackAsync(backup);
        
        Console.WriteLine(result.GenerateReport());
        
        if (result.FailureCount > 0)
        {
            Environment.ExitCode = 1;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        Environment.ExitCode = 1;
    }
}, backupDirOption);

// Add commands to root
rootCommand.AddCommand(migrateCommand);
rootCommand.AddCommand(validateCommand);
rootCommand.AddCommand(rollbackCommand);

// Invoke
return await rootCommand.InvokeAsync(args);
