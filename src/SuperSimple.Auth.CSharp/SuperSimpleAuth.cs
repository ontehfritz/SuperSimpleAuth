namespace SuperSimple.Auth
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Api.Token;

    public class SuperSimpleAuth
    {
        private const string _headerDomainKey = "Ssa-Domain-Key";
        private const string _headerDomain = "Ssa-Domain";
        private const string _authorization = "Authorization";

        private string _uri;
        private Guid _domainKey { get; set; }
        private string _name;

        public SuperSimpleAuth (string name,
                                string domainKey,
                                string uri = "https://api.authenticate.technology")
        {
            _name = name;
            _uri = uri;

            Guid key;

            if (Guid.TryParse (domainKey, out key))
            {
                _domainKey = key;
            }
            else
            {
                throw new Exception ("Domain key is in an incorrect format.");
            }
        }

        /// <summary>
        /// /*Forgot the specified email.*/
        /// </summary>
        /// <param name="email">Email.</param>
        public string Forgot (string email)
        {
            if (string.IsNullOrEmpty (email))
            {
                throw new ArgumentException ("email cannot be null or empty string", "email");
            }

            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                reqparm.Add ("Email", email);

                client.Headers [_headerDomainKey] = this._domainKey.ToString ();
                client.Headers [_headerDomain] = this._name;

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate
                    {
                        return true;
                    };

                    byte [] responsebytes = 
                        client.UploadValues (string.Format ("{0}/forgot", _uri),
                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                return JsonConvert.DeserializeObject<string> (responsebody);
            }
        }

        public User ChangeUserName (User user, string newUserName)
        {
            bool end = false;

            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                client.Headers [_authorization] = user.Jwt;

                reqparm.Add ("NewUserName", newUserName);

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };

                    byte [] responsebytes =
                        client.UploadValues (string.Format ("{0}/username", _uri),
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                user = JwtToUser(responsebody.Replace("\"",string.Empty));
                return user;
            }

            throw new Exception("Cannot update user name");
        }

        public User ChangeEmail (User user, string newEmail)
        {
            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                client.Headers [_authorization] = user.Jwt;
                reqparm.Add ("NewEmail", newEmail);

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };

                    var responsebytes =
                        client.UploadValues (string.Format ("{0}/email", _uri),
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                user = JwtToUser(responsebody.Replace("\"",string.Empty));
                return user;
            }

            throw new Exception("Cannot update user email");
        }

        /// <summary>
        /// End the specified authToken.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        public bool ChangePassword (Guid authToken, string newPassword)
        {
            bool end = false;

            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                client.Headers [_headerDomainKey] = this._domainKey.ToString ();
                client.Headers [_headerDomain] = this._name;

                reqparm.Add ("AuthToken", authToken.ToString ());
                reqparm.Add ("NewPassword", newPassword);

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                    byte [] responsebytes =
                        client.UploadValues (string.Format ("{0}/password", _uri),
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                end = JsonConvert.DeserializeObject<bool> (responsebody);
            }

            return end;
        }

        /// <summary>
        /// End the specified authToken.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        public bool End (Guid authToken)
        {
            bool end = false;

            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                client.Headers [_headerDomainKey] = this._domainKey.ToString ();
                client.Headers [_headerDomain] = this._name;

                reqparm.Add ("AuthToken", authToken.ToString ());

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                    byte [] responsebytes =
                        client.UploadValues (string.Format ("{0}/end", _uri),
                                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                end = JsonConvert.DeserializeObject<bool> (responsebody);
            }

            return end;
        }

        /// <summary>
        /// Authenticate the specified username, secret and IP.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="secret">Secret.</param>
        /// <param name="IP">I.</param>
        public User Authenticate (string username, string secret, 
                                  string IP = null)
        {
            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                reqparm.Add ("Username", username);
                reqparm.Add ("Secret", secret);

                if (IP != null)
                {
                    reqparm.Add ("IP", IP);
                }

                client.Headers [_headerDomainKey] = this._domainKey.ToString();

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                    var responsebytes = 
                        client.UploadValues (
                            string.Format ("{0}/authenticate", _uri),
                                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);
                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                var user = JwtToUser(responsebody.Replace("\"",string.Empty));
                return user;
            }
        }

        private User JwtToUser(string token)
        {
            var jwt = Jwt.ToObject(token);
            var user = new User();
            user.Id = Guid.Parse(jwt.Payload.JwtTokenId);
            user.Jwt = token;
            user.Email = jwt.Payload.Email;
            user.UserName = jwt.Payload.Username;

            foreach(var role in jwt.Payload.Roles)
            {
                var r = new Role();

                r.Name = role.Name;
                foreach(var permission in role.Permissions)
                {
                    r.Permissions.Add(permission);
                }

                user.Roles.Add(r);
            }

            return user;
        }

        /// <summary>
        /// Validate the specified authToken and IP.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        /// <param name="IP">I.</param>
        public bool Validate (User user, string IP = null)
        {
            bool valid = false;
            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                client.Headers [_authorization] = user.Jwt;
              
                if (IP != null)
                {
                    reqparm.Add ("IP", IP);
                }

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                    byte [] responsebytes =
                        client.UploadValues (string.Format ("{0}/validate", _uri),
                                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                valid = JsonConvert.DeserializeObject<bool> 
                                   (responsebody.Replace("\"", string.Empty));
            }

            return valid;
        }

        public User Validate (string token, string IP = null)
        {
            var user = JwtToUser(token);

            if(Validate(user, IP))
            {
                return user;
            }

            return null;
        }

        public User Validate (Guid authToken, string IP = null)
        {
            User user = null;

            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                client.Headers [_headerDomainKey] = this._domainKey.ToString ();
                client.Headers [_headerDomain] = this._name;

                reqparm.Add ("AuthToken", authToken.ToString ());

                if (IP != null)
                {
                    reqparm.Add ("IP", IP);
                }

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                    byte [] responsebytes =
                        client.UploadValues (string.Format ("{0}/validate", _uri),
                                             "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                user = JsonConvert.DeserializeObject<User> (responsebody);
            }

            return user;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userName">User name.</param>
        /// <param name="secret">Secret.</param>
        /// <param name="email">Email.</param>
        public bool CreateUser (string userName, string secret, string email = null)
        {
            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();

                reqparm.Add ("Username", userName);
                reqparm.Add ("Secret", secret);

                if (email != null)
                {
                    reqparm.Add ("Email", email);
                }

                client.Headers [_headerDomainKey] = this._domainKey.ToString ();
                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                     var responsebytes = 
                        client.UploadValues (string.Format ("{0}/user", _uri),
                        "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                return(JsonConvert.DeserializeObject<bool>(
                    responsebody.Replace("\"", string.Empty)));
            }
        }

        public bool Disable (User user)
        {
            bool disabled = false;

            using (WebClient client = new WebClient ())
            {
                var reqparm =
                    new System.Collections.Specialized.NameValueCollection ();
                
                client.Headers [_authorization] = user.Jwt;

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = 
                        delegate { return true; };
                    var responsebytes =
                        client.UploadValues (string.Format ("{0}/disable", _uri),
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                }
                catch (WebException e)
                {
                    HandleWebException (e);
                }

                disabled = JsonConvert.DeserializeObject<bool> (responsebody);
            }

            return disabled;
        }

        /// <summary>
        /// Reads the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="response">Response.</param>
        private string ReadResponse (WebResponse response)
        {
            var responseText = string.Empty;

            if (response != null)
            {
                var responseStream = response.GetResponseStream ();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader (responseStream))
                    {
                        responseText = reader.ReadToEnd ();
                    }
                }
            }

            return responseText;
        }
        
         


        /// <summary>
        /// Handles the web exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        private void HandleWebException (WebException exception)
        {
            if (exception.Status != WebExceptionStatus.ProtocolError)
            {
                throw exception;
            }

            var error = new ErrorMessage ();

            try
            {
                string response = ReadResponse (exception.Response);
                error =
                    JsonConvert.DeserializeObject<ErrorMessage> (response);
            }
            catch (Exception e)
            {
                throw exception;
            }

            switch (error.Status)
            {
            case "DuplicateUser":
                throw new DuplicateUserException (error.Message);
            case "InvalidKey":
                throw new InvalidKeyException (error.Message);
            case "AuthenticationFailed":
                throw new AuthenticationFailedException (error.Message);
            case "InvalidToken":
                throw new InvalidTokenException (error.Message);
            case "IpNotAllowed":
                throw new InvalidIpException (error.Message);
            default:
                throw exception;
            }
        }
    }
}