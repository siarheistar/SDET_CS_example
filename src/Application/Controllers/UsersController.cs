using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SDET.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> _users = new();
    private static int _nextId = 1;

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Validate username length (minimum 3 characters)
        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
        {
            return BadRequest(new { error = "Username must be at least 3 characters long" });
        }

        // Validate email format
        if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
        {
            return BadRequest(new { error = "Invalid email format" });
        }

        // Validate password (minimum 8 characters)
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return BadRequest(new { error = "Password must be at least 8 characters long" });
        }

        // Check if user already exists
        if (_users.Any(u => u.Username == request.Username))
        {
            return Conflict(new { error = "Username already exists" });
        }

        if (_users.Any(u => u.Email == request.Email))
        {
            return Conflict(new { error = "Email already exists" });
        }

        // Create new user
        var user = new User
        {
            Id = _nextId++,
            Username = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _users.Add(user);

        return StatusCode(201, new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            createdAt = user.CreatedAt,
            message = "User registered successfully"
        });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Username and password are required" });
        }

        // Find user
        var user = _users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }

        // In a real app, you'd verify the password hash here
        // For testing purposes, we just return success
        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            token = "mock-jwt-token",
            user = new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email
            }
        });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_users.Select(u => new
        {
            id = u.Id,
            username = u.Username,
            email = u.Email,
            createdAt = u.CreatedAt
        }));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound(new { error = "User not found" });
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            createdAt = user.CreatedAt
        });
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
