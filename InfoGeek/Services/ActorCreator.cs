using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Services
{
    public class ActorCreator : IActorCreator
    {
        private readonly MongoContext mongoContext;

        public ActorCreator(MongoContext mongoContext)
        {
            this.mongoContext = mongoContext;
        }

        public Task CreateUserAsync(ApplicationUser applicationUser)
        {
            User user = new User
            {
                Id = ObjectId.GenerateNewId(),
                Posts = new List<ObjectId>(),
                Subscribes = new List<ObjectId>(),
            };

            UpdateDefinition<ApplicationUser> updateDefinition = Builders<ApplicationUser>.Update.Set("ActorId", user.Id);
            this.mongoContext.ApplicationUsers.FindOneAndUpdateAsync(u => u.NormalizedEmail.Equals(applicationUser.NormalizedEmail), updateDefinition);

            this.mongoContext.Users.InsertOne(user);

            return Task.CompletedTask;
        }

        public Task CreateEnterpiseAsync(ApplicationUser applicationUser, RegisterEnterpriseViewModel model)
        {
            Enterprise enterprise = new Enterprise
            {
                Id = ObjectId.GenerateNewId(),
                Name = model.Name,
                Description = model.Description,
                Address = model.Address,
                Photo = model.Photo,
                Jobs = new List<ObjectId>(),
            };

            UpdateDefinition<ApplicationUser> updateDefinition = Builders<ApplicationUser>.Update.Set("ActorId", enterprise.Id);
            this.mongoContext.ApplicationUsers.FindOneAndUpdateAsync(u => u.NormalizedEmail.Equals(applicationUser.NormalizedEmail), updateDefinition);

            this.mongoContext.Enterprises.InsertOne(enterprise);

            return Task.CompletedTask;
        }

        public Task CreateSponsorAsync(ApplicationUser applicationUser)
        {
            Sponsor sponsor = new Sponsor
            {
                Id = ObjectId.GenerateNewId(),
                SponsorShips = new List<ObjectId>(),
            };

            UpdateDefinition<ApplicationUser> updateDefinition = Builders<ApplicationUser>.Update.Set("ActorId", sponsor.Id);
            this.mongoContext.ApplicationUsers.FindOneAndUpdateAsync(u => u.NormalizedEmail.Equals(applicationUser.NormalizedEmail), updateDefinition);

            this.mongoContext.Sponsors.InsertOne(sponsor);

            return Task.CompletedTask;
        }

        public List<ObjectId> CreateFolders()
        {
            List<ObjectId> result = new List<ObjectId>();

            Folder inbox = new Folder
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Inbox",
                Messages = new List<ObjectId>(),
                Principal = true
            };
            mongoContext.Folders.InsertOne(inbox);
            result.Add(inbox.Id);

            Folder outbox = new Folder
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Outbox",
                Messages = new List<ObjectId>(),
                Principal = true
            };
            mongoContext.Folders.InsertOne(outbox);
            result.Add(outbox.Id);

            Folder trash = new Folder
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Trash",
                Messages = new List<ObjectId>(),
                Principal = true
            };
            mongoContext.Folders.InsertOne(trash);
            result.Add(trash.Id);

            return result;
        }
    }
}
