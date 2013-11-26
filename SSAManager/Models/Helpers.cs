using System;
using System.Collections.Generic;
using Nancy.Validation;

namespace SSAManager
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
                foreach(var member in e.MemberNames)
                {
                    Error error = new Error();
                    error.Name = member;
                    error.Message = e.GetMessage(member);
                    errors.Add(error);
                }
            }
            
            return errors;
        }
    }
}

