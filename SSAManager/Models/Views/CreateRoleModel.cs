using System;
using System.Collections.Generic;
using Nancy.Validation;
using FluentValidation;

namespace SSAManager
{
    public class CreateRoleModel
    {

        public App App { get; set; }
        public Manager Manager { get; set; }
        public string Name { get; set; }
        public IEnumerable<ModelValidationError> Errors { get; set; }

        public class RoleValidator : AbstractValidator<CreateRoleModel>
        {
            public RoleValidator()
            {
                RuleFor(role => role.Name).NotEmpty();
            }
        }
    }
}

