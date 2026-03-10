using Identity.Domain.Enums;
using System.Data;

namespace Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;

    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();
}