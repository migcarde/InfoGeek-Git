using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models
{
    public class CreditCard
    {
        [Required]
        public string HolderName { get; set; }

        [Required]
        public string BrandName { get; set; }

        [Required]
        [CreditCard]
        public string Number { get; set; }

        [Required]
        [Range(0, 12)]
        public int ExpirationMonth { get; set; }

        [Required]
        [Range(0, 99)]
        public int ExpirationYear { get; set; }

        [Required]
        [Range(100, 999)]
        public int CvvCode { get; set; }
    }
}
