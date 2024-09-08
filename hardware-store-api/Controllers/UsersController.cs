using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using hardware_store_api.Models.Requests;
using hardware_store_api.Models.Responses;
using hardware_store_api.Services.AuthService;
using hardware_store_api.Services.UserRoleService;
using hardware_store_api.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net;

namespace hardware_store_api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;

        public UsersController(ILogger<UsersController> logger, IAuthService authService, 
            IUserService userService, IUserRoleService userRoleService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
            _userRoleService = userRoleService;
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            return Ok(new User(user.Id, user.FirstName, user.LastName, user.Username, new string('*', user.Password.Length),
                    IUserService.HideEmailCharacters(user.Email), user.Birthdate, user.IsActive, user.Role));
        }

        [HttpGet("verify_email/{email}")]
        public async Task<IActionResult> VerifyUserEmail([FromRoute] string email)
        {
            try
            {
                var user = await _userService.GetByEmail(email);

                return BadRequest(new APIResponse(400, "The email is already in use."));
            }catch(HttpStatusException)
            {
                return Ok(true);
            }catch(Exception)
            {
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
        {
            _logger.LogInformation(String.Format("[{0}] User login process has been initialized...", DateTime.Now));

            try
            {
                var user = await _userService.GetUserLogin(request.Email, request.Password);

                if (user.IsActive == false)
                {
                    throw new HttpStatusException(HttpStatusCode.Unauthorized, "The user is disable, you can't login using this user credentials.");
                }

                var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

                _logger.LogInformation(String.Format("[{0}] ...User '{1}' login was success.", DateTime.Now, user.Username));
                var accessToken = _authService.GetToken(user, DateTime.UtcNow.AddHours(8), jwtSecretKey!, user.Role.Name);
                var refreshToken = _authService.GetToken(user, DateTime.UtcNow.AddDays(30), jwtSecretKey!, "Refresh");

                return Ok(new LoginUserResponse(user.Username, user.Role.Name, accessToken, refreshToken));
            }
            catch(Exception ex)
            {
                _logger.LogInformation(String.Format("[{0}] ...User login process fail.", DateTime.Now));
                throw ex;
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            _logger.LogInformation(String.Format("[{0}] A user registration process has been initialized...", DateTime.Now));

            if (!request.Password.Equals(request.Confirmpassword))
            {
                _logger.LogError(String.Format("[{0}] ...The user registration process failed because bad information.", 
                    DateTime.Now));
                return BadRequest(new APIResponse(400, "The both passwords are not equal."));
            }

            try {
                var user = await _userService.GetByEmail(request.Email);

                _logger.LogError(String.Format("[{0}] ...The user registration process failed because the email was already in use.", 
                    DateTime.Now));

                return BadRequest(new APIResponse(400,
                    String.Format("The email '{0}' is taken.", request.Email)));
            }
            catch(HttpStatusException e)
            {}
            catch(Exception e)
            {
                throw e;
            }

            var role = await _userRoleService.GetByName("User");

            var new_user = await _userService.Insert(new User(0, request.FirstName, request.LastName, request.Username,
                request.Password, request.Email, request.Birthdate, true, role));

            _logger.LogInformation(String.Format("[{0}] ...A new user has been register.", DateTime.Now));
            return Ok(new RegisterUserResponse(new_user.FirstName, new_user.LastName, 
                new_user.Username, IUserService.HideEmailCharacters(new_user.Email)));
        }
        
        [Authorize]
        [HttpGet("verify_access_token")]
        public async Task<IActionResult> VerifyAccessToken()
        {
            return Ok();
        }

        [Authorize(Roles = "Refresh")]
        [HttpGet("refresh_access_token")]
        public async Task<IActionResult> GetNewAccessToken()
        {
            _logger.LogInformation(String.Format("[{0}] User trying to generate new token...", DateTime.Now));
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            var newAccessToken = _authService.GetToken(user, DateTime.UtcNow.AddHours(8), jwtSecretKey, user.Role.Name);

            _logger.LogInformation(String.Format("[{0}] ...User generate new token successfully", DateTime.Now));
            return Ok(new
            {
                newAccesToken = newAccessToken,
                date = DateTime.UtcNow,
            });
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpGet("admin/verify")]
        public async Task<IActionResult> VerifyHighAccess()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpGet("admin/valid_roles")]
        public async Task<IActionResult> GetAdminUserRoles()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var rolesList = await _userRoleService.GetList();

            var validRolesList = new List<UserRole>();

            foreach (var role in rolesList)
            {
                if(role.Id > user.Role.Id)
                {
                    validRolesList.Add(role);
                }
            }

            return Ok(validRolesList);
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpPost("admin/add")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var role = await _userRoleService.GetByName(request.Role);

            if (user.Role.Id >= role.Id)
            {
                return BadRequest(new APIResponse(400, "A user with an inferior role can't register a user with superior or same role."));
            }

            try
            {
                var newUserEmail = await _userService.GetByEmail(request.Email);

                return BadRequest(new APIResponse(400, String.Format("The email '{0}' is taken.", request.Email)));
            }
            catch (HttpStatusException e)
            { }
            catch (Exception e)
            {
                throw e;
            }

            var newUser = await _userService.Insert(new User(0, request.FirstName, request.LastName, request.Username, 
                request.Password, request.Email, request.Birthdate, true, role));

            return Ok(new RegisterUserResponse(newUser.FirstName, newUser.LastName, 
                newUser.Username, newUser.Email));
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpPut("admin/disable/{id}")]
        public async Task<IActionResult> DisableUser([FromRoute] int id)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var targetUser = await _userService.GetByID(id);
            
            if(user.Role.Id >= targetUser.Role.Id)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, 
                    "A user with an inferior role can't disable a user with superior or same role.");
            }

            var updatedUser = await _userService.UpdateActive(targetUser, false);
            
            return Ok(new DisableUserResponse(
                new RegisterUserResponse(targetUser.FirstName, targetUser.LastName, targetUser.Username,
                    targetUser.Email),
                new RegisterUserResponse(updatedUser.FirstName, updatedUser.LastName, updatedUser.Username,
                    updatedUser.Email)
            ));
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpPut("admin/activate/{id}")]
        public async Task<IActionResult> ActivateUser([FromRoute] int id)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var targetUser = await _userService.GetByID(id);

            if(user.Role.Id >= targetUser.Role.Id)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "A user with an inferior role can't activate a user with superior or same role.");
            }

            var updatedUser = await _userService.UpdateActive(targetUser, true);

            return Ok(new DisableUserResponse(
                new RegisterUserResponse(targetUser.FirstName, targetUser.LastName, targetUser.Username,
                    targetUser.Email),
                new RegisterUserResponse(updatedUser.FirstName, updatedUser.LastName, updatedUser.Username,
                    updatedUser.Email)
            ));
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpGet("admin/list")]
        public async Task<IActionResult> GetListUsers()
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenEmail = _authService.GetTokenEmail(identity);

            var user = await _userService.GetByEmail(tokenEmail);

            var usersList = await _userService.GetListUsers();

            foreach(var u in usersList)
            {
                if (u.Id == user.Id)
                {
                    usersList.Remove(u);
                    break;
                }
            }

            return Ok(usersList);
        }

        [Authorize(Roles = "Admin,DepartmentHead,Manager")]
        [HttpGet("admin/find/{id}")]
        public async Task<IActionResult> GetUser([FromRoute(Name = "id")] int id)
        {
            var user = await _userService.GetByID(id);

            return Ok(user);
        }
    }
}
