using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Bislerium;

[Route("api/[controller]")]
[ApiController]

public class AuthenticationController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> signInManager;
    public AuthenticationController(UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
    {

        _userManager = userManager;
        _configuration = configuration;
        this.signInManager = signInManager;
        _roleManager = roleManager; 
    }
    public record LoginResponse(bool Flag, string Token, string Message,string? role = "", string? name = "", string? image = "", string? userID = "",string? email = "");
    public record UserSession(string? Id, string? Name, string? Email, string? Role);

    [HttpPost("login")]
    public async Task<LoginResponse> Login([FromBody] LoginModel loginUser)
    {
        if (!ModelState.IsValid)
        {
            return new LoginResponse(false, null!, "Login not completed");
        }

        var user = await _userManager.FindByEmailAsync(loginUser.Email!);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginUser.Password!))
        {
            return new LoginResponse(false, null!, "Login not completed");
        }

        var getUser = await _userManager.FindByEmailAsync(loginUser.Email!);
        var getUserRole = await _userManager.GetRolesAsync(getUser!);
        var userSession = new UserSession(getUser!.Id!.ToString(), getUser.UserName, getUser.Email, getUserRole.First());
        string token = GenerateJSONWebToken(userSession);

        return new LoginResponse(true, token!, "Login completed", getUserRole[0]!,getUser.UserName!, getUser.Image!,getUser.Id,getUser.Email);
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
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
   

}
