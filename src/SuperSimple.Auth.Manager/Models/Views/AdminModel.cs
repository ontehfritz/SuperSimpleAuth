namespace SuperSimple.Auth.Manager
{
    using Api;
    using Repository;
    using FluentValidation;

    public class AdminModel : PageModel
    {
        public string Email     { get; set; }        
        public Domain Domain    { get; set; }
        public IUser Admin       { get; set; }
    }

    public class AdminModelValidator : AbstractValidator<AdminModel>
    {
        public AdminModelValidator()
        {
            RuleFor(logon => logon.Email).NotEmpty();
        }
    }
}

