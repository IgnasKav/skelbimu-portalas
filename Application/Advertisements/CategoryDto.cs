using System;

namespace Application.Advertisements
{
    public class CategoryDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public Guid parentId { get; set; }
    }
}