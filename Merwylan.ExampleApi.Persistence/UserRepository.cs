using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Merwylan.ExampleApi.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly ExampleContext _context;

        public UserRepository(ExampleContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.Include(x=>x.Roles).ThenInclude(x=>x.Actions);
        }

        public User? GetUserById(int id)
        {
            return _context.Users
                .Include(x => x.Roles)
                .ThenInclude(x => x.Actions)
                .FirstOrDefault(user => user.Id == id);
        }

        public User? GetUserByName(string userName)
        {
            return _context.Users.FirstOrDefault(user => user.Username == userName);
        }

        public User? GetUserByRefreshToken(string refreshToken)
        {
            return _context.Users.FirstOrDefault(user => user.RefreshTokens.Select(r => r.Token).Contains(refreshToken));
        }

        public async Task<User> AddUserAsync(User user)
        {
            var insertedUser = await _context.Users.AddAsync(user);
            return insertedUser.Entity;
        }

        public void AddRefreshToken(int userId, RefreshToken refreshToken)
        {
            _context.Users.FirstOrDefault(user => userId == user.Id)?.RefreshTokens.Add(refreshToken);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void UpdateRefreshToken(int userId, RefreshToken refreshToken)
        {
            _context.Users.FirstOrDefault(user => userId == user.Id)?.RefreshTokens?.Add(refreshToken);
        }
        
        public void DeleteUser(int id)
        {
            var userToDelete = _context.Users.FirstOrDefault(user => user.Id == id);

            if(userToDelete != default)
                _context.Users.Remove(userToDelete);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
