using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webapi.Data; // مهم جداً عشان يشوف الـ AppDbContext
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // 1. تعريف المتغير
    private readonly AppDbContext _context;

    // 2. الـ Constructor اللي بيخلي الـ _context موجودة
    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public ActionResult<User> Login([FromBody] UserDto loginInfo)
    {
        // دلوقتي الـ _context بقت موجودة وتقدر تستخدمها هنا
        var user = _context.Users.FirstOrDefault(u => u.Username == loginInfo.Username);
        if (user is null)
            return Unauthorized();

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginInfo.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized();

        return Ok(user);
    }

    [HttpPost("register")]
    [Authorize("Admin")] // لو عايز بس الأدمين يقدر يسجل مستخدمين جدد
    public ActionResult<User> Register([FromBody] UserDto registerInfo)
    {
        if (_context.Users.Any(u => u.Username == registerInfo.Username))
            return BadRequest(new { Message = "Username already exists!" });
        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Username = registerInfo.Username
        };
        user.PasswordHash = hasher.HashPassword(user, registerInfo.Password);
        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize("Admin")]
    public IActionResult Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return NotFound(new { Message = "User does not exist!" });
        _context.Users.Remove(user);
        _context.SaveChanges();
        return Ok(new { Message = "User deleted successfully!" });
    }
}