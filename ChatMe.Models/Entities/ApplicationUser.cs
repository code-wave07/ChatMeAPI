using Microsoft.AspNetCore.Identity;
using ChatMe.Models.Entities;

namespace ChatMe.Models.Entities;

// Inheriting from IdentityUser gives us: Id, UserName, Email, PhoneNumber, PasswordHash, etc.
public class ApplicationUser : IdentityUser<string>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }

    // Navigation Properties
    public ICollection<GroupMember> GroupMemberships { get; set; }
    public ICollection<Message> SentMessages { get; set; }
}