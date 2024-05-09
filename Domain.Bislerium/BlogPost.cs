using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Bislerium
{
    public class BlogPost
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
        public DateTime PostDate { get; set; } = DateTime.UtcNow;
        public bool isDeleted { get; set; } = false;
        public virtual AppUser? Author { get; set; }
    }
}
