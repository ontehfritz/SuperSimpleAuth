using System;
using System.Collections.Generic;
using FluentValidation;

namespace SSAManager
{
    public class ForgotPasswordModel : PageModel
    {
        public string Email { get; set; }

        public ForgotPasswordModel() : base()
        {

        }
    }

    public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordModelValidator()
        {
            RuleFor(forgot => forgot.Email).NotEmpty();
            RuleFor(forgot => forgot.Email).EmailAddress();
        }
    }
}

