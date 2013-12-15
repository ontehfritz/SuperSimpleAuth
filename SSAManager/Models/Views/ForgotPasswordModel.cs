using System;
using System.Collections.Generic;

namespace SSAManager
{
    public class ForgotPasswordModel : IPageModel
    {
        public Manager Manager { get; set; }
        public List<string> Messages { get; set; }
        public List<Error> Errors { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }

        public ForgotPasswordModel()
        {
            Messages = new List<string> ();
            Errors = new List<Error> ();
        }

    }
}

