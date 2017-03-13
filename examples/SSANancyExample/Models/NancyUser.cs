using System;
using Nancy.Authentication.Forms;
using Nancy.Security;
using Nancy;
using System.Collections.Generic;
using SuperSimple.Auth;

namespace SSANancyExample
{
    public class NancyUserMapper: IUserMapper
    {
        private SuperSimpleAuth _ssa; 

        public NancyUserMapper(SuperSimpleAuth ssa)
        {
            _ssa = ssa;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var ssaUser = _ssa.Validate (identifier,
                                         context.Request.UserHostAddress);
            
            var user = new NancyUserIdentity(ssaUser);
           
            return user;
        }
    }

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
        }
    }
}

