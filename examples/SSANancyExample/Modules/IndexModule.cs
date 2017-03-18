namespace SSANancyExample
{
    using Nancy;
    using Nancy.Security;

    public class IndexModule : NancyModule
    {
        public IndexModule ()
        {
            this.RequiresAuthentication ();

            Get ["/"] = parameters =>
            {
                var nuser = (NancyUserIdentity)Context.CurrentUser;
                return View ["index", nuser];
            };

        }
    }
}

