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
                Guid authToken = Guid.Parse(jwt.Payload.JwtTokenId);

                string newEmail = Request.Form ["NewEmail"];
                //string IP = Request.Form["IP"];

                if (repository.EmailExists (domainKey, newEmail))
                {
                    var e = new Error ();
                    e.Detail = "Email already exist. Please choose another.";
                    e.Status =  (int)HttpStatusCode.UnprocessableEntity;
                    e.Title = "Duplicate";
                    e.Source = new Source("/email");
                    return Response.AsJson (e,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                var key = repository.GetKey(authToken);

                if(repository.ChangeEmail 
                   (domainKey, authToken, newEmail))
                {
                    jwt.Payload.Email = newEmail;
                }

                return Response.AsJson (Jwt.ToToken(jwt, 
                                                    key),HttpStatusCode.OK);
            };

            Post ["/username"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);
                Guid authToken = Guid.Parse(jwt.Payload.JwtTokenId);

                string newUserName = Request.Form ["NewUserName"];
                //string IP = Request.Form["IP"];

                if (repository.UsernameExists (domainKey, newUserName))
                {
                    var e = new Error ();
                    e.Detail = "Username already exist. Please choose another.";
                    e.Status =  (int)HttpStatusCode.UnprocessableEntity;
                    e.Title = "Duplicate";
                    e.Source = new Source("/username");

                    return Response.AsJson (e,
                        HttpStatusCode.UnprocessableEntity);
                }

                var key = repository.GetKey(authToken);


                if(repository
                   .ChangeUserName (domainKey,
                                    authToken, newUserName))
                {
                    jwt.Payload.Username = newUserName;
                }

                return Response.AsJson (Jwt.ToToken(jwt, 
                                                    key),HttpStatusCode.OK);
            };

            Post ["/password"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);
                Guid authToken = Guid.Parse(jwt.Payload.JwtTokenId);

                string newPassword = Request.Form ["NewPassword"];
                //string IP = Request.Form["IP"];

                return Response.AsJson (repository
                                        .ChangePassword (domainKey, authToken, 
                                                        newPassword),
                                        HttpStatusCode.OK);
            };

            Post ["/forgot"] = parameters =>
            {
                var email = Request.Form ["Email"];

                Guid domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey]
                                .FirstOrDefault ());

                if (string.IsNullOrEmpty (email))
                {
                    var er = new Error ();
                    er.Detail = "Email could not be found.";
                    er.Status =  (int)HttpStatusCode.UnprocessableEntity;
                    er.Title = "Invalid";
                    er.Source = new Source("/forgot");

                    return Response.AsJson (er,
                        Nancy.HttpStatusCode.UnprocessableEntity);
                }

                if (repository.EmailExists (domainKey, email))
                {
                    string newPassword = repository.Forgot (domainKey, email);

                    if (newPassword != null)
                    {
                        return Response.AsJson (newPassword, HttpStatusCode.OK);
                    }
                }

                var e = new Error ();
                e.Detail = "Email could not be found.";
                e.Status =  (int)HttpStatusCode.UnprocessableEntity;
                e.Title = "Invalid";
                e.Source = new Source("/forgot");

                return Response.AsJson (e,
                    Nancy.HttpStatusCode.UnprocessableEntity);
            };

            /// <summary>
            /// End the user's session and erase auth token.
            /// After this is called the user must logon again
            /// </summary>
            /// <param name="authToken">Auth token.</param>
            Post ["/end"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);
                Guid authToken = Guid.Parse(jwt.Payload.JwtTokenId);

                var end = repository.End (domainKey, authToken);

                return Response.AsJson (end, HttpStatusCode.OK);
            };

            Post ["/validate"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));

                string IP = Request.Form ["IP"];

                var key = repository.GetKey(Guid.Parse(jwt.Payload.JwtTokenId));

                if(key == null) return Response.AsJson (false);

                if(Jwt.Validate(token.Replace("\"", 
                                              string.Empty), key))
                {
                    return Response.AsJson (true, HttpStatusCode.OK);
                }

                //return error here
                return Response.AsJson (false, HttpStatusCode.OK);
            };

            Post ["/validateauthtoken"] = parameters =>
            {
                Guid domainKey =
                    Guid.Parse (Request.Headers [_headerDomainKey].FirstOrDefault ());
                
                Guid token = Guid.Parse (Request.Form ["AuthToken"]);
                string IP = Request.Form ["IP"];
                User user = null;

                user = repository.Validate (token, domainKey, IP);

                if (user == null)
                {
                    var e = new Error ();
                    e.Detail = "AuthToken is not valid or expired.Re-Authenticate user";
                    e.Status =  (int)HttpStatusCode.UnprocessableEntity;
                    e.Title = "Invalid";
                    e.Source = new Source("/validateauthtoken");

                    return Response.AsJson (e,
                        Nancy.HttpStatusCode.Forbidden);
                }

                var header = new Header();
                header.Algorithm = "HS256";
                header.Type = "JWT";

                var payload = new Payload();
                payload.Issuer = "autheticate.technology";
                payload.Audience = user.DomainId.ToString();
                payload.JwtTokenId = user.AuthToken.ToString();
                payload.Username = user.UserName;
                payload.Email = user.Email;

                var jwt = new Jwt(header,payload);
                var u = new
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Jwt = Jwt.ToToken(jwt, repository.GetKey(user.AuthToken)),
                    AuthToken = user.AuthToken,
                    Claims = user.GetClaims (),
                    Roles = user.GetRoles ()
                };

                return Response.AsJson (u, HttpStatusCode.OK);

            };

            Post ["/disable"] = parameters =>
            {
                var token =
                    Request.Headers [_authorization].FirstOrDefault ();

                var jwt = Jwt.ToObject(token.Replace("\"", string.Empty));
                Guid domainKey = Guid.Parse(jwt.Payload.Audience);

                var key = Guid.Parse(jwt.Payload.JwtTokenId);

                return Response.AsJson (repository.Disable (key, domainKey),
                                        HttpStatusCode.OK);
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
                    var e = new Error ();
                    e.Detail = "Authentication failed. User not found.";
                    e.Status =  (int)HttpStatusCode.Forbidden;
                    e.Title = "Unauthorized";
                    e.Source = new Source("/authenticate");

                    return Response.AsJson (e,
                        HttpStatusCode.Forbidden);
                }

                var header = new Header();
                header.Algorithm = "HS256";
                header.Type = "JWT";

                var payload = new Payload();
                payload.Issuer = "autheticate.technology";
                payload.Audience = domainKey.ToString();
                payload.JwtTokenId = user.AuthToken.ToString();
                payload.Username = user.UserName;
                payload.Email = user.Email;

                var jwt = new Jwt(header,payload);

                return Response.AsJson (Jwt.ToToken(jwt, 
                                                    user.Key),HttpStatusCode.OK);
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
                     
                        var e = new Error ();
                        e.Detail = "Email is already used.";
                        e.Status =  (int)HttpStatusCode.UnprocessableEntity;
                        e.Title = "Duplicate";
                        e.Source = new Source("/user");

                    
                        return Response.AsJson (e,
                            HttpStatusCode.UnprocessableEntity);
                    }

                    user = repository.CreateUser (domainKey,
                        Request.Form ["Username"],
                        Request.Form ["Secret"],
                        Request.Form ["Email"]);
                }
                catch (Exception exc)
                {
                    var e = new Error ();
                    e.Detail = "Username already exists for this application.";
                    e.Status =  (int)HttpStatusCode.UnprocessableEntity;
                    e.Title = "Duplicate";
                    e.Source = new Source("/user");

                    return Response.AsJson (e,
                        HttpStatusCode.UnprocessableEntity);
                }

                return Response.AsJson (true, HttpStatusCode.OK);
            };
        }
    }
}

