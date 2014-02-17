using System;
using System.Collections.Generic;

namespace SSAManager
{
    public class PageModel
    {
        public Manager Manager { get; set; }
        public string Title { get; set; }
        public List<string> Messages { get; set; }
        public List<Error> Errors { get; set; }

        public PageModel()
        {
            Messages = new List<string> ();
            Errors = new List<Error> ();
        }
    }
}

