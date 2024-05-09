using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Bislerium.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class BlogReactionController : Controller
    {
        private readonly IReactionService _reactionService;
        private readonly TokenService tokenService;
        public BlogReactionController(IReactionService reactionService, TokenService tokenService)
        {
            _reactionService = reactionService;
            this.tokenService = tokenService;
        }
        [Authorize(Roles = "Blogger")]
        [HttpPost("addblogreaction/{blogId}")]
        public async Task<IActionResult> AddBlogReaction(string blogId, [FromBody] AddBlogReaction userReaction)
        {
            try
            {
                if (!Enum.TryParse(userReaction.UserReaction, out ReactionType reactionType))
                {
                    return BadRequest("The reaction type is invalid");
                }

                string userId = tokenService.GetUserIdFromToken(HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()!);

                var reaction = await _reactionService.AddBlogReaction(blogId, userId, reactionType)!;
                if (reaction == null)
                {
                    return Ok("Reaction Removed");
                }
                return Ok("Reaction Added");
            }
            catch (Exception ex) { 
                return StatusCode(500, "An error occurred while adding the blog reaction.");
            }
        }
        [Authorize(Roles = "Blogger")]
        [HttpPost("addcommentreaction/{commentId}")]
        public async Task<IActionResult> AddCommentReaction(string commentId, [FromBody] AddBlogReaction userReaction)
        {
            try
            {
                if (!Enum.TryParse(userReaction.UserReaction, out ReactionType reactionType))
                {
                    return BadRequest("The reaction type is invalid");
                }

                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                string userId = tokenService.GetUserIdFromToken(token!);

                var reaction = await _reactionService.AddCommentReaction(commentId, userId, reactionType)!;

                if (reaction == null)
                {
                    return Ok("Reaction removed");
                }
                else
                {
                    return Ok("Reaction added");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the comment reaction.");
            }
        }
    }
}