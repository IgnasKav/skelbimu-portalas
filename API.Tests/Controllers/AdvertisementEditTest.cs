using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Advertisements;
using AutoMapper;
using Domain;
using ElasticSearch;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using Xunit;

namespace API.Tests.Controllers
{
    public class AdvertisementEditTest
    {
        private readonly Mock<DbSet<Advertisement>> dbSet;
        private readonly Mock<DataContext> context;
        private readonly Mock<IElasticSearchService> es;
        private readonly Mock<IMapper> mapper;
        private readonly Edit.Handler handler;

        public AdvertisementEditTest()
        {
            dbSet = new Mock<DbSet<Advertisement>>();
            var options = new DbContextOptionsBuilder<DataContext>().Options;
            context = new Mock<DataContext>(options);
            es = new Mock<IElasticSearchService>();
            mapper = new Mock<IMapper>();
            handler = new Edit.Handler(context.Object, mapper.Object, es.Object);
        }
        
        [Fact]
        public async Task CantEditOnlyExistingAdvertisements()
        {
            var command = new Edit.Command
            {
                Advertisement = new Advertisement
                {
                    Id = new Guid(),
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    CategoryId = new Guid(),
                    Category = new Category
                    {
                        Name = "deafault"
                    },
                    State = "test",
                    City = "test",
                    Views = 0,
                    Price = 0  
                }
            };
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            context.Setup(m => m.Advertisements.FindAsync()).ReturnsAsync((Advertisement)null);
            
            var exception = await Assert.ThrowsAsync<Exception>( async () => await handler.Handle(command, new CancellationToken()));
            Assert.Equal("Advertisement does not exist", exception.Message);
        }
        
        [Fact]
        public async Task CantDecreaseViews()
        {
            var savedAdvertisement = new Advertisement
            {
                Id = new Guid(),
                Title = "test",
                Date = DateTime.Now,
                Description = "test",
                CategoryId = new Guid(),
                Category = new Category
                {
                    Name = "deafault"
                },
                State = "test",
                City = "test",
                Views = 2,
                Price = 0
            };
            
            var command = new Edit.Command
            {
                Advertisement = new Advertisement
                {
                    Id = new Guid(),
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    CategoryId = new Guid(),
                    Category = new Category
                    {
                        Name = "deafault"
                    },
                    State = "test",
                    City = "test",
                    Views = 1,
                    Price = 0  
                }
            };
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            dbSet.Setup(m => m.FindAsync(It.IsAny<Guid>())).ReturnsAsync(savedAdvertisement);
            
            var exception = await Assert.ThrowsAsync<Exception>( async () => await handler.Handle(command, new CancellationToken()));
            Assert.Equal("Cant decrease advertisement views", exception.Message);
        }

        [Fact]
        public async Task CanEditValidAdvertisements()
        {
            var savedAdvertisement = new Advertisement
            {
                Id = new Guid(),
                Title = "test",
                Date = DateTime.Now,
                Description = "test",
                CategoryId = new Guid(),
                Category = new Category
                {
                    Name = "default"
                },
                State = "test",
                City = "test",
                Views = 2,
                Price = 0
            };

            var command = new Edit.Command
            {
                Advertisement = new Advertisement
                {
                    Id = new Guid(),
                    Title = "test edited",
                    Date = DateTime.Now,
                    Description = "test",
                    CategoryId = new Guid(),
                    Category = new Category
                    {
                        Name = "default"
                    },
                    State = "test",
                    City = "test",
                    Views = 2,
                    Price = 0
                }
            };
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            dbSet.Setup(m => m.FindAsync(It.IsAny<Guid>())).ReturnsAsync(savedAdvertisement);

            await handler.Handle(command, new CancellationToken());
            context.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

    }
}