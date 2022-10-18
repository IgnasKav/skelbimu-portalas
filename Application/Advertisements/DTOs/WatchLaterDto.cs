using System;

namespace Application.Advertisements.WatchLater;

public class WatchLaterDto
{
    public Guid UserId { get; set; }
    public Guid AdvertisementId { get; set; }
}