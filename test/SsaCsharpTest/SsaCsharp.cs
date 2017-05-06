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

        [Test()]
        public void Authenticate()
        {
            var api = new SuperSimpleAuth(LOCAL_KEY,
                                          LOCAL_URL);

            var created = api.CreateUser("test", "test", "test@test.net").Result;

            var user = api.Authenticate("test", "test").Result;

            Assert.True(!string.IsNullOrEmpty(user.Jwt));

            var valid = api.Validate(user).Result;
            Assert.True(valid);
            var u  = api.Validate(user.AuthToken).Result;
            Assert.IsNotNull(u);
            u = api.Validate (user.Jwt);
            Assert.IsNotNull(u);

            user.Email = "test@test1.com";
            user = api.ChangeEmail(user, user.Email).Result;

            user.UserName = "mutha";
            user = api.ChangeUserName(user, user.UserName).Result;

            valid = api.ChangePassword(user, "pazzwurd").Result;
            Assert.True(valid);

            var newpassword = api.Forgot(user.Email).Result;
            Assert.True(!string.IsNullOrEmpty(newpassword));

            valid = api.End(user).Result;
            Assert.True(valid);
            valid = api.Validate(user).Result;
            Assert.False(valid);

            user = api.Authenticate(user.UserName, newpassword).Result;

            Assert.IsNotNull(user);

            valid = api.Disable(user);
            Assert.True(valid);
        }
    }
}
