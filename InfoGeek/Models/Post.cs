using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Post
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Description")]
        [BsonRequired]
        public string Description { get; set; }

        [BsonElement("Date")]
        [BsonRequired]
        public DateTime Date { get; set; }

        [BsonElement("Photo")]
        [Url]
        public string Photo { get; set; }

        [BsonElement("Link")]
        [Url]
        public string Url { get; set; }

        [BsonElement("Writer")]
        [BsonRequired]
        public string Writer { get; set; }

        [BsonElement("Principal")]
        [BsonRequired]
        public bool Principal { get; set; }

        [BsonRequired]
        [BsonElement("Replies")]
        public ICollection<ObjectId> Replies { get; set; }
    }
}
