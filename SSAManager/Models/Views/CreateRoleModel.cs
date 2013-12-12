using System;
using System.Collections.Generic;
using Nancy.Validation;
using FluentValidation;

namespace SSAManager
{
    public class CreateRoleModel: IPageModel
    {

        public Domain Domain { get; set; }
        public Manager Manager { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<Error> Errors { get; set; }
        public List<string> Messages { get; set; }

        public CreateRoleModel()
        {
            Messages = new List<string> ();
            Errors = new List<Error> ();
        }

        public class RoleValidator : AbstractValidator<CreateRoleModel>
        {
            public RoleValidator()
            {
                RuleFor(role => role.Name).NotEmpty();
            }
        }
    }
}

