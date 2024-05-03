using Domain.Bislerium;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Presentation.Bislerium.Controllers
{
    [ApiController]
    [Route("accounts/[controller]")]

    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;

        public record LoginResponse(bool Flag, string Token, string Message);
        public record UserSession(string? Id, string? Name, string? Email, string? Role);

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterBloggerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var roleExists = await _roleManager.RoleExistsAsync(model.Role!);


            if (!roleExists)
            {
                return BadRequest("Invalid role specified.");
            }

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role!);



                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors);
        }


        private string GenerateJSONWebToken(UserSession userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
               new Claim(ClaimTypes.Email, userInfo.Email!),
               new Claim(ClaimTypes.Name, userInfo.Name!),
               new Claim(ClaimTypes.NameIdentifier, userInfo.Id!),
               new Claim(ClaimTypes.Role, userInfo.Role!),

            };
            var token = new JwtSecurityToken(
              issuer: _configuration["Jwt:Issuer"],
              audience: _configuration["Jwt:Audience"],
              claims: claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("Login")]
        public async Task<LoginResponse> Login([FromBody] LoginModel loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {

                var getUser = await _userManager.FindByEmailAsync(loginUser.Email);
                var getUserRole = await _userManager.GetRolesAsync(getUser);
                var userSession = new UserSession(getUser.Id, getUser.UserName, getUser.Email, getUserRole.First());
                string token = GenerateJSONWebToken(userSession);

                return new LoginResponse(true, token!, "Login completed");

            }
            else
            {
                return new LoginResponse(false, null!, "Login not completed");
            }
        }


        [HttpGet("Getuser")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User deleted successfully.");
            }
            return BadRequest(result.Errors);
        }

        [HttpPut, Route("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent(string userId, string email, string username, string phoneNumber)
        {
            var student = await _userManager.FindByIdAsync(userId);
            if (student != null)
            {
                student.Email = email;
                student.UserName = username;
                student.PhoneNumber = phoneNumber;
            }

            await _userManager.UpdateAsync(student!);
            return StatusCode(StatusCodes.Status200OK, "Successfully updated");
        }

        [HttpPut, Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string userId, string password)
        {
            var student = await _userManager.FindByIdAsync(userId);
            if (student != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(student);
                var result = await _userManager.ResetPasswordAsync(student, token, password);
            }

            return StatusCode(StatusCodes.Status200OK, "Successfully updated");
        }
    }
}
