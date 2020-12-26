using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Utility;
using Microsoft.AspNetCore.Identity;

namespace AMZEnterpriseWebsite.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedSettings(context);
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var adminCount = userManager.GetUsersInRoleAsync(SD.AdminEndUser).Result.Count;

            if (adminCount <= 0)
            {
                if (userManager.FindByNameAsync
                        ("admin123").Result == null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.UserName = "admin123";
                    user.Email = "example@gmail.com";
                    user.FirstName = "FristName";
                    user.LastName = "LastName";
                    user.DateTime = DateTime.Now;
                    user.EmailConfirmed = true;
                    IdentityResult result = userManager.CreateAsync
                        (user, "p@Ss123").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, SD.AdminEndUser).Wait();
                    }
                }
            }

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(SD.AdminEndUser).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = SD.AdminEndUser;
                IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync(SD.WriterEndUser).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = SD.WriterEndUser;
                IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;
            }
        }

        public static void SeedSettings(ApplicationDbContext context)
        {
            if (!context.Settings.Any())
            {
                context.Settings.Add(new Setting()
                {
                    SiteName = "SiteName"
                });
                context.SaveChanges();
            }
        }
    }
}
