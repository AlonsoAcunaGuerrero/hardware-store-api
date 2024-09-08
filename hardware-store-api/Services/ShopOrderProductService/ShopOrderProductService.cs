using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Services.ShopOrderService;
using hardware_store_api.Services.ProductService;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace hardware_store_api.Services.ShopOrderProductService
{
    public class ShopOrderProductService : IShopOrderProductService
    {
        private readonly IShopOrderService _orderService;
        private readonly IProductService _productService;

        public ShopOrderProductService(IShopOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<List<ShopOrderProduct>> GetListByOrder(ShopOrder order)
        {
            var orderProductsList = new List<ShopOrderProduct>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListShopOrderProductsByOrder", sql))
                    {
                        cmd.Parameters.AddWithValue("ShopOrderID", order.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                string idShopOrder = reader.GetString("id_shop_order");
                                var currentOrder = await _orderService.GetByID(idShopOrder);

                                int idProduct = reader.GetInt32("id_product");
                                var currentProduct = await _productService.GetByID(idProduct);

                                int quantity = reader.GetInt32("quantity");

                                var orderProduct = new ShopOrderProduct(currentOrder, currentProduct, quantity);

                                orderProductsList.Add(orderProduct);
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
                    "Database Error, list of products of order couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError,
                    "Error getting list of products of order.");
            }

            return orderProductsList;
        }

        public async Task<ShopOrderProduct> GetByOrderProduct(ShopOrder order, Product product)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShopOrderProduct", sql))
                    {
                        cmd.Parameters.AddWithValue("ShopOrderID", order.Id);
                        cmd.Parameters.AddWithValue("ProductID", product.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            if (await reader.ReadAsync())
                            {
                                string idShopOrder = reader.GetString("id_shop_order");
                                var currentOrder = await _orderService.GetByID(idShopOrder);

                                int idProduct = reader.GetInt32("id_product");
                                var currentProduct = await _productService.GetByID(idProduct);

                                int quantity = reader.GetInt32("quantity");

                                return new ShopOrderProduct(currentOrder, currentProduct, quantity);
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
                    String.Format("Database Error, order with ID '{0}' and product ID '{1}' couldn't be get.", order.Id, product.Id));
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, 
                    String.Format("Error getting order with ID '{0}' and product ID '{1}'.", order.Id, product.Id));
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, "The order with that product not exist.");
        }

        public async Task<ShopOrderProduct> Insert(ShopOrderProduct orderProduct)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertShopOrderProduct", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("ShopOrderID", orderProduct.Order.Id);
                        cmd.Parameters.AddWithValue("ProductID", orderProduct.Product.Id);
                        cmd.Parameters.AddWithValue("ProductQuantity", orderProduct.Quantity);
                        
                        await cmd.ExecuteNonQueryAsync();
                        return await GetByOrderProduct(orderProduct.Order, orderProduct.Product);
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
                    String.Format("Database Error, product couldn't be added to the order."));
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error adding product to order.");
            }
        }
    }
}
