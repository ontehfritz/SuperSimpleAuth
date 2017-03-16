using System;
using System.Collections.Generic;

namespace SuperSimple.Auth
{
    public class Role
    {
        public string Name                  { get; set; }
        public List<string> Permissions     { get; set; }

        public Role()
        {
            Permissions = new List<string>();
        }
    }
    public class User
    {
        public string UserName  { get; set; }
        public string Email     { get; set; }
        public Guid AuthToken   { get; set; }
        public string Jwt       { get; set; }
        public List<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }

        public bool InRole(string role)
        {
            if (Roles == null) 
            {
                return false;
            }

            foreach(var r in Roles)
            {
                if (r.Name == role) 
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasPermission(string role, string permission)
        {
            if (Roles == null) 
            {
                return false;
            }

            foreach(var r in Roles)
            {
                if (r.Name == role) 
                {
                    return true;
                }
            }

            return false;
        }
    }
}

