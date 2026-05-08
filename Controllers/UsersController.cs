using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webapi.DTOs;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<ArrayHolder<User>> GetAllUsers()
    {
        var users = _context.Users;
        return Ok(ArrayHolder.Create(users));
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = _context.Users.Find(id);
        if (user is null)
            return NotFound(new ApiResponse("User does not exist!"));
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public ActionResult<ApiResponse> Delete(int id)
    {
        if (id == 1)
            return BadRequest(new ApiResponse("Cannot delete the super admin!"));
        var user = _context.Users.Find(id);
        if (user is null)
            return NotFound(new ApiResponse("User does not exist!"));
        _context.Users.Remove(user);
        _context.SaveChanges();
        return Ok(new ApiResponse("User deleted successfully!"));
    }

    [HttpPut("{id}")]
    public ActionResult<ApiResponse> Update(int id, [FromBody] UserUpdateDto updateInfo)
    {
        if (id == 1 && Models.User.GetIdFromUser(User) != 1)
            return BadRequest(new ApiResponse("Cannot update the super admin!"));
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return NotFound(new ApiResponse("User does not exist!"));
        if (_context.Users.Any(u => u.Username == updateInfo.Username && u.Id != id))
            return BadRequest(new ApiResponse("Username already exists!"));
        if (!string.IsNullOrWhiteSpace(updateInfo.Username))
            user.Username = updateInfo.Username;
        if (!string.IsNullOrWhiteSpace(updateInfo.Password))
        {
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, updateInfo.Password);
        }
        if (!string.IsNullOrWhiteSpace(updateInfo.Role)
            && Models.User.IsValidRole(updateInfo.Role))
            user.Role = updateInfo.Role;
        _context.Users.Update(user);
        _context.SaveChanges();
        return Ok(new ApiResponse("User updated successfully!"));
    }
}
