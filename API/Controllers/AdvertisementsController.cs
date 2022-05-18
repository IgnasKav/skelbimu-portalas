using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Advertisements;
using Application.Advertisements.WatchLater;
using Domain;
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
        
        [HttpGet("list/{id}")]
        public async Task<ActionResult<List<AdvertisementDto>>> GetAdvertisementsByCategory(Guid id)
        {
            return await Mediator.Send(new List.Query{});
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

        [HttpPut("{id}/watchLater")]
        public async Task<IActionResult> AddtoWatchLater(WatchLaterDto request)
        {
            return Ok(await Mediator.Send(new AddToWatchLater.Command{WatchLater = request}));
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
    }
}