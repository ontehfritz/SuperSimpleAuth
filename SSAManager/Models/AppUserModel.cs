using System;
using System.Collections.Generic;

namespace SSAManager
{
    public class AppUserModel
    {
        public Manager Manager { get; set; }
        public App App { get; set; }
        public User User { get; set; }
        public List<Role> Roles { get; set; }
        public List<string> NewRoles { get; set; }
        public List<string> Claims { get; set; }
        public string Debug { get; set; }
    }
}

