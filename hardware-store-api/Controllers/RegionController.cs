using hardware_store_api.Models;
using hardware_store_api.Services.CountryService;
using hardware_store_api.Services.RegionService;
using Microsoft.AspNetCore.Mvc;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/regions")]
    public class RegionController : ControllerBase
    {
        private readonly ILogger<RegionController> _logger;
        private readonly IRegionService _regionService;
        private readonly ICountryService _countryService;

        public RegionController(ILogger<RegionController> logger, IRegionService regionService, ICountryService countryService)
        {
            _logger = logger;
            _regionService = regionService;
            _countryService = countryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegions([FromRoute] int id)
        {
            var country = await _countryService.GetByID(id);

            var regionsList = await _regionService.GetListByCountry(country);

            if(regionsList.Count == 0)
            {
                return BadRequest(new APIResponse(400, String.Format("The country with ID {0} not exist.", id)));
            }
            else
            {
                return Ok(regionsList);
            }
        }
    }
}
