using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Models.Requests;
using hardware_store_api.Models.Responses;
using hardware_store_api.Services.AddressService;
using hardware_store_api.Services.AuthService;
using hardware_store_api.Services.CityService;
using hardware_store_api.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hardware_store_api.Controllers
{

    [ApiController]
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly ILogger<AddressController> _logger;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ICityService _cityService;

        public AddressController(ILogger<AddressController> logger, IAddressService addressService, IUserService userService, 
            IAuthService authService, ICityService cityService)
        {
            _logger = logger;
            _addressService = addressService;
            _userService = userService;
            _authService = authService;
            _cityService = cityService;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddUserAddress([FromBody] InsertUserAddressRequest request)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var city = await _cityService.GetByName(request.CityName);

            var newAddress = await _addressService.Insert(new Address(0, request.UnitNumber, request.StreetNumber,
                request.AddressLine1, request.AddressLine2, request.PostalCode, city));


            await _addressService.LinkToUser(user, newAddress);
       
            var userData = new UserResponse(new User(user.Id, user.FirstName, user.LastName, user.Username, 
                new string('*', user.Password.Length), IUserService.HideEmailCharacters(user.Email), user.Birthdate, 
                user.IsActive, user.Role));

            return Ok(new InsertUserAddressResponse(userData, newAddress));
            
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetAddress()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var addressList = await _addressService.GetListByUser(user);

            return Ok(addressList);
        }
    }
}
