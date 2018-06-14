using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using InfoGeek.Models;
using InfoGeek.Models.SponsorShipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class SponsorShipController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly UserManager<ApplicationUser> userManager;

        public SponsorShipController(MongoContext mongoContext, UserManager<ApplicationUser> userManager)
        {
            this.mongoContext = mongoContext;
            this.userManager = userManager;
        }

        // GET: SponsorShip
        [Authorize(Roles = "ADMIN")]
        public ActionResult Index()
        {
            var sponsorships = this.mongoContext.SponsorShips.AsQueryable().ToEnumerable();

            return View(sponsorships);
        }

        [Authorize(Roles = "SPONSOR")]
        public ActionResult MySponsorShips()
        {
            var applicaitonUser = this.userManager.GetUserAsync(HttpContext.User).Result;
            var sponsor = this.mongoContext.Sponsors.Find(s => s.Id.Equals(applicaitonUser.ActorId)).First();

            var filter = new FilterDefinitionBuilder<SponsorShip>().In(f => f.Id, sponsor.SponsorShips);

            var sponsorships = this.mongoContext.SponsorShips.Find(filter).ToEnumerable();

            return View(sponsorships);
        }

        [Authorize(Roles = "SPONSOR, ADMIN")]
        // GET: SponsorShip/Details/5
        public ActionResult Details(string id)
        {
            ObjectId objectId = new ObjectId(id);

            if(User.IsInRole("SPONSOR"))
            {
                var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                var sponsor = this.mongoContext.Sponsors.Find(s => s.Id.Equals(applicationUser.ActorId)).First();

                if (!sponsor.SponsorShips.Contains(objectId))
                {
                    TempData["Error"] = "This sponsorship isn't yours.";
                    return RedirectToAction(nameof(MySponsorShips));
                }
            }

            var sponsorship = this.mongoContext.SponsorShips.Find(s => s.Id.Equals(objectId)).First();

            return View(sponsorship);
        }

        [Authorize(Roles = "SPONSOR")]
        // GET: SponsorShip/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SponsorShip/Create
        [Authorize(Roles = "SPONSOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SponsorShipViewModel collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                    var sponsor = this.mongoContext.Sponsors.Find(f => f.Id.Equals(applicationUser.ActorId)).First();

                    CreditCard creditCard = new CreditCard
                    {
                        HolderName = collection.HolderName,
                        BrandName = collection.BrandName,
                        Number = collection.Number,
                        ExpirationMonth = collection.ExpirationMonth,
                        ExpirationYear = collection.ExpirationYear,
                        CvvCode = collection.CvvCode
                    };

                    SponsorShip sponsorShip = new SponsorShip
                    {
                        Id = ObjectId.GenerateNewId(),
                        Banner = collection.Banner,
                        CreditCard = creditCard
                    };
                    sponsor.SponsorShips.Add(sponsorShip.Id);

                    this.mongoContext.SponsorShips.InsertOne(sponsorShip);
                    this.mongoContext.Sponsors.FindOneAndUpdate(s => s.Id.Equals(sponsor.Id), Builders<Sponsor>.Update.Set(s => s.SponsorShips, sponsor.SponsorShips));


                    return RedirectToAction(nameof(MySponsorShips));
                }
                catch
                {
                    return View(collection);
                }
            }
            return View(collection);
        }

        // GET: SponsorShip/Edit/5
        [Authorize(Roles = "SPONSOR")]
        public ActionResult Edit(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

            var sponsor = this.mongoContext.Sponsors.Find(s => s.Id.Equals(applicationUser.ActorId)).First();

            if (!sponsor.SponsorShips.Contains(objectId))
            {
                TempData["Error"] = "This sponsorship isn't yours.";
                return RedirectToAction(nameof(MySponsorShips));
            }

            var sponsorship = this.mongoContext.SponsorShips.Find(s => s.Id.Equals(objectId)).First();

            var model = new SponsorShipViewModel
            {
                Banner = sponsorship.Banner,
                BrandName = sponsorship.CreditCard.BrandName,
                CvvCode = sponsorship.CreditCard.CvvCode,
                ExpirationMonth = sponsorship.CreditCard.ExpirationMonth,
                ExpirationYear = sponsorship.CreditCard.ExpirationYear,
                HolderName = sponsorship.CreditCard.HolderName,
                Number = sponsorship.CreditCard.Number
            };

            return View(model);
        }

        // POST: SponsorShip/Edit/5
        [Authorize(Roles = "SPONSOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, SponsorShipViewModel collection)
        {

            if(ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here

                    ObjectId objectId = new ObjectId(id);

                    var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                    var sponsor = this.mongoContext.Sponsors.Find(s => s.Id.Equals(applicationUser.ActorId)).First();

                    if (!sponsor.SponsorShips.Contains(objectId))
                    {
                        TempData["Error"] = "This sponsorship isn't yours.";
                        return RedirectToAction(nameof(MySponsorShips));
                    }

                    CreditCard creditCard = new CreditCard
                    {
                        HolderName = collection.HolderName,
                        BrandName = collection.BrandName,
                        Number = collection.Number,
                        ExpirationMonth = collection.ExpirationMonth,
                        ExpirationYear = collection.ExpirationYear,
                        CvvCode = collection.CvvCode
                    };

                    UpdateDefinition<SponsorShip> updateDefinition = Builders<SponsorShip>.Update.Set(s => s.Banner, collection.Banner)
                        .Set(s => s.CreditCard, creditCard);

                    this.mongoContext.SponsorShips.FindOneAndUpdate(s => s.Id.Equals(objectId), updateDefinition);

                    return RedirectToAction(nameof(MySponsorShips));
                }
                catch
                {
                    return View();
                }
            }
            return View();
            
        }

        // GET: SponsorShip/Delete/5
        [Authorize(Roles = "ADMIN, SPONSOR")]
        public ActionResult Delete(string id)
        {
            ObjectId objectId = new ObjectId(id);

            if (User.IsInRole("SPONSOR"))
            {
                var applicationUser = this.userManager.GetUserAsync(HttpContext.User).Result;

                var sponsor = this.mongoContext.Sponsors.Find(s => s.Id.Equals(applicationUser.ActorId)).First();

                if (!sponsor.SponsorShips.Contains(objectId))
                {
                    TempData["Error"] = "This sponsorship isn't yours.";
                    return RedirectToAction(nameof(MySponsorShips));
                }
            }

            this.mongoContext.SponsorShips.FindOneAndDelete(s => s.Id.Equals(objectId));

            return RedirectToAction(nameof(Index));
        }
    }
}