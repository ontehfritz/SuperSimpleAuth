namespace SuperSimple.Auth.Api.Repository
{
    using System;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Bson;
    using System.Security.Cryptography;
    using System.Linq;

    public class ApiMongoRepository : IApiRepository
    {
        private string _connection { get; set; }
        private MongoClient _client;
        private MongoServer _server;
        private MongoDatabase _database;

        public ApiMongoRepository (string connection)
        {
            _connection = connection;
            _client = new MongoClient (_connection);
            _server = _client.GetServer ();
            _database = _server.GetDatabase ("SsAuthDb");
        }

        private string PasswordGenerator (int passwordLength)
        {
            var r = new Random ();
            int seed = r.Next (1, int.MaxValue);
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            var chars = new char [passwordLength];
            var rd = new Random (seed);

            for (var i = 0; i < passwordLength; i++)
            {
                chars [i] = allowedChars [rd.Next (0, allowedChars.Length)];
            }

            return new string (chars);
        }

        private string Hash (string salt, string password)
        {
            var hasher = 
                new Rfc2898DeriveBytes (password,
                                        System.Text.Encoding.Default.GetBytes (salt)
                                        , 10000);

            return Convert.ToBase64String (hasher.GetBytes (25));
        }


        public bool ChangeEmail (Guid domainKey, Guid authToken, string newEmail)
        {
            newEmail = newEmail.ToLower ();
            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            var user = users.FindOne (query);

            if (user != null && user.Enabled)
            {
                user.Email = newEmail;
                WriteConcernResult result = users.Save (user);

                return result.UpdatedExisting;
            }

            return false;

        }


        public bool ChangeUserName (Guid domainKey, Guid authToken, string newUserName)
        {
            newUserName = newUserName.ToLower ();
            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            var user = users.FindOne (query);

            if (user != null && user.Enabled)
            {
                user.UserName = newUserName;
                WriteConcernResult result = users.Save (user);

                return result.UpdatedExisting;
            }

            return false;
        }


        public bool ChangePassword (Guid domainKey, Guid authToken, string newPassword)
        {
            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            var user = users.FindOne (query);

            if (user != null && user.Enabled)
            {
                user.Secret = this.Hash (domain ["Salt"].AsGuid.ToString (), newPassword);
                WriteConcernResult result = users.Save (user);

                return result.UpdatedExisting;
            }

            return false;
        }

        public string Forgot (Guid domainKey, string email)
        {
            User user = null;
            email = email.ToLower ();
            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);


            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.Email, email),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null && user.Email != null && user.Enabled)
            {
                string newPassword = this.PasswordGenerator (8);
                user.Secret = this.Hash (domain ["Salt"].AsGuid.ToString (), newPassword);
                var result = users.Save (user);

                if (result.UpdatedExisting)
                {
                    return newPassword;
                }
            }

            return null;
        }

        public bool IpAllowed (Guid domainKey, string ip)
        {
            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dquery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dquery);

            if (!domain ["WhiteListIps"].IsBsonNull)
            {
                var ips =
                    domain ["WhiteListIps"].AsBsonArray
                                           .Select (p => p.AsString).ToArray ();

                if (ips.Length > 0)
                {
                    return ips.Contains (ip);
                }
            }

            return true;
        }

        public bool UsernameExists (Guid domainKey, string username)
        {
            User user = null;
            username = username.ToLower ();

            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dquery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dquery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.UserName, username),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

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

            email = email.ToLower ();

            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.Email, email),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null)
            {
                return true;
            }

            return false;
        }

        public bool End (Guid domainKey, Guid authToken)
        {
            User user = null;

            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            user = users.FindOne (query);

            user.LastIp = user.CurrentIp;
            user.LastLogon = user.CurrentLogon;
            user.CurrentIp = null;
            user.AuthToken = Guid.Empty;
            user.CurrentLogon = null;

            var result = users.Save (user);

            return result.UpdatedExisting;

        }

        public User Authenticate (Guid domainKey, string username,
                                  string secret, string IP = null)
        {
            User user = null;

            username = username.ToLower ();

            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            //TODO: Change application salt to user salt
            var query = Query.And
            (Query<User>.EQ (e => e.UserName, username),
             Query<User>.EQ (e => e.Secret, 
                             Hash (domain ["Salt"].AsGuid.ToString (), secret)),
             Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null &&
                user.Enabled)
            {
                user.CurrentIp = IP;
                user.AuthToken = Guid.NewGuid ();
                user.LogonCount += 1;
                user.LastRequest = DateTime.Now;
                user.CurrentLogon = DateTime.Now;

                var result = users.Save (user);

                if (!result.UpdatedExisting)
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

            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null && user.Enabled)
            {
                user.CurrentIp = IP;
                user.LastRequest = DateTime.Now;
                var result = users.Save (user);

                if (!result.UpdatedExisting)
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
            username = username.ToLower ();

            if (email != null)
            {
                email = email.ToLower ();
            }

            var appCollection = _database.GetCollection<RawBsonDocument> ("domains");
            var query = Query.And (Query.EQ ("Key", domainKey));
            var domain = appCollection.FindOne (query);
            var user = new User ();
            user.UserName = username;
            user.Email = email;
            //TODO: Change application salt to user salt
            user.Secret = this.Hash (domain ["Salt"].AsGuid.ToString (), password);
            user.DomainId = domain ["_id"].AsGuid;
            user.CreatedAt = DateTime.Now;
            user.Enabled = true;
            user.ModifiedAt = DateTime.Now;

            var collection = _database.GetCollection<User> ("users");
            collection.Insert (user);
            //user.AuthToken = user.AppId.ToString();
            return user;
        }

        public bool Disable (Guid authToken, Guid domainKey, string IP = null)
        {
            User user = null;

            var domains = _database.GetCollection<RawBsonDocument> ("domains");
            var dQuery = Query.And (Query.EQ ("Key", domainKey));
            var domain = domains.FindOne (dQuery);

            var users = _database.GetCollection<User> ("users");
            var query = Query.And (Query<User>.EQ (e => e.AuthToken, authToken),
                                  Query<User>.EQ (e => e.DomainId, domain ["_id"].AsGuid));

            user = users.FindOne (query);

            if (user != null)
            {
                user.CurrentIp = IP;
                user.LastRequest = DateTime.Now;
                user.Enabled = false;
                var result = users.Save (user);

                if (!result.UpdatedExisting)
                {
                    return false;
                }

                this.End (domainKey, authToken);
            }

            return true;
        }

        public bool ValidateDomainKey (string domainName, Guid domainKey)
        {
            var appCollection = _database.GetCollection<RawBsonDocument> ("domains");
            var query = Query.And (Query.EQ ("Key", domainKey),
                                  Query.EQ ("Name", domainName));

            var app = appCollection.FindOne (query);

            if (app != null)
            {
                return true;
            }

            return false;
        }
    }
}

