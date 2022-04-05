using System;

namespace Domain
{
    public class Advertisement
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public AdvertisementState State { get; set; }
        public string City { get; set; }
        public int Views { get; set; }
        public decimal Price { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public enum AdvertisementState
    {
        New,
        Approved,
        Rejected
    }
}