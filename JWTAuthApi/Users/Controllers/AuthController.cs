using System.Net;
using System.Security.Claims;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.DTOs;
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
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResult>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.Register(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenDTO>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid == false)
        {
            return BadRequest(ModelState);
        }

        var result = await authService.Login(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
    [Route("confirmed/email")]
    [Authorize(Roles = nameof(UserRoles.Guest))]
    public async Task<ActionResult<ServiceResult>> ConfirmedEmail(CancellationToken cancellationToken = default)
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(id)
            || string.IsNullOrEmpty(email)
            || Guid.TryParse(id, out var userId) == false)
        {
            return Unauthorized();
        }

        var verifyEmailRequest = new VerifyEmailRequest(userId, email);

        var result = await authService.ConfirmedEmail(verifyEmailRequest, cancellationToken);

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
            string.IsNullOrEmpty(username) || Guid.TryParse(id, out var userId) == false)
        {
            return Unauthorized();
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

    [HttpPost]
    [Route("refresh-tokens")]
    [Authorize(Roles = nameof(UserRoles.User))]
    public async Task<ActionResult<TokenDTO>> RefreshTokens([FromBody] RefreshTokensRequest request, CancellationToken cancellationToken = default)
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(id) 
            || Guid.TryParse(id, out var userId) == false
            || userId.Equals(request.UserId) == false)
        {
            return Unauthorized();
        }

        var result = await authService.RefreshTokens(request, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }
}
