using hardware_store_api.Models;

namespace hardware_store_api.Services.ShopOrderProductService
{
    public interface IShopOrderProductService
    {
        Task<List<ShopOrderProduct>> GetListByOrder(ShopOrder order);
        Task<ShopOrderProduct> GetByOrderProduct(ShopOrder order, Product product);
        Task<ShopOrderProduct> Insert(ShopOrderProduct orderProduct);
    }
}
