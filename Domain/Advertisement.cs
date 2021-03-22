using System;

namespace Domain
{
    public class Advertisement
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category {get; set;}
        public string State { get; set; }
        public string City { get; set; }
        public int Views { get; set; }

        public decimal Price { get; set; }

    }
}