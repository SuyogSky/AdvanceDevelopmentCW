using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class UpdateBlogDetails
    {

        [Required, MaxLength(200)]
        public string? Title { get; set; }
        [Required]
        public string? Body { get; set; }
    }
}
