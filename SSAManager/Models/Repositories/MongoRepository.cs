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


namespace SSAManager
{
    public class MongoRepository : IRepository
    {
        private string connectionString { get; set; }
        private MongoClient client;
        private MongoServer server;
        private MongoDatabase database;


        public Role[] GetRolesWithClaim(Guid appId, string claim)
        {
            List<Role> roles = new List<Role> ();
            var collection = database.GetCollection<Role> ("roles");

            var query = Query.And(Query<Role>.EQ (e => e.AppId, appId),
                Query<Role>.EQ (e => e.Claims, claim));

            var rolesdb = collection.Find (query);

            foreach(var role in rolesdb ){
                roles.Add (role);
            }

            return roles.ToArray ();
        }

        public User[] GetUsersWithClaim(Guid appId, string claim)
        {
            List<User> users = new List<User> ();
            var collection = database.GetCollection<User> ("users");

            var query = Query.And(Query<User>.EQ (e => e.AppId, appId),
                Query<User>.EQ (e => e.Claims, claim));

            var usersdb = collection.Find(query)
                .SetFields(Fields.Exclude("Secret","AuthToken"));

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
            var query = Query.And(Query<User>.EQ (e => e.AppId, role.AppId),
                                  Query<User>.EQ (e => e.Roles, role));

            var usersdb = collection.Find(query)
                .SetFields(Fields.Exclude("Secret","AuthToken"));

            foreach(var user in usersdb )
            {
                users.Add (user);
            }

            return users.ToArray ();
        }

        public User[] GetAppUsers(Guid appId)
        {
            List<User> users = new List<User> ();
            var collection = database.GetCollection<User> ("users");
            var query = Query<User>.EQ (e => e.AppId, appId);
            var usersdb = collection.Find(query)
                .SetFields(Fields.Exclude("Secret","AuthToken"));

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

            foreach (var u in user) {
                return u;
            }

            return null;
        }

        public User GetUser(Guid appId, string username)
        {
            var collection = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AppId, appId),
                Query<User>.EQ (e => e.Username, username));
            var user = collection.Find(query)
                .SetFields(Fields.Exclude("Secret","AuthToken"));

            foreach (var u in user) {
                return u;
            }

