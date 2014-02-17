using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Linq;

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

        public bool ChangeEmail (Guid domainKey, Guid authToken, string newEmail)
        {
            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            User user = users.FindOne (query);

            if(user != null && user.Enabled)
            {
                user.Email = newEmail;
                WriteConcernResult result = users.Save (user);

                return result.UpdatedExisting;
            }

            return false;

        }


        public bool ChangeUserName (Guid domainKey, Guid authToken, string newUserName)
        {
            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            User user = users.FindOne (query);

            if(user != null && user.Enabled)
            {
                user.UserName = newUserName;
                WriteConcernResult result = users.Save (user);

                return result.UpdatedExisting;
            }

            return false;
        }


        public bool ChangePassword (Guid domainKey, Guid authToken, string newPassword)
        {
            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            User user = users.FindOne (query);

            if(user != null && user.Enabled)
            {
                user.Secret = this.Hash(domain["Salt"].AsGuid.ToString(), newPassword);
                WriteConcernResult result = users.Save (user);

                return result.UpdatedExisting;
            }

            return false;
        }

        public string Forgot(Guid domainKey, string email)
        {
            User user = null;
            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);


            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.Email, email),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            if(user != null && user.Email != null && user.Enabled)
            {
                string newPassword = this.PasswordGenerator (8);
                user.Secret = this.Hash(domain["Salt"].AsGuid.ToString(), newPassword);
                WriteConcernResult result = users.Save (user);

                if (result.UpdatedExisting) 
                {
                    return newPassword;
                }
            }

            return null;
        }

        public bool IpAllowed(Guid domainKey, string ip)
        {
            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dquery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dquery);

            if (!domain ["WhiteListIps"].IsBsonNull) 
            {
                string[] ips = 
                    domain ["WhiteListIps"].AsBsonArray.Select (p => p.AsString).ToArray ();

                if (ips.Length > 0) {
                    return ips.Contains (ip);
                }
            }

            return true;
        }

        public bool UsernameExists (Guid domainKey, string username)
        {
            User user = null;

            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dquery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dquery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.UserName, username),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null) 
            {
                return true;
            }
          
            return false;
        }

        public bool EmailExists (Guid domainKey, string email)
        {
            User user = null;

            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.Email, email),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null) 
            {
                return true;
            }

            return false;
        }

        public bool End(Guid domainKey, Guid authToken)
        {
            User user = null;

            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            user.LastIp = user.CurrentIp;
            user.LastLogon = user.CurrentLogon;
            user.CurrentIp = null;
            user.AuthToken = Guid.Empty;
            user.CurrentLogon = null;

            WriteConcernResult result = users.Save (user);

            return result.UpdatedExisting;

        }

        public User Authenticate (Guid domainKey, string username, 
                                  string secret, string IP = null)
        {
            User user = null;

            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            //TODO: Change application salt to user salt
            var query = Query.And(Query<User>.EQ (e => e.UserName, username),
                Query<User>.EQ(e => e.Secret, this.Hash(domain["Salt"].AsGuid.ToString(), secret)),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null &&
                user.Enabled) {
                user.CurrentIp = IP;
                user.AuthToken = Guid.NewGuid ();
                user.LogonCount += 1;
                user.LastRequest = DateTime.Now;
                user.CurrentLogon = DateTime.Now;

                WriteConcernResult result = users.Save (user);

                if(!result.UpdatedExisting)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return user;
        }

        public User Validate (Guid authToken, Guid domainKey, string IP = null)
        {
            User user = null;

            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null && user.Enabled) {
                user.CurrentIp = IP;
                user.LastRequest = DateTime.Now;
                WriteConcernResult result = users.Save (user);

                if(!result.UpdatedExisting)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return user;
        }

        public User CreateUser (Guid domainKey, string username, 
            string password, string email = null)
        {
            var appCollection = database.GetCollection<RawBsonDocument> ("domains");
            var query = Query.And(Query.EQ ("Key", domainKey));
            var domain = appCollection.FindOne (query);
            User user = new User ();
            user.UserName = username;
            user.Email = email;
            //TODO: Change application salt to user salt
            user.Secret = this.Hash(domain["Salt"].AsGuid.ToString(), password);
            user.DomainId = domain["_id"].AsGuid;
            user.CreatedAt = DateTime.Now;
            user.Enabled = true;
            user.ModifiedAt = DateTime.Now;

            var collection = database.GetCollection<User>("users");
            collection.Insert(user);
            //user.AuthToken = user.AppId.ToString();
            return user;
        }

        public bool Disable (Guid authToken, Guid domainKey, string IP = null)
        {
            User user = null;

            var domains = database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And(Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = database.GetCollection<User> ("users");
            var query = Query.And(Query<User>.EQ (e => e.AuthToken, authToken),
                Query<User>.EQ(e => e.DomainId, domain["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null) {
                user.CurrentIp = IP;
                user.LastRequest = DateTime.Now;
                user.Enabled = false;
                WriteConcernResult result = users.Save (user);

                if(!result.UpdatedExisting)
                {
                    return false;
                }

                this.End (domainKey, authToken);
            }

            return true;
        }

        public bool ValidateDomainKey (string domainName, Guid domainKey)
        {
            var appCollection = database.GetCollection<RawBsonDocument> ("domains");
            var query = Query.And(Query.EQ ("Key", domainKey), 
                Query.EQ("Name", domainName));

            var app = appCollection.FindOne (query);

            if(app != null)
            {
                return true;
            }
          
            return false;
        }

        public string Hash(string Salt, string Password) 
        {
            Rfc2898DeriveBytes Hasher = new Rfc2898DeriveBytes(Password,
                System.Text.Encoding.Default.GetBytes(Salt), 10000);

            return Convert.ToBase64String(Hasher.GetBytes(25));
        }

    }
}

