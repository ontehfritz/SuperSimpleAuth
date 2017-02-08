using System;
using System.Collections.Generic;
using Nancy.Validation;
using System.Security.Cryptography;

namespace SuperSimple.Auth.Manager
{
    public static class Helpers
    {
        /// <summary>
        /// Values the specified array.
        /// </summary>
        /// <param name="array">Array.</param>
        public static string Values(IEnumerable<string> array)
        {
            if (array != null) {
                return string.Join (",", array);
            }

            return "";
        }

        /// <summary>
        /// Used to convert fluent validation errors from nancyfx to a simple error object
        /// </summary>
        /// <returns>The validation errors.</returns>
        public static List<Error> GetValidationErrors(ModelValidationResult result)
        {
            List<Error> errors = new List<Error>();

            foreach(var e in result.Errors)
            {
                foreach(var message in e.Value)
                {
                    Error error = new Error();
                    error.Name = e.Key;
                    error.Message = message.ErrorMessage;
                    errors.Add(error);
                }
            }
            
            return errors;
        }

        public static string Hash(string Salt, string Password) 
        {
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(Password,
                System.Text.Encoding.Default.GetBytes(Salt), 10000);

            return Convert.ToBase64String(hash.GetBytes(25));
        }

    }
}

