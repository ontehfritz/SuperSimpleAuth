namespace SuperSimple.Auth.Manager
{
    using System;
    using Nancy;
    using Nancy.Security;
    using Nancy.Authentication.Forms;
    using System.Collections.Generic;
    using Api.Repository;
    using Api;
    using SuperSimple.Auth.Manager.Repository;

    public class ManagerMapper : IUserMapper
    {
        public IRepository Repository { get; set; }

        public ManagerMapper(IRepository repository)
        {
            Repository = repository;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var manager = new Manager(Repository.GetManager (identifier));

            if (manager == null) 
            {
                return null;
            }

            return manager;
        }
    }

    public class Manager : IUser, IUserIdentity
    {
        private IUser _user   { get; }
        public string IP     { get { return _user.CurrentIp; } }

        IEnumerable<string> IUserIdentity.Claims
        {
            get
            {
                return _user.Claims;
            }
        }

        public Guid Id
        {
            get
            {
                return _user.Id;
            }

            set
            {
                _user.Id = value;
            }
        }

        public Guid DomainId
        {
            get
            {
                return _user.DomainId;
            }

            set
            {
                _user.DomainId = value;
            }
        }

        public string UserName
        {
            get
            {
                return _user.UserName;
            }

            set
            {
                _user.UserName = value;
            }
        }

        public string Email
        {
            get
            {
                return _user.Email;
            }

            set
            {
                _user.Email = value;
            }
        }

        public string Secret
        {
            get
            {
                return _user.Secret;
            }

            set
            {
                _user.Secret = value;
            }
        }

        public Guid AuthToken
        {
            get
            {
                return _user.AuthToken;
            }

            set
            {
                _user.AuthToken = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _user.Enabled;
            }

            set
            {
                _user.Enabled = value;
            }
        }

        public List<Role> Roles
        {
            get
            {
                return _user.Roles;
            }

            set
            {
                _user.Roles = value;
            }
        }

        public List<string> Claims
        {
            get
            {
                return _user.Claims;
            }

            set
            {
                _user.Claims = value;
            }
        }

        public string CurrentIp
        {
            get
            {
                return _user.CurrentIp;
            }

            set
            {
                _user.CurrentIp = value;
            }
        }

        public DateTime? CurrentLogon
        {
            get
            {
                return _user.CurrentLogon;
            }

            set
            {
                _user.CurrentLogon = value;
            }
        }

        public string LastIp
        {
            get
            {
                return _user.LastIp;
            }

            set
            {
                _user.LastIp = value;
            }
        }

        public DateTime? LastLogon
        {
            get
            {
                return _user.LastLogon;
            }

            set
            {
                _user.LastLogon = value;
            }
        }

        public DateTime? LastRequest
        {
            get
            {
                return _user.LastRequest;
            }

            set
            {
                _user.LastRequest = value;
            }
        }

        public int LogonCount
        {
            get
            {
                return _user.LogonCount;
            }

            set
            {
                _user.LogonCount = value;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return _user.CreatedAt;
            }

            set
            {
                _user.CreatedAt = value;
            }
        }

        public DateTime ModifiedAt
        {
            get
            {
                return _user.ModifiedAt;
            }

            set
            {
                _user.ModifiedAt = value;
            }
        }

        public Manager (IUser user)
        {
            _user = user;
        }

        public string [] GetRoles ()
        {
            return _user.GetRoles();
        }

        public void AddRole (Role role)
        {
            _user.AddRole(role);
        }

        public void RemoveRole (Role role)
        {
            _user.RemoveRole(role);
        }

        public string [] GetClaims ()
        {
            return _user.GetClaims();
        }

        public bool InRole (string role)
        {
            return _user.InRole(role);
        }

        public bool HasClaim (string claim)
        {
            return _user.HasClaim(claim);
        }
    }
}

