using System.Text.Json.Serialization;

namespace webapi.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool Active { get; set; } = true;
    [JsonIgnore] public List<Dish> Dishes { get; set; } = [];
}