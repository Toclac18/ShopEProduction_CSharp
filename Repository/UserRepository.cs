using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ShopEproductionContext _context;

        public UserRepository(ShopEproductionContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phonenumber == phoneNumber);
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
