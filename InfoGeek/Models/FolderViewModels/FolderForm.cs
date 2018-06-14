using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoGeek.Forms
{
    public class FolderForm
    {
        [Required]
        public string Name { get; set; }
    }
}
