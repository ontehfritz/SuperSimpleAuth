namespace SuperSimple.Auth.Api.Token
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Newtonsoft.Json;
    /// <summary>
    /// Jwt token. https://scotch.io/tutorials/the-anatomy-of-a-json-web-token
    /// </summary>
    public class JwtToken
    {
        public Header Header        { get; }
        public Payload Payload      { get; }
        public string Secret        { get; }

        public JwtToken (Header header, Payload payload, string secret)
        {
            Header = header;
            Payload = payload;
            Secret = secret;
        }

        // great replacement for microsofts urlencode that is pretty shit 
        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

               
        // great replacement for microsofts urldecode that is pretty shit 
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        public static string ToToken(JwtToken jwttoken)
        {
            var header =
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(jwttoken.Header));
            var payload = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(jwttoken.Payload));
          
            var token =
                string.Format("{0}.{1}.{2}",
                              Base64UrlEncode(header),
                              Base64UrlEncode(payload),
                              HMACSHA256(jwttoken.Header,jwttoken.Payload,
                                         jwttoken.Secret));

            return token;
        }

        public static JwtToken Validate(string token, string secret)
        {
            var splittoken = token.Split('.');

            if(splittoken.Length != 3){ return null; }

            var header = JsonConvert.DeserializeObject<Header>(
                Encoding.UTF8.GetString(
                    Encoding.Unicode.GetBytes(splittoken[0]))); 
            var payload = JsonConvert.DeserializeObject<Payload>(
                Encoding.UTF8.GetString(
                    Encoding.Unicode.GetBytes(splittoken[1]))); 

            var hash = HMACSHA256(header, payload, secret);
            if(hash != splittoken[2]) { return null; }
            var tokenObject = new JwtToken(header, payload, secret);

            return tokenObject;
        }

        private static string HMACSHA256(Header header, Payload payload,
                                         string secret)
        {
            var hash = string.Empty;
            var h = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(header));
            var p = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(payload));

          
            var bytes = Encoding.UTF8.GetBytes(Base64UrlEncode(h) + "." +
                                               Base64UrlEncode(p));

            using (HMACSHA256 hmac = 
                   new HMACSHA256(Encoding.Default.GetBytes(secret)))
            {
                hash = 
                    Base64UrlEncode(hmac.ComputeHash(bytes));

            }

            return hash;
        }
    }

    public class Header 
    {
        [JsonProperty(PropertyName = "alg")]
        public string Algorithm { get; set; }
        [JsonProperty(PropertyName = "typ")]
        public string Type { get; set; }
    }

    public class Payload
    {
        // ** registered jwt properites 
        [JsonProperty(PropertyName = "sub")]
        public string Subject       { get; set; }
        [JsonProperty(PropertyName = "iss")]
        public string Issuer        { get; set; }
        [JsonProperty(PropertyName = "aud")]
        public string Audience      { get; set; }
        [JsonProperty(PropertyName = "exp")]
        public int Expiration       { get; set; }
        [JsonProperty(PropertyName = "nbf")]
        public int NotBefore        { get; set; }
        [JsonProperty(PropertyName = "iat")]
        public int IssuedAt         { get; set; }
        [JsonProperty(PropertyName = "jti")]
        public string JwtTokenId   { get; set; }
        //****************************************

        //*** Public ***//
        [JsonProperty(PropertyName = "name")]
        public string Username      { get; set; }
        [JsonProperty(PropertyName = "mail")]
        public string Email         { get; set; }
    }
}
