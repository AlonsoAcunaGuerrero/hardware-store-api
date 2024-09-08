using hardware_store_api.Models;

namespace hardware_store_api.Services.CountryService
{
    public interface ICountryService
    {
        Task<Country> GetByID(int id);
        Task<Country> GetByName(string name);
        Task<List<Country>> GetList();
    }
}
