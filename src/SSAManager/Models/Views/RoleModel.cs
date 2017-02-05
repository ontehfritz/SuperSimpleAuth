using System;
using System.Collections;
using System.Collections.Generic;
using SuperSimple.Auth.Api;

namespace SSAManager
{
    public class RoleModel : PageModel
    {
        public Manager Manager { get; set; }
        public Domain Domain { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public Role Role { get; set; }
        public List<string> RoleUsers { get; set; }
        public List<string> uRoleUsers { get; set; }
        public List<User> Users { get; set; }
    }
}

