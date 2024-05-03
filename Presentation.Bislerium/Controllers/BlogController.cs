using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService blogService;
        public BlogController(IBlogService blogService)
        {
            this.blogService = blogService;
        }

        [HttpPost, Route("AddBlog")]
        public async Task<IActionResult> AddBlog(Blog blog)
        {
            var addBlog = await blogService.AddBlog(blog);
            return Ok(addBlog);
        }

        [Authorize(Roles = "User")]
        [HttpGet, Route("GetAllBlogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            return Ok(await blogService.GetAllBlogs());
        }
        [HttpGet, Route("GetBlogById")]
        public async Task<IActionResult> GetBlogById(String id)
        {
            return Ok(await blogService.GetBlogById(id));
        }

        [HttpDelete, Route("DeleteBlog")]
        public async Task<IActionResult> DeleteBlogById(String id)
        {
            return Ok(await blogService.DeleteBlog(id));
        }
        [HttpPut, Route("UpdateBlog")]
        public async Task<IActionResult> UpdateBlog(Blog blog)
        {
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();
            dict.Add("success", 1);
            dict.Add("data", await blogService.UpdateBlog(blog));

            return Ok(dict);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
