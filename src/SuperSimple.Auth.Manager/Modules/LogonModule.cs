namespace SuperSimple.Auth.Manager
{
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.ModelBinding;
    using Nancy.Validation;
    using System.Collections.Generic;
    using SuperSimple.Auth.Api.Repository;
    using SuperSimple.Auth.Api;
    using SuperSimple.Auth.Manager.Repository;

    public class LogonModule : NancyModule
    {
        public LogonModule (IRepository repository, IApiRepository api)
        {
            
            Get ["/forgot"] = parameters =>
            {
                var model = new ForgotPasswordModel ();

                return View ["Forgot", model];
            };

            Post ["/forgot"] = parameters =>
            {
                var model = this.Bind<ForgotPasswordModel> ();
                model.Errors = new List<Error> ();
                var result = this.Validate (model);

                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors (result);
                    return View ["Forgot", model];
                }

                string password = repository.ForgotPassword (model.Email);

                if (string.IsNullOrEmpty (password))
                {
                    var error = new Error ();
                    error.Name = "NoAccount";
                    error.Message = "Account does not exist. Please sign up for an account.";
                    model.Errors.Add (error);
                }
                else
                {
                    var body = "You have requested a new password.\n";
                    body += string.Format ("password: {0}\n", password);
                    body += "login at: https://www.supersimpleauth.com\n\n\n";
                    body += "If you did not request this password please report this activity to: abuse@supersimpleauth.com \n";
                    body += string.Format ("The request was generated from IP: {0}", Request.UserHostAddress);

                    Email.Send ("supersimpleauth.com", model.Email,
                        string.Format ("New password for: {0}", model.Email), body);

                    model.Messages.Add ("Your new password has been sent to your email.");
                }

                return View ["Forgot", model];
            };

            Get ["/logon"] = parameters =>
            {
                var logon = new LogonModel ();

                return View ["Logon", logon];
            };

            Post ["/logon"] = parameters =>
            {
                var model = this.Bind<LogonModel> ();

                model.Errors = new List<Error> ();
                var result = this.Validate (model);

                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors (result);
                    return View ["Logon", model];
                }

                var manager = api.Authenticate(repository.SsaDomain.Key,
                                               model.Username,
                                               model.Secret);
                var error = new Error ();
                error.Name = "SignInError";
                error.Message = "Password or username incorrect.";
                model.Errors.Add (error);

                if (manager == null)
                {
                    return View ["Logon", model];
                }

                return this.LoginAndRedirect (manager.Id, fallbackRedirectUrl: "/home");
            };

            Get ["/logoff"] = parameters =>
            {
                return this.LogoutAndRedirect ("/Logon");
            };

            Get ["/signup"] = parameters =>
            {
                var model = new SignupModel ();
                return View ["Index", model];
            };

            Post ["/signup"] = parameters =>
            {
                var model = this.Bind<SignupModel> ();
                model.Errors = new List<Error> ();
                var result = this.Validate (model);

                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors (result);
                    return View ["Index", model];
                }

                try
                {
                    repository.CreateManager (model.Email,model.Secret);
                }
                catch (MongoDB.Driver.WriteConcernException e)
                {
                    var error = new Error ()
                    {
                        Name = "Duplicate",
                        Message = "This email has an account."
                    };

                    model.Errors.Add (error);
                    return View ["Index", model];
                }

                var logon = new LogonModel ();
                logon.Messages.Add ("Successully created your account. Please Sign In.");

                return View ["Logon", logon];
            };

            Get ["/settings"] = parameters =>
            {
                var model = new SettingsModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                return View ["Settings", model];
            };

            Post ["/settings"] = parameters =>
            {
                var model = this.Bind<SettingsModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;

                if (Request.Form.ChangeEmail != null)
                {
                    repository.ChangeEmail (model.Manager.Id,
                        model.Password, model.Email);
                }

                if (Request.Form.ChangePassword)
                {
                    repository.ChangePassword (model.Manager.Id, model.OldPassword,
                        model.NewPassword, model.ConfirmPassword);
                }

                if (Request.Form.Delete)
                {
                    if (!string.IsNullOrEmpty (model.DeletePassword))
                    {
                        repository.DeleteManager (model.Manager.Id, model.DeletePassword);
                        return this.Response.AsRedirect ("/");
                    }
                    else
                    {
                        var error = new Error ();
                        error.Name = "Password";
                        error.Message = "Please supply a valid password to delete account.";
                        model.Errors.Add (error);

                        return View ["Settings", model];
                    }
                }

                return Response.AsRedirect ("/settings");
            };
        }
    }
}

