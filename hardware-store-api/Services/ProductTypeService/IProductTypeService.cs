using hardware_store_api.Models;

namespace hardware_store_api.Services.ProductTypeService
{
    public interface IProductTypeService
    {
        Task<List<ProductType>> GetList();
        Task<ProductType> GetByID(int id);
        Task<ProductType> GetByName(string name);
        Task<ProductType> Insert(ProductType type);
    }
}
