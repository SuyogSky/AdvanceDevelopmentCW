using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class BlogReactionWithUser
    {
        public Guid Id { get; set; }
        public ReactionType Type { get; set; }
        public DateTime ReactionDate { get; set; }
        public UserWithName User { get; set; }
    }
}
