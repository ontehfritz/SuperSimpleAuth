using System;
using System.Collections.Generic;
using System.Configuration;
using SSAManager;
using Mono.Security.X509;
using System.Dynamic;

namespace SSAManager
{
    public class ManageModel : PageModel
    {
        public Domain[] Domains { get; set; }


        public ManageModel() : base()
        {

        }
    }
}

