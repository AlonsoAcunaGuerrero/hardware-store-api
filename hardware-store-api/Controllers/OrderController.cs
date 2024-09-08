using Microsoft.AspNetCore.Mvc;
using hardware_store_api.Models;
using hardware_store_api.Services.PaypalService;
using hardware_store_api.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using hardware_store_api.Models.Responses;
using hardware_store_api.Services.UserService;
using hardware_store_api.Services.ProductService;
using hardware_store_api.Services.ProductTypeService;
using hardware_store_api.Services.ShopOrderService;
using hardware_store_api.Services.AuthService;
using System.Security.Claims;
using hardware_store_api.Services.AddressService;
using hardware_store_api.Services.ShippingMethodService;
using hardware_store_api.Models.Paypal;
using hardware_store_api.Services.ShoppingCartService;
using hardware_store_api.Services.OrderStatusService;
using hardware_store_api.Services.ShopOrderProductService;
using hardware_store_api.Services.StoreOrderService;
using hardware_store_api.Services.PaymentTypeService;
using hardware_store_api.Services.CityService;
using hardware_store_api.Services.StoreOrderProductService;

namespace hardware_store_api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IPaypalService _paypalService;
        private readonly IProductService _productService;
        private readonly IPaymentTypeService _paymentTypeService;

        private readonly ICityService _cityService;

        private readonly IShopOrderService _shopOrderService;
        private readonly IShopOrderProductService _shopOrderProductService;
        private readonly IStoreOrderService _storeOrderService;
        private readonly IStoreOrderProductService _storeOrderProductService;
        private readonly IOrderStatusService _orderStatusService;

        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IShoppingCartService _shoppingCartService;

        public OrderController(ILogger<OrderController> logger, IPaypalService paypalService, IProductService productService, IPaymentTypeService paymentTypeService, ICityService cityService, IShopOrderService shopOrderService, IShopOrderProductService shopOrderProductService, IStoreOrderService storeOrderService, IStoreOrderProductService storeOrderProductService, IOrderStatusService orderStatusService, IUserService userService, IAuthService authService, IAddressService addressService, IShippingMethodService shippingMethodService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _paypalService = paypalService;
            _productService = productService;
            _paymentTypeService = paymentTypeService;
            _cityService = cityService;
            _shopOrderService = shopOrderService;
            _shopOrderProductService = shopOrderProductService;
            _storeOrderService = storeOrderService;
            _storeOrderProductService = storeOrderProductService;
            _orderStatusService = orderStatusService;
            _userService = userService;
            _authService = authService;
            _addressService = addressService;
            _shippingMethodService = shippingMethodService;
            _shoppingCartService = shoppingCartService;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetUserListOrders()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var userOrdersList = await _shopOrderService.GetListByUser(user);
            var storeOrders = await _storeOrderService.GetListByEmail(user.Email);

            List<OrderResponse> ordersList = new List<OrderResponse>();

            foreach (var order in userOrdersList)
            {
                ordersList.Add(new OrderResponse(order.Id, order.User, order.PaymentType, order.Address, 
                    order.ShippingMethod, null, order.OrderTotal, order.CreationDate, order.UpdateDate, order.OrderStatus));
            }

            foreach(var order in storeOrders)
            {
                ordersList.Add(new OrderResponse(order.Id, order.ClientFirstName, order.ClientLastName, order.ClientEmail, 
                    order.PaymentType, order.Address, order.City, order.ShippingMethod, null, order.OrderTotal, order.CreationDate, 
                    order.UpdateDate, order.OrderStatus));
            }

            return Ok(ordersList);
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserOrder([FromRoute] string id)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            try
            {
                var order = await _shopOrderService.GetByID(id);

                var orderProductsList = await _shopOrderProductService.GetListByOrder(order);

                var productsList = new List<ProductQuantityResponse>();

                foreach (var orderProduct in orderProductsList)
                {
                    var productQuantity = new ProductQuantityResponse(orderProduct.Product, orderProduct.Quantity);

                    productsList.Add(productQuantity);
                }

                var response = new OrderResponse(order.Id, order.User, order.PaymentType, order.Address,
                    order.ShippingMethod, productsList, order.OrderTotal, order.CreationDate, order.UpdateDate, order.OrderStatus);

                return Ok(response);
            }
            catch(Exception ex) { throw ex; }

            /*
            try
            {
                var order = await _storeOrderService.GetByID(id);
            }*/
        }

        [Authorize(Roles = "User")]
        [HttpPost("shop/create")]
        public async Task<IActionResult> CreateNewShopOrder([FromBody] CreateShopOrderRequest request)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            if (request.IdAddress == null || request.IdAddress <= 0)
            {
                return BadRequest(new APIResponse(400, "The address ID is not specified or not valid."));
            }

            if (request.ShippingMethod == null || request.ShippingMethod.Length == 0)
            {
                return BadRequest(new APIResponse(400, "The shipping method ID is not specified or not valid."));
            }

            var address = await _addressService.GetByID(request.IdAddress);

            var shippingMethod = await _shippingMethodService.GetByName(request.ShippingMethod);

            var paypalClientID = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID");
            var paypalSecretID = Environment.GetEnvironmentVariable("PAYPAL_SECRET_KEY");
            var token = await _paypalService.Authentication(paypalClientID, paypalSecretID);

            if(token == null)
            {
                return StatusCode(500, new APIResponse(500, "The order service is not available"));
            }
            
            List<ShoppingCart> shoppingCartProductsList = await _shoppingCartService.GetListByUser(user);

            if(shoppingCartProductsList.Count == 0)
            {
                return BadRequest(new APIResponse(400, "The shopping cart is empty"));
            }

            List<PaypalProductModel> listProducts = new List<PaypalProductModel>();

            decimal orderTotal = shoppingCartProductsList.Sum(product =>
            {
                var newProduct = product.Product;
                var descriptionShort = newProduct.Description.Split('|').First();

                listProducts.Add(new PaypalProductModel(
                    newProduct.Id.ToString(),
                    newProduct.Name,
                    descriptionShort,
                    product.Quantity.ToString(),
                    new PaypalAmountModel("USD", 
                        (newProduct.Price * Convert.ToDecimal(1.12) * Convert.ToDecimal(product.Quantity)).ToString("F"))
                ));

                return newProduct.Price * Convert.ToDecimal(1.12);
            });

            orderTotal += shippingMethod.Price;

            var order = await _paypalService.CreateOrder(token.access_token, listProducts);

            var currentDate = DateTime.UtcNow;

            var shopOrder = await _shopOrderService.Insert(new ShopOrder(order.id, user, new PaymentType(1, ""), address, 
                shippingMethod, orderTotal, new OrderStatus(1, ""), currentDate, currentDate));

            List<ProductQuantityResponse> ListOrderProducts = new List<ProductQuantityResponse>();
            foreach (var shoppingCart in shoppingCartProductsList)
            {
                var order_product = await _shopOrderProductService.Insert(new ShopOrderProduct(shopOrder, shoppingCart.Product, 
                    shoppingCart.Quantity));

                ListOrderProducts.Add(new ProductQuantityResponse(order_product.Product, order_product.Quantity));
            }

            return Ok(new CreateOrderResponse(order.id));
        }

        [Authorize(Roles = "Manager,Supervisor,Employee")]
        [HttpPost("store/create")]
        public async Task<IActionResult> CreateNewStoreOrder([FromBody] CreateStoreOrderRequest request)
        {
            var paymentType = await _paymentTypeService.GetByName("Cash");
            var shippingMethod = await _shippingMethodService.GetByName(request.ShippingMethod.Trim());
            var orderStatus = await _orderStatusService.GetByName("Confirmed");
            var city = await _cityService.GetByName(request.City.Trim());

            var products = request.ProductsList;

            decimal orderTotal = products.Sum(product =>
            {
                var newProduct = _productService.GetByName(product.Name).Result;
                return newProduct.Price * Convert.ToDecimal(product.Quantity);
            });

            var newOrderID = Guid.NewGuid().ToString();
            var currentDate = DateTime.Now;

            var newOrder = await _storeOrderService.Insert(new StoreOrder(newOrderID, request.ClientFirstName.Trim(), 
                request.ClientLastName.Trim(), request.ClientEmail.Trim(), paymentType, request.Address.Trim(), city, 
                shippingMethod, orderTotal, orderStatus, currentDate, currentDate));

            var orderProductsList = new List<ProductQuantityResponse>();

            foreach(var product in products)
            {
                var newProduct = await _productService.GetByName(product.Name);

                var orderProduct = await _storeOrderProductService.Insert(new StoreOrderProduct(newOrder, newProduct, product.Quantity));
                orderProductsList.Add(new ProductQuantityResponse(orderProduct.Product, orderProduct.Quantity));
            }

            return Ok(new OrderResponse(newOrder.Id, newOrder.ClientFirstName, newOrder.ClientLastName, newOrder.ClientEmail,
                newOrder.PaymentType, newOrder.Address, newOrder.City, newOrder.ShippingMethod,
                orderProductsList, newOrder.OrderTotal, newOrder.CreationDate, newOrder.UpdateDate, newOrder.OrderStatus));
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpGet("admin/list/confirmed")]
        public async Task<IActionResult> GetListOrdersConfirmed()
        {
            var confirmedStatus = await _orderStatusService.GetByName("Confirmed");

            var ordersList = await _shopOrderService.GetListByStatus(confirmedStatus);

            return Ok(ordersList);
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpGet("admin/list/processing")]
        public async Task<IActionResult> GetListOrdersProcessing()
        {
            var processingStatus = await _orderStatusService.GetByName("Processing");

            var ordersList = await _shopOrderService.GetListByStatus(processingStatus);

            return Ok(ordersList);
        }
    }
}
