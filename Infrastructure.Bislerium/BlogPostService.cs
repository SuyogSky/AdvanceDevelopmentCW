using Application.Bislerium;
using Domain.Bislerium;
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
        public BlogPostService(AplicationDBContext context) { 
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
            var blog = await _context.BlogPosts.Where(s => s.Id.ToString() == blogPostId.ToString()).ToListAsync();
            if (blog != null) {
                _context.BlogPosts.Remove(blog[0]);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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

        public async Task<BlogPost> UpdateBlogPost(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
            return blogPost; 
        }


    }
}
