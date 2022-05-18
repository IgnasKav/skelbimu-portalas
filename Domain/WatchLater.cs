using System;

namespace Domain
{
    public class WatchLater
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AdvertisementId { get; set; }

        public User User { get; set; }
        public Advertisement Advertisement { get; set; }
    }
}