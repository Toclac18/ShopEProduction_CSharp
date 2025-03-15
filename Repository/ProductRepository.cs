using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopEproductionContext _context;
        public ProductRepository(ShopEproductionContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
