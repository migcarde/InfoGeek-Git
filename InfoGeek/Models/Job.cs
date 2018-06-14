using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Job
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Title")]
        [BsonRequired]
        public string Title { get; set; }

        [BsonElement("Description")]
        [BsonRequired]
        public string Description { get; set; }

        [BsonElement("Salary")]
        [Range(0, Double.MaxValue)]
        public double Salary { get; set; }

        [BsonElement("Month")]
        [BsonRequired]
        public bool Month { get; set; }

        [BsonElement("Date")]
        [BsonRequired]
        public DateTime Date { get; set; }

        [BsonElement("Tags")]
        [BsonRequired]
        public ICollection<String> Tags { get; set; }

        [BsonElement("Category")]
        [BsonRequired]
        public Category Category { get; set; }

        [BsonElement("Curriculums")]
        [BsonRequired]
        public ICollection<ObjectId> Curriculums { get; set; } 
    }
}
