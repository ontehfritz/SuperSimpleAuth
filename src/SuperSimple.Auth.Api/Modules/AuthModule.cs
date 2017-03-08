namespace SuperSimple.Auth.Api
{
    using System;
    using Nancy;
    using System.Linq;
    using Repository;
    using Token;

    public class AuthModule : NancyModule
    {
        private const string _headerDomainKey = "Ssa-Domain-Key";
        private const string _authorization = "Authorization";

        public AuthModule (IApiRepository repository)
        {
            Before += ctx =>
            {
                //var token =
                //    Request.Headers [_authorization].FirstOrDefault ();

                //var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));

                //var key = repository.GetAuthToken(Guid.Parse(jwt.Payload.Audience),
                //                                   jwt.Payload.Username);

                //if(Jwt.Validate(token.Replace("\"", 
                //                              string.Empty), key))
                //{
                //    return Response.AsJson (true);
                //}
                
                //var error = Error.VerifyRequest (Request, repository);

                //if (error != null)
                //{
                //    return Response.AsJson (error,
                //        Nancy.HttpStatusCode.UnprocessableEntity);
                //}
                ///////old
                //var domainKey =
                //    Guid.Parse (Request.Headers [_headerDomainKey].FirstOrDefault ());

                //if (!repository.IpAllowed (domainKey, Request.UserHostAddress))
                //{
                //    var e = new ErrorMessage ();
                //    e.Message = "Server IP not accepted";
                //    e.Status = "IpNotAllowed";

                //    return Response.AsJson (e,
                //       HttpStatusCode.UnprocessableEntity);
                //}

                return null;
            };

            Post ["/email"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);

                string newEmail = Request.Form ["NewEmail"];
                //string IP = Request.Form["IP"];

                if (repository.EmailExists (domainKey, newEmail))
                {
                    var e = new ErrorMessage ();
                    e.Message = "Email already exist. Please choose another.";
                    e.Status = "DuplicateUser";

                    return Response.AsJson (e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                var key = Guid.Parse(repository.GetAuthToken(
                    Guid.Parse(jwt.Payload.Audience),
                    jwt.Payload.Username));
                
                return Response.AsJson (repository.ChangeEmail 
                                        (domainKey, key, newEmail));
            };

            Post ["/username"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);
                string newUserName = Request.Form ["NewUserName"];
                //string IP = Request.Form["IP"];

                if (repository.UsernameExists (domainKey, newUserName))
                {
                    var e = new ErrorMessage ();
                    e.Message = "Username already exist. Please choose another.";
                    e.Status = "DuplicateUser";

                    return Response.AsJson (e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                var key = Guid.Parse(repository.GetAuthToken(
                    Guid.Parse(jwt.Payload.Audience),
                    jwt.Payload.Username));
                
                return Response.AsJson (repository
                                       .ChangeUserName (domainKey,
                                                       key, newUserName));
            };

            Post ["/password"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);

                Guid key = Guid.Parse(repository.GetAuthToken(
                    Guid.Parse(jwt.Payload.Audience),
                    jwt.Payload.Username));
                
                string newPassword = Request.Form ["NewPassword"];
                //string IP = Request.Form["IP"];

                return Response.AsJson (repository
                                       .ChangePassword (domainKey, key, 
                                                        newPassword));
            };

            Post ["/forgot"] = parameters =>
            {
                string email = Request.Form ["Email"];

                Guid domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey]
                                .FirstOrDefault ());

                if (string.IsNullOrEmpty (email))
                {
                    var noemail = new ErrorMessage ();
                    noemail.Status = "EmailNotFound";
                    noemail.Message = "No email provided";

                    return Response.AsJson (noemail,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                if (repository.EmailExists (domainKey, email))
                {
                    string newPassword = repository.Forgot (domainKey, email);

                    if (newPassword != null)
                    {
                        return Response.AsJson (newPassword);
                    }
                }

                var emailNotFound = new ErrorMessage ();
                emailNotFound.Status = "EmailNotFound";
                emailNotFound.Message = "Email does not exist";

                return Response.AsJson (emailNotFound,
                    Nancy.HttpStatusCode.UnprocessableEntity);
            };

            /// <summary>
            /// End the user's session and erase auth token.
            /// After this is called the user must logon again
            /// </summary>
            /// <param name="authToken">Auth token.</param>
            Post ["/end"] = parameters =>
            {
                Guid domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey]
                                .FirstOrDefault ());

                Guid token = Guid.Parse (Request.Form ["AuthToken"]);

                bool end = repository.End (domainKey, token);

                return Response.AsJson (end);
            };

            Post ["/validate"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));

                string IP = Request.Form ["IP"];

                var key = repository.GetAuthToken(Guid.Parse(jwt.Payload.Audience),
                                        jwt.Payload.Username);

                if(Jwt.Validate(token.Replace("\"", 
                                              string.Empty), key))
                {
                    return Response.AsJson (true);
                }

                //return error here
                return Response.AsJson (false);
            };

            Post ["/disable"] = parameters =>
            {
                var domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey].FirstOrDefault ());


                Guid token = Guid.Parse (Request.Form ["AuthToken"]);

                return Response.AsJson (repository.Disable (token, domainKey));
            };

            Post ["/authenticate"] = parameters =>
            {
                var domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey]
                                .FirstOrDefault ());

                User user = null;
                string username = Request.Form ["Username"];
                string secret = Request.Form ["Secret"];
                string IP = Request.Form ["IP"];

                user = repository.Authenticate (domainKey,
                    username, secret,
                    IP);

                if (user == null)
                {
                    var message = new ErrorMessage
                    {
                        Status = "AuthenticationFailed",
                        Message = "User cannot be authenticated."
                    };

                    return Response.AsJson (message,
                        Nancy.HttpStatusCode.Forbidden);
                }


                var header = new Header();
                header.Algorithm = "HS256";
                header.Type = "JWT";

                var payload = new Payload();
                payload.Issuer = "autheticate.technology";
                payload.Audience = domainKey.ToString();
                payload.Username = user.UserName;
                payload.Email = user.Email;

                var jwt = new Jwt(header,payload);

                return Response.AsJson (Jwt.ToToken(jwt, 
                                                    user.AuthToken.ToString()));
            };

            Post ["/user"] = parameters =>
            {
                var domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey]
                                .FirstOrDefault ());

                var user = new User ();

                try
                {
                    if (Request.Form ["Email"] != null &&
                        Request.Form ["Email"] != "" &&
                        repository.EmailExists (domainKey, Request.Form ["Email"]))
                    {
                        var message = new ErrorMessage
                        {
                            Status = "DuplicateUser",
                            Message = "Email is already being used for this application."
                        };

                        return Response.AsJson (message,
                            Nancy.HttpStatusCode.UnprocessableEntity);
                    }

                    user = repository.CreateUser (domainKey,
                        Request.Form ["Username"],
                        Request.Form ["Secret"],
                        Request.Form ["Email"]);
                }
                catch (Exception e)
                {
                    var message = new ErrorMessage
                    {
                        Status = "DuplicateUser",
                        Message = "Username already exists for this application."
                    };

                    return Response.AsJson (message,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                return Response.AsJson (true);
            };
        }
    }
}

