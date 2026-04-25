namespace webapi.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}