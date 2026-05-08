using System.Text.Json.Serialization;

namespace webapi.Models;

public class Dish
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required int CategoryId { get; set; }
    public bool Active { get; set; } = true;
    [JsonIgnore] public Category? Category;
}