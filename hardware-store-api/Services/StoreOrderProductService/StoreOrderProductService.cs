using hardware_store_api.Connect;
using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Services.ShopOrderService;
using hardware_store_api.Services.ProductService;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using hardware_store_api.Services.StoreOrderService;

namespace hardware_store_api.Services.StoreOrderProductService
{
    public class StoreOrderProductService : IStoreOrderProductService
    {
        private readonly IStoreOrderService _storeOrderService;
        private readonly IProductService _productService;

        public StoreOrderProductService(IStoreOrderService storeOrderService, IProductService productService)
        {
            _storeOrderService = storeOrderService;
            _productService = productService;
        }

        public async Task<List<StoreOrderProduct>> GetListByOrder(StoreOrder order)
        {
            var orderProductsList = new List<StoreOrderProduct>();

            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListStoreOrderProductByOrder", sql))
                    {
                        cmd.Parameters.AddWithValue("StoreOrderID", order.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                string idStoreOrder = reader.GetString("id_store_order_product");
                                var currentOrder = await _storeOrderService.GetByID(idStoreOrder);

                                int idProduct = reader.GetInt32("id_product");
                                var currentProduct = await _productService.GetByID(idProduct);

                                int quantity = reader.GetInt32("quantity");

                                var orderProduct = new StoreOrderProduct(currentOrder, currentProduct, quantity);

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

        public async Task<StoreOrderProduct> GetByOrderProduct(StoreOrder order, Product product)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetStoreOrderProduct", sql))
                    {
                        cmd.Parameters.AddWithValue("StoreOrderID", order.Id);
                        cmd.Parameters.AddWithValue("ProductID", product.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string idStoreOrder = reader.GetString("id_store_order_product");
                                var currentOrder = await _storeOrderService.GetByID(idStoreOrder);

                                int idProduct = reader.GetInt32("id_product");
                                var currentProduct = await _productService.GetByID(idProduct);

                                int quantity = reader.GetInt32("quantity");

                                return new StoreOrderProduct(currentOrder, currentProduct, quantity);
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

        public async Task<StoreOrderProduct> Insert(StoreOrderProduct orderProduct)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("InsertStoreOrderProduct", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("StoreOrderID", orderProduct.StoreOrder.Id);
                        cmd.Parameters.AddWithValue("ProductID", orderProduct.Product.Id);
                        cmd.Parameters.AddWithValue("ProductQuantity", orderProduct.Quantity);

                        await cmd.ExecuteNonQueryAsync();
                        return await GetByOrderProduct(orderProduct.StoreOrder, orderProduct.Product);
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
