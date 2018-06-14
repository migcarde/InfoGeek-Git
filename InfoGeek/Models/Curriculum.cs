using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Curriculum
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        [BsonRequired]
        public string Name { get; set; }

        [BsonElement("Surname")]
        [BsonRequired]
        public string Surname { get; set; }

        [BsonElement("Titles")]
        public ICollection<string> Titles { get; set; }

        [BsonElement("Knowledges")]
        public ICollection<string> Knowledges { get; set; }
    }
}
