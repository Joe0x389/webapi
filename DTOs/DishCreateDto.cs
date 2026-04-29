namespace webapi.DTOs;

public class DishCreateDto
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public int AvailableQty { get; set; } = 0;
    public required int CategoryId { get; set; }
}