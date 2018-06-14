using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class SponsorShip
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("Banner")]
        [Url]
        public string Banner { get; set; }

        [BsonRequired]
        [BsonElement("CreditCard")]
        public CreditCard CreditCard { get; set; }
    }
}
