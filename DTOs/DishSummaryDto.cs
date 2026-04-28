namespace webapi.DTOs;

public record DishSummaryDto
(
    int Id,
    string Name,
    decimal Price,
    string ImageUrl
);