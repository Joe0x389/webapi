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
}