            return null;
        }

        public User UpdateUser(User user){
            user.ModifiedAt = DateTime.Now;

            MongoCollection<User> users = database.GetCollection<User> ("users");
            var query = Query<User>.EQ (e => e.Id, user.Id);

            var u = users.Find(query);

            User updateUser = null;
            foreach (User temp in u) {
                updateUser = temp;
            }

            if (updateUser != null) {
                updateUser.Roles = user.Roles;
                updateUser.Claims = user.Claims;
         
                users.Save(updateUser);
            }

            return GetUser(user.Id);
        }

        public void DeleteUser(Guid appId, string userName)
        {
            var collection = database.GetCollection<User> ("users");

            Dictionary<string, object> query = new Dictionary<string, object> ();
            query.Add ("AppId", appId);
            query.Add ("Username", userName);

            collection.Remove(new QueryDocument(query));
        }
       
        public App GetApp(string name, Guid managerId)
        {
            var collection = database.GetCollection<App> ("apps");
            var query = Query.And(Query<App>.EQ (e => e.ManagerId, managerId),
                                  Query<App>.EQ(e => e.Name, name));

            App app = collection.FindOne (query);

            return app;
        }

        public App[] GetApps(Guid managerId)
        {
            var collection = database.GetCollection<App> ("apps");
            var query = Query<App>.EQ (e => e.ManagerId, managerId);
            var cursor = collection.Find(query);

            List<App> apps = new List<App>();

            foreach (App a in cursor) {
                apps.Add (a);
            }

            return apps.ToArray();
        }

        public App CreateApp(string name, Manager manager)
        {
            var collection = database.GetCollection<App>("apps");
            App app = new App ();
            app.Id = Guid.NewGuid ();
            app.Name = name;
            app.ManagerId = manager.Id;
            app.ModifiedBy = manager.UserName;
            app.Key = Guid.NewGuid ();
            app.CreatedAt = DateTime.Now;
            app.ModifiedAt = DateTime.Now;

            collection.Insert(app);
          
            return app;
        }

        public App UpdateApp(App app)
        {
            app.ModifiedAt = DateTime.Now;

            MongoCollection<BsonDocument> apps = database.GetCollection<BsonDocument> ("apps");
            var query = Query.EQ ("_id", app.Id);

            BsonDocument updateApp = apps.FindOne(query);

            if (updateApp != null) {
                updateApp ["Key"] = app.Key;
                updateApp ["Claims"] = new BsonArray(app.Claims);
                updateApp ["WhiteListIps"] = new BsonArray(app.WhiteListIps);
             
                apps.Save(updateApp);
            }

            return GetApp(app.Name, app.ManagerId);
        }

        public void DeleteApp(string name, Guid managerId)
        {
            App app = this.GetApp (name, managerId);
            Role[] roles = this.GetRoles(app.Id);

            foreach (Role r in roles) {
                this.DeleteRole (r);
            }

            User[] users = this.GetAppUsers (app.Id);

            foreach (User u in users) {
                this.DeleteUser(app.Id, u.Username);
            }

            var collection = database.GetCollection<App>("apps");
            collection.Remove(new QueryDocument("_id", app.Id));
        }

        public Manager CreateManager(Manager manager)
        {
            var collection = database.GetCollection<Manager>("managers");
            manager.Id = Guid.NewGuid();
            collection.Insert(manager);
            return manager;
        }

        public void DeleteManager(Guid id)
        {
            var collection = database.GetCollection<Manager>("managers");
            collection.Remove(new QueryDocument("_id", id));
        }

        public Manager UpdateManager(Manager manager)
        {
            return manager;
        }

        public Manager GetManager(Guid id)
        {
            var collection = database.GetCollection<Manager> ("managers");
            var query = Query<Manager>.EQ (e => e.Id, id);
            Manager m = collection.FindOne (query);

            return m;
        }

        public Manager GetManager(string username)
        {
            var collection = database.GetCollection<Manager> ("managers");
            var query = Query<Manager>.EQ (e => e.UserName, username);
            Manager m = collection.FindOne (query);
             
            return m;
        }

        public Role GetRole(Guid appId, string name)
        {
            var collection = database.GetCollection<Role> ("roles");
            var query = Query.And (
                Query<Role>.EQ (e => e.AppId, appId),
                Query<Role>.EQ (e => e.Name, name));

            var r = collection.Find(query);

            foreach (Role role in r) {
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

            if (updateRole != null) {
                updateRole ["Claims"] = new BsonArray(role.Claims);
                roles.Save(updateRole);
            }

            return role;
        }

        public Role[] GetRoles(Guid appId)
        {
            List<Role> roles = new List<Role> ();
            var collection = database.GetCollection<Role> ("roles");
            var query = Query<Role>.EQ (e => e.AppId, appId);

            var r = collection.Find(query);
          
            foreach (Role role in r) {
                roles.Add (role);
            }

            return roles.ToArray();
        }

        public Role CreateRole(Guid appId, string name)
        {
            var collection = database.GetCollection<Role>("roles");

            Role role = new Role ();
            role.Id = Guid.NewGuid ();
            role.AppId = appId;
            role.Name = name;
            collection.Insert(role);

            return role;
        }

        public void DeleteRole(Role role)
        {
            User[] users = this.GetUsersInRole (role);

            foreach (User u in users) {
                u.RemoveRole (role);
                this.UpdateUser (u);
            }

            var collection = database.GetCollection<Role>("roles");
            collection.Remove(new QueryDocument("_id", role.Id));
        }


        private void CreateAppIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("Name","ManagerId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);

            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase("SsAuthDb");

            var collection = database.GetCollection<App>("apps");

            collection.EnsureIndex(keys, options);
        }

        private void CreateUserIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("Username","AppId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);

            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase("SsAuthDb");

            var collection = database.GetCollection<User>("users");

            collection.EnsureIndex(keys, options);
        }

        private void CreateRoleIndexes()
        {
            var keys = new IndexKeysBuilder();

            keys.Ascending("Name","AppId");

            var options = new IndexOptionsBuilder();
            options.SetSparse(true);
            options.SetUnique(true);

            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase("SsAuthDb");

            var collection = database.GetCollection<User>("roles");

            collection.EnsureIndex(keys, options);
        }


        public MongoRepository (string connection)
        {
            connectionString = connection;

            this.CreateAppIndexes ();
            this.CreateUserIndexes ();
            this.CreateRoleIndexes ();
        }
    }
}

