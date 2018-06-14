using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Category
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        [BsonRequired]
        public string Name { get; set; }

        [BsonElement("Sons")]
        [BsonRequired]
        public ICollection<ObjectId> Sons { get; set; }

        [BsonElement("Father")]
        public ObjectId Father { get; set; }
    }
}
