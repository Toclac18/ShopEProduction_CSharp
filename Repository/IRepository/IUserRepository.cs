using ShopEProduction.Models;

namespace ShopEProduction.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<User> RegisterUserAsync(User user);
    }
}
