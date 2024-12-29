using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace RoleBasedAPI.Models
{
    public class UserRole
    {
        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
