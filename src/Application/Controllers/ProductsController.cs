using Microsoft.AspNetCore.Mvc;

namespace SDET.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Category = "Electronics", Stock = 50 },
        new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Category = "Electronics", Stock = 200 },
        new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 89.99m, Category = "Electronics", Stock = 150 },
        new Product { Id = 4, Name = "Monitor", Description = "27-inch 4K monitor", Price = 399.99m, Category = "Electronics", Stock = 75 },
        new Product { Id = 5, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 199.99m, Category = "Electronics", Stock = 100 }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_products);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { error = $"Product with ID {id} not found" });
        }

        return Ok(product);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Product name is required" });
        }

        if (request.Price <= 0)
        {
            return BadRequest(new { error = "Price must be greater than zero" });
        }

        var product = new Product
        {
            Id = _products.Max(p => p.Id) + 1,
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Price = request.Price,
            Category = request.Category ?? "Uncategorized",
            Stock = request.Stock
        };

        _products.Add(product);

        return StatusCode(201, product);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateProductRequest request)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { error = $"Product with ID {id} not found" });
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
            product.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Description))
            product.Description = request.Description;
        if (request.Price.HasValue && request.Price.Value > 0)
            product.Price = request.Price.Value;
        if (!string.IsNullOrWhiteSpace(request.Category))
            product.Category = request.Category;
        if (request.Stock.HasValue)
            product.Stock = request.Stock.Value;

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { error = $"Product with ID {id} not found" });
        }

        _products.Remove(product);
        return NoContent();
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Stock { get; set; }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
}

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public int? Stock { get; set; }
}
