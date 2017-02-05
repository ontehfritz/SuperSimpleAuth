using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace SuperSimple.Auth.Api
{
	public class User
	{
        [BsonId]
		public Guid Id { get; set; }
        [BsonElement]
        public Guid DomainId { get; set;}
        [BsonElement]
        public string UserName { get; set;}
        [BsonElement]
        public string Email { get; set;}
        [BsonElement]
        public string Secret { get; set;}
        [BsonElement]
        public bool Enabled { get; set; }
        [BsonElement]
        public Guid AuthToken { get; set; }
        [BsonElement]
        public string[] Claims { get; set; }
        [BsonElement]
        public Role[] Roles { get; set; }
        [BsonElement]
        public string CurrentIp { get; set; }
        [BsonElement]
        public string LastIp { get; set; }
        [BsonElement]
        public DateTime ?LastRequest { get; set; }
        [BsonElement]
        public DateTime ?CurrentLogon { get; set; }
        [BsonElement]
        public DateTime ?LastLogon { get; set; }
        [BsonElement]
        public int LogonCount { get; set; }
        [BsonElement]
        public DateTime CreatedAt { get; set; }
        [BsonElement]
        public DateTime ModifiedAt { get; set; }


        public string[] GetRoles()
        {
            List<string> roles = new List<string> ();

            if (this.Roles != null) 
            {
                foreach (Role role in this.Roles) 
                {
                    roles.Add (role.Name);
                }
            }

            return roles.ToArray ();
        }

        
        public string[] GetClaims()
        {
            List<string> claims = new List<string>();

            if (this.Claims != null) 
            {
                claims.AddRange (this.Claims);
            }

            if (this.Roles != null) {

                claims = new List<string> ();
                foreach (Role role in this.Roles) 
                {
                    if (role.Claims != null) 
                    {
                        claims.AddRange (role.Claims);
                    }
                }

                return claims.ToArray ();
            }

            return claims.ToArray();
        }

        public bool InRole(string role)
        {
            if (Roles == null) 
            {
                return false;
            }

            foreach(Role r in Roles)
            {
                if (r.Name == role) 
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasClaim(string claim)
        {
            string[] claims = this.GetClaims ();

            if (claims == null) 
            {
                return false;
            }

            foreach(string c in claims)
            {
                if (c == claim) 
                {
                    return true;
                }
            }

            return false;
        }
	}
}

