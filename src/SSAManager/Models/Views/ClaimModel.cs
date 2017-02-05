namespace SSAManager
{
    using FluentValidation;
    using SuperSimple.Auth.Api;

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

