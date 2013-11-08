using System;
using FluentValidation;
using Nancy.Validation;
using System.Collections.Generic;

namespace SSAManager
{
    public class ClaimModel : IPageModel
    {
        public Manager Manager { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string AppName { get; set; }
        public IEnumerable<ModelValidationError> Errors { get; set; }
    }

    public class ClaimModelValidator : AbstractValidator<ClaimModel>
    {
        public ClaimModelValidator()
        {
            RuleFor(claim => claim.Name).NotEmpty();
        }
    }
}

