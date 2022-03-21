using System;

namespace Application.Categories
{
    public class CategoryDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public Guid parentId { get; set; }

        public CategoryDto[] children { get; set; }
    }
}