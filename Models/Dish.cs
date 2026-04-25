namespace webapi.Models;

public class Dish
{
    public int Id { get; init; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableBowls { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
}