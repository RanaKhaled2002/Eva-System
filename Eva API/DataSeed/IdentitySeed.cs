using Microsoft.AspNetCore.Identity;

namespace Eva_API.DataSeed
{
    public class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();

            var adminEmail = "ranakhaeled@gmail.com";
            var adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
            }
        }
    }
}
