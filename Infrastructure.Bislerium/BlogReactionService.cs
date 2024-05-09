using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class BlogReactionService : IReactionService
    {
        private readonly AplicationDBContext _context;

        public BlogReactionService(AplicationDBContext context)
        {
            _context = context;
        }

        public async Task<BlogReaction>? AddBlogReaction(string blogId, string userId, ReactionType reactionType)
        {
            try
            {
                var existingReaction = await _context.BlogReactions
                   .FirstOrDefaultAsync(r => r.BlogPostId == Guid.Parse(blogId) && r.UserId == Guid.Parse(userId));

                if (existingReaction != null)
                {
                    if (existingReaction.Type == reactionType)
                    {
                        _context.BlogReactions.Remove(existingReaction);
                        await _context.SaveChangesAsync();
                        return null;
                    }
                    else
                    {
                        existingReaction.Type = reactionType;
                        await _context.SaveChangesAsync();
                        return existingReaction;
                    }
                }
                else
                {
                    var newReaction = new BlogReaction
                    {
                        BlogPostId = Guid.Parse(blogId),
                        UserId = Guid.Parse(userId),
                        Type = reactionType,
                        ReactionDate = DateTime.Now
                    };

                    await _context.BlogReactions.AddAsync(newReaction);
                    await _context.SaveChangesAsync();
                    return newReaction;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<BlogReaction>? AddCommentReaction(string commentId, string userId, ReactionType reactionType)
        {
            var existingReaction = await _context.BlogReactions
     .FirstOrDefaultAsync(r => r.CommentId == Guid.Parse(commentId) && r.UserId == Guid.Parse(userId));

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                {
                    _context.BlogReactions.Remove(existingReaction);
                    await _context.SaveChangesAsync();
                    return null;
                }
                else
                {
                    existingReaction.Type = reactionType;
                    await _context.SaveChangesAsync();
                    return existingReaction;
                }
            }
            else
            {
                var newReaction = new BlogReaction
                {
                    CommentId = Guid.Parse(commentId),
                    UserId = Guid.Parse(userId),
                    Type = reactionType,
                    ReactionDate = DateTime.Now
                };

                await _context.BlogReactions.AddAsync(newReaction);
                await _context.SaveChangesAsync();
                return newReaction;
            }
        }

    
    }
}