using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly AplicationDBContext _context;
        public BlogCommentService(AplicationDBContext context)
        {
            _context = context;
        }
        public async Task<BlogComment> AddBlogComment(BlogComment blogComment)
        {
            var result = await _context.BlogComments.AddAsync(blogComment);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<BlogComment> UpdateBlogComment(BlogComment blogComment)
        {
            _context.BlogComments.Update(blogComment);
            await _context.SaveChangesAsync();
            return blogComment;
        }

        public async Task<bool> DeleteBlogComment(string commentId)
        {
            var comment = await _context.BlogComments.Where(c => c.Id.ToString() == commentId.ToString()).ToListAsync();
            if (comment != null)
            {
                _context.BlogComments.Remove(comment[0]);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<BlogCommentWithReactions>> GetBlogComments(string blogId)
        {
            var comments = await _context.BlogComments
                .Where(c => c.BlogPostId.ToString() == blogId)
                .Include(c => c.Author)
                .ToListAsync();

            var commentsWithReactions = comments.Select(c => new BlogCommentWithReactions
            {
                Id = c.Id,
                BlogPostId = c.BlogPostId,
                AuthorId = c.AuthorId,
                Content = c.Content,
                CommentDate = c.CommentDate,
                BlogPost = c.BlogPost,
                Author = c.Author,
                Reactions = _context.BlogReactions.Where(r => r.CommentId == c.Id).ToList()
            });

            return commentsWithReactions.Any() ? commentsWithReactions : new BlogCommentWithReactions[0];
        }

        public async Task<IEnumerable<BlogComment>> GetBlogCommentById(string blogId)
        {
            var comments = await _context.BlogComments.Where(c => c.BlogPostId.ToString() == blogId).ToListAsync();
            return comments.Any() ? comments : new BlogComment[0];
        }
        public async Task<BlogComment> GetCommentById(string commentId)
        {
            var comments = await _context.BlogComments.Where(c => c.Id.ToString() == commentId).ToListAsync();
            return comments![0];
        }
    }
}
