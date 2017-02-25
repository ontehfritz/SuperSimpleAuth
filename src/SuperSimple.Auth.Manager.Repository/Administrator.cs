namespace SuperSimple.Auth.Manager.Repository
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    public class Administrator
    {
        [BsonId]
        public Guid Id              { get; set; }
        public Guid ManagerId       { get; set; }
        public Guid DomainId        { get; set; }
        public DateTime CreatedAt   { get; set; }
    }
}

