using InfoGeek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Services
{
    interface IPostCreator
    {
        Task CreatePostAsync(ApplicationUser user);
    }
}
