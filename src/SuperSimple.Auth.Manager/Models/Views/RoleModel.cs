namespace SuperSimple.Auth.Manager
{
    using System.Collections.Generic;
    using SuperSimple.Auth.Api;
    using SuperSimple.Auth.Api.Repository;
    using SuperSimple.Auth.Manager.Repository;

    public class RoleModel : PageModel
    {
        public Domain Domain { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public Role Role { get; set; }
        public List<string> RoleUsers { get; set; }
        public List<string> uRoleUsers { get; set; }
        public List<User> Users { get; set; }
    }
}

