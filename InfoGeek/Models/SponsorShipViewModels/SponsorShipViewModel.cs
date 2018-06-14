using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Models.SponsorShipViewModels
{
    public class SponsorShipViewModel
    {
        [Required]
        [Url]
        public string Banner { get; set; }

        [Required]
        public string HolderName { get; set; }

        [Required]
        public string BrandName { get; set; }

        [Required]
        [CreditCard]
        public string Number { get; set; }
        
        [Required]
        [Range(minimum: 0.0, maximum: 12.0, ErrorMessage = "Error")]
        public int ExpirationMonth { get; set; }

        [Required]
        [Range(0, 99)]
        public int ExpirationYear { get; set; }

        [Required]
        [Range(100, 999)]
        public int CvvCode { get; set; }
    }
}
