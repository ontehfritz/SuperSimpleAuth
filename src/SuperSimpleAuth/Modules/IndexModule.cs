using System;
using Nancy;
using System.Web.UI.WebControls.WebParts;
using System.Web.Security;
using Mono.Security.X509;
using System.Linq;
using MongoDB.Driver;
using System.Web;
using System.Net;
using System.Web.Services.Description;

namespace SuperSimple.Auth.Api
{
	public class IndexModule : NancyModule
	{
       
        public IndexModule ()
		{

            Get ["/"] = parameters => {
                return View["index"];
            };

            Get ["/test"] = parameters => {
                return Response.AsJson(Request.Headers);
            };
           
           
		}
	}
}
