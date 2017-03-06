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
            payload.admin = true;
            payload.name = "John Doe";
            payload.sub = "1234567890";

            var token = new JwtToken(header,payload, "testing");

            var tokenString = JwtToken.ToToken(token);

            Assert.True(!string.IsNullOrEmpty(tokenString));
        }
    }
}
