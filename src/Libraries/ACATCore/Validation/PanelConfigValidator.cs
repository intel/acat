////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelConfigValidator.cs
//
// FluentValidation validators for PanelConfig configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using FluentValidation;
using System;

namespace ACAT.Core.Validation
{
    /// <summary>
    /// Validator for PanelConfigJson configuration
    /// </summary>
    public class PanelConfigValidator : AbstractValidator<PanelConfigJson>
    {
        public PanelConfigValidator()
        {
            RuleFor(x => x.WidgetAttributes)
                .NotNull()
                .WithMessage("WidgetAttributes cannot be null");

            RuleForEach(x => x.WidgetAttributes)
                .SetValidator(new WidgetAttributeValidator());

            RuleFor(x => x.Layout)
                .NotNull()
                .WithMessage("Layout is required")
                .SetValidator(new LayoutValidator());

            RuleFor(x => x.Animations)
                .NotNull()
                .WithMessage("Animations cannot be null");

            RuleForEach(x => x.Animations)
                .SetValidator(new AnimationValidator());

            // Business rule: No duplicate widget attribute names
            RuleFor(x => x.WidgetAttributes)
                .Must(attributes => {
                    var names = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var attr in attributes)
                    {
                        if (!string.IsNullOrEmpty(attr.Name))
                        {
                            if (names.Contains(attr.Name))
                                return false;
                            names.Add(attr.Name);
                        }
                    }
                    return true;
                })
                .WithMessage("Widget attribute names must be unique")
                .When(x => x.WidgetAttributes != null && x.WidgetAttributes.Count > 0);

            // Business rule: No duplicate animation names
            RuleFor(x => x.Animations)
                .Must(animations => {
                    var names = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var anim in animations)
                    {
                        if (!string.IsNullOrEmpty(anim.Name))
                        {
                            if (names.Contains(anim.Name))
                                return false;
                            names.Add(anim.Name);
                        }
                    }
                    return true;
                })
                .WithMessage("Animation names must be unique")
                .When(x => x.Animations != null && x.Animations.Count > 0);
        }
    }

    /// <summary>
    /// Validator for WidgetAttributeJson
    /// </summary>
    public class WidgetAttributeValidator : AbstractValidator<WidgetAttributeJson>
    {
        public WidgetAttributeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Widget attribute name is required")
                .MaximumLength(100)
                .WithMessage("Widget attribute name cannot exceed 100 characters");

            RuleFor(x => x.Label)
                .MaximumLength(200)
                .WithMessage("Label cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Label));

            RuleFor(x => x.Value)
                .MaximumLength(200)
                .WithMessage("Value cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Value));

            RuleFor(x => x.FontName)
                .MaximumLength(100)
                .WithMessage("Font name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FontName));

            RuleFor(x => x.FontSize)
                .MaximumLength(50)
                .WithMessage("Font size cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.FontSize));
        }
    }

    /// <summary>
    /// Validator for LayoutJson
    /// </summary>
    public class LayoutValidator : AbstractValidator<LayoutJson>
    {
        public LayoutValidator()
        {
            RuleFor(x => x.ColorScheme)
                .NotEmpty()
                .WithMessage("Color scheme is required")
                .MaximumLength(100)
                .WithMessage("Color scheme cannot exceed 100 characters");

            RuleFor(x => x.Widgets)
                .NotNull()
                .WithMessage("Widgets list cannot be null");

            RuleForEach(x => x.Widgets)
                .SetValidator(new WidgetValidator());
        }
    }

    /// <summary>
    /// Validator for WidgetJson
    /// </summary>
    public class WidgetValidator : AbstractValidator<WidgetJson>
    {
        public WidgetValidator()
        {
            RuleFor(x => x.Class)
                .NotEmpty()
                .WithMessage("Widget class is required")
                .MaximumLength(100)
                .WithMessage("Widget class cannot exceed 100 characters");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Widget name is required")
                .MaximumLength(100)
                .WithMessage("Widget name cannot exceed 100 characters");

            RuleFor(x => x.ColorScheme)
                .MaximumLength(100)
                .WithMessage("Color scheme cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.ColorScheme));

            RuleFor(x => x.Enabled)
                .MaximumLength(50)
                .WithMessage("Enabled value cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Enabled));

            RuleFor(x => x.Children)
                .NotNull()
                .WithMessage("Children list cannot be null");

            RuleForEach(x => x.Children)
                .SetValidator(new WidgetValidator());

            // Business rule: Container widgets (like RowWidget) should have children
            RuleFor(x => x.Children)
                .NotEmpty()
                .WithMessage("Container widgets should have at least one child")
                .When(x => x.Class != null && (x.Class.Contains("Row") || x.Class.Contains("Container")));
        }
    }

    /// <summary>
    /// Validator for AnimationJson
    /// </summary>
    public class AnimationValidator : AbstractValidator<AnimationJson>
    {
        public AnimationValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Animation name is required")
                .MaximumLength(100)
                .WithMessage("Animation name cannot exceed 100 characters");

            RuleFor(x => x.FirstPauseTime)
                .MaximumLength(100)
                .WithMessage("FirstPauseTime cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstPauseTime));

            RuleFor(x => x.OnEnter)
                .MaximumLength(100)
                .WithMessage("OnEnter cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.OnEnter));

            RuleFor(x => x.ScanTime)
                .MaximumLength(100)
                .WithMessage("ScanTime cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.ScanTime));

            RuleFor(x => x.Iterations)
                .MaximumLength(100)
                .WithMessage("Iterations cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Iterations));

            RuleFor(x => x.Steps)
                .NotNull()
                .WithMessage("Steps list cannot be null");

            RuleForEach(x => x.Steps)
                .SetValidator(new AnimationStepValidator());

            // Business rule: Animations should have at least one step
            RuleFor(x => x.Steps)
                .NotEmpty()
                .WithMessage("Animations should have at least one step")
                .When(x => x.Steps != null);
        }
    }

    /// <summary>
    /// Validator for AnimationStepJson
    /// </summary>
    public class AnimationStepValidator : AbstractValidator<AnimationStepJson>
    {
        public AnimationStepValidator()
        {
            RuleFor(x => x.WidgetName)
                .NotEmpty()
                .WithMessage("Widget name is required")
                .MaximumLength(100)
                .WithMessage("Widget name cannot exceed 100 characters");

            RuleFor(x => x.OnSelect)
                .MaximumLength(500)
                .WithMessage("OnSelect action cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.OnSelect));
        }
    }
}
