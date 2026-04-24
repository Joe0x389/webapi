using Microsoft.AspNetCore.Mvc;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
[ApiController] [Route("api/[controller]")]
public class DishesController : ControllerBase 
{
    private readonly AppDbContext _context; // بنعرف المتغير هنا

    public DishesController(AppDbContext context) // الـ Constructor اللي بيسحب الداتابيز
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_context.Dishes.ToList()); // بيسحب من الجدول بجد
    
    // 1. تحديد نوع الـ Request والـ Path
// الـ {id} دي معناها إننا مستنيين رقم في آخر اللينك
    [HttpGet("{id}")] 
    public IActionResult GetDishById(int id)
    {
        // 2. البحث في الداتابيز عن طريق الـ Id
        // بنقول للـ Entity Framework: هات أول طبق يقابلك الـ Id بتاعه بيساوي الرقم اللي مبعوث
        var dish = _context.Dishes.FirstOrDefault(d => d.Id == id);

        // 3. التأكد إن الطبق موجود
        if (dish == null)
        {
            // لو مش موجود رجع 404 (Not Found)
            return NotFound(new { message = "للأسف الطبق ده مش موجود" });
        }

        // 4. لو موجود رجع بيانات الطبق
        return Ok(dish);
    }
    
    [HttpPost]
    public IActionResult Post([FromBody] Dish newDish) 
    {
        _context.Dishes.Add(newDish); // بيضيف للجدول
        _context.SaveChanges();      // سيف التعديلات في ملف الـ .db
        return Ok(new { Message = "Dish saved to DB!" });
    }
}