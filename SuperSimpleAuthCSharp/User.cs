using System;
using System.Collections.Generic;

namespace SuperSimple.Auth
{
    public class User : ISsaUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set;}
        public Guid AuthToken { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public bool InRole(string role)
        {
            if (Roles == null) {
                return false;
            }

            foreach(string r in Roles)
            {
                if (r == role) {
                    return true;
                }
            }

            return false;
        }

        public bool HasClaim(string claim)
        {
            if (Claims == null) {
                return false;
            }

            foreach(string c in Claims)
            {
                if (c == claim) {
                    return true;
                }
            }

            return false;
        }
    }
}

