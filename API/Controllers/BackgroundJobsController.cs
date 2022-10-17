using System;
using System.Threading.Tasks;
using API.DTOs;
using Application.BackgroundJobs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BackgroundJobsController: BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> EnqueueAdvertisementElasticJob(BackgroundJobRequest request)
    {
        await Mediator.Send(new AdvertisementBackgroundJob.Query{ ElasticBackgroundOperation = request.operation});
        return Ok();
    }
}