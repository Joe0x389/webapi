using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webapi.Data;
using webapi.DTOs;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase 
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // اللينك هيكون: api/orders
    [HttpGet]
    public ActionResult<List<Order>> GetAllOrders()
    {
        // بيجيب كل الطلبات من الداتابيز ويحولها لـ List
        var orders = _context.Orders
            .Include(o => o.Items)
            .ToList();
        return Ok(orders);
    }

// لو عايز تجيب طلب واحد بالـ ID
// اللينك هيكون: api/orders/5
    [HttpGet("{id}")]
    public ActionResult<Order> GetOrderById(int id)
    {
        var order = _context.Orders
            .Include(o => o.Items) // عشان يجيب تفاصيل الأيتمز مع الطلب
            .FirstOrDefault(o => o.Id == id);
        if (order is null)
            return NotFound(new { message = "Order does not exist" });
        return Ok(order);
    }
    
    [HttpPost("checkout")]
    [Authorize]
    public IActionResult Checkout([FromBody] OrderDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest(new { message = "Order must contain at least one item." });
        foreach (var item in dto.Items)
        {
            var dish = _context.Dishes.Find(item.DishId);
            if (dish is null)
                return BadRequest(new { message = $"Dish with ID {item.DishId} does not exist!" });
            if (dish.AvailableQty < item.Quantity)
                return BadRequest(new { message = $"Not enough bowls available for \"{dish.Name}\"!" });
        }

        var order = new Order
        {
            CustomerName = dto.CustomerName,
            Status = dto.Status ?? "Pending",
            TotalPayment = 0m,
            Items = dto.Items,
            UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
        };

        foreach (var item in order.Items)
        {
            var dish = _context.Dishes.First(d => d.Id == item.DishId);
            dish.AvailableQty -= item.Quantity;
            order.TotalPayment += dish.Price * item.Quantity;
        }

        _context.Orders.Add(order);
        _context.SaveChanges();
        return Ok(new { OrderId = order.Id, Status = "Order saved permanently" });
    }
}