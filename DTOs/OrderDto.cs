using webapi.Models;
namespace webapi.DTOs;

public class OrderDto
{
    public required string CustomerName { get; set; }
    public required List<OrderItem> Items { get; set; }
    public string? Status { get; set; }
}