namespace webapi.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; set; }
    public decimal TotalPayment { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}