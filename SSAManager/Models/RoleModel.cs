using System;
using System.Collections;
using System.Collections.Generic;

namespace SSAManager
{
    public class RoleModel
    {
        public Manager Manager { get; set; }
        public App App { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public Role Role { get; set; }
        public List<string> RoleUsers { get; set; }
        public List<string> uRoleUsers { get; set; }
        public List<User> Users { get; set; }
    }
}

