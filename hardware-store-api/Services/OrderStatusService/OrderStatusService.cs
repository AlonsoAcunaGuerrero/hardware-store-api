using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.OrderStatusService
{
    public class OrderStatusService : IOrderStatusService
    {
        public async Task<OrderStatus> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetOrderStatusByID", sql))
                    {
                        cmd.Parameters.AddWithValue("OrderStatusID", id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                int idOrderStatus = reader.GetInt32("id_order_status");
                                string statusName = reader.GetString("status");

                                return new OrderStatus(idOrderStatus, statusName);
                            }
                        }

                    }
                }

                
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, order status couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting order status by id.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The order status with ID '{id}' not exist.");
        }

        public async Task<OrderStatus> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetOrderStatusByName", sql))
                    {
                        cmd.Parameters.AddWithValue("OrderStatusName", name);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                int idOrderStatus = reader.GetInt32("id_order_status");
                                string statusName = reader.GetString("status");

                                return new OrderStatus(idOrderStatus, statusName);
                            }
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, order status couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting order status by name.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The order status with name '{name}' not exist.");
        }
    }
}
