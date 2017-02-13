namespace SuperSimple.Auth.Manager.Repository
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using FluentValidation;
    using System.Collections.Generic;
    using Api.Repository;
    using SuperSimple.Auth.Api;

    public class Domain
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement]
        public string Name { get; set; }

        [BsonElement]
        public Guid Key { get; set; }

        [BsonElement]
        public Guid Salt { get; set; }

        [BsonElement]
        public Guid ManagerId { get; set; }

        [BsonElement]
        public string[] WhiteListIps { get; set; }

        [BsonElement]
        public List<string> Claims { get; set; }

        [BsonElement]
        public bool Enabled { get; set; }

        [BsonElement]
        public DateTime CreatedAt { get; set; }

        [BsonElement]
        public DateTime ModifiedAt { get; set; }

        [BsonElement]
        public string ModifiedBy { get; set; } 

        public int UserCount { get; set; }

        public bool IsOwner(IUser manager)
        {
            if(manager.Id == this.ManagerId)
            {
                return true;
            }

            return false;
        }
            

    }

    public class DomainValidator : AbstractValidator<Domain>
    {
        public DomainValidator()
        {
            RuleFor(domain => domain.Name).NotEmpty();
        }
    }
}

