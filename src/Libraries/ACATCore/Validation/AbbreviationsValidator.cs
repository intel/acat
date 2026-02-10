////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AbbreviationsValidator.cs
//
// FluentValidation validator for abbreviations configuration.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.Configuration;
using FluentValidation;
using System;
using System.Linq;

namespace ACAT.Core.Validation
{
    /// <summary>
    /// Validator for abbreviations configuration
    /// </summary>
    public class AbbreviationsValidator : AbstractValidator<AbbreviationsJson>
    {
        public AbbreviationsValidator()
        {
            RuleFor(x => x.Abbreviations)
                .NotNull()
                .WithMessage("Abbreviations list cannot be null");

            RuleForEach(x => x.Abbreviations)
                .SetValidator(new AbbreviationValidator());
        }
    }

    /// <summary>
    /// Validator for a single abbreviation entry
    /// </summary>
    public class AbbreviationValidator : AbstractValidator<AbbreviationJson>
    {
        private static readonly string[] ValidModes = Enum.GetNames(typeof(Abbreviation.AbbreviationMode));

        public AbbreviationValidator()
        {
            RuleFor(x => x.Word)
                .NotEmpty()
                .WithMessage("Abbreviation word cannot be empty");

            RuleFor(x => x.ReplaceWith)
                .NotEmpty()
                .WithMessage("Abbreviation expansion (replaceWith) cannot be empty");

            RuleFor(x => x.Mode)
                .NotEmpty()
                .WithMessage("Abbreviation mode cannot be empty")
                .Must(mode => ValidModes.Contains(mode, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Abbreviation mode must be one of: {string.Join(", ", ValidModes)}");
        }
    }
}
