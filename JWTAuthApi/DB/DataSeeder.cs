using JWTAuthApi.Users.Entities;
using JWTAuthApi.Users.Services.Interfaces;

namespace JWTAuthApi.DB;

public static class DataSeeder
{
    public static void SeedData(AppDbContext dbContext, IHashingService  hashingService)
    {
        if (dbContext.Users.Any())
        {
            return;
        }

        List<User> users = [];

        var adminUser = User.CreateGuestUser(
            name: "Admin User",
            username: "admin_user",
            email: "admin@gmail.com"
        );

        var adminPassword = hashingService.HashPassword(adminUser, "Admin1234");
        
        adminUser.UpdatePassword(adminPassword);
        adminUser.AddRole(nameof(UserRoles.User));
        adminUser.AddRole(nameof(UserRoles.Admin));

        var defaultUser = User.CreateGuestUser(
            name: "Default User",
            username: "default_user",
            email: "default@gmail.com"
        );
        
        var defaultPassword = hashingService.HashPassword(defaultUser, "Default1234");
        
        defaultUser.UpdatePassword(defaultPassword);
        defaultUser.AddRole(nameof(UserRoles.User));

        users.Add(adminUser);
        users.Add(defaultUser);
        
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }
}