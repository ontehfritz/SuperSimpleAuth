namespace SuperSimple.Auth.Token.Test
{
    using NUnit.Framework;
    using System;
    using SuperSimple.Auth.Api.Token;

    [TestFixture ()]
    public class Test
    {
        [Test ()]
        public void JwtTokenTest ()
        {
            var header = new Header();
            header.Algorithm = "HS256";
            header.Type = "JWT";

            var payload = new Payload();
            payload.Issuer = "autheticate.technology";
            payload.Username = "John Doe";
            payload.Subject = "1234567890";

            var role1 = new Role();
            role1.Name = "role1";
            role1.Permissions.Add("perm1");
            role1.Permissions.Add("perm2");

            var role2 = new Role();
            role2.Name = "role2";
            role2.Permissions.Add("perm3");
            role2.Permissions.Add("perm4");

            payload.Roles.Add(role1);
            payload.Roles.Add(role2);

            var token = new Jwt(header,payload);

            var tokenString = Jwt.ToToken(token, "testing");
            Assert.True(!string.IsNullOrEmpty(tokenString));

            var valToken = Jwt.Validate(tokenString,"testing");
            Assert.True(valToken);
        }
    }
}
