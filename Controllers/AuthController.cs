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
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserDto loginInfo)
    {
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
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]),
            Expires = DateTime.UtcNow.AddDays(1), // Token valid for 1 day
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
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
        var user = _context.Users.Find(id);
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
        if (_context.Users.Any(u => u.Username == updateInfo.Username && u.Id != id))
            return BadRequest(new { Message = "Username already exists!" });
        if (!string.IsNullOrWhiteSpace(updateInfo.Username))
            user.Username = updateInfo.Username;
        if (!string.IsNullOrWhiteSpace(updateInfo.Password))
        {
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, updateInfo.Password);
        }
        if (!string.IsNullOrWhiteSpace(updateInfo.Role)
            && updateInfo.Role is "Admin" or "Cashier")
            user.Role = updateInfo.Role;
        _context.Users.Update(user);
        _context.SaveChanges();
        return Ok(new { Message = "User updated successfully!" });
    }
}