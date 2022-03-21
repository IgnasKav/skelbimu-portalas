using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<User>
                {
                    new User {DisplayName = "Rokas", UserName = "rok45", Email = "rok45@local.com"},
                    new User {DisplayName = "Ignas", UserName = "IgnasKav", Email = "ignas.kavaliauskas5@gmail.com"},
                    new User {DisplayName = "Matas", UserName = "Mat42Bo55", Email = "mat42boss@gmail.com"},
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }
            }

            if (!context.Categories.Any())
            {
                var category = new Category
                {
                    Name = "default",
                };
                
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }

            if (context.Advertisements.Any()) return;

            var categoryList = await context.Categories.ToListAsync();
            var defaultCategory = categoryList[0];

            var advertisements = new List<Advertisement>
            {
                new Advertisement
                {
                    Title = "Skelbimas 1",
                    Date = DateTime.Now.AddMonths(-2),
                    Description = "naujas telefonas",
                    CategoryId = defaultCategory.Id,
                    State = AdvertisementState.Approved,
                    City = "Kaunas",
                    Views = 0,
                    Price = 20
                },
                new Advertisement
                {
                    Title = "Skelbimas 2",
                    Date = DateTime.Now.AddMonths(-1),
                    Description = "naudotas arbatinukas",
                    CategoryId = defaultCategory.Id,
                    State = AdvertisementState.Approved,
                    City = "Vilnius",
                    Views = 2,
                    Price = 30
                },
                new Advertisement
                {
                    Title = "Skelbimas 3",
                    Date = DateTime.Now.AddMonths(1),
                    Description = "naudotas siurblys",
                    CategoryId = defaultCategory.Id,
                    State = AdvertisementState.Approved,
                    City = "Vilnius",
                    Views = 8,
                    Price = 15
                },
                new Advertisement
                {
                    Title = "Skelbimas 4",
                    Date = DateTime.Now.AddMonths(2),
                    Description = "zaidimu kompiuteris",
                    CategoryId = defaultCategory.Id,
                    State = AdvertisementState.Approved,
                    City = "Utena",
                    Views = 4,
                    Price = 500
                },
                new Advertisement
                {
                    Title = "Skelbimas 5",
                    Date = DateTime.Now.AddMonths(3),
                    Description = "Klaviatura",
                    CategoryId = defaultCategory.Id,
                    State = AdvertisementState.Approved,
                    City = "Kaunas",
                    Views = 7,
                    Price = 50
                }
            };

            await context.Advertisements.AddRangeAsync(advertisements);
            await context.SaveChangesAsync();
        }
    }
}