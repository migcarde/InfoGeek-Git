using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InfoGeek.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : MongoIdentityUser
    {
        public ObjectId ActorId { get; set; }

        public ICollection<ObjectId> Folders { get; set;}
    }
}
