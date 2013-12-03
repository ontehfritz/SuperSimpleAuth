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


            Get["/forgot"] = parameters => {
                ForgotPasswordModel model = new ForgotPasswordModel();

                return View["forgot",model];
            };

            Post["/forgot"] = parameters => {
                ForgotPasswordModel model = this.Bind<ForgotPasswordModel>();
                string password = repository.ForgotPassword(model.Email);

                string body = "You have requested a new password.\n";
                body += string.Format("password: {0}\n", password);
                body += "login at: https://www.supersimpleauth.com\n\n\n";
                body += "If you did not request this password please report this activity to: abuse@supersimpleauth.com \n";
                body += string.Format("The request was generated from IP: {0}", Request.UserHostAddress);

//                Email.Send("supersimpleauth.com",model.Email,
//                    string.Format("New password for: {0}", model.Email), body);

                if(password == null)
                {
                    model.Message = "Account does not exist. Please sign up for an account.";
                }
                else
                {
                    model.Message = "Your new password has been sent to your email.";
                }

                return View["forgot", model];
            };
           
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
                    return View["logon", model];
                }
  
                if (manager != null && manager.Secret != model.Secret)
                {
                    return View["logon", model];
                }

                return this.LoginAndRedirect(manager.Id, fallbackRedirectUrl: "/home");
            };

            Get["/logoff"] = parameters => {
                return this.LogoutAndRedirect("/logon");
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
                    UserName = signup.Email,
                    Secret = signup.Secret
                };

                newManager = repository.CreateManager(newManager);

                return this.Response.AsRedirect("/logon");
            };

            Get ["/settings"] = parameters => {
                SettingsModel model = new SettingsModel();
                model.Manager = (Manager)this.Context.CurrentUser;
                return View["settings", model];
            };

            Post ["/settings"] = parameters => {
                SettingsModel model = this.Bind<SettingsModel>();
                model.Manager = (Manager)this.Context.CurrentUser;

                if(Request.Form.ChangeEmail != null)
                {
                    repository.ChangeEmail(model.Manager.Id,
                        model.Password, model.Email);
                }

                if(Request.Form.ChangePassword)
                {
                    repository.ChangePassword(model.Manager.Id, model.OldPassword,
                        model.NewPassword,model.ConfirmPassword);
                }

                if(Request.Form.Delete)
                {
                    repository.DeleteManager(model.Manager.Id);
                    return this.Response.AsRedirect("/");
                }

                //model.Manager = (Manager)this.Context.CurrentUser;

                return this.Response.AsRedirect("/settings");
            };
        }
    }
}

