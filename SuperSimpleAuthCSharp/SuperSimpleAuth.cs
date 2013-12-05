using System;
using System.Net.Security;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Collections;

namespace SuperSimple.Auth
{
    public class SuperSimpleAuth
    {
        private string URI;
        private Guid DomainKey { get; set; }
        private string Name;
 
        public SuperSimpleAuth (string name, 
            string domainKey, string uri = "http://api.supersimpleauth.com")
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


        public bool ChangeUserName(Guid authToken, string newUserName)
        {
            bool end = false;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                client.Headers["ssa_domain"] = this.Name;

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

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                client.Headers["ssa_domain"] = this.Name;

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

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                client.Headers["ssa_domain"] = this.Name;

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

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                client.Headers["ssa_domain"] = this.Name;

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

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                //client.Headers["ssa_app_key"] = "hjkjkj";
                client.Headers["ssa_domain"] = this.Name;

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

        public User Validate(Guid authToken, string IP = null)
        {
            User user = null;
            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                client.Headers["ssa_domain"] = this.Name;

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

                client.Headers["ssa_domain_key"] = this.DomainKey.ToString();
                client.Headers["ssa_domain"] = this.Name;

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