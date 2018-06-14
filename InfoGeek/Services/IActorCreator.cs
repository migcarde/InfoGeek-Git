using InfoGeek.Models;
using InfoGeek.Models.AccountViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Services
{
    public interface IActorCreator
    {
        Task CreateUserAsync(ApplicationUser applicationUser);
        Task CreateEnterpiseAsync(ApplicationUser applicationUser, RegisterEnterpriseViewModel model);
        Task CreateSponsorAsync(ApplicationUser applicationUser);
        List<ObjectId> CreateFolders();
    }
}
