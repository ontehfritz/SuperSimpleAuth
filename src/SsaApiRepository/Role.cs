using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SuperSimple.Auth.Api
{
    public class Role
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public Guid DomainId { get; set; }
        [BsonElement]
        public List<string> Claims { get; set; }


        public bool HasClaim(string claim)
        {
            if (Claims == null) {
                return false;
            }

            foreach(string c in Claims)
            {
                if (c == claim) {
                    return true;
                }
            }

            return false;
        }
    }
}

