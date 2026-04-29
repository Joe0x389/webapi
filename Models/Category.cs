using System.Text.Json.Serialization;

namespace webapi.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [JsonIgnore] public List<Dish> Dishes { get; set; } = [];
}