using System;
using Nancy;
using SuperSimple.Auth;
using Nancy.Security;


namespace SSANancyExample
{
    public class IndexModule : NancyModule
    {
        public IndexModule ()
        {
            this.RequiresAuthentication ();

            //Get ["/"] = parameters => {
            //    var nuser = (NancyUserIdentity)Context.CurrentUser;
            //    return View["index", nuser];
            //};

        }
    }
}

