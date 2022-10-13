using System;

namespace Domain;

public class AdvertisementImage
{
    public Guid Id { get; set; }
    public Guid AdvertisementId { get; set; }
    public Guid UserId { get; set; }
    public string ImagePath { get; set; }
}