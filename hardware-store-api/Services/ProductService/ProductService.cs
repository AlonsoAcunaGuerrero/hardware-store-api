using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using hardware_store_api.Services.ProductTypeService;

namespace hardware_store_api.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductTypeService _productTypeService;

        public ProductService(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        public async Task<Product> Insert(Product product)
        {
            if (product.Name.Contains('/'))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "The product name can´t contain characters like '/'.");
            }

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertProduct", sql))
                    {
                        
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("NameProduct", product.Name);
                        cmd.Parameters.AddWithValue("DescriptionProduct", product.Description);
                        cmd.Parameters.AddWithValue("PriceProduct", product.Price);
                        cmd.Parameters.AddWithValue("StockProduct", product.Stock);
                        cmd.Parameters.AddWithValue("ImageProduct", product.Image);
                        cmd.Parameters.AddWithValue("IDType", product.Type.Id);

                        var newIdProduct = await cmd.ExecuteScalarAsync() as int?;
                        
                        if (newIdProduct != null)
                        {
                            return await GetByID(newIdProduct.Value);
                        }
                    }
                }
            }
            catch (HttpStatusException ex)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be saved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error saving product.");
            }

            return null;
        }

        public async Task<List<Product>> GetList()
        {
            var productsList = new List<Product>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListProducts", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idProduct = reader.GetInt32("id_products");
                                string productName = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");
                                int stock = reader.GetInt32("stock");
                                byte[] image = (byte[])reader["image"];

                                int idProductType = reader.GetInt32("id_product_type");
                                var productType = await _productTypeService.GetByID(idProductType);

                                var product = new Product(idProduct, productName, description, price, stock, image, productType);

                                productsList.Add(product);
                            }
                        }

                    }
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, products couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting products.");
            }

            return productsList;
        }

        public async Task<List<Product>> GetListByType(ProductType type)
        {
            var productsList = new List<Product>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListProductsByNameType", sql))
                    {
                        cmd.Parameters.AddWithValue("NameType", type.Name);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idProduct = reader.GetInt32("id_products");
                                string productName = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");
                                int stock = reader.GetInt32("stock");
                                byte[] image = (byte[])reader["image"];

                                int idProductType = reader.GetInt32("id_product_type");
                                var productType = await _productTypeService.GetByID(idProductType);

                                var product = new Product(idProduct, productName, description, price, stock, image, productType);

                                productsList.Add(product);
                            }
                        }

                    }
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, products couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting products by type.");
            }

            return productsList;
        }

        public async Task<List<Product>> GetListSearch(string search)
        {
            var productsList = new List<Product>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListProductsSearch", sql))
                    {
                        cmd.Parameters.AddWithValue("searchValue", search.Trim());
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idProduct = reader.GetInt32("id_products");
                                string productName = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");
                                int stock = reader.GetInt32("stock");
                                byte[] image = (byte[])reader["image"];

                                int idProductType = reader.GetInt32("id_product_type");
                                var productType = await _productTypeService.GetByID(idProductType);

                                var product = new Product(idProduct, productName, description, price, stock, image, productType);

                                productsList.Add(product);
                            }
                        }

                    }
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, products couldn't be found.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting products searched.");
            }

            return productsList;
        }

        public async Task<Product> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetProductByID", sql))
                    {
                        cmd.Parameters.AddWithValue("ProductID", id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idProduct = reader.GetInt32("id_products");
                                string productName = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");
                                int stock = reader.GetInt32("stock");
                                byte[] image = (byte[])reader["image"];

                                int idProductType = reader.GetInt32("id_product_type");
                                var productType = await _productTypeService.GetByID(idProductType);

                                return new Product(idProduct, productName, description, price, stock, image, productType);
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The product with ID '{id}' does not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error retrieving product by id.");
            }
        }

        public async Task<Product> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    using (var cmd = new MySqlCommand("GetProductByName", sql))
                    {
                        await sql.OpenAsync();
                        cmd.Parameters.AddWithValue("FindProductName", name);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idProduct = reader.GetInt32("id_products");
                                string productName = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");
                                int stock = reader.GetInt32("stock");
                                byte[] image = (byte[])reader["image"];

                                int idProductType = reader.GetInt32("id_product_type");
                                var productType = await _productTypeService.GetByID(idProductType);

                                return new Product(idProduct, productName, description, price, stock, image, productType);
                            }
                        }

                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The product with name '{name}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error retrieving product by id.");
            }
        }

        public async Task<Product> Update(Product product)
        {
            if (product.Name.Contains('/'))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "The product name can't contain characters like '/'.");
            }

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("UpdateProductByID", sql))
                    {
                        cmd.Parameters.AddWithValue("IDProductUpdate", product.Id);
                        cmd.Parameters.AddWithValue("NameProduct", product.Name);
                        cmd.Parameters.AddWithValue("DescriptionProduct", product.Description);
                        cmd.Parameters.AddWithValue("PriceProduct", product.Price);
                        cmd.Parameters.AddWithValue("StockProduct", product.Stock);
                        cmd.Parameters.AddWithValue("ImageProduct", product.Image);
                        cmd.Parameters.AddWithValue("IDType", product.Type.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                
                return await GetByID(product.Id);
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be updated.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error updating product.");
            }
        }

        public async Task Delete(Product product)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("DeleteProductByID", sql))
                    {
                        cmd.Parameters.AddWithValue("ProductID", product.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be deleted.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error deleting product by id.");
            }
        }
    }
}
