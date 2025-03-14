using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{
    public class ProductDetailRepository : IProductDetailRepository
    {

        private readonly ShopEProductionContext _context;

        public ProductDetailRepository(ShopEProductionContext context)
        {
            _context = context;
        }
        public async Task<List<ProductDetail>> getAllProductDetail()
        {
            return await _context.ProductDetails.ToListAsync(); 
        }
    }
}
