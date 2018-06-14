using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Forms;
using InfoGeek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class FolderController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;

        public FolderController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        // GET: Folder
        [Authorize]
        public ActionResult Index()
        {
            var errMessage = TempData["ErrorMessage"] as string;

            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            var filter = new FilterDefinitionBuilder<Folder>().In(x => x.Id, user.Folders);
            var folders = this.mongoContext.Folders.Find(filter).ToEnumerable();

            return View(folders);
        }

        // GET: Folder/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            ObjectId objectId = new ObjectId(id);

            if (!user.Folders.Contains(objectId))
            {
                TempData["ErrorMessage"] = "No puedes visualizar esta carpeta.";
                return RedirectToAction(nameof(Index));
            }

            var folder = this.mongoContext.Folders.Find(f => f.Id.Equals(objectId)).First();

            return View(folder.Messages);
        }

        // GET: Folder/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Folder/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FolderForm folderForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here

                    var user = this.userManager.GetUserAsync(HttpContext.User).Result;

                    Folder folder = new Folder
                    {
                        Id = ObjectId.GenerateNewId(),
                        Name = folderForm.Name,
                        Messages = new List<ObjectId>(),
                        Principal = false
                    };
                    user.Folders.Add(folder.Id);

                    this.mongoContext.Folders.InsertOne(folder);

                    UpdateDefinition<ApplicationUser> updateDefinition = Builders<ApplicationUser>.Update.Set(a => a.Folders, user.Folders);

                    this.mongoContext.ApplicationUsers.FindOneAndUpdate(a => a.NormalizedEmail.Equals(user.NormalizedEmail), updateDefinition);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: Folder/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            ObjectId objectId = new ObjectId(id);

            var folder = this.mongoContext.Folders.Find(f => f.Id.Equals(objectId)).First();

            if (folder.Principal == true)
            {
                TempData["ErrorMessage"] = "Este carpeta no se puede modificar.";
                return RedirectToAction(nameof(Index));
            }
            else if (!user.Folders.Contains(objectId))
            {
                TempData["ErrorMessage"] = "usted no puede modificar esta carpeta.";
                return RedirectToAction(nameof(Index));
            }

            FolderForm folderForm = new FolderForm
            {
                Name = folder.Name
            };

            return View(folderForm);
        }

        // POST: Folder/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, FolderForm folderForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here

                    var user = this.userManager.GetUserAsync(HttpContext.User).Result;

                    ObjectId objectId = new ObjectId(id);

                    if (!user.Folders.Contains(objectId))
                    {
                        TempData["ErrorMessage"] = "usted no puede modificar esta carpeta.";
                        return RedirectToAction(nameof(Index));
                    }

                    UpdateDefinition<Folder> updateDefinition = Builders<Folder>.Update.Set("Name", folderForm.Name);

                    this.mongoContext.Folders.FindOneAndUpdate(f => f.Id.Equals(objectId), updateDefinition);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(folderForm);
                }
            }
            return View(folderForm);
        }

        // GET: Folder/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            var folder = this.mongoContext.Folders.Find(f => f.Id.Equals(objectId)).First();

            if (folder.Principal == true)
            {
                TempData["ErrorMessage"] = "Este carpeta no se puede borrar.";
                return RedirectToAction(nameof(Index));
            }
            else if (!user.Folders.Contains(objectId))
            {
                TempData["ErrorMessage"] = "usted no puede borrar esta carpeta.";
                return RedirectToAction(nameof(Index));
            }

            var filter = new FilterDefinitionBuilder<Message>().In(x => x.Id, folder.Messages);
            this.mongoContext.Messages.FindOneAndDelete(filter);

            this.mongoContext.Folders.FindOneAndDelete(f => f.Id.Equals(objectId));

            user.Folders.Remove(objectId);

            UpdateDefinition<ApplicationUser> updateDefinition = Builders<ApplicationUser>.Update.Set(a => a.Folders, user.Folders);

            return RedirectToAction(nameof(Index));
        }
    }
}