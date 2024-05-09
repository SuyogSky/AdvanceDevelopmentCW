using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class BlogCommentWithReactions
    {
        public Guid Id { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid AuthorId { get; set; }
        public string? Content { get; set; }
        public DateTime CommentDate { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
        public virtual AppUser? Author { get; set; }
        public virtual IEnumerable<BlogReaction> Reactions { get; set; }
    }
}
