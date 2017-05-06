namespace SuperSimple.Auth
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Api.Token;
    using System.Net.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;

    public class SuperSimpleAuth
    {
        private const string ServiceUri = "https://api.authenticate.technology";
        private const string _headerDomainKey = "Ssa-Domain-Key";
        private const string _headerDomain = "Ssa-Domain";
        private const string _authorization = "Authorization";
        private HttpClient _client; 

        private string _uri;
        private Guid _domainKey { get; set; }

        public SuperSimpleAuth (string domainKey,
                                string uri = ServiceUri)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(uri);
            _uri = uri;

            Guid key;

            if (Guid.TryParse (domainKey, out key))
            {
                _domainKey = key;
                _client.DefaultRequestHeaders.Add(_headerDomainKey, 
                                                   _domainKey.ToString ());
            }
            else
            {
                throw new ArgumentException 
                ("Domain key is in an incorrect format.");
            }
        }

        /// <summary>
        /// /*Forgot the specified email.*/
        /// </summary>
        /// <param name="email">Email.</param>
        public async Task<string> Forgot (string email)
        {
            if (string.IsNullOrEmpty (email))
            {
                throw
                new ArgumentException ("email cannot be null or empty string",
                                       "email");
            }

            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("Email", email)
            });

            var result = await _client.PostAsync ("/forgot",
                                                 content);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                return(JsonConvert.DeserializeObject<string>(
                    resultContent));
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }

        public async Task<User> ChangeUserName (User user, string newUserName)
        {
            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("NewUserName", newUserName)
            });

            _client.DefaultRequestHeaders.Authorization =
                       new AuthenticationHeaderValue (user.Jwt);

            var result = await _client.PostAsync ("/username", content);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync ();
                user = JwtToUser (resultContent.Replace ("\"", string.Empty));
                return user;
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }

        public async Task<User> ChangeEmail (User user, string newEmail)
        {
            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("NewEmail", newEmail)
            });

            _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue (user.Jwt);

            var result = await _client.PostAsync ("/email", content);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync ();
                user = JwtToUser (resultContent.Replace ("\"", string.Empty));
                return user;
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }

        /// <summary>
        /// End the specified authToken.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        public async Task<bool> ChangePassword (User user, string newPassword)
        {
            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("NewPassword", newPassword)
            });

            _client.DefaultRequestHeaders.Authorization =
                       new AuthenticationHeaderValue (user.Jwt);

            var result = await _client.PostAsync ("/password", content);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                return(JsonConvert.DeserializeObject<bool>(
                    resultContent.Replace("\"", string.Empty)));
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }

        /// <summary>
        /// End the specified authToken.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        public async Task<bool> End (User user)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue (user.Jwt);

            var result = await _client.PostAsync ("/end", null);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                return(JsonConvert.DeserializeObject<bool>(
                    resultContent.Replace("\"", string.Empty)));
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }
        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userName">User name.</param>
        /// <param name="secret">Secret.</param>
        /// <param name="email">Email.</param>
        public async Task<bool> CreateUser (string userName, string secret, 
                                            string email = null)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Username", userName),
                new KeyValuePair<string, string>("Secret", secret),
                new KeyValuePair<string, string>("Email", email)

            });

            var result = await _client.PostAsync("/user", 
                                                 content);
            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                return(JsonConvert.DeserializeObject<bool>(
                    resultContent.Replace("\"", string.Empty)));
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }
        /// <summary>
        /// Authenticate the specified username, secret and IP.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="secret">Secret.</param>
        /// <param name="IP">I.</param>
        public async Task<User> Authenticate (string userName, string secret,
                                  string IP = null)
        {
            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("Username", userName),
                new KeyValuePair<string, string>("Secret", secret),
                new KeyValuePair<string, string>("IP",IP)
            });

            var result = await _client.PostAsync ("/authenticate",
                                                 content);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync ();
                var user = JwtToUser (resultContent.Replace ("\"", string.Empty));
                return user;
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }

        private User JwtToUser(string token)
        {
            var jwt = Jwt.ToObject(token);
            var user = new User();
            user.AuthToken = Guid.Parse(jwt.Payload.JwtTokenId);
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
        public async Task<bool> Validate (User user, string IP = null)
        {
            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("IP",IP)
            });

            _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue (user.Jwt);

            var result = await _client.PostAsync ("/validate",
                                                  content);

            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                return(JsonConvert.DeserializeObject<bool>(
                    resultContent));
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }
        }

        public User Validate (string token, string IP = null)
        {
            var user = JwtToUser(token);

            if(Validate(user, IP).Result)
            {
                return user;
            }

            return null;
        }

        public async Task<User> Validate (Guid authToken, string IP = null)
        {
            var content = new FormUrlEncodedContent (new []
            {
                new KeyValuePair<string, string>("AuthToken",
                                                 authToken.ToString ()),
                new KeyValuePair<string, string>("IP",IP)
            });


            var result = await _client.PostAsync ("/validateauthtoken",
                                                  content);
            if(result.IsSuccessStatusCode)
            {
                var resultContent = await result.Content.ReadAsStringAsync ();
                var user = JsonConvert.DeserializeObject<User> (resultContent);
                return user;
            }
            else
            {
                var errorJson = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(
                    errorJson);

                throw new Exception(error.Detail);
            }

            //User user = null;

            //using (WebClient client = new WebClient ())
            //{
            //    var reqparm =
            //        new System.Collections.Specialized.NameValueCollection ();

            //    client.Headers [_headerDomainKey] = this._domainKey.ToString ();

            //    reqparm.Add ("AuthToken", );

            //    if (IP != null)
            //    {
            //        reqparm.Add ("IP", IP);
            //    }

            //    string responsebody = "";

            //    try
            //    {
            //        ServicePointManager.ServerCertificateValidationCallback =
            //            delegate { return true; };

            //        var responsebytes =
            //            client.UploadValues (string.Format ("{0}/validateauthtoken",
            //                                                _uri),
            //                                 "Post", reqparm);

            //        responsebody = Encoding.UTF8.GetString (responsebytes);

            //    }
            //    catch (WebException e)
            //    {
            //        HandleWebException (e);
            //    }

            //    user = JsonConvert.DeserializeObject<User> (responsebody);

            //}

            //return user;
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

            var error = new Error ();

            try
            {
                var response = ReadResponse (exception.Response);
                error =
                    JsonConvert.DeserializeObject<Error> (response);
            }
            catch (Exception e)
            {
                throw exception;
            }

            switch (error.Title)
            {
                case "Duplicate":
                    throw new DuplicateException (error.Detail);
                case "Invalid":
                    throw new InvalidException (error.Detail);
                case "Unauthorized":
                    throw new UnauthorizedException (error.Detail);
                default:
                    throw exception;
            }
        }
    }
}