using webapi.Models;

namespace webapi.DTOs;

public record DishUpdateDto
(
    string? Name,
    decimal? Price,
    int? AvailableQty,
    string? ImageUrl,
    int? CategoryId
);