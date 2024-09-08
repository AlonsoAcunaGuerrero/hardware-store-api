using hardware_store_api.Connect;
using hardware_store_api.Controllers;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Services.CityService;
using hardware_store_api.Services.CountryService;
using hardware_store_api.Services.OrderStatusService;
using hardware_store_api.Services.PaymentTypeService;
using hardware_store_api.Services.RegionService;
using hardware_store_api.Services.ShippingMethodService;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.StoreOrderService
{
    public class StoreOrderService : IStoreOrderService
    {
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly ICityService _cityService;

        public StoreOrderService(IPaymentTypeService paymentTypeService, IShippingMethodService shippingMethodService, 
            IOrderStatusService orderStatusService, ICityService cityService)
        {
            _paymentTypeService = paymentTypeService;
            _shippingMethodService = shippingMethodService;
            _orderStatusService = orderStatusService;
            _cityService = cityService;
        }

        public async Task<List<StoreOrder>> GetListByEmail(string email)
        {
            var ordersList = new List<StoreOrder>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListStoreOrderByEmail", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("Email", email);

                        using(var reader = await cmd.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                int idPaymentType = reader.GetInt32("id_payment_type");
                                var paymentType = await _paymentTypeService.GetByID(idPaymentType);

                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                var shippingMethod = await _shippingMethodService.GetByID(idShippingMethod);

                                int idOrderStatus = reader.GetInt32("id_order_status");
                                var orderStatus = await _orderStatusService.GetByID(idOrderStatus);

                                int idCity = reader.GetInt32("id_city");
                                var city = await _cityService.GetByID(idCity);

                                string idStoreOrder = reader.GetString("id_store_order");

                                string clientFirstName = reader.GetString("client_first_name");
                                string clientLastName = reader.GetString("client_last_name");
                                string clientEmail = reader.GetString("client_email");

                                string address = reader.GetString("address");

                                decimal orderTotal = reader.GetDecimal("order_total");

                                DateTime creationDate = reader.GetDateTime("creation_date");
                                DateTime updateDate = reader.GetDateTime("update_date");


                                var order = new StoreOrder(idStoreOrder, clientFirstName, clientLastName, clientEmail,
                                    paymentType, address, city, shippingMethod, orderTotal, orderStatus, creationDate, updateDate);

                                ordersList.Add(order);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Database Error, order couldn't be created.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error creating a new order.");
            }

            return ordersList;
        }

        public async Task<StoreOrder> Insert(StoreOrder storeOrder)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertStoreOrder", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("StoreOrderID", storeOrder.Id);
                        cmd.Parameters.AddWithValue("NewClientFirstName", storeOrder.ClientFirstName);
                        cmd.Parameters.AddWithValue("NewClientLastName", storeOrder.ClientLastName);
                        cmd.Parameters.AddWithValue("NewClientEmail", storeOrder.ClientEmail);
                        cmd.Parameters.AddWithValue("PaymentTypeID", storeOrder.PaymentType.Id);
                        cmd.Parameters.AddWithValue("OrderAddress", storeOrder.Address);
                        cmd.Parameters.AddWithValue("CityID", storeOrder.City.Id);
                        cmd.Parameters.AddWithValue("ShippingMethodID", storeOrder.ShippingMethod.Id);
                        cmd.Parameters.AddWithValue("NewOrderTotal", storeOrder.OrderTotal);
                        cmd.Parameters.AddWithValue("OrderStatusID", storeOrder.OrderStatus.Id);
                        cmd.Parameters.AddWithValue("OrderCreationDate", storeOrder.CreationDate);
                        cmd.Parameters.AddWithValue("OrderUpdateDate", storeOrder.UpdateDate);

                        await cmd.ExecuteNonQueryAsync();

                        return await GetByID(storeOrder.Id);
                    }
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Database Error, order couldn't be created.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error creating a new order.");
            }
        }

        public async Task<StoreOrder> GetByID(string id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetStoreOrderByID", sql))
                    {
                        cmd.Parameters.AddWithValue("StoreOrderID", id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int idPaymentType = reader.GetInt32("id_payment_type");
                                var paymentType = await _paymentTypeService.GetByID(idPaymentType);

                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                var shippingMethod = await _shippingMethodService.GetByID(idShippingMethod);

                                int idOrderStatus = reader.GetInt32("id_order_status");
                                var orderStatus = await _orderStatusService.GetByID(idOrderStatus);

                                int idCity = reader.GetInt32("id_city");
                                var city = await _cityService.GetByID(idCity);

                                string idStoreOrder = reader.GetString("id_store_order");

                                string clientFirstName = reader.GetString("client_first_name");
                                string clientLastName = reader.GetString("client_last_name");
                                string clientEmail = reader.GetString("client_email");

                                string address = reader.GetString("address");

                                decimal orderTotal = reader.GetDecimal("order_total");

                                DateTime creationDate = reader.GetDateTime("creation_date");
                                DateTime updateDate = reader.GetDateTime("update_date");

                                return new StoreOrder(idStoreOrder, clientFirstName, clientLastName, clientEmail, paymentType, 
                                    address, city, shippingMethod, orderTotal, orderStatus, creationDate, updateDate);
                            }
                        }
                    }
                }

                throw new HttpStatusException(HttpStatusCode.NotFound, $"The order with ID '{id}' not exist.");
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, order couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting order by id.");
            }
        }
    }
}
