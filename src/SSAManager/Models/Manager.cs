using System;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;
using System.Collections.Generic;
using System.Web.Razor.Text;
using MongoDB;
using MongoDB.Bson; 


namespace SSAManager
{
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
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime LastLogon { get; set; }
        public string Secret { get; set; }
        public string IP { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}

