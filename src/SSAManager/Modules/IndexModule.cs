namespace SuperSimple.Auth.Manager
{
    using System;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;

    public class IndexModule : NancyModule
    {
        IRepository repository; 

        public IndexModule (IRepository repository)
        {
            this.repository = repository;

            Get ["/"] = parameters => {
                SignupModel model = new SignupModel();
   
                if(this.Context.CurrentUser.IsAuthenticated())
                {
                    Manager manager  = (Manager)this.Context.CurrentUser;
                    return this.LoginAndRedirect(manager.Id, fallbackRedirectUrl: "/home");
                }

                return View["Index", model];
            };
        }
    }
}

