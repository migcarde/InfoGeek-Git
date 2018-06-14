using InfoGeek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Services
{
    public interface IFolderCreator
    {
        Task CreateFolderAsync(ApplicationUser applicationUser);
    }
}
