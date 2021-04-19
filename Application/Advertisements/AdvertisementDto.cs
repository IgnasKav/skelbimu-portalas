using System;
using Application.Categories;
using Domain;

namespace Application.Advertisements
{
    public class AdvertisementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public CategoryDto Category { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int Views { get; set; }
        public decimal Price { get; set; }
    }
}