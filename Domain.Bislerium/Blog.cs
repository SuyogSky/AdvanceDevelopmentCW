using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class Blog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Content { get; set; }
        [Required]
        public string? Image { get; set; }
    }
}
