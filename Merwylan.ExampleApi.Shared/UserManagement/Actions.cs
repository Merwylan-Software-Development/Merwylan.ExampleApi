using System;
using System.Collections.Generic;
using System.Text;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    public enum Actions
    {
        RevokeTokens=1,
        ViewTokens,
        ViewUsers,
        AddUsers,
        EditUsers,
        DeleteUsers,
        AuditSearch,
        GetAllClaims
    }
}
