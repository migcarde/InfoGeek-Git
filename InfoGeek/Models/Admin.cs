using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class Admin
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
