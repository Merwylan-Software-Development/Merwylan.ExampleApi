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
                RefreshTokens = user.RefreshTokens?.Select(ToDto).ToArray() ?? new RefreshTokenDto[0],
                Roles = user.Roles.Select(x => new RoleDto { Name = x.Name }).ToArray(),
                AuthorizedActions = user.Roles?
                                        .SelectMany(userRole => userRole.Actions)
                                        .Select(action => new ActionDto(){Name = action.Value})
                                        .ToArray()
                                    ?? new ActionDto[0]
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

        public static ActionDto ToDto(this Action action)
        {
            return new ActionDto
            {
                Name = action.Value
            };
        }
    }
}
