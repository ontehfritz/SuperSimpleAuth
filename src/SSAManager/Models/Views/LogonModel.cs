using System.Collections.Generic;

namespace SSAManager
{
    using System;

    public class LogonModel: PageModel
    {
       
        public string Username { get; set ; }
        public string Secret { get; set; }

        public LogonModel() : base()
        {
           
        }
    }
}

