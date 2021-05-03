using System.Collections.Generic;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    public class RoleDto
    {
        public string Name { get; set; } = string.Empty;
        public IList<ActionDto> Actions { get; set; } = new List<ActionDto>();
    }
}