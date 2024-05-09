using Domain.Bislerium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IBlogPostService
    {
        Task<BlogPost> AddBlogPost(BlogPost blogPost);
        Task<BlogPost> UpdateBlogPost(BlogPost blogPost);
        Task<bool> DeleteBlogPost(String blogPostId);
        Task<(IEnumerable<BlogWithReactions> blogPostsWithReactions, int totalCount)> GetAllBlogPosts(int pageNumber, int pageSize, string? sortType);
        Task<IEnumerable<BlogPost?>> GetBlogPostById(String blogPostId);
    }
}
