using System;
using NUnit.Framework;
using SSAManager;
using Nancy;
using System.Collections.Generic;

//SSA Manager Tests 
namespace SSA_Test
{
    [TestFixture()]
    public class SSAManagerTest
    {
        IRepository repository = new MongoRepository ("mongodb://localhost");
        string appGuid = "09070f2c-3cb3-486f-82be-13dc5b054b71";
        string managerId = "c189f4d6-1f25-4599-a20c-290b647c11f8";
        string appName = "test";
     
        [Test()]
        public void TestGetUser ()
        {
            Guid appId = Guid.Parse (appGuid);
            User[] users = repository.GetAppUsers(appId);
            User user = repository.GetUser(users[0].Id);
            Assert.IsNotNull (user);
        }

        [Test()]
        public void TestUpdateUser ()
        {
            Guid appId = Guid.Parse (appGuid);
            User[] users = repository.GetAppUsers(appId);
            User user = repository.GetUser(users[0].Id);
            user.Roles = user.Roles;
            user = repository.UpdateUser (user);

            Assert.IsNotNull (user);
        }
       
        [Test()]
        public void TestGetAppUsers ()
        {
            Guid appId = Guid.Parse (appGuid);
            User[] users = repository.GetAppUsers(appId);
            Assert.Greater (users.Length,0);
        }

        [Test()]
        public void TestGetManager()
        {
            Manager manager = repository.GetManager ("test24@test.com");
            System.Console.WriteLine (manager.Id);
            Assert.AreEqual ("test24@test.com", manager.UserName);
        }

        [Test()]
        public void TestGetApp()
        {
            Guid appId = Guid.Parse (appGuid);
            App app = repository.GetApp(Guid.Parse(managerId),
                                        appName);

            Assert.AreEqual (appId, app.Id);
        }

        [Test()]
        public void TestCreateRole()
        {
            Guid appId = Guid.Parse (appGuid);
           
            Role role = new Role ();
            role.Id = Guid.NewGuid ();
            role.AppId = appId;
            role.Name = "test_test";

            Role r = repository.CreateRole(role);

            Assert.AreEqual ("test_test",r.Name);
        }


        [Test()]
        public void TestGetRole()
        {
            Guid appId = Guid.Parse (appGuid);
           
            Role role = repository.GetRole (appId, "test_test");

            Assert.IsNotNull (role);
        }

        [Test()]
        public void TestGetRoles()
        {
            Guid appId = Guid.Parse (appGuid);

            Role[] roles = repository.GetRoles (appId);

            Assert.IsNotNull (roles);
        }

        [Test()]
        public void TestUpdateRole()
        {
            Guid appId = Guid.Parse (appGuid);

            Role role = repository.GetRole (appId, "test_test");

            List<string> claims = new List<string>();
            claims.Add ("Write");
            claims.Add ("Read");

            role.Claims = claims;
            repository.UpdateRole (role);
            role = repository.GetRole (appId, "test_test");

            Assert.IsNotNull (role.Claims);
        }

        [Test()]
        public void TestGetUsersInRole ()
        {
            Guid appId = Guid.Parse (appGuid);
            Role role = repository.GetRole (appId, "test_test");

            User[] users = repository.GetUsersInRole(role);
            Assert.Greater (users.Length,0);
        }

        [Test()]
        public void TestUserInRole()
        {
            Guid appId = Guid.Parse (appGuid);
            User[] users = repository.GetAppUsers(appId);
            User user = repository.GetUser(users[0].Id);

            Assert.IsTrue(user.InRole ("test_test"));
        }

        [Test()]
        public void TestUserGetClaims ()
        {
            Guid appId = Guid.Parse (appGuid);
            User[] users = repository.GetAppUsers(appId);
            User user = repository.GetUser(users[0].Id);

            string[] claims = user.GetClaims ();

            Assert.IsNotNull (claims);
        }
    }
}

