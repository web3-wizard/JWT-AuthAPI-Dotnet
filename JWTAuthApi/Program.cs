using JWTAuthApi.Users.Services;
using JWTAuthApi.Users.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IHashingService, HashingService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();