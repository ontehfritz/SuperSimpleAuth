namespace SuperSimple.Auth.Manager
{
    using System;
    using Nancy;
    using Nancy.Security;
    using Nancy.Authentication.Forms;
    using System.Collections.Generic;
    using SuperSimple.Auth.Api.Repository;

    public class ManagerMapper : IUserMapper
    {
        public IRepository Repository { get; set; }

        public ManagerMapper(IRepository repository)
        {
            Repository = repository;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            Manager manager = Repository.GetManager (identifier);

            if (manager == null) 
            {
                return null;
            }

            return manager;
        }
    }

    public class Manager : IUserIdentity
    {
        private User User   { get; }

        public Guid Id                          { get; }
        public string UserName                  { get; }
        public DateTime? LastLogon              { get; }
        public string Secret                    { get; }
        public string IP                        { get; }
        public IEnumerable<string> Claims       { get; }


        public Manager(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            LastLogon = user.LastLogon;
            Secret = user.Secret;
            IP = user.CurrentIp;
            Claims = user.Claims;
        }
    }
}

