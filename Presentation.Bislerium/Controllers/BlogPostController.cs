using Application.Bislerium;
using Domain.Bislerium;
using Infrastructure.Bislerium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
namespace Presentation.Bislerium.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<AddBlogPost> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly AplicationDBContext aplicationDbContext;
        private readonly TokenService tokenService;
        private readonly UserManager<AppUser> _userManager;
        public BlogPostController(AplicationDBContext aplicationDbContext,IBlogPostService blogPostService, ILogger<AddBlogPost> logger, IWebHostEnvironment environment, TokenService tokenService, UserManager<AppUser> userManager = null)
        {
            this.aplicationDbContext = aplicationDbContext;
            _logger = logger;
            _blogPostService = blogPostService;
            _environment = environment;
            this.tokenService = tokenService;
            _userManager = userManager;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPost(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid blog post ID.");
            }

            try
            {
                var blogPost = await _blogPostService.GetBlogPostById(id);
                if (blogPost == null)
                {
                    return NotFound($"No blog post found with ID {id}.");
                }
                return Ok(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the blog post with ID {id}.");
                return StatusCode(500, "An error occurred while retrieving the blog post. Please try again.");
            }
        }
        [Authorize(Roles = "Blogger")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBlogPost(string id, [FromBody] UpdateBlogDetails addBlogPost)
        {
            if (addBlogPost == null)
            {
                return BadRequest("Invalid blog post data.");
            }


            var existingBlogPost = await _blogPostService.GetBlogPostById(id);
            if (existingBlogPost == null)
            {
                return NotFound($"No blog post found with ID {id} to update.");
            }
            BlogPost existingBlog = existingBlogPost.First()!;
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string userId = tokenService.GetUserIdFromToken(token!);
            if (existingBlog.AuthorId.ToString() != userId)
            {
                return StatusCode(403, "You donot have the permission to do this.");
            }

            existingBlog.Title = addBlogPost.Title;
            existingBlog.Body = addBlogPost.Body;

            // Handle image file if provided
            try
            {
                var updatedBlogPost = await _blogPostService.UpdateBlogPost(existingBlog);
                return Ok(updatedBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog post.");
                return StatusCode(500, "An error occurred while updating the blog post. Please try again.");
            }
        }
        [Authorize(Roles = "Blogger")]
        [HttpPut("update/image/{id}")]
        public async Task<IActionResult> UpdateBlogPost(string id, [FromForm] UpdateBlogImageModel addBlogPost)
        {
            if (addBlogPost == null)
            {
                return BadRequest("Invalid blog post data.");
            }

            var existingBlogPost = await _blogPostService.GetBlogPostById(id);
            if (existingBlogPost == null)
            {
                return NotFound($"No blog post found with ID {id} to update.");
            }
            BlogPost existingBlog = existingBlogPost.First()!;
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string userId = tokenService.GetUserIdFromToken(token!);
            if (existingBlog.AuthorId.ToString() != userId)
            {
                return StatusCode(403, "You donot have the permission to do this.");
            }
            if (addBlogPost.ImageFile != null && addBlogPost.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addBlogPost.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await addBlogPost.ImageFile.CopyToAsync(stream);
                }
                existingBlog.ImageUrl = Url.Content($"~/uploads/{fileName}");
            }

            try
            {
                var updatedBlogPost = await _blogPostService.UpdateBlogPost(existingBlog);
                return Ok(updatedBlogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog post.");
                return StatusCode(500, "An error occurred while updating the blog post. Please try again.");
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> All(string? sortType, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > 50)
            {
                pageSize = 50;
            }
            try
            {
                var (blogPostsWithReactions, totalCount) = await _blogPostService.GetAllBlogPosts(pageNumber, pageSize,sortType);
                if (blogPostsWithReactions == null || !blogPostsWithReactions.Any())
                {
                    _logger.LogWarning("No blog posts found.");
                    return NotFound("No blog posts found.");
                }
                var paginationMetadata = new
                {
                    totalCount,
                    pageSize,
                    pageNumber
                };
                return Ok(new { blogPostsWithReactions, paginationMetadata });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving blog posts.");
                return StatusCode(500, "An error occurred while retrieving blog posts. Please try again.");
            }
        }
        [Authorize(Roles = "Blogger")]
        [HttpDelete("delete/{blogPostId}")]
        public async Task<IActionResult> RemoveBlogPost(string blogPostId)
        {
            if (string.IsNullOrWhiteSpace(blogPostId))
            {
                return BadRequest("Invalid blog post ID.");
            }
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                string userId = tokenService.GetUserIdFromToken(token!);
                var blogPost = await _blogPostService.GetBlogPostById(blogPostId.ToString());
                if(blogPost == null || !blogPost.Any()) { 
                     return NotFound("No blog posts found.");
                }
                BlogPost post = blogPost.First()!;
                if(post.AuthorId.ToString() != userId)
                {
                    return StatusCode(403, "You donot have the permission to do this.");
                }
                bool removalSuccessful = await _blogPostService.DeleteBlogPost(blogPostId);
                if (removalSuccessful)
                {
                    return Ok($"Blog post with ID {blogPostId} successfully deleted.");
                }
                else
                {
                    return NotFound($"Blog post with ID {blogPostId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog post.");
                return StatusCode(500, "An error occurred while deleting the blog post. Please try again.");
            }
        }
        [Authorize(Roles = "Blogger")]
        [HttpPost("add")]
        public async Task<IActionResult> AddBlog([FromForm] AddBlogPost addBlogPost)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string userId = tokenService.GetUserIdFromToken(token!);
            var user = await _userManager.FindByIdAsync(userId);
            var blog = new BlogPost
            {
                AuthorId = Guid.Parse(userId),
                Title = addBlogPost.Title,
                Body = addBlogPost.Body,
                Author = user
            };
            if (addBlogPost.ImageFile != null && addBlogPost.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addBlogPost.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await addBlogPost.ImageFile.CopyToAsync(stream);
                }
                blog.ImageUrl = Url.Content($"~/uploads/{fileName}");
            }
            await aplicationDbContext.BlogPosts.AddAsync(blog);
            try
            {
                await aplicationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the new blog post.");
                return StatusCode(500, "An error occurred while saving the blog post. Please try again.");
            }
            var locationUri = Url.Action("GetBlogPost", new { id = blog.Id });
            return Created(locationUri, blog);
        }
    }
}
