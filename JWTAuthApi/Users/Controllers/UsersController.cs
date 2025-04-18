using JWTAuthApi.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthApi.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await dbContext.Users.ToListAsync();
        return Ok(users);
    }
}