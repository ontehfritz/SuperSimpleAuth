namespace SuperSimple.Auth.Manager
{
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;
    using Repository;

    public class IndexModule : NancyModule
    {
        public IndexModule (IRepository repository)
        {
            Get ["/"] = parameters =>
            {
                var model = new SignupModel ();

                if (Context.CurrentUser.IsAuthenticated ())
                {
                    var manager = (Manager)this.Context.CurrentUser;
                    return this.LoginAndRedirect 
                               (manager.Id, fallbackRedirectUrl: "/home");
                }

                return View ["Index", model];
            };
        }
    }
}

