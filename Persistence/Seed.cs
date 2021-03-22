using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Advertisements.Any()) return;
            
            var advertisements = new List<Advertisement>
            {
                new Advertisement
                {
                    Title = "Skelbimas 1",
                    Date = DateTime.Now.AddMonths(-2),
                    Description = "naujas telefonas",
                    Category = "Mobilieji telefonai",
                    State = "Patvirtintas",
                    City = "Kaunas",
                    Views = 0,
                    Price = 20
                },
                new Advertisement
                {
                    Title = "Skelbimas 2",
                    Date = DateTime.Now.AddMonths(-1),
                    Description = "naudotas arbatinukas",
                    Category = "Buitis",
                    State = "Patvirtintas",
                    City = "Vilnius",
                    Views = 2,
                    Price = 30
                },
                new Advertisement
                {
                    Title = "Skelbimas 3",
                    Date = DateTime.Now.AddMonths(1),
                    Description = "naudotas siurblys",
                    Category = "Buitis",
                    State = "Patvirtintas",
                    City = "Vilnius",
                    Views = 8,
                    Price = 15
                },
                new Advertisement
                {
                    Title = "Skelbimas 4",
                    Date = DateTime.Now.AddMonths(2),
                    Description = "zaidimu kompiuteris",
                    Category = "Kompiuteriai",
                    State = "Patvirtintas",
                    City = "Utena",
                    Views = 4,
                    Price = 500
                },
                new Advertisement
                {
                    Title = "Skelbimas 5",
                    Date = DateTime.Now.AddMonths(3),
                    Description = "Klaviatura",
                    Category = "Elektronika",
                    State = "Patvirtintas",
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