using Domain.Bislerium;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class CommentHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BlogPostId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime CommentUpdateDate { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
        public virtual AppUser? Author { get; set; }

    }
}