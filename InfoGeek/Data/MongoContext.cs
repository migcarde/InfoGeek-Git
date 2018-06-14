using AspNetCore.Identity.MongoDbCore.Models;
using InfoGeek.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Data
{
    public class MongoContext
    {
        private MongoClient _client { get; set; }

        //Getters and setters of collections
        public IMongoCollection<Post> Posts { get; set; }
        public IMongoCollection<ApplicationUser> ApplicationUsers { get; set; }
        public IMongoCollection<Folder> Folders { get; set; }
        public IMongoCollection<Message> Messages { get; set; }
        public IMongoCollection<Job> Jobs { get; set; }
        public IMongoCollection<Curriculum> Curriculums { get; set; }
        public IMongoCollection<Models.Category> Tags { get; set; }
        public IMongoCollection<Enterprise> Enterprises { get; set; }
        public IMongoCollection<User> Users { get; set; }
        public IMongoCollection<Category> Categories { get; set; }
        public IMongoCollection<Sponsor> Sponsors { get; set; }
        public IMongoCollection<SponsorShip> SponsorShips { get; set; }
        public IMongoCollection<MongoIdentityRole> Roles { get; set; }
        public IMongoCollection<Admin> Admins { get; set; }
        public BsonDocument Stats { get; set; }

        public MongoContext(string databaseConnectionString, string databaseName)
        {
            _client = new MongoClient(databaseConnectionString);

            IMongoDatabase mongoDatabase = _client.GetDatabase(databaseName);

            //Collections of database
            Posts = mongoDatabase.GetCollection<Post>("posts");
            ApplicationUsers = mongoDatabase.GetCollection<ApplicationUser>("applicationUsers");
            Folders = mongoDatabase.GetCollection<Folder>("folders");
            Messages = mongoDatabase.GetCollection<Message>("messages");
            Jobs = mongoDatabase.GetCollection<Job>("jobs");
            Curriculums = mongoDatabase.GetCollection<Curriculum>("curriculum");
            Tags = mongoDatabase.GetCollection<Models.Category>("tags");
            Enterprises = mongoDatabase.GetCollection<Enterprise>("enterprise");
            Users = mongoDatabase.GetCollection<User>("user");
            Categories = mongoDatabase.GetCollection<Category>("category");
            Sponsors = mongoDatabase.GetCollection<Sponsor>("sponsor");
            SponsorShips = mongoDatabase.GetCollection<SponsorShip>("sponsorship");
            Roles = mongoDatabase.GetCollection<MongoIdentityRole>("mongoIdentityRoles");
            Admins = mongoDatabase.GetCollection<Admin>("admin");
            Stats = mongoDatabase.RunCommand<BsonDocument>("{dbStats: 1}");
        }
    }
}
