using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DishesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DishesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<DishGetDto>> GetWithCategory(int? categoryId)
    {
        var dishes = _context.Dishes
            .Include(d => d.Categories)
            .Where(d => !categoryId.HasValue || d.Categories.Any(c => c.Id == categoryId))
            .Select(d => DishGetDto.FromDish(d))
            .ToList();
        return Ok(dishes);
    }

    [HttpGet("{id}")]
    public ActionResult<DishGetDto> GetDishById(int id)
    {
        var dish = _context.Dishes.Find(id);

        // 3. التأكد إن الطبق موجود
        if (dish is null)
        {
            // لو مش موجود رجع 404 (Not Found)
            return NotFound(new { message = "Dish does not exist" });
        }

        // 4. لو موجود رجع بيانات الطبق
        return Ok(DishGetDto.FromDish(dish));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Post([FromBody] DishCreateDto dto)
    {
        if (dto.AvailableQty < 0) // بنشيك لو عدد الأطباق المتاحة أقل من صفر
            return BadRequest(new { Message = "Available bowls cannot be negative!" });

        if (_context.Dishes.Any(d => d.Name == dto.Name)) // بنشيك لو في طبق بنفس الاسم موجود
            return BadRequest(new { Message = "Dish with the same name already exists!" });

        foreach (var id in dto.CategoryIds) // بنشيك لو في أي كاتيجوري غير موجودة
        {
            if (!_context.Categories.Any(c => c.Id == id))
                return BadRequest(new { Message = $"Category with ID {id} does not exist!" });
        }

        var dish = new Dish
        {
            Name = dto.Name,
            Price = dto.Price,
            AvailableQty = dto.AvailableQty,
            Categories = [.. dto.CategoryIds.Select(id => _context.Categories.Find(id)!)]
        };

        _context.Dishes.Add(dish); // بيضيف للجدول
        _context.SaveChanges();      // سيف التعديلات في ملف الـ .db
        return Ok(new { Message = "Dish saved to DB!" });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, [FromBody] DishUpdateDto dto)
    {
        var dish = _context.Dishes.Find(id);
        if (dish is null)
            return NotFound(new { Message = "Dish does not exist!" });
        if (dto.AvailableQty < 0)
            return BadRequest(new { Message = "Available quantity cannot be negative!" });
        if (dto.Price < 0)
            return BadRequest(new { Message = "Price cannot be negative!" });
        if (_context.Dishes.Any(d => d.Name == dto.Name && d.Id != id))
            return BadRequest(new { Message = "Dish with the same name already exists!" });

        Helpers.Patch(dto, dish);
        _context.SaveChanges();
        return Ok(new { Message = "Dish updated successfully!" });
    }
}