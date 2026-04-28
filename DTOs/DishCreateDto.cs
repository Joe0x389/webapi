namespace webapi.DTOs;

public class DishCreateDto
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQty { get; set; }
    public int[] CategoryIds { get; set; } = null!;
}