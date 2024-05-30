namespace ArchiBase.Utils;
using ArchiBase.Users;
using Microsoft.AspNetCore.Identity;

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ArchiBaseRole>>();
        if (roleManager != null)
        {
            ArchiBaseRole? adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                var results = await roleManager.CreateAsync(new ArchiBaseRole("Admin"));
            }
            ArchiBaseRole? editorRole = await roleManager.FindByNameAsync("Editor");
            if (editorRole == null)
            {
                var results = await roleManager.CreateAsync(new ArchiBaseRole("Editor"));
            }
        }
        var userManager = serviceProvider.GetRequiredService<UserManager<ArchiBaseUser>>();
        if (userManager != null)
        {
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new ArchiBaseUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    DateOfBirth = new DateTime(2001, 1, 1),
                    Bio = "Temp admin. <b>REMOVE ON PRODUCTION!!!</b>",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                await userManager.CreateAsync(admin, "1Password!");
            }
            await userManager.AddToRolesAsync(admin, ["Admin", "Editor"]);
        }
    }
}
