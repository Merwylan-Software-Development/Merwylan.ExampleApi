using System.Collections.Generic;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Shared.Auth;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Services
{
    public interface IUserService
    {
        IEnumerable<UserDto> GetAllUsers();
        UserDto GetUserById(int id);
        Task<UserDto> AddUserAsync(PostUser user);
        Task EditUserAsync(PutUser user);
        Task DeleteUserAsync(int userId);
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
    }
}