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
                return View["index"];
            };


            Post ["/forgot"] = parameters => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);
            
                if(error != null)
                {
                    return Response.AsJson(error,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid domainKey = 
                    Guid.Parse(Request.Headers["ssa_domain_key"].FirstOrDefault());

                if(!repository.IpAllowed(domainKey, Request.UserHostAddress))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Server IP not accepted";
                    e.Status = "IpNotAllowed";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                string email = Request.Form["Email"];
                string website = Request.Form["website"];

                if(repository.EmailExists(domainKey, email))
                {

                    
                    return Response.AsJson(true);
                }

                ErrorMessage emailNotFound = new ErrorMessage();
                error.Status = "EmailNotFound";
                error.Message = "Email does not exist";

                return Response.AsJson(emailNotFound,
                    Nancy.HttpStatusCode.UnprocessableEntity);
            };

            /// <summary>
            /// End the user's session and erase auth token.
            /// After this is called the user must logon again
            /// </summary>
            /// <param name="authToken">Auth token.</param>
            Post ["/end"] = parameters => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid domainKey = 
                    Guid.Parse(Request.Headers["ssa_domain_key"].FirstOrDefault());

                if(!repository.IpAllowed(domainKey, Request.UserHostAddress))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Server IP not accepted";
                    e.Status = "IpNotAllowed";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid token = Guid.Parse(Request.Form["AuthToken"]);

                bool end = repository.End(domainKey,token);

                return Response.AsJson(end);

            };

            Post ["/validate"] = parameters => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                                           Nancy.HttpStatusCode.UnprocessableEntity);
                }


                Guid domainKey = 
                    Guid.Parse(Request.Headers["ssa_domain_key"].FirstOrDefault());

                if(!repository.IpAllowed(domainKey, Request.UserHostAddress))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Server IP not accepted";
                    e.Status = "IpNotAllowed";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid token = Guid.Parse(Request.Form["AuthToken"]);
                string IP = Request.Form["IP"];
                User user = null;

                user = repository.Validate(token,domainKey,IP);

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
                    Email = user.Email,
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

                Guid domainKey = 
                    Guid.Parse(Request.Headers["ssa_domain_key"].FirstOrDefault());

                if(!repository.IpAllowed(domainKey, Request.UserHostAddress))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Server IP not accepted";
                    e.Status = "IpNotAllowed";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                User user = null;
                string username = Request.Form["Username"];
                string secret = Request.Form["Secret"];
                string IP = Request.Form["IP"];

                user = repository.Authenticate(domainKey, username, secret,IP);

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
                    Email = user.Email,
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

                Guid domainKey = 
                    Guid.Parse(Request.Headers["ssa_domain_key"].FirstOrDefault());

                if(!repository.IpAllowed(domainKey, Request.UserHostAddress))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Server IP not accepted";
                    e.Status = "IpNotAllowed";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                User user = new User ();

                try
                {
                    if(Request.Form["Email"] != null && 
                        Request.Form["Email"] != "" &&
                        repository.EmailExists(domainKey, Request.Form["Email"]))
                    {
                        ErrorMessage message = new ErrorMessage{
                            Status = "DuplicateUser",
                            Message =  "Email is already being used for this application."
                        };

                        return Response.AsJson(message,
                            Nancy.HttpStatusCode.UnprocessableEntity);
                    }

                    user = repository.CreateUser (domainKey, 
                        Request.Form["Username"],
                        Request.Form["Secret"],
                        Request.Form["Email"]);
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
                    Email = user.Email,
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };
            
                return Response.AsJson(u);
            };
		}
	}
}
