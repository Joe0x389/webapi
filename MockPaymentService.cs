using webapi.DTOs;

namespace webapi;

public class MockPaymentService : IPaymentService
{
    public async Task<PaymentResponse> ProcessPaymentAsync(decimal amount)
    {
        // Simulate payment processing delay
        await Task.Delay(2000);

        if (amount > 1000)
        {
            return new PaymentResponse(false, string.Empty, "Transaction limit exceeded.");
        }

        return new PaymentResponse(true, $"Sim-TXN-{Guid.NewGuid().ToString()[..8]}", $"Payment of {amount:C} processed successfully.");
    }
}