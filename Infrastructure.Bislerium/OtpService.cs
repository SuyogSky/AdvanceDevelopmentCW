using Application.Bislerium;
using Domain.Bislerium;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class OtpService : IOtpService
    {
        private readonly AplicationDBContext _context;

        public OtpService(AplicationDBContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string? otp)> GenerateOtpAsync(AppUser user)
        {
            if (user == null)
            {
                return (false, null);
            }

            var otp = new Random().Next(100000, 999999).ToString(); // 6 digit OTP
            var expiryTime = DateTime.UtcNow.AddMinutes(15); // OTP expires in 15 minutes

            var passwordResetOtp = new PasswordResetOtp
            {
                UserId = user.Id,
                Otp = otp,
                ExpiryTime = expiryTime
            };

            _context.Add(passwordResetOtp);
            await _context.SaveChangesAsync();

            return (true, otp);
        }

        public async Task<bool> VerifyOtpAsync(string userId, string otp)
        {
            var record = await _context.PasswordResetOtps
                                       .FirstOrDefaultAsync(x => x.UserId == userId && x.Otp == otp && x.ExpiryTime > DateTime.UtcNow);

            if (record != null)
            {
                _context.PasswordResetOtps.Remove(record);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
