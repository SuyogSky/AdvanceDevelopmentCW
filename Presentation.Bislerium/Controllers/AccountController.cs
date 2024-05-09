using Domain.Bislerium;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Reflection.Metadata;
using Application.Bislerium;
using Microsoft.AspNetCore.Identity.UI.Services;
using Infrastructure.Bislerium;
namespace Presentation.Bislerium.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IOtpService _otpService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailCustomSender _emailSender;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment, IOtpService service, IEmailCustomSender emailSenderService)
        {
            _userManager = userManager;
            _otpService = service;
            _roleManager = roleManager;
            _environment = environment;
            _emailSender = emailSenderService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailToOtpModel emailToOtp)
        {
            var user = await _userManager.FindByEmailAsync(emailToOtp.Email);
            if (user == null)
                return BadRequest("No user associated with the email address.");

            var (success, otp) = await _otpService.GenerateOtpAsync(user);
            if (!success)
                return BadRequest("Error generating OTP.");

            // Assuming EmailSender is correctly configured and handles sending emails.
            await _emailSender.SendEmailAsync(emailToOtp.Email, "Password Reset OTP", $"Your OTP is: {otp}");

            return Ok("OTP sent to your email address.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] OtpDetailsModel otpDetails)
        {
            var user = await _userManager.FindByEmailAsync(otpDetails.Email);
            if(user == null)
            {
                return NotFound("User not found.");
            }
            var verifyOtp = await _otpService.VerifyOtpAsync(user!.Id, otpDetails.Otp);
            if (!verifyOtp)
                return BadRequest("Invalid or expired OTP.");

 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Failed to generate password reset token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, otpDetails.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password successfully updated.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.UserName, Email = model.Email, };
            var roleExists = await _roleManager.RoleExistsAsync(model.Role!);


            if (!roleExists)
            {
                return BadRequest("Invalid role specified.");
            }
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                } 
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                user.Image = Url.Content($"~/uploads/{fileName}");
            }
            var result = await _userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role!);
                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors);
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
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return Unauthorized("Invalid user token.");
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password successfully updated.");
        }
           
        }

    }
