namespace SsaCsharpTest
{
    using NUnit.Framework;
    using SuperSimple.Auth;

    [TestFixture ()]
    public class SsaCsharp
    {
        private const string LOCAL_URL = "http://localhost:8082";
        private const string LOCAL_KEY = "59abff40-3b13-4e5f-be46-9cea815a8e0a";

        private const string URL = "https://api.authenticate.technology";
        private const string KEY = "30fb3a81-75b6-47dc-9da6-7a7016cae4b4";

        [Test()]
        public void Authenticate()
        {
            
            var api = new SuperSimpleAuth(KEY,
                                          URL);

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

            valid = api.Disable(user).Result;
            Assert.True(valid);
        }
    }
}
