using hardware_store_api.Models;

namespace hardware_store_api.Services.ShippingMethodService
{
    public interface IShippingMethodService
    {
        Task<List<ShippingMethod>> GetList();
        Task<ShippingMethod> GetByID(int id);
        Task<ShippingMethod> GetByName(string name);
    }
}
