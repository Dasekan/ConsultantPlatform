using ConsultantPlatform.Api.Interfaces;
using ConsultantPlatform.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAuditService auditService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditService = auditService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null) return Unauthorized(new { message = "Wrong email or password" });

            var result = await _signInManager.PasswordSignInAsync(
                user,
                req.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            if (!result.Succeeded) return Unauthorized(new { message = "Wrong email or password" });

            var roles = await _userManager.GetRolesAsync(user);

            await _auditService.LogAsync(
                "Auth",
                0,
                "Login",
                "User logged in successfully.",
                user.Email ?? "Unknown");

            return Ok(new
            {
                email = user.Email,
                roles
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = user?.Email ?? User.Identity?.Name ?? "Unknown";

            await _auditService.LogAsync(
                "Auth",
                0,
                "Logout",
                "User logged out.",
                userEmail);

            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { email = user.Email, roles });
        }
    }
}
