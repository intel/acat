////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ThemeValidator.cs
//
// FluentValidation validators for Theme configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace ACAT.Core.Validation
{
    /// <summary>
    /// Validator for ThemeJson configuration
    /// </summary>
    public class ThemeValidator : AbstractValidator<ThemeJson>
    {
        private static readonly ColorSchemeValidator _colorSchemeValidator = new ColorSchemeValidator();

        public ThemeValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Theme description is required")
                .MaximumLength(500)
                .WithMessage("Theme description cannot exceed 500 characters");

            RuleFor(x => x.ColorSchemes)
                .NotNull()
                .WithMessage("ColorSchemes cannot be null")
                .NotEmpty()
                .WithMessage("At least one color scheme must be defined");

            RuleForEach(x => x.ColorSchemes)
                .SetValidator(_colorSchemeValidator);

            // Business rule: No duplicate color scheme names
            RuleFor(x => x.ColorSchemes)
                .Must(schemes => {
                    var names = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (ColorSchemeJson scheme in schemes)
                    {
                        if (!string.IsNullOrEmpty(scheme.Name))
                        {
                            if (names.Contains(scheme.Name))
                                return false;
                            names.Add(scheme.Name);
                        }
                    }
                    return true;
                })
                .WithMessage("Color scheme names must be unique")
                .When(x => x.ColorSchemes != null && x.ColorSchemes.Count > 0);

            // Business rule: Should have essential color schemes
            RuleFor(x => x.ColorSchemes)
                .Must(schemes => schemes.Exists(s => s.Name.Equals("Scanner", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Theme should include a 'Scanner' color scheme")
                .When(x => x.ColorSchemes != null && x.ColorSchemes.Count > 0);
        }
    }

    /// <summary>
    /// Validator for ColorSchemeJson
    /// </summary>
    public class ColorSchemeValidator : AbstractValidator<ColorSchemeJson>
    {
        private static readonly Regex ColorRegex = new Regex(@"^(#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})|[A-Za-z]+)$", RegexOptions.Compiled);

        public ColorSchemeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Color scheme name is required")
                .MaximumLength(100)
                .WithMessage("Color scheme name cannot exceed 100 characters");

            RuleFor(x => x.Background)
                .Must(BeValidColor)
                .WithMessage("Background must be a valid color (e.g., 'White', '#232433')")
                .When(x => !string.IsNullOrEmpty(x.Background));

            RuleFor(x => x.Foreground)
                .Must(BeValidColor)
                .WithMessage("Foreground must be a valid color (e.g., 'White', '#232433')")
                .When(x => !string.IsNullOrEmpty(x.Foreground));

            RuleFor(x => x.HighlightBackground)
                .Must(BeValidColor)
                .WithMessage("HighlightBackground must be a valid color")
                .When(x => !string.IsNullOrEmpty(x.HighlightBackground));

            RuleFor(x => x.HighlightForeground)
                .Must(BeValidColor)
                .WithMessage("HighlightForeground must be a valid color")
                .When(x => !string.IsNullOrEmpty(x.HighlightForeground));

            RuleFor(x => x.HighlightSelectedBackground)
                .Must(BeValidColor)
                .WithMessage("HighlightSelectedBackground must be a valid color")
                .When(x => !string.IsNullOrEmpty(x.HighlightSelectedBackground));

            RuleFor(x => x.HighlightSelectedForeground)
                .Must(BeValidColor)
                .WithMessage("HighlightSelectedForeground must be a valid color")
                .When(x => !string.IsNullOrEmpty(x.HighlightSelectedForeground));

            RuleFor(x => x.BackgroundImage)
                .MaximumLength(255)
                .WithMessage("Background image path cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.BackgroundImage));

            RuleFor(x => x.HighlightBackgroundImage)
                .MaximumLength(255)
                .WithMessage("Highlight background image path cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.HighlightBackgroundImage));

            // Business rule: Must specify either background color or background image
            RuleFor(x => x)
                .Must(scheme => !string.IsNullOrEmpty(scheme.Background) || !string.IsNullOrEmpty(scheme.BackgroundImage))
                .WithMessage("Color scheme must specify either a background color or background image");

            // Business rule: Should specify foreground if background is specified
            RuleFor(x => x.Foreground)
                .NotEmpty()
                .WithMessage("Foreground color should be specified when background is defined")
                .When(x => !string.IsNullOrEmpty(x.Background));
        }

        private bool BeValidColor(string color)
        {
            if (string.IsNullOrEmpty(color))
                return true;

            return ColorRegex.IsMatch(color);
        }
    }
}
