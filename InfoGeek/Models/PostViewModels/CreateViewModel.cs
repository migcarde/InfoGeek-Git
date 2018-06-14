using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models.PostViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        [Url]
        public string Photo { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }
    }
}
