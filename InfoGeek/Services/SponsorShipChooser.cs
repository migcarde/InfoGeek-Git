using InfoGeek.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Services
{
    public class SponsorShipChooser : ISponsorShipChooser
    {
        private readonly MongoContext mongoContext;

        public SponsorShipChooser(MongoContext mongoContext)
        {
            this.mongoContext = mongoContext;
        }

        public string ChooseSponsorShip()
        {
            var sponsorships = this.mongoContext.SponsorShips.AsQueryable().ToArray();

            if (sponsorships.Count() == 0)
            {
                return "";
            }

            var random = new Random();

            var nRandom = random.Next(0, sponsorships.Count());

            return sponsorships[nRandom].Banner;
        }
    }
}
