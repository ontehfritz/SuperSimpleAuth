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
        private Domain  _domain;
        private Manager _manager;
        private Manager _admin;
   
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

            Manager admin = new Manager ();
            admin.UserName = "admin@test.ing";
            admin.Secret = "test";
            admin = repository.CreateManager (admin);
            _admin = admin;

            Domain domain = repository.CreateDomain ("test", _manager);
            domain.Claims = new List<string> ();
            domain.Claims.Add ("test1");
            domain.Claims.Add ("test2");

            _domain = repository.UpdateDomain (domain);

            SuperSimple.Auth.Api.User apiOne = _api.CreateUser (_domain.Key, "test1", "test1");
            SuperSimple.Auth.Api.User apiTwo =  _api.CreateUser (_domain.Key, "test2", "test2");

            User one = repository.GetUser (apiOne.Id);
            User two = repository.GetUser (apiTwo.Id);
            one.Claims = new List<string> ();
            two.Claims = new List<string> ();
            one.Claims.Add ("test1");
            two.Claims.Add ("test2");

            repository.UpdateUser(one);
            repository.UpdateUser(two);
        }

        [TearDown] 
        public void Dispose()
        { 
            if (_domain != null) 
            {
                repository.DeleteDomain (_domain.Name, _manager.Id);
            }

            if (_manager != null) 
            {
                repository.DeleteManager (_manager.Id, "test");
            }

            if (_admin != null) 
            {
                repository.DeleteManager (_admin.Id, "test");
            }
        }

        [Test()]
        public void Forgot_password()
        {
            string newPassword = repository.ForgotPassword ("manager@test.ing");
            Assert.IsNotNull (newPassword);
            repository.ChangePassword (_manager.Id, newPassword, "test", "test");
        }

        [Test()]
        public void Change_password()
        {
            repository.ChangePassword (_manager.Id,"test", "test1", "test1");
            Manager manager = repository.GetManager(_manager.Id);
            Assert.AreEqual (Helpers.Hash (manager.Id.ToString(), 
                "test1"), manager.Secret);

            repository.ChangePassword (_manager.Id,"test1", "test", "test");
        }

        [Test()]
        public void Change_email()
        {
            repository.ChangeEmail (_manager.Id, "test", "manager@test.com");
            Manager manager = repository.GetManager(_manager.Id);
            Assert.AreEqual ("manager@test.com", manager.UserName);
        }

        [Test()]
        public void Delete_user()
        {
            repository.DeleteUser (_domain.Id, "test1");
            User user = repository.GetUser(_domain.Id, "test1");
            Assert.IsNull (user);
        }

        [Test()]
        public void Get_roles_with_claim()
        {
            Role role1 = repository.CreateRole (_domain.Id, "test1");
            Role role2 = repository.CreateRole (_domain.Id, "test2");

            role1.Claims = new List<string> ();
            role1.Claims.Add ("test1");

            role1 = repository.UpdateRole (role1);

            role2.Claims = new List<string> ();
            role2.Claims.Add ("test1");

            role2 = repository.UpdateRole (role2);

            Role[] roles = repository.GetRolesWithClaim (_domain.Id, "test1");

            Assert.Greater (roles.Length, 0);

        }

        [Test()]
        public void Create_manager()
        {
            Manager manager = new Manager ();
            manager.UserName = "create@manager.test";
            manager.Secret = "test";
            manager = repository.CreateManager (manager);
            Assert.IsNotNull (manager.Id);
            repository.DeleteManager (manager.Id, "test");
        }
       
        [Test()]
        public void Create_domain()
        {
            Domain domain = repository.CreateDomain ("create_domain_test", _manager);
            Assert.IsNotNull (domain);
            repository.DeleteDomain (domain.Name, _manager.Id);
        }

        [Test()]
        public void Get_manager()
        {
            Manager manager = repository.GetManager ("manager@test.ing");
            Assert.AreEqual ("manager@test.ing", manager.UserName);
        }

        [Test()]
        public void Get_manager_by_id()
        {
            Manager manager = repository.GetManager (_manager.Id);
            Assert.AreEqual ("manager@test.ing", manager.UserName);
        }

        [Test()]
        public void Get_domain()
        {
            Domain domain = repository.GetDomain(_domain.Name, 
                                        _manager.Id);

            Assert.AreEqual (_domain.Id, domain.Id);
        }

        [Test()]
        public void Get_domain_users ()
        {
            User[] users = repository.GetDomainUsers(_domain.Id);
            Assert.IsNotNull (users);
        }

        [Test()]
        public void Get_a_user_by_name ()
        {
            User user = repository.GetUser(_domain.Id, "test1");
            Assert.IsNotNull (user);
        }

        [Test()]
        public void Update_a_user ()
        {
            User[] users = repository.GetDomainUsers(_domain.Id);
            User user = repository.GetUser(users[0].Id);
            user.Roles = user.Roles;
            user = repository.UpdateUser (user);

            Assert.IsNotNull (user);
        }
       
        [Test()]
        public void Get_all_users_with_claim()
        {
            User[] users = repository.GetUsersWithClaim (_domain.Id, "test1");
            Assert.Greater(users.Length, 0);
        }

        [Test()]
        public void Get_all_users_for_domain ()
        {
            User[] users = repository.GetDomainUsers(_domain.Id);
            Assert.Greater (users.Length,0);
        }

        [Test()]
        public void Create_a_role()
        {
            Role r = repository.CreateRole(_domain.Id, "test_test");
            Assert.AreEqual ("test_test",r.Name);
        }


        [Test()]
        public void Get_a_role()
        {
            Role role = repository.CreateRole (_domain.Id, "test_test");
            role = repository.GetRole (_domain.Id, "test_test");
            Assert.IsNotNull (role);
        }

        [Test()]
        public void Get_all_roles()
        {
            Role[] roles = repository.GetRoles (_domain.Id);
            Assert.IsNotNull (roles);
        }

        [Test()]
        public void Update_a_role()
        {
            Role role = repository.CreateRole (_domain.Id, "test_test");

            List<string> claims = new List<string>();
            claims.Add ("Write");
            claims.Add ("Read");

            role.Claims = claims;
            repository.UpdateRole (role);

            role = repository.GetRole (_domain.Id, "test_test");

            Assert.IsNotNull (role.Claims);
        }

        [Test()]
        public void Delete_a_role()
        {
            Role role = repository.CreateRole (_domain.Id, "test_test");

            repository.DeleteRole (role);

            role = repository.GetRole (_domain.Id, "test_test");

            Assert.IsNull (role);
        }

        [Test()]
        public void Get_users_in_role ()
        {
            Role role = repository.CreateRole (_domain.Id, "test_test");

            User[] users = repository.GetDomainUsers(_domain.Id);

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
            Role role = repository.CreateRole (_domain.Id, "test_test");

            User[] users = repository.GetDomainUsers(_domain.Id);
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
            User[] users = repository.GetDomainUsers(_domain.Id);
            User user = repository.GetUser(users[0].Id);

            string[] claims = user.GetClaims ();

            Assert.IsNotNull (claims);
        }

        [Test()]
        public void Delete_domain()
        {
            repository.DeleteDomain (_domain.Name,_manager.Id);
            _domain = null;
        }

        [Test()]
        public void Delete_manager()
        {
            repository.DeleteManager (_manager.Id, "test");
            _domain = null;
            _manager = null;
        }

        [Test()]
        public void Add_admin()
        {
            Manager admin = 
                repository.AddAdministrator(_domain.Id, "admin@test.ing");
            Assert.NotNull(admin);
            repository.DeleteAdministrator(_domain.Id,_admin.Id);
        }

        [Test()]
        public void Get_domains_by_admin()
        {
            Manager admin = 
                repository.AddAdministrator(_domain.Id, "admin@test.ing");

            Domain [] domains = repository.GetDomainsAdmin(admin.Id);

            Assert.Greater(domains.Length, 0);

            repository.DeleteAdministrator(_domain.Id,_admin.Id);
        }

        [Test()]
        public void Delete_admin()
        {
            Manager admin = 
                repository.AddAdministrator(_domain.Id, "admin@test.ing");
                
            repository.DeleteAdministrator(_domain.Id, admin.Id);
        }

        [Test()]
        public void Get_admins()
        {
            Manager admin = 
                repository.AddAdministrator(_domain.Id, "admin@test.ing");

            Manager [] admins = 
                repository.GetAdministrators(_domain.Id);

            Assert.Greater(admins.Length, 0);

            repository.DeleteAdministrator(_domain.Id, _admin.Id);
        }
    }
}

