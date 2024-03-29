using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Advertisements;
using Application.Advertisements.Images;
using Application.Advertisements.WatchLater;
using ElasticSearch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AdvertisementsController: BaseApiController
    {
        [HttpPost("search")]
        public async Task<ActionResult<List<AdvertisementDto>>> GetAdvertisements(ElasticSearchRequest request)
        {
            return await Mediator.Send(new List.Query{ElasticSearchRequest = request});
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<AdvertisementDto>> GetAdvertisement(Guid id)
        {
            return await Mediator.Send(new Details.Query{Id = id});
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateAdvertisement(AdvertisementDto advertisement)
        {
            return Ok(await Mediator.Send(new Create.Command{Advertisement = advertisement}));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvertisement(Guid id, AdvertisementDto advertisement)
        {
            advertisement.Id = id;
            return Ok(await Mediator.Send(new Edit.Command{Advertisement = advertisement}));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisement(Guid id)
        {
            return Ok(await Mediator.Send(new Delete.Command{Id = id}));
        }

        [HttpPut("{id}/watchLater")]
        public async Task<IActionResult> AddtoWatchLater(WatchLaterDto request)
        {
            return Ok(await Mediator.Send(new AddToWatchLater.Command{WatchLater = request}));
        }
        
        [HttpPost("watchLater")]
        public async Task<ActionResult<List<AdvertisementDto>>> GetWatchLater(ElasticSearchRequest request)
        {
            return await Mediator.Send(new WatchLaterList.Query{ElasticSearchRequest = request});
        }

        [HttpPost("updateImage")]
        public async Task<ActionResult<string>> UpdateImage([FromForm]AdvertisementImageUpdateRequest request)
        {
            return Ok(await Mediator.Send(new UpdateImage.Command{Request = request}));
        }
    }
}