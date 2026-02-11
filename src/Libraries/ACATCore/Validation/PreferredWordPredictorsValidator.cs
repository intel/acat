////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferredWordPredictorsValidator.cs
//
// FluentValidation validator for PreferredWordPredictorsJson configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using FluentValidation;
using System;

namespace ACAT.Core.Validation
{
    /// <summary>
    /// Validates PreferredWordPredictorsJson configuration using FluentValidation
    /// </summary>
    public class PreferredWordPredictorsValidator : AbstractValidator<PreferredWordPredictorsJson>
    {
        public PreferredWordPredictorsValidator()
        {
            RuleFor(x => x.WordPredictors)
                .NotNull()
                .WithMessage("WordPredictors list cannot be null");

            RuleForEach(x => x.WordPredictors)
                .SetValidator(new PreferredWordPredictorItemValidator());
        }
    }

    /// <summary>
    /// Validates individual PreferredWordPredictorJson items
    /// </summary>
    public class PreferredWordPredictorItemValidator : AbstractValidator<PreferredWordPredictorJson>
    {
        public PreferredWordPredictorItemValidator()
        {
            RuleFor(x => x.Language)
                .NotEmpty()
                .WithMessage("Language cannot be empty");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Word predictor ID cannot be empty")
                .Must(BeValidGuid)
                .WithMessage("Word predictor ID must be a valid GUID");
        }

        private bool BeValidGuid(string id)
        {
            return Guid.TryParse(id, out _);
        }
    }
}
