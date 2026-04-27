namespace webapi.DTOs;

public class CreateDishDto
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQty { get; set; }
    public int[] CategoryIds { get; set; } = null!;
}