namespace SSAManager
{
    using System;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.ModelBinding;
    using Nancy.Validation;

    public class LogonModule : NancyModule
    {
        IRepository repository;

        public LogonModule (IRepository repository)
        {
            this.repository = repository;

            Get["/logon"] = parameters => {
                LogonModel logon = new LogonModel();
               
                return View["logon",logon];
            };

            Post["/logon"] = parameters => {
                LogonModel model = this.Bind<LogonModel>();

                Manager manager = repository.GetManager(model.Username);

                model.Message = "Password or/and Username is incorrect.";

                if(manager == null)
                {
                    return View["index", model];
                }
  
                if (manager != null && manager.Secret != model.Secret)
                {
                    return View["index", model];
                }

                return this.LoginAndRedirect(manager.Id, fallbackRedirectUrl: "/home");
            };

            Get["/logoff"] = parameters => {
                return this.LogoutAndRedirect("/");
            };

            Post["/logoff"] = parameters => {
                return "logoff";
            };

            Get ["/signup"] = parameters => {
                SignupModel signup = new SignupModel();
                return View["signup", signup];
            };

            Post ["/signup"] = parameters => {

                SignupModel signup = this.Bind<SignupModel>();
                var result = this.Validate(signup);
                signup.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["signup", signup];
                }

                Manager newManager = new Manager(){
                    Id = Guid.NewGuid(),
                    UserName = signup.Email,
                    Secret = signup.Secret
                };

                newManager = repository.CreateManager(newManager);

                return this.Response.AsRedirect("/");
            };
        }
    }
}

