using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
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
    public ActionResult<List<CategorySummaryDto>> Get()
    {
        var categories = _context.Categories
            .Select(c => new CategorySummaryDto(c.Id, c.Name))
            .ToList();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public ActionResult<Category> GetById(int id)
    {
        var category = _context.Categories.Include(c => c.Dishes).FirstOrDefault(c => c.Id == id);
        if (category is null)
            return NotFound(new { Message = "Category not found!" });
        return Ok(category);
    }

    [HttpPost("{name}")]
    public IActionResult Post(string name)
    {
        if (_context.Categories.Any(c => c.Name == name))
            return BadRequest(new { Message = "Category with the same name already exists!" });
        var category = new Category { Name = name };
        _context.Categories.Add(category);
        _context.SaveChanges();
        return Ok(new { Message = "Category saved to DB!" });
    }
}
