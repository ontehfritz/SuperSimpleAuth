using System;
using System.Collections.Generic;

namespace SSAManager
{
    public class DomainUserModel
    {
        public Manager Manager { get; set; }
        public Domain Domain { get; set; }
        public User User { get; set; }
        public List<Role> Roles { get; set; }
        public List<string> NewRoles { get; set; }
        public List<string> Claims { get; set; }
        public string Debug { get; set; }
    }
}

