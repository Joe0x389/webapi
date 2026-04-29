using webapi.DTOs;

namespace webapi;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(decimal amount);
}