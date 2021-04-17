using System;
using System.Collections.Generic;

namespace Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public Category Parent { get; set; }

        public ICollection<Advertisement> Advertisements { get; set; }
        public ICollection<Category> Children { get; set; }
    }
}