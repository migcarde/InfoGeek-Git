using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Request
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Message")]
        [BsonRequired]
        public string Message { get; set; }

        [BsonElement("Job")]
        public Job Job { get; set; }
    }
}
