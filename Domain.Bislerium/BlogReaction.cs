using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{

    public class BlogReaction
    {
            [Key]
            public Guid Id { get; set; }
            public Guid? BlogPostId { get; set; }  
            public Guid? CommentId { get; set; } 
            public Guid? UserId { get; set; } 
            [Required]
            public ReactionType Type { get; set; }
            public DateTime ReactionDate { get; set; } = DateTime.Now;
            public virtual BlogPost? BlogPost { get; set; }
            public virtual BlogComment? Comment { get; set; }
            public virtual AppUser? User { get; set; }        
        }

}
