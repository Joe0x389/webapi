namespace webapi.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; } // في الحقيقة بنعمل لها Hashing
    public string Role { get; set; } // "Admin" أو "Cashier"
}
