using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<Category>> GetAllCategories()
    {
        var categories = _context.Categories.ToList();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public ActionResult<Category> GetById(int id)
    {
        var category = _context.Categories.Find(id);
        if (category is null)
            return NotFound(new ApiResponse("Category not found!"));
        return Ok(category);
    }

    [HttpPost("{name}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse> Post(string name)
    {
        if (_context.Categories.Any(c => c.Name == name))
            return BadRequest(new ApiResponse("Category with the same name already exists!"));
        var category = new Category { Name = name };
        _context.Categories.Add(category);
        _context.SaveChanges();
        return Ok(new ApiResponse("Category saved to DB!"));
    }
}
