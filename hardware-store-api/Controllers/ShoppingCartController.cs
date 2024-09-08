using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Models.Requests;
using hardware_store_api.Models.Responses;
using hardware_store_api.Services.AuthService;
using hardware_store_api.Services.ProductService;
using hardware_store_api.Services.ShoppingCartService;
using hardware_store_api.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/shopping_cart")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, IAuthService authService, IUserService userService, 
            IProductService productService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        [Authorize(Roles = "User")]
        [HttpGet("products")]
        public async Task<IActionResult> GetShoppingCartItems()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var shoppingCartProductsList = await _shoppingCartService.GetListByUser(user);
            var productsList = new List<Object>();

            foreach (var shoppingCartProduct in shoppingCartProductsList)
            {
                productsList.Add(new
                {
                    id_products = shoppingCartProduct.Product.Id,
                    name = shoppingCartProduct.Product.Name,
                    description = shoppingCartProduct.Product.Description,
                    price = shoppingCartProduct.Product.Price,
                    stock = shoppingCartProduct.Product.Stock,
                    image = shoppingCartProduct.Product.Image,
                    type = shoppingCartProduct.Product.Type,
                    quantity = shoppingCartProduct.Quantity
                });
            }

            return Ok(productsList);
        }

        [Authorize(Roles = "User")]
        [HttpPost("product")]
        public async Task<IActionResult> AddShoppingCartProduct([FromBody] InsertProductShoppingCartRequest request)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var product = await _productService.GetByName(request.Product);

            var shoppingCartProduct = await _shoppingCartService.Insert(new ShoppingCart(user, product, request.Quantity));

            return Ok(new InsertProductShoppingCartResponse(user.Username, product, 
                shoppingCartProduct.Quantity));
        }

        [Authorize(Roles = "User")]
        [HttpDelete("products")]
        public async Task<IActionResult> CleanShoppingCart()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            await _shoppingCartService.DeleteAllByUser(user);

            return Ok(new APIResponse(200, "The shopping cart was successfully cleaned."));
        }

        [Authorize(Roles = "User")]
        [HttpDelete("product/{id}")]
        public async Task<IActionResult> DeleteShoppingCartProduct([FromRoute] int id)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var product = await _productService.GetByID(id);

            await _shoppingCartService.DeleteByUserProduct(user, product);

            return Ok(new APIResponse(200, "The product was successfully removed of the shopping cart."));
        }

        [Authorize(Roles = "User")]
        [HttpPut("product/{id}/{quantity}")]
        public async Task<IActionResult> DeleteShoppingCartProduct([FromRoute] int id, [FromRoute] int quantity)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var product = await _productService.GetByID(id);

            var shoppingCart = await _shoppingCartService.GetByUserProduct(user, product);

            var updatedShoppingCart = await _shoppingCartService.Update(new ShoppingCart(user, product, quantity));

            if (updatedShoppingCart == null)
            {
                return StatusCode(500, new APIResponse(500, "A error ocurred updating you shopping cart."));
            }

            return Ok(new
            {
                before = new { product = shoppingCart.Product, quantity = shoppingCart.Quantity },
                now = new { product = updatedShoppingCart.Product, quantity = updatedShoppingCart.Quantity }
            });
        }
    }
}
