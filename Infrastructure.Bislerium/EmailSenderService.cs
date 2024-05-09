using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Application.Bislerium;

namespace Infrastructure.Bislerium
{
    public class EmailSenderService : IEmailCustomSender
    {
        public  async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("timsinarewon3@gmail.com", "jyhgnuwyaagcwcvv");
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("noreply@example.com"),
                    Subject = subject,
                    Body = message
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }

}
