namespace webapi.Models;

public class Order
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal TotalPayment { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}