using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class BlogWithReactions
    {
        public BlogPost Blog { get; set; }
        public List<BlogReaction> Reactions { get; set; }

        public List<BlogComment> Comments { get; set; }

        public int? Popularity { get; set; }
    }
}
