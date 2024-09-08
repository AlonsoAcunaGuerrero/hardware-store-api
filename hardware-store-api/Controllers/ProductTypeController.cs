using hardware_store_api.Models;
using hardware_store_api.Services.ProductTypeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/product_types")]
    public class ProductTypeController : ControllerBase
    {
        private readonly ILogger<ProductTypeController> _logger;
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(ILogger<ProductTypeController> logger, IProductTypeService productTypeService)
        {
            _logger = logger;
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductType>>> GetAll()
        {
            var list = await _productTypeService.GetList();
            
            return Ok(list);
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpPost("admin/add")]
        public async Task<IActionResult> AddType([FromBody] string name)
        {
            _logger.LogInformation(String.Format("[{0}]Trying to add a new product type...", DateTime.Now));
            try
            {
                var newType = await _productTypeService.Insert(new ProductType(0, name));

                _logger.LogInformation(String.Format("[{0}]... Product type with name '{1}' was added successfully.", 
                    DateTime.Now, newType.Name));
                return Ok(newType);
            }
            catch(Exception ex)
            {
                _logger.LogError(String.Format("[{0}]... Error adding a new product type.", DateTime.Now));
                throw ex;
            }
        }
    }
}
