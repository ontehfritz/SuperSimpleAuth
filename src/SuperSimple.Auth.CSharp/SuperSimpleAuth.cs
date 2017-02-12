namespace SuperSimple.Auth
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;

    public class SuperSimpleAuth
    {
//        private const string _headerDomainKey = "SSA_DOMAIN_KEY";
//        private const string _headerDomain = "SSA_DOMAIN";
        private const string _headerDomainKey = "Ssa-Domain-Key";
        private const string _headerDomain = "Ssa-Domain";

        private string URI;
        private Guid DomainKey { get; set; }
        private string Name;
 
        public SuperSimpleAuth (string name, 
                                string domainKey, 
                                string uri = "https://api.authenticate.technology")
        {
            this.Name = name;
            this.URI = uri;

            Guid key; 

            if (Guid.TryParse (domainKey, out key)) 
            {
                DomainKey = key;
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
        public string Forgot(string email)
        {
            if(email == null || email == "")
            {
                throw new ArgumentException ("email cannot be null or empty string", "email");
            }

            using (WebClient client = new WebClient ()) {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection ();
             
                reqparm.Add ("Email", email);

               
                client.Headers [_headerDomainKey] = this.DomainKey.ToString ();
                client.Headers [_headerDomain] = this.Name;

                string responsebody = "";

                try {
                    ServicePointManager.ServerCertificateValidationCallback = delegate {
                        return true;
                    };

                    byte[] responsebytes = client.UploadValues (string.Format ("{0}/forgot", URI), 
                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString (responsebytes);

                } catch (WebException e) {
                    HandleWebException (e);
                }

                return JsonConvert.DeserializeObject<string> (responsebody);
            }
        }
       
        public bool ChangeUserName(Guid authToken, string newUserName)
        {
            bool end = false;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());
                reqparm.Add("NewUserName", newUserName);

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/username",URI), 
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }

                end = JsonConvert.DeserializeObject<bool>(responsebody);
            }

            return end;
        }

        public bool ChangeEmail(Guid authToken, string newEmail)
        {
            bool end = false;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());
                reqparm.Add("NewEmail", newEmail);

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/email",URI), 
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }

                end = JsonConvert.DeserializeObject<bool>(responsebody);
            }

            return end;
        }

        /// <summary>
        /// End the specified authToken.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        public bool ChangePassword(Guid authToken, string newPassword)
        {
            bool end = false;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());
                reqparm.Add("NewPassword", newPassword);

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/password",URI), 
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }

                end = JsonConvert.DeserializeObject<bool>(responsebody);
            }

            return end;
        }
       
        /// <summary>
        /// End the specified authToken.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        public bool End(Guid authToken)
        {
            bool end = false;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/end",URI), 
                                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }

                end = JsonConvert.DeserializeObject<bool>(responsebody);
            }

            return end;
        }

        /// <summary>
        /// Authenticate the specified username, secret and IP.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="secret">Secret.</param>
        /// <param name="IP">I.</param>
        public User Authenticate(string username, string secret, string IP = null)
        {
            User user = null;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                reqparm.Add("Username", username);
                reqparm.Add("Secret", secret);

                if (IP != null) 
                {
                    reqparm.Add ("IP", IP);
                }

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    byte[] responsebytes = client.UploadValues(string.Format("{0}/authenticate", URI), 
                                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);
                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }
               
                user = JsonConvert.DeserializeObject<User>(responsebody);
            }

            return user;
        }

        /// <summary>
        /// Validate the specified authToken and IP.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        /// <param name="IP">I.</param>
        public User Validate(Guid authToken, string IP = null)
        {
            User user = null;
            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());

                if (IP != null) 
                {
                    reqparm.Add ("IP", IP);
                }
              
                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/validate", URI), 
                                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }

                user = JsonConvert.DeserializeObject<User>(responsebody);
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
        public User CreateUser(string userName, string secret, string email = null)
        {
            User user = null;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                reqparm.Add("Username", userName);
                reqparm.Add("Secret", secret);

                if (email != null) 
                {
                    reqparm.Add("Email", email);
                }

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    byte[] responsebytes = client.UploadValues(string.Format("{0}/user",URI), 
                        "Post", reqparm);
                   
                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }
              
                user = JsonConvert.DeserializeObject<User>(responsebody);
            }

            return user;
        }

        public bool Disable(Guid authToken)
        {
            bool disabled = false;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers[_headerDomainKey] = this.DomainKey.ToString();
                client.Headers[_headerDomain] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());

                string responsebody = "";

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/disable",URI), 
                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) 
                {
                    HandleWebException (e);
                }

                disabled = JsonConvert.DeserializeObject<bool>(responsebody);
            }

            return disabled;
        }

        /// <summary>
        /// Reads the response.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="response">Response.</param>
        private string ReadResponse(WebResponse response)
        {
            string responseText = "";

            if (response != null)
            {
                var responseStream = response.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
            }

            return responseText;
        }

        /// <summary>
        /// Handles the web exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        private void HandleWebException(WebException exception)
        {
            if (exception.Status != WebExceptionStatus.ProtocolError) 
            {
                throw exception;
            }

            ErrorMessage error = new ErrorMessage();

            try
            {
                string response = ReadResponse (exception.Response);
                error = 
                    JsonConvert.DeserializeObject<ErrorMessage>(response);
            }
            catch(Exception e)
            {
                throw exception;
            }

            switch (error.Status) 
            {
                case "DuplicateUser":
                    throw new DuplicateUserException (error.Message);
                case "InvalidKey":
                    throw new InvalidKeyException(error.Message);
                case "AuthenticationFailed":
                    throw new AuthenticationFailedException(error.Message);
                case "InvalidToken":
                    throw new InvalidTokenException(error.Message);
                case "IpNotAllowed":
                    throw new InvalidIpException(error.Message);
                default:
                    throw exception;
            }
        }
    }
}