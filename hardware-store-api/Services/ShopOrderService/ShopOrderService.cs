using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Services.AddressService;
using hardware_store_api.Services.OrderStatusService;
using hardware_store_api.Services.PaymentTypeService;
using hardware_store_api.Services.ShippingMethodService;
using hardware_store_api.Services.UserRoleService;
using hardware_store_api.Services.UserService;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.ShopOrderService
{
    public class ShopOrderService : IShopOrderService
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly IAddressService _addressService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IOrderStatusService _orderStatusService;

        public ShopOrderService(IUserService userService, IUserRoleService userRoleService, IPaymentTypeService paymentTypeService, 
            IAddressService addressService, IShippingMethodService shippingMethodService, IOrderStatusService orderStatusService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _paymentTypeService = paymentTypeService;
            _addressService = addressService;
            _shippingMethodService = shippingMethodService;
            _orderStatusService = orderStatusService;
        }

        public async Task<ShopOrder> Insert(ShopOrder shopOrder)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertShopOrder", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("ShopOrderID", shopOrder.Id);
                        cmd.Parameters.AddWithValue("UserID", shopOrder.User.Id);
                        cmd.Parameters.AddWithValue("UserAddressID", shopOrder.Address.Id);
                        cmd.Parameters.AddWithValue("ShippingMethodID", shopOrder.ShippingMethod.Id);
                        cmd.Parameters.AddWithValue("ShopOrderTotal", shopOrder.OrderTotal);
                        cmd.Parameters.AddWithValue("CreationShopOrderDate", shopOrder.CreationDate);
                        cmd.Parameters.AddWithValue("UpdateShopOrderDate", shopOrder.UpdateDate);

                        await cmd.ExecuteNonQueryAsync();

                        return await GetByID(shopOrder.Id);
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

        public async Task<List<ShopOrder>> GetListByUser(User user)
        {
            List<ShopOrder> ordersList = new List<ShopOrder>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListShopOrdersByUser", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                int idPaymentType = reader.GetInt32("id_payment_type");
                                var paymentType = await _paymentTypeService.GetByID(idPaymentType);
                                
                                int idAddress = reader.GetInt32("id_address");
                                var address = await _addressService.GetByID(idAddress);

                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                var shippingMethod = await _shippingMethodService.GetByID(idShippingMethod);

                                int idOrderStatus = reader.GetInt32("id_order_status");
                                var orderStatus = await _orderStatusService.GetByID(idOrderStatus);

                                string idShopOrder = reader.GetString("id_shop_order");
                                decimal orderTotal = reader.GetDecimal("order_total");

                                DateTime creationDate = reader.GetDateTime("creation_date");
                                DateTime updateDate = reader.GetDateTime("update_date");

                                var order = new ShopOrder(idShopOrder, user, paymentType, address, shippingMethod, 
                                    orderTotal, orderStatus, creationDate, updateDate);

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
                    "Database Error, orders of user couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Error getting orders of user.");
            }

            return ordersList;
        }

        public async Task<ShopOrder> GetByID(string id)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShopOrderByID", sql))
                    {
                        cmd.Parameters.AddWithValue("OrderID", id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                int idUser = reader.GetInt32("id_user");
                                var user = await _userService.GetByID(idUser);

                                int idPaymentType = reader.GetInt32("id_payment_type");
                                var paymentType = await _paymentTypeService.GetByID(idPaymentType);

                                int idAddress = reader.GetInt32("id_address");
                                var address = await _addressService.GetByID(idAddress);

                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                var shippingMethod = await _shippingMethodService.GetByID(idShippingMethod);

                                int idOrderStatus = reader.GetInt32("id_order_status");
                                var orderStatus = await _orderStatusService.GetByID(idOrderStatus);

                                string idShopOrder = reader.GetString("id_shop_order");
                                decimal orderTotal = reader.GetDecimal("order_total");

                                DateTime creationDate = reader.GetDateTime("creation_date");
                                DateTime updateDate = reader.GetDateTime("update_date");

                                return new ShopOrder(idShopOrder, user, paymentType, address, shippingMethod, 
                                    orderTotal, orderStatus, creationDate, updateDate);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, order couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting order by id.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, $"The order with ID '{id}' not exist.");
        }

        public async Task<ShopOrder> GetByUser(User user)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShopOrderByUser", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                int idPaymentType = reader.GetInt32("id_payment_type");
                                var paymentType = await _paymentTypeService.GetByID(idPaymentType);

                                int idAddress = reader.GetInt32("id_address");
                                var address = await _addressService.GetByID(idAddress);

                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                var shippingMethod = await _shippingMethodService.GetByID(idShippingMethod);

                                int idOrderStatus = reader.GetInt32("id_order_status");
                                var orderStatus = await _orderStatusService.GetByID(idOrderStatus);

                                string idShopOrder = reader.GetString("id_shop_order");
                                decimal orderTotal = reader.GetDecimal("order_total");

                                DateTime creationDate = reader.GetDateTime("creation_date");
                                DateTime updateDate = reader.GetDateTime("update_date");

                                return new ShopOrder(idShopOrder, user, paymentType, address, shippingMethod,
                                    orderTotal, orderStatus, creationDate, updateDate);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, order couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting order of the user.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, "The order of that user not exist.");
        }
        
        public async Task<List<ShopOrder>> GetListByStatus(OrderStatus orderStatus)
        {
            List<ShopOrder> ordersList = new List<ShopOrder>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListShopOrdersByStatus", sql))
                    {
                        cmd.Parameters.AddWithValue("OrderStatusID", orderStatus.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                int idUser = reader.GetInt32("id_user");
                                var user = await _userService.GetByID(idUser);

                                int idPaymentType = reader.GetInt32("id_payment_type");
                                var paymentType = await _paymentTypeService.GetByID(idPaymentType);

                                int idAddress = reader.GetInt32("id_address");
                                var address = await _addressService.GetByID(idAddress);

                                int idShippingMethod = reader.GetInt32("id_shipping_method");
                                var shippingMethod = await _shippingMethodService.GetByID(idShippingMethod);

                                string idShopOrder = reader.GetString("id_shop_order");
                                decimal orderTotal = reader.GetDecimal("order_total");

                                DateTime creationDate = reader.GetDateTime("creation_date");
                                DateTime updateDate = reader.GetDateTime("update_date");

                                var order = new ShopOrder(idShopOrder, user, paymentType, address, shippingMethod,
                                    orderTotal, orderStatus, creationDate, updateDate);

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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, shop orders couldn't be retrieved.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting shop orders.");
            }

            return ordersList;
        }
    }
}
