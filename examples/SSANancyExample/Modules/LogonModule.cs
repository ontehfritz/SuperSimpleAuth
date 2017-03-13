namespace SSANancyExample
{
    using System;
    using System.Linq;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.ModelBinding;
    using Nancy.Validation;
    using SuperSimple.Auth;

    public class LogonModule : NancyModule
    {
        public LogonModule (SuperSimpleAuth ssa)
        {
            Get["/logon"] = parameters => {
                var model = new LogonModel();
               
                return View["logon",model];
            };

            Post["/logon"] = parameters => {
                var model = this.Bind<LogonModel>();
                model.Message = "Password or/and Username is incorrect.";

                User user = null;

                try
                {
                   user = ssa.Authenticate(model.Username,model.Secret,
                                            this.Context.Request.UserHostAddress);
                    
                }
                catch(Exception e)
                {
                    model.Message = e.Message;
                    if(user == null)
                    {
                        return View["logon", model];
                    }
                }

                return this.LoginAndRedirect(user.AuthToken, 
                                             fallbackRedirectUrl: "/");
            };

            //Get["/logoff"] = parameters => {
            //    var nuser = (NancyUserIdentity)Context.CurrentUser;
            //    ssa.End(nuser.AuthToken);

            //    return this.LogoutAndRedirect("/");
            //};

            Get ["/signup"] = parameters => {
                var signup = new SignupModel();
                return View["signup", signup];
            };

            Post ["/signup"] = parameters => {

                var signup = this.Bind<SignupModel>();
                var result = this.Validate(signup);
                signup.Errors = Error.GetValidationErrors(result).ToList();

                if (!result.IsValid)
                {
                    return View["signup", signup];
                }

                try
                {
                    ssa.CreateUser(signup.UserName, signup.Secret, signup.Email);
                }
                catch(Exception e)
                {
                    signup.Message = e.Message;
                    return View["signup", signup];
                }

                return this.Response.AsRedirect("/");
            };
        }
    }
}

