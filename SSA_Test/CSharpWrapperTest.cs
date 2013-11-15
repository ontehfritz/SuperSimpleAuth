using NUnit.Framework;
using System;
using SuperSimple.Auth;
using System.Net;
using System.IO;

//Csharp API wrapper tests
using System.Runtime;
using System.Collections.Generic;

////////////////////////////
// Note: you need to run: xsp4 in the SuperSimpleAuth folder before running these test
// The wrapper needs a uri to connect that is running 
/// ////////////////////////

namespace SSA_Test
{
    [TestFixture()]
    public class CSharpWrapperTest
    {
        //string app = "test";
        //string appKey = "900de06b-c0d3-4430-842e-b1512bf32b24";
        private SuperSimple.Auth.Api.MongoRepository _api = 
            new SuperSimple.Auth.Api.MongoRepository("mongodb://localhost");
        SSAManager.IRepository mRepository = new SSAManager.MongoRepository ("mongodb://localhost");
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
            ssa = new SuperSimpleAuth (_app.Name, _app.Key.ToString());
        }

        [TearDown] 
        public void Dispose()
        { 
            if (_app != null) {
                mRepository.DeleteApp (_app.Name, _manager.Id);
            }

            if (_manager != null) {
                mRepository.DeleteManager (_manager.Id);
            }
        }

        [Test()]
        public void TestCreateUser ()
        {
            User user = ssa.CreateUser("test_api_wrapper", "test1");
            Assert.AreEqual ("test_api_wrapper", user.UserName);
        }

        [Test()]
        public void TestAuthenticate ()
        {
            User user = ssa.Authenticate("test1", "test1");
            Assert.AreEqual ("test1", user.UserName);
        }

        [Test()]
        public void TestFailedAuthenticate ()
        {
            AuthenticationFailedException fail = null;

            User user = null;

            try
            {
                user = ssa.Authenticate("test", "test");
                Assert.IsNull(user);
            }
            catch(AuthenticationFailedException e) {
               
                fail = e;
            }

            Assert.IsNotNull (fail);
        }

        [Test()]
        public void TestValidate ()
        {
            User user = ssa.Authenticate("tester", "test");
            User validUser = ssa.Validate (user.AuthToken);
            Assert.IsNotNull (validUser);
        }

        [Test()]
        public void TestValidateFailed ()
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
        public void TestEnd ()
        {
            User user = ssa.Authenticate("tester", "test");
            bool end = ssa.End (user.AuthToken);

            Assert.AreEqual (true, end);
        }
    }
}

