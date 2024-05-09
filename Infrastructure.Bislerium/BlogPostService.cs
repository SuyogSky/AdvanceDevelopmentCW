using Application.Bislerium;
using Application.Bislerium;
using Domain.Bislerium;
using Domain.Bislerium;
using Infrastructure.Bislerium;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class BlogPostService : IBlogPostService
    {
        private readonly AplicationDBContext _context;
        public BlogPostService(AplicationDBContext context)
        {
            _context = context;
        }
        public async Task<BlogPost> AddBlogPost(BlogPost blogPost)
        {
            var result = await _context.BlogPosts.AddAsync(blogPost);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> DeleteBlogPost(string blogPostId)
        {
            var blogPostGuid = Guid.Parse(blogPostId);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var blogPost = await _context.BlogPosts.FindAsync(blogPostGuid);
                    if (blogPost == null)
                        return false;

                    var postReactions = await _context.BlogReactions
                                                      .Where(br => br.BlogPostId == blogPost.Id)
                                                      .ToListAsync();
                    if (postReactions.Any())
                    {
                        _context.BlogReactions.RemoveRange(postReactions);
                    }

                    var comments = await _context.BlogComments
                                                 .Where(bc => bc.BlogPostId == blogPost.Id)
                                                 .ToListAsync();
                    foreach (var comment in comments)
                    {
                        var commentReactions = await _context.BlogReactions
                                                             .Where(br => br.CommentId == comment.Id)
                                                             .ToListAsync();
                        if (commentReactions.Any())
                        {
                            _context.BlogReactions.RemoveRange(commentReactions);
                        }
                        _context.BlogComments.Remove(comment);
                    }

                    _context.BlogPosts.Remove(blogPost);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Error deleting blog post: " + ex.Message);
                    return false;
                }
            }
        }



        public async Task<(IEnumerable<BlogWithReactions> blogPostsWithReactions, int totalCount)> GetAllBlogPosts(
            int pageNumber = 1,
            int pageSize = 10,
            string? sortType = null)
        {
            var blogsQuery = _context.BlogPosts
                .Where(bp => !bp.isDeleted)
                .Include(bp => bp.Author)
                .Select(bp => new BlogWithReactions
                {
                    Blog = bp,
                    Reactions = _context.BlogReactions.Where(br => br.BlogPostId == bp.Id).ToList(),
                    Comments = _context.BlogComments.Where(bc => bc.BlogPostId == bp.Id).ToList(),
                    Popularity = 2 * _context.BlogReactions.Count(br => br.BlogPostId == bp.Id && br.Type == ReactionType.UpVote) -
                                 _context.BlogReactions.Count(br => br.BlogPostId == bp.Id && br.Type == ReactionType.DownVote) +
                                 _context.BlogComments.Count(bc => bc.BlogPostId == bp.Id)
                })
                .AsQueryable();

            switch (sortType?.ToLower())
            {
                case "popularity":
                    blogsQuery = blogsQuery.OrderByDescending(x => x.Popularity);
                    break;
                case "recency":
                    blogsQuery = blogsQuery.OrderByDescending(x => x.Blog.PostDate);
                    break;
                default:
                    blogsQuery = blogsQuery.OrderBy(x => Guid.NewGuid());
                    break;
            }

            var totalCount = await blogsQuery.CountAsync();
            var pagedBlogs = await blogsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedBlogs, totalCount);
        }


        public async Task<IEnumerable<BlogPost?>> GetBlogPostById(string blogPostId)
        {
            var result = await _context.BlogPosts.Where(s => s.Id.ToString() == blogPostId.ToString()).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<BlogHistory>> GetUsersBlogHistoru(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            Guid userGuid;
            if (!Guid.TryParse(userId, out userGuid))
                throw new ArgumentException("Invalid user ID format.", nameof(userId));
            var history = await _context.BlogHistory
                                        .Where(bh => bh.AuthorId == userGuid)
                                        .OrderByDescending(bh => bh.UpdateDate)
                                        .ToListAsync();

            return history;
        }


        public async Task<IEnumerable<BlogWithReactions>> GetUsersBlogs(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var userBlogs = await _context.BlogPosts
                .Where(b => b.AuthorId.ToString() == userId && !b.isDeleted)
                .Include(b => b.Author)
                .Select(bp => new BlogWithReactions
                {
                    Blog = bp,
                    Reactions = _context.BlogReactions.Where(br => br.BlogPostId == bp.Id).ToList(),
                    Comments = _context.BlogComments.Where(bc => bc.BlogPostId == bp.Id).ToList(),
                    Popularity = 2 * _context.BlogReactions.Count(br => br.BlogPostId == bp.Id && br.Type == ReactionType.UpVote) -
                                 _context.BlogReactions.Count(br => br.BlogPostId == bp.Id && br.Type == ReactionType.DownVote) +
                                 _context.BlogComments.Count(bc => bc.BlogPostId == bp.Id)
                })
                .ToListAsync();

            if (!userBlogs.Any())
            {
                throw new InvalidOperationException($"No blogs found for the user with ID {userId}.");
            }

            return userBlogs;
        }




        public async Task<BlogPost> UpdateBlogPost(BlogPost blogPost)
        {
            var existingBlogPost = await _context.BlogPosts.FindAsync(blogPost.Id);
            if (existingBlogPost == null)
                throw new ArgumentException("Blog post not found.");

            var blogHistory = new BlogHistory
            {
                Id = Guid.NewGuid(),
                AuthorId = existingBlogPost.AuthorId,
                Title = existingBlogPost.Title,
                Body = existingBlogPost.Body,
                ImageUrl = existingBlogPost.ImageUrl,
                UpdateDate = DateTime.UtcNow
            };

            await _context.BlogHistory.AddAsync(blogHistory);

            existingBlogPost.Title = blogPost.Title;
            existingBlogPost.Body = blogPost.Body;
            existingBlogPost.ImageUrl = blogPost.ImageUrl;

            _context.BlogPosts.Update(existingBlogPost);
            await _context.SaveChangesAsync();
            return existingBlogPost;
        }



    }
}