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
        private IRepository repository = new MongoRepository ("mongodb://localhost");
        private App _app;
        private Manager _manager;
        private SuperSimple.Auth.Api.MongoRepository _api = 
            new SuperSimple.Auth.Api.MongoRepository("mongodb://localhost");

        [SetUp()]
        public void Init()
        {
            Manager manager = new Manager ();
            manager.UserName = "manager@test.ing";
            manager.Secret = "test";
            manager = repository.CreateManager (manager);
            _manager = manager;

            App app = repository.CreateApp ("test", _manager);
            _app = app;

            _api.CreateUser (_app.Key, "test1", "test1");
            _api.CreateUser (_app.Key, "test2", "test2");

           

        }

        [TearDown] 
        public void Dispose()
        { 
            if (_app != null) {
                repository.DeleteApp (_app.Name, _manager.Id);
            }

            if (_manager != null) {
                repository.DeleteManager (_manager.Id);
            }
        }
       
        [Test()]
        public void Create_manager()
        {
            Manager manager = new Manager ();
            manager.UserName = "create@manager.test";
            manager.Secret = "test";
            manager = repository.CreateManager (manager);
            Assert.IsNotNull (manager.Id);
            repository.DeleteManager (manager.Id);
        }
       
        [Test()]
        public void Create_app()
        {
            App app = repository.CreateApp ("create_app_test", _manager);
            Assert.IsNotNull (app);
            repository.DeleteApp (app.Name, _manager.Id);
        }

        [Test()]
        public void Get_manager()
        {
            Manager manager = repository.GetManager ("manager@test.ing");
            Assert.AreEqual ("manager@test.ing", manager.UserName);
        }

        [Test()]
        public void Get_app()
        {
            App app = repository.GetApp(_app.Name, 
                                        _manager.Id);

            Assert.AreEqual (_app.Id, app.Id);
        }

        [Test()]
        public void Get_a_user ()
        {
            User[] users = repository.GetAppUsers(_app.Id);
            User user = repository.GetUser(users[0].Id);
            Assert.IsNotNull (user);
        }

        [Test()]
        public void Update_a_user ()
        {
            User[] users = repository.GetAppUsers(_app.Id);
            User user = repository.GetUser(users[0].Id);
            user.Roles = user.Roles;
            user = repository.UpdateUser (user);

            Assert.IsNotNull (user);
        }
       
        [Test()]
        public void Get_all_users_for_application ()
        {
            User[] users = repository.GetAppUsers(_app.Id);
            Assert.Greater (users.Length,0);
        }



        [Test()]
        public void Create_a_role()
        {
            Role r = repository.CreateRole(_app.Id, "test_test");
            Assert.AreEqual ("test_test",r.Name);
        }


        [Test()]
        public void Get_a_role()
        {
            Role role = repository.CreateRole (_app.Id, "test_test");
            role = repository.GetRole (_app.Id, "test_test");
            Assert.IsNotNull (role);
        }

        [Test()]
        public void Get_all_roles()
        {
            Role[] roles = repository.GetRoles (_app.Id);
            Assert.IsNotNull (roles);
        }

        [Test()]
        public void Update_a_role()
        {
            Role role = repository.CreateRole (_app.Id, "test_test");

            List<string> claims = new List<string>();
            claims.Add ("Write");
            claims.Add ("Read");

            role.Claims = claims;
            repository.UpdateRole (role);

            role = repository.GetRole (_app.Id, "test_test");

            Assert.IsNotNull (role.Claims);
        }

        [Test()]
        public void Delete_a_role()
        {
            Role role = repository.CreateRole (_app.Id, "test_test");

            repository.DeleteRole (role);

            role = repository.GetRole (_app.Id, "test_test");

            Assert.IsNull (role);
        }

        [Test()]
        public void Get_users_in_role ()
        {
            Role role = repository.CreateRole (_app.Id, "test_test");

            User[] users = repository.GetAppUsers(_app.Id);

            foreach (User u in users) {

                if (u.Roles == null) {
                    u.Roles = new List<Role> ();
                }

                u.Roles.Add (role);
                repository.UpdateUser (u);
            }
          
            users = repository.GetUsersInRole(role);
            Assert.Greater (users.Length,0);
        }

        [Test()]
        public void Is_user_in_role()
        {
            Role role = repository.CreateRole (_app.Id, "test_test");

            User[] users = repository.GetAppUsers(_app.Id);
            User user = repository.GetUser(users[0].Id);

            if (user.Roles == null) {
                user.Roles = new List<Role> ();
                user.Roles.Add (role);
                repository.UpdateUser (user);
            }

            Assert.IsTrue(user.InRole ("test_test"));
        }

        [Test()]
        public void Get_user_claims ()
        {
            User[] users = repository.GetAppUsers(_app.Id);
            User user = repository.GetUser(users[0].Id);

            string[] claims = user.GetClaims ();

            Assert.IsNotNull (claims);
        }

        [Test()]
        public void Delete_app()
        {
            repository.DeleteApp (_app.Name,_manager.Id);
            _app = null;
        }

        [Test()]
        public void Delete_manager()
        {
            repository.DeleteManager (_manager.Id);
            repository.DeleteApp (_app.Name,_manager.Id);
            _app = null;
            _manager = null;
        }
    }
}

