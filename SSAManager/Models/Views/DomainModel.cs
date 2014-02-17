using System;
using System.Collections.Generic;
using Nancy.Validation;
using FluentValidation;

namespace SSAManager
{
    public class DomainModel : PageModel
    {
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
        public Domain Domain { get; set; }
        public string[] WhiteListIps { get; set; }
        public string Name { get; set; }


        public DomainModel() : base()
        {
          
        }
    }


    public class DomainModelValidator : AbstractValidator<DomainModel>
    {
        public DomainModelValidator()
        {
            RuleFor(domain => domain.Name).NotEmpty();
        }
    }
}

