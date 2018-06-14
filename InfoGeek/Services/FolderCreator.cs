using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using MongoDB.Bson;

namespace InfoGeek.Services
{
    public class FolderCreator : IFolderCreator
    {
        private readonly MongoContext mongoContext;

        public FolderCreator(MongoContext mongoContext)
        {
            this.mongoContext = mongoContext;
        }

        public Task CreateFolderAsync(ApplicationUser applicationUser)
        {
            List<Folder> result = new List<Folder>();

            Folder inbox = new Folder
            {
                Name = "Inbox",
                Messages = new List<ObjectId>(),
                Principal = true
            };
            mongoContext.Folders.InsertOne(inbox);

            Folder outbox = new Folder
            {
                Name = "Outbox",
                Messages = new List<ObjectId>(),
                Principal = true
            };
            mongoContext.Folders.InsertOne(outbox);

            Folder trash = new Folder
            {
                Name = "Trash",
                Messages = new List<ObjectId>(),
                Principal = true
            };
            mongoContext.Folders.InsertOne(trash);


            return Task.CompletedTask;
        }

        /*public Task CreateFolderAsync(Enterprise applicationUser)
        {
            List<Folder> result = new List<Folder>();

            Folder inbox = new Folder
            {
                Name = "Inbox",
                Messages = new List<Message>(),
                User = applicationUser.NormalizedEmail,
                Principal = true
            };
            mongoContext.Folders.InsertOne(inbox);

            Folder outbox = new Folder
            {
                Name = "Outbox",
                Messages = new List<Message>(),
                User = applicationUser.NormalizedEmail,
                Principal = true
            };
            mongoContext.Folders.InsertOne(outbox);

            Folder trash = new Folder
            {
                Name = "Trash",
                Messages = new List<Message>(),
                User = applicationUser.NormalizedEmail,
                Principal = true
            };
            mongoContext.Folders.InsertOne(trash);
            

            return Task.CompletedTask;
        }*/
    }
}
