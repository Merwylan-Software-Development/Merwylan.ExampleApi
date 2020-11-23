using System.Collections.Generic;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence.Entities;

namespace Merwylan.ExampleApi.Persistence
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);
        User? GetUserByName(string userName);
        User? GetUserByRefreshToken(string refreshToken);
        Task<User> AddUserAsync(User user);
        void UpdateUser(User user);
        void AddRefreshToken(int userId, RefreshToken refreshToken);
        void UpdateRefreshToken(int userId, RefreshToken refreshToken);
        void DeleteUser(int id);
        Task SaveAsync();
        void Save();
    }
}