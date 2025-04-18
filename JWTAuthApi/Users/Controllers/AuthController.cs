using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthApi.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IHashingService hashingService,
    ITokenService tokenService,
    ILogger<AuthController> logger) : ControllerBase
{
    private static readonly List<User> _users = [];

    [HttpPost]
    [Route("register")]
    public ActionResult Register([FromBody] RegisterRequest request)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.Password,
        };

        var hashPassword = hashingService.HashPassword(user, request.Password);
        user.PasswordHash = hashPassword;

        _users.Add(user);

        logger.LogInformation("User created successfully.");

        return Created();
    }

    [HttpPost]
    [Route("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        var user = _users.FirstOrDefault(x => x.Username == request.Username);

        if (user is null)
        {
            logger.LogWarning("User not found.");
            return BadRequest("Invalid Credentials!");
        }

        var isValidPassword = hashingService.IsValidPassword(user, request.Password);

        if (isValidPassword == false)
        {
            logger.LogWarning("Invalid Password.");
            return BadRequest("Invalid Credentials!");
        }

        var token = tokenService.GenerateToken(user);
        logger.LogInformation("Token generated successfully.");

        return Ok(new { Token = token });
    }
}
