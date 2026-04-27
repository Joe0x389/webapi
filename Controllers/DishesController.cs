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
    public ActionResult<List<GetDishDto>> Get() => Ok(_context.Dishes
        .Include(d => d.Categories)
        .Select(d => GetDishDto.FromDish(d))
        .ToList());

    [HttpGet("{id}")]
    public ActionResult<GetDishDto> GetDishById(int id)
    {
        var dish = _context.Dishes.Find(id);

        // 3. التأكد إن الطبق موجود
        if (dish is null)
        {
            // لو مش موجود رجع 404 (Not Found)
            return NotFound(new { message = "Dish does not exist" });
        }

        // 4. لو موجود رجع بيانات الطبق
        return Ok(GetDishDto.FromDish(dish));
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateDishDto dto)
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
            AvailableQty = dto.AvailableQty
        };

        foreach (var id in dto.CategoryIds)
        {
            var category = _context.Categories.Find(id)!;
            dish.Categories.Add(category); // بنضيف الكاتيجوري للطبق
        }

        _context.Dishes.Add(dish); // بيضيف للجدول
        _context.SaveChanges();      // سيف التعديلات في ملف الـ .db
        return Ok(new { Message = "Dish saved to DB!" });
    }
}