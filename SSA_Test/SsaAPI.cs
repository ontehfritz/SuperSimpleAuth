using System;
using NUnit.Framework;
using SuperSimple.Auth.Api;

//API Test
namespace SSA_Test
{
    [TestFixture()]
    public class SsaAPI
    {
        IRepository repository = new MongoRepository ("mongodb://localhost");
        Guid appKey = Guid.Parse("900de06b-c0d3-4430-842e-b1512bf32b24");

        [Test()]
        public void TestCreateUsers ()
        {
            User user = 
                repository.CreateUser (appKey,
                                   "testuser_api", "test");

            Assert.IsNotNull (user);
        }

        [Test()]
        public void TestUpdateUsers ()
        {
            User user = repository.Authenticate(appKey,"testuser_api","test");
            DateTime before = user.ModifiedAt;
            user = repository.UpdateUser (appKey,
                                   user);

            Assert.Greater (user.ModifiedAt,before);
        }

        [Test()]
        public void TestRepositoryValidateKey()
        {
            bool isValid = repository.ValidateAppKey ("test", 
                                       appKey);

            Assert.AreEqual (true, isValid);
        }

        [Test()]
        public void TestAuthenticate()
        {
            User user = repository.Authenticate (appKey, "testuser_api", "test");

            Assert.IsNotNull (user);
        }

        [Test()]
        public void TestAuthenticateFailed()
        {
            User user = repository.Authenticate (appKey, "testuser_api", "t");

            Assert.IsNull (user);
        }

        [Test()]
        public void TestValidate()
        {
            User user = repository.Authenticate (appKey, "testuser_api", "test");
            User validateUser = repository.Validate (user.AuthToken, appKey);

            Assert.IsNotNull (validateUser);
        }

        public void TestValidateFail()
        {
            User user = repository.Authenticate (appKey, "testuser_api", "test");
            User validateUser = repository.Validate (user.AuthToken, appKey);

            Assert.IsNull (validateUser);
        }

        [Test()]
        public void TestEnd ()
        {
            User user = repository.Authenticate (appKey, "testuser_api", "test");
            bool done = repository.End (appKey,user.AuthToken);

            Assert.AreEqual (true, done);
        }
    }
}

