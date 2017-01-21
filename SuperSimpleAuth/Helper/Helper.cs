using System;
using Nancy;
using System.Linq;

namespace SuperSimple.Auth.Api
{
    public static class Helper
    {
        private const string _headerDomainKey = "Ssa-Domain-Key";
        private const string _headerDomain = "Ssa-Domain";
        /// <summary>
        /// Checks the request header if the proper keys exist and are in correct format
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        public static ErrorMessage VerifyRequest(Request request,IRepository repository)
        {
            ErrorMessage message = null;
            Guid key = new Guid();

            string appKey = request.Headers[_headerDomainKey].FirstOrDefault();
            string app = request.Headers[_headerDomain].FirstOrDefault();

            if(appKey != null && app != null)
            {               
                try
                {
                    key = Guid.Parse(request.Headers[_headerDomainKey].FirstOrDefault());

                    if(!repository.ValidateDomainKey(app,key))
                    {
                        message = new ErrorMessage{
                            Status = "InvalidKey",
                            Message = "Key does not exist or is invalid."
                        };

                        return message;
                    }
                }
                catch(FormatException e)
                {
                    message = new ErrorMessage {
                        Status = "InvalidKey",
                        Message = "Domain key is not in correct format."
                    };

                    return message;
                }
            }
            else
            {
                message = new ErrorMessage{
                    Status = "InvalidKey",
                    Message =  "Domain Key and/or Domain Name not provided in request header"
                };

                return message;
            }

            return null;
       }
    }
}

