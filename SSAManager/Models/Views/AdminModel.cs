using System;

namespace SSAManager
{
    public class AdminModel : PageModel
    {
        public string Email     { get; set; }        
        public Domain Domain    { get; set; }

        public AdminModel () : base(){}
    }
}

