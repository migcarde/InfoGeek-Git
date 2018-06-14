using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfoGeek.Models
{
    public class Enterprise
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonRequired]
        [BsonElement("Address")]
        public string Address { get; set; }

        [BsonRequired]
        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonRequired]
        [Url]
        [BsonElement("Photo")]
        public string Photo { get; set; }

        [BsonRequired]
        [BsonElement("Jobs")]
        public ICollection<ObjectId> Jobs { get; set; }
    }
}
