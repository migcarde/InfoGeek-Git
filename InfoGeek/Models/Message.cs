using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Subject")]
        [BsonRequired]
        public string Subject { get; set; }

        [BsonElement("Body")]
        [BsonRequired]
        public string Body { get; set; }

        [BsonElement("DeliveryDate")]
        [BsonRequired]
        public DateTime DeliveryDate { get; set; }

        [BsonElement("ActorFrom")]
        [BsonRequired]
        public string ActorFrom { get; set; }

        [BsonElement("ActorTo")]
        [BsonRequired]
        public string ActorTo { get; set; }
    }
}
