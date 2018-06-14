using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.CurriculumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class CurriculumController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ObjectId emptyId = new ObjectId("000000000000000000000000");

        public CurriculumController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        // GET: Curriculum
        [Authorize(Roles = "USER")]
        public ActionResult Index()
        {
            var user = this.userManager.GetUserAsync(HttpContext.User).Result;

            var curriculumId = this.mongoContext.Users.Find(u => u.Id.Equals(user.ActorId)).First().Curriculum;

            if (curriculumId.Equals(this.emptyId))
            {
                TempData["Empty"] = "true";
                return View();
            }

            TempData["Empty"] = "false";

            var curriculum = this.mongoContext.Curriculums.Find(c => c.Id.Equals(curriculumId)).First();

            return View(curriculum);
        }

        [Authorize(Roles = "USER, ENTERPRISE")]
        public ActionResult Details(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var curriculum = this.mongoContext.Curriculums.Find(c => c.Id.Equals(objectId)).First();

            return View(curriculum);
        }

        // GET: Curriculum/Create
        [Authorize(Roles = "USER")]
        public ActionResult Create()
        {
            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

            if (!user.Curriculum.Equals(this.emptyId))
            {
                TempData["ErrorMessage"] = "You have already a curriculum.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: Curriculum/Create
        [Authorize(Roles = "USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCurriculumViewModel collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                    var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

                    if (!user.Curriculum.Equals(this.emptyId))
                    {
                        TempData["Empty"] = "false";
                        TempData["ErrorMessage"] = "You have already a curriculum.";
                        return RedirectToAction(nameof(Index));
                    }

                    Curriculum curriculum = new Curriculum
                    {
                        Id = ObjectId.GenerateNewId(),
                        Name = collection.Name,
                        Surname = collection.Surname,
                        Titles = collection.Titles.Split(","),
                        Knowledges = collection.Knowledges.Split(",")
                    };

                    this.mongoContext.Curriculums.InsertOne(curriculum);

                    UpdateDefinition<User> updateDefinition = Builders<User>.Update.Set(u => u.Curriculum, curriculum.Id);
                    this.mongoContext.Users.FindOneAndUpdate(u => u.Id.Equals(user.Id), updateDefinition);

                    TempData["Empty"] = "false";

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            return View(collection);
        }

        // GET: Curriculum/Edit/5
        [Authorize(Roles = "USER")]
        public ActionResult Edit(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

            if (user.Curriculum.Equals(this.emptyId) || !user.Curriculum.Equals(objectId))
            {
                TempData["ErrorMessage"] = "You can't edit this curriculum.";
                return RedirectToAction(nameof(Index));
            }

            var curriculum = this.mongoContext.Curriculums.Find(c => c.Id.Equals(objectId)).First();

            var eCurriculum = new CreateCurriculumViewModel
            {
                Name = curriculum.Name,
                Surname = curriculum.Surname,
                Titles = string.Join(",", curriculum.Titles.ToArray()),
                Knowledges = string.Join(",", curriculum.Knowledges.ToArray())
            };

            return View(eCurriculum);
        }

        // POST: Curriculum/Edit/5
        [Authorize(Roles = "USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, CreateCurriculumViewModel collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here

                    ObjectId objectId = new ObjectId(id);

                    var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                    var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

                    if (user.Curriculum.Equals(this.emptyId) || !user.Curriculum.Equals(objectId))
                    {
                        TempData["ErrorMessage"] = "You can't edit this curriculum.";
                        return RedirectToAction(nameof(Index));
                    }

                    UpdateDefinition<Curriculum> update = Builders<Curriculum>.Update.Set(c => c.Name, collection.Name)
                        .Set(c => c.Surname, collection.Surname)
                        .Set(c => c.Titles, collection.Titles.Split(","))
                        .Set(c => c.Knowledges, collection.Knowledges.Split(","));

                    this.mongoContext.Curriculums.FindOneAndUpdate(c => c.Id.Equals(objectId), update);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: Curriculum/Delete/5
        [Authorize(Roles = "USER")]
        public ActionResult Delete(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

            if (!user.Curriculum.Equals(objectId))
            {
                TempData["ErrorMessage"] = "This curriculum it's not yours";
                return View(nameof(Index));
            }
            this.mongoContext.Curriculums.FindOneAndDelete(c => c.Id.Equals(objectId));

            UpdateDefinition<User> update = Builders<User>.Update.Set(u => u.Curriculum, this.emptyId)
                .Set(u => u.Subscribes, new List<ObjectId>());
            this.mongoContext.Users.FindOneAndUpdate(u => u.Id.Equals(user.Id), update);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public ActionResult Curriculums(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var enterprise = this.mongoContext.Enterprises.Find(e => e.Id.Equals(applicationUser.ActorId)).First();

            if (!enterprise.Jobs.Contains(objectId))
            {
                TempData["Error"] = "Yoy don't have access to this offer.";
                return RedirectToAction(nameof(Index));
            }

            var job = this.mongoContext.Jobs.Find(j => j.Id.Equals(objectId)).First();

            var filter = new FilterDefinitionBuilder<Curriculum>().In(x => x.Id, job.Curriculums);

            var curriculums = this.mongoContext.Curriculums.Find(filter).ToEnumerable();

            TempData["Enterprise"] = id;

            return View(curriculums);
        }
    }
}