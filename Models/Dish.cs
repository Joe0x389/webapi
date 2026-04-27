using System.Text.Json.Serialization;

namespace webapi.Models;

public class Dish
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQty { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    [JsonIgnore] public List<Category> Categories { get; set; } = [];
}