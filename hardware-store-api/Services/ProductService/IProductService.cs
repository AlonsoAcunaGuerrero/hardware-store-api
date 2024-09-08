using hardware_store_api.Models;
using hardware_store_api.Models.Requests;

namespace hardware_store_api.Services.ProductService
{
    public interface IProductService
    {
        Task<Product> Insert(Product product);
        Task<List<Product>> GetList();
        Task<List<Product>> GetListByType(ProductType type);
        Task<List<Product>> GetListSearch(string search);
        Task<Product> GetByID(int id);
        Task<Product> GetByName(string name);
        Task<Product> Update(Product product);
        Task Delete(Product product);
    }
}
