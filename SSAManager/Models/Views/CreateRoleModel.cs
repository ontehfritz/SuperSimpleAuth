using System;
using System.Collections.Generic;
using Nancy.Validation;
using FluentValidation;

namespace SSAManager
{
    public class CreateRoleModel: PageModel
    {

        public Domain Domain { get; set; }
        public string Name { get; set; }
      
        public CreateRoleModel() : base()
        {

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

