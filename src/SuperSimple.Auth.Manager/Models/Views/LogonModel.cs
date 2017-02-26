namespace SuperSimple.Auth.Manager
{
    using FluentValidation;

    public class LogonModel : PageModel
    {
        public string Username { get; set; }
        public string Secret { get; set; }
    }

    public class LogonModelValidator : AbstractValidator<LogonModel>
    {
        public LogonModelValidator()
        {
            RuleFor(logon => logon.Username).NotEmpty();
            RuleFor(logon => logon.Secret).NotEmpty();
        }
    }
}

