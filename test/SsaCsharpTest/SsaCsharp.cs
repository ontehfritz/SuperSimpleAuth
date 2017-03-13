namespace SsaCsharpTest
{
    using System;
    using NUnit.Framework;
    using SuperSimple.Auth;

    [TestFixture ()]
    public class SsaCsharp
    {
        private const string LOCAL_URL = "http://localhost:8082";
        private const string LOCAL_KEY = "9fa95fd4-04e5-4cea-8dca-3a818f525a91";

        private const string URL = "https://api.authenticate.technology";
        private const string KEY = "894b3a57-6311-436e-97c7-d4e1984fa94b";


        [Test ()]
        public void CreateUser ()
        {
            //var api = new SuperSimpleAuth("test",
            //                              LOCAL_KEY,
            //                              LOCAL_URL);

            //var user = api.CreateUser("test", "test", "test@test.net");
            //user = api.Authenticate("test", "test");
            //Assert.IsNotNull(user);
        }

        [Test()]
        public void Authenticate()
        {
            var api = new SuperSimpleAuth("test",
                                          LOCAL_KEY,
                                          LOCAL_URL);

            api.CreateUser("test", "test", "test@test.net");

            var user = api.Authenticate("test", "test");
            Assert.True(!string.IsNullOrEmpty(user.Jwt));

            var valid = api.Validate(user);
            Assert.True(valid);
            user.Email = "test@test1.com";

            user = api.ChangeEmail(user, user.Email);
          

            user.UserName = "mutha";
            user = api.ChangeUserName(user, user.UserName);

            valid = api.Disable(user);

            Assert.True(valid);
        }
    }
}
