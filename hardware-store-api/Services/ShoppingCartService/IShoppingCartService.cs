using hardware_store_api.Models;

namespace hardware_store_api.Services.ShoppingCartService
{
    public interface IShoppingCartService
    {
        Task<List<ShoppingCart>> GetListByUser(User user);
        Task<ShoppingCart> GetByUserProduct(User user, Product product);
        Task<ShoppingCart> Insert(ShoppingCart cart);
        Task<ShoppingCart> Update(ShoppingCart cart);
        Task DeleteAllByUser(User user);
        Task DeleteByUserProduct(User user, Product product);
        Task DeleteByProduct(Product product);
    }
}
