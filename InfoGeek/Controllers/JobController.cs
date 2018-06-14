using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.JobViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class JobController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ObjectId emptyId = new ObjectId("000000000000000000000000");

        public JobController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        // GET: Job
        [Authorize(Roles = "USER")]
        public ActionResult Index()
        {
            var jobs = this.mongoContext.Jobs.AsQueryable().ToEnumerable();

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            return View(jobs);
        }

        [Authorize(Roles = "ENTERPRISE")]
        public ActionResult MyJobs()
        {
            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var enterpise = this.mongoContext.Enterprises.Find(e => e.Id.Equals(applicationUser.ActorId)).First();

            var filter = new FilterDefinitionBuilder<Job>().In(x => x.Id, enterpise.Jobs);

            var jobs = this.mongoContext.Jobs.Find(filter).ToEnumerable();

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            return View(jobs);
        }

        [Authorize(Roles = "USER")]
        public ActionResult Jobs(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var enterprise = this.mongoContext.Enterprises.Find(e => e.Id.Equals(objectId)).First();

            var filter = new FilterDefinitionBuilder<Job>().In(j => j.Id, enterprise.Jobs);

            var jobs = this.mongoContext.Jobs.Find(filter).ToEnumerable();

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            return View("MyJobs", jobs);
        }

        // GET: Job/Details/5
        [HttpGet("job/details/{id}")]
        public ActionResult Details(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var job = this.mongoContext.Jobs.Find(f => f.Id.Equals(objectId)).First();

            var actor = this.userManager.GetUserAsync(HttpContext.User).Result;

            var user = this.mongoContext.Users.Find(u => u.Id.Equals(actor.ActorId)).First();

            TempData["Subscribed"] = user.Subscribes.Contains(objectId) || user.Curriculum.Equals(emptyId);

            return View(job);
        }

        [HttpGet("job/enterprise/details/{id}")]
        public ActionResult DetailsEnterprise(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var job = this.mongoContext.Jobs.Find(f => f.Id.Equals(objectId)).First();

            TempData["Subscribed"] = true;

            return View("Details", job);
        }

        // GET: Job/Create
        [Authorize(Roles = "ENTERPRISE")]
        public ActionResult Create()
        {
            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            return View();
        }

        // POST: Job/Create
        [Authorize(Roles = "ENTERPRISE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobViewModel jobViewModel)
        {
            IEnumerable<Category> categories;
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                    var user = this.mongoContext.Enterprises.Find(f => f.Id.Equals(applicationUser.ActorId)).First();

                    ObjectId objectId = new ObjectId(jobViewModel.Category);

                    var category = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();

                    Job job = new Job
                    {
                        Id = ObjectId.GenerateNewId(),
                        Title = jobViewModel.Title,
                        Description = jobViewModel.Description,
                        Salary = jobViewModel.Salary,
                        Month = jobViewModel.Month,
                        Date = DateTime.Now,
                        Tags = jobViewModel.Tags.Split(","),
                        Category = category,
                        Curriculums = new List<ObjectId>()
                    };
                    user.Jobs.Add(job.Id);

                    this.mongoContext.Jobs.InsertOne(job);

                    UpdateDefinition<Enterprise> updateDefinition = Builders<Enterprise>.Update.Set("Jobs", user.Jobs);

                    this.mongoContext.Enterprises.FindOneAndUpdate(f => f.Id.Equals(user.Id), updateDefinition);

                    return RedirectToAction(nameof(MyJobs));
                }
                catch
                {
                    categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
                    TempData["Categories"] = categories;

                    return View();
                }
            }
            categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;
            return View();
        }

        // GET: Job/Edit/5
        [Authorize(Roles = "ENTERPRISE")]
        public ActionResult Edit(string id)
        {
            var actor = this.userManager.GetUserAsync(HttpContext.User).Result;
            var enterprise = this.mongoContext.Enterprises.Find(u => u.Id.Equals(actor.ActorId)).First();

            ObjectId objectId = new ObjectId(id);

            if (!enterprise.Jobs.Contains(objectId))
            {
                TempData["ErrorMessage"] = "You can't do this action.";
                return RedirectToAction(nameof(Index));
            }

            var job = this.mongoContext.Jobs.Find(j => j.Id.Equals(objectId)).First();

            JobViewModel jobViewModel = new JobViewModel
            {
                Title = job.Title,
                Description = job.Description,
                Salary = job.Salary,
                Tags = string.Join(",", job.Tags.ToArray()),
                Category = job.Category.Id.ToString(),
                Month = job.Month
            };

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            return View(jobViewModel);
        }

        // POST: Job/Edit/5
        [Authorize(Roles = "ENTERPRISE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, JobViewModel jobViewModel)
        {
            IEnumerable<Category> categories;
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here

                    var actor = this.userManager.GetUserAsync(HttpContext.User).Result;
                    var enterprise = this.mongoContext.Enterprises.Find(u => u.Id.Equals(actor.ActorId)).First();

                    ObjectId objectId = new ObjectId(id);
                    ObjectId categoryId = new ObjectId(jobViewModel.Category);

                    if (!enterprise.Jobs.Contains(objectId))
                    {
                        TempData["ErrorMessage"] = "You can't do this action.";
                        return RedirectToAction(nameof(Index));
                    }

                    var category = this.mongoContext.Categories.Find(f => f.Id.Equals(categoryId)).First();

                    UpdateDefinition<Job> updateDefinition = Builders<Job>.Update.Set("Title", jobViewModel.Title)
                        .Set(j => j.Description, jobViewModel.Description)
                        .Set(j => j.Salary, jobViewModel.Salary)
                        .Set(j => j.Tags, jobViewModel.Tags.Split(","))
                        .Set(j => j.Category, category)
                        .Set(j => j.Month, jobViewModel.Month);

                    this.mongoContext.Jobs.FindOneAndUpdate(j => j.Id.Equals(objectId), updateDefinition);

                    return RedirectToAction(nameof(MyJobs));
                }
                catch
                {
                    categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
                    TempData["Categories"] = categories;

                    return View(jobViewModel);
                }
            }
            categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;
            return View(jobViewModel);
        }

        // GET: Job/Delete/5
        [Authorize(Roles = "ENTERPRISE")]
        public ActionResult Delete(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var enterprise = this.mongoContext.Enterprises.Find(e => e.Id.Equals(applicationUser.ActorId)).First();

            if (!enterprise.Jobs.Contains(objectId))
            {
                TempData["ErrorMessage"] = "You can't do this action.";
                return RedirectToAction(nameof(Index));
            }
            enterprise.Jobs.Remove(objectId);

            UpdateDefinition<Enterprise> update = Builders<Enterprise>.Update.Set(e => e.Jobs, enterprise.Jobs);
            this.mongoContext.Enterprises.FindOneAndUpdate(e => e.Id.Equals(objectId), update);

            this.mongoContext.Jobs.FindOneAndDelete(j => j.Id.Equals(objectId));

            return RedirectToAction(nameof(MyJobs));
        }

        [Authorize(Roles = "USER")]
        public ActionResult Subscribe(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

            if (user.Subscribes.Contains(objectId))
            {
                TempData["ErrorMessage"] = "You are subscribed to this offer.";
                return RedirectToAction(nameof(Index));
            }
            else if (user.Curriculum.Equals(emptyId))
            {
                TempData["ErrorMessage"] = "You must have a curriculum to subscribe to this offer.";
                return RedirectToAction(nameof(Index));
            }

            var job = this.mongoContext.Jobs.Find(j => j.Id.Equals(objectId)).First();
            job.Curriculums.Add(user.Curriculum);

            UpdateDefinition<Job> update = Builders<Job>.Update.Set(j => j.Curriculums, job.Curriculums);
            this.mongoContext.Jobs.FindOneAndUpdate(j => j.Id.Equals(objectId), update);

            user.Subscribes.Add(objectId);

            UpdateDefinition<User> updateDefinition = Builders<User>.Update.Set(u => u.Subscribes, user.Subscribes);
            this.mongoContext.Users.FindOneAndUpdate(u => u.Id.Equals(user.Id), updateDefinition);

            TempData["Success"] = "Your request is send to the enterprise.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "USER")]
        public ActionResult Filter(string id)
        {
            IEnumerable<Job> jobs;

            ObjectId objectId = new ObjectId(id);
            var category = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();
            

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;

            if (category.Name.Equals("CATEGORY"))
            {
                jobs = this.mongoContext.Jobs.AsQueryable().ToEnumerable();

                return View("MyJobs", jobs);
            }

            jobs = this.mongoContext.Jobs.Find(j => j.Category.Id.Equals(objectId)).ToEnumerable();

            return View("Index", jobs);
        }

        [Authorize(Roles = "USER")]
        public ActionResult Search(string key)
        {
            IEnumerable<Job> jobs;
            
            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;

            if (string.IsNullOrEmpty(key))
            {
                jobs = this.mongoContext.Jobs.AsQueryable().ToEnumerable();

                return View("Index", jobs);
            }

            jobs = this.mongoContext.Jobs.Find(j => j.Title.ToUpper().Contains(key.ToUpper()) || j.Description.ToUpper().Contains(key.ToUpper())).ToEnumerable();

            return View("Index", jobs);
        }

        [Authorize(Roles = "ENTERPRISE")]
        public ActionResult FilterEnterprise(string id)
        {
            FilterDefinition<Job> filter;
            IEnumerable<Job> jobs;

            var actor = this.userManager.GetUserAsync(HttpContext.User).Result;
            var enterprise = this.mongoContext.Enterprises.Find(e => e.Id.Equals(actor.ActorId)).First();

            ObjectId objectId = new ObjectId(id);
            var category = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;

            if (category.Name.Equals("CATEGORY"))
            {
                filter = new FilterDefinitionBuilder<Job>().In(x => x.Id, enterprise.Jobs);
                jobs = this.mongoContext.Jobs.Find(filter).ToEnumerable();

                return View("MyJobs", jobs);
            }

            filter = new FilterDefinitionBuilder<Job>().In(x => x.Id, enterprise.Jobs);
            jobs = this.mongoContext.Jobs.Find(filter).ToEnumerable().AsQueryable().Where(j => j.Category.Id.Equals(objectId)).AsEnumerable();

            

            return View("MyJobs", jobs);
        }

        [Authorize(Roles = "ENTERPRISE")]
        public ActionResult SearchEnterprise(string key)
        {
            FilterDefinition<Job> filter;
            IEnumerable<Job> jobs;

            var actor = this.userManager.GetUserAsync(HttpContext.User).Result;
            var enterprise = this.mongoContext.Enterprises.Find(e => e.Id.Equals(actor.ActorId)).First();

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;

            if (string.IsNullOrEmpty(key))
            {
                filter = new FilterDefinitionBuilder<Job>().In(x => x.Id, enterprise.Jobs);
                jobs = this.mongoContext.Jobs.Find(filter).ToEnumerable();

                return View("MyJobs", jobs);
            }

            filter = new FilterDefinitionBuilder<Job>().In(x => x.Id, enterprise.Jobs);
            jobs = this.mongoContext.Jobs.Find(filter)
                .ToEnumerable().Where(j => j.Title.ToUpper().Contains(key.ToUpper()) || j.Description.ToUpper()
                .Contains(key.ToUpper()))
                .AsEnumerable();

            return View("MyJobs", jobs);
        }
    }
}