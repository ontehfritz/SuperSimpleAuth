namespace NancyStatelessJwt
{
    using System.Collections.Generic;
    using Nancy.Validation;

    public static class Error
    {
        /// <summary>
        /// Used to convert fluent validation errors from nancyfx to a simple error object
        /// </summary>
        /// <returns>The validation errors.</returns>
        public static IEnumerable<string> GetValidationErrors(ModelValidationResult result)
        {
            var errors = new List<string>();

            foreach(var e in result.Errors)
            {
                foreach(var message in e.Value)
                {
                    errors.Add(message.ErrorMessage);
                }
            }
            
            return errors;
        }
    }
}

