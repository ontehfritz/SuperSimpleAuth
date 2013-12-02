using NUnit.Framework;
using System;
using SuperSimple.Auth;
using System.Net;
using System.IO;

//Csharp API wrapper tests
using System.Runtime;
using System.Collections.Generic;


///////////////////////////////////////////////////////////////////////////////////////
// Note: you need to run: xsp4 in the SuperSimpleAuth sub folder before running these test
// this will run the SSA Api Command: 
// xsp4 --port 8082
// ssl: xsp4 --https --port 4433 --p12file hostname.p12 --pkpwd password
// see: http://www.mono-project.com/UsingClientCertificatesWithXSP 
/// ///////////////////////// ////////////////////////////////////////////////////////

namespace SSA_Test
{
    [TestFixture()]
    public class CSharpWrapperTest
    {
        private SuperSimple.Auth.Api.MongoRepository _api = 
            new SuperSimple.Auth.Api.MongoRepository("mongodb://localhost");

        private SSAManager.IRepository mRepository = 
            new SSAManager.MongoRepository ("mongodb://localhost");

        private SSAManager.Domain _domain;
        private SSAManager.Manager _manager;
        private SuperSimpleAuth ssa;
       

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

            //domain.WhiteListIps = new string[]{"127.0.0.2","127.1.1.1"};

            _domain = mRepository.UpdateDomain (domain);

            SuperSimple.Auth.Api.User apiOne = _api.CreateUser (_domain.Key, "test1", "test1");
            SuperSimple.Auth.Api.User apiTwo = _api.CreateUser (_domain.Key, "test2", "test2");

            SSAManager.User one = mRepository.GetUser (apiOne.Id);
            SSAManager.User two = mRepository.GetUser (apiTwo.Id);
            one.Claims = new List<string> ();
            two.Claims = new List<string> ();
            one.Claims.Add ("test1");
            two.Claims.Add ("test2");

            mRepository.UpdateUser(one);
            mRepository.UpdateUser(two);
            ssa = new SuperSimpleAuth (_domain.Name, _domain.Key.ToString(), "http://127.0.0.1:8082");
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
                mRepository.DeleteManager (_manager.Id);
            }
        }

        [Test()]
        public void Change_user_password ()
        {
            User user = ssa.Authenticate("test1", "test1");
            Assert.IsTrue (ssa.ChangePassword (user.AuthToken,"test3"));
        }

        [Test()]
        public void Check_for_duplicate_email ()
        {
            ssa.CreateUser("test_api_wrapper", "test1",
                "test_api_wrapper@test.com");

            User user2 = null;

            try
            {
               user2 = ssa.CreateUser("test_api_wrapper1", "test1",
                "test_api_wrapper@test.com");
            }
            catch(Exception e) {
                Console.WriteLine (e.Message);
            }

            Assert.IsNull (user2);
        }


        [Test()]
        public void Create_a_user ()
        {
            User user = ssa.CreateUser("test_api_wrapper", "test1"/*,
                "test_api_wrapper@test.com"*/);
            Assert.AreEqual ("test_api_wrapper", user.UserName);
        }

        [Test()]
        public void Authenticate ()
        {
            User user = ssa.Authenticate("test1", "test1");
            Assert.AreEqual ("test1", user.UserName);
        }

        [Test()]
        public void Authenticate_failed ()
        {
            AuthenticationFailedException fail = null;

            User user = null;

            try
            {
                user = ssa.Authenticate("test", "test");
                Assert.IsNull(user);
            }
            catch(AuthenticationFailedException e) 
            {
               
                fail = e;
            }

            Assert.IsNotNull (fail);
        }

        [Test()]
        public void Validate ()
        {
            User user = ssa.Authenticate("test1", "test1");
            User validUser = ssa.Validate (user.AuthToken);
            Assert.IsNotNull (validUser);
        }

        [Test()]
        public void Validate_failed ()
        {
            InvalidTokenException fail = null;

            try
            {
                User validUser = ssa.Validate (_domain.Key);
                Assert.IsNull(validUser);
            }
            catch(InvalidTokenException e)
            {
                fail = e;
            }

            Assert.IsNotNull (fail);
        }

        [Test()]
        public void End ()
        {
            User user = ssa.Authenticate("test1", "test1");
            bool end = ssa.End (user.AuthToken);

            //Assert.AreEqual (true, end);
        }
    }
}

