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
        private const string URI = "http://127.0.0.1:8080";
        private Guid ApplicationKey { get; set; }
        private string Name;

        public SuperSimpleAuth (string name, string applicationKey)
        {
            this.Name = name;

            Guid key; 

            if (Guid.TryParse (applicationKey, out key)) {
                ApplicationKey = key;
            } else {
                throw new Exception ("Application key is in an incorrect format.");
            }
        }

        public bool End(Guid authToken)
        {
            bool end = false;
            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers["ssa_app_key"] = this.ApplicationKey.ToString();
                client.Headers["ssa_app"] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());

                string responsebody = "";

                try
                {
                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/end",URI), 
                                            "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) {
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

                if (IP != null) {
                    reqparm.Add ("IP", IP);
                }

                client.Headers["ssa_app_key"] = this.ApplicationKey.ToString();
                //client.Headers["ssa_app_key"] = "hjkjkj";
                client.Headers["ssa_app"] = this.Name;

                string responsebody = "";

                try
                {
                   
                    byte[] responsebytes = client.UploadValues(string.Format("{0}/authenticate", URI), 
                                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) {
                    HandleWebException (e);
                }
               
                user = JsonConvert.DeserializeObject<User>(responsebody);
            }


            return user;
        }

        public bool Authorize(ISsaUser user, string action)
        {
            bool isAuthorized = false;
            return isAuthorized;
        }

        public User Validate(Guid authToken, string IP = null)
        {
            User user = null;
            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                client.Headers["ssa_app_key"] = this.ApplicationKey.ToString();
                client.Headers["ssa_app"] = this.Name;

                reqparm.Add("AuthToken", authToken.ToString());

                if (IP != null) {
                    reqparm.Add ("IP", IP);
                }
              
                string responsebody = "";

                try
                {
                    byte[] responsebytes = 
                        client.UploadValues(string.Format("{0}/validate", URI), 
                                                               "Post", reqparm);

                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) {
                    HandleWebException (e);
                }

                user = JsonConvert.DeserializeObject<User>(responsebody);
            }

            return user;
        }

        public User CreateUser(string userName, string secret)
        {
            User user = null;

            using(WebClient client = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = 
                    new System.Collections.Specialized.NameValueCollection();

                reqparm.Add("Username", userName);
                reqparm.Add("Secret", secret);

                client.Headers["ssa_app_key"] = this.ApplicationKey.ToString();
                client.Headers["ssa_app"] = this.Name;

                string responsebody = "";

                try
                {
                    byte[] responsebytes = client.UploadValues(string.Format("{0}/user",URI), 
                                                           "Post", reqparm);
                   
                    responsebody = Encoding.UTF8.GetString(responsebytes);

                }
                catch(WebException e) {
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
            if (exception.Status != WebExceptionStatus.ProtocolError) {
                throw exception;
            }

            ErrorMessage error = new ErrorMessage();

            try{
                string response = ReadResponse (exception.Response);
                error = 
                    JsonConvert.DeserializeObject<ErrorMessage>(response);
            }
            catch(Exception e){
                throw exception;
            }

            switch (error.Status) {
                case "DuplicateUser":
                    throw new DuplicateUserException (error.Message);
                case "InvalidKey":
                    throw new InvalidKeyException(error.Message);
                case "AuthenticationFailed":
                    throw new AuthenticationFailedException(error.Message);
                case "InvalidToken":
                    throw new InvalidTokenException(error.Message);
                default:
                    throw exception;
            }
        }
    }
}