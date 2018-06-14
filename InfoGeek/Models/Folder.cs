using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Folder
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        [BsonRequired]
        public string Name { get; set; }

        [BsonElement("Principal")]
        [BsonRequired]
        public bool Principal { get; set; }

        [BsonElement("Messages")]
        [BsonRequired]
        public ICollection<ObjectId> Messages { get; set; }

        /*[BsonElement("User")]
        [BsonRequired]
        public string User { get; set; }*/
    }
}
