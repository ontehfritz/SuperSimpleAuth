//namespace SuperSimple.Auth.Api
//{
//    using System;
//    using Nancy;
//    using System.Linq;
//    using System.Diagnostics.Contracts;
//    using Repository;

//    public static class Errors
//    {
//        private const string _headerDomainKey = "Ssa-Domain-Key";
//        private const string _headerDomain = "Ssa-Domain";
//        /// <summary>
//        /// Checks the request header if the proper keys exist and are in correct format
//        /// </summary>
//        /// <returns>The request.</returns>
//        /// <param name="request">Request.</param>
//        public static ErrorMessage VerifyRequest(Request request,
//                                                 IApiRepository repository)
//        {
//            Contract.Ensures (Contract.Result<ErrorMessage> () != null);
//            Error message = null;
//            var key = new Guid();

//            var appKey = request.Headers[_headerDomainKey].FirstOrDefault();
//            var app = request.Headers[_headerDomain].FirstOrDefault();

//            if(appKey != null && app != null)
//            {               
//                try
//                {
//                    key = Guid.Parse(request.Headers[_headerDomainKey].FirstOrDefault());

//                    if(!repository.ValidateDomainKey(app,key))
//                    {
//                        message = new Error{
//                            Status = "InvalidKey",
//                            Message = "Key does not exist or is invalid."
//                        };

//                        return message;
//                    }
//                }
//                catch(FormatException e)
//                {
//                    message = new ErrorMessage {
//                        Status = "InvalidKey",
//                        Message = "Domain key is not in correct format."
//                    };

//                    return message;
//                }
//            }
//            else
//            {
//                message = new ErrorMessage{
//                    Status = "InvalidKey",
//                    Message =  "Domain Key and/or Domain Name not provided in request header"
//                };

//                return message;
//            }

//            return null;
//       }
//    }
//}

