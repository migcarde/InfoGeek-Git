using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.PostViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class PostController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;

        public PostController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        [Authorize]
        // GET: Post
        public ActionResult Index()
        {
            var posts = this.mongoContext.Posts.Find(p => p.Principal.Equals(true)).ToEnumerable();

            return View(posts);
        }

        [Authorize(Roles = "USER")]
        public ActionResult MyPosts()
        {
            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

            var filter = new FilterDefinitionBuilder<Post>();
            var aux = filter.In(x => x.Id, user.Posts);

            var posts = this.mongoContext.Posts.Find(aux).ToEnumerable();

            return View(posts);
        }

        [Authorize]
        // GET: Post/Details/5
        public ActionResult Details(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var post = this.mongoContext.Posts.Find(p => p.Id.Equals(objectId)).First();

            return View(post);
        }

        // GET: Post/Create
        [Authorize(Roles = "USER")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        [Authorize(Roles = "USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here

                    var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                    var user = this.mongoContext.Users.Find(u => u.Id.Equals(applicationUser.ActorId)).First();

                    Post post = new Post
                    {
                        Id = ObjectId.GenerateNewId(),
                        Description = collection.Description,
                        Date = DateTime.Now,
                        Photo = collection.Photo,
                        Url = collection.Url,
                        Writer = applicationUser.Email,
                        Replies = new List<ObjectId>(),
                        Principal = true
                    };
                    user.Posts.Add(post.Id);

                    UpdateDefinition<User> updateDefinition = Builders<User>.Update.Set("Posts", user.Posts);
                    this.mongoContext.Users.FindOneAndUpdate(u => u.Id.Equals(user.Id), updateDefinition);

                    this.mongoContext.Posts.InsertOne(post);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: Post/Edit/5
        [Authorize(Roles = "USER")]
        public ActionResult Edit(string id)
        {
            var actor = this.userManager.GetUserAsync(HttpContext.User).Result;
            var user = this.mongoContext.Users.Find(a => a.Id.Equals(actor.ActorId)).First();

            ObjectId objectId = new ObjectId(id);

            if (!user.Posts.Contains(objectId))
            {
                TempData["Error"] = "This post isn't yours.";
                return RedirectToAction(nameof(Index));
            }

            var post = this.mongoContext.Posts.Find(p => p.Id.Equals(objectId)).First();

            CreateViewModel result = new CreateViewModel
            {
                Description = post.Description,
                Photo = post.Photo,
                Url = post.Url
            };

            return View(result);
        }

        // POST: Post/Edit/5
        [Authorize(Roles = "USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, CreateViewModel collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here

                    var actor = userManager.GetUserAsync(HttpContext.User).Result;

                    var user = this.mongoContext.Users.Find(u => u.Id.Equals(actor.ActorId)).First();

                    ObjectId objectId = new ObjectId(id);

                    if (!user.Posts.Contains(objectId))
                    {
                        TempData["Error"] = "This post isn't yours.";
                        return RedirectToAction(nameof(Index));
                    }

                    UpdateDefinition<Post> updateDefinition = Builders<Post>.Update.Set("Description", collection.Description)
                        .Set("Photo", collection.Photo)
                        .Set("Url", collection.Url);

                    mongoContext.Posts.FindOneAndUpdate(p => p.Id.Equals(objectId), updateDefinition);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: Post/Delete/5
        [Authorize(Roles = "USER")]
        public ActionResult Delete(string id)
        {
            var actor = this.userManager.GetUserAsync(HttpContext.User).Result;
            var user = this.mongoContext.Users.Find(u => u.Id.Equals(actor.ActorId)).First();

            ObjectId objectId = new ObjectId(id);
            var post = this.mongoContext.Posts.Find(p => p.Id.Equals(objectId)).First();

            if (!user.Posts.Contains(objectId))
            {
                TempData["Error"] = "This post isn't yours.";
                return RedirectToAction(nameof(Index));
            }
            var filter = new FilterDefinitionBuilder<Post>().In(x => x.Id, post.Replies);
            this.mongoContext.Posts.DeleteMany(filter);

            user.Posts.Remove(objectId);
            UpdateDefinition<User> updateDefinition = Builders<User>.Update.Set("Posts", user.Posts);
            this.mongoContext.Posts.FindOneAndDelete(p => p.Id.Equals(objectId));

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "USER")]
        public ActionResult Reply(string id)
        {
            ReplyViewModel reply = new ReplyViewModel
            {
                Id = id
            };

            return View();
        }

        [Authorize(Roles = "USER")]
        public ActionResult Replies(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var post = this.mongoContext.Posts.Find(p => p.Id.Equals(objectId)).First();

            var filter = new FilterDefinitionBuilder<Post>();
            var aux = filter.In(x => x.Id, post.Replies);

            var replies = this.mongoContext.Posts.Find(aux).ToEnumerable();

            return View(replies);
        }

        // POST: Post/Reply/5
        [Authorize(Roles = "USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(ReplyViewModel replyViewModel)
        {
            ObjectId objectId = new ObjectId(replyViewModel.Id);

            var actor = userManager.GetUserAsync(HttpContext.User).Result;

            var post = mongoContext.Posts.Find(p => p.Id.Equals(objectId)).First();

            Post reply = new Post
            {
                Id = ObjectId.GenerateNewId(),
                Date = DateTime.Now,
                Description = replyViewModel.Description,
                Photo = replyViewModel.Photo,
                Url = replyViewModel.Url,
                Writer = actor.Email,
                Replies = new List<ObjectId>(),
                Principal = false
            };
            post.Replies.Add(reply.Id);

            this.mongoContext.Posts.InsertOne(reply);

            UpdateDefinition<Post> updateDefinition = Builders<Post>.Update.Set("Replies", post.Replies);

            mongoContext.Posts.FindOneAndUpdate(p => p.Id.Equals(objectId), updateDefinition);

            return RedirectToAction(nameof(Index));
        }
    }
}