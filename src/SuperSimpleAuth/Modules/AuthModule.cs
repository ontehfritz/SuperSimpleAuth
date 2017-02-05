using System;
using Nancy;
using System.Linq;

namespace SuperSimple.Auth.Api
{
    public class AuthModule : NancyModule
    {
        IApiRepository repository;
        private const string _headerDomainKey = "Ssa-Domain-Key";

        public AuthModule (IApiRepository repository)
        {
            this.repository = repository;

            Before += ctx => {
                ErrorMessage error = Helper.VerifyRequest(Request,repository);

                if(error != null)
                {
                    return Response.AsJson(error,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());

                if(!repository.IpAllowed(domainKey, Request.UserHostAddress))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Server IP not accepted";
                    e.Status = "IpNotAllowed";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                return null;
            };

            Post ["/email"] = parameters => {

                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());

                Guid token = Guid.Parse(Request.Form["AuthToken"]);
                string newEmail= Request.Form["NewEmail"];
                //string IP = Request.Form["IP"];

                if(repository.EmailExists(domainKey, newEmail))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Email already exist. Please choose another.";
                    e.Status = "DuplicateUser";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }


                return Response.AsJson(repository.ChangeEmail(domainKey, token, newEmail));
            };

            Post ["/username"] = parameters => {
                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    
                Guid token = Guid.Parse(Request.Form["AuthToken"]);
                string newUserName = Request.Form["NewUserName"];
                //string IP = Request.Form["IP"];

                if(repository.UsernameExists(domainKey, newUserName))
                {
                    ErrorMessage e = new ErrorMessage();
                    e.Message = "Username already exist. Please choose another.";
                    e.Status = "DuplicateUser";

                    return Response.AsJson(e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }


                return Response.AsJson(repository.ChangeUserName(domainKey, token, newUserName));
            };



            Post ["/password"] = parameters => {

                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    
                Guid token = Guid.Parse(Request.Form["AuthToken"]);
                string newPassword = Request.Form["NewPassword"];
                //string IP = Request.Form["IP"];

                return Response.AsJson(repository.ChangePassword(domainKey, token, newPassword));
            };

            Post ["/forgot"] = parameters => {
                string email = Request.Form["Email"];

                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    
                if(email == null || email == "")
                {
                    ErrorMessage noemail = new ErrorMessage();
                    noemail.Status = "EmailNotFound";
                    noemail.Message = "No email provided";

                    return Response.AsJson(noemail,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                if(repository.EmailExists(domainKey, email))
                {
                    string newPassword = repository.Forgot(domainKey, email);

                    if(newPassword != null)
                    {
                        return Response.AsJson(newPassword);
                    }
                }

                ErrorMessage emailNotFound = new ErrorMessage();
                emailNotFound.Status = "EmailNotFound";
                emailNotFound.Message = "Email does not exist";

                return Response.AsJson(emailNotFound,
                    Nancy.HttpStatusCode.UnprocessableEntity);

            };

            /// <summary>
            /// End the user's session and erase auth token.
            /// After this is called the user must logon again
            /// </summary>
            /// <param name="authToken">Auth token.</param>
            Post ["/end"] = parameters => {

                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    
                Guid token = Guid.Parse(Request.Form["AuthToken"]);

                bool end = repository.End(domainKey,token);

                return Response.AsJson(end);

            };

            Post ["/validate"] = parameters => {
                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());

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
                    Username = user.UserName, 
                    Email = user.Email,
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };

                return Response.AsJson(u);

            };

            Post ["/disable"] = parameters => {

                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    

                Guid token = Guid.Parse(Request.Form["AuthToken"]);

                return Response.AsJson(repository.Disable(token,domainKey));
            };

            Post ["/authenticate"] = parameters => {
            
                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    
                User user = null;
                string username = Request.Form["Username"];
                string secret = Request.Form["Secret"];
                string IP = Request.Form["IP"];

                user = repository.Authenticate(domainKey, 
                    username, secret,
                    IP);

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
                    UserName = user.UserName, 
                    Email = user.Email,
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };

                return Response.AsJson(u);

            };

            Post ["/user"] = parameters => {
                Guid domainKey = 
                    Guid.Parse(Request.Headers[_headerDomainKey].FirstOrDefault());
                    
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
                    UserName = user.UserName, 
                    Email = user.Email,
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims(),
                    Roles = user.GetRoles()
                };

                return Response.AsJson(u);
                //return Response.AsJson(true);
            };
        }
    }
}

