using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using ElasticSearch;
using ElasticSearch.Indexing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Application.Advertisements.Images;

public class UpdateImage
{
    public class Command : IRequest<string>
    {
        public AdvertisementImageUpdateRequest Request { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, string>
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IElasticSearchService _es;
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public Handler(IConfiguration config, IHttpContextAccessor httpContextAccessor, IElasticSearchService es, DataContext context, UserManager<User> userManager, IWebHostEnvironment env)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _es = es;
            _context = context;
            _userManager = userManager;
            _env = env;
        }
    
        public async Task<string> Handle(Command command, CancellationToken cancellationToken)
        {
            var currentUserClaim = _httpContextAccessor.HttpContext?.User;
            if (currentUserClaim == null)
            {
                throw new Exception("Current user not found");
            }

            var currentUserId = new Guid(_userManager.GetUserId(currentUserClaim));
            var advertisementId = new Guid(command.Request.AdvertisementId);
            var image = command.Request.Image;
            var imageName = $"{image.FileName.Split('.')[0]}.jpg";
            var rootPath = _env.ContentRootPath;
            var imageUrl = $"images/{advertisementId.ToString()}/{imageName}";
            var advertisementPath = Path.Combine(rootPath, imageUrl);

            CheckIfAdvertisementFolderExists(advertisementId);

            await using FileStream fs = File.Create(advertisementPath);
            await image.CopyToAsync(fs, cancellationToken);
            await fs.FlushAsync(cancellationToken);
            
            var imageEntities = await _context.AdvertisementImage.Where(i =>
                i.AdvertisementId == advertisementId).ToListAsync(cancellationToken);

            var imageEntity = new AdvertisementImage();
            
            if (imageEntities.Count == 0)
            {
                imageEntity.Id = Guid.NewGuid();
                imageEntity.AdvertisementId = advertisementId;
                imageEntity.UserId = currentUserId;
                imageEntity.ImagePath = imageUrl;
                _context.AdvertisementImage.Add(imageEntity);
            }
            else
            {
                imageEntity = imageEntities[0];
                imageEntity.ImagePath = imageUrl;
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            var advertisement = await _context.Advertisements.FindAsync(advertisementId);
            if (advertisement != null)
            {
                await _es.Reindex(IndexDefinition.Advertisement, advertisementId);
            }

            return imageUrl;
        }
        
        private void CheckIfAdvertisementFolderExists(Guid advertisementId)
        {
            var rootPath = _env.ContentRootPath;
            var advertisementFolderPath = Path.Combine(rootPath, $"images/{advertisementId.ToString()}");

            if (!Directory.Exists(advertisementFolderPath))
            {
                Directory.CreateDirectory(advertisementFolderPath);
            }
        }
    }
}