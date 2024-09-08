using hardware_store_api.Connect;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using hardware_store_api.Services.UserService;
using hardware_store_api.Services.ProductService;

namespace hardware_store_api.Services.ShoppingCartService
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public ShoppingCartService(IUserService userService, IProductService productService)
        {
            _userService = userService;
            _productService = productService;
        }

        public async Task<List<ShoppingCart>> GetListByUser(User user)
        {
            var shoppingCartList = new List<ShoppingCart>();
            
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetListShoppingCartsByUser", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int idProduct = reader.GetInt32("id_product");
                                var product = await _productService.GetByID(idProduct);

                                int quantity = reader.GetInt32("quantity");

                                var shoppingCart = new ShoppingCart(user, product, quantity);

                                shoppingCartList.Add(shoppingCart);
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
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, list of shopping cart items couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error getting list of shopping cart items.");
            }

            return shoppingCartList;
        }

        public async Task<ShoppingCart> GetByUserProduct(User user, Product product)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("GetShoppingCartByUserProduct", sql))
                    {
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("ProductID", product.Id);
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int quantity = reader.GetInt32("quantity");

                                return new ShoppingCart(user, product, quantity);
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
                    "Database Error, shopping cart of the user couldn't be get.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, 
                    "Error getting shopping cart of that user.");
            }

            throw new HttpStatusException(HttpStatusCode.NotFound, 
                $"The product '{product.Name}' not exist in the shopping cart of the user with username '{user.Username}'.");
        }

        public async Task<ShoppingCart> Insert(ShoppingCart cart)
        {
            ShoppingCart newCart = null;

            try
            {
                var shopping_cart = await GetByUserProduct(cart.User, cart.Product);

                newCart = await Update(cart);

            }catch(Exception)
            {
                try
                {
                    using (var sql = new MySqlConnection(ConDB.getConnection()))
                    {
                        await sql.OpenAsync();
                        using (var cmd = new MySqlCommand("InsertShoppingCart", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("UserID", cart.User.Id);
                            cmd.Parameters.AddWithValue("ProductID", cart.Product.Id);
                            cmd.Parameters.AddWithValue("ProductQuantity", cart.Quantity);
                            
                            await cmd.ExecuteNonQueryAsync();
                        }

                        newCart = await GetByUserProduct(cart.User, cart.Product);
                    }
                }
                catch (MySqlException)
                {
                    throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be added to the shopping cart of user.");
                }
                catch (Exception)
                {
                    throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error adding product to shopping cart of user.");
                }
            }

            return newCart;
        }

        public async Task<ShoppingCart> Update(ShoppingCart cart)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("UpdateShoppingCartQuantity", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", cart.User.Id);
                        cmd.Parameters.AddWithValue("ProductID", cart.Product.Id);
                        cmd.Parameters.AddWithValue("ProductQuantity", cart.Quantity);
                        
                        await cmd.ExecuteNonQueryAsync();
                        return await GetByUserProduct(cart.User, cart.Product);
                    }
                }
            }
            catch (HttpStatusException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, shopping cart couldn't be updated.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error updating shopping cart.");
            }
        }
    
        public async Task DeleteAllByUser(User user)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("DeleteShoppingCartByUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, shopping cart of the user couldn't be clean.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error cleaning shopping cart of the user.");
            }
        }

        public async Task DeleteByUserProduct(User user, Product product)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("DeleteShoppingCartProductByUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", user.Id);
                        cmd.Parameters.AddWithValue("ProductID", product.Id);
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product of the shopping cart couldn't be removed.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error removing product of shopping cart.");
            }
        }

        public async Task DeleteByProduct(Product product)
        {
            try
            {
                using (var sql = new MySqlConnection(ConDB.getConnection()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new MySqlCommand("DeleteShoppingCartByProduct", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("ProductID", product.Id);

                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Database Error, product couldn't be deleted in all shopping carts.");
            }
            catch (Exception)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error deleting product of all shopping carts.");
            }
        }
    }
}
