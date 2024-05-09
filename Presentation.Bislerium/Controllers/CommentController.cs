using Application.Bislerium;
using Domain.Bislerium;
using Infrastructure.Bislerium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
namespace Presentation.Bislerium.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly IBlogCommentService _blogCommentService;
        private readonly AplicationDBContext _aplicationDbContext;
        private readonly ILogger<AddCommentModel> _logger;
        private readonly TokenService tokenService;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(AplicationDBContext aplicationDbContext, IBlogCommentService blogCommentService, ILogger<AddCommentModel> logger, TokenService tokenService, UserManager<AppUser> userManager) 
        {
            _aplicationDbContext = aplicationDbContext;
            _blogCommentService = blogCommentService;
            _logger = logger;
            this.tokenService = tokenService;
            this._userManager = userManager;
        }
        [Authorize(Roles="Blogger")]
        [HttpPost("add")]
        public async Task<IActionResult> AddBlogComment([FromBody] AddCommentModel addCommentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string userId = tokenService.GetUserIdFromToken(token!);
            var user = await _userManager.FindByIdAsync(userId);
            var blogComment = new BlogComment
            {
                BlogPostId = addCommentModel.BlogPostId,
                Content = addCommentModel.Content,
                AuthorId = Guid.Parse(userId),
                CommentDate = DateTime.UtcNow,
                Author = user
            };

            try
            {
                var createdComment = await _blogCommentService.AddBlogComment(blogComment);
                return CreatedAtAction(nameof(GetBlogComment), new { id = createdComment.Id }, createdComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the blog comment.");
                return StatusCode(500, "An error occurred while adding the blog comment. Please try again.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogComment(String id)
        {
 
            try
            {
                var blogComment = await _blogCommentService.GetBlogComments(id);
                if (blogComment == null)
                {
                    return NotFound($"No blog comment found with ID {id}.");
                }
                return Ok(blogComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the blog comment with ID {id}.");
                return StatusCode(500, "An error occurred while retrieving the blog comment. Please try again.");
            }
        }

        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult> GetBlogComments(Guid blogId)
        {
            if (blogId == Guid.Empty)
            {
                return BadRequest("Invalid blog ID.");
            }

            try
            {
                var blogComments = await _blogCommentService.GetBlogComments(blogId.ToString());
                if (blogComments == null || !blogComments.Any())
                {
                    _logger.LogWarning("No blog comments found for the given blog ID.");
                    return NotFound("No blog comments found for the given blog ID.");
                }
                return Ok(blogComments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving blog comments for the given blog ID {blogId}.");
                return StatusCode(500, "An error occurred while retrieving blog comments. Please try again.");
            }
        }

        [Authorize(Roles = "Blogger")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBlogComment(String id, [FromBody] UpdateCommentModel updateCommentModel)
        {
            if (updateCommentModel == null)
            {
                return BadRequest("Invalid blog comment data.");
            }
            var existingComment = await _blogCommentService.GetCommentById(id);

            if (existingComment == null)
            {
                return NotFound($"No blog comment found with ID {id} to update.");
            }
            BlogComment existing = existingComment;
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string userId = tokenService.GetUserIdFromToken(token!);
            if(userId != existing.AuthorId.ToString())
            {
                return StatusCode(403, "You dont have the permission");
            }
            existing.Content = updateCommentModel.Content;

            try
            {
                var updatedComment = await _blogCommentService.UpdateBlogComment(existing);
                return Ok(updatedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog comment.");
                return StatusCode(500, "An error occurred while updating the blog comment. Please try again.");
            }
        }
        [Authorize(Roles = "Blogger")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> RemoveBlogComment(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid comment ID.");
            }

            try
            {
                var existingComment = await _blogCommentService.GetCommentById(id.ToString());
                Console.WriteLine(existingComment);
                if (existingComment == null)
                {
                    return NotFound($"No blog comment found with ID {id} to update.");
                }
                BlogComment existing = existingComment;
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                string userId = tokenService.GetUserIdFromToken(token!);
                if (userId != existing.AuthorId.ToString())
                {
                    return StatusCode(403, "You dont have the permission");
                }
                bool removalSuccessful = await _blogCommentService.DeleteBlogComment(id.ToString());
                if (removalSuccessful)
                {
                    return Ok($"Blog comment with ID {id} successfully deleted.");
                }
                else
                {
                    return NotFound($"Blog comment with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog comment.");
                return StatusCode(500, "An error occurred while deleting the blog comment. Please try again.");
            }
        }
    }
}
