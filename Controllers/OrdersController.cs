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
    private readonly IPaymentService _paymentService;

    public OrdersController(AppDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
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
            if (dish is null)
                return BadRequest(new ApiResponse($"Dish with ID {item.DishId} does not exist!"));
            if (dish.AvailableQty < item.Quantity)
                return BadRequest(new ApiResponse($"Not enough bowls available for \"{dish.Name}\"!"));
            dishQuantities.Add((dish, item.Quantity));
        }

        var total = 0m;

        foreach (var (dish, quantity) in dishQuantities)
            total += dish.Price * quantity;

        var result = await _paymentService.ProcessPaymentAsync(total);

        if (!result.IsSuccess)
            return BadRequest(new ApiResponse(result.Message));

        foreach (var (dish, quantity) in dishQuantities)
        {
            dish.AvailableQty -= quantity;
            _context.Dishes.Update(dish);
        }

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

    [HttpPost("{id}/status")]
    public ActionResult<ApiResponse> UpdateOrderStatus(int id, string newStatus)
    {
        var order = _context.Orders.Find(id);
        if (order is null)
            return NotFound(new ApiResponse("Order does not exist!"));
        if (!Order.IsValidStatus(newStatus))
            return BadRequest(new ApiResponse("Invalid order status!"));
        order.Status = newStatus;
        _context.Orders.Update(order);
        _context.SaveChanges();
        return Ok(new ApiResponse("Order status updated successfully!"));
    }
}