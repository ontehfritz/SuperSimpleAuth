namespace SuperSimple.Auth.Manager
{
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;
    using SuperSimple.Auth.Manager.Repository;

    public class IndexModule : NancyModule
    {
        public IndexModule (IRepository repository)
        {
            Get ["/"] = parameters =>
            {
                SignupModel model = new SignupModel ();

                if (this.Context.CurrentUser.IsAuthenticated ())
                {
                    Manager manager = (Manager)this.Context.CurrentUser;
                    return this.LoginAndRedirect (manager.Id, fallbackRedirectUrl: "/home");
                }

                return View ["Index", model];
            };
        }
    }
}

