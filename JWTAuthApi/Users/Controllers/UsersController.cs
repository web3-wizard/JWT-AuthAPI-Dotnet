using System.Security.Claims;
using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Models;
using JWTAuthApi.Users.Models.DTOs;
using JWTAuthApi.Users.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthApi.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRoles.User))]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<ActionResult<ServiceResult<List<UserDTO>>>> GetAll(CancellationToken cancellationToken = default)
    {
       var result = await userService.GetAllUser(cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet]
    [Route("details")]
    public async Task<ActionResult<ServiceResult<UserDTO>>> GetDetails(CancellationToken cancellationToken = default)
    {
        var claimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(claimId) 
            || Guid.TryParse(claimId, out var userId) == false)
        {
            return Unauthorized();
        }
        
       var result = await userService.GetUser(userId, cancellationToken);

        return StatusCode((int)result.StatusCode, result);
    }
}