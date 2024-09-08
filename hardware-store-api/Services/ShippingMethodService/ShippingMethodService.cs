using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.ShippingMethodService
{
    public class ShippingMethodService : IShippingMethodService
    {
        public async Task<List<ShippingMethod>> GetList()
        {
            var shippingMethodsList = new List<ShippingMethod>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShippingMethods", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                string name = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");

                                var shippingMethod = new ShippingMethod(idShippingMethod, name, description, price);

                                shippingMethodsList.Add(shippingMethod);
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, list of shipping methods couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting list of shipping methods.");
            }

            return shippingMethodsList;
        }
        
        public async Task<ShippingMethod> GetByID(int id)
        {   
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShippingMethodByID", sql))
                    {
                        cmd.Parameters.AddWithValue("ShippingMethodID", id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                string name = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");

                                return new ShippingMethod(idShippingMethod, name, description, price);
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, shipping method couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting shipping method by id.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The shipping method with ID '{id}' not exist.");
        }

        public async Task<ShippingMethod> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShippingMethodByName", sql))
                    {
                        cmd.Parameters.AddWithValue("ShippingMethodName", name);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                string shippingMethodName = reader.GetString("name");
                                string description = reader.GetString("description");
                                decimal price = reader.GetDecimal("price");

                                return new ShippingMethod(idShippingMethod, shippingMethodName, description, price);
                            }
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                   "Database Error, shipping method couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting shipping method by name.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The shipping method with name '{name}' not exist.");
        }
    }
}
