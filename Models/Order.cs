namespace webapi.Models;

public class Order
{
    public int Id { get; set; }
    public required List<OrderItem> Items { get; set; }
    public required string CustomerName { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal TotalPayment { get; set; } = 0m;
    public DateTime Date { get; set; } = DateTime.Now;
    public int UserId { get; set; }

    public static bool IsValidStatus(string status)
        => status is "Pending" or "In Progress" or "Completed" or "Delivered";
}