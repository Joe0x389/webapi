using System.Text.Json.Serialization;
namespace webapi.Models;

public class OrderItem
{
    public int Id { get; set; }
    [JsonIgnore] public int OrderId { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    [JsonIgnore] public Order? Order { get; set; }
}