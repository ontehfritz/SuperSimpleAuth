using System.Collections.Generic;

namespace SSAManager
{
    using System;

    public class LogonModel: IPageModel
    {
        public List<string> Messages { get; set; }
        public List<Error> Errors { get; set; }
        public string Title { get; set; }
        public Manager Manager { get; set; }
        public string Username { get; set ; }
        public string Secret { get; set; }

        public LogonModel()
        {
            Messages = new List<string> ();
            Errors = new List<Error> ();
        }


    }
}

