using JWTAuthApi.DB;
using JWTAuthApi.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthApi.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRoles.User))]
public class UsersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .OrderByDescending(u => u.UpdatedAt)
            .ToListAsync(cancellationToken: cancellationToken);
        
        return Ok(users);
    }
}