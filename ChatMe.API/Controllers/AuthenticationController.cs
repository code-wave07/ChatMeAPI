using ChatMe.Infrastructure.Interfaces;
using ChatMe.Models.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatMe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authenticationService.RegisterUserAsync(request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authenticationService.LoginUserAsync(request);
            if (!response.Success) return Unauthorized(response);
            return Ok(response);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _authenticationService.UpdateUserProfileAsync(userId, request);

            if (!success) return BadRequest("Failed to update profile.");
            return Ok(new { Message = "Profile updated successfully." });
        }
    }
}