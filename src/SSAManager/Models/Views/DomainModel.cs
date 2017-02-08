namespace SuperSimple.Auth.Manager
{
    using System.Collections.Generic;
    using FluentValidation;
    using Api;
    using SuperSimple.Auth.Api.Repository;

    public class DomainModel : PageModel
    {
        public List<Role> Roles         { get; set; }
        public List<User> Users         { get; set; }
        public List<Manager> Admins     { get; set; }
        public Domain Domain            { get; set; }
        public string[] WhiteListIps    { get; set; }
        public string Name              { get; set; }

        public DomainModel() : base(){}
    }
        
    public class DomainModelValidator : AbstractValidator<DomainModel>
    {
        public DomainModelValidator()
        {
            RuleFor(domain => domain.Name).NotEmpty();
        }
    }
}

