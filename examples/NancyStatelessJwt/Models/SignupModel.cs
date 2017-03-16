namespace NancyStatelessJwt
{
    using FluentValidation;
    using System.Collections.Generic;

    public class SignupModel
    {
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Secret { get; set; }
        public string ComfirmSecret { get; set; }
        public List<string> Errors { get; set; }

    }

    public class SignupValidator : AbstractValidator<SignupModel>
    {
        public SignupValidator()
        {
            RuleFor(signup => signup.UserName).NotEmpty();
            RuleFor(signup => signup.Email).EmailAddress();
            RuleFor(signup => signup.Secret).NotEmpty();
        }
    }
}

