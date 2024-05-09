using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Domain.Bislerium
{
    public class BlogComment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BlogPostId { get; set; }

        [Required]
        public Guid AuthorId { get; set; } 

        [Required]
        public string? Content { get; set; }

        public DateTime CommentDate { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
        public virtual AppUser? Author { get; set; }
    }
}
