using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence;
using Merwylan.ExampleApi.Persistence.Entities;
using Merwylan.ExampleApi.Services.Extensions;
using Merwylan.ExampleApi.Shared.Auth;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.IdentityModel.Tokens;
using static BCrypt.Net.BCrypt;
using Action = Merwylan.ExampleApi.Persistence.Entities.Action;

namespace Merwylan.ExampleApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly byte[] _securityKey;

        public UserService(IUserRepository userRepository, byte[] securityKey)
        {
            _userRepository = userRepository;
            _securityKey = securityKey;
        }
        public IEnumerable<UserDto> GetAllUsers()
        {
            return _userRepository.GetAllUsers().Select(user => user.ToDto());
        }

        public UserDto GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if(user == null) throw new UserDoesNotExistException();
            return user.ToDto();
        }

        public async Task<UserDto> AddUserAsync(PostUser user)
        {
            AssertUsernameIsAvailable(user.Username);

            var userToAdd = new User
            {
                Username = user.Username,
                HashedPassword = HashPassword(user.Password),
                Roles = user.Roles.Select(role => new Role{Id = role}).ToArray()
            };

            var addedUser = await _userRepository.AddUserAsync(userToAdd);
            await _userRepository.SaveAsync();
            return addedUser.ToDto();
        }

        public async Task EditUserAsync(PutUser user)
        {
            var actualUser = _userRepository.GetUserById(user.Id);

            if(actualUser == null) throw new UserDoesNotExistException();

            var isUpdateUsernameDesired = !string.IsNullOrEmpty(user.Username) && user.Username != actualUser.Username;
            if (isUpdateUsernameDesired)
            {
                AssertUsernameIsAvailable(user.Username!);
                actualUser.Username = user.Username!;
            }

            var isUpdatePasswordDesired = !Verify(user.Password, actualUser.HashedPassword);
            if (isUpdatePasswordDesired)
            {
                actualUser.HashedPassword = HashPassword(user.Password);
            }

            var isUpdateRolesDesired = !actualUser.Roles.All(x => user.Roles.Contains(x.Id));
            
            if (isUpdateRolesDesired)
            {
                actualUser.Roles = user.Roles.Select(x => new Role {Id = x}).ToArray();
            }

            _userRepository.UpdateUser(actualUser);
            await _userRepository.SaveAsync();
        }

        private void AssertUsernameIsAvailable(string username)
        {
            var isNameAlreadyUsed = _userRepository.GetUserByName(username) != null;
            if (isNameAlreadyUsed)
            {
                throw new UserAlreadyExistsException();
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            if (_userRepository.GetUserById(userId) == null)
            {
                throw new UserDoesNotExistException();
            }

            _userRepository.DeleteUser(userId);
            await _userRepository.SaveAsync();
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress)
        {
            var user = _userRepository.GetUserByName(model.Username);

            if (user == null) throw new UserDoesNotExistException();

            if (!Verify(model.Password, user.HashedPassword)) 
                throw new AuthenticationException();

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // save refresh token
            _userRepository.AddRefreshToken(user.Id, refreshToken);
            await _userRepository.SaveAsync();

            return new AuthenticateResponse(user.ToDto(), jwtToken, refreshToken.Token);
        }

        public IEnumerable<ActionDto> GetActions()
        {
            return _userRepository.GetAllActions().Select(x => x.ToDto());
        }

        public async Task<RoleDto> AddRole(RoleDto role)
        {
            AssertRoleIsAvailable(role);

            var addedRole = await _userRepository.AddRoleAsync(new Role()
            {
                Name = role.Name,
                Actions = role.Actions.Select(x => new Action
                {
                    Id = x.Id,
                    Value = x.Name
                }).ToList()
            });

            await _userRepository.SaveAsync();
            return addedRole.ToDto();
        }

        private void AssertRoleIsAvailable(RoleDto role)
        {
            var isRoleAvailable = _userRepository.GetAllRoles().FirstOrDefault(x => x.Name == role.Name) == null;

            if (!isRoleAvailable) throw new RoleAlreadyExistsException();
        }


        public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress)
        {
            var user = _userRepository.GetUserByRefreshToken(token);

            if (user == null) throw new RefreshTokenNotFoundException();

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) throw new RefreshTokenNotFoundException();

            // Create new token
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // Set old token to revoked
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            // Update the user
            _userRepository.UpdateUser(user);

            var jwtToken = GenerateJwtToken(user); 
            var response = new AuthenticateResponse(user.ToDto(), jwtToken, newRefreshToken.Token);
            
            // Only when everything succeeded, we want to save the database changes
            await _userRepository.SaveAsync();
            return response;
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var user = _userRepository.GetUserByRefreshToken(token);

            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            
            _userRepository.UpdateRefreshToken(user.Id, refreshToken);
            await _userRepository.SaveAsync();

            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_securityKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                IpAddress = ipAddress,
                IsActive = true
            };
        }
    }

}
