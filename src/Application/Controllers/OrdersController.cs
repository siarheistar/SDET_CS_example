using Microsoft.AspNetCore.Mvc;

namespace SDET.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private static readonly List<Order> _orders = new();
    private static int _nextOrderId = 1;

    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        // Validate product ID
        if (request.Product_Id <= 0)
        {
            return BadRequest(new { error = "Invalid product ID" });
        }

        // Validate quantity
        if (request.Quantity <= 0)
        {
            return BadRequest(new { error = "Quantity must be greater than zero" });
        }

        // Use user_id from request or default to 1 for testing
        int userId = request.User_Id > 0 ? request.User_Id : 1;

        var order = new Order
        {
            Id = _nextOrderId++,
            UserId = userId,
            ProductId = request.Product_Id,
            Quantity = request.Quantity,
            TotalPrice = request.Quantity * 100, // Mock price calculation
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _orders.Add(order);

        return StatusCode(201, new
        {
            order_id = order.Id,
            user_id = order.UserId,
            product_id = order.ProductId,
            quantity = order.Quantity,
            total_price = order.TotalPrice,
            status = order.Status,
            created_at = order.CreatedAt
        });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_orders);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound(new { error = $"Order with ID {id} not found" });
        }

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetByUserId(int userId)
    {
        var userOrders = _orders.Where(o => o.UserId == userId).ToList();
        return Ok(userOrders);
    }

    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound(new { error = $"Order with ID {id} not found" });
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { error = "Status is required" });
        }

        order.Status = request.Status;
        return Ok(order);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound(new { error = $"Order with ID {id} not found" });
        }

        _orders.Remove(order);
        return NoContent();
    }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderRequest
{
    public int User_Id { get; set; }
    public int Product_Id { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
