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
        public Domain Domain { get; set; }
        public User[] Users { get; set; }
        public Role[] Roles { get; set; }
        public List<string> Messages { get; set; }
        public List<Error> Errors { get; set; }

        public ClaimModel()
        {
            Messages = new List<string> ();
            Errors = new List<Error> ();
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

