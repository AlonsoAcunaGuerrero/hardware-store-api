using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Models.Requests;
using hardware_store_api.Models.Responses;
using hardware_store_api.Services.ProductService;
using hardware_store_api.Services.ProductTypeService;
using hardware_store_api.Services.ShoppingCartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IShoppingCartService _shoppingCartService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService, 
            IProductTypeService productTypeService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _productService = productService;
            _productTypeService = productTypeService;
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = -1, int pageSize = 10)
        {
            var list = await _productService.GetList();

            if (page == -1)
            {
                return Ok(new
                {
                    total = 1,
                    listItems = list
                });
            }
            else if (page >= 1)
            {
                var totalPages = (int) Math.Ceiling((decimal)list.Count / pageSize);
                var productsList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                if (page > totalPages)
                {
                    return BadRequest(new APIResponse(400, String.Format("The page {0} is not valid.", page)));
                }
                else
                {
                    return Ok(new
                    {
                        total = totalPages,
                        listItems = productsList
                    });
                }
            }
            else
            {
                return BadRequest(new APIResponse(400, String.Format("The page value can't be least of -1.")));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByID([FromRoute] int id)
        {
            if (id < 1)
            {
                return BadRequest(new APIResponse(400, "The product ID is not valid."));
            }

            var product = await _productService.GetByID(id);

            return Ok(product);
        }

        [HttpGet("find/{name}")]
        public async Task<IActionResult> GetProductByName([FromRoute(Name = "name")] string productName)
        {
            var product = await _productService.GetByName(productName);

            return Ok(product);
        }

        [HttpGet("search/{search}")]
        public async Task<ActionResult<List<Product>>> GetSearchProducts([FromRoute] string search, int page = -1, int pageSize = 10)
        {
            var list = await _productService.GetListSearch(search);

            if(page == -1 && list.Count > 0)
            {
                return Ok(new
                {
                    total = 1,
                    listItems = list
                });
            }
            else if (page >= 1 && list.Count > 0)
            {
                var totalPages = (int)Math.Ceiling((decimal)list.Count / pageSize);
                var productsList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                if (page > totalPages)
                {
                    return BadRequest(new APIResponse(400, String.Format("The page {0} is not valid.", page)));
                }
                else
                {
                    return Ok(new
                    {
                        total = totalPages,
                        listItems = productsList
                    });
                }
            }
            else if (page < -1)
            {
                return BadRequest(new APIResponse(400, String.Format("The page value can't be least of -1.")));
            }
            else
            {
                return NotFound(new APIResponse(404, "Not exist products with that searchs parameters."));
            }
        }

        [HttpGet("type/{typeName}")]
        public async Task<ActionResult<List<Product>>> GetAllByType([FromRoute] string typeName, int page = -1, int pageSize = 10)
        {
            var type = await _productTypeService.GetByName(typeName);

            var list = await _productService.GetListByType(type);
            
            if (page == -1 && list.Count > 0)
            {
                return Ok(new
                {
                    total = 1,
                    listItems = list
                });
            }
            else if (page >= 1 && list.Count > 0)
            {
                var totalPages = (int)Math.Ceiling((decimal)list.Count / pageSize);
                var productsList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                if (page > totalPages)
                {
                    return BadRequest(new APIResponse(400, String.Format("The page {0} is not valid.", page)));
                }
                else
                {
                    return Ok(new
                    {
                        total = totalPages,
                        listItems = productsList
                    });
                }
            }
            else if(page < -1)
            {
                return BadRequest(new APIResponse(400, String.Format("The page value can't be least of -1.")));
            }
            else
            {
                return NotFound(new APIResponse(404, String.Format("The products of type {0} not exist.", typeName)));
            }
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpPost("admin/add")]
        public async Task<IActionResult> CreateProduct([FromForm] InsertProductRequest request)
        {
            string[] allowedFileExtensions = new string[] { ".jpg", ".png", ".jpeg" };
            var ext = Path.GetExtension(request.Image.FileName);

            if (!allowedFileExtensions.Contains(ext.ToLower()))
            {
                return BadRequest(new APIResponse(400, "The image file is not valid."));
            }

            var productType = await _productTypeService.GetByName(request.Type);

            byte[] imageFileBytes;

            using (var ms = new MemoryStream())
            {
                request.Image.CopyTo(ms);
                imageFileBytes = ms.ToArray();
            }

            var product = await _productService.Insert(new Product(0, request.Name, request.Description,
                request.Price, request.Stock, imageFileBytes, productType));

            _logger.LogInformation(String.Format("[{0}] Product with name {1} was added successfully.", DateTime.Now, product.Name));
            return Ok(new InsertProductResponse(product.Id, product.Name, product.Description, product.Price,
                product.Stock, Convert.ToBase64String(product.Image), product.Type));
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpPut("admin/update/{id}")]
        public async Task<IActionResult> UpdateProductByID([FromRoute] int id, [FromForm] InsertProductRequest request)
        {
            string[] allowedFileExtensions = new string[] { ".jpg", ".png", ".jpeg" };
            var ext = Path.GetExtension(request.Image.FileName);

            if (!allowedFileExtensions.Contains(ext))
            {
                return BadRequest(new APIResponse(400, "The image file is not valid."));
            }

            var oldProduct = await _productService.GetByID(id);

            byte[] imageFileBytes;

            using (var ms = new MemoryStream())
            {
                request.Image.CopyTo(ms);
                imageFileBytes = ms.ToArray();
            }

            var productType = await _productTypeService.GetByName(request.Type);

            var newProduct = await _productService.Update(new Product(id, request.Name, request.Description,
                request.Price, request.Stock, imageFileBytes, productType));
            
            return Ok(new UpdateProductResponse(oldProduct, newProduct));
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpDelete("admin/delete/{id}")]
        public async Task<IActionResult> DeleteProductByID([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation(String.Format("[{0}]Trying to delete a product...", DateTime.Now));
                if (id < 1)
                {
                    _logger.LogError(String.Format("[{0}]...product not found.", DateTime.Now));
                    return BadRequest(new APIResponse(400, "The product ID couldn't be 0 or negative."));
                }

                var product = await _productService.GetByID(id);

                _logger.LogInformation(String.Format("[{0}]...product with ID [{1}] was deleted.", DateTime.Now, product.Id));
                
                await _shoppingCartService.DeleteByProduct(product);
                await _productService.Delete(product);

                return Ok(product);
            }catch(Exception ex)
            {
                _logger.LogError(String.Format("[{0}]...product not found.", DateTime.Now));
                throw ex;
            }
        }
    }
}
