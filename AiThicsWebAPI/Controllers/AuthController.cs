using aithics.service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace aithics.api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterUserAsync(new aithics.data.Models.User
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email
            }, model.Password);

            return Ok(new { Message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.LoginAsync(model.Email, model.Password);
            return token == "Invalid login attempt"
                ? Unauthorized(new { Message = "Invalid login attempt" })
                : Ok(new { Token = token });
        }
    }

    public class RegisterModel
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
