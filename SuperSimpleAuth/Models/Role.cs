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
        public Guid DomainId { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public string[] Claims { get; set; }
    }
}

