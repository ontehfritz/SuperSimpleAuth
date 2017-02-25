namespace SuperSimple.Auth.Manager
{
    using FluentValidation;
    using Repository;

    public class CreateRoleModel: PageModel
    {
        public Domain Domain { get; set; }
        public string Name { get; set; }
      
        public class RoleValidator : AbstractValidator<CreateRoleModel>
        {
            public RoleValidator()
            {
                RuleFor(role => role.Name).NotEmpty();
            }
        }
    }
}

