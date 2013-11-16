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
        private SSAManager.App _app;
        private SSAManager.Manager _manager;
       

        [SetUp()]
        public void Init()
        {
            SSAManager.Manager manager = new SSAManager.Manager ();
            manager.UserName = "manager@test.ing";
            manager.Secret = "test";
            manager = mRepository.CreateManager (manager);
            _manager = manager;

            SSAManager.App app = mRepository.CreateApp ("test", _manager);
            app.Claims = new List<string> ();
            app.Claims.Add ("test1");
            app.Claims.Add ("test2");

            _app = mRepository.UpdateApp (app);

            User apiOne = repository.CreateUser (_app.Key, "test1", "test1");
            User apiTwo = repository.CreateUser (_app.Key, "test2", "test2");

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
            if (_app != null) 
            {
                mRepository.DeleteApp (_app.Name, _manager.Id);
            }

            if (_manager != null) 
            {
                mRepository.DeleteManager (_manager.Id);
            }
        }

        [Test()]
        public void Create_a_user ()
        {
            User user = 
                repository.CreateUser (_app.Key,
                                   "testuser_api", "test");

            Assert.IsNotNull (user);
        }

        [Test()]
        public void Update_a_users ()
        {
            User user = 
                repository.CreateUser (_app.Key,
                    "testuser_api", "test");

            user = repository.Authenticate(_app.Key,"testuser_api","test");
            DateTime before = user.ModifiedAt;
            user = repository.UpdateUser (_app.Key,
                                   user);

            Assert.Greater (user.ModifiedAt,before);
        }

        [Test()]
        public void Validate_key()
        {
            bool isValid = repository.ValidateAppKey ("test", 
                _app.Key);

            Assert.AreEqual (true, isValid);
        }

        [Test()]
        public void Authenticate_user()
        {
            User user = 
                repository.CreateUser (_app.Key,
                    "testuser_api", "test");

            user = repository.Authenticate (_app.Key, "testuser_api", "test");

            Assert.IsNotNull (user);
        }

        [Test()]
        public void Authenticate_user_failure()
        {
            User user = repository.Authenticate (_app.Key, "testuser_api", "t");

            Assert.IsNull (user);
        }

        [Test()]
        public void Validate_user()
        {
            User user = 
                repository.CreateUser (_app.Key,
                    "testuser_api", "test");

            user = repository.Authenticate (_app.Key, "testuser_api", "test");
            user = repository.Validate (user.AuthToken, _app.Key);

            Assert.IsNotNull (user);
        }

        public void Validate_user_fail()
        {
            User user = repository.Authenticate (_app.Key, "testuser_api", "test");
            User validateUser = repository.Validate (user.AuthToken, _app.Key);

            Assert.IsNull (validateUser);
        }

        [Test()]
        public void End_session ()
        {
            User user = 
                repository.CreateUser (_app.Key,
                    "testuser_api", "test");

            user = repository.Authenticate (_app.Key, "testuser_api", "test");
            bool done = repository.End (_app.Key, user.AuthToken);

            Assert.AreEqual (true, done);
        }
    }
}

