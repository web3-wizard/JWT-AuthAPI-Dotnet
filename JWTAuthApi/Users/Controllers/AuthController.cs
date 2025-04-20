using System.Net;
using System.Security.Claims;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthApi.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<ServiceResult>> Register([FromBody] RegisterRequest request)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.Register(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.Login(request);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
    [Route("confirmed/email")]
    [Authorize(Roles = nameof(UserRoles.Guest))]
    public async Task<ActionResult<ServiceResult>> VerifyEmail()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        if (Guid.TryParse(id, out var userId) == false)
        {
            return BadRequest();
        }

        var result = await authService.ConfirmedEmail(new VerifyEmailRequest(userId, email));
        
        return StatusCode((int)result.StatusCode, result);
    }
    
    [HttpGet]
    [Route("verify")]
    [Authorize]
    public ActionResult<ServiceResult<VerifyResponse>> Verify()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var username = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        if (Guid.TryParse(id, out var userId) == false)
        {
            return BadRequest();
        }

        var verifyResponse = new VerifyResponse(
            Id: userId, 
            Name: name, 
            Email: email, 
            Username: username, 
            Roles: roles);
        
        return Ok(new ServiceResult<VerifyResponse>(
            HttpStatusCode.OK,
            "User verified",
            verifyResponse));
    }
}
