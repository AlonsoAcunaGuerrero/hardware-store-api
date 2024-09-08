using hardware_store_api.Models;

namespace hardware_store_api.Services.OrderStatusService
{
    public interface IOrderStatusService
    {
        Task<OrderStatus> GetByID(int id);
        Task<OrderStatus> GetByName(string name);
    }
}
