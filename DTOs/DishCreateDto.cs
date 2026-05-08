namespace webapi.DTOs;

public class DishCreateDto
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required int CategoryId { get; set; }
}