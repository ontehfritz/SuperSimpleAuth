using System;
using System.Collections.Generic;
using Nancy.Validation;
using FluentValidation;

namespace SSAManager
{
    public class AppModel : IPageModel
    {
        public IEnumerable<ModelValidationError> Errors { get; set; }
        public Manager Manager { get; set; }
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
        public App App { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public class AppModelValidator : AbstractValidator<AppModel>
    {
        public AppModelValidator()
        {
            RuleFor(app => app.Name).NotEmpty();
        }
    }
}

