using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models.JobViewModels
{
    public class JobViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, Double.MaxValue)]
        public double Salary { get; set; }
        
        [Required]
        public bool Month { get; set; }

        [Required]
        public string Tags { get; set; }

        [Required]
        public string Category { get; set; }
    }
}
