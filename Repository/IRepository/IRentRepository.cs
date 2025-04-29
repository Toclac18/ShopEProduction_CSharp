using ShopEProduction.Models;

namespace ShopEProduction.Repository.IRepository
{
    public interface IRentRepository
    {
        Task<RentInProcess> createNewRentProcess(RentInProcess rentInProcess);
        Task<RentInProcess> updateRentProcess(RentInProcess rentInProcess);
        //Task<bool> RentProduct(RentInProcess rentInProcess);
        //Task<bool> ExtendRent(int rentId, int duration);
        //Task<bool> ReturnProduct(int rentId);
        //Task<List<RentInProcess>> GetUserRentals(int userId);
    }
}
