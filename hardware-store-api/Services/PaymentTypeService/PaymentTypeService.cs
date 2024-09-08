using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.PaymentTypeService
{
    public class PaymentTypeService : IPaymentTypeService
    {
        public async Task<PaymentType> GetByName(string name)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetPaymentTypeByName", sql))
                    {
                        cmd.Parameters.AddWithValue("PaymentTypeName", name);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idPaymentType = reader.GetInt32("id_payment_type");
                                string newName = reader.GetString("name");

                                return new PaymentType(idPaymentType, newName);
                            }
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, payment type couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting payment type.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The payment type with name '{name}' not exist.");
        }

        public async Task<PaymentType> GetByID(int id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetPaymentTypeByID", sql))
                    {
                        cmd.Parameters.AddWithValue("PaymentTypeByID", id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idPaymentType = reader.GetInt32("id_payment_type");
                                string name = reader.GetString("name");

                                return new PaymentType(idPaymentType, name);
                            }
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, payment type couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting payment type.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The payment type with ID '{id}' not exist.");
        }
    }
}
