namespace webapi.DTOs;

public record DishUpdateDto
(
    string? Name,
    decimal? Price,
    int? CategoryId,
    bool? Active
);