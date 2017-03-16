using System;
using System.Collections.Generic;
using Nancy.Security;
using SuperSimple.Auth;

namespace NancyStatelessJwt
{
    public class NancyUserIdentity : IUserIdentity
    {
        public Guid AuthToken { get; set; }
        public string UserName { get; set; }
        public string Jwt {get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Claims { get; set; }

        public NancyUserIdentity(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            AuthToken = user.AuthToken;
            Jwt = user.Jwt;

            var claims = new List<string>();

            foreach(var role in user.Roles)
            {
                foreach(var permission in role.Permissions)
                {
                    claims.Add(string.Format("{0}:{1}", role.Name, permission));
                }
            }

            Claims = claims;
        }
    }
}
