using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public ActionResult<List<Dish>> GetWithCategory(int? categoryId)
    {
        var dishesContext = _context.Dishes;
        if (categoryId is null)
            return Ok(dishesContext.ToList());
        if (_context.Categories.Find(categoryId ?? 0) is null)
            return NotFound(new ApiResponse($"Category with ID {categoryId} does not exist!"));

        var dishes = dishesContext
            .Where(d => d.CategoryId == categoryId)
            .ToList();
        return Ok(dishes);
    }

    [HttpGet("{id}")]
    public ActionResult<Dish> GetDishById(int id)
    {
        var dish = _context.Dishes.Find(id);

        // 3. التأكد إن الطبق موجود
        if (dish is null)
        {
            // لو مش موجود رجع 404 (Not Found)
            return NotFound(new ApiResponse("Dish does not exist"));
        }

        // 4. لو موجود رجع بيانات الطبق
        return Ok(dish);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse> Post([FromBody] DishCreateDto dto)
    {
        if (dto.AvailableQty < 0) // بنشيك لو عدد الأطباق المتاحة أقل من صفر
            return BadRequest(new ApiResponse("Available bowls cannot be negative!"));

        if (_context.Dishes.Any(d => d.Name == dto.Name)) // بنشيك لو في طبق بنفس الاسم موجود
            return BadRequest(new ApiResponse("Dish with the same name already exists!"));

        var catId = dto.CategoryId;
        var category = _context.Categories.Find(catId);

        if (category is null)
            return BadRequest(new ApiResponse($"Category with ID {catId} dors not exist!"));

        var dish = new Dish
        {
            Name = dto.Name,
            Price = dto.Price,
            AvailableQty = dto.AvailableQty,
            CategoryId = catId
        };

        _context.Dishes.Add(dish); // بيضيف للجدول
        _context.SaveChanges();      // سيف التعديلات في ملف الـ .db
        return Ok(new ApiResponse("Dish saved to DB!"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse> Update(int id, [FromBody] DishUpdateDto dto)
    {
        var dish = _context.Dishes.Find(id);
        if (dish is null)
            return NotFound(new ApiResponse("Dish does not exist!"));

        var qty = dto.AvailableQty;
        if (qty is not null and < 0)
            return BadRequest(new ApiResponse("Available quantity cannot be negative!"));

        var price = dto.Price;
        if (price is not null and <= 0)
            return BadRequest(new ApiResponse("Price must be positive!"));

        var name = dto.Name;
        if (name != null && _context.Dishes.Any(d => d.Name == name && d.Id != id))
            return BadRequest(new ApiResponse("Dish with the same name already exists!"));

        var catId = dto.CategoryId;
        var category = _context.Categories.Find(catId);
        if (catId != null && category == null)
            return BadRequest(new ApiResponse($"Category ID {catId} does not exist!"));

        if (name is not null) dish.Name = name;
        if (qty is not null) dish.AvailableQty = qty.Value;
        if (price is not null) dish.Price = price.Value;
        if (catId is not null) dish.Category = category;

        _context.SaveChanges();
        return Ok(new ApiResponse("Dish updated successfully!"));
    }
}