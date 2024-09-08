using hardware_store_api.Models;
using hardware_store_api.Services.CityService;
using hardware_store_api.Services.RegionService;
using Microsoft.AspNetCore.Mvc;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        private readonly ICityService _cityService;
        private readonly IRegionService _regionService;

        public CityController(ILogger<CityController> logger, ICityService cityService, IRegionService regionService)
        {
            _logger = logger;
            _cityService = cityService;
            _regionService = regionService;
        }

        [HttpGet("{id_region}")]
        public async Task<IActionResult> GetCitiesByRegion([FromRoute(Name = "id_region")] int idRegion)
        {
            var region = await _regionService.GetByID(idRegion);

            var citiesList = await _cityService.GetListByRegion(region);

            if (citiesList.Count == 0)
            {
                return BadRequest(new APIResponse(400, String.Format("The region with ID {0} not exist.", idRegion)));
            }
            else
            {
                return Ok(citiesList);
            }
        }
    }
}
