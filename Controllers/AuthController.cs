using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Data; // مهم جداً عشان يشوف الـ AppDbContext
using webapi.DTOs;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // 1. تعريف المتغير
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    // 2. الـ Constructor اللي بيخلي الـ _context موجودة
    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")] // لو عايز بس الأدمين يقدر يسجل مستخدمين جدد
    public ActionResult<User> Register([FromBody] UserCreateDto registerInfo)
    {
        if (_context.Users.Any(u => u.Username == registerInfo.Username))
            return BadRequest(new { Message = "Username already exists!" });
        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Username = registerInfo.Username,
            Role = registerInfo.Role
        };
        user.PasswordHash = hasher.HashPassword(user, registerInfo.Password);
        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return NotFound(new { Message = "User does not exist!" });
        _context.Users.Remove(user);
        _context.SaveChanges();
        return Ok(new { Message = "User deleted successfully!" });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, [FromBody] UserUpdateDto updateInfo)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return NotFound(new { Message = "User does not exist!" });
        if (!string.IsNullOrEmpty(updateInfo.Username))
            user.Username = updateInfo.Username;
        if (!string.IsNullOrEmpty(updateInfo.Password))
        {
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, updateInfo.Password);
        }
        if (!string.IsNullOrEmpty(updateInfo.Role))
            user.Role = updateInfo.Role;
        _context.Users.Update(user);
        _context.SaveChanges();
        return Ok(new { Message = "User updated successfully!" });
    }
}