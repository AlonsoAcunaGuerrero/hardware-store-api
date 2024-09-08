using hardware_store_api.Models;

namespace hardware_store_api.Services.PaymentTypeService
{
    public interface IPaymentTypeService
    {
        Task<PaymentType> GetByName(string name);
        Task<PaymentType> GetByID(int id);
    }
}
