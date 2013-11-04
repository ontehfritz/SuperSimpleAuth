using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Runtime.Remoting.Messaging;

namespace SuperSimple.Auth.Api
{
    public class MongoRepository : IRepository
    {
        private string connectionString { get; set; }
        private MongoClient client;
        private MongoServer server;
        private MongoDatabase database;

        public MongoRepository (string connection)
        {
            connectionString = connection;
            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase("SsAuthDb");
        }


        public bool End(Guid appKey, Guid authToken)
        {
            User user = null;

            var appCollection = database.GetCollection<RawBsonDocument> ("apps");
            var appQuery = Query.And(Query.EQ ("Key", appKey));
            var app = appCollection.FindOne (appQuery);

            var collection = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ(e => e.AppId, app["_id"].AsGuid));

            user = collection.FindOne (query);

            user.LastIp = user.CurrentIp;
            user.LastLogon = user.CurrentLogon;
            user.CurrentIp = null;
            user.AuthToken = Guid.Empty;
            user.CurrentLogon = null;

            if (UpdateUser (appKey, user) == null) {
                return false;
            }

            return true; 

        }

        public User Authenticate (Guid appKey, string username, 
                                  string secret, string IP = null)
        {
            User user = null;

            var appCollection = database.GetCollection<RawBsonDocument> ("apps");
            var appQuery = Query.And(Query.EQ ("Key", appKey));
            var app = appCollection.FindOne (appQuery);

            var collection = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.Username, username),
                                  Query<User>.EQ(e => e.Secret, secret),
                                  Query<User>.EQ(e => e.AppId, app["_id"].AsGuid));

            user = collection.FindOne (query);

            if (user != null) {
                user.CurrentIp = IP;
                user.AuthToken = Guid.NewGuid ();
                user.LogonCount += 1;
                user.LastRequest = DateTime.Now;
                user.CurrentLogon = DateTime.Now;

                UpdateUser (appKey, user);
            }

            return user;
        }

        public User Validate (Guid authToken, Guid appKey, string IP = null)
        {
            User user = null;

            var appCollection = database.GetCollection<RawBsonDocument> ("apps");
            var appQuery = Query.And(Query.EQ ("Key", appKey));
            var app = appCollection.FindOne (appQuery);

            var collection = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ(e => e.AppId, app["_id"].AsGuid));

            user = collection.FindOne (query);

            if (user != null) {
                user.CurrentIp = IP;
                user.LastRequest = DateTime.Now;
                UpdateUser (appKey, user);
            }

            return user;
        }

        public User CreateUser (Guid appKey, User user)
        {
            var appCollection = database.GetCollection<RawBsonDocument> ("apps");
            var query = Query.And(Query.EQ ("Key", appKey));
            var app = appCollection.FindOne (query);
            user.AppId = app["_id"].AsGuid;
            user.CreatedAt = DateTime.Now;
            user.Enabled = true;
            user.ModifiedAt = DateTime.Now;

            var collection = database.GetCollection<User>("users");
            collection.Insert(user);
            //user.AuthToken = user.AppId.ToString();
            return user;
        }

        public User UpdateUser (Guid appKey, User user)
        {
            user.ModifiedAt = DateTime.Now;

            MongoCollection<User> users = database.GetCollection<User> ("users");
            var query = Query<User>.EQ (e => e.Id, user.Id);

            var u = users.Find(query);

            User updateUser = null;

            foreach (User temp in u) {
                updateUser = temp;
            }

            if (updateUser != null) {
                updateUser.AuthToken = user.AuthToken;
                updateUser.LastRequest = user.LastRequest;
                updateUser.LogonCount = user.LogonCount;
                updateUser.Secret = user.Secret;
                updateUser.CurrentIp = user.CurrentIp;
                updateUser.CurrentLogon = user.CurrentLogon;
                updateUser.LastLogon = user.LastLogon;
                updateUser.LastIp = user.LastIp;

                users.Save (updateUser);
            } else {
                user = null;
            }

            return user;
        }


        public bool ValidateAppKey (string appName, Guid appKey)
        {
            var appCollection = database.GetCollection<RawBsonDocument> ("apps");
            var query = Query.And(Query.EQ ("Key", appKey), 
                                  Query.EQ("Name", appName));

            var app = appCollection.FindOne (query);

            if(app != null)
            {
                return true;
            }
          
            return false;
        }
    }
}

