using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models.CategoryViewModel
{
    public class CategoryViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Father { get; set; }
    }
}
