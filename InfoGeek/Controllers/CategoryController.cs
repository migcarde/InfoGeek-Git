using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.CategoryViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class CategoryController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;

        public CategoryController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        [Authorize(Roles = "ADMIN")]
        // GET: Category
        public ActionResult Index()
        {
            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            return View(categories);
        }

        [Authorize(Roles = "ADMIN")]
        // GET: Category/Details/5
        public ActionResult Details(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var category = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();

            return View(category);
        }

        [Authorize(Roles = "ADMIN")]
        // GET: Category/Create
        public ActionResult Create()
        {
            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            return View();
        }

        // POST: Category/Create
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryViewModel categoryViewModel)
        {
            IEnumerable<Category> categories;
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here

                    ObjectId objectId = new ObjectId(categoryViewModel.Father);
                    /*var cat = this.mongoContext.Categories.Find(c => c.Name.Equals(categoryViewModel.Name)).First();

                    var aux = this.mongoContext.Categories.AsQueryable().ToEnumerable().Contains(cat);*/

                    var aux = this.mongoContext.Categories.Find(c => c.Name.Equals(categoryViewModel.Name)).FirstOrDefault();

                    if (this.mongoContext.Categories.Find(c => c.Name.Equals(categoryViewModel.Name)).FirstOrDefault() != null)
                    {
                        TempData["Error"] = "This category already exist.";
                        return RedirectToAction(nameof(Create));
                    }

                    var father = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();

                    Category category = new Category
                    {
                        Id = ObjectId.GenerateNewId(),
                        Name = categoryViewModel.Name,
                        Father = objectId,
                        Sons = new List<ObjectId>()
                    };
                    father.Sons.Add(category.Id);

                    UpdateDefinition<Category> updateDefinition = Builders<Category>.Update.Set("Sons", father.Sons);
                    this.mongoContext.Categories.FindOneAndUpdate(c => c.Id.Equals(objectId), updateDefinition);

                    this.mongoContext.Categories.InsertOne(category);

                    TempData["Error"] = "";

                    return RedirectToAction(nameof(Index));
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
            return View(categoryViewModel);
        }

        // GET: Category/Edit/5
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var category = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();

            if (category.Name.Equals("CATEGORY"))
            {
                TempData["Error"] = "You can't edit this category.";
                return RedirectToAction(nameof(Index));
            }

            var categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();

            TempData["Categories"] = categories;

            CategoryViewModel categoryViewModel = new CategoryViewModel
            {
                Name = category.Name,
                Father = category.Father.ToString()
            };

            return View(categoryViewModel);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit(string id, CategoryViewModel collection)
        {
            IEnumerable<Category> categories;
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here

                    ObjectId objectId = new ObjectId(id);

                    if (this.mongoContext.Categories.Find(c => c.Name.Equals(collection.Name)).FirstOrDefault() != (null))
                    {
                        TempData["Error"] = "This category already exist.";
                        return RedirectToAction(nameof(Index));
                    }

                    ObjectId father = new ObjectId(collection.Father);

                    UpdateDefinition<Category> update = Builders<Category>.Update.Set(c => c.Name, collection.Name)
                        .Set(c => c.Father, father)
                        .Set(c => c.Sons, new List<ObjectId>());

                    this.mongoContext.Categories.FindOneAndUpdate(c => c.Id.Equals(objectId), update);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
                    TempData["Categories"] = categories;

                    return View(collection);
                }
            }
            categories = this.mongoContext.Categories.AsQueryable().ToEnumerable();
            TempData["Categories"] = categories;

            return View(collection);
        }

        // GET: Category/Delete/5
        [Authorize(Roles = "ADMIN")]
        public ActionResult Delete(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var category = this.mongoContext.Categories.Find(c => c.Id.Equals(objectId)).First();

            if (category.Name.Equals("CATEGORY"))
            {
                TempData["Error"] = "You can delete this category.";
                return RedirectToAction(nameof(Index));
            }
            var father = this.mongoContext.Categories.Find(c => c.Name.Equals("CATEGORY")).First();

            var update = Builders<Job>.Update.Set(j => j.Category, father);
            this.mongoContext.Jobs.UpdateMany(j => j.Category.Id.Equals(objectId), update);

            this.mongoContext.Categories.FindOneAndDelete(c => c.Id.Equals(objectId));

            return RedirectToAction(nameof(Index));
        }
    }
}