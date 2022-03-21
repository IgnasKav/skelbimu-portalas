using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Advertisement>().Property(a => a.State)
                .HasConversion(
                    v => v.ToString(),
                    v => (AdvertisementState)Enum.Parse(typeof(AdvertisementState), v));
            
            base.OnModelCreating(builder);
        }
    }
}