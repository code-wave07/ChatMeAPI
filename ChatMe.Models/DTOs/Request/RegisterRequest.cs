using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } // We still keep email for recovery/notifications

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } // Used for Login

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}