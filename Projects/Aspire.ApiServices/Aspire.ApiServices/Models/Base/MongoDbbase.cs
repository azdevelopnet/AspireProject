using System;
using MongoAttribute = MongoDB.Bson.Serialization.Attributes;

namespace Aspire.ApiServices.Models
{
    public class MongoDbbase
    {
        [MongoAttribute.BsonId]
        [MongoAttribute.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
