using hardware_store_api.Models;

namespace hardware_store_api.Services.UserRoleService
{
    public interface IUserRoleService
    {
        Task<List<UserRole>> GetList();
        Task<UserRole> GetByName(string name);
        Task<UserRole> GetByID(int id);
    }
}
