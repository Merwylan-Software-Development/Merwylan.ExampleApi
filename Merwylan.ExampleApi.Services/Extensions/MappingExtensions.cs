using System.Linq;
using Merwylan.ExampleApi.Persistence.Entities;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Services.Extensions
{
    public static class MappingExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                HashedPassword = user.HashedPassword,
                RefreshTokens = user.RefreshTokens.Select(ToDto).ToArray(),
                Roles = user.Roles.Select(x => new RoleDto { Name = x.Name, Actions = x.Actions.Select(ToDto).ToList()}).ToArray(),
            };
        }

        public static RefreshTokenDto ToDto(this RefreshToken token)
        {
            return new RefreshTokenDto()
            {
                Token = token.Token,
                Created = token.Created,
                Expires = token.Expires,
                IpAddress = token.IpAddress
            };
        }

        public static RoleDto ToDto(this Role role)
        {
            return new RoleDto
            {
                Actions = role.Actions.Select(ToDto).ToList(),
                Name = role.Name
            };
        }

        public static ActionDto ToDto(this Action action)
        {
            return new ActionDto
            {
                Id = action.Id,
                Name = action.Value
            };
        }
    }
}
