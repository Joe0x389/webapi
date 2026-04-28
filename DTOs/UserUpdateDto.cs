using Microsoft.AspNetCore.Identity;
using webapi.Models;

namespace webapi.DTOs;

public class UserUpdateDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }

    public void ApplyTo(User user)
    {
        if (!string.IsNullOrWhiteSpace(Username))
            user.Username = Username;
        if (!string.IsNullOrWhiteSpace(Password))
        {
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, Password);
        }
        if (!string.IsNullOrWhiteSpace(Role) && Role is "Admin" or "Cashier")
            user.Role = Role;
    }
}