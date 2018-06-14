using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("Curriculum")]
        public ObjectId Curriculum { get; set; }

        [BsonRequired]
        [BsonElement("Posts")]
        public ICollection<ObjectId> Posts { get; set; }

        [BsonRequired]
        [BsonElement("Subscribes")]
        public ICollection<ObjectId> Subscribes { get; set; }
    }
}
