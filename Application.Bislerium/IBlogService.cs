using Domain.Bislerium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IBlogService
    {
        Task<Blog> AddBlog(Blog blog);
        Task<IEnumerable<Blog>> GetAllBlogs();
        Task<Blog> UpdateBlog(Blog blog);
        Task<Blog> DeleteBlog(String id);
        Task<IEnumerable<Blog>> GetBlogById(String id);
    }
}
