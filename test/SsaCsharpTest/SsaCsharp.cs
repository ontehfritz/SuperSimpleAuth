namespace SsaCsharpTest
{
    using NUnit.Framework;
    using System;
    using SuperSimple.Auth;

    [TestFixture ()]
    public class SsaCsharp
    {
        private const string LOCAL_URL = "http://localhost:8082";
        private const string LOCAL_KEY = "2fc74c26-9de9-431b-bc42-58b9ceb1e89f";

        private const string URL = "https://api.authenticate.technology";
        private const string KEY = "894b3a57-6311-436e-97c7-d4e1984fa94b";


        [Test ()]
        public void CreateUser ()
        {
            var api = new SuperSimpleAuth("test",
                                          LOCAL_KEY,
                                          LOCAL_URL);

            var user = api.CreateUser("test", "test", "test@test.net");
            Assert.IsNotNull(user);
        }

        [Test()]
        public void Authenticate()
        {

            var api = new SuperSimpleAuth("test",
                                          LOCAL_KEY,
                                          LOCAL_URL);

            var user = api.Authenticate("test", "test");
            Assert.True(Guid.Empty != user.AuthToken);
        }
    }
}
