using hardware_store_api.Models;

namespace hardware_store_api.Services.CityService
{
    public interface ICityService
    {
        Task<List<City>> GetListByRegion(Region region);
        Task<City> GetByID(int id);
        Task<City> GetByName(string name);
    }
}
