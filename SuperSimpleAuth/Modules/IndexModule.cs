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
        IRepository repository;

        public IndexModule (IRepository repository)
		{
            this.repository = repository;

            Get ["/"] = parameters => {
                return "index";
            };

            Post ["/"] = parameters => {
                return "index";
            };

            Post ["/end"] = parameters => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                                           Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid appKey = 
                Guid.Parse(Request.Headers["ssa_app_key"].FirstOrDefault());

                Guid token = Guid.Parse(Request.Form["AuthToken"]);

                bool end = repository.End(appKey,token);

                return Response.AsJson(end);

            };

            Post ["/validate"] = parameters => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                                           Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid appKey = 
                Guid.Parse(Request.Headers["ssa_app_key"].FirstOrDefault());

                Guid token = Guid.Parse(Request.Form["AuthToken"]);
                string IP = Request.Form["IP"];
                User user = null;

                user = repository.Validate(token,appKey,IP);

                if(user == null)
                {
                    ErrorMessage message = new ErrorMessage{
                        Status = "InvalidToken",
                        Message =  "AuthToken is not valid or expired. Re-Authenticate user."
                    };

                    return Response.AsJson(message,
                                           Nancy.HttpStatusCode.Forbidden);
                }

                var u = new { 
                    Id = user.Id,
                    Username = user.Username, 
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };

                return Response.AsJson(u);
               
            };


	        Post ["/authenticate"] = parameters => {
               
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                                           Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid appKey = 
                Guid.Parse(Request.Headers["ssa_app_key"].FirstOrDefault());

                User user = null;
                string username = Request.Form["Username"];
                string secret = Request.Form["Secret"];
                string IP = Request.Form["IP"];

                user = repository.Authenticate(appKey, username, secret,IP);

                if(user == null)
                {
                    ErrorMessage message = new ErrorMessage{
                        Status = "AuthenticationFailed",
                        Message =  "User cannot be authenticated."
                    };

                    return Response.AsJson(message,
                                    Nancy.HttpStatusCode.Forbidden);
                }

                var u = new { 
                    Id = user.Id,
                    Username = user.Username, 
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };
            
                return Response.AsJson(u);
               
			};

            Post ["/Authorize"] = parameters => {
                return "Authorize";
            };

            Post ["/user"] = parameters => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                                           Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid appKey = 
                    Guid.Parse(Request.Headers["ssa_app_key"].FirstOrDefault());

                //string app = Request.Headers["ssa_app_key"].FirstOrDefault();

                User user = new User ();

                try
                {
                    user = repository.CreateUser (appKey, 
                                                  Request.Form["Username"],
                                                  Request.Form["Secret"]);
                }
                catch(Exception e)
                {
                    ErrorMessage message = new ErrorMessage{
                        Status = "DuplicateUser",
                        Message =  "Username already exists for this application."
                    };

                    return Response.AsJson(message,
                                           Nancy.HttpStatusCode.UnprocessableEntity);
                }

                var u = new { 
                    Id = user.Id,
                    Username = user.Username, 
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };
            
                return Response.AsJson(u);
            };
		}
	}
}
