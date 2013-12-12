using System;
using System.Collections.Generic;
using System.Configuration;
using SSAManager;
using Mono.Security.X509;
using System.Dynamic;

namespace SSAManager
{
    public class ManageModel : IPageModel
    {
        public Manager Manager { get; set; }
        public string Title { get; set;  }
        public Domain[] Domains { get; set; }
        public List<string> Messages { get; set; }
        public List<Error> Errors { get; set; }
    }
}

