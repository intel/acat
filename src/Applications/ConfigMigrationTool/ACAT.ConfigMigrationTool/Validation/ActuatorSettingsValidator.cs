////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSettingsValidator.cs
//
// FluentValidation validators for ActuatorSettings configuration
// Provides business rule validation and cross-field validation
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool.Configuration;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace ACAT.ConfigMigrationTool.Validation
{
    /// <summary>
    /// Validator for ActuatorSettingsJson configuration
    /// </summary>
    public class ActuatorSettingsValidator : AbstractValidator<ActuatorSettingsJson>
    {
        public ActuatorSettingsValidator()
        {
            RuleFor(x => x.ActuatorSettings)
                .NotNull()
                .WithMessage("ActuatorSettings cannot be null")
                .NotEmpty()
                .WithMessage("At least one actuator must be configured");

            RuleForEach(x => x.ActuatorSettings)
                .SetValidator(new ActuatorSettingValidator());

            // Business rule: At least one actuator should be enabled
            RuleFor(x => x.ActuatorSettings)
                .Must(actuators => actuators.Exists(a => a.Enabled))
                .WithMessage("At least one actuator must be enabled for ACAT to function")
                .When(x => x.ActuatorSettings != null && x.ActuatorSettings.Count > 0);

            // Business rule: No duplicate actuator IDs
            RuleFor(x => x.ActuatorSettings)
                .Must(actuators => {
                    HashSet<string> ids = new System.Collections.Generic.HashSet<string>();
                    foreach (var actuator in actuators)
                    {
                        if (!string.IsNullOrEmpty(actuator.Id))
                        {
                            if (ids.Contains(actuator.Id))
                                return false;
                            ids.Add(actuator.Id);
                        }
                    }
                    return true;
                })
                .WithMessage("Actuator IDs must be unique")
                .When(x => x.ActuatorSettings != null && x.ActuatorSettings.Count > 0);
        }
    }

    /// <summary>
    /// Validator for ActuatorSettingJson
    /// </summary>
    public class ActuatorSettingValidator : AbstractValidator<ActuatorSettingJson>
    {
        private static readonly SwitchSettingValidator _switchSettingValidator = new();

        public ActuatorSettingValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Actuator name is required")
                .MaximumLength(100)
                .WithMessage("Actuator name cannot exceed 100 characters");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Actuator ID is required")
                .Must(BeValidGuid)
                .WithMessage("Actuator ID must be a valid GUID");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ImageFileName)
                .MaximumLength(255)
                .WithMessage("Image file name cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.ImageFileName));

            RuleFor(x => x.SwitchSettings)
                .NotNull()
                .WithMessage("SwitchSettings cannot be null");

            RuleForEach(x => x.SwitchSettings)
                .SetValidator(_switchSettingValidator);

            // Business rule: Enabled actuators should have at least one enabled switch
            RuleFor(x => x.SwitchSettings)
                .Must(switches => switches.Exists(s => s.Enabled))
                .WithMessage("Enabled actuators must have at least one enabled switch")
                .When(x => x.Enabled && x.SwitchSettings != null && x.SwitchSettings.Count > 0);

            // Business rule: No duplicate switch names within an actuator
            RuleFor(x => x.SwitchSettings)
                .Must(switches => {
                    HashSet<string> names = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var sw in switches)
                    {
                        if (!string.IsNullOrEmpty(sw.Name))
                        {
                            if (names.Contains(sw.Name))
                                return false;
                            names.Add(sw.Name);
                        }
                    }
                    return true;
                })
                .WithMessage("Switch names must be unique within an actuator")
                .When(x => x.SwitchSettings != null && x.SwitchSettings.Count > 0);
        }

        private bool BeValidGuid(string id)
        {
            return Guid.TryParse(id, out _);
        }
    }

    /// <summary>
    /// Validator for SwitchSettingJson
    /// </summary>
    public class SwitchSettingValidator : AbstractValidator<SwitchSettingJson>
    {
        public SwitchSettingValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Switch name is required")
                .MaximumLength(100)
                .WithMessage("Switch name cannot exceed 100 characters");

            RuleFor(x => x.Source)
                .MaximumLength(100)
                .WithMessage("Switch source cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Source));

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Switch description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Command)
                .MaximumLength(200)
                .WithMessage("Command cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Command));

            RuleFor(x => x.MinHoldTime)
                .MaximumLength(50)
                .WithMessage("MinHoldTime cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.MinHoldTime));

            RuleFor(x => x.BeepFile)
                .MaximumLength(255)
                .WithMessage("BeepFile cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.BeepFile));

            // Business rule: Enabled switches that actuate should have a command
            RuleFor(x => x.Command)
                .NotEmpty()
                .WithMessage("Enabled switches that actuate must have a command")
                .When(x => x.Enabled && x.Actuate);

            // Business rule: Trigger switches should use the @Trigger command
            RuleFor(x => x.Command)
                .Must(cmd => cmd == "@Trigger")
                .WithMessage("Trigger switches should use '@Trigger' as the command")
                .When(x => x.Name.Equals("Trigger", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.Command));
        }
    }
}
