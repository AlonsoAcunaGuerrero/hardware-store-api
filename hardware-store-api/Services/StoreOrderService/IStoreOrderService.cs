using hardware_store_api.Models;

namespace hardware_store_api.Services.StoreOrderService
{
    public interface IStoreOrderService
    {
        Task<List<StoreOrder>> GetListByEmail(string email);
        Task<StoreOrder> Insert(StoreOrder storeOrder);
        Task<StoreOrder> GetByID(string id);
    }
}
