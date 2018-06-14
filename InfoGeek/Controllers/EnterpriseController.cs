using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoGeek.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InfoGeek.Controllers
{
    public class EnterpriseController : Controller
    {
        private readonly MongoContext _mongoContext;

        public EnterpriseController(MongoContext mongoContext)
        {
            this._mongoContext = mongoContext;
        }

        // GET: Enterprise
        public ActionResult Index()
        {
            var enterprises = this._mongoContext.Enterprises.AsQueryable().ToEnumerable();

            return View(enterprises);
        }

        // GET: Enterprise/Details/5
        public ActionResult Details(string id)
        {
            ObjectId objectId = new ObjectId(id);

            var enterprise = this._mongoContext.Enterprises.Find(e => e.Id.Equals(objectId)).First();

            return View(enterprise);
        }
    }
}