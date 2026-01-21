using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    public string FirstName { get; set; }
    
    public string? LastName { get; set; }

    
    [EmailAddress]
    public string? Email { get; set; } // email for recovery/notifications

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } // Used for Login

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}