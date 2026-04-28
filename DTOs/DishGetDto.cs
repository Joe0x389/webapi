using webapi.Models;

namespace webapi.DTOs;

public class DishGetDto
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQty { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<CategorySummaryDto> Categories { get; set; } = [];

    public static DishGetDto FromDish(Dish dish) => new()
    {
        Id = dish.Id,
        Name = dish.Name,
        Price = dish.Price,
        AvailableQty = dish.AvailableQty,
        ImageUrl = dish.ImageUrl,
        Categories = [..dish.Categories
            .Select(c => new CategorySummaryDto(c.Id, c.Name))
            ]
    };
}
