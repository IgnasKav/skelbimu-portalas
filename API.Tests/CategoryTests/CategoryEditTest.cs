using Application.Categories;
using Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests.CategoryTests
{
    public class CategoryEditTest
    {
        private readonly Mock<DataContext> mockContext;
        private readonly Mock<DbSet<Category>> mockDbSet;

        public CategoryEditTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>().Options;
            mockContext = new(options);
            mockDbSet = new();
        }

        [Fact]
        public async Task TestCategoryEdit()
        {
            var category = new Category { Name = "AAA" };
            
            mockContext.Setup(m => m.Categories).Returns(mockDbSet.Object);
            mockDbSet.Setup(c => c.FindAsync(category.Id).Result).Returns(category);

            var editHandler = new Edit.Handler(mockContext.Object);
            var command = new Edit.Command
            {
                Category = category
            };

            await editHandler.Handle(command, CancellationToken.None);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TestCategoryEditFailDueToBadData()
        {
            var mockEditHandler = new Edit.Handler(mockContext.Object);
            var mockCommand = new Edit.Command
            {
                Category = new Category
                {
                    Name = null

                }
            };

            mockContext.Setup(m => m.Categories).Returns(mockDbSet.Object);

            var exception = await Assert.ThrowsAsync<Exception>(async () => await mockEditHandler.Handle(mockCommand, CancellationToken.None));
            Assert.Equal("Category must have a name", exception.Message);
        }
    }
}
