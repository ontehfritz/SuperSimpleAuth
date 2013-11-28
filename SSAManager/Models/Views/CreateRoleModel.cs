using System;
using System.Collections.Generic;
using Nancy.Validation;
using FluentValidation;

namespace SSAManager
{
    public class CreateRoleModel
    {

        public Domain Domain { get; set; }
        public Manager Manager { get; set; }
        public string Name { get; set; }
        public List<Error> Errors { get; set; }

        public class RoleValidator : AbstractValidator<CreateRoleModel>
        {
            public RoleValidator()
            {
                RuleFor(role => role.Name).NotEmpty();
            }
        }
    }
}

