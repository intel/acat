////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PronunciationsValidator.cs
//
// FluentValidation validator for pronunciations configuration.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using FluentValidation;

namespace ACAT.Core.Validation
{
    /// <summary>
    /// Validator for pronunciations configuration
    /// </summary>
    public class PronunciationsValidator : AbstractValidator<PronunciationsJson>
    {
        public PronunciationsValidator()
        {
            RuleFor(x => x.Pronunciations)
                .NotNull()
                .WithMessage("Pronunciations list cannot be null");

            RuleForEach(x => x.Pronunciations)
                .SetValidator(new PronunciationValidator());
        }
    }

    /// <summary>
    /// Validator for a single pronunciation entry
    /// </summary>
    public class PronunciationValidator : AbstractValidator<PronunciationJson>
    {
        public PronunciationValidator()
        {
            RuleFor(x => x.Word)
                .NotEmpty()
                .WithMessage("Pronunciation word cannot be empty");

            RuleFor(x => x.Pronunciation)
                .NotEmpty()
                .WithMessage("Pronunciation value cannot be empty");
        }
    }
}
