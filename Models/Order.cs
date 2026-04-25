namespace webapi.Models;

public class Order
{
    public int Id { get; set; }
    public required List<OrderItem> Items { get; set; }
    public required string CustomerName { get; set; }
    public string Status { get; set; }
    public decimal TotalPayment { get; set; } = 0m;
    public DateTime Date { get; set; } = DateTime.Now;
}