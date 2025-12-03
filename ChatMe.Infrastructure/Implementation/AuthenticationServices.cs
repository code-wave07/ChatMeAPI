using ChatMe.Infrastructure.Interfaces;
using ChatMe.Models.DTOs.Requests;
using ChatMe.Models.DTOs.Responses;
using ChatMe.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Required for async queries
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatMe.Infrastructure.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationResponse> RegisterUserAsync(RegisterRequest request)
        {
            // 1. Check if Phone Number already exists (Since we use it for login)
            var userWithPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (userWithPhone != null)
                return new AuthenticationResponse { Success = false, Message = "Phone number is already registered!" };

            // 2. Check email just in case
            var userWithEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithEmail != null)
                return new AuthenticationResponse { Success = false, Message = "Email is already in use!" };

            // 3. Create User
            // Note: We set UserName = PhoneNumber so Identity treats the phone as the unique identifier
            var user = new ApplicationUser
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.PhoneNumber, // CRITICAL: Allows us to use FindByNameAsync with the phone number
                FirstName = request.FirstName,
                LastName = request.LastName
                // Id is generated automatically by the base IdentityUser class
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthenticationResponse { Success = false, Message = errors };
            }

            return new AuthenticationResponse { Success = true, Message = "User created successfully!" };
        }

        public async Task<AuthenticationResponse> LoginUserAsync(LoginRequest request)
        {
            // 1. Find User by Phone Number (Which we saved as UserName)
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);

            if (user == null)
                return new AuthenticationResponse { Success = false, Message = "Invalid phone number or password." };

            // 2. Check Password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return new AuthenticationResponse { Success = false, Message = "Invalid phone number or password." };

            // 3. Generate Token
            var token = GenerateJwtToken(user);

            return new AuthenticationResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                UserId = user.Id,
                FirstName = user.FirstName
            };
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtKey = _configuration["JwtSettings:Key"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];

            // Safety check
            if (string.IsNullOrEmpty(jwtKey)) throw new Exception("JWT Key is missing in appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("PhoneNumber", user.PhoneNumber ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}