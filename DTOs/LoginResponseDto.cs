using webapi.Models;

namespace webapi.DTOs;

public record LoginResponseDto(string Token, User User);