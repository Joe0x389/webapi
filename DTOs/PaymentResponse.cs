namespace webapi.DTOs;

public record PaymentResponse(bool IsSuccess, string TransactionId, string Message);