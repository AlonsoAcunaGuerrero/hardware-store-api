using hardware_store_api.Services.ShippingMethodService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/shipping_method")]
    public class ShippingMethodController : ControllerBase
    {
        private readonly ILogger<ShippingMethodController> _logger;
        private readonly IShippingMethodService _shippingMethodService;

        public ShippingMethodController(ILogger<ShippingMethodController> logger, IShippingMethodService shippingMethodService)
        {
            _logger = logger;
            _shippingMethodService = shippingMethodService;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetShippingMethods()
        {
            var shippingMethodsList = await _shippingMethodService.GetList();
            
            return Ok(shippingMethodsList);
        }
    }
}
