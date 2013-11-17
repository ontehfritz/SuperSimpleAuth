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

        private SSAManager.App _app;
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

            SSAManager.App app = mRepository.CreateApp ("test", _manager);
            app.Claims = new List<string> ();
            app.Claims.Add ("test1");
            app.Claims.Add ("test2");

            _app = mRepository.UpdateApp (app);

            SuperSimple.Auth.Api.User apiOne = _api.CreateUser (_app.Key, "test1", "test1");
            SuperSimple.Auth.Api.User apiTwo = _api.CreateUser (_app.Key, "test2", "test2");

            SSAManager.User one = mRepository.GetUser (apiOne.Id);
            SSAManager.User two = mRepository.GetUser (apiTwo.Id);
            one.Claims = new List<string> ();
            two.Claims = new List<string> ();
            one.Claims.Add ("test1");
            two.Claims.Add ("test2");

            mRepository.UpdateUser(one);
            mRepository.UpdateUser(two);
            ssa = new SuperSimpleAuth (_app.Name, _app.Key.ToString(), "http://127.0.0.1:8082");
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
            User user = ssa.CreateUser("test_api_wrapper", "test1");
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
                User validUser = ssa.Validate (_app.Key);
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

            Assert.AreEqual (true, end);
        }
    }
}

