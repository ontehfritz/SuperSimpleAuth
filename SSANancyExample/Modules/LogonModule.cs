using SSANancyExample;
using SuperSimple.Auth;

namespace SSANancyExample
{
    using System;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.ModelBinding;
    using Nancy.Validation;

    public class LogonModule : NancyModule
    {
        //take advantage of nancy's IoC
        //see bootstrapper.cs this is where SSA gets intialized
        SuperSimpleAuth ssa; 

        public LogonModule (SuperSimpleAuth ssa)
        {
            this.ssa = ssa;

            Get["/logon"] = parameters => {
                LogonModel logon = new LogonModel();
               
                return View["logon",logon];
            };

            Post["/logon"] = parameters => {
                LogonModel logon = this.Bind<LogonModel>();
                logon.Message = "Password or/and Username is incorrect.";

                User user = null;

                try
                {
                   user = ssa.Authenticate(logon.Username,logon.Secret,
                                            this.Context.Request.UserHostAddress);
                }
                catch(Exception e)
                {
                    logon.Message = e.Message;
                    if(user == null)
                    {
                        return View["logon", logon];
                    }
                }

                return this.LoginAndRedirect(user.AuthToken, fallbackRedirectUrl: "/");
            };

            Get["/logoff"] = parameters => {
                NancyUserIdentity nuser = (NancyUserIdentity)Context.CurrentUser;
                ssa.End(nuser.AuthToken);

                return this.LogoutAndRedirect("/");
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

