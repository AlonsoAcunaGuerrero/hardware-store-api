using hardware_store_api.Models;

namespace hardware_store_api.Services.ShopOrderService
{
    public interface IShopOrderService
    {
        Task<ShopOrder> Insert(ShopOrder shopOrder);
        Task<List<ShopOrder>> GetListByUser(User user);
        Task<ShopOrder> GetByID(string id);
        Task<ShopOrder> GetByUser(User user);
        Task<List<ShopOrder>> GetListByStatus(OrderStatus orderStatus);
    }
}
