namespace webapi.Models;

public class Dish
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public int AvailableQty { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<Category> Categories { get; set; } = [];
}