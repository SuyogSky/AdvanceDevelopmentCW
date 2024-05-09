using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class BlogHistory
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        [Required, MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string? Body { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
    }
}