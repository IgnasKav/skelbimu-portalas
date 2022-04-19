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
            // handler = new Create.Handler(context.Object, es.Object);
        }

        [Fact]
        public async Task CanCreateAdvertisement()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
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

        [Fact]
        public async Task CantCreateAdvertisementWithoutTitle()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "",
                    Date = DateTime.Now,
                    Description = "test",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
                    City = "test",
                    Views = 0,
                    Price = 0  
                }
            };
            
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);

            var exception = await Assert.ThrowsAsync<Exception>( async () => await handler.Handle(command, new CancellationToken()));
            Assert.Equal("Advertisement must have title", exception.Message);
        }

        [Fact]
        public async Task CantCreateAdvertisementWithoutCategory()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    // CategoryId = new Guid(),
                    // State = AdvertisementState.Approved,
                    City = "test",
                    Views = 0,
                    Price = 0  
                }
            };
            
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            
            var exception = await Assert.ThrowsAsync<Exception>( async () => await handler.Handle(command, new CancellationToken()));
            Assert.Equal("Advertisement category does not exist", exception.Message);
        }
        
        [Fact]
        public async Task InitialViewsMustBeZero()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
                    City = "test",
                    Views = 2,
                    Price = 0  
                }
            };
            
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            
            var exception = await Assert.ThrowsAsync<Exception>( async () => await handler.Handle(command, new CancellationToken()));
            Assert.Equal("Initial views must be 0", exception.Message);
        }
        
        [Fact]
        public async Task DateMustBeToday()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now.AddDays(2),
                    Description = "test",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
                    City = "test",
                    Views = 0,
                    Price = 0  
                }
            };
            
            context.Setup(m => m.Advertisements).Returns(dbSet.Object);
            
            var exception = await Assert.ThrowsAsync<Exception>( async () => await handler.Handle(command, new CancellationToken()));
            Assert.Equal("Date must be today", exception.Message);
        }

        [Fact]
        public async Task CanCreateAdvertisementWithoutDescription()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
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

        [Fact]
        public async Task CanCreateAdvertisementWithoutState()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
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

        [Fact]
        public async Task CanCreateAdvertisementWithoutCity()
        {
            var command = new Create.Command
            {
                Advertisement = new AdvertisementDto
                {
                    Title = "test",
                    Date = DateTime.Now,
                    Description = "test",
                    // CategoryId = new Guid(),
                    // Category = new Category
                    // {
                    //     Name = "deafault"
                    // },
                    // State = AdvertisementState.Approved,
                    City = "",
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