using System;
using Nancy.Validation;
using System.Collections.Generic;
using FluentValidation;

namespace SSAManager
{
    public class Claim
    {
        public string Name { get; set; }
        public string AppName { get; set; }
        public IEnumerable<ModelValidationError> Errors { get; set; }
    }

    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator()
        {
            RuleFor(claim => claim.Name).NotEmpty();
        }
    }
}

