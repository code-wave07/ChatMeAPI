namespace ChatMe.Models.DTOs.Responses
{
    public class AuthenticationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } // The JWT
        public string UserId { get; set; }
        public string FirstName { get; set; }
    }
}
