namespace ChatMe.Models.DTOs.Responses;

public class UserSearchResponse
{
    public string UserId { get; set; } // The UI needs this to start a chat
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}
