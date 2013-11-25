using System;
using Nancy.Validation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FluentValidation;
using System.Collections.Generic;

namespace SSAManager
{
    public class App
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

        public User[] GetUsers()
        {
            List<User> user = new List<User> ();

            return user.ToArray ();
        }
    }

    public class AppValidator : AbstractValidator<App>
    {
        public AppValidator()
        {
            RuleFor(app => app.Name).NotEmpty();
        }
    }
}

