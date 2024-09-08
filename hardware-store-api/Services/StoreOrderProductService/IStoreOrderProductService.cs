using hardware_store_api.Models;

namespace hardware_store_api.Services.StoreOrderProductService
{
    public interface IStoreOrderProductService
    {
        Task<List<StoreOrderProduct>> GetListByOrder(StoreOrder order);
        Task<StoreOrderProduct> GetByOrderProduct(StoreOrder order, Product product);
        Task<StoreOrderProduct> Insert(StoreOrderProduct orderProduct);
    }
}
