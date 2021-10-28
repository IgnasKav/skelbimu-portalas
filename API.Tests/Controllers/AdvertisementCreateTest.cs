using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Advertisements;
using Domain;
using ElasticSearch;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using Xunit;

namespace API.Tests.Controllers
{
    public class AdvertisementCreateTest
    {
        private readonly Mock<DbSet<Advertisement>> dbSet;
        private readonly Mock<DataContext> context;
        private readonly Mock<IElasticSearchService> es;
        private readonly Create.Handler handler;

        public AdvertisementCreateTest()
        {
            dbSet = new Mock<DbSet<Advertisement>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;
            context = new Mock<DataContext>(options);
            es = new Mock<IElasticSearchService>();
            handler = new Create.Handler(context.Object, es.Object);
        }

        [Fact]
        public async Task CantCreateAdvertisement()
        {
            var command = new Create.Command
            {
                Advertisement = new Advertisement
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    CategoryId = new Guid(),
                    State = "test",
                    City = "test",
                    Views = 0,
                    Price = 0  
                }
            };
            
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            
            await handler.Handle(command, new CancellationToken());
            
            dbSet.Verify(m => m.Add(It.IsAny<Advertisement>()), Times.Once());
            context.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}