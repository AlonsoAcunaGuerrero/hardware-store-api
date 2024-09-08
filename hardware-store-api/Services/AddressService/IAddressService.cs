using hardware_store_api.Models;

namespace hardware_store_api.Services.AddressService
{
    public interface IAddressService
    {
        Task<Address> Insert(Address address);
        Task<Address> GetByID(int id);
        Task<List<Address>> GetListByUser(User user);
        Task LinkToUser(User user, Address address);
        Task UnlinkToUser(User user, Address address);
    }
}
