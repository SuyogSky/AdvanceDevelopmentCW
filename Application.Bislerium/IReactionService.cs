using Domain.Bislerium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IReactionService
    {
        Task<BlogReaction>? AddBlogReaction(string blogId, string userId, ReactionType reactionType);
        Task<BlogReaction>? AddCommentReaction(string commentId, string userId, ReactionType reactionType);

    }
}
