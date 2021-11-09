using Application.Categories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Domain;

namespace API.Tests.CategoryTests
{
    public class CategoryCreateTest
    {
        private readonly Mock<DataContext> mockContext;
        private readonly Mock<DbSet<Category>> mockDbSet;
        public CategoryCreateTest()
        {     
            var options = new DbContextOptionsBuilder<DataContext>().Options;
            mockContext = new(options);
            mockDbSet = new();
        }

        public static IEnumerable<object[]> createTestData => new List<object[]>
        {
            new object[] { "Electronics", null },
            new object[] { "Food", new Guid("0f8fad5b-d9cb-469f-a165-70867728950e") },
            new object[] { "Cars", new Guid("7c9e6679-7425-40de-944b-e07fc1f90ae7") },
        };

        [Theory]
        [MemberData(nameof(createTestData))]
        public async Task TestCategoryCreation(string name, Guid parentId)
        {
            var mockCreateHandler = new Create.Handler(mockContext.Object);
            var mockCommand = new Create.Command
            {
                Category = new Domain.Category
                {
                    Name = name,
                    ParentId = parentId
                }
            };

            mockContext.Setup(m => m.Categories).Returns(mockDbSet.Object);

            await mockCreateHandler.Handle(mockCommand, CancellationToken.None);

            mockDbSet.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory]
        [InlineData("Name")]
        [InlineData("Electronics")]
        [InlineData("Food")]
        public async Task TestCreatedCategoryFieldsCorrect(string name)
        {
            var mockCreateHandler = new Create.Handler(mockContext.Object);
            var mockCommand = new Create.Command
            {
                Category = new Category
                {
                    Name = name
                }
            };

            mockContext.Setup(m => m.Categories).Returns(mockDbSet.Object);

            await mockCreateHandler.Handle(mockCommand, CancellationToken.None);

            mockDbSet.Verify(m => m.Add(It.Is<Category>(c => c.Name == name)), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        }

        [Fact]
        public async Task TestCategoryCreationFailDueToBadData()
        {
            var mockCreateHandler = new Create.Handler(mockContext.Object);
            var mockCommand = new Create.Command
            {
                Category = new Category
                {
                    Name = null
                }
            };

            mockContext.Setup(m => m.Categories).Returns(mockDbSet.Object);

            var exception = await Assert.ThrowsAsync<Exception>(async () => await mockCreateHandler.Handle(mockCommand, CancellationToken.None));
            Assert.Equal("Category must have name", exception.Message);
        }
    }
}
