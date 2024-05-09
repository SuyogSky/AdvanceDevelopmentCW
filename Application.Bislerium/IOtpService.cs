using Domain.Bislerium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IOtpService
    {
        Task<(bool success, string otp)> GenerateOtpAsync(AppUser user);
        Task<bool> VerifyOtpAsync(string userId, string otp);
    }

}
