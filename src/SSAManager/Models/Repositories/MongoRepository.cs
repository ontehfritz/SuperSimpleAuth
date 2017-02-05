namespace SSAManager
{
    using System;
    using MongoDB;
    using System.Collections.Generic;
    using System.Configuration;
    using MongoDB.Driver;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.Linq;
    using System.Web.Hosting;
    using System.Web.Configuration;
    using System.Xml.Serialization;
    using System.Security.Cryptography;
    using System.Linq;
    using SuperSimple.Auth.Api;

    public class MongoRepository : IRepository
    {
        private string connectionString { get; set; }

        public Domain SsaDomain
        {
            get
            {
                return _ssaDomain;
            }
        }

        private const string SSA_DOMAIN = "ssa";
        private readonly MongoClient client;
        private readonly MongoServer server;
        private readonly MongoDatabase database;
        private Domain _ssaDomain;
        IApiRepository _api;

        public void DeleteAdministrator(Guid domainId, Guid adminId)
        {
            var collection = database.GetCollection<Administrator> ("administrators");

            Dictionary<string, object> query = new Dictionary<string, object> ();
            query.Add ("DomainId", domainId);
            query.Add ("ManagerId", adminId);

            collection.Remove(new QueryDocument(query));
        }

        public Manager[] GetAdministrators(Guid domainId)
        {
            List<Manager> admins = new List<Manager>();
            var collection = database.GetCollection<Administrator> ("administrators");
            var query = Query<Administrator>.EQ (e => e.DomainId, domainId);
            var cursor = collection.Find(query);

            foreach (Administrator a in cursor) 
            {
                admins.Add(this.GetManager(a.ManagerId));
            }

            return admins.ToArray();
        }

        public Manager AddAdministrator(Guid domainId, string email)
        {
            Manager admin =          this.GetManager(email);

            if(admin != null)
            {
                Administrator addAdmin = new Administrator();
                addAdmin.Id =            Guid.NewGuid();
                addAdmin.DomainId =      domainId;
                addAdmin.ManagerId =     admin.Id;
                addAdmin.CreatedAt =     DateTime.Now;

                var collection = database.GetCollection<Administrator>("administrators");
                collection.Insert(addAdmin);
            }

            return admin;
        }

        public void ChangePassword(Guid id, string password, string newPassword, string confirmPassword)
        {
            MongoCollection<BsonDocument> managers = database.GetCollection<BsonDocument> ("managers");
            var query = Query.EQ ("_id", id);

            BsonDocument manager = managers.FindOne(query);

            if(manager["Secret"].AsString == Helpers.Hash (manager["_id"].AsGuid.ToString(), 
                                                           password))
            {
                if(newPassword == confirmPassword)
                {
                    manager["Secret"] = Helpers.Hash (manager["_id"].AsGuid.ToString(), 
                                                      newPassword);
                    managers.Save (manager);
                }
                else
                {
                    throw(new Exception ("The new password does not match confirmation password."));
                }
            }
            else
            {
                throw(new Exception ("Password not valid."));
            }
        }


        private string PasswordGenerator(int passwordLength)
        {
            Random r = new Random ();
            int seed = r.Next(1, int.MaxValue);
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            var chars = new char[passwordLength];
            var rd = new Random(seed);

            for (var i = 0 ; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0 , allowedChars.Length)];
            }

            return new string(chars);
        }


        public string ForgotPassword(string email)
        {
            MongoCollection<BsonDocument> managers = 
                database.GetCollection<BsonDocument> ("managers");

            var query = Query.EQ ("UserName", email);

            BsonDocument manager = managers.FindOne(query);

            string newPassword = null;

            if (manager != null) {
                newPassword = this.PasswordGenerator (8);
                manager ["Secret"] = Helpers.Hash (manager["_id"].AsGuid.ToString(), 
                                                   newPassword);
                managers.Save (manager);
            }

            return newPassword;
        }

        public void ChangeEmail(Guid id, string password, string email)
        {
            MongoCollection<BsonDocument> managers = database.GetCollection<BsonDocument> ("managers");
            var query = Query.EQ ("_id", id);

            BsonDocument manager = managers.FindOne(query);

            if(manager["Secret"].AsString == Helpers.Hash (manager["_id"].AsGuid.ToString(), 
                                                           password))
            {
                manager["UserName"] = email;
                managers.Save (manager);
            }
            else
            {
                throw(new Exception ("Password not valid."));
            }
        }

        public Role[] GetRolesWithClaim(Guid domainId, string claim)
        {
            List<Role> roles = new List<Role> ();
            var collection = database.GetCollection<Role> ("roles");

            var query = Query.And(Query<Role>.EQ (e => e.DomainId, domainId),
                                  Query<Role>.EQ (e => e.Claims, claim));

            var rolesdb = collection.Find (query);

            foreach(var role in rolesdb )
            {
                roles.Add (role);
            }

            return roles.ToArray ();
        }

        public User[] GetUsersWithClaim(Guid domainId, string claim)
        {
            List<User> users = new List<User> ();
            var collection = database.GetCollection<User> ("users");

            var query = Query.And(Query<User>.EQ (e => e.DomainId, domainId),
                                  Query<User>.EQ (e => e.Claims, claim));

            var usersdb = collection.Find(query)
                                    .SetFields(Fields.Exclude("Secret","AuthToken"))
                                    .SetSortOrder(SortBy.Ascending("UserName"));

            foreach(var user in usersdb )
            {
                users.Add (user);
            }

            return users.ToArray ();
        }

        public User[] GetUsersInRole(Role role)
        {
            List<User> users = new List<User> ();
            var collection = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.DomainId, role.DomainId),
                                  Query<User>.EQ (e => e.Roles, role));

            var usersdb = collection.Find(query)
                                    .SetFields(Fields.Exclude("Secret","AuthToken"))
                                    .SetSortOrder(SortBy.Ascending("UserName"));

            foreach(var user in usersdb )
            {
                users.Add (user);
            }

            return users.ToArray ();
        }

        public User[] GetDomainUsers(Guid domainId)
        {
            List<User> users = new List<User> ();
            var collection = database.GetCollection<User> ("users");
            var query = Query<User>.EQ (e => e.DomainId, domainId);
            var usersdb = collection.Find(query)
                                    .SetFields(Fields.Exclude("Secret","AuthToken"))
                                    .SetSortOrder(SortBy.Ascending("UserName"));

            foreach(var user in usersdb )
            {
                users.Add (user);
            }

            return users.ToArray ();
        }

        public User[] GetManagerUsers(Guid managerId)
        {
            List<User> users = new List<User> ();

            return users.ToArray ();
        }

        public User GetUser(Guid userId)
        {
            var collection = database.GetCollection<User> ("users");
            var query = Query<User>.EQ (e => e.Id, userId);
            var user = collection.Find(query)
                                 .SetFields(Fields.Exclude("Secret","AuthToken"));

            foreach (var u in user) 
            {
                return u;
            }

            return null;
        }

        public User GetUser(Guid domainId, string username)
        {
            var collection = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.DomainId, domainId),
                                  Query<User>.EQ (e => e.UserName, username));
            var user = collection.Find(query)
                                 .SetFields(Fields.Exclude("Secret","AuthToken"));

            foreach (var u in user) 
            {
                return u;
            }

            return null;
        }

        public User UpdateUser(User user)
        {
            user.ModifiedAt = DateTime.Now;

            MongoCollection<User> users = database.GetCollection<User> ("users");
            var query = Query<User>.EQ (e => e.Id, user.Id);

            var u = users.Find(query);

            User updateUser = null;
            foreach (User temp in u) 
            {
                updateUser = temp;
            }

            if (updateUser != null) 
            {
                updateUser.Roles = user.Roles;
                updateUser.Claims = user.Claims;
                updateUser.Enabled = user.Enabled;

                users.Save(updateUser);
            }

            return GetUser(user.Id);
        }

        public void DeleteUser(Guid domainId, string userName)
        {
            var collection = database.GetCollection<User> ("users");

            Dictionary<string, object> query = new Dictionary<string, object> ();
            query.Add ("DomainId", domainId);
            query.Add ("UserName", userName);

            collection.Remove(new QueryDocument(query));
        }

        public Domain GetDomain(Guid id)
        {
            var collection = database.GetCollection<Domain> ("domains");
            var query = Query.And(Query<Domain>.EQ (e => e.Id, id));

            Domain domain = collection.FindOne (query);

            return domain;
        }

        //        public Domain GetDomain(string name, Guid managerId)
        //        {
        //            var collection = database.GetCollection<Domain> ("domains");
        //            var query = Query.And(Query<Domain>.EQ (e => e.ManagerId, managerId),
        //                Query<Domain>.EQ(e => e.Name, name));
        //
        //            Domain domain = collection.FindOne (query);
        //
        //            return domain;
        //        }

        public Domain[] GetDomainsAdmin(Guid managerId)
        {
            var collection = database.GetCollection<Administrator> ("administrators");
            var query = Query<Administrator>.EQ (e => e.ManagerId, managerId);
            var cursor = collection.Find(query);

            List<Domain> domains = new List<Domain>();

            Domain domain = null;
            foreach (Administrator admin in cursor) 
            {
                domain = this.GetDomain(admin.DomainId);

                if(domain != null)
                {
                    domains.Add (domain);
                }
            }

            return domains.ToArray();
        }

        public Domain[] GetDomains(Guid managerId)
        {
            var collection = database.GetCollection<Domain> ("domains");
            var query = Query<Domain>.EQ (e => e.ManagerId, managerId);
            var cursor = collection.Find(query);

            List<Domain> domains = new List<Domain>();

            foreach (Domain d in cursor) 
            {
                domains.Add (d);
            }

            return domains.ToArray();
        }

        public Domain CreateDomain(string name, Manager manager)
        {
            var collection = database.GetCollection<Domain>("domains");
            Domain domain = new Domain ();
            domain.Id = Guid.NewGuid ();
            domain.Name = name;
            domain.Enabled = true;
            domain.ManagerId = manager.Id;
            domain.ModifiedBy = manager.UserName;
            domain.Key = Guid.NewGuid ();
            //TODO: Change application salt to user salt
            domain.Salt = Guid.NewGuid ();
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;

            collection.Insert(domain);

            return domain;
        }

        public Domain UpdateDomain(Domain domain)
        {
            domain.ModifiedAt = DateTime.Now;

            MongoCollection<BsonDocument> domains = 
                database.GetCollection<BsonDocument> ("domains");
            var query = Query.EQ ("_id", domain.Id);

            BsonDocument updateDomain = domains.FindOne(query);

            if (updateDomain != null) 
            {
                updateDomain ["Key"] = domain.Key;
                updateDomain ["Claims"] = new BsonArray(domain.Claims);
                updateDomain ["WhiteListIps"] = new BsonArray(domain.WhiteListIps);
                updateDomain ["Enabled"] = domain.Enabled;

                domains.Save(updateDomain);
            }

            return GetDomain(domain.Id);
        }

        public void DeleteDomain(Guid id)
        {
            Domain domain = this.GetDomain (id);
            Role[] roles = this.GetRoles(domain.Id);

            foreach (Role r in roles) 
            {
                this.DeleteRole (r);
            }

            User[] users = this.GetDomainUsers (domain.Id);

            foreach (User u in users) 
            {
                this.DeleteUser(domain.Id, u.UserName);
            }

            var collection = database.GetCollection<Domain>("domains");
            collection.Remove(new QueryDocument("_id", domain.Id));
        }

        public Manager CreateManager(string userName, string secret)
        {
            var collection = database.GetCollection<Manager>("managers");


            var user = _api.CreateUser(_ssaDomain.Key,userName,
                                       secret, userName);

            var manager = new Manager(user);
            collection.Insert(manager);


            return manager;
        }

        public void DeleteManager(Guid id, string password)
        {
            MongoCollection<BsonDocument> managers = 
                database.GetCollection<BsonDocument>("managers");
            //var managers = database.GetCollection<Manager>("managers");
            var query = Query.EQ ("_id", id);

            BsonDocument manager = managers.FindOne(query);

            if (manager ["Secret"].AsString == Helpers.Hash (manager ["_id"].AsGuid.ToString (), 
                                                             password)) 
            {
                managers.Remove (new QueryDocument ("_id", id));

                Domain[] domains = GetDomains (id);

                if (domains != null && domains.Length > 0) {
                    foreach (Domain domain in domains) {
                        DeleteDomain (domain.Id);
                    }
                }
            }
        }

        public Manager GetManager(Guid id)
        {
            MongoCollection<BsonDocument> managers = 
                database.GetCollection<BsonDocument>("managers");
            var query = Query.EQ ("_id", id);

            var managerBson = managers.FindOne(query);
            Guid managerId = managerBson["_id"].AsGuid;
            var user = GetUser(managerId);


            var manager = new Manager(user);

            return manager;
        }

        public Manager GetManager(string username)
        {
            var collection = database.GetCollection<User> ("users");

            var query = Query.And (
                Query<User>.EQ (e => e.UserName, username),
                Query<User>.EQ (e => e.DomainId, _ssaDomain.Id));
            
            User user = collection.FindOne (query);

            if(user == null) return null;

            return new Manager(user);
        }

        public Role GetRole(Guid domainId, string name)
        {
            var collection = database.GetCollection<Role> ("roles");
            var query = Query.And (
                Query<Role>.EQ (e => e.DomainId, domainId),
                Query<Role>.EQ (e => e.Name, name));

            var r = collection.Find(query);

            foreach (Role role in r) 
            {
                return role;
            }

            return null;
        }

        public Role UpdateRole(Role role)
        {
            MongoCollection<BsonDocument> roles = 
                database.GetCollection<BsonDocument> ("roles");

            var query = Query.EQ ("_id", role.Id);

            BsonDocument updateRole = roles.FindOne(query);

            if (updateRole != null) 
            {
                updateRole ["Claims"] = new BsonArray(role.Claims);
                roles.Save(updateRole);
            }

            return role;
        }

        public Role[] GetRoles(Guid domainId)
        {
            List<Role> roles = new List<Role> ();
            var collection = database.GetCollection<Role> ("roles");
            var query = Query<Role>.EQ (e => e.DomainId, domainId);

            var r = collection.Find(query);

            foreach (Role role in r) 
            {
                roles.Add (role);
            }

            return roles.ToArray();
        }

        public Role CreateRole(Guid domainId, string name)
        {
            var collection = database.GetCollection<Role>("roles");

            Role role = new Role ();
            role.Id = Guid.NewGuid ();
            role.DomainId =domainId;
            role.Name = name;
            collection.Insert(role);

            return role;
        }

        public void DeleteRole(Role role)
        {
            User[] users = this.GetUsersInRole (role);

            foreach (User u in users) 
            {
                u.RemoveRole (role);
                this.UpdateUser (u);
            }

            var collection = database.GetCollection<Role>("roles");
            collection.Remove(new QueryDocument("_id", role.Id));
        }


        private void CreateDomainIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("Name","ManagerId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);

            var collection = database.GetCollection<Domain>("domains");

            collection.EnsureIndex(keys, options);
        }

        private void CreateUserIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("UserName", "DomainId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);


            var collection = database.GetCollection<User>("users");

            collection.EnsureIndex(keys, options);
        }

        private void CreateRoleIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("Name","DomainId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);

            var collection = database.GetCollection<Role>("roles");

            collection.EnsureIndex(keys, options);
        }

        private void CreateManagerIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("UserName");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);


            var collection = database.GetCollection<Manager>("managers");

            collection.EnsureIndex(keys, options);
        }

        private void CreateAdminIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("DomainId", "ManagerId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);

            var collection = database.GetCollection<Administrator>("administrators");

            collection.EnsureIndex(keys, options);
        }

        private void CreateSsaDomain()
        {
            var collection = database.GetCollection<Domain>("domains");
            var ssaDomain = GetDomains(Guid.Empty);

            if(ssaDomain.Any()) 
            {
                _ssaDomain = ssaDomain.First();
                return;
            }

            var domain = new Domain ();
            domain.Id = Guid.NewGuid ();
            domain.Name = "ssa";
            domain.Enabled = true;
            domain.ManagerId = Guid.Empty;
            domain.Key = Guid.NewGuid ();
            domain.Salt = Guid.NewGuid ();
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;

            collection.Insert(domain);
            _ssaDomain = domain;
        }

        public MongoRepository (string connection,IApiRepository api)
        {
            connectionString = connection;
            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase("SsAuthDb");
            _api = api;

            CreateSsaDomain      ();
            CreateManagerIndexes ();
            CreateDomainIndexes  ();
            CreateUserIndexes    ();
            CreateRoleIndexes    ();
            CreateAdminIndexes   ();

        } 
    }
}

