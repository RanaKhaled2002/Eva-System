using Eva_BLL.Interfaces;
using Eva_DAL.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Eva_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            var userDto = new UserDTO
            {
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };

            return Ok(userDto);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromServices] ITokenService blacklistService)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Token is missing or invalid.");

            var token = authHeader.Substring("Bearer ".Length);

            var handler = new JwtSecurityTokenHandler();

            // استخدم ReadJwtToken بدل ReadToken
            var jwtToken = handler.ReadJwtToken(token);

            var expiration = jwtToken.ValidTo;

            if (expiration == DateTime.MinValue)
                return BadRequest("Token does not have a valid expiration.");

            await blacklistService.RevokeTokenAsync(token, expiration);

            await _signInManager.SignOutAsync();

            return Ok("You have been logged out successfully.");
        }
    }
}
