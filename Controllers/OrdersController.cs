using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
            return NotFound(new ApiResponse("Order does not exist"));
        return Ok(order);
    }

    [HttpPost("checkout")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> Checkout([FromBody] OrderDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest(new ApiResponse("Order must contain at least one item."));

        var status = dto.Status;
        if (status is not null && !Order.IsValidStatus(status))
            return BadRequest(new ApiResponse("Invalid order status!"));

        var dishQuantities = new List<(Dish, int)>();
        foreach (var item in dto.Items)
        {
            var dish = _context.Dishes.Find(item.DishId);
            if (dish == null || !dish.Active)
                return BadRequest(new ApiResponse($"Dish with ID {item.DishId} does not exist or is inactive!"));
            if (item.Quantity <= 0)
                return BadRequest(new ApiResponse("Quantity must be positive!"));
            dishQuantities.Add((dish, item.Quantity));
        }

        var total = 0m;

        foreach (var (dish, quantity) in dishQuantities)
            total += dish.Price * quantity;

        var order = new Order
        {
            CustomerName = dto.CustomerName,
            Status = status ?? "Pending",
            TotalPayment = total,
            Items = dto.Items,
            UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
        };


        _context.Orders.Add(order);
        _context.SaveChanges();
        return Ok(new { OrderId = order.Id, Status = "Order saved permanently" });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Chef")]
    public ActionResult<ApiResponse> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateDto dto)
    {
        var order = _context.Orders.Find(id);
        if (order is null)
            return NotFound(new ApiResponse("Order does not exist!"));
        if (!Order.IsValidStatus(dto.NewStatus))
            return BadRequest(new ApiResponse("Invalid order status!"));
        order.Status = dto.NewStatus;
        _context.Orders.Update(order);
        _context.SaveChanges();
        return Ok(new ApiResponse("Order status updated successfully!"));
    }

    [HttpDelete("{id}")]
    public ActionResult<ApiResponse> DeleteOrder(int id)
    {
        var order = _context.Orders.Find(id);
        if (order is null)
            return NotFound(new ApiResponse("Order does not exist!"));
        if (order.Status != "Pending")
            return BadRequest(new ApiResponse("Only pending orders can be deleted!"));
        _context.Orders.Remove(order);
        _context.SaveChanges();
        return Ok(new ApiResponse("Order deleted successfully!"));
    }
}