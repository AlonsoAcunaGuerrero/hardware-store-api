using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using System.Numerics;

namespace hardware_store_api.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        public async Task<List<ProductType>> GetList()
        {
            var productTypesList = new List<ProductType>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListProductTypes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var idProductType = reader.GetInt32("id_product_types");
                                var typeName = reader.GetString("name");

                                ProductType parent = null;

                                if (!reader.IsDBNull("parent_id"))
                                {
                                    int idParent = reader.GetInt32("parent_id");

                                    parent = await GetByID(idParent);
                                }

                                var productType = new ProductType(idProductType, typeName, parent);

                                productTypesList.Add(productType);
                            }
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, list of product types couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting list of product types.");
            }

            return productTypesList;
        }

        public async Task<ProductType> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetProductTypeByID", sql))
                    {
                        cmd.Parameters.AddWithValue("IDType", id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idProductType = reader.GetInt32("id_product_types");
                                string typeName = reader.GetString("name");

                                ProductType parent = null;

                                if (!reader.IsDBNull("parent_id"))
                                {
                                    int idParent = reader.GetInt32("parent_id");

                                    parent = await GetByID(idParent);
                                }

                                return new ProductType(idProductType, typeName, parent);
                            }
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product type couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting product type by id.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The product type with ID '{id}' not exist.");
        }

        public async Task<ProductType> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetProductTypeByName", sql))
                    {
                        cmd.Parameters.AddWithValue("TypeName", name);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                int idProductType = reader.GetInt32("id_product_types");
                                string typeName = reader.GetString("name");

                                ProductType parent = null;

                                if (!reader.IsDBNull("parent_id"))
                                {
                                    int idParent = reader.GetInt32("parent_id");

                                    parent = await GetByID(idParent);
                                }

                                return new ProductType(idProductType, typeName, parent);
                            }
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product type couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting product type by name.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The product type with name '{name}' not exist.");
        }

        public async Task<ProductType> Insert(ProductType type)
        {
            if (type.Name.Contains('/'))
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "The name of a product type can´t contain characters like '/'.");
            }

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertProductType", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("NewTypeName", type.Name.Trim());

                        var newIDProductType = await cmd.ExecuteScalarAsync() as int?;

                        if (newIDProductType != null)
                        {
                            return await GetByID(newIDProductType.Value);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product type couldn't be saved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error saving product type.");
            }

            return null;
        }

    }
}
