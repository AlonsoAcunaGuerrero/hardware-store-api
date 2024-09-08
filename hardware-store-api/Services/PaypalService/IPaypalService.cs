using hardware_store_api.Models;
using hardware_store_api.Models.Paypal;
using System.Collections;

namespace hardware_store_api.Services.PaypalService
{
    public interface IPaypalService
    {
        Task<PaypalToken> Authentication(string client_id, string secret_key);
        Task<PaypalOrderDetails> GetOrder(string access_token, string order_id);
        Task<PaypalOrder> CreateOrder(string access_token, List<PaypalProductModel> productsList);
    }
}
