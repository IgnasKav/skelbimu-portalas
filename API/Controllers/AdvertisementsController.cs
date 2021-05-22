using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Advertisements;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AdvertisementsController: BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<AdvertisementDto>>> GetAdvertisements()
        {
            return await Mediator.Send(new List.Query());
        }
        [HttpGet("list/{id}")]
        public async Task<ActionResult<List<AdvertisementDto>>> GetAdvertisementsByCategory(Guid id)
        {
            return await Mediator.Send(new List.Query{Id = id});
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AdvertisementDto>> GetAdvertisement(Guid id)
        {
            return await Mediator.Send(new Details.Query{Id = id});
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateAdvertisement(Advertisement advertisement)
        {
            return Ok(await Mediator.Send(new Create.Command{Advertisement = advertisement}));
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvertisement(Guid id, Advertisement advertisement)
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