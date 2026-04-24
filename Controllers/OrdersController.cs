using Microsoft.AspNetCore.Mvc;
using webapi.Data;
using webapi.Models;

[ApiController] [Route("api/[controller]")]
public class OrdersController : ControllerBase 
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // اللينك هيكون: api/orders
    [HttpGet]
    public IActionResult GetAllOrders()
    {
        // بيجيب كل الطلبات من الداتابيز ويحولها لـ List
        var orders = _context.Orders.ToList();
        return Ok(orders);
    }

// لو عايز تجيب طلب واحد بالـ ID
// اللينك هيكون: api/orders/5
    [HttpGet("{id}")]
    public IActionResult GetOrderById(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return NotFound(new { message = "Order does not exist" });
        return Ok(order);
    }
    
    [HttpPost("checkout")]
    public IActionResult Checkout([FromBody] Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges(); 
        return Ok(new { OrderId = order.Id, Status = "Order Saved Permanently" });
    }
}