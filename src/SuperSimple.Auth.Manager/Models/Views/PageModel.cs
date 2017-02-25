namespace SuperSimple.Auth.Manager
{
    using System.Collections.Generic;
    using Api;

    public class PageModel
    {
        public IUser Manager            { get; set; }
        public string Title             { get; set; }
        public List<string> Messages    { get; set; }
        public List<Error> Errors       { get; set; }

        public PageModel()
        {
            Messages = new List<string> ();
            Errors = new List<Error> ();
        }
    }
}

