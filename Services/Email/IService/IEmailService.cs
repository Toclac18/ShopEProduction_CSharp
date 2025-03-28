using ShopEProduction.Models;

namespace ShopEProduction.Services.Email.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);

        Task<User> GetUserByUserId(int? userId);
    }
}
