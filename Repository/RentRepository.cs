using Microsoft.EntityFrameworkCore;
using ShopEProduction.Models;
using ShopEProduction.Repository.IRepository;

namespace ShopEProduction.Repository
{

    public class RentRepository : IRentRepository
    {
        private readonly ShopEproductionContext _context;
        public RentRepository(ShopEproductionContext context)
        {
            _context = context;
        }

        // Add new rent process
        public async Task<RentInProcess> createNewRentProcess(RentInProcess rentInProcess)
        {
            _context.RentInProcesses.Add(rentInProcess);
            await _context.SaveChangesAsync();
            return rentInProcess;
        }

        public async Task<RentInProcess> updateRentProcess(RentInProcess rentInProcess)
        {
            _context.RentInProcesses.Update(rentInProcess);
            await _context.SaveChangesAsync();
            return rentInProcess;
        }
    }
}
