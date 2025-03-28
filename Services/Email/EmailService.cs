using ShopEProduction.Services.Email.IService;

namespace ShopEProduction.Services.Email
{
    using MailKit.Net.Smtp;
    using Microsoft.EntityFrameworkCore;
    using MimeKit;
    using ShopEProduction.Models;

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ShopEproductionContext _context;

        public EmailService(IConfiguration configuration, ShopEproductionContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]
            ));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["Port"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    emailSettings["Username"],
                    emailSettings["Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task<User> GetUserByUserId(int? userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
