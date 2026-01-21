namespace ChatMe.Models.DTOs.Requests
{
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePhotoUrl { get; set; } // Send the URL returned from the Upload endpoint
    }
}

