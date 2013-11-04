using NUnit.Framework;
using System;
using SuperSimple.Auth;
using System.Net;
using System.IO;

//Csharp API wrapper tests
using System.Runtime;


namespace SSA_Test
{
    [TestFixture()]
    public class CSharpWrapperTest
    {
        string app = "test";
        string appKey = "900de06b-c0d3-4430-842e-b1512bf32b24";

        [Test()]
        public void TestCreateUser ()
        {
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth (app, appKey);
           
            User user = ssa.CreateUser("tester", "test");
            Assert.AreEqual ("tester", user.UserName);
        }

        [Test()]
        public void TestAuthenticate ()
        {
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth (app, appKey);



            User user = ssa.Authenticate("tester", "test");
            Assert.AreEqual ("tester", user.UserName);
        }

        [Test()]
        public void TestFailedAuthenticate ()
        {
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth (app, appKey);

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
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth (app, appKey);

            User user = ssa.Authenticate("tester", "test");
            User validUser = ssa.Validate (user.AuthToken);
            Assert.IsNotNull (validUser);
        }

        [Test()]
        public void TestValidateFailed ()
        {
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth (app, appKey);

            InvalidTokenException fail = null;

            try
            {
                User validUser = ssa.Validate (Guid.Parse(appKey));
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
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth (app, appKey);

            User user = ssa.Authenticate("tester", "test");
            bool end = ssa.End (user.AuthToken);

            Assert.AreEqual (true, end);
        }
    }
}

