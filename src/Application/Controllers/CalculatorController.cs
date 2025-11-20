using Microsoft.AspNetCore.Mvc;

namespace SDET.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    [HttpPost]
    public IActionResult Calculate([FromBody] CalculationRequest request)
    {
        // Validate operator
        if (string.IsNullOrWhiteSpace(request.Operation))
        {
            return BadRequest(new { error = "Operation is required" });
        }

        double result;

        try
        {
            result = request.Operation.ToLower() switch
            {
                "add" or "+" => request.Num1 + request.Num2,
                "subtract" or "-" => request.Num1 - request.Num2,
                "multiply" or "*" => request.Num1 * request.Num2,
                "divide" or "/" => PerformDivision(request.Num1, request.Num2),
                _ => throw new InvalidOperationException($"Unsupported operation: {request.Operation}")
            };
        }
        catch (DivideByZeroException)
        {
            return BadRequest(new { error = "Cannot divide by zero" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        return Ok(new
        {
            operation = request.Operation,
            num1 = request.Num1,
            num2 = request.Num2,
            result = result
        });
    }

    private static double PerformDivision(double a, double b)
    {
        if (Math.Abs(b) < double.Epsilon)
        {
            throw new DivideByZeroException();
        }

        return a / b;
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "calculator" });
    }
}

public class CalculationRequest
{
    public double Num1 { get; set; }
    public double Num2 { get; set; }
    public string Operation { get; set; } = string.Empty;
}
