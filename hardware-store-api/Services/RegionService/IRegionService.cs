using hardware_store_api.Models;

namespace hardware_store_api.Services.RegionService
{
    public interface IRegionService
    {
        Task<Region> GetByID(int id);
        Task<List<Region>> GetListByCountry(Country country);
        Task<Region> GetByName(string name);
    }
}
