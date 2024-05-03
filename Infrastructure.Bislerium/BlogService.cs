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
    public class BlogService : IBlogService
    {
        private readonly ApplicationDBContext _dbContext;
        public BlogService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Blog> AddBlog(Blog blog)
        {
            var result = await _dbContext.Blog.AddAsync(blog);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Blog> DeleteBlog(string id)
        {
            var blog = await _dbContext.Blog.Where(b => b.Id.ToString() == id.ToString()).ToListAsync();
            if (blog != null)
            {
                _dbContext.Blog.Remove(blog[0]);
                await _dbContext.SaveChangesAsync();
            }
            return blog![0];
        }

        public async Task<IEnumerable<Blog>> GetAllBlogs()
        {
            return await _dbContext.Blog.ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetBlogById(string id)
        {
            var result = await _dbContext.Blog.Where(b => b.Id.ToString() == id.ToString()).ToListAsync();
            return result;
        }

        public async Task<Blog> UpdateBlog(Blog blogToUpdate)
        {
            _dbContext.Blog.Update(blogToUpdate);
            await _dbContext.SaveChangesAsync();
            return blogToUpdate;
        }
    }
}
