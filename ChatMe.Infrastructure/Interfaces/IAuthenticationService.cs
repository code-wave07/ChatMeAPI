using ChatMe.Models.DTOs.Requests;
using ChatMe.Models.DTOs.Responses;
using System.Threading.Tasks;

namespace ChatMe.Infrastructure.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> RegisterUserAsync(RegisterRequest request);
        Task<AuthenticationResponse> LoginUserAsync(LoginRequest request);
    }
}
