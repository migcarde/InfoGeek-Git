using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class AdminController : Controller
    {
        public readonly MongoContext _mongoContext;

        public AdminController(MongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        public ActionResult Dashboard()
        {
            var stats = this._mongoContext.Stats.ToDictionary();

            //------------Data from database----------------
            ViewData["avgObjSize"] = stats["avgObjSize"];
            ViewData["dataSize"] = stats["dataSize"];
            ViewData["totalSize"] = stats["fsTotalSize"];

            //-----------Data from users--------------------

            //The average of number of post vs users.
            double userSubscriptions = (this._mongoContext.Users.AsQueryable().Select(u => u.Posts.Count()).Sum()*1.0) / (this._mongoContext.Users.AsQueryable().Count()*1.0); 
            ViewData["userSubscriptions"] = userSubscriptions;

            //The average of users that have curriculums.
            ObjectId objectIdNull = new ObjectId("000000000000000000000000");

            double usersWithCurriculums = (this._mongoContext.Users.Find(u => !u.Curriculum.Equals(objectIdNull)).Count() * 1.0) / (this._mongoContext.Users.AsQueryable().Count() * 1.0);
            ViewData["usersWithCurriculums"] = usersWithCurriculums;

            //The users that have at least 10% more posts that the average
            int postsAverage = this._mongoContext.Posts.AsQueryable().Count() / this._mongoContext.Users.AsQueryable().Count();
            var userIds = _mongoContext.Users.AsQueryable().Where(u => u.Posts.Count() > postsAverage).Select(u => u.Id).AsEnumerable();

            var filter = new FilterDefinitionBuilder<ApplicationUser>().In(x => x.ActorId, userIds);

            var actors = this._mongoContext.ApplicationUsers.Find(filter).ToEnumerable();


            //------------------Data from Enterprises------------------

            //Enterprises that have at least one job offer
            var job = _mongoContext.Enterprises.AsQueryable().Where(e => e.Jobs.Count() > 1).Select(e => e.Name).AsEnumerable();
            ViewData["enterprises"] = job;

            return View(actors);
        }
    }
}