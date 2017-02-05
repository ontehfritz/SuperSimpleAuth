namespace SSAManager
{
    using System;
    using System.Collections.Generic;
    using SuperSimple.Auth.Api;

    public class DomainUserModel: PageModel
    {

        public Domain Domain { get; set; }
        public User User { get; set; }
        public List<Role> Roles { get; set; }
        public List<string> NewRoles { get; set; }
        public List<string> Claims { get; set; }
        public bool Enabled { get; set; }

       
        public DomainUserModel() : base()
        {
            Roles = new List<Role> ();
            NewRoles = new List<string> ();
            Claims = new List<string> ();
        }
    }
}

