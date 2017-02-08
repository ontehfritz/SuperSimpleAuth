namespace SuperSimple.Auth.Manager
{
    using FluentValidation;

    public class ForgotPasswordModel : PageModel
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordModelValidator ()
        {
            RuleFor (forgot => forgot.Email).NotEmpty ();
            RuleFor (forgot => forgot.Email).EmailAddress ();
        }
    }
}

