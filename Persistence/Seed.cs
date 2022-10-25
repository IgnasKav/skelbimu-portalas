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
        public static async Task SeedData(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await CreateUsers(userManager);
            await CreateUserRoles(userManager, roleManager);
            await CreateCategories(context);
            await CreateAdvertisements(context, userManager);
        }

        private static async Task CreateUsers(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<User>
                {
                    new User {DisplayName = "Ignas", UserName = "IgnasKav", Email = "ignas.kavaliauskas5@gmail.com"},
                    new User {DisplayName = "Rokas", UserName = "rok45", Email = "rok45@local.com"},
                    new User {DisplayName = "Matas", UserName = "Mat42Bo55", Email = "mat42boss@gmail.com"},
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }
            }
        }

        private static async Task CreateUserRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.Roles.Any())
            {
                return;
            }

            string[] userRoles = {"Admin", "Support", "User"};
            var users = userManager.Users.ToList();

            foreach (var role in userRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            foreach (var user in users)
            {
                switch (user.Email)
                {
                    case "ignas.kavaliauskas5@gmail.com":
                        await userManager.AddToRoleAsync(user, "Admin");
                        break;
                    case "rok45@local.com": 
                        await userManager.AddToRoleAsync(user, "Support");
                        break;
                    default:
                        await userManager.AddToRoleAsync(user, "User");
                        break;
                }
            }
        }

        private static async Task CreateCategories(DataContext context)
        {
            if (!context.Categories.Any())
            {
                var phones = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Phones",
                };

                var applePhones = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Apple",
                    ParentId = phones.Id
                };

                var googlePhones = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Google",
                    ParentId = phones.Id
                };
                
                var iphone11 = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Iphone 11",
                    ParentId = applePhones.Id
                };

                var iphone12 = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Iphone 12",
                    ParentId = applePhones.Id
                };

                var pixel = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Pixel",
                    ParentId = googlePhones.Id
                };

                var computers = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Computers"
                };

                var appleComputers = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Apple",
                    ParentId = computers.Id
                };

                var macbook = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "MacBook",
                    ParentId = appleComputers.Id
                };

                var categories = new List<Category>
                    {phones, applePhones, googlePhones, iphone11, iphone12, pixel, computers, appleComputers, macbook};


                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateAdvertisements(DataContext context, UserManager<User> userManager)
        {
            if (context.Advertisements.Any()) return;

            var advertisementOwner = await userManager.FindByEmailAsync("ignas.kavaliauskas5@gmail.com");

            var categoryList = await context.Categories.ToListAsync();

            var iphone11Category = categoryList.FirstOrDefault(c => c.Name == "Iphone 11");
            var iphone12Category = categoryList.FirstOrDefault(c => c.Name == "Iphone 12");
            var pixelCategory = categoryList.FirstOrDefault(c => c.Name == "Pixel");
            var macBookCategory = categoryList.FirstOrDefault(c => c.Name == "MacBook");
            

            var advertisements = new List<Advertisement>
            {
                new Advertisement
                {
                    Id = Guid.NewGuid(),
                    Title = "Iphone 12",
                    Date = DateTime.Now.AddMonths(-2).ToUniversalTime(),
                    Description = "Selling used Apple Iphone 12.",
                    CategoryId = iphone12Category.Id,
                    OwnerId = new Guid(advertisementOwner.Id),
                    State = AdvertisementState.Approved,
                    City = "Kaunas",
                    Views = 0,
                    Price = 20,
                    
                },
                new Advertisement
                {
                    Id = Guid.NewGuid(),
                    Title = "Pixel",
                    Date = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                    Description = "Used Pixel phone.",
                    CategoryId = pixelCategory.Id,
                    OwnerId = new Guid(advertisementOwner.Id),
                    State = AdvertisementState.Approved,
                    City = "Vilnius",
                    Views = 2,
                    Price = 30
                },
                new Advertisement
                {
                    Id = Guid.NewGuid(),
                    Title = "Apple Macbook 2019",
                    Date = DateTime.Now.AddMonths(1).ToUniversalTime(),
                    Description = "Almost ideal 2019 macbook pro 16 inch",
                    CategoryId = macBookCategory.Id,
                    OwnerId = new Guid(advertisementOwner.Id),
                    State = AdvertisementState.Approved,
                    City = "Vilnius",
                    Views = 8,
                    Price = 15
                },
                new Advertisement
                {
                    Id = Guid.NewGuid(),
                    Title = "Iphone 11",
                    Date = DateTime.Now.AddMonths(2).ToUniversalTime(),
                    Description = "Selling used iphone 11, great condition",
                    CategoryId = iphone11Category.Id,
                    OwnerId = new Guid(advertisementOwner.Id),
                    State = AdvertisementState.Approved,
                    City = "Utena",
                    Views = 4,
                    Price = 500
                },
                new Advertisement
                {
                    Id = Guid.NewGuid(),
                    Title = "MacBook pro m1 max 2021",
                    Date = DateTime.Now.AddMonths(3).ToUniversalTime(),
                    Description = "Screen size: 16 inch, Ram: 32GB, SSD: 1TB",
                    CategoryId = macBookCategory.Id,
                    OwnerId = new Guid(advertisementOwner.Id),
                    State = AdvertisementState.Approved,
                    City = "Kaunas",
                    Views = 7,
                    Price = 3250
                }
            };

            var advertisementsImages = new List<AdvertisementImage>();

            for (var i = 0; i < advertisements.Count; i++)
            {
                var advertisement = advertisements[i];
                var image = new AdvertisementImage
                {
                    AdvertisementId = advertisement.Id,
                    UserId = new Guid(advertisementOwner.Id),
                    ImagePath = $"images/default/{i}.jpeg"
                };
                
                advertisementsImages.Add(image);
            }

            await context.Advertisements.AddRangeAsync(advertisements);
            await context.AdvertisementImage.AddRangeAsync(advertisementsImages);
            await context.SaveChangesAsync();
        }
    }
}