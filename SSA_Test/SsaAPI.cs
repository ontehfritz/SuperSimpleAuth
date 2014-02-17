using System;
using NUnit.Framework;
using SuperSimple.Auth.Api;

//API Test
using System.Collections.Generic;


namespace SSA_Test
{
    [TestFixture()]
    public class SsaAPI
    {
        IRepository repository = new MongoRepository ("mongodb://localhost");
        SSAManager.IRepository mRepository = new SSAManager.MongoRepository ("mongodb://localhost");
        private SSAManager.Domain _domain;
        private SSAManager.Manager _manager;
       

        [SetUp()]
        public void Init()
        {
            SSAManager.Manager manager = new SSAManager.Manager ();
            manager.UserName = "manager@test.ing";
            manager.Secret = "test";
            manager = mRepository.CreateManager (manager);
            _manager = manager;

            SSAManager.Domain domain = mRepository.CreateDomain ("test", _manager);
            domain.Claims = new List<string> ();
            domain.Claims.Add ("test1");
            domain.Claims.Add ("test2");
            domain.WhiteListIps = new string[]{"127.0.0.1","127.1.1.1"};

            _domain = mRepository.UpdateDomain (domain);

            User apiOne = repository.CreateUser (_domain.Key, "test1", "test1", "test1@test1.com");
            User apiTwo = repository.CreateUser (_domain.Key, "test2", "test2", "test2@test2.com");

            SSAManager.User one = mRepository.GetUser (apiOne.Id);
            SSAManager.User two = mRepository.GetUser (apiTwo.Id);
            one.Claims = new List<string> ();
            two.Claims = new List<string> ();
            one.Claims.Add ("test1");
            two.Claims.Add ("test2");

            mRepository.UpdateUser(one);
            mRepository.UpdateUser(two);
        }

        [TearDown] 
        public void Dispose()
        { 
            if (_domain != null) 
            {
                mRepository.DeleteDomain (_domain.Name, _manager.Id);
            }

            if (_manager != null) 
            {
                mRepository.DeleteManager (_manager.Id,"test");
            }
        }

        [Test()]
        public void Disable_account()
        {
            User user = repository.Authenticate(_domain.Key,"test1","test1");
            Assert.IsTrue (repository.Disable(user.AuthToken, _domain.Key));
            user = repository.Authenticate(_domain.Key,"test1","test1");
            Assert.IsNull (user);
        }

        [Test()]
        public void Change_email()
        {
            User user = repository.Authenticate(_domain.Key,"test1","test1");
            Assert.IsTrue (repository.ChangeEmail (_domain.Key,user.AuthToken,"test3@test3.com"));
            user = repository.Authenticate(_domain.Key,"test1","test1");
            Assert.AreEqual ("test3@test3.com", user.Email);
        }
     
        [Test()]
        public void Change_username()
        {
            User user = repository.Authenticate(_domain.Key,"test1","test1");
            Assert.IsTrue (repository.ChangeUserName (_domain.Key,user.AuthToken,"test3"));
            user = repository.Authenticate(_domain.Key,"test3","test1");
            Assert.IsNotNull (user);
        }

        [Test()]
        public void Change_password()
        {
            User user = repository.Authenticate(_domain.Key,"test1","test1");
            Assert.IsTrue (repository.ChangePassword (_domain.Key,user.AuthToken,"test3"));
            user = repository.Authenticate(_domain.Key,"test1","test3");
            Assert.IsNotNull (user);
        }

        [Test()]
        public void Forgot_password()
        {
            string newPassword = repository.Forgot (_domain.Key, 
                "test1@test1.com");

            Assert.IsNotNull (newPassword);
        }

        [Test()]
        public void IpNotAllowed()
        {
            bool allowed = repository.IpAllowed (_domain.Key, "127.0.0.2");
            Assert.IsFalse (allowed);
        }

        [Test()]
        public void IpAllowed()
        {
            bool allowed = repository.IpAllowed (_domain.Key, "127.0.0.1");
            Assert.IsTrue (allowed);
        }

        [Test()]
        public void Does_username_exist()
        {
            bool exist = repository.UsernameExists (_domain.Key, "test1");
            Assert.IsTrue (exist);
        }

        [Test()]
        public void Does_email_exist()
        {
            bool exist = repository.EmailExists (_domain.Key, "test1@test1.com");
            Assert.IsTrue (exist);
        }


        [Test()]
        public void Create_a_user ()
        {
            User user = 
                repository.CreateUser (_domain.Key,
                    "testuser_api", "test", "testuser_api@test.com");

            Assert.IsNotNull (user);
        }

//        [Test()]
//        public void Update_a_users ()
//        {
//            User user = 
//                repository.CreateUser (_domain.Key,
//                    "testuser_api", "test", "testuser_api@test.com");
//
//            user = repository.Authenticate(_domain.Key,"testuser_api","test");
//            DateTime before = user.ModifiedAt;
//            user = repository.UpdateUser (_domain.Key,
//                                   user);
//
//            Assert.Greater (user.ModifiedAt,before);
//        }

        [Test()]
        public void Validate_key()
        {
            bool isValid = repository.ValidateDomainKey ("test", 
                _domain.Key);

            Assert.AreEqual (true, isValid);
        }

        [Test()]
        public void Authenticate_user()
        {
            User user = 
                repository.CreateUser (_domain.Key,
                    "testuser_api", "test", "testuser_api@test.com");

            user = repository.Authenticate (_domain.Key, "testuser_api", "test");
          
            Assert.IsNotNull (user);
        }

        [Test()]
        public void Authenticate_user_failure()
        {
            User user = repository.Authenticate (_domain.Key, "testuser_api", "t");

            Assert.IsNull (user);
        }

        [Test()]
        public void Validate_user()
        {
            User user = 
                repository.CreateUser (_domain.Key,
                    "testuser_api", "test", "testuser_api@test.com");

            user = repository.Authenticate (_domain.Key, "testuser_api", "test");
            user = repository.Validate (user.AuthToken, _domain.Key);

            Assert.IsNotNull (user);
        }

        public void Validate_user_fail()
        {
            User user = repository.Authenticate (_domain.Key, "testuser_api", "test");
            User validateUser = repository.Validate (user.AuthToken, _domain.Key);

            Assert.IsNull (validateUser);
        }

        [Test()]
        public void End_session ()
        {
            User user = 
                repository.CreateUser (_domain.Key,
                    "testuser_api", "test", "testuser_api@test.com");

            user = repository.Authenticate (_domain.Key, "testuser_api", "test");
            bool done = repository.End (_domain.Key, user.AuthToken);

            Assert.AreEqual (true, done);
        }
    }
}

