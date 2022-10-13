using System;
using Microsoft.AspNetCore.Http;

namespace Application.Advertisements.Images;

public class AdvertisementImageUpdateRequest
{
    public string AdvertisementId { get; set; }
    public IFormFile Image { get; set; }
}