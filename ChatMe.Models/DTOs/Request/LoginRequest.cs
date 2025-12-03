using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class LoginRequest
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } // Login with Phone

        [Required]
        public string Password { get; set; }
    }
}