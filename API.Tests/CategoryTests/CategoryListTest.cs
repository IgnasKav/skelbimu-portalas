using Application.Categories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Application.Categories.List;

namespace API.Tests.CategoryTests
{
    public class CategoryListTest
    {
        private readonly Mock<DataContext> mockContext;
        private Mock<DbSet<Category>> mockDbSet;

        public CategoryListTest()
        {
            var options = new DbContextOptionsBuilder<DataContext>().Options;
            mockContext = new(options);
            mockDbSet = new();
        }

        [Fact]
        public async Task TestCategoryList()
        {
            List<Category> list = new ()
            {
                new Category() { Name = "First" },
                new Category() { Name = "Second" },
                new Category() { Name = "Third" }
            };

            mockDbSet = list.AsQueryable().BuildMockDbSet();
            mockContext.Setup(m => m.Categories).Returns(mockDbSet.Object);

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<Category, CategoryDto>(It.IsAny<Category>()))
                .Returns(new CategoryDto());

            var listHandler = new List.Handler(mockContext.Object, mapper.Object);

            var result = listHandler.Handle(new Query(), CancellationToken.None);

            mockContext.Verify(c => c.Categories, Times.Once);
            Assert.Equal(list.Count, mockContext.Object.Categories.Count());
            //mockDbSet.Verify(c => c.ProjectTo<CategoryDto>(mapper.Object.ConfigurationProvider), Times.Once);

        }
    }
}
