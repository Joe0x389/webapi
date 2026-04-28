using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetDashboardSummary() 
    {
        return Ok(new { TotalRevenue = 10243.00, MostOrdered = "Spicy Noodles" });
    }
}