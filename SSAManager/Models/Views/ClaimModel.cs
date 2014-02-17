using System;
using FluentValidation;
using Nancy.Validation;
using System.Collections.Generic;

namespace SSAManager
{
    public class ClaimModel : PageModel
    {
        public string Name { get; set; }
        public Domain Domain { get; set; }
        public User[] Users { get; set; }
        public Role[] Roles { get; set; }

        public ClaimModel() : base()
        {
           
        }
    }

    public class ClaimModelValidator : AbstractValidator<ClaimModel>
    {
        public ClaimModelValidator()
        {
            RuleFor(claim => claim.Name).NotEmpty();
        }
    }
}

