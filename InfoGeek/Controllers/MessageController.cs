using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.MessageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class MessageController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;

        public MessageController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        // GET: Message
        [Authorize]
        public ActionResult Index(string id)
        {
            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            ObjectId objectId = new ObjectId(id);

            if (!user.Folders.Contains(objectId))
            {
                TempData["ErrorMessage"] = "No puedes visualizar estos mensajes.";
                return RedirectToAction(nameof(Index));
            }

            var folder = this.mongoContext.Folders.Find(f => f.Id.Equals(objectId)).First();

            var filter = new FilterDefinitionBuilder<Message>().In(x => x.Id, folder.Messages);
            var messages = this.mongoContext.Messages.Find(filter).ToEnumerable();

            return View(messages);
        }

        // GET: Message/Details/5
        [Authorize]
        public ActionResult Details(string folderId, string messageId)
        {
            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            ObjectId objectIdFolder = new ObjectId(folderId);

            ObjectId objectIdMessage = new ObjectId(messageId);

            var folder = this.mongoContext.Folders.Find(f => f.Id.Equals(objectIdFolder)).First();

            if (!user.Folders.Contains(objectIdFolder) || !folder.Messages.Contains(objectIdMessage))
            {
                TempData["ErrorMessage"] = "No puedes visualizar este mensaje.";
                return RedirectToAction(nameof(Index));
            }

            var message = this.mongoContext.Messages.Find(f => f.Id.Equals(objectIdMessage)).First();

            return View(message);
        }

        // GET: Message/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Message/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MessageViewModel messageViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here

                    var actorFrom = this.userManager.GetUserAsync(HttpContext.User).Result;
                    var filterFrom = new FilterDefinitionBuilder<Folder>().In(x => x.Id, actorFrom.Folders);
                    var foldersFrom = this.mongoContext.Folders.Find(filterFrom).ToEnumerable();
                    var folderFrom = foldersFrom.Where(f => f.Name.Equals("Outbox")).First();

                    var actorTo = this.mongoContext.ApplicationUsers.Find(a => a.NormalizedEmail.Equals(messageViewModel.ActorTo.ToUpper())).First();
                    var filterTo = new FilterDefinitionBuilder<Folder>().In(x => x.Id, actorTo.Folders);
                    var foldersTo = this.mongoContext.Folders.Find(filterTo).ToEnumerable();
                    var folderTo = foldersTo.Where(f => f.Name.Equals("Inbox")).First();

                    if (actorFrom.NormalizedEmail.Equals(messageViewModel.ActorTo))
                    {
                        TempData["ErrorMessage"] = "No puedes enviar un mensaje a ti mismo.";
                        return RedirectToAction(nameof(Index));
                    }

                    Message message = new Message
                    {
                        Id = ObjectId.GenerateNewId(),
                        ActorFrom = actorFrom.UserName,
                        ActorTo = messageViewModel.ActorTo,
                        Body = messageViewModel.Body,
                        DeliveryDate = DateTime.Now,
                        Subject = messageViewModel.Subject
                    };
                    folderFrom.Messages.Add(message.Id);
                    folderTo.Messages.Add(message.Id);

                    this.mongoContext.Messages.InsertOne(message);
                    UpdateDefinition<Folder> updateDefinition = Builders<Folder>.Update.Set("Messages", folderFrom.Messages);
                    mongoContext.Folders.FindOneAndUpdate(f => f.Id.Equals(folderFrom.Id), updateDefinition);

                    UpdateDefinition<Folder> updateDefinition2 = Builders<Folder>.Update.Set("Messages", folderTo.Messages);
                    mongoContext.Folders.FindOneAndUpdate(f => f.Id.Equals(folderTo.Id), updateDefinition2);

                    this.mongoContext.Folders.FindOneAndUpdate(f => f.Id.Equals(folderFrom.Id), updateDefinition);
                    this.mongoContext.Folders.FindOneAndUpdate(f => f.Id.Equals(folderTo.Id), updateDefinition2);

                    return RedirectToAction("Index", "Folder");
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: Message/Delete/5
        [Authorize]
        public ActionResult Delete(string folderId, string messageId)
        {
            try
            {
                ObjectId fId = new ObjectId(folderId);
                ObjectId mId = new ObjectId(messageId);

                var user = this.userManager.GetUserAsync(HttpContext.User).Result;

                var folder = this.mongoContext.Folders.Find(f => f.Id.Equals(fId)).First();

                if (!user.Folders.Contains(fId) || !folder.Messages.Contains(mId))
                {
                    TempData["ErrorMessage"] = "No puedes borrar este mensaje.";
                    return RedirectToAction(nameof(Index));
                }

                

                var message = folder.Messages.AsQueryable().Where(m => m.Equals(mId)).First();

                folder.Messages.Remove(message);

                UpdateDefinition<Folder> updateDefinition = Builders<Folder>.Update.Set("Messages", folder.Messages);

                this.mongoContext.Folders.FindOneAndUpdate(f => f.Id.Equals(fId), updateDefinition);

                return RedirectToAction("Index", "Folder");
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            
        }
    }
}