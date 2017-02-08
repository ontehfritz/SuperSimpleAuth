namespace SuperSimple.Auth.Manager
{
    using FluentValidation;
    using Api;
    using SuperSimple.Auth.Api.Repository;

    public class ClaimModel : PageModel
    {
        public string Name { get; set; }
        public Domain Domain { get; set; }
        public User[] Users { get; set; }
        public Role[] Roles { get; set; }
    }

    public class ClaimModelValidator : AbstractValidator<ClaimModel>
    {
        public ClaimModelValidator()
        {
            RuleFor(claim => claim.Name).NotEmpty();
        }
    }
}

