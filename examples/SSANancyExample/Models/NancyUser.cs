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
            var user = (NancyUserIdentity)context.CurrentUser;


            var valid = _ssa.Validate (user._user,
                                         context.Request.UserHostAddress);
            if(valid) return user;
            return null;
        }
    }

    public class NancyUserIdentity : IUserIdentity
    {
        public User _user;
        public Guid Id { get; }
        public string AuthToken { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Claims { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public NancyUserIdentity(User user)
        {
            _user = user;
            Id = user.Id;
        }
    }
}

