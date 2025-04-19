using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.Requests;
using JWTAuthApi.Users.Models.Responses;
using JWTAuthApi.Users.Services.Interfaces;
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
}
