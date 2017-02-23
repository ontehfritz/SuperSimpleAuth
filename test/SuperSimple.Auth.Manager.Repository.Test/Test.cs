namespace SuperSimple.Auth.Manager.Repository.Test
{
    using NUnit.Framework;
    using System;
    using SuperSimple.Auth.Api.Repository;

    [TestFixture ()]
    public class RepositoryTest
    {
        private const string CONNECTION_STRING = "mongodb://localhost";

        [Test ()]
        public void CreateManager ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            Assert.AreEqual(manager.UserName, userName);
            repository.DeleteManager(manager.Id,secret);
        }

        [Test()]
        public void DeleteManager()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var id = manager.Id;

            repository.DeleteManager(manager.Id, secret);
            manager = repository.GetManager(id);
            Assert.IsNull(manager);
        }

        [Test()]
        public void GetManager()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            manager = repository.GetManager(manager.Id);

            Assert.AreEqual(manager.UserName, userName);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetManagerByName()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            manager = repository.GetManager(userName);

            Assert.AreEqual(manager.UserName, userName);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void ChangeEmail()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            repository.ChangeEmail(manager.Id, secret, "test@test.com");
            manager = repository.GetManager(manager.Id);
            Assert.AreEqual(manager.UserName, "test@test.com");
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void ChangePassword()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var pass = manager.Secret;
            repository.ChangePassword(manager.Id, secret,"password", "password");
            manager = repository.GetManager(manager.Id);
            //Assert.AreEqual(manager.Secret, pass);
            //implicit assert, would fail if password didn't change
            repository.DeleteManager(manager.Id, "password");
        }

        [Test()]
        public void ForgotPassword ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var newPassword = repository.ForgotPassword(userName);
            //implicit assert, would fail if password didn't change
            repository.DeleteManager(manager.Id, newPassword);
        }

        [Test()]
        public void CreateDomain()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            Assert.AreEqual("test", domain.Name);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void DeleteDomain()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);
            repository.DeleteDomain(domain.Id);
            domain = repository.GetDomain(domain.Id);

            Assert.IsNull(domain);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetDomain()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);
            domain = repository.GetDomain(domain.Id);

            Assert.AreEqual(domain.Name, "test");
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetDomains()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);


            repository.CreateDomain("test", manager);
            repository.CreateDomain("test2", manager);
            var domains = repository.GetDomains(manager.Id);

            Assert.AreEqual(2, domains.Length);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void UpdateDomain()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);
            Assert.True(domain.Enabled);
            domain.Enabled = false;
            domain = repository.UpdateDomain(domain);
            Assert.False(domain.Enabled);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void HasAccess()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            var result = repository.HasAccess(domain, manager);

            Assert.True(result);

            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetOwnerName()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            var result = repository.GetOwnerName(domain);

            Assert.AreEqual(result, manager.UserName);

            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetDomainsAdmin()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var admin = repository.CreateManager(userName + "2", secret);

            var domain = repository.CreateDomain("test", manager);

            admin = repository.AddAdministrator(domain.Id,userName + "2");

            var admins = repository.GetDomainsAdmin(admin.Id);
            Assert.AreEqual(1, admins.Length);

            repository.DeleteManager(manager.Id, secret);
            repository.DeleteManager(admin.Id, secret);
        }

        [Test()]
        public void GetDomainUsers()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                                "test@test.com");

            var users = repository.GetDomainUsers(domain.Id);

            Assert.AreEqual(1, users.Length);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void AddAdministrator()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var admin = repository.CreateManager(userName + "2", secret);

            var domain = repository.CreateDomain("test", manager);

            admin = repository.AddAdministrator(domain.Id,userName + "2");

            Assert.AreEqual(userName + "2", admin.UserName);

            repository.DeleteManager(manager.Id, secret);
            repository.DeleteManager(admin.Id, secret);
        }

        [Test()]
        public void GetAdministrators()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var admin = repository.CreateManager(userName + "2", secret);

            var domain = repository.CreateDomain("test", manager);

            admin = repository.AddAdministrator(domain.Id,userName + "2");
            var result = repository.GetAdministrators(domain.Id);

            Assert.AreEqual(1, result.Length);

            repository.DeleteManager(manager.Id, secret);
            repository.DeleteManager(admin.Id, secret);
        }

        [Test()]
        public void DeleteAdministrator()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var admin = repository.CreateManager(userName + "2", secret);

            var domain = repository.CreateDomain("test", manager);

            admin = repository.AddAdministrator(domain.Id,userName + "2");
            repository.DeleteAdministrator(domain.Id,admin.Id);
            var result = repository.GetAdministrators(domain.Id);

            Assert.AreEqual(0, result.Length);

            repository.DeleteManager(manager.Id, secret);
            repository.DeleteManager(admin.Id, secret);
        }

        [Test()]
        public void GetUser()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                     "test@test.com");

            user = repository.GetUser(user.Id);

            Assert.NotNull(user);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetUserInDomain()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                                "test@test.com");

            user = repository.GetUser(domain.Id, "user1");

            Assert.NotNull(user);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetUsersInRole()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                                "test@test.com");
            
            var role = repository.CreateRole(domain.Id,"test");

            user.AddRole(role);
            repository.UpdateUser(user);

            var users = repository.GetUsersInRole(role);
            Assert.AreEqual(1,users.Length);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void UpdateUser()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                                "test@test.com");
            Assert.True(user.Enabled);
            user.Enabled = false;
            user = repository.UpdateUser(user);
            Assert.False(user.Enabled);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void DeleteUser()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                                "test@test.com");

            repository.DeleteUser(domain.Id, user.UserName);

            user = repository.GetUser(user.Id);

            Assert.IsNull(user);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetUsersWithClaim()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);
            var domain = repository.CreateDomain("test", manager);
            var user = apiRepository.CreateUser(domain.Key, "user1","test",
                                                "test@test.com");

            user.Claims.Add("claim1");
            user.Claims.Add("claim2");

            repository.UpdateUser(user);

            var users = repository.GetUsersWithClaim(domain.Id, "claim1");
            Assert.AreEqual(1,users.Length);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void CreateRole()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            var role = repository.CreateRole(domain.Id,"test");

            Assert.AreEqual("test", role.Name);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetRole()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            var role = repository.CreateRole(domain.Id,"test");
            role = repository.GetRole(domain.Id,"test");

            Assert.AreEqual("test", role.Name);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void DeleteRole()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            var role = repository.CreateRole(domain.Id,"test");
            repository.DeleteRole(role);

            role = repository.GetRole(domain.Id,"test");

            Assert.IsNull(role);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetRoles()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            repository.CreateRole(domain.Id,"test");
            repository.CreateRole(domain.Id,"test2");

            var roles = repository.GetRoles(domain.Id);
            Assert.AreEqual(2, roles.Length);
            repository.DeleteManager(manager.Id, secret);
        }

        [Test()]
        public void GetRolesWithClaim()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION_STRING);
            var repository = new MongoRepository(CONNECTION_STRING, 
                                                 apiRepository);
            var userName = "test";
            var secret = "test";
            var manager = repository.CreateManager(userName, secret);

            var domain = repository.CreateDomain("test", manager);

            var role = repository.CreateRole(domain.Id,"test");
            role.Claims.Add("claim1");

            repository.UpdateRole(role);

            var roles = repository.GetRolesWithClaim(domain.Id,"claim1");

            Assert.AreEqual(1, roles.Length);
            repository.DeleteManager(manager.Id, secret);
        }
    }
}
