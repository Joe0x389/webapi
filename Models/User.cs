namespace webapi.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "Cashier"; // "Admin" أو "Cashier"

    public static bool IsValidRole(string role) => role is "Admin" or "Cashier";
